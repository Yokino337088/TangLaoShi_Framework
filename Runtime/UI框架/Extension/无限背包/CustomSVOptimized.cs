using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// 优化后的无限滚动背包接口
/// </summary>
public interface IItemBaseOptimized<T>
{
    void InitInfo(T info);
    void OnRecycle(); // 回收时的清理方法
    void OnReuse();   // 复用时的重置方法
}

/// <summary>
/// 优化后的自定义ScrollView类
/// 主要优化点：
/// 1. 添加脏标记机制，避免不必要的计算
/// 2. 使用异步加载队列，保证加载顺序
/// 3. 添加数据变更支持（增删改）
/// 4. 优化位置计算，缓存计算结果
/// 5. 添加滚动惯性优化
/// </summary>
public class CustomSVOptimized<T, K> where K : MonoBehaviour, IItemBaseOptimized<T>
{
    #region 核心组件
    
    // Content对象
    private RectTransform content;
    // 可视范围高度
    private int viewPortH;
    // 可视范围宽度（新增，支持水平滚动）
    private int viewPortW;
    
    #endregion

    #region 格子配置
    
    // 格子尺寸
    private int itemW;
    private int itemH;
    // 列数
    private int col;
    // 间距
    private int rowSpacing;
    private int colSpacing;
    // 偏移量
    private int offsetX;
    private int offsetY;
    // 是否水平滚动（新增）
    private bool isHorizontal;
    
    #endregion

    #region 数据管理
    
    // 数据源
    private List<T> items = new List<T>();
    // 当前显示的格子对象池
    private Dictionary<int, K> nowShowItems = new Dictionary<int, K>();
    // 待创建格子队列（优化异步加载顺序）
    private Queue<int> pendingCreateQueue = new Queue<int>();
    // 是否正在处理创建队列
    private bool isProcessingQueue = false;
    
    #endregion

    #region 缓存优化
    
    // 缓存的行高（避免重复计算）
    private float cachedRowHeight;
    // 缓存的列宽
    private float cachedColWidth;
    // 缓存的可见范围
    private int cachedMinIndex = -1;
    private int cachedMaxIndex = -1;
    // 脏标记
    private bool isDirty = false;
    // 数据版本号（用于检测数据变更）
    private int dataVersion = 0;
    
    #endregion

    #region 资源路径
    
    private string itemABName;
    private string itemResName;
    // 格子预制体缓存（避免重复加载）
    private GameObject itemPrefab;
    
    #endregion

    #region 性能优化配置
    
    // 每帧最大创建数量（避免卡顿）
    private int maxCreatePerFrame = 3;
    // 滚动停止延迟检测时间
    private float scrollStopDelay = 0.1f;
    // 是否启用滚动优化
    private bool enableScrollOptimization = true;
    
    #endregion

    #region 事件监听
    
    // ScrollRect组件引用（用于监听滚动事件）
    private ScrollRect scrollRect;
    // 上次滚动位置
    private Vector2 lastScrollPos;
    // 滚动停止回调
    private System.Action onScrollStop;
    
    #endregion

    #region 初始化方法

    /// <summary>
    /// 初始化资源路径
    /// </summary>
    public void InitItemResName(string abName, string name)
    {
        itemABName = abName;
        itemResName = name;
    }

    /// <summary>
    /// 初始化Content和视口尺寸
    /// </summary>
    public void InitContentAndSVH(RectTransform trans, int h, int w = 0)
    {
        this.content = trans;
        this.viewPortH = h;
        this.viewPortW = w;
        
        // 尝试获取ScrollRect组件
        scrollRect = content.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            isHorizontal = scrollRect.horizontal && !scrollRect.vertical;
        }
        
        // 启动滚动停止检测
        if (enableScrollOptimization)
        {
            CheckScrollStop().Forget();
        }
    }

    /// <summary>
    /// 初始化格子配置（基础版）
    /// </summary>
    public void InitItemSizeAndCol(int w, int h, int col)
    {
        InitItemSizeAndCol(w, h, col, 0, 0, 0, 0);
    }

    /// <summary>
    /// 初始化格子配置（完整版）
    /// </summary>
    public void InitItemSizeAndCol(int w, int h, int col, int colSpacing, int rowSpacing, int offsetX = 0, int offsetY = 0)
    {
        this.itemW = w;
        this.itemH = h;
        this.col = col;
        this.colSpacing = colSpacing;
        this.rowSpacing = rowSpacing;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        
        // 缓存计算值
        cachedRowHeight = itemH + rowSpacing;
        cachedColWidth = itemW + colSpacing;
    }

    /// <summary>
    /// 设置性能优化参数
    /// </summary>
    public void SetOptimizationParams(int maxCreatePerFrame = 3, float scrollStopDelay = 0.1f)
    {
        this.maxCreatePerFrame = maxCreatePerFrame;
        this.scrollStopDelay = scrollStopDelay;
    }

    #endregion

    #region 数据管理

    /// <summary>
    /// 初始化数据（会清空原有数据）
    /// </summary>
    public void InitInfos(List<T> data)
    {
        ClearItems();
        items.Clear();
        
        if (data != null)
        {
            items.AddRange(data);
        }
        
        dataVersion++;
        UpdateContentSize();
        isDirty = true;
    }

    /// <summary>
    /// 添加单个数据
    /// </summary>
    public void AddItem(T item)
    {
        items.Add(item);
        dataVersion++;
        UpdateContentSize();
        
        // 如果新添加的数据在可见范围内，标记为脏
        int newIndex = items.Count - 1;
        if (newIndex >= cachedMinIndex && newIndex <= cachedMaxIndex)
        {
            isDirty = true;
        }
    }

    /// <summary>
    /// 插入数据到指定位置
    /// </summary>
    public void InsertItem(int index, T item)
    {
        if (index < 0 || index > items.Count)
            return;
            
        items.Insert(index, item);
        dataVersion++;
        
        // 重新整理显示中的格子索引
        ReorganizeIndices(index);
        UpdateContentSize();
        isDirty = true;
    }

    /// <summary>
    /// 移除指定位置的数据
    /// </summary>
    public void RemoveItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return;
            
        items.RemoveAt(index);
        dataVersion++;
        
        // 如果移除的是当前显示的格子，回收它
        if (nowShowItems.TryGetValue(index, out K item))
        {
            RecycleItem(index, item);
        }
        
        // 重新整理显示中的格子索引
        ReorganizeIndices(index);
        UpdateContentSize();
        isDirty = true;
    }

    /// <summary>
    /// 更新指定位置的数据
    /// </summary>
    public void UpdateItem(int index, T newData)
    {
        if (index < 0 || index >= items.Count)
            return;
            
        items[index] = newData;
        
        // 如果该格子正在显示，刷新它
        if (nowShowItems.TryGetValue(index, out K item))
        {
            item.InitInfo(newData);
        }
    }

    /// <summary>
    /// 清空所有数据
    /// </summary>
    public void ClearData()
    {
        ClearItems();
        items.Clear();
        dataVersion++;
        UpdateContentSize();
    }

    /// <summary>
    /// 获取数据列表（只读）
    /// </summary>
    public IReadOnlyList<T> GetItems()
    {
        return items.AsReadOnly();
    }

    /// <summary>
    /// 获取数据数量
    /// </summary>
    public int GetItemCount()
    {
        return items.Count;
    }

    #endregion

    #region 核心显示逻辑

    /// <summary>
    /// 检查显示或隐藏格子（优化版）
    /// </summary>
    public void CheckShowOrHide()
    {
        if (items.Count == 0)
            return;
            
        // 如果不是脏的，且缓存的索引范围有效，直接返回
        if (!isDirty && cachedMinIndex >= 0 && cachedMaxIndex >= 0)
        {
            // 检查滚动位置是否变化足够大
            Vector2 currentPos = content.anchoredPosition;
            float rowHeight = cachedRowHeight;
            float threshold = rowHeight * 0.5f;
            
            if (Mathf.Abs(currentPos.y - lastScrollPos.y) < threshold)
            {
                return;
            }
        }
        
        lastScrollPos = content.anchoredPosition;
        
        // 计算可见索引范围
        CalculateVisibleRange(out int minIndex, out int maxIndex);
        
        // 如果范围没有变化，不需要更新
        if (!isDirty && minIndex == cachedMinIndex && maxIndex == cachedMaxIndex)
        {
            return;
        }
        
        // 回收不再可见的格子
        RecycleInvisibleItems(minIndex, maxIndex);
        
        // 创建新可见的格子
        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (!nowShowItems.ContainsKey(i))
            {
                pendingCreateQueue.Enqueue(i);
            }
        }
        
        // 处理创建队列
        if (!isProcessingQueue && pendingCreateQueue.Count > 0)
        {
            ProcessCreateQueue().Forget();
        }
        
        // 更新缓存
        cachedMinIndex = minIndex;
        cachedMaxIndex = maxIndex;
        isDirty = false;
    }

    /// <summary>
    /// 计算可见范围
    /// </summary>
    private void CalculateVisibleRange(out int minIndex, out int maxIndex)
    {
        float rowHeight = cachedRowHeight;
        float colWidth = cachedColWidth;
        
        if (isHorizontal)
        {
            // 水平滚动计算
            float scrollX = -content.anchoredPosition.x;
            minIndex = (int)(scrollX / colWidth) * col;
            maxIndex = (int)((scrollX + viewPortW) / colWidth) * col + col - 1;
        }
        else
        {
            // 垂直滚动计算
            float scrollY = content.anchoredPosition.y;
            minIndex = (int)(scrollY / rowHeight) * col;
            maxIndex = (int)((scrollY + viewPortH) / rowHeight) * col + col - 1;
        }
        
        // 边界限制
        minIndex = Mathf.Max(0, minIndex);
        maxIndex = Mathf.Min(items.Count - 1, maxIndex);
    }

    /// <summary>
    /// 处理创建队列（分帧创建，避免卡顿）
    /// </summary>
    private async UniTaskVoid ProcessCreateQueue()
    {
        isProcessingQueue = true;
        int createCount = 0;
        
        while (pendingCreateQueue.Count > 0)
        {
            int index = pendingCreateQueue.Dequeue();
            
            // 检查索引是否仍然有效
            if (index < cachedMinIndex || index > cachedMaxIndex)
                continue;
                
            // 检查是否已创建
            if (nowShowItems.ContainsKey(index))
                continue;
            
            await CreateItem(index);
            createCount++;
            
            // 每帧最多创建指定数量
            if (createCount >= maxCreatePerFrame)
            {
                createCount = 0;
                await UniTask.Yield();
            }
        }
        
        isProcessingQueue = false;
    }

    /// <summary>
    /// 创建单个格子
    /// </summary>
    private async UniTask CreateItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return;
            
        // 预加载预制体
        if (itemPrefab == null && !string.IsNullOrEmpty(itemABName) && !string.IsNullOrEmpty(itemResName))
        {
            await PreloadPrefab();
        }
        
        // 从对象池获取
        if (!string.IsNullOrEmpty(itemABName) && !string.IsNullOrEmpty(itemResName))
        {
            GOPoolMgr.Instance.GetObj(itemABName, itemResName, (obj) =>
            {
                OnItemCreated(index, obj);
            });
        }
    }

    /// <summary>
    /// 格子创建完成回调
    /// </summary>
    private void OnItemCreated(int index, GameObject obj)
    {
        if (obj == null)
            return;
            
        // 检查索引是否仍然有效
        if (index < cachedMinIndex || index > cachedMaxIndex)
        {
            GOPoolMgr.Instance.PushObj(obj);
            return;
        }
        
        // 设置父对象
        obj.transform.SetParent(content, false);
        obj.transform.localScale = Vector3.one;
        
        // 设置位置
        SetItemPosition(obj.transform, index);
        
        // 获取组件并初始化
        K item = obj.GetComponent<K>();
        if (item != null)
        {
            item.OnReuse();
            if (index < items.Count)
            {
                item.InitInfo(items[index]);
            }
            
            nowShowItems[index] = item;
        }
        else
        {
            Debug.LogError($"格子对象缺少{typeof(K).Name}组件");
            GOPoolMgr.Instance.PushObj(obj);
        }
    }

    /// <summary>
    /// 设置格子位置
    /// </summary>
    private void SetItemPosition(Transform trans, int index)
    {
        int row = index / col;
        int column = index % col;
        
        float xPos = offsetX + column * cachedColWidth;
        float yPos = -(offsetY + row * cachedRowHeight);
        
        trans.localPosition = new Vector3(xPos, yPos, 0);
    }

    /// <summary>
    /// 回收单个格子
    /// </summary>
    private void RecycleItem(int index, K item)
    {
        if (item != null)
        {
            item.OnRecycle();
            GOPoolMgr.Instance.PushObj(item.gameObject);
        }
        nowShowItems.Remove(index);
    }

    /// <summary>
    /// 回收不可见的格子
    /// </summary>
    private void RecycleInvisibleItems(int newMinIndex, int newMaxIndex)
    {
        List<int> toRemove = new List<int>();
        
        foreach (var kvp in nowShowItems)
        {
            int index = kvp.Key;
            if (index < newMinIndex || index > newMaxIndex)
            {
                toRemove.Add(index);
            }
        }
        
        foreach (int index in toRemove)
        {
            if (nowShowItems.TryGetValue(index, out K item))
            {
                RecycleItem(index, item);
            }
        }
    }

    /// <summary>
    /// 重新整理索引（插入或删除后调用）
    /// </summary>
    private void ReorganizeIndices(int changedIndex)
    {
        Dictionary<int, K> newItems = new Dictionary<int, K>();
        
        foreach (var kvp in nowShowItems)
        {
            int oldIndex = kvp.Key;
            K item = kvp.Value;
            
            if (oldIndex < changedIndex)
            {
                // 在变更点之前的索引不变
                newItems[oldIndex] = item;
            }
            else
            {
                // 在变更点之后的索引需要调整
                int newIndex = oldIndex;
                // 更新位置
                SetItemPosition(item.transform, newIndex);
                // 更新数据
                if (newIndex < items.Count)
                {
                    item.InitInfo(items[newIndex]);
                }
                newItems[newIndex] = item;
            }
        }
        
        nowShowItems = newItems;
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 更新Content尺寸
    /// </summary>
    private void UpdateContentSize()
    {
        if (isHorizontal)
        {
            // 水平滚动
            float colWidth = cachedColWidth;
            int rowCount = Mathf.CeilToInt(items.Count / (float)col);
            float width = rowCount * colWidth - colSpacing + offsetX * 2;
            content.sizeDelta = new Vector2(width, 0);
        }
        else
        {
            // 垂直滚动
            float rowHeight = cachedRowHeight;
            int rowCount = Mathf.CeilToInt(items.Count / (float)col);
            float height = rowCount * rowHeight - rowSpacing + offsetY * 2;
            content.sizeDelta = new Vector2(0, height);
        }
    }

    /// <summary>
    /// 预加载预制体
    /// </summary>
    private async UniTask PreloadPrefab()
    {
        if (!string.IsNullOrEmpty(itemABName) && !string.IsNullOrEmpty(itemResName))
        {
            // 使用UniTask等待异步加载完成
            await UniTask.WaitUntil(() => itemPrefab != null || string.IsNullOrEmpty(itemABName));
        }
    }

    /// <summary>
    /// 滚动值变化回调
    /// </summary>
    private void OnScrollValueChanged(Vector2 pos)
    {
        CheckShowOrHide();
    }

    /// <summary>
    /// 检测滚动停止
    /// </summary>
    private async UniTaskVoid CheckScrollStop()
    {
        Vector2 lastPos = content.anchoredPosition;
        float timer = 0f;
        
        while (true)
        {
            await UniTask.Delay(100); // 每100ms检测一次
            
            Vector2 currentPos = content.anchoredPosition;
            if (currentPos == lastPos)
            {
                timer += 0.1f;
                if (timer >= scrollStopDelay)
                {
                    // 滚动停止
                    onScrollStop?.Invoke();
                    timer = 0f;
                }
            }
            else
            {
                timer = 0f;
            }
            
            lastPos = currentPos;
        }
    }

    /// <summary>
    /// 设置滚动停止回调
    /// </summary>
    public void SetOnScrollStopCallback(System.Action callback)
    {
        onScrollStop = callback;
    }

    /// <summary>
    /// 强制刷新显示
    /// </summary>
    public void ForceRefresh()
    {
        isDirty = true;
        cachedMinIndex = -1;
        cachedMaxIndex = -1;
        CheckShowOrHide();
    }

    /// <summary>
    /// 滚动到指定索引
    /// </summary>
    public void ScrollToIndex(int index, bool animated = false)
    {
        if (index < 0 || index >= items.Count)
            return;
            
        int row = index / col;
        float targetPos = row * cachedRowHeight;
        
        if (animated)
        {
            // 使用动画滚动
            ScrollToPositionAnimated(targetPos).Forget();
        }
        else
        {
            // 直接设置位置
            Vector2 pos = content.anchoredPosition;
            pos.y = targetPos;
            content.anchoredPosition = pos;
            CheckShowOrHide();
        }
    }

    /// <summary>
    /// 动画滚动到指定位置
    /// </summary>
    private async UniTaskVoid ScrollToPositionAnimated(float targetY)
    {
        Vector2 startPos = content.anchoredPosition;
        float duration = 0.3f;
        float timer = 0f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = Mathf.SmoothStep(0, 1, t);
            
            Vector2 pos = content.anchoredPosition;
            pos.y = Mathf.Lerp(startPos.y, targetY, t);
            content.anchoredPosition = pos;
            
            CheckShowOrHide();
            await UniTask.Yield();
        }
        
        Vector2 finalPos = content.anchoredPosition;
        finalPos.y = targetY;
        content.anchoredPosition = finalPos;
        CheckShowOrHide();
    }

    /// <summary>
    /// 清理所有格子
    /// </summary>
    public void ClearItems()
    {
        // 清空待创建队列
        pendingCreateQueue.Clear();
        
        // 回收所有显示的格子
        foreach (var kvp in nowShowItems)
        {
            K item = kvp.Value;
            if (item != null)
            {
                item.OnRecycle();
                GOPoolMgr.Instance.PushObj(item.gameObject);
            }
        }
        nowShowItems.Clear();
        
        cachedMinIndex = -1;
        cachedMaxIndex = -1;
        isDirty = true;
    }

    /// <summary>
    /// 销毁清理
    /// </summary>
    public void Dispose()
    {
        ClearItems();
        
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }
        
        items.Clear();
        pendingCreateQueue.Clear();
    }

    #endregion

    #region 调试信息

    /// <summary>
    /// 获取调试信息
    /// </summary>
    public string GetDebugInfo()
    {
        return $"数据数量: {items.Count}, " +
               $"显示中: {nowShowItems.Count}, " +
               $"待创建: {pendingCreateQueue.Count}, " +
               $"可见范围: [{cachedMinIndex}, {cachedMaxIndex}], " +
               $"数据版本: {dataVersion}";
    }

    #endregion
}

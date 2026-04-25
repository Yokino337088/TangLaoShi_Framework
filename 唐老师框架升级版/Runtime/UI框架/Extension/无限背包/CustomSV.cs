using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 该接口 作为 格子对象 必须继承的类 它用于实现初始化格子的方法
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IItemBase<T>
{
    void InitInfo(T info);
}

/// <summary>
/// 自定义sv类 用于节约性能 通过缓存池创建复用对象
/// </summary>
/// <typeparam name="T">代表的 数据来源类</typeparam>
/// <typeparam name="K">代表的 格子类</typeparam>
public class CustomSV<T, K> where K : IItemBase<T>
{
    //履带对象  需要通过他得到可视范围的位置  还要把动态创建的格子设置为他的子对象
    private RectTransform content;
    //可是范围高
    private int viewPortH;

    //当前现实着的格子对象
    private Dictionary<int, GameObject> nowShowItems = new Dictionary<int, GameObject>();

    //数据来源
    private List<T> items;

    //记录上一次显示的索引范围
    private int oldMinIndex = -1;
    private int oldMaxIndex = -1;

    //格子的间隔宽高
    private int itemW;
    private int itemH;

    //格子的列数
    private int col;
    
    // 添加行间距和列间距
    private int rowSpacing;
    private int colSpacing;
    
    // 添加左上角偏移量
    private int offsetX;
    private int offsetY;

    private string itemABName;
    //预设体资源的名字
    private string itemResName;



    /// <summary>
    /// 初始化格子资源路径
    /// </summary>
    /// <param name="name"></param>
    public void InitItemResName(string abName,string name)
    {
        itemABName = abName;
        itemResName = name;
    }

    /// <summary>
    /// 初始化Content父对象 以及 我们可视范围的高
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="h"></param>
    public void InitContentAndSVH(RectTransform trans, int h)
    {
        this.content = trans;
        this.viewPortH = h;
    }

    /// <summary>
    /// 初始化数据来源 并且把content的高初始化
    /// </summary>
    /// <param name="items"></param>
    public void InitInfos(List<T> items)
    {
        this.items = items;
        // 计算content高度时考虑行高和行间距
        float rowHeight = itemH + rowSpacing;
        content.sizeDelta = new Vector2(0, Mathf.CeilToInt(items.Count / (float)col) * rowHeight - rowSpacing + offsetY * 2);
    }

    /// <summary>
    /// 初始化格子间隔大小 以及 一行几列
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <param name="col"></param>
    public void InitItemSizeAndCol(int w, int h, int col)
    {
        this.itemW = w;
        this.itemH = h;
        this.col = col;
        // 默认间距和偏移量都为0
        this.rowSpacing = 0;
        this.colSpacing = 0;
        this.offsetX = 0;
        this.offsetY = 0;
    }
    
    /// <summary>
    /// 初始化格子大小、行列数及间距和偏移量
    /// </summary>
    /// <param name="w">格子宽度</param>
    /// <param name="h">格子高度</param>
    /// <param name="col">列数</param>
    /// <param name="colSpacing">列间距</param>
    /// <param name="rowSpacing">行间距</param>
    /// <param name="offsetX">X轴偏移量</param>
    /// <param name="offsetY">Y轴偏移量</param>
    public void InitItemSizeAndCol(int w, int h, int col, int colSpacing, int rowSpacing, int offsetX = 0, int offsetY = 0)
    {
        this.itemW = w;
        this.itemH = h;
        this.col = col;
        this.colSpacing = colSpacing;
        this.rowSpacing = rowSpacing;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
    }

    /// <summary>
    /// 更新格子显示的方法
    /// </summary>
    public void CheckShowOrHide()
    {
        // 计算可见索引范围时考虑行高和行间距
        float rowHeight = itemH + rowSpacing;
        int minIndex = (int)(content.anchoredPosition.y / rowHeight) * col;
        int maxIndex = (int)((content.anchoredPosition.y + viewPortH) / rowHeight) * col + col - 1;

        //最小值判断
        if (minIndex < 0)
            minIndex = 0;

        //超出道具最大数量
        if (maxIndex >= items.Count)
            maxIndex = items.Count - 1;

        if (minIndex != oldMinIndex ||
            maxIndex != oldMaxIndex)
        {
            //在记录当前索引之前 要做一些事儿
            //根据上一次索引和这一次新算出来的索引 用来判断 哪些该移除
            //删除上一节溢出
            for (int i = oldMinIndex; i < minIndex; ++i)
            {
                if (nowShowItems.ContainsKey(i))
                {
                    if (nowShowItems[i] != null)
                        GOPoolMgr.Instance.PushObj(nowShowItems[i]);
                    nowShowItems.Remove(i);
                }
            }
            //删除下一节溢出
            for (int i = maxIndex + 1; i <= oldMaxIndex; ++i)
            {
                if (nowShowItems.ContainsKey(i))
                {
                    if (nowShowItems[i] != null)
                        GOPoolMgr.Instance.PushObj(nowShowItems[i]);
                    nowShowItems.Remove(i);
                }
            }
        }

        oldMinIndex = minIndex;
        oldMaxIndex = maxIndex;

        //创建指定索引范围内的格子
        for (int i = minIndex; i <= maxIndex; ++i)
        {
            if (nowShowItems.ContainsKey(i))
                continue;
            else
            {
                //根据这个关键索引 用来设置位置 初始化道具信息
                int index = i;
                nowShowItems.Add(index, null);

                GOPoolMgr.Instance.GetObj(itemABName, itemResName, (obj) =>
                {
                    //当格子创建出来后我们要做什么
                    //设置它的父对象
                    obj.transform.SetParent(content);
                    //重置相对缩放大小
                    obj.transform.localScale = Vector3.one;

                    // 计算位置时考虑间距和偏移量
                    int row = index / col;
                    int column = index % col;
                    float xPos = offsetX + column * (itemW + colSpacing);
                    float yPos = -(offsetY + row * (itemH + rowSpacing));
                    obj.transform.localPosition = new Vector3(xPos, yPos, 0);

                    //更新格子信息
                    obj.GetComponent<K>().InitInfo(items[index]);

                    //判断有没有这个坑
                    if (nowShowItems.ContainsKey(index))
                        nowShowItems[index] = obj;
                    else
                        GOPoolMgr.Instance.PushObj(obj);
                });

                
                
                
            }
        }

    }

    /// <summary>
    /// 清理物件的方法
    /// </summary>
    public void ClearItems()
    {
        foreach (var item in nowShowItems.Values)
        {
            if (item != null)
                GOPoolMgr.Instance.PushObj(item);
        }
        nowShowItems.Clear();
        oldMinIndex = -1;
        oldMaxIndex = -1;
    }
}

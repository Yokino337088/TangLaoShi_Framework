using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 引用对象池管理器
/// 负责管理所有引用类型对象的缓存和复用
/// </summary>
public class ReferencePoolMgr : BaseManager<ReferencePoolMgr>
{
    // 私有构造函数
    private ReferencePoolMgr() { }
    
    // 引用对象池字典
    private Dictionary<string, ReferencePoolBase> poolDic = new Dictionary<string, ReferencePoolBase>();
    
    /// <summary>
    /// 初始化引用对象池管理器
    /// </summary>
    public void Initialize()
    {
        LogSystem.Info("引用对象池管理器初始化");
    }
    
    /// <summary>
    /// 获取引用对象
    /// </summary>
    /// <typeparam name="T">引用对象类型</typeparam>
    /// <param name="nameSpace">命名空间，用于区分不同用途的同类型对象</param>
    /// <returns>引用对象实例</returns>
    public T Get<T>(string nameSpace = "") where T : class, IReferencePoolObject, new()
    {
        string poolKey = GetPoolKey<T>(nameSpace);
        
        // 检查是否存在对应类型的对象池
        if (!poolDic.ContainsKey(poolKey))
        {
            // 创建新的对象池
            ReferencePool<T> pool = new ReferencePool<T>();
            poolDic.Add(poolKey, pool);
            LogSystem.Info($"创建引用对象池: {poolKey}");
        }
        
        // 从对象池获取对象
        ReferencePool<T> targetPool = poolDic[poolKey] as ReferencePool<T>;
        if (targetPool == null)
        {
            LogSystem.Error($"对象池类型转换失败: {poolKey}");
            return new T();
        }
        
        return targetPool.Get();
    }

    
    
    /// <summary>
    /// 归还引用对象到对象池
    /// </summary>
    /// <typeparam name="T">引用对象类型</typeparam>
    /// <param name="obj">要归还的对象</param>
    /// <param name="nameSpace">命名空间，用于区分不同用途的同类型对象</param>
    public void Return<T>(T obj, string nameSpace = "") where T : class, IReferencePoolObject, new()
    {
        if (obj == null)
        {
            LogSystem.Warning("尝试归还空对象到引用对象池");
            return;
        }
        
        string poolKey = GetPoolKey<T>(nameSpace);
        
        // 检查是否存在对应类型的对象池
        if (!poolDic.ContainsKey(poolKey))
        {
            LogSystem.Warning($"尝试归还对象到不存在的对象池: {poolKey}");
            return;
        }
        
        // 归还对象到对象池
        ReferencePool<T> targetPool = poolDic[poolKey] as ReferencePool<T>;
        if (targetPool == null)
        {
            LogSystem.Error($"对象池类型转换失败: {poolKey}");
            return;
        }
        
        targetPool.Return(obj);
    }

    
    
    /// <summary>
    /// 设置对象池的最大容量
    /// </summary>
    /// <typeparam name="T">引用对象类型</typeparam>
    /// <param name="maxCapacity">最大容量</param>
    /// <param name="nameSpace">命名空间，用于区分不同用途的同类型对象</param>
    public void SetMaxCapacity<T>(int maxCapacity, string nameSpace = "") where T : class, IReferencePoolObject, new()
    {
        string poolKey = GetPoolKey<T>(nameSpace);
        
        // 检查是否存在对应类型的对象池
        if (!poolDic.ContainsKey(poolKey))
        {
            // 创建新的对象池
            ReferencePool<T> pool = new ReferencePool<T>();
            pool.SetMaxCapacity(maxCapacity);
            poolDic.Add(poolKey, pool);
            LogSystem.Info($"创建引用对象池并设置最大容量: {poolKey}, 容量: {maxCapacity}");
        }
        else
        {
            // 设置现有对象池的最大容量
            ReferencePool<T> targetPool = poolDic[poolKey] as ReferencePool<T>;
            if (targetPool != null)
            {
                targetPool.SetMaxCapacity(maxCapacity);
                LogSystem.Info($"设置引用对象池最大容量: {poolKey}, 容量: {maxCapacity}");
            }
        }
    }

    
    
    /// <summary>
    /// 获取对象池的当前大小
    /// </summary>
    /// <typeparam name="T">引用对象类型</typeparam>
    /// <param name="nameSpace">命名空间，用于区分不同用途的同类型对象</param>
    /// <returns>对象池当前大小</returns>
    public int GetPoolSize<T>(string nameSpace = "") where T : class, IReferencePoolObject,new()
    {
        string poolKey = GetPoolKey<T>(nameSpace);
        
        if (poolDic.ContainsKey(poolKey))
        {
            ReferencePool<T> targetPool = poolDic[poolKey] as ReferencePool<T>;
            if (targetPool != null)
            {
                return targetPool.GetPoolSize();
            }
        }
        
        return 0;
    }
    
    /// <summary>
    /// 清理指定类型的对象池
    /// </summary>
    /// <typeparam name="T">引用对象类型</typeparam>
    /// <param name="nameSpace">命名空间，用于区分不同用途的同类型对象</param>
    public void ClearPool<T>(string nameSpace = "") where T : class, IReferencePoolObject, new()
    {
        string poolKey = GetPoolKey<T>(nameSpace);
        
        if (poolDic.ContainsKey(poolKey))
        {
            ReferencePool<T> targetPool = poolDic[poolKey] as ReferencePool<T>;
            if (targetPool != null)
            {
                targetPool.Clear();
                LogSystem.Info($"清理引用对象池: {poolKey}");
            }
        }
    }
    
    /// <summary>
    /// 清理所有对象池
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in poolDic.Values)
        {
            pool.Clear();
        }
        poolDic.Clear();
        LogSystem.Info("清理所有引用对象池");
    }
    
    /// <summary>
    /// 获取对象池键值
    /// </summary>
    /// <typeparam name="T">引用对象类型</typeparam>
    /// <param name="nameSpace">命名空间</param>
    /// <returns>对象池键值</returns>
    private string GetPoolKey<T>(string nameSpace)
    {
        string typeName = typeof(T).FullName;
        if (!string.IsNullOrEmpty(nameSpace))
        {
            return $"{nameSpace}_{typeName}";
        }
        return typeName;
    }
}

/// <summary>
/// 引用对象池基类
/// 用于统一管理不同类型的引用对象池
/// </summary>
public abstract class ReferencePoolBase
{
    /// <summary>
    /// 清理对象池
    /// </summary>
    public abstract void Clear();
}

/// <summary>
/// 引用对象池
/// 用于管理特定类型的引用对象
/// </summary>
/// <typeparam name="T">引用对象类型</typeparam>
// LRU节点类，用于双向链表
public class LRUNode<T> where T : class
{
    public T Value { get; set; }
    public LRUNode<T> Prev { get; set; }
    public LRUNode<T> Next { get; set; }
    
    public LRUNode(T value)
    {
        Value = value;
    }
}

public class ReferencePool<T> : ReferencePoolBase where T : class, IReferencePoolObject, new()
{
    // 双向链表，用于实现LRU
    private LRUNode<T> head;
    private LRUNode<T> tail;
    
    // 哈希表，用于快速查找节点
    private Dictionary<T, LRUNode<T>> nodeMap;
    
    // 最大容量
    private int maxCapacity = 30;
    
    // 当前使用中的对象数量
    private int usingCount = 0;
    
    public ReferencePool()
    {
        nodeMap = new Dictionary<T, LRUNode<T>>();
    }
    
    /// <summary>
    /// 设置最大容量
    /// </summary>
    /// <param name="capacity">最大容量</param>
    public void SetMaxCapacity(int capacity)
    {
        if (capacity > 0)
        {
            maxCapacity = capacity;
            // 如果当前池大小超过新的最大容量，清理多余的对象
            while (nodeMap.Count > maxCapacity)
            {
                RemoveLeastRecentlyUsed();
            }
        }
    }
    
    /// <summary>
    /// 获取对象
    /// </summary>
    /// <returns>对象实例</returns>
    public T Get()
    {
        T obj;
        
        // 如果池中有对象，直接取出最近使用的
        if (nodeMap.Count > 0)
        {
            // 取出最近使用的对象（链表头部）
            obj = head.Value;
            // 从链表中移除
            RemoveFromList(head);
            nodeMap.Remove(obj);
        }
        else
        {
            // 池中没有对象，创建新实例
            obj = new T();
        }
        
        // 增加使用计数
        usingCount++;
        
        // 初始化对象
        obj.OnGet();
        
        return obj;
    }

    
    /// <summary>
    /// 归还对象
    /// </summary>
    /// <param name="obj">要归还的对象</param>
    public void Return(T obj)
    {
        if (obj == null)
        {
            LogSystem.Warning("尝试归还空对象到引用对象池");
            return;
        }
        
        // 重置对象状态
        obj.OnReturn();
        
        // 减少使用计数
        usingCount = Math.Max(0, usingCount - 1);
        
        // 如果对象已经在池中，先移除
        if (nodeMap.ContainsKey(obj))
        {
            RemoveFromList(nodeMap[obj]);
            nodeMap.Remove(obj);
        }
        
        // 检查是否超过最大容量
        if (nodeMap.Count >= maxCapacity)
        {
            // 移除最久未使用的对象（链表尾部）
            RemoveLeastRecentlyUsed();
        }
        
        // 将对象添加到链表头部（标记为最近使用）
        AddToHead(obj);
    }
    
    /// <summary>
    /// 添加对象到链表头部
    /// </summary>
    /// <param name="obj">要添加的对象</param>
    private void AddToHead(T obj)
    {
        LRUNode<T> node = new LRUNode<T>(obj);
        
        if (head == null)
        {
            // 链表为空
            head = node;
            tail = node;
        }
        else
        {
            // 添加到头部
            node.Next = head;
            head.Prev = node;
            head = node;
        }
        
        // 添加到哈希表
        nodeMap[obj] = node;
    }
    
    /// <summary>
    /// 从链表中移除节点
    /// </summary>
    /// <param name="node">要移除的节点</param>
    private void RemoveFromList(LRUNode<T> node)
    {
        if (node == head && node == tail)
        {
            // 链表只有一个节点
            head = null;
            tail = null;
        }
        else if (node == head)
        {
            // 移除头部节点
            head = node.Next;
            head.Prev = null;
        }
        else if (node == tail)
        {
            // 移除尾部节点
            tail = node.Prev;
            tail.Next = null;
        }
        else
        {
            // 移除中间节点
            node.Prev.Next = node.Next;
            node.Next.Prev = node.Prev;
        }
    }
    
    /// <summary>
    /// 移除最久未使用的对象（链表尾部）
    /// </summary>
    private void RemoveLeastRecentlyUsed()
    {
        if (tail != null)
        {
            T obj = tail.Value;
            RemoveFromList(tail);
            nodeMap.Remove(obj);
        }
    }

    
    /// <summary>
    /// 获取对象池当前大小
    /// </summary>
    /// <returns>对象池当前大小</returns>
    public int GetPoolSize()
    {
        return nodeMap.Count;
    }
    
    /// <summary>
    /// 获取当前使用中的对象数量
    /// </summary>
    /// <returns>当前使用中的对象数量</returns>
    public int GetUsingCount()
    {
        return usingCount;
    }
    
    /// <summary>
    /// 清理对象池
    /// </summary>
    public override void Clear()
    {
        nodeMap.Clear();
        head = null;
        tail = null;
        usingCount = 0;
    }
}

/// <summary>
/// 引用对象接口
/// 所有需要被引用对象池管理的类都需要实现此接口
/// </summary>
public interface IReferencePoolObject
{
    /// <summary>
    /// 对象被获取时调用
    /// </summary>
    void OnGet();
    
    /// <summary>
    /// 对象被归还时调用
    /// </summary>
    void OnReturn();
}

/// <summary>
/// 引用对象基类
/// 提供默认的OnGet和OnReturn实现
/// </summary>
public abstract class ReferencePoolObjectBase : IReferencePoolObject
{
    /// <summary>
    /// 对象被获取时调用
    /// </summary>
    public virtual void OnGet()
    {
        // 默认实现，子类可以重写
    }
    
    /// <summary>
    /// 对象被归还时调用
    /// </summary>
    public virtual void OnReturn()
    {
        // 默认实现，子类可以重写
    }
}

using System;

/// <summary>
/// 基础数据类
/// 所有数据类的基类
/// </summary>
public abstract class BaseData : IData
{
    /// <summary>
    /// 数据ID
    /// </summary>
    public string Id { get; protected set; }
    
    /// <summary>
    /// 数据名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 数据是否已初始化
    /// </summary>
    public bool IsInitialized { get; protected set; }
    
    /// <summary>
    /// 数据是否已加载
    /// </summary>
    public bool IsLoaded { get; protected set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id">数据ID</param>
    /// <param name="name">数据名称</param>
    protected BaseData(string id, string name = null)
    {
        Id = id;
        Name = name ?? id;
        IsInitialized = false;
        IsLoaded = false;
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    public virtual void Initialize()
    {
        if (!IsInitialized)
        {
            OnInitialize();
            IsInitialized = true;
        }
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    public virtual void Reset()
    {
        OnReset();
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    public virtual void Save()
    {
        if (IsInitialized)
        {
            OnSave();
        }
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    public virtual void Load()
    {
        OnLoad();
        IsLoaded = true;
    }

    #region 虚方法

    /// <summary>
    /// 初始化时调用
    /// </summary>
    protected virtual void OnInitialize() { }
    
    /// <summary>
    /// 重置时调用
    /// </summary>
    protected virtual void OnReset() { }
    
    /// <summary>
    /// 保存时调用
    /// </summary>
    protected virtual void OnSave() { }
    
    /// <summary>
    /// 加载时调用
    /// </summary>
    protected virtual void OnLoad() { }

    #endregion

    #region 工具方法

    /// <summary>
    /// 检查数据是否已初始化
    /// </summary>
    protected void CheckInitialized()
    {
        if (!IsInitialized)
        {
            throw new Exception($"Data {Id} is not initialized");
        }
    }

    /// <summary>
    /// 检查数据是否已加载
    /// </summary>
    protected void CheckLoaded()
    {
        if (!IsLoaded)
        {
            throw new Exception($"Data {Id} is not loaded");
        }
    }

    #endregion

    #region 重写方法

    public override string ToString()
    {
        return $"{GetType().Name} [Id: {Id}, Name: {Name}]";
    }

    #endregion
}

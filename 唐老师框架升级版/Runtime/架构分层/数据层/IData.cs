using System;

/// <summary>
/// 数据接口
/// 所有数据类都需要实现此接口
/// </summary>
public interface IData
{
    /// <summary>
    /// 数据ID
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// 数据名称
    /// </summary>
    string Name { get; set; }
    
    /// <summary>
    /// 初始化数据
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// 重置数据
    /// </summary>
    void Reset();
    
    /// <summary>
    /// 保存数据
    /// </summary>
    void Save();
    
    /// <summary>
    /// 加载数据
    /// </summary>
    void Load();
}

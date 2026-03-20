using System;

/// <summary>
/// 系统接口，所有游戏系统都需要实现此接口
/// </summary>
public interface ISystem : IDisposable
{
    /// <summary>
    /// 系统初始化
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// 系统启动
    /// </summary>
    void Start();
    
    /// <summary>
    /// 系统更新
    /// </summary>
    void Update();
    
    /// <summary>
    /// 系统停止
    /// </summary>
    void Stop();
    
    /// <summary>
    /// 系统是否已初始化
    /// </summary>
    bool IsInitialized { get; }
    
    /// <summary>
    /// 系统是否已启动
    /// </summary>
    bool IsStarted { get; }
}
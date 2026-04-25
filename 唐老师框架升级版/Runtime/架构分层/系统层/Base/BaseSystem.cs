using System;

/// <summary>
/// 系统基类，实现ISystem接口的通用功能
/// </summary>
public abstract class BaseSystem : ISystem
{
    /// <summary>
    /// 系统是否已初始化
    /// </summary>
    public bool IsInitialized { get; protected set; }
    
    /// <summary>
    /// 系统是否已启动
    /// </summary>
    public bool IsStarted { get; protected set; }
    
    /// <summary>
    /// 系统初始化
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
    /// 系统启动
    /// </summary>
    public virtual void Start()
    {
        if (!IsStarted && IsInitialized)
        {
            OnStart();
            IsStarted = true;
        }
    }
    
    /// <summary>
    /// 系统更新
    /// </summary>
    public virtual void Update()
    {
        if (IsStarted && IsInitialized)
        {
            OnUpdate();
        }
    }
    
    /// <summary>
    /// 系统停止
    /// </summary>
    public virtual void Stop()
    {
        if (IsStarted)
        {
            OnStop();
            IsStarted = false;
        }
    }
    
    /// <summary>
    /// 释放资源
    /// </summary>
    public virtual void Dispose()
    {
        Stop();
        OnDispose();
    }
    
    /// <summary>
    /// 初始化时调用
    /// </summary>
    protected virtual void OnInitialize() { }
    
    /// <summary>
    /// 启动时调用
    /// </summary>
    protected virtual void OnStart() { }
    
    /// <summary>
    /// 更新时调用
    /// </summary>
    protected virtual void OnUpdate() { }
    
    /// <summary>
    /// 停止时调用
    /// </summary>
    protected virtual void OnStop() { }
    
    /// <summary>
    /// 释放资源时调用
    /// </summary>
    protected virtual void OnDispose() { }
}
using System;

/// <summary>
/// 表现层基类，实现IPresenter接口并提供默认实现
/// </summary>
public abstract class BasePresenter : IPresenter
{
    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Initialize() { }
    
    /// <summary>
    /// 启动
    /// </summary>
    public virtual void Start() { }
    
    /// <summary>
    /// 更新
    /// </summary>
    public virtual void Update() { }
    
    /// <summary>
    /// 停止
    /// </summary>
    public virtual void Stop() { }
    
    /// <summary>
    /// 销毁
    /// </summary>
    public virtual void Dispose() { }
}
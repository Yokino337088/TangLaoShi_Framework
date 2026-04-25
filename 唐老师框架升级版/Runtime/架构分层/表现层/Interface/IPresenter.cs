using System;

/// <summary>
/// 表现层接口，定义表现层的基本方法
/// </summary>
public interface IPresenter
{
    /// <summary>
    /// 初始化
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// 启动
    /// </summary>
    void Start();
    
    /// <summary>
    /// 更新
    /// </summary>
    void Update();
    
    /// <summary>
    /// 停止
    /// </summary>
    void Stop();
    
    /// <summary>
    /// 销毁
    /// </summary>
    void Dispose();
}
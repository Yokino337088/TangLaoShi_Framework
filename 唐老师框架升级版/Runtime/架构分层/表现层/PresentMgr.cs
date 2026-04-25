using System;
using System.Collections.Generic;

/// <summary>
/// 表现层管理器，负责统一管理所有表现层
/// </summary>
public class PresentMgr : BaseManager<PresentMgr>
{
    /// <summary>
    /// 表现层字典，存储所有注册的表现层
    /// </summary>
    private Dictionary<Type, IPresenter> presenterDict = new Dictionary<Type, IPresenter>();
    
    /// <summary>
    /// 表现层列表，用于按顺序更新
    /// </summary>
    private List<IPresenter> presenterList = new List<IPresenter>();
    
    /// <summary>
    /// 私有构造函数
    /// </summary>
    private PresentMgr() { }
    
    /// <summary>
    /// 注册表现层
    /// </summary>
    /// <typeparam name="T">表现层类型</typeparam>
    /// <param name="presenter">表现层实例</param>
    /// <returns>注册的表现层实例</returns>
    public T RegisterPresenter<T>(T presenter) where T : IPresenter
    {
        Type presenterType = typeof(T);
        
        if (!presenterDict.ContainsKey(presenterType))
        {
            presenterDict.Add(presenterType, presenter);
            presenterList.Add(presenter);
            
            // 自动初始化表现层
            presenter.Initialize();
        }
        
        return presenter;
    }
    
    /// <summary>
    /// 获取表现层
    /// </summary>
    /// <typeparam name="T">表现层类型</typeparam>
    /// <returns>表现层实例，如果未注册则返回null</returns>
    public T GetPresenter<T>() where T : IPresenter
    {
        Type presenterType = typeof(T);
        
        if (presenterDict.TryGetValue(presenterType, out var presenter))
        {
            return (T)presenter;
        }
        
        return default;
    }
    
    /// <summary>
    /// 注销表现层
    /// </summary>
    /// <typeparam name="T">表现层类型</typeparam>
    public void UnregisterPresenter<T>() where T : IPresenter
    {
        Type presenterType = typeof(T);
        
        if (presenterDict.TryGetValue(presenterType, out var presenter))
        {
            presenter.Dispose();
            presenterDict.Remove(presenterType);
            presenterList.Remove(presenter);
        }
    }
    
    /// <summary>
    /// 初始化所有表现层
    /// </summary>
    public void InitializeAllPresenters()
    {
        foreach (var presenter in presenterList)
        {
            presenter.Initialize();
        }
    }
    
    /// <summary>
    /// 启动所有表现层
    /// </summary>
    public void StartAllPresenters()
    {
        foreach (var presenter in presenterList)
        {
            presenter.Start();
        }
    }
    
    /// <summary>
    /// 更新所有表现层
    /// </summary>
    public void UpdateAllPresenters()
    {
        foreach (var presenter in presenterList)
        {
            presenter.Update();
        }
    }
    
    /// <summary>
    /// 停止所有表现层
    /// </summary>
    public void StopAllPresenters()
    {
        foreach (var presenter in presenterList)
        {
            presenter.Stop();
        }
    }
    
    /// <summary>
    /// 清理所有表现层
    /// </summary>
    public void ClearAllPresenters()
    {
        foreach (var presenter in presenterList)
        {
            presenter.Dispose();
        }
        
        presenterDict.Clear();
        presenterList.Clear();
    }
    
    /// <summary>
    /// 获取表现层数量
    /// </summary>
    public int PresenterCount => presenterList.Count;
}
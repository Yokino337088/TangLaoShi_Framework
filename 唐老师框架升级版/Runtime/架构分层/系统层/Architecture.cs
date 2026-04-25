using System;
using UnityEngine;

/// <summary>
/// 架构核心类，用于统一管理所有系统、事件、资源等
/// </summary>
public class Architecture : BaseManager<Architecture>
{
    /// <summary>
    /// 私有构造函数
    /// </summary>
    private Architecture() { }
    
    /// <summary>
    /// 初始化架构
    /// </summary>
    public void Initialize()
    {
        // 初始化系统管理器
        SystemMgr.Instance.InitializeAllSystems();
        
        // 启动所有系统
        SystemMgr.Instance.StartAllSystems();
        
        // 初始化表现层管理器
        PresentMgr.Instance.InitializeAllPresenters();
        
        // 启动所有表现层
        PresentMgr.Instance.StartAllPresenters();
        
        // 注册系统更新到MonoMgr
        MonoMgr.Instance.AddUpdateListener(Update);
    }
    
    /// <summary>
    /// 更新架构
    /// </summary>
    private void Update()
    {
        // 更新所有系统
        SystemMgr.Instance.UpdateAllSystems();
        
        // 更新所有表现层
        PresentMgr.Instance.UpdateAllPresenters();
    }
    
    /// <summary>
    /// 注册系统
    /// </summary>
    /// <typeparam name="T">系统类型</typeparam>
    /// <param name="system">系统实例</param>
    /// <returns>注册的系统实例</returns>
    public T RegisterSystem<T>(T system) where T : ISystem
    {
        return SystemMgr.Instance.RegisterSystem(system);
    }
    
    /// <summary>
    /// 获取系统
    /// </summary>
    /// <typeparam name="T">系统类型</typeparam>
    /// <returns>系统实例，如果未注册则返回null</returns>
    public T GetSystem<T>() where T : ISystem
    {
        return SystemMgr.Instance.GetSystem<T>();
    }
    
    /// <summary>
    /// 注销系统
    /// </summary>
    /// <typeparam name="T">系统类型</typeparam>
    public void UnregisterSystem<T>() where T : ISystem
    {
        SystemMgr.Instance.UnregisterSystem<T>();
    }
    
    /// <summary>
    /// 注册表现层
    /// </summary>
    /// <typeparam name="T">表现层类型</typeparam>
    /// <param name="presenter">表现层实例</param>
    /// <returns>注册的表现层实例</returns>
    public T RegisterPresenter<T>(T presenter) where T : IPresenter
    {
        return PresentMgr.Instance.RegisterPresenter(presenter);
    }
    
    /// <summary>
    /// 获取表现层
    /// </summary>
    /// <typeparam name="T">表现层类型</typeparam>
    /// <returns>表现层实例，如果未注册则返回null</returns>
    public T GetPresenter<T>() where T : IPresenter
    {
        return PresentMgr.Instance.GetPresenter<T>();
    }
    
    /// <summary>
    /// 注销表现层
    /// </summary>
    /// <typeparam name="T">表现层类型</typeparam>
    public void UnregisterPresenter<T>() where T : IPresenter
    {
        PresentMgr.Instance.UnregisterPresenter<T>();
    }
    
    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    public void TriggerEvent(E_EventType eventType)
    {
        EventCenter.Instance.EventTrigger(eventType);
    }
    
    /// <summary>
    /// 触发事件
    /// </summary>
    /// <typeparam name="T">事件参数类型</typeparam>
    /// <param name="eventType">事件类型</param>
    /// <param name="param">事件参数</param>
    public void TriggerEvent<T>(E_EventType eventType, T param)
    {
        EventCenter.Instance.EventTrigger(eventType, param);
    }
    
    /// <summary>
    /// 添加事件监听器
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="action">事件处理函数</param>
    public void AddEventListener(E_EventType eventType, Action action)
    {
        EventCenter.Instance.AddEventListener(eventType, action);
    }
    
    /// <summary>
    /// 添加事件监听器
    /// </summary>
    /// <typeparam name="T">事件参数类型</typeparam>
    /// <param name="eventType">事件类型</param>
    /// <param name="action">事件处理函数</param>
    public void AddEventListener<T>(E_EventType eventType, Action<T> action)
    {
        EventCenter.Instance.AddEventListener(eventType, action);
    }
    
    /// <summary>
    /// 移除事件监听器
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="action">事件处理函数</param>
    public void RemoveEventListener(E_EventType eventType, Action action)
    {
        EventCenter.Instance.RemoveEventListener(eventType, action);
    }
    
    /// <summary>
    /// 移除事件监听器
    /// </summary>
    /// <typeparam name="T">事件参数类型</typeparam>
    /// <param name="eventType">事件类型</param>
    /// <param name="action">事件处理函数</param>
    public void RemoveEventListener<T>(E_EventType eventType, Action<T> action)
    {
        EventCenter.Instance.RemoveEventListener(eventType, action);
    }
    
    /// <summary>
    /// 清理架构
    /// </summary>
    public void Clear()
    {
        // 停止所有系统
        SystemMgr.Instance.StopAllSystems();
        
        // 停止所有表现层
        PresentMgr.Instance.StopAllPresenters();
        
        // 清理所有系统
        SystemMgr.Instance.ClearAllSystems();
        
        // 清理所有表现层
        PresentMgr.Instance.ClearAllPresenters();
        
        // 移除更新监听
        MonoMgr.Instance.RemoveUpdateListener(Update);
    }
}
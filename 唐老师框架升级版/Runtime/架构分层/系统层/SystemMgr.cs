using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 系统管理器，负责统一管理所有游戏系统
/// </summary>
public class SystemMgr : BaseManager<SystemMgr>
{
    /// <summary>
    /// 系统字典，存储所有注册的系统
    /// </summary>
    private Dictionary<Type, ISystem> systemDict = new Dictionary<Type, ISystem>();
    
    /// <summary>
    /// 系统列表，用于按顺序更新
    /// </summary>
    private List<ISystem> systemList = new List<ISystem>();
    
    /// <summary>
    /// 私有构造函数
    /// </summary>
    private SystemMgr() { }
    
    /// <summary>
    /// 注册系统
    /// </summary>
    /// <typeparam name="T">系统类型</typeparam>
    /// <param name="system">系统实例</param>
    /// <returns>注册的系统实例</returns>
    public T RegisterSystem<T>(T system) where T : ISystem
    {
        Type systemType = typeof(T);
        
        if (!systemDict.ContainsKey(systemType))
        {
            systemDict.Add(systemType, system);
            systemList.Add(system);
            
            // 自动初始化系统
            system.Initialize();
        }
        
        return system;
    }
    
    /// <summary>
    /// 获取系统
    /// </summary>
    /// <typeparam name="T">系统类型</typeparam>
    /// <returns>系统实例，如果未注册则返回null</returns>
    public T GetSystem<T>() where T : ISystem
    {
        Type systemType = typeof(T);
        
        if (systemDict.TryGetValue(systemType, out var system))
        {
            return (T)system;
        }
        
        return default;
    }
    
    /// <summary>
    /// 注销系统
    /// </summary>
    /// <typeparam name="T">系统类型</typeparam>
    public void UnregisterSystem<T>() where T : ISystem
    {
        Type systemType = typeof(T);
        
        if (systemDict.TryGetValue(systemType, out var system))
        {
            system.Dispose();
            systemDict.Remove(systemType);
            systemList.Remove(system);
        }
    }
    
    /// <summary>
    /// 初始化所有系统
    /// </summary>
    public void InitializeAllSystems()
    {
        foreach (var system in systemList)
        {
            system.Initialize();
        }
    }
    
    /// <summary>
    /// 启动所有系统
    /// </summary>
    public void StartAllSystems()
    {
        foreach (var system in systemList)
        {
            system.Start();
        }
    }
    
    /// <summary>
    /// 更新所有系统
    /// </summary>
    public void UpdateAllSystems()
    {
        foreach (var system in systemList)
        {
            system.Update();
        }
    }
    
    /// <summary>
    /// 停止所有系统
    /// </summary>
    public void StopAllSystems()
    {
        foreach (var system in systemList)
        {
            system.Stop();
        }
    }
    
    /// <summary>
    /// 清理所有系统
    /// </summary>
    public void ClearAllSystems()
    {
        foreach (var system in systemList)
        {
            system.Dispose();
        }
        
        systemDict.Clear();
        systemList.Clear();
    }
    
    /// <summary>
    /// 获取系统数量
    /// </summary>
    public int SystemCount => systemList.Count;
}
using UnityEngine;

/// <summary>
/// 游戏系统示例，演示如何创建和使用系统
/// </summary>
public class GameSystemExample : BaseSystem
{
    /// <summary>
    /// 初始化时调用
    /// </summary>
    protected override void OnInitialize()
    {
        Debug.Log("GameSystemExample: OnInitialize");
    }
    
    /// <summary>
    /// 启动时调用
    /// </summary>
    protected override void OnStart()
    {
        Debug.Log("GameSystemExample: OnStart");
    }
    
    /// <summary>
    /// 更新时调用
    /// </summary>
    protected override void OnUpdate()
    {
        // 这里可以添加游戏逻辑更新
    }
    
    /// <summary>
    /// 停止时调用
    /// </summary>
    protected override void OnStop()
    {
        Debug.Log("GameSystemExample: OnStop");
    }
    
    /// <summary>
    /// 释放资源时调用
    /// </summary>
    protected override void OnDispose()
    {
        Debug.Log("GameSystemExample: OnDispose");
    }
    
    /// <summary>
    /// 示例方法
    /// </summary>
    public void ExampleMethod()
    {
        Debug.Log("GameSystemExample: ExampleMethod");
    }
}
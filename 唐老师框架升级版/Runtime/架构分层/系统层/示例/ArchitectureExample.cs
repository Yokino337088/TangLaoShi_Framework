using UnityEngine;

/// <summary>
/// 架构示例，演示如何使用架构和系统
/// </summary>
public class ArchitectureExample : MonoBehaviour
{
    private void Start()
    {
        // 初始化架构
        Architecture.Instance.Initialize();
        
        // 注册游戏系统
        var gameSystem = Architecture.Instance.RegisterSystem(new GameSystemExample());
        
        // 获取游戏系统并调用方法
        var gameSystemFromArchitecture = Architecture.Instance.GetSystem<GameSystemExample>();
        if (gameSystemFromArchitecture != null)
        {
            gameSystemFromArchitecture.ExampleMethod();
        }
        
        // 示例：如何在其他地方获取系统
        Debug.Log("ArchitectureExample: 架构初始化完成，系统已注册");
    }
    
    private void OnDestroy()
    {
        // 清理架构
        Architecture.Instance.Clear();
    }
}
using UnityEngine;

/// <summary>
/// 摄像机管理示例
/// 演示如何注册、切换和使用不同类型的摄像机
/// </summary>
public class CameraExample : MonoBehaviour
{
    [Header("摄像机对象")]
    public Camera playerCamera;          // 玩家摄像机
    public Camera combatCamera;          // 战斗摄像机
    public Camera cutsceneCamera;        // 过场动画摄像机
    public Camera minimapCamera;         // 小地图摄像机
    
    [Header("摄像机控制器")]
    public AdvancedPlayerCamera playerCameraController; // 玩家摄像机控制器
    
    [Header("测试设置")]
    public bool enableDebugUI = true;    // 是否启用调试UI
    
    private bool showCameraInfo = false;
    
    private void Start()
    {
        // 初始化摄像机管理
        InitializeCameraSystem();
        
        // 注册摄像机
        RegisterCameras();
        
        // 设置默认摄像机
        SetDefaultCamera();
        
        // 切换到默认摄像机
        CameraManager.Instance.SwitchCamera(CameraId.Player);
        
        Debug.Log("CameraExample initialized. Press 'C' to toggle camera info.");
    }
    
    private void Update()
    {
        // 检查调试输入
        if (Input.GetKeyDown(KeyCode.C))
        {
            showCameraInfo = !showCameraInfo;
            Debug.Log($"Camera info display: {showCameraInfo}");
        }
        
        // 测试摄像机切换
        TestCameraSwitching();
        
        // 测试摄像机控制
        TestCameraControls();
    }
    
    private void OnGUI()
    {
        if (enableDebugUI && showCameraInfo)
        {
            GUI.Box(new Rect(10, 10, 300, 250), "Camera Info");
            
            // 显示当前激活的摄像机
            CameraId activeCameraId = CameraManager.Instance.GetActiveCameraId();
            GUI.Label(new Rect(20, 40, 280, 20), $"Active Camera: {activeCameraId}");
            
            // 显示摄像机切换提示
            GUI.Label(new Rect(20, 60, 280, 20), "Camera Switching:");
            GUI.Label(new Rect(30, 80, 280, 20), "1: Player Camera");
            GUI.Label(new Rect(30, 100, 280, 20), "2: Combat Camera");
            GUI.Label(new Rect(30, 120, 280, 20), "3: Cutscene Camera");
            GUI.Label(new Rect(30, 140, 280, 20), "4: Minimap Camera");
            GUI.Label(new Rect(30, 160, 280, 20), "0: Default Camera");
            
            // 显示摄像机控制提示
            GUI.Label(new Rect(20, 180, 280, 20), "Camera Controls:");
            GUI.Label(new Rect(30, 200, 280, 20), "Mouse: Look around");
            GUI.Label(new Rect(30, 220, 280, 20), "Mouse Wheel: Zoom");
            GUI.Label(new Rect(30, 240, 280, 20), "V: Toggle First Person");
        }
    }
    
    /// <summary>
    /// 初始化摄像机系统
    /// </summary>
    private void InitializeCameraSystem()
    {
        // 设置摄像机切换过渡时长
        CameraManager.Instance.SetTransitionDuration(0.3f);
        
        Debug.Log("Camera system initialized.");
    }
    
    /// <summary>
    /// 注册摄像机
    /// </summary>
    private void RegisterCameras()
    {
        // 注册玩家摄像机
        if (playerCamera != null)
        {
            CameraManager.Instance.RegisterCamera(CameraId.Player, playerCamera, playerCameraController, true);
        }
        
        // 注册战斗摄像机
        if (combatCamera != null)
        {
            CameraManager.Instance.RegisterCamera(CameraId.Combat, combatCamera);
        }
        
        // 注册过场动画摄像机
        if (cutsceneCamera != null)
        {
            CameraManager.Instance.RegisterCamera(CameraId.Cutscene, cutsceneCamera);
        }
        
        // 注册小地图摄像机
        if (minimapCamera != null)
        {
            CameraManager.Instance.RegisterCamera(CameraId.Minimap, minimapCamera);
        }
        
        Debug.Log("Cameras registered.");
    }
    
    /// <summary>
    /// 设置默认摄像机
    /// </summary>
    private void SetDefaultCamera()
    {
        // 默认摄像机已经在注册时设置
        Debug.Log("Default camera set.");
    }
    
    /// <summary>
    /// 测试摄像机切换
    /// </summary>
    private void TestCameraSwitching()
    {
        // 数字键1-4切换不同摄像机
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CameraManager.Instance.SwitchCamera(CameraId.Player);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CameraManager.Instance.SwitchCamera(CameraId.Combat);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CameraManager.Instance.SwitchCamera(CameraId.Cutscene);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CameraManager.Instance.SwitchCamera(CameraId.Minimap);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CameraManager.Instance.SwitchToDefaultCamera();
        }
    }
    
    /// <summary>
    /// 测试摄像机控制
    /// </summary>
    private void TestCameraControls()
    {
        // 这里可以添加自定义的摄像机控制逻辑
        // 例如，根据游戏状态调整摄像机参数
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // 测试获取摄像机控制器
            var controller = CameraManager.Instance.GetCameraController<AdvancedPlayerCamera>(CameraId.Player);
            if (controller != null)
            {
                // 切换碰撞检测
                bool currentState = controller.GetComponent<AdvancedPlayerCamera>().enabled;
                controller.ToggleCollisionAvoidance(!currentState);
                Debug.Log($"Collision avoidance toggled: {!currentState}");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            // 测试设置摄像机距离
            var controller = CameraManager.Instance.GetCameraController<AdvancedPlayerCamera>(CameraId.Player);
            if (controller != null)
            {
                float newDistance = Random.Range(2f, 8f);
                controller.SetDistance(newDistance);
                Debug.Log($"Camera distance set to: {newDistance}");
            }
        }
    }
    
    /// <summary>
    /// 播放测试摄像机动画
    /// </summary>
    public void PlayTestCameraAnimation()
    {
        // 这里可以创建并播放摄像机动画
        // 例如：
        // CameraAnimation animation = new CameraAnimation();
        // // 设置动画参数
        // CameraManager.Instance.PlayCameraAnimation(animation);
        
        Debug.Log("Test camera animation would play here.");
    }
    
    private void OnDestroy()
    {
        // 清理摄像机管理器
        CameraManager.Instance.Clear();
        Debug.Log("CameraExample destroyed. Camera manager cleared.");
    }
}
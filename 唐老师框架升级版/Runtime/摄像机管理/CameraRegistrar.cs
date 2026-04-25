using UnityEngine;

/// <summary>
/// 摄像机注册组件 - 用于在Unity编辑器中方便地注册摄像机到管理系统
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraRegistrar : MonoBehaviour
{
    [Tooltip("摄像机的唯一标识符")]
    [SerializeField] private CameraId cameraId = CameraId.Player;
    
    [Tooltip("是否将此摄像机设为默认摄像机")]
    [SerializeField] private bool isDefaultCamera = false;
    
    [Tooltip("是否在Awake时自动注册")]
    [SerializeField] private bool registerOnAwake = true;
    
    private Camera cameraComponent;
    private bool isRegistered = false;
    
    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
        
        if (registerOnAwake)
        {
            Register();
        }
    }
    
    /// <summary>
    /// 注册摄像机到管理系统
    /// </summary>
    public void Register()
    {
        if (!isRegistered && CameraManager.Instance != null && cameraComponent != null)
        {
            // 获取摄像机对象上的控制器脚本（如果有）
            AdvancedPlayerCamera cameraController = GetComponent<AdvancedPlayerCamera>();
            
            // 注册摄像机
            CameraManager.Instance.RegisterCamera(cameraId, cameraComponent, cameraController, isDefaultCamera);
            isRegistered = true;
            
            UnityEngine.Debug.Log($"CameraRegistrar: 摄像机已注册 - {cameraId}");
        }
    }
    
    /// <summary>
    /// 从管理系统中注销摄像机
    /// </summary>
    public void Unregister()
    {
        if (isRegistered && CameraManager.Instance != null)
        {
            CameraManager.Instance.UnregisterCamera(cameraId);
            isRegistered = false;
            
            UnityEngine.Debug.Log($"CameraRegistrar: 摄像机已注销 - {cameraId}");
        }
    }
    
    private void OnDestroy()
    {
        Unregister();
    }
    
    /// <summary>
    /// 获取或设置摄像机ID
    /// </summary>
    public CameraId CameraId
    {
        get { return cameraId; }
        set
        {
            if (cameraId != value)
            {
                // 如果已经注册，先注销再重新注册
                if (isRegistered)
                {
                    Unregister();
                    cameraId = value;
                    Register();
                }
                else
                {
                    cameraId = value;
                }
            }
        }
    }
}
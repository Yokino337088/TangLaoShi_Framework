using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摄像机ID枚举 - 用于标识游戏中的所有摄像机，避免字符串拼写错误
/// 可以根据游戏需求扩展更多摄像机ID
/// </summary>
public enum CameraId
{
    /// <summary>
    /// 默认玩家摄像机
    /// </summary>
    Player, 
    
    /// <summary>
    /// 战斗专用摄像机
    /// </summary>
    Combat, 
    
    /// <summary>
    /// 过场动画摄像机
    /// </summary>
    Cutscene, 
    
    /// <summary>
    /// 小地图摄像机
    /// </summary>
    Minimap, 
    
    /// <summary>
    /// 第一人称视角摄像机
    /// </summary>
    FirstPerson, 
    
    /// <summary>
    /// 第三人称视角摄像机
    /// </summary>
    ThirdPerson,
    
    /// <summary>
    /// 策略模式摄像机
    /// </summary>
    Strategy,
    
    /// <summary>
    /// 世界地图摄像机
    /// </summary>
    WorldMap
}

/// <summary>
/// 摄像机管理系统 - 负责管理游戏中的所有摄像机，处理摄像机切换和动画
/// </summary>
public class CameraManager : BaseManager<CameraManager>
{
    // 摄像机字典，存储所有注册的摄像机
    private Dictionary<CameraId, CameraControllerData> cameraDict = new Dictionary<CameraId, CameraControllerData>();
    
    // 当前激活的摄像机ID
    private CameraId activeCameraId = CameraId.Player; // 默认使用Player作为初始值
    
    // 默认摄像机ID
    private CameraId? defaultCameraId = null;
    
    // 摄像机切换的动画时长
    private float transitionDuration = 0.5f;
    
    // 当前是否正在进行摄像机切换
    private bool isTransitioning = false;
    
    // 当前正在播放的摄像机动画
    private CameraAnimation currentAnimation = null;
    
    // 构造函数
    private CameraManager()
    {
        // 在MonoMgr中注册Update监听，用于处理摄像机切换动画
        MonoMgr.Instance.AddUpdateListener(OnUpdate);
    }
    
    /// <summary>
    /// 每帧更新，处理摄像机切换动画
    /// </summary>
    private void OnUpdate()
    {
        // 处理摄像机动画更新
        if (currentAnimation != null && currentAnimation.IsPlaying)
        {
            currentAnimation.Update(Time.deltaTime);
        }
    }
    
    #region 摄像机注册与管理
    /// <summary>
    /// 注册一个摄像机
    /// </summary>
    /// <param name="cameraId">摄像机唯一标识符</param>
    /// <param name="camera">摄像机组件</param>
    /// <param name="controller">摄像机控制器脚本</param>
    /// <param name="isDefault">是否设为默认摄像机</param>
    public void RegisterCamera(CameraId cameraId, Camera camera, MonoBehaviour controller = null, bool isDefault = false)
    {
        if (camera == null)
        {
            LogSystem.Error("CameraManager: 注册摄像机失败 - 摄像机组件为空");
            return;
        }
        
        if (cameraDict.ContainsKey(cameraId))
        {
            LogSystem.Warning($"CameraManager: 已存在相同ID的摄像机，将被覆盖 - {cameraId}");
        }
        
        // 创建摄像机数据并注册
        CameraControllerData data = new CameraControllerData
        {
            CameraId = cameraId,
            CameraComponent = camera,
            Controller = controller,
            IsActive = false
        };
        
        cameraDict[cameraId] = data;
        
        // 设置默认摄像机
        if (isDefault)
        {
            defaultCameraId = cameraId;
        }
        
        // 初始状态下禁用摄像机
        camera.gameObject.SetActive(false);
        
        LogSystem.Info($"CameraManager: 摄像机注册成功 - {cameraId}");
    }
    
    /// <summary>
    /// 注销一个摄像机
    /// </summary>
    /// <param name="cameraId">摄像机唯一标识符</param>
    public void UnregisterCamera(CameraId cameraId)
    {
        if (!cameraDict.ContainsKey(cameraId))
        {
            LogSystem.Warning($"CameraManager: 要注销的摄像机不存在 - {cameraId}");
            return;
        }
        
        // 如果要注销的是当前激活的摄像机，则切换到默认摄像机
        if (cameraId == activeCameraId)
        {
            SwitchToDefaultCamera();
        }
        
        cameraDict.Remove(cameraId);
        
        // 如果注销的是默认摄像机，则清除默认设置
        if (cameraId == defaultCameraId)
        {
            defaultCameraId = null;
        }
        
        LogSystem.Info($"CameraManager: 摄像机注销成功 - {cameraId}");
    }
    
    /// <summary>
    /// 获取指定ID的摄像机
    /// </summary>
    /// <param name="cameraId">摄像机唯一标识符</param>
    /// <returns>摄像机组件，如果不存在则返回null</returns>
    public Camera GetCamera(CameraId cameraId)
    {
        if (!cameraDict.ContainsKey(cameraId))
        {
            return null;
        }
        
        return cameraDict[cameraId].CameraComponent;
    }
    
    /// <summary>
    /// 获取指定ID的摄像机控制器
    /// </summary>
    /// <param name="cameraId">摄像机唯一标识符</param>
    /// <returns>摄像机控制器脚本，如果不存在则返回null</returns>
    public T GetCameraController<T>(CameraId cameraId) where T : MonoBehaviour
    {
        if (!cameraDict.ContainsKey(cameraId))
        {
            return null;
        }
        
        return cameraDict[cameraId].Controller as T;
    }
    #endregion
    
    #region 摄像机切换功能
    /// <summary>
    /// 切换到指定ID的摄像机
    /// </summary>
    /// <param name="cameraId">目标摄像机ID</param>
    /// <param name="instant">是否立即切换（无过渡动画）</param>
    public void SwitchCamera(CameraId cameraId, bool instant = false)
    {
        if (!cameraDict.ContainsKey(cameraId))
        {
            LogSystem.Error($"CameraManager: 无法切换到指定摄像机 - 摄像机ID不存在: {cameraId}");
            return;
        }
        
        if (cameraId == activeCameraId)
        {
            LogSystem.Warning($"CameraManager: 目标摄像机已经是当前激活的摄像机 - {cameraId}");
            return;
        }
        
        if (isTransitioning)
        {
            LogSystem.Warning("CameraManager: 当前正在进行摄像机切换，请等待完成");
            return;
        }
        
        // 如果有正在播放的摄像机动画，停止它
        if (currentAnimation != null)
        {
            currentAnimation.Stop();
            currentAnimation = null;
        }
        
        // 禁用当前激活的摄像机
        if (cameraDict.ContainsKey(activeCameraId))
        {
            cameraDict[activeCameraId].IsActive = false;
            cameraDict[activeCameraId].CameraComponent.gameObject.SetActive(false);
        }
        
        // 激活新的摄像机
        activeCameraId = cameraId;
        cameraDict[cameraId].IsActive = true;
        cameraDict[cameraId].CameraComponent.gameObject.SetActive(true);
        
        LogSystem.Info($"CameraManager: 摄像机切换成功 - {cameraId}");
    }
    
    /// <summary>
    /// 切换到默认摄像机
    /// </summary>
    public void SwitchToDefaultCamera()
    {
        if (defaultCameraId.HasValue)
        {
            SwitchCamera(defaultCameraId.Value);
        }
        else
        {
            LogSystem.Warning("CameraManager: 没有设置默认摄像机");
        }
    }
    
    /// <summary>
    /// 获取当前激活的摄像机ID
    /// </summary>
    /// <returns>当前激活的摄像机ID</returns>
    public CameraId GetActiveCameraId()
    {
        return activeCameraId;
    }
    #endregion
    
    #region 摄像机动画功能
    /// <summary>
    /// 播放摄像机动画
    /// </summary>
    /// <param name="animation">要播放的摄像机动画</param>
    public void PlayCameraAnimation(CameraAnimation animation)
    {
        if (animation == null)
        {
            LogSystem.Error("CameraManager: 无法播放摄像机动画 - 动画对象为空");
            return;
        }
        
        // 如果有正在播放的动画，停止它
        if (currentAnimation != null)
        {
            currentAnimation.Stop();
        }
        
        currentAnimation = animation;
        currentAnimation.Play();
        
        LogSystem.Info("CameraManager: 开始播放摄像机动画");
    }
    
    /// <summary>
    /// 停止当前播放的摄像机动画
    /// </summary>
    public void StopCameraAnimation()
    {
        if (currentAnimation != null)
        {
            currentAnimation.Stop();
            currentAnimation = null;
            LogSystem.Info("CameraManager: 已停止摄像机动画");
        }
    }
    
    /// <summary>
    /// 检查是否正在播放摄像机动画
    /// </summary>
    /// <returns>是否正在播放动画</returns>
    public bool IsCameraAnimationPlaying()
    {
        return currentAnimation != null && currentAnimation.IsPlaying;
    }
    #endregion
    
    #region 设置与配置
    /// <summary>
    /// 设置摄像机切换的过渡动画时长
    /// </summary>
    /// <param name="duration">过渡时长（秒）</param>
    public void SetTransitionDuration(float duration)
    {
        if (duration >= 0f)
        {
            transitionDuration = duration;
        }
        else
        {
            LogSystem.Warning("CameraManager: 过渡时长不能为负数");
        }
    }
    
    /// <summary>
    /// 获取摄像机切换的过渡动画时长
    /// </summary>
    /// <returns>过渡时长（秒）</returns>
    public float GetTransitionDuration()
    {
        return transitionDuration;
    }
    #endregion
    
    /// <summary>
    /// 清理资源
    /// </summary>
    public void Clear()
    {
        // 停止当前动画
        StopCameraAnimation();
        
        // 清空摄像机字典
        cameraDict.Clear();
        
        activeCameraId = CameraId.Player;
        defaultCameraId = null;
        
        // 移除Update监听
        MonoMgr.Instance.RemoveUpdateListener(OnUpdate);
        
        LogSystem.Info("CameraManager: 已清理所有资源");
    }
}

/// <summary>
/// 摄像机控制器数据类 - 存储摄像机及其控制器的相关信息
/// </summary>
public class CameraControllerData
{
    public CameraId CameraId { get; set; }
    public Camera CameraComponent { get; set; }
    public MonoBehaviour Controller { get; set; }
    public bool IsActive { get; set; }
}

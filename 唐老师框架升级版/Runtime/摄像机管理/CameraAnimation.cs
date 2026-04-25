using UnityEngine;

/// <summary>
/// 摄像机动画基类 - 提供摄像机动画的基础功能
/// </summary>
public abstract class CameraAnimation
{
    // 动画是否正在播放
    public bool IsPlaying { get; protected set; } = false;
    
    // 动画持续时间
    protected float duration = 1.0f;
    
    // 动画已播放时间
    protected float elapsedTime = 0.0f;
    
    // 动画曲线，用于控制动画的缓动效果
    protected AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // 目标摄像机
    protected Camera targetCamera;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="camera">目标摄像机</param>
    /// <param name="duration">动画持续时间</param>
    public CameraAnimation(Camera camera, float duration = 1.0f)
    {
        this.targetCamera = camera;
        this.duration = Mathf.Max(0.01f, duration); // 确保持续时间不为0
    }
    
    /// <summary>
    /// 开始播放动画
    /// </summary>
    public virtual void Play()
    {
        if (targetCamera == null)
        {
            LogSystem.Error("CameraAnimation: 无法播放动画 - 目标摄像机为空");
            return;
        }
        
        IsPlaying = true;
        elapsedTime = 0.0f;
    }
    
    /// <summary>
    /// 更新动画
    /// </summary>
    /// <param name="deltaTime">帧时间</param>
    public virtual void Update(float deltaTime)
    {
        if (!IsPlaying || targetCamera == null)
            return;
        
        // 更新已播放时间
        elapsedTime += deltaTime;
        
        // 计算动画进度（0-1）
        float progress = Mathf.Clamp01(elapsedTime / duration);
        
        // 应用动画曲线
        float curvedProgress = animationCurve.Evaluate(progress);
        
        // 执行具体的动画逻辑
        Animate(curvedProgress);
        
        // 检查动画是否结束
        if (progress >= 1.0f)
        {
            OnAnimationComplete();
        }
    }
    
    /// <summary>
    /// 停止动画
    /// </summary>
    public virtual void Stop()
    {
        IsPlaying = false;
        elapsedTime = 0.0f;
    }
    
    /// <summary>
    /// 设置动画曲线
    /// </summary>
    /// <param name="curve">动画曲线</param>
    public void SetAnimationCurve(AnimationCurve curve)
    {
        if (curve != null)
        {
            this.animationCurve = curve;
        }
    }
    
    /// <summary>
    /// 具体的动画逻辑，由子类实现
    /// </summary>
    /// <param name="progress">动画进度（0-1）</param>
    protected abstract void Animate(float progress);
    
    /// <summary>
    /// 动画完成时的回调
    /// </summary>
    protected virtual void OnAnimationComplete()
    {
        IsPlaying = false;
    }
}

/// <summary>
/// 位置动画 - 控制摄像机从一个位置移动到另一个位置
/// </summary>
public class CameraMoveAnimation : CameraAnimation
{
    private Vector3 startPosition;
    private Vector3 targetPosition;
    
    public CameraMoveAnimation(Camera camera, Vector3 targetPosition, float duration = 1.0f)
        : base(camera, duration)
    {
        this.targetPosition = targetPosition;
    }
    
    public override void Play()
    {
        if (targetCamera != null)
        {
            startPosition = targetCamera.transform.position;
        }
        base.Play();
    }
    
    protected override void Animate(float progress)
    {
        if (targetCamera != null)
        {
            targetCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
        }
    }
}

/// <summary>
/// 旋转动画 - 控制摄像机从一个旋转状态过渡到另一个旋转状态
/// </summary>
public class CameraRotateAnimation : CameraAnimation
{
    private Quaternion startRotation;
    private Quaternion targetRotation;
    
    public CameraRotateAnimation(Camera camera, Quaternion targetRotation, float duration = 1.0f)
        : base(camera, duration)
    {
        this.targetRotation = targetRotation;
    }
    
    public override void Play()
    {
        if (targetCamera != null)
        {
            startRotation = targetCamera.transform.rotation;
        }
        base.Play();
    }
    
    protected override void Animate(float progress)
    {
        if (targetCamera != null)
        {
            targetCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
        }
    }
}

/// <summary>
/// FOV动画 - 控制摄像机的FOV变化
/// </summary>
public class CameraFOVAnimation : CameraAnimation
{
    private float startFOV;
    private float targetFOV;
    
    public CameraFOVAnimation(Camera camera, float targetFOV, float duration = 1.0f)
        : base(camera, duration)
    {
        this.targetFOV = targetFOV;
    }
    
    public override void Play()
    {
        if (targetCamera != null)
        {
            startFOV = targetCamera.fieldOfView;
        }
        base.Play();
    }
    
    protected override void Animate(float progress)
    {
        if (targetCamera != null)
        {
            targetCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, progress);
        }
    }
}

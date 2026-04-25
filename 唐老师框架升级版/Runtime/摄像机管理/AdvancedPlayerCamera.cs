using UnityEngine;

/// <summary>
/// 高级角色摄像机控制器 - 参考3A大作摄像机控制标准
/// 挂载在角色的摄像机对象上，提供流畅的第三人称/第一人称视角控制
/// </summary>
public class AdvancedPlayerCamera : MonoBehaviour
{
    #region 核心配置变量
    [Header("核心设置")]
    [Tooltip("摄像机跟随的目标角色")]
    [SerializeField] private Transform target;
    
    [Tooltip("摄像机与角色的初始距离")]
    [SerializeField] private float distance = 5.0f;
    
    [Tooltip("摄像机与角色的最大距离")]
    [SerializeField] private float maxDistance = 10.0f;
    
    [Tooltip("摄像机与角色的最小距离")]
    [SerializeField] private float minDistance = 1.0f;
    
    [Tooltip("垂直视角的角度")]
    [SerializeField] private float verticalAngle = 30.0f;
    
    [Tooltip("垂直视角的最大角度")]
    [SerializeField] private float maxVerticalAngle = 70.0f;
    
    [Tooltip("垂直视角的最小角度")]
    [SerializeField] private float minVerticalAngle = -10.0f;
    
    [Tooltip("水平视角的角度")]
    [SerializeField] private float horizontalAngle = 0.0f;
    
    [Tooltip("目标在屏幕上的偏移位置")]
    [SerializeField] private Vector3 targetScreenOffset = Vector3.zero;
    #endregion
    
    #region 平滑度与响应速度设置
    [Header("平滑度与响应设置")]
    [Tooltip("移动平滑度系数")]
    [SerializeField] private float moveSmoothness = 5.0f;
    
    [Tooltip("旋转平滑度系数")]
    [SerializeField] private float rotationSmoothness = 5.0f;
    
    [Tooltip("缩放平滑度系数")]
    [SerializeField] private float zoomSmoothness = 3.0f;
    
    [Tooltip("相机旋转灵敏度")]
    [SerializeField] private float rotationSensitivity = 2.0f;
    
    [Tooltip("相机缩放灵敏度")]
    [SerializeField] private float zoomSensitivity = 2.0f;
    #endregion
    
    #region 碰撞检测设置
    [Header("碰撞检测设置")]
    [Tooltip("是否启用碰撞检测")]
    [SerializeField] private bool enableCollisionAvoidance = true;
    
    [Tooltip("碰撞检测的层级掩码")]
    [SerializeField] private LayerMask collisionLayerMask = ~0;
    
    [Tooltip("碰撞检测的射线半径")]
    [SerializeField] private float collisionRadius = 0.2f;
    
    [Tooltip("碰撞检测的额外偏移量")]
    [SerializeField] private float collisionOffset = 0.1f;
    #endregion
    
    #region 高级功能设置
    [Header("高级功能设置")]
    [Tooltip("是否启用FOV动态调整")]
    [SerializeField] private bool enableDynamicFOV = true;
    
    [Tooltip("基础FOV值")]
    [SerializeField] private float baseFOV = 60.0f;
    
    [Tooltip("奔跑时的FOV值")]
    [SerializeField] private float sprintFOV = 70.0f;
    
    [Tooltip("FOV平滑过渡速度")]
    [SerializeField] private float fovSmoothSpeed = 5.0f;
    
    [Tooltip("第一人称模式切换键")]
    [SerializeField] private KeyCode firstPersonToggleKey = KeyCode.V;
    
    [Tooltip("是否启用自动视角回复")]
    [SerializeField] private bool enableAutoCorrection = false;
    
    [Tooltip("自动视角回复速度")]
    [SerializeField] private float autoCorrectionSpeed = 1.0f;
    #endregion
    
    // 私有变量
    private float targetDistance;
    private float currentFOV;
    private Camera playerCamera;
    private bool isFirstPersonMode = false;
    private Vector3 previousTargetPosition;
    private Quaternion targetRotation;
    private Vector3 desiredPosition;
    private bool hasTarget = false;
    
    /// <summary>
    /// 初始化摄像机组件
    /// </summary>
    private void Start()
    {
        // 获取摄像机组件
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
        {
            playerCamera = gameObject.AddComponent<Camera>();
        }
        
        // 设置初始值
        targetDistance = distance;
        currentFOV = baseFOV;
        
        // 检查并设置目标
        if (target == null)
        {
            // 如果没有设置目标，尝试查找标签为Player的对象
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                hasTarget = true;
            }
        }
        else
        {
            hasTarget = true;
        }
        
        if (hasTarget)
        {
            previousTargetPosition = target.position;
            // 初始化摄像机位置
            UpdateCameraPosition();
        }
        
        // 隐藏并锁定光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    /// <summary>
    /// 每一帧更新摄像机状态
    /// </summary>
    private void Update()
    {
        if (!hasTarget)
            return;
        
        // 处理输入
        HandleInput();
        
        // 处理动态FOV
        if (enableDynamicFOV)
        {
            UpdateDynamicFOV();
        }
        
        // 更新摄像机位置
        UpdateCameraPosition();
    }
    
    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandleInput()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * rotationSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSensitivity;
        
        // 更新水平角度
        horizontalAngle += mouseX;
        if (horizontalAngle > 360f)
            horizontalAngle -= 360f;
        if (horizontalAngle < -360f)
            horizontalAngle += 360f;
        
        // 更新垂直角度并限制范围
        verticalAngle -= mouseY;
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
        
        // 处理滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (!isFirstPersonMode)
        {
            targetDistance = Mathf.Clamp(targetDistance - scroll * zoomSensitivity, minDistance, maxDistance);
        }
        
        // 切换第一人称/第三人称模式
        if (Input.GetKeyDown(firstPersonToggleKey))
        {
            ToggleFirstPersonMode();
        }
    }
    
    /// <summary>
    /// 切换第一人称/第三人称模式
    /// </summary>
    private void ToggleFirstPersonMode()
    {
        isFirstPersonMode = !isFirstPersonMode;
        
        if (isFirstPersonMode)
        {
            // 进入第一人称模式
            targetDistance = 0.1f;
            // 可以在这里添加第一人称视角特有的设置
        }
        else
        {
            // 退出第一人称模式，恢复原来的距离
            targetDistance = distance;
        }
    }
    
    /// <summary>
    /// 更新动态FOV
    /// </summary>
    private void UpdateDynamicFOV()
    {
        // 根据角色移动速度调整FOV（示例：按下Shift键时表示奔跑）
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float targetFOV = isSprinting ? sprintFOV : baseFOV;
        
        // 平滑过渡FOV
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovSmoothSpeed);
        playerCamera.fieldOfView = currentFOV;
    }
    
    /// <summary>
    /// 更新摄像机位置和旋转
    /// </summary>
    private void UpdateCameraPosition()
    {
        // 计算目标旋转
        targetRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        
        // 计算目标位置
        Vector3 direction = targetRotation * Vector3.back * targetDistance;
        desiredPosition = target.position + direction;
        
        // 应用屏幕偏移
        ApplyScreenOffset();
        
        // 处理碰撞检测
        if (enableCollisionAvoidance && !isFirstPersonMode)
        {
            HandleCollisionAvoidance();
        }
        
        // 平滑移动到目标位置
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * moveSmoothness);
        
        // 平滑旋转到目标旋转
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothness);
        
        // 记录当前目标位置用于下一帧计算
        previousTargetPosition = target.position;
    }
    
    /// <summary>
    /// 应用屏幕偏移量
    /// </summary>
    private void ApplyScreenOffset()
    {
        // 将屏幕空间偏移转换为世界空间偏移
        Vector3 worldOffset = transform.right * targetScreenOffset.x + transform.up * targetScreenOffset.y;
        desiredPosition += worldOffset;
    }
    
    /// <summary>
    /// 处理摄像机碰撞规避
    /// </summary>
    private void HandleCollisionAvoidance()
    {
        // 计算从目标到理想摄像机位置的射线
        Vector3 rayOrigin = target.position;
        Vector3 rayDirection = desiredPosition - rayOrigin;
        
        RaycastHit hit;
        // 使用SphereCast来检测碰撞
        if (Physics.SphereCast(rayOrigin, collisionRadius, rayDirection, out hit, rayDirection.magnitude, collisionLayerMask))
        {
            // 如果检测到碰撞，调整摄像机位置到碰撞点前一点
            desiredPosition = hit.point - rayDirection.normalized * collisionOffset;
        }
    }
    
    #region 公共API方法
    /// <summary>
    /// 设置摄像机目标
    /// </summary>
    /// <param name="newTarget">新的目标Transform</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        hasTarget = target != null;
        if (hasTarget)
        {
            previousTargetPosition = target.position;
        }
    }
    
    /// <summary>
    /// 立即重置摄像机位置
    /// </summary>
    public void ResetPosition()
    {
        if (hasTarget)
        {
            UpdateCameraPosition();
            transform.position = desiredPosition;
            transform.rotation = targetRotation;
        }
    }
    
    /// <summary>
    /// 设置摄像机距离
    /// </summary>
    /// <param name="newDistance">新的距离值</param>
    public void SetDistance(float newDistance)
    {
        distance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        if (!isFirstPersonMode)
        {
            targetDistance = distance;
        }
    }
    
    /// <summary>
    /// 切换碰撞检测状态
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public void ToggleCollisionAvoidance(bool enabled)
    {
        enableCollisionAvoidance = enabled;
    }
    #endregion
}
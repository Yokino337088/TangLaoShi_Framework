using UnityEngine;

/// <summary>
/// 输入管理示例
/// 演示如何使用输入管理器的各种功能
/// </summary>
public class InputExample : MonoBehaviour
{
    private InputConfig inputConfig;
    private bool showInputInfo = false;
    
    private void Start()
    {
        // 初始化输入配置
        InitializeInputConfig();
        
        // 绑定输入事件
        BindInputEvents();
        
        // 监听触摸事件
        ListenToTouchEvents();
        
        // 监听轴输入事件
        ListenToAxisEvents();
        
        Debug.Log("InputExample initialized. Press 'I' to toggle input info display.");
    }
    
    private void Update()
    {
        // 检查调试输入
        if (Input.GetKeyDown(KeyCode.I))
        {
            showInputInfo = !showInputInfo;
            Debug.Log($"Input info display: {showInputInfo}");
        }
        
        // 检查输入状态
        CheckInputStates();
    }
    
    private void OnGUI()
    {
        if (showInputInfo)
        {
            GUI.Box(new Rect(10, 10, 300, 200), "Input Info");
            
            // 显示轴输入
            float horizontal = InputMgr.Instance.GetHorizontalAxis();
            float vertical = InputMgr.Instance.GetVerticalAxis();
            GUI.Label(new Rect(20, 40, 280, 20), $"Horizontal: {horizontal:F2}");
            GUI.Label(new Rect(20, 60, 280, 20), $"Vertical: {vertical:F2}");
            
            // 显示绑定信息
            GUI.Label(new Rect(20, 80, 280, 20), "Jump: " + InputMgr.Instance.GetBindingInfo(E_EventType.E_Input_Jump));
            GUI.Label(new Rect(20, 100, 280, 20), "Attack: " + InputMgr.Instance.GetBindingInfo(E_EventType.E_Input_Attack));
            GUI.Label(new Rect(20, 120, 280, 20), "Special: " + InputMgr.Instance.GetBindingInfo(E_EventType.E_Input_Special));
            
            // 显示输入状态
            GUI.Label(new Rect(20, 140, 280, 20), "Jump pressed: " + InputMgr.Instance.IsInputPressed(E_EventType.E_Input_Jump));
            GUI.Label(new Rect(20, 160, 280, 20), "Attack pressed: " + InputMgr.Instance.IsInputPressed(E_EventType.E_Input_Attack));
        }
    }
    
    /// <summary>
    /// 初始化输入配置
    /// </summary>
    private void InitializeInputConfig()
    {
        // 创建默认配置
        inputConfig = InputConfig.CreateDefaultConfig();
        
        // 自定义配置
        inputConfig.AddKeyboardBinding(E_EventType.E_Input_Menu, KeyCode.Escape, InputInfo.InputState.Down);
        inputConfig.AddKeyboardBinding(E_EventType.E_Input_Pause, KeyCode.P, InputInfo.InputState.Down);
        
        // 应用配置
        inputConfig.ApplyToInputMgr();
        
        Debug.Log("Input config initialized and applied.");
    }
    
    /// <summary>
    /// 绑定输入事件
    /// </summary>
    private void BindInputEvents()
    {
        // 绑定跳跃事件
        Architecture.Instance.AddEventListener(E_EventType.E_Input_Jump, OnJump);
        
        // 绑定攻击事件
        Architecture.Instance.AddEventListener(E_EventType.E_Input_Attack, OnAttack);
        
        // 绑定特殊技能事件
        Architecture.Instance.AddEventListener(E_EventType.E_Input_Special, OnSpecial);
        
        // 绑定菜单事件
        Architecture.Instance.AddEventListener(E_EventType.E_Input_Menu, OnMenu);
        
        // 绑定暂停事件
        Architecture.Instance.AddEventListener(E_EventType.E_Input_Pause, OnPause);
        
        Debug.Log("Input events bound.");
    }
    
    /// <summary>
    /// 监听触摸事件
    /// </summary>
    private void ListenToTouchEvents()
    {
        // 触摸开始
        InputMgr.Instance.OnTouchDown += OnTouchDown;
        
        // 触摸结束
        InputMgr.Instance.OnTouchUp += OnTouchUp;
        
        // 触摸移动
        InputMgr.Instance.OnTouchMove += OnTouchMove;
        
        // 滑动事件
        InputMgr.Instance.OnSwipe += OnSwipe;
        
        // 点击事件
        InputMgr.Instance.OnTap += OnTap;
        
        // 双击事件
        InputMgr.Instance.OnDoubleTap += OnDoubleTap;
        
        Debug.Log("Touch events listeners added.");
    }
    
    /// <summary>
    /// 监听轴输入事件
    /// </summary>
    private void ListenToAxisEvents()
    {
        // 水平轴变化
        InputMgr.Instance.OnHorizontalAxisChanged += OnHorizontalAxisChanged;
        
        // 垂直轴变化
        InputMgr.Instance.OnVerticalAxisChanged += OnVerticalAxisChanged;
        
        // 绑定轴输入事件
        Architecture.Instance.AddEventListener<float>(E_EventType.E_Input_Horizontal, OnHorizontalInput);
        Architecture.Instance.AddEventListener<float>(E_EventType.E_Input_Vertical, OnVerticalInput);
        
        Debug.Log("Axis events listeners added.");
    }
    
    /// <summary>
    /// 检查输入状态
    /// </summary>
    private void CheckInputStates()
    {
        // 检查方向键输入
        if (InputMgr.Instance.IsInputPressed(E_EventType.E_Input_Up))
        {
            // 处理向上输入
        }
        
        if (InputMgr.Instance.IsInputPressed(E_EventType.E_Input_Down))
        {
            // 处理向下输入
        }
        
        if (InputMgr.Instance.IsInputPressed(E_EventType.E_Input_Left))
        {
            // 处理向左输入
        }
        
        if (InputMgr.Instance.IsInputPressed(E_EventType.E_Input_Right))
        {
            // 处理向右输入
        }
    }
    
    #region 输入事件处理
    
    private void OnJump()
    {
        Debug.Log("Jump input detected!");
        // 处理跳跃逻辑
    }
    
    private void OnAttack()
    {
        Debug.Log("Attack input detected!");
        // 处理攻击逻辑
    }
    
    private void OnSpecial()
    {
        Debug.Log("Special input detected!");
        // 处理特殊技能逻辑
    }
    
    private void OnMenu()
    {
        Debug.Log("Menu input detected!");
        // 处理菜单逻辑
    }
    
    private void OnPause()
    {
        Debug.Log("Pause input detected!");
        // 处理暂停逻辑
    }
    
    private void OnTouchDown(Vector2 position)
    {
        Debug.Log($"Touch down at: {position}");
    }
    
    private void OnTouchUp(Vector2 position)
    {
        Debug.Log($"Touch up at: {position}");
    }
    
    private void OnTouchMove(Vector2 position)
    {
        // 可以在这里处理触摸移动逻辑
    }
    
    private void OnSwipe(InputInfo.SwipeDirection direction, Vector2 delta)
    {
        Debug.Log($"Swipe detected: {direction}, Delta: {delta}");
        // 处理滑动逻辑
        switch (direction)
        {
            case InputInfo.SwipeDirection.Up:
                // 向上滑动
                break;
            case InputInfo.SwipeDirection.Down:
                // 向下滑动
                break;
            case InputInfo.SwipeDirection.Left:
                // 向左滑动
                break;
            case InputInfo.SwipeDirection.Right:
                // 向右滑动
                break;
        }
    }
    
    private void OnTap(Vector2 position)
    {
        Debug.Log($"Tap detected at: {position}");
        // 处理点击逻辑
    }
    
    private void OnDoubleTap(Vector2 position)
    {
        Debug.Log($"Double tap detected at: {position}");
        // 处理双击逻辑
    }
    
    private void OnHorizontalAxisChanged(float value)
    {
        Debug.Log($"Horizontal axis changed: {value}");
        // 处理水平轴输入变化
    }
    
    private void OnVerticalAxisChanged(float value)
    {
        Debug.Log($"Vertical axis changed: {value}");
        // 处理垂直轴输入变化
    }
    
    private void OnHorizontalInput(float value)
    {
        // 处理水平输入事件
    }
    
    private void OnVerticalInput(float value)
    {
        // 处理垂直输入事件
    }
    
    #endregion
    
    private void OnDestroy()
    {
        // 移除事件监听
        Architecture.Instance.RemoveEventListener(E_EventType.E_Input_Jump, OnJump);
        Architecture.Instance.RemoveEventListener(E_EventType.E_Input_Attack, OnAttack);
        Architecture.Instance.RemoveEventListener(E_EventType.E_Input_Special, OnSpecial);
        Architecture.Instance.RemoveEventListener(E_EventType.E_Input_Menu, OnMenu);
        Architecture.Instance.RemoveEventListener(E_EventType.E_Input_Pause, OnPause);
        
        // 移除触摸事件监听
        InputMgr.Instance.OnTouchDown -= OnTouchDown;
        InputMgr.Instance.OnTouchUp -= OnTouchUp;
        InputMgr.Instance.OnTouchMove -= OnTouchMove;
        InputMgr.Instance.OnSwipe -= OnSwipe;
        InputMgr.Instance.OnTap -= OnTap;
        InputMgr.Instance.OnDoubleTap -= OnDoubleTap;
        
        // 移除轴输入事件监听
        InputMgr.Instance.OnHorizontalAxisChanged -= OnHorizontalAxisChanged;
        InputMgr.Instance.OnVerticalAxisChanged -= OnVerticalAxisChanged;
        
        Architecture.Instance.RemoveEventListener<float>(E_EventType.E_Input_Horizontal, OnHorizontalInput);
        Architecture.Instance.RemoveEventListener<float>(E_EventType.E_Input_Vertical, OnVerticalInput);
        
        Debug.Log("InputExample destroyed. Event listeners removed.");
    }
}
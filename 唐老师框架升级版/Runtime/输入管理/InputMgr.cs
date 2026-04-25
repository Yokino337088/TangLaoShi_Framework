using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 输入管理器
/// 统一管理键盘、鼠标、触摸输入
/// </summary>
public class InputMgr : BaseManager<InputMgr>
{
    #region 数据结构

    /// <summary>
    /// 输入绑定结构
    /// </summary>
    private class InputBinding
    {
        public InputInfo.InputType Type { get; set; }
        public InputInfo.InputState State { get; set; }
        public KeyCode Key { get; set; }        // 键盘
        public int MouseButton { get; set; }   // 鼠标
        public int FingerId { get; set; }      // 触摸
    }

    /// <summary>
    /// 触摸状态结构
    /// </summary>
    private class TouchState
    {
        public int FingerId { get; set; }
        public Vector2 StartPosition { get; set; }
        public Vector2 CurrentPosition { get; set; }
        public Vector2 LastPosition { get; set; }
        public float StartTime { get; set; }
        public InputInfo.SwipeDirection Direction { get; set; }
    }

    #endregion

    #region 字段

    // 输入绑定字典
    private Dictionary<E_EventType, InputBinding> _inputBindings = new Dictionary<E_EventType, InputBinding>();

    // 触摸状态管理
    private Dictionary<int, TouchState> _touchStates = new Dictionary<int, TouchState>();
    
    // 滑动检测设置
    private float _swipeThreshold = 50f;
    private float _doubleTapThreshold = 0.3f;
    private float _lastTapTime = 0f;
    private Vector2 _lastTapPosition = Vector2.zero;

    // 输入状态
    private bool _isEnabled = true;
    private bool _isCheckingInput = false;
    private Action<InputInfo> _inputCheckCallback;

    // 输入轴
    private float _horizontalAxis = 0f;
    private float _verticalAxis = 0f;

    #endregion

    #region 事件

    // 触摸事件
    public Action<Vector2> OnTouchDown;
    public Action<Vector2> OnTouchUp;
    public Action<Vector2> OnTouchMove;
    public Action<Vector2> OnTouchStationary;
    
    // 滑动事件
    public Action<InputInfo.SwipeDirection, Vector2> OnSwipe;
    
    // 点击事件
    public Action<Vector2> OnTap;
    public Action<Vector2> OnDoubleTap;
    
    // 轴输入事件
    public Action<float> OnHorizontalAxisChanged;
    public Action<float> OnVerticalAxisChanged;

    #endregion

    #region 初始化

    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(UpdateInput);
        LogSystem.Info("InputMgr initialized");
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 启用或禁用输入管理器
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        _isEnabled = enabled;

    }

    /// <summary>
    /// 绑定键盘输入
    /// </summary>
    public void BindKeyboard(E_EventType eventType, KeyCode key, InputInfo.InputState state)
    {
        if (!_inputBindings.ContainsKey(eventType))
        {
            _inputBindings[eventType] = new InputBinding();
        }
        
        var binding = _inputBindings[eventType];
        binding.Type = InputInfo.InputType.Keyboard;
        binding.State = state;
        binding.Key = key;
        
        LogSystem.Info($"Bound keyboard input: {eventType} -> {key} ({state})");
    }

    /// <summary>
    /// 绑定鼠标输入
    /// </summary>
    public void BindMouse(E_EventType eventType, int mouseButton, InputInfo.InputState state)
    {
        if (!_inputBindings.ContainsKey(eventType))
        {
            _inputBindings[eventType] = new InputBinding();
        }
        
        var binding = _inputBindings[eventType];
        binding.Type = InputInfo.InputType.Mouse;
        binding.State = state;
        binding.MouseButton = mouseButton;
        
        LogSystem.Info($"Bound mouse input: {eventType} -> Button {mouseButton} ({state})");
    }

    /// <summary>
    /// 绑定触摸输入
    /// </summary>
    public void BindTouch(E_EventType eventType, int fingerId, InputInfo.InputState state)
    {
        if (!_inputBindings.ContainsKey(eventType))
        {
            _inputBindings[eventType] = new InputBinding();
        }
        
        var binding = _inputBindings[eventType];
        binding.Type = InputInfo.InputType.Touch;
        binding.State = state;
        binding.FingerId = fingerId;
        
        LogSystem.Info($"Bound touch input: {eventType} -> Finger {fingerId} ({state})");
    }

    /// <summary>
    /// 移除输入绑定
    /// </summary>
    public void UnbindInput(E_EventType eventType)
    {
        if (_inputBindings.Remove(eventType))
        {
            LogSystem.Info($"Unbound input: {eventType}");
        }
    }

    /// <summary>
    /// 清除所有输入绑定
    /// </summary>
    public void ClearAllBindings()
    {
        _inputBindings.Clear();
        LogSystem.Info("Cleared all input bindings");
    }

    /// <summary>
    /// 获取输入信息
    /// </summary>
    public void GetInputInfo(Action<InputInfo> callback)
    {
        _inputCheckCallback = callback;
        _isCheckingInput = true;
        LogSystem.Info("Started input checking");
    }

    /// <summary>
    /// 设置滑动阈值
    /// </summary>
    public void SetSwipeThreshold(float threshold)
    {
        _swipeThreshold = Mathf.Max(10f, threshold);
        LogSystem.Info($"Set swipe threshold to {_swipeThreshold}");
    }

    /// <summary>
    /// 获取当前轴输入
    /// </summary>
    public float GetHorizontalAxis() => _horizontalAxis;
    public float GetVerticalAxis() => _verticalAxis;

    #endregion

    #region 输入更新

    private void UpdateInput()
    {
        if (!_isEnabled)
            return;

        // 检查输入绑定
        CheckInputBindings();
        
        // 处理触摸输入
        HandleTouchInput();
        
        // 处理轴输入
        HandleAxisInput();
    }

    /// <summary>
    /// 检查输入绑定
    /// </summary>
    private void CheckInputBindings()
    {
        foreach (var binding in _inputBindings)
        {
            E_EventType eventType = binding.Key;
            InputBinding inputBinding = binding.Value;
            
            switch (inputBinding.Type)
            {
                case InputInfo.InputType.Keyboard:
                    CheckKeyboardInput(eventType, inputBinding);
                    break;
                case InputInfo.InputType.Mouse:
                    CheckMouseInput(eventType, inputBinding);
                    break;
                case InputInfo.InputType.Touch:
                    // 触摸输入在HandleTouchInput中处理
                    break;
            }
        }
    }

    /// <summary>
    /// 检查键盘输入
    /// </summary>
    private void CheckKeyboardInput(E_EventType eventType, InputBinding binding)
    {
        bool isTriggered = false;
        
        switch (binding.State)
        {
            case InputInfo.InputState.Down:
                isTriggered = Input.GetKeyDown(binding.Key);
                break;
            case InputInfo.InputState.Up:
                isTriggered = Input.GetKeyUp(binding.Key);
                break;
            case InputInfo.InputState.Hold:
                isTriggered = Input.GetKey(binding.Key);
                break;
        }
        
        if (isTriggered)
        {
            EventCenter.Instance.EventTrigger(eventType);
        }
    }

    /// <summary>
    /// 检查鼠标输入
    /// </summary>
    private void CheckMouseInput(E_EventType eventType, InputBinding binding)
    {
        bool isTriggered = false;
        
        switch (binding.State)
        {
            case InputInfo.InputState.Down:
                isTriggered = Input.GetMouseButtonDown(binding.MouseButton);
                break;
            case InputInfo.InputState.Up:
                isTriggered = Input.GetMouseButtonUp(binding.MouseButton);
                break;
            case InputInfo.InputState.Hold:
                isTriggered = Input.GetMouseButton(binding.MouseButton);
                break;
        }
        
        if (isTriggered)
        {
            EventCenter.Instance.EventTrigger(eventType);
        }
    }

    /// <summary>
    /// 处理触摸输入
    /// </summary>
    private void HandleTouchInput()
    {
        // 检查输入检测
        if (_isCheckingInput && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var inputInfo = new InputInfo(
                    InputInfo.InputState.Down,
                    touch.fingerId,
                    touch.phase,
                    touch.position
                );
                _inputCheckCallback?.Invoke(inputInfo);
                _inputCheckCallback = null;
                _isCheckingInput = false;
            }
        }
        
        // 处理所有触摸
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchBegan(touch);
                    break;
                case TouchPhase.Moved:
                    HandleTouchMoved(touch);
                    break;
                case TouchPhase.Stationary:
                    HandleTouchStationary(touch);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    HandleTouchEnded(touch);
                    break;
            }
        }
    }

    /// <summary>
    /// 处理触摸开始
    /// </summary>
    private void HandleTouchBegan(Touch touch)
    {
        // 创建触摸状态
        TouchState state = new TouchState
        {
            FingerId = touch.fingerId,
            StartPosition = touch.position,
            CurrentPosition = touch.position,
            LastPosition = touch.position,
            StartTime = Time.time,
            Direction = InputInfo.SwipeDirection.None
        };
        
        _touchStates[touch.fingerId] = state;
        
        // 触发事件
        OnTouchDown?.Invoke(touch.position);
        
        // 检查双击
        CheckDoubleTap(touch.position);
    }

    /// <summary>
    /// 处理触摸移动
    /// </summary>
    private void HandleTouchMoved(Touch touch)
    {
        if (_touchStates.TryGetValue(touch.fingerId, out TouchState state))
        {
            state.LastPosition = state.CurrentPosition;
            state.CurrentPosition = touch.position;
            
            // 触发移动事件
            OnTouchMove?.Invoke(touch.position);
        }
    }

    /// <summary>
    /// 处理触摸静止
    /// </summary>
    private void HandleTouchStationary(Touch touch)
    {
        OnTouchStationary?.Invoke(touch.position);
    }

    /// <summary>
    /// 处理触摸结束
    /// </summary>
    private void HandleTouchEnded(Touch touch)
    {
        if (_touchStates.TryGetValue(touch.fingerId, out TouchState state))
        {
            // 计算滑动
            Vector2 delta = touch.position - state.StartPosition;
            float duration = Time.time - state.StartTime;
            
            // 检查滑动
            if (delta.magnitude >= _swipeThreshold)
            {
                state.Direction = InputInfo.CalculateSwipeDirection(delta, _swipeThreshold);
                OnSwipe?.Invoke(state.Direction, delta);
            }
            // 检查点击
            else if (duration < 0.3f)
            {
                OnTap?.Invoke(touch.position);
            }
            
            // 触发结束事件
            OnTouchUp?.Invoke(touch.position);
            
            // 清理触摸状态
            _touchStates.Remove(touch.fingerId);
        }
    }

    /// <summary>
    /// 检查双击
    /// </summary>
    private void CheckDoubleTap(Vector2 position)
    {
        float timeSinceLastTap = Time.time - _lastTapTime;
        float distance = Vector2.Distance(position, _lastTapPosition);
        
        if (timeSinceLastTap < _doubleTapThreshold && distance < 50f)
        {
            OnDoubleTap?.Invoke(position);
            _lastTapTime = 0f;
            _lastTapPosition = Vector2.zero;
        }
        else
        {
            _lastTapTime = Time.time;
            _lastTapPosition = position;
        }
    }

    /// <summary>
    /// 处理轴输入
    /// </summary>
    private void HandleAxisInput()
    {
        float newHorizontal = Input.GetAxis("Horizontal");
        float newVertical = Input.GetAxis("Vertical");
        
        // 触发轴输入事件
        if (Mathf.Abs(newHorizontal - _horizontalAxis) > 0.01f)
        {
            _horizontalAxis = newHorizontal;
            OnHorizontalAxisChanged?.Invoke(_horizontalAxis);
            EventCenter.Instance.EventTrigger(E_EventType.E_Input_Horizontal, _horizontalAxis);
        }
        
        if (Mathf.Abs(newVertical - _verticalAxis) > 0.01f)
        {
            _verticalAxis = newVertical;
            OnVerticalAxisChanged?.Invoke(_verticalAxis);
            EventCenter.Instance.EventTrigger(E_EventType.E_Input_Vertical, _verticalAxis);
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 获取输入绑定信息
    /// </summary>
    public string GetBindingInfo(E_EventType eventType)
    {
        if (_inputBindings.TryGetValue(eventType, out InputBinding binding))
        {
            switch (binding.Type)
            {
                case InputInfo.InputType.Keyboard:
                    return $"Keyboard: {binding.Key} ({binding.State})";
                case InputInfo.InputType.Mouse:
                    return $"Mouse: Button {binding.MouseButton} ({binding.State})";
                case InputInfo.InputType.Touch:
                    return $"Touch: Finger {binding.FingerId} ({binding.State})";
                default:
                    return "Unknown";
            }
        }
        return "Not bound";
    }

    /// <summary>
    /// 检查输入是否按下
    /// </summary>
    public bool IsInputPressed(E_EventType eventType)
    {
        if (_inputBindings.TryGetValue(eventType, out InputBinding binding))
        {
            switch (binding.Type)
            {
                case InputInfo.InputType.Keyboard:
                    return Input.GetKey(binding.Key);
                case InputInfo.InputType.Mouse:
                    return Input.GetMouseButton(binding.MouseButton);
                default:
                    return false;
            }
        }
        return false;
    }

    #endregion
}

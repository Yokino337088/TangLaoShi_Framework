using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入配置类
/// 管理输入绑定的配置
/// </summary>
[Serializable]
public class InputConfig
{
    /// <summary>
    /// 输入绑定配置
    /// </summary>
    [Serializable]
    public class BindingConfig
    {
        /// <summary>事件类型</summary>
        public E_EventType eventType;
        /// <summary>输入类型</summary>
        public InputInfo.InputType inputType;
        /// <summary>输入状态</summary>
        public InputInfo.InputState inputState;
        /// <summary>键盘按键</summary>
        public KeyCode keyCode;
        /// <summary>鼠标按钮</summary>
        public int mouseButton;
        /// <summary>触摸手指ID</summary>
        public int fingerId;
    }

    /// <summary>
    /// 输入设置
    /// </summary>
    [Serializable]
    public class InputSettings
    {
        /// <summary>滑动阈值</summary>
        public float swipeThreshold = 50f;
        /// <summary>双击阈值</summary>
        public float doubleTapThreshold = 0.3f;
        /// <summary>轴输入灵敏度</summary>
        public float axisSensitivity = 1f;
        /// <summary>是否启用触摸</summary>
        public bool enableTouch = true;
        /// <summary>是否启用鼠标</summary>
        public bool enableMouse = true;
        /// <summary>是否启用键盘</summary>
        public bool enableKeyboard = true;
    }

    // 绑定配置列表
    public List<BindingConfig> bindingConfigs = new List<BindingConfig>();
    
    // 输入设置
    public InputSettings settings = new InputSettings();

    #region 公共方法

    /// <summary>
    /// 添加键盘绑定
    /// </summary>
    public void AddKeyboardBinding(E_EventType eventType, KeyCode key, InputInfo.InputState state = InputInfo.InputState.Down)
    {
        BindingConfig config = new BindingConfig
        {
            eventType = eventType,
            inputType = InputInfo.InputType.Keyboard,
            inputState = state,
            keyCode = key,
            mouseButton = 0,
            fingerId = 0
        };
        bindingConfigs.Add(config);
    }

    /// <summary>
    /// 添加鼠标绑定
    /// </summary>
    public void AddMouseBinding(E_EventType eventType, int mouseButton, InputInfo.InputState state = InputInfo.InputState.Down)
    {
        BindingConfig config = new BindingConfig
        {
            eventType = eventType,
            inputType = InputInfo.InputType.Mouse,
            inputState = state,
            keyCode = KeyCode.None,
            mouseButton = mouseButton,
            fingerId = 0
        };
        bindingConfigs.Add(config);
    }

    /// <summary>
    /// 添加触摸绑定
    /// </summary>
    public void AddTouchBinding(E_EventType eventType, int fingerId, InputInfo.InputState state = InputInfo.InputState.Down)
    {
        BindingConfig config = new BindingConfig
        {
            eventType = eventType,
            inputType = InputInfo.InputType.Touch,
            inputState = state,
            keyCode = KeyCode.None,
            mouseButton = 0,
            fingerId = fingerId
        };
        bindingConfigs.Add(config);
    }

    /// <summary>
    /// 移除绑定
    /// </summary>
    public void RemoveBinding(E_EventType eventType)
    {
        bindingConfigs.RemoveAll(config => config.eventType == eventType);
    }

    /// <summary>
    /// 清空所有绑定
    /// </summary>
    public void ClearBindings()
    {
        bindingConfigs.Clear();
    }

    /// <summary>
    /// 应用配置到输入管理器
    /// </summary>
    public void ApplyToInputMgr()
    {
        // 清空现有绑定
        InputMgr.Instance.ClearAllBindings();
        
        // 应用新绑定
        foreach (var config in bindingConfigs)
        {
            switch (config.inputType)
            {
                case InputInfo.InputType.Keyboard:
                    InputMgr.Instance.BindKeyboard(config.eventType, config.keyCode, config.inputState);
                    break;
                case InputInfo.InputType.Mouse:
                    InputMgr.Instance.BindMouse(config.eventType, config.mouseButton, config.inputState);
                    break;
                case InputInfo.InputType.Touch:
                    InputMgr.Instance.BindTouch(config.eventType, config.fingerId, config.inputState);
                    break;
            }
        }
        
        // 应用设置
        InputMgr.Instance.SetSwipeThreshold(settings.swipeThreshold);
        
        LogSystem.Info($"Applied input config with {bindingConfigs.Count} bindings");
    }

    /// <summary>
    /// 从输入管理器同步配置
    /// </summary>
    public void SyncFromInputMgr()
    {
        // 这里可以实现从InputMgr同步绑定信息到配置
        // 目前InputMgr没有提供获取所有绑定的方法，需要根据实际情况实现
    }

    #endregion

    #region 预设配置

    /// <summary>
    /// 创建默认配置
    /// </summary>
    public static InputConfig CreateDefaultConfig()
    {
        InputConfig config = new InputConfig();
        
        // 添加默认绑定
        config.AddKeyboardBinding(E_EventType.E_Input_Up, KeyCode.W, InputInfo.InputState.Hold);
        config.AddKeyboardBinding(E_EventType.E_Input_Down, KeyCode.S, InputInfo.InputState.Hold);
        config.AddKeyboardBinding(E_EventType.E_Input_Left, KeyCode.A, InputInfo.InputState.Hold);
        config.AddKeyboardBinding(E_EventType.E_Input_Right, KeyCode.D, InputInfo.InputState.Hold);
        config.AddKeyboardBinding(E_EventType.E_Input_Jump, KeyCode.Space, InputInfo.InputState.Down);
        config.AddKeyboardBinding(E_EventType.E_Input_Attack, KeyCode.Mouse0, InputInfo.InputState.Down);
        config.AddKeyboardBinding(E_EventType.E_Input_Special, KeyCode.E, InputInfo.InputState.Down);
        
        // 鼠标绑定
        config.AddMouseBinding(E_EventType.E_Input_Attack, 0, InputInfo.InputState.Down);
        config.AddMouseBinding(E_EventType.E_Input_Special, 1, InputInfo.InputState.Down);
        
        // 触摸绑定
        config.AddTouchBinding(E_EventType.E_Input_Jump, 0, InputInfo.InputState.Down);
        
        return config;
    }

    #endregion
}

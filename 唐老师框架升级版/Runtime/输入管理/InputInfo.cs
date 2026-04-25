using System;
using UnityEngine;

/// <summary>
/// 输入信息类
/// 统一管理键盘、鼠标、触摸输入
/// </summary>
public class InputInfo
{
    /// <summary>
    /// 输入类型枚举
    /// </summary>
    public enum InputType
    {
        /// <summary>键盘输入</summary>
        Keyboard,
        /// <summary>鼠标输入</summary>
        Mouse,
        /// <summary>触摸输入</summary>
        Touch
    }

    /// <summary>
    /// 输入状态枚举
    /// </summary>
    public enum InputState
    {
        /// <summary>按下</summary>
        Down,
        /// <summary>抬起</summary>
        Up,
        /// <summary>持续按住</summary>
        Hold,
        /// <summary>移动</summary>
        Move
    }

    /// <summary>
    /// 滑动方向枚举
    /// </summary>
    public enum SwipeDirection
    {
        /// <summary>无滑动</summary>
        None,
        /// <summary>向上</summary>
        Up,
        /// <summary>向下</summary>
        Down,
        /// <summary>向左</summary>
        Left,
        /// <summary>向右</summary>
        Right
    }

    // 基本信息
    public InputType Type { get; set; }
    public InputState State { get; set; }
    
    // 键盘信息
    public KeyCode Key { get; set; }
    
    // 鼠标信息
    public int MouseButton { get; set; }
    public Vector2 MousePosition { get; set; }
    
    // 触摸信息
    public int FingerId { get; set; }
    public Vector2 TouchPosition { get; set; }
    public TouchPhase TouchPhase { get; set; }
    
    // 滑动信息
    public SwipeDirection Direction { get; set; }
    public Vector2 Delta { get; set; }
    public float Magnitude { get; set; }

    // 时间信息
    public float Timestamp { get; set; }
    public float Duration { get; set; }

    #region 构造函数

    /// <summary>
    /// 键盘输入构造函数
    /// </summary>
    public InputInfo(InputState state, KeyCode key)
    {
        Type = InputType.Keyboard;
        State = state;
        Key = key;
        Timestamp = Time.time;
    }

    /// <summary>
    /// 鼠标输入构造函数
    /// </summary>
    public InputInfo(InputState state, int mouseButton, Vector2 position)
    {
        Type = InputType.Mouse;
        State = state;
        MouseButton = mouseButton;
        MousePosition = position;
        Timestamp = Time.time;
    }

    /// <summary>
    /// 触摸输入构造函数
    /// </summary>
    public InputInfo(InputState state, int fingerId, TouchPhase phase, Vector2 position, SwipeDirection direction = SwipeDirection.None, Vector2 delta = default, float magnitude = 0f)
    {
        Type = InputType.Touch;
        State = state;
        FingerId = fingerId;
        TouchPhase = phase;
        TouchPosition = position;
        Direction = direction;
        Delta = delta;
        Magnitude = magnitude;
        Timestamp = Time.time;
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 从Unity的TouchPhase转换为InputState
    /// </summary>
    public static InputState FromTouchPhase(TouchPhase phase)
    {
        return phase switch
        {
            TouchPhase.Began => InputState.Down,
            TouchPhase.Ended => InputState.Up,
            TouchPhase.Canceled => InputState.Up,
            TouchPhase.Moved => InputState.Move,
            TouchPhase.Stationary => InputState.Hold,
            _ => InputState.Hold
        };
    }

    /// <summary>
    /// 从InputState转换为Unity的TouchPhase
    /// </summary>
    public static TouchPhase ToTouchPhase(InputState state)
    {
        return state switch
        {
            InputState.Down => TouchPhase.Began,
            InputState.Up => TouchPhase.Ended,
            InputState.Move => TouchPhase.Moved,
            _ => TouchPhase.Stationary
        };
    }

    /// <summary>
    /// 计算滑动方向
    /// </summary>
    public static SwipeDirection CalculateSwipeDirection(Vector2 delta, float threshold = 50f)
    {
        if (delta.magnitude < threshold)
            return SwipeDirection.None;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        }
        else
        {
            return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
        }
    }

    #endregion

    #region 重写方法

    public override string ToString()
    {
        return $"InputInfo: {Type} - {State} - {GetDetailInfo()}";
    }

    private string GetDetailInfo()
    {
        switch (Type)
        {
            case InputType.Keyboard:
                return $"Key: {Key}";
            case InputType.Mouse:
                return $"Button: {MouseButton}, Position: {MousePosition}";
            case InputType.Touch:
                return $"Finger: {FingerId}, Position: {TouchPosition}, Direction: {Direction}";
            default:
                return "Unknown";
        }
    }

    #endregion
}

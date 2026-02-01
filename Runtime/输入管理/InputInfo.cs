using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入信息
/// </summary>
public class InputInfo
{
    public enum E_KeyOrMouse
    {
        /// <summary>
        /// 键盘输入
        /// </summary>
        Key,
        /// <summary>
        /// 鼠标输入
        /// </summary>
        Mouse,

        /// <summary>
        /// 新增触摸类型
        /// </summary>
        Touch
    }

    public enum E_InputType
    {
        /// <summary>
        /// 按下
        /// </summary>
        Down,
        /// <summary>
        /// 抬起
        /// </summary>
        Up,
        /// <summary>
        /// 长按
        /// </summary>
        Always,
    }



    //具体输入的类型——键盘还是鼠标
    public E_KeyOrMouse keyOrMouse;
    //输入的类型——抬起、按下、长按
    public E_InputType inputType;
    //KeyCode
    public KeyCode key;
    //mouseID
    public int mouseID;

    // 触摸专用字段
    public int fingerId;     // 触点ID
    public Vector2 position; // 触摸位置
    public TouchPhase phase; // 触摸阶段


    // 新增滑动方向字段
    public E_SwipeDirection swipeDirection;

    /// <summary>
    /// 主要给键盘输入初始化
    /// </summary>
    /// <param name="inputType"></param>
    /// <param name="key"></param>
    public InputInfo(E_InputType inputType, KeyCode key)
    {
        this.keyOrMouse = E_KeyOrMouse.Key;
        this.inputType = inputType;
        this.key = key;
    }

    /// <summary>
    /// 主要给鼠标输入初始化
    /// </summary>
    /// <param name="inputType"></param>
    /// <param name="mouseID"></param>
    public InputInfo(E_InputType inputType, int mouseID)
    {
        this.keyOrMouse = E_KeyOrMouse.Mouse;
        this.inputType = inputType;
        this.mouseID = mouseID;
    }

    /// <summary>
    /// 触摸输入构造函数
    /// </summary>
    public InputInfo(E_InputType inputType, int fingerId, TouchPhase phase, Vector2 position)
    {
        this.keyOrMouse = E_KeyOrMouse.Touch;
        this.inputType = inputType;
        this.fingerId = fingerId;
        this.phase = phase;
        this.position = position;
    }

    // 修改构造函数以支持滑动方向
    //public InputInfo(E_InputType inputType, KeyCode key, E_SwipeDirection swipeDirection = E_SwipeDirection.None)
    //{
    //    this.keyOrMouse = E_KeyOrMouse.Key;
    //    this.inputType = inputType;
    //    this.key = key;
    //    this.swipeDirection = swipeDirection;
    //}

    //public InputInfo(E_InputType inputType, int mouseID, E_SwipeDirection swipeDirection = E_SwipeDirection.None)
    //{
    //    this.keyOrMouse = E_KeyOrMouse.Mouse;
    //    this.inputType = inputType;
    //    this.mouseID = mouseID;
    //    this.swipeDirection = swipeDirection;
    //}

    public InputInfo(E_InputType inputType, int fingerId, TouchPhase phase, Vector2 position, E_SwipeDirection swipeDirection = E_SwipeDirection.None)
    {
        this.keyOrMouse = E_KeyOrMouse.Touch;
        this.inputType = inputType;
        this.fingerId = fingerId;
        this.phase = phase;
        this.position = position;
        this.swipeDirection = swipeDirection;
    }
}

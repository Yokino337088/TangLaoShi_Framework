using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 新增滑动事件类型
public enum E_SwipeDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}

public class InputMgr : BaseManager<InputMgr>
{
    private Dictionary<E_EventType, InputInfo> inputDic = new Dictionary<E_EventType, InputInfo>();

    //当前遍历时取出的输入信息
    private InputInfo nowInputInfo;

    //是否开启了输入系统检测
    private bool isStart;
    //用于在改建时获取输入信息的委托 只有当update中获取到信息的时候 再通过委托传递给外部
    private UnityAction<InputInfo> getInputInfoCallBack;
    //是否开始检测输入信息
    private bool isBeginCheckInput = false;

    // 滑动检测相关字段
    private Dictionary<int, Vector2> touchStartPositions = new Dictionary<int, Vector2>(); // 记录每个触点的起始位置
    private Dictionary<int, Vector2> touchCurrentPositions = new Dictionary<int, Vector2>(); // 记录每个触点的当前位置
    private float swipeThreshold = 50f; // 滑动检测的阈值（像素）


    // 新增点击事件委托
    public UnityAction<Vector2> OnScreenTapped;
    // 新增滑动事件委托
    public UnityAction<E_SwipeDirection> OnSwipeDetected;


    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }

    /// <summary>
    /// 开启或者关闭我们的输入管理模块的检测
    /// </summary>
    /// <param name="isStart"></param>
    public void StartOrCloseInputMgr(bool isStart)
    {
        this.isStart = isStart;
    }

    /// <summary>
    /// 提供给外部改建或初始化的方法(键盘)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="inputType"></param>
    public void ChangeKeyboardInfo(E_EventType eventType, KeyCode key, InputInfo.E_InputType inputType)
    {
        //初始化
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(inputType, key));
        }
        else//改建
        {
            //如果之前是鼠标 我们必须要修改它的按键类型
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Key;
            inputDic[eventType].key = key;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// 提供给外部改建或初始化的方法(鼠标)
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="mouseID"></param>
    /// <param name="inputType"></param>
    public void ChangeMouseInfo(E_EventType eventType, int mouseID, InputInfo.E_InputType inputType)
    {
        //初始化
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(inputType, mouseID));
        }
        else//改建
        {
            //如果之前是鼠标 我们必须要修改它的按键类型
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Mouse;
            inputDic[eventType].mouseID = mouseID;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// 移除指定行为的输入监听
    /// </summary>
    /// <param name="eventType"></param>
    public void RemoveInputInfo(E_EventType eventType)
    {
        if (inputDic.ContainsKey(eventType))
            inputDic.Remove(eventType);
    }

    /// <summary>
    /// 获取下一次的输入信息
    /// </summary>
    /// <param name="callBack"></param>
    public void GetInputInfo(UnityAction<InputInfo> callBack)
    {
        getInputInfoCallBack = callBack;
        MonoMgr.Instance.StartCoroutine(BeginCheckInput());
    }

    private IEnumerator BeginCheckInput()
    {
        //等一帧
        yield return 0;
        //一帧后才会被置成true
        isBeginCheckInput = true;

        // 添加触摸检测
        int lastTouchCount = 0;
        while (isBeginCheckInput)
        {
            if (Input.touchCount > lastTouchCount)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        getInputInfoCallBack.Invoke(new InputInfo(
                            InputInfo.E_InputType.Down,
                            touch.fingerId,
                            TouchPhase.Began,
                            touch.position
                        ));
                        isBeginCheckInput = false;
                        yield break;
                    }
                }
            }
            lastTouchCount = Input.touchCount;
            yield return null;
        }
    }

    private void InputUpdate()
    {
        //当委托不为空时 证明想要获取到输入的信息 传递给外部
        if (isBeginCheckInput)
        {
            //当一个键按下时 然后遍历所有按键信息 得到是谁被按下了
            if (Input.anyKeyDown)
            {
                InputInfo inputInfo = null;
                //我们需要去遍历监听所有键位的按下 来得到对应输入的信息
                //键盘
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                foreach (KeyCode inputKey in keyCodes)
                {
                    //判断到底是谁被按下了 那么就可以得到对应的输入的键盘信息
                    if (Input.GetKeyDown(inputKey))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, inputKey);
                        break;
                    }
                }
                //鼠标
                for (int i = 0; i < 3; i++)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, i);
                        break;
                    }
                }
                //把获取到的信息传递给外部
                getInputInfoCallBack.Invoke(inputInfo);
                getInputInfoCallBack = null;
                //检测一次后就停止检测了
                isBeginCheckInput = false;
            }
        }



        //如果外部没有开启检测功能 就不要检测
        if (!isStart)
            return;

        foreach (E_EventType eventType in inputDic.Keys)
        {
            nowInputInfo = inputDic[eventType];
            //如果是键盘输入
            if (nowInputInfo.keyOrMouse == InputInfo.E_KeyOrMouse.Key)
            {
                //是抬起还是按下还是长按
                switch (nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if (Input.GetKeyDown(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetKeyUp(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Always:
                        if (Input.GetKey(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    default:
                        break;
                }
            }
            //如果是鼠标输入
            else
            {
                switch (nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if (Input.GetMouseButtonDown(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetMouseButtonUp(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Always:
                        if (Input.GetMouseButton(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    default:
                        break;
                }
            }
        }

        // 检测触摸点击
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // 获取第一个触点
            if (touch.phase == TouchPhase.Began)
            {
                //UnityEngine.Debug.Log("触发点击事件");
                // 触发点击事件
                OnScreenTapped?.Invoke(touch.position);
            }
            //处理滑动事件
            //DetectSwipe(touch);
        }



        //// 新增触摸检测
        //foreach (Touch touch in Input.touches)
        //{
        //    HandleTouchInput(touch);
        //    DetectSwipe(touch); // 新增滑动检测
        //}


        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Horizontal, Input.GetAxis("Horizontal"));
        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Vertical, Input.GetAxis("Vertical"));
    }

    /// <summary>
    /// 处理触摸输入
    /// </summary>
    private void HandleTouchInput(Touch touch)
    {
        foreach (var pair in inputDic)
        {
            InputInfo info = pair.Value;
            if (info.keyOrMouse != InputInfo.E_KeyOrMouse.Touch)
                continue;

            // 匹配触摸阶段和输入类型
            bool isMatch = (touch.phase == TouchPhase.Began && info.inputType == InputInfo.E_InputType.Down) ||
                          (touch.phase == TouchPhase.Ended && info.inputType == InputInfo.E_InputType.Up) ||
                          ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) &&
                           info.inputType == InputInfo.E_InputType.Always);

            if (isMatch && touch.fingerId == info.fingerId)
            {
                // 更新位置信息
                info.position = touch.position;
                EventCenter.Instance.EventTrigger(pair.Key, info);
            }
        }
    }

    /// <summary>
    /// 新增触摸配置方法
    /// </summary>
    public void ChangeTouchInfo(E_EventType eventType, int fingerId, InputInfo.E_InputType inputType)
    {
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(
                inputType,
                fingerId,
                GetPhaseFromInputType(inputType),
                Vector2.zero
            ));
        }
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Touch;
            inputDic[eventType].fingerId = fingerId;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// 转换输入类型到触摸阶段
    /// </summary>
    private TouchPhase GetPhaseFromInputType(InputInfo.E_InputType type)
    {
        return type switch
        {
            InputInfo.E_InputType.Down => TouchPhase.Began,
            InputInfo.E_InputType.Up => TouchPhase.Ended,
            _ => TouchPhase.Moved
        };
    }

    /// <summary>
    /// 检测滑动操作
    /// </summary>
    private void DetectSwipe(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPositions[touch.fingerId] = touch.position;
                touchCurrentPositions[touch.fingerId] = touch.position;
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                touchCurrentPositions[touch.fingerId] = touch.position;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                Vector2 startPos = touchStartPositions[touch.fingerId];
                Vector2 endPos = touchCurrentPositions[touch.fingerId];
                Vector2 swipeDelta = endPos - startPos;

                if (swipeDelta.magnitude >= swipeThreshold)
                {
                    E_SwipeDirection direction = GetSwipeDirection(swipeDelta);
                    OnSwipeDetected?.Invoke(direction);
                }

                touchStartPositions.Remove(touch.fingerId);
                touchCurrentPositions.Remove(touch.fingerId);
                break;
        }
    }

    /// <summary>
    /// 根据滑动差值计算滑动方向
    /// </summary>
    private E_SwipeDirection GetSwipeDirection(Vector2 swipeDelta)
    {
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            return swipeDelta.x > 0 ? E_SwipeDirection.Right : E_SwipeDirection.Left;
        }
        else
        {
            return swipeDelta.y > 0 ? E_SwipeDirection.Up : E_SwipeDirection.Down;
        }
    }

}

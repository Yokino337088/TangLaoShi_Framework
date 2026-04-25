using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 面板状态枚举
/// </summary>
public enum PanelState
{
    Inactive,
    Initializing,
    Active,
    Hiding,
    Destroying
}

public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 用于存储将来需要用到的UI控件的字典 用来代替原来的 通过GameObject.Find查找
    /// </summary>
    protected Dictionary<string, UIBehaviour> controlDic = new Dictionary<string, UIBehaviour>();

    /// <summary>
    /// 控件默认名称 这些名称的控件我们不进行存储 因为这些通常是不通过代码去使用的 只作为界面显示使用的控件
    /// </summary>
    private static List<string> defaultNameList = new List<string>() { "Image",
                                                                   "Text (TMP)",
                                                                   "RawImage",
                                                                   "Background",
                                                                   "Checkmark",
                                                                   "Label",
                                                                   "Text (Legacy)",
                                                                   "Arrow",
                                                                   "Placeholder",
                                                                   "Fill",
                                                                   "Handle",
                                                                   "Viewport",
                                                                   "Scrollbar Horizontal",
                                                                   "Scrollbar Vertical"};



    /// <summary>
    /// 当前面板状态
    /// </summary>
    public PanelState CurrentState { get; private set; } = PanelState.Inactive;

    protected List<Button> btnLists = new List<Button>();

    protected virtual void Awake()
    {
        CurrentState = PanelState.Initializing;
        //为了方便 一次性把所有需要的控件全部找到并存储起来
        //将来要使用的话 直接通过名称去字典中获取即可
        FindChildrenControl<Button>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<InputField>();
        FindChildrenControl<TMP_InputField>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<Dropdown>();
        //为了方便获取到文本和图片 只要能找到的都存储起来
        //之后也可以通过需要用到的控件名称来获取对应的控件
        FindChildrenControl<Text>();
        FindChildrenControl<TextMeshPro>();
        FindChildrenControl<TextMeshProUGUI>();
        FindChildrenControl<Image>();       
        CurrentState = PanelState.Inactive;
    }



    /// <summary>
    /// 面板显示时调用的方法
    /// </summary>
    public virtual void ShowMe()
    {
        CurrentState = PanelState.Active;
    }

    /// <summary>
    /// 面板隐藏时调用的方法
    /// </summary>
    public virtual void HideMe()
    {
        CurrentState = PanelState.Hiding;

    }

    /// <summary>
    /// 为所有按钮添加颜色变化动画
    /// </summary>
    /// <param name="color"></param>
    /// <param name="time"></param>
    protected void AddButtonColorAnimation(Color color, float time)
    {
        foreach (Button btn in btnLists)
        {
            btn.gameObject.GetComponent<Image>().AddImageHoverColorEffect(color, time);
        }
    }

    /// <summary>
    /// 获取指定名称并且指定类型的控件
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <param name="name">控件名称</param>
    /// <returns></returns>
    public T GetControl<T>(string name) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(name))
        {
            T control = controlDic[name] as T;
            if (control == null)
                LogSystem.Error($"没有找到对应名称{name}的{typeof(T)}类型控件");
            return control;
        }
        else
        {
            LogSystem.Error($"没有找到对应名称{name}的控件");
            return null;
        }
    }

    protected virtual void ClickBtn(string btnName)
    {

    }

    protected virtual void SliderValueChange(string sliderName, float value)
    {

    }

    protected virtual void ToggleValueChange(string sliderName, bool value)
    {

    }

    protected virtual void InputValueChange(string inputName, string value)
    {

    }

    protected virtual void InputEndEdit(string inputName, string value)
    {

    }

    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>(true);
        for (int i = 0; i < controls.Length; i++)
        {
            //获取当前控件的名称
            string controlName = controls[i].gameObject.name;
            //通过字典方式 存储对应控件的引用
            if (!controlDic.ContainsKey(controlName))
            {
                if (!defaultNameList.Contains(controlName))
                {
                    controlDic.Add(controlName, controls[i]);
                    //判断控件的类型 看是否需要添加对应的事件监听
                    if (controls[i] is Button)
                    {
                        (controls[i] as Button).onClick.AddListener(() =>
                        {
                            ClickBtn(controlName);
                        });
                        btnLists.Add(controls[i] as Button);
                    }
                    else if (controls[i] is Slider)
                    {
                        (controls[i] as Slider).onValueChanged.AddListener((value) =>
                        {
                            SliderValueChange(controlName, value);
                        });
                    }
                    else if (controls[i] is Toggle)
                    {
                        (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                        {
                            ToggleValueChange(controlName, value);
                        });
                    }
                    else if (controls[i] is InputField)
                    {
                        (controls[i] as InputField).onValueChanged.AddListener((value) =>
                        {
                            InputValueChange(controlName, value);
                            InputEndEdit(controlName, value);
                        });
                    }
                }

            }
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 层级枚举
/// </summary>
public enum E_UILayer
{
    /// <summary>
    /// 最底层
    /// </summary>
    Bottom,
    /// <summary>
    /// 中层
    /// </summary>
    Middle,
    /// <summary>
    /// 高层
    /// </summary>
    Top,
    /// <summary>
    /// 系统层 最高层
    /// </summary>
    System,
}

public enum E_HideType
{
    缩放退出,
    淡出,
    顶部滑出,
    底部滑出,
    左侧滑出,
    右侧滑出,
}

/// <summary>
/// 管理所有UI面板的管理器
/// 注意：面板预设体名要和面板类名一致！！！！！
/// </summary>
public class UIMgr : BaseManager<UIMgr>
{
    /// <summary>
    /// 主要用于里式替换原则 在字典中 用父类容器装载子类对象
    /// </summary>
    private abstract class BasePanelInfo { }

    /// <summary>
    /// 用于存储面板信息 和加载完成的回调函数的
    /// </summary>
    /// <typeparam name="T">面板的类型</typeparam>
    private class PanelInfo<T> : BasePanelInfo where T:BasePanel
    {
        public T panel;
        public Action<T> callBack;
        public bool isHide;

        public PanelInfo(Action<T> callBack)
        {
            this.callBack += callBack;
        }
    }


    private Camera uiCamera;
    private Canvas uiCanvas;
    private EventSystem uiEventSystem;

    //层级父对象
    private Transform bottomLayer;
    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    /// <summary>
    /// Canvas的缩放因子
    /// </summary>
    public float CanvasScaleFactor
    {
        get
        {
            Canvas canvas = uiCanvas;
            return canvas != null ? canvas.scaleFactor : 1f;
        }
    }

    /// <summary>
    /// 用于存储所有的面板对象
    /// </summary>
    private Dictionary<string, BasePanelInfo> panelDic = new Dictionary<string, BasePanelInfo>();

    private UIMgr()
    {
        //动态创建唯一的Canvas和EventSystem（摄像机）
        uiCamera = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
        //ui摄像机过场景不移除 专门用来渲染UI面板
        GameObject.DontDestroyOnLoad(uiCamera.gameObject);

        //动态创建Canvas
        uiCanvas = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        //设置使用的UI摄像机
        uiCanvas.worldCamera = uiCamera;
        //过场景不移除
        GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

        //找到层级父对象
        bottomLayer = uiCanvas.transform.Find("Bottom");
        middleLayer = uiCanvas.transform.Find("Middle");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");

        //动态创建EventSystem
        uiEventSystem = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
    }

    /// <summary>
    /// 获取对应层级的父对象
    /// </summary>
    /// <param name="layer">层级枚举值</param>
    /// <returns></returns>
    public Transform GetLayerFather(E_UILayer layer)
    {
        switch (layer)
        {
            case E_UILayer.Bottom:
                return bottomLayer;
            case E_UILayer.Middle:
                return middleLayer;
            case E_UILayer.Top:
                return topLayer;
            case E_UILayer.System:
                return systemLayer;
            default:
                return null;
        }
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板的类型</typeparam>
    /// <param name="layer">面板显示的层级</param>
    /// <param name="callBack">由于可能是异步加载 因此通过委托回调的形式 将加载完成的面板传递出去进行使用</param>
    /// <param name="isSync">是否采用同步加载 默认为false</param>
    public void ShowPanel<T>(E_UILayer layer = E_UILayer.Middle,  Action<T> callBack = null, string abName = "ui", bool isSync = false) where T:BasePanel
    {
        //获取面板名 预设体名必须和面板类名一致 
        string panelName = typeof(T).Name;
        //存在面板
        if(panelDic.ContainsKey(panelName))
        {
            //取出字典中已经占好位置的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在异步加载中
            if(panelInfo.panel == null)
            {
                //如果之前显示了又隐藏 现在又想显示 那么直接设为false
                panelInfo.isHide = false;

                //如果正在异步加载 应该等待它加载完毕 只需要记录回调函数 加载完后去调用即可
                if (callBack != null)
                    panelInfo.callBack += callBack;
            }
            else//已经加载结束
            {
                //如果是失活状态 直接激活面板 就可以显示了
                if (!panelInfo.panel.gameObject.activeSelf)
                    panelInfo.panel.gameObject.SetActive(true);

                //动画完成后执行默认显示逻辑
                panelInfo.panel.ShowMe();
                //如果存在回调 直接返回出去即可
                callBack?.Invoke(panelInfo.panel);
   
            }
            return;
        }

        //不存在面板 先存入字典当中 占个位置 之后如果又显示 我才能得到字典中的信息进行判断
        panelDic.Add(panelName, new PanelInfo<T>(callBack));

        //不存在面板 加载面板
        ABResMgr.Instance.LoadResAsync<GameObject>(abName, panelName, (res) =>
        {
            //取出字典中已经占好位置的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //表示异步加载结束前 就想要隐藏该面板了 
            if (panelInfo.isHide)
            {
                panelDic.Remove(panelName);
                return;
            }

            //层级的处理
            Transform father = GetLayerFather(layer);
            //避免没有按指定规则传递层级参数 避免为空
            if (father == null)
                father = middleLayer;
            //将面板预设体创建到对应父对象下 并且保持原本的缩放大小
            GameObject panelObj = GameObject.Instantiate(res, father, false);

            //获取对应UI组件返回出去
            T panel = panelObj.GetComponent<T>();

            //显示面板时执行的默认方法
            panel.ShowMe();
            //传出去使用
            panelInfo.callBack?.Invoke(panel);
            //回调执行完 将其清空 避免内存泄漏
            panelInfo.callBack = null;
            //存储panel
            panelInfo.panel = panel;

        }, isSync);

       
        
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void HidePanel<T>(bool isDestory = false) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中已经占好位置的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //但是正在加载中
            if(panelInfo.panel == null)
            {
                //修改隐藏表示 表示 这个面板即将要隐藏
                panelInfo.isHide = true;
                //既然要隐藏了 回调函数都不会调用了 直接置空
                panelInfo.callBack = null;
            }
            else//已经加载结束
            {
                //执行默认的隐藏面板想要做的事情
                panelInfo.panel.HideMe();

                //如果要销毁  就直接将面板销毁从字典中移除记录
                if (isDestory)
                {
                    //销毁面板
                    GameObject.Destroy(panelInfo.panel.gameObject);
                    //从容器中移除
                    panelDic.Remove(panelName);
                }
                //如果不销毁 那么就只是失活 下次再显示的时候 直接复用即可
                else
                    panelInfo.panel.gameObject.SetActive(false);
               
            }
        }
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T">面板的类型</typeparam>
    public void GetPanel<T>( Action<T> callBack ) where T:BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            //取出字典中已经占好位置的数据
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在加载中
            if(panelInfo.panel == null)
            {
                //加载中 应该等待加载结束 再通过回调传递给外部去使用
                panelInfo.callBack += callBack;
            }
            else if(!panelInfo.isHide)//加载结束 并且没有隐藏
            {
                callBack?.Invoke(panelInfo.panel);
            }
        }
    }

    /// <summary>
    /// 获取面板的重载方法，直接返回面板对象
    /// </summary>
    /// <typeparam name="T">面板的类型</typeparam>
    /// <returns>如果面板已加载且未隐藏，则返回面板实例；否则返回 null</returns>
    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            if (panelInfo.panel != null && !panelInfo.isHide)
            {
                return panelInfo.panel;
            }
        }
        return null;
    }


    /// <summary>
    /// 为控件添加自定义事件
    /// </summary>
    /// <param name="control">对应的控件</param>
    /// <param name="type">事件的类型</param>
    /// <param name="callBack">响应的函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        //这种逻辑主要是用于保证 控件上只会挂载一个EventTrigger
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 获取EventSystem实例
    /// </summary>
    public EventSystem GetEventSystem()
    {
        return uiEventSystem;
    }

    /// <summary>
    /// 带动画的隐藏面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="hideType"></param>
    /// <param name="duration"></param>
    /// <param name="onComplete"></param>
    public void HidePanelWithAnimation<T>(E_HideType hideType,float duration = 0.5f, Action onComplete = null) where T: BasePanel
    {
        BasePanel basePanel = GetPanel<T>();
        basePanel.HideMe();
        switch (hideType)
        {
            case E_HideType.缩放退出:
                basePanel.DoPanelScaleOutAnimation(duration, onComplete);
                break;
            case E_HideType.淡出:
                basePanel.DoPanelFadeOutAnimation(duration, onComplete);
                break;
            case E_HideType.顶部滑出:
                basePanel.DoPanelSlideOutToTop(duration, onComplete);
                break;
            case E_HideType.底部滑出:
                basePanel.DoPanelSlideOutToBottom(duration, onComplete);
                break;
            case E_HideType.左侧滑出:
                basePanel.DoPanelSlideOutToLeft(duration, onComplete);
                break;
            case E_HideType.右侧滑出:
                basePanel.DoPanelSlideOutToRight(duration, onComplete);
                break;
        }
    }
}



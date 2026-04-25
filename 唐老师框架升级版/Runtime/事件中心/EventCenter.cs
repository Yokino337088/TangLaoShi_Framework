using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 事件 基类用来 替代原来 的委托 进行解耦
/// </summary>
public abstract class EventInfoBase { }

/// <summary>
/// 事件信息 对应一个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventInfo<T> : EventInfoBase
{
    //事件对应的 委托类型 存储对应的方法
    public Action<T> actions;

    public EventInfo(Action<T> action)
    {
        actions += action;
    }
}



/// <summary>
/// 事件信息 对应两个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class EventInfo<T1, T2> : EventInfoBase
{
    public Action<T1, T2> actions;

    public EventInfo(Action<T1, T2> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应三个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class EventInfo<T1, T2, T3> : EventInfoBase
{
    public Action<T1, T2, T3> actions;

    public EventInfo(Action<T1, T2, T3> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应四个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
public class EventInfo<T1, T2, T3, T4> : EventInfoBase
{
    public Action<T1, T2, T3, T4> actions;

    public EventInfo(Action<T1, T2, T3, T4> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应五个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应六个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应七个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应八个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7, T8> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7, T8> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应九个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
/// <typeparam name="T9"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应十个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
/// <typeparam name="T9"></typeparam>
/// <typeparam name="T10"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应十一个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
/// <typeparam name="T9"></typeparam>
/// <typeparam name="T10"></typeparam>
/// <typeparam name="T11"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应十二个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
/// <typeparam name="T9"></typeparam>
/// <typeparam name="T10"></typeparam>
/// <typeparam name="T11"></typeparam>
/// <typeparam name="T12"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应十三个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
/// <typeparam name="T9"></typeparam>
/// <typeparam name="T10"></typeparam>
/// <typeparam name="T11"></typeparam>
/// <typeparam name="T12"></typeparam>
/// <typeparam name="T13"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应十四个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
/// <typeparam name="T9"></typeparam>
/// <typeparam name="T10"></typeparam>
/// <typeparam name="T11"></typeparam>
/// <typeparam name="T12"></typeparam>
/// <typeparam name="T13"></typeparam>
/// <typeparam name="T14"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应十五个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
/// <typeparam name="T9"></typeparam>
/// <typeparam name="T10"></typeparam>
/// <typeparam name="T11"></typeparam>
/// <typeparam name="T12"></typeparam>
/// <typeparam name="T13"></typeparam>
/// <typeparam name="T14"></typeparam>
/// <typeparam name="T15"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件信息 对应十六个参数 的委托类型 的事件
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
/// <typeparam name="T8"></typeparam>
/// <typeparam name="T9"></typeparam>
/// <typeparam name="T10"></typeparam>
/// <typeparam name="T11"></typeparam>
/// <typeparam name="T12"></typeparam>
/// <typeparam name="T13"></typeparam>
/// <typeparam name="T14"></typeparam>
/// <typeparam name="T15"></typeparam>
/// <typeparam name="T16"></typeparam>
public class EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : EventInfoBase
{
    public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> actions;

    public EventInfo(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action)
    {
        actions += action;
    }
}

/// <summary>
/// 无参数无返回值执行
/// </summary>
public class EventInfo : EventInfoBase
{
    public Action actions;

    public EventInfo(Action action)
    {
        actions += action;
    }
}

// 优先级事件信息类
public class PrioritizedEventInfo<T> : EventInfoBase
{
    public List<(Action<T> action, int priority)> prioritizedActions = new List<(Action<T>, int)>();

    public void AddAction(Action<T> action, int priority = 0)
    {
        prioritizedActions.Add((action, priority));
        // 按优先级排序，优先级高的先执行
        prioritizedActions.Sort((a, b) => b.priority.CompareTo(a.priority));

    }

    public void RemoveAction(Action<T> action)
    {
        prioritizedActions.RemoveAll(x => x.action == action);

    }

    public void Invoke(T info)
    {
        foreach (var (action, _) in prioritizedActions)
        {
            action?.Invoke(info);
        }
    }
}

/// <summary>
/// 事件中心模块 
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    //用于存储对应事件 的键值对 对应的方法
    private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>();
    
    //用于存储对应事件 的键值对 对应的方法（使用string作为key）
    private Dictionary<string, EventInfoBase> stringEventDic = new Dictionary<string, EventInfoBase>();

    private EventCenter() { }

    /// <summary>
    /// 事件触发 一个参数
    /// </summary>
    /// <param name="eventName">事件名</param>
    public void EventTrigger<T>(E_EventType eventName, T info)
    {
        //如果字典中找到了 就通知所有注册的方法
        if (eventDic.ContainsKey(eventName))
        {
            //去执行对应的方法
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }

    /// <summary>
    /// 事件触发 无参数
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(E_EventType eventName)
    {
        //如果字典中找到了 就通知所有注册的方法
        if (eventDic.ContainsKey(eventName))
        {
            //去执行对应的方法
            (eventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }
    
    /// <summary>
    /// 事件触发 一个参数（使用string作为key）
    /// </summary>
    /// <param name="eventName">事件名</param>
    public void EventTrigger<T>(string eventName, T info)
    {
        //如果字典中找到了 就通知所有注册的方法
        if (stringEventDic.ContainsKey(eventName))
        {
            //去执行对应的方法
            (stringEventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }

    /// <summary>
    /// 事件触发 无参数（使用string作为key）
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(string eventName)
    {
        //如果字典中找到了 就通知所有注册的方法
        if (stringEventDic.ContainsKey(eventName))
        {
            //去执行对应的方法
            (stringEventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }
    
    /// <summary>
    /// 事件触发 支持两个参数的事件（使用string作为key）
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    public void EventTrigger<T1, T2>(string eventName, T1 info1, T2 info2)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2>).actions?.Invoke(info1, info2);
        }
    }
    
    /// <summary>
    /// 事件触发 支持三个参数（使用string作为key）
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="eventName"></param>
    /// <param name="info1"></param>
    /// <param name="info2"></param>
    /// <param name="info3"></param>
    public void EventTrigger<T1, T2, T3>(string eventName, T1 info1, T2 info2, T3 info3)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2, T3>).actions?.Invoke(info1, info2, info3);
        }
    }
    
    /// <summary>
    /// 事件触发 支持四个参数的事件（使用string作为key）
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    public void EventTrigger<T1, T2, T3, T4>(string eventName, T1 info1, T2 info2, T3 info3, T4 info4)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2, T3, T4>).actions?.Invoke(info1, info2, info3, info4);
        }
    }
    
    /// <summary>
    /// 事件触发 支持五个参数的事件（使用string作为key）
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5>(string eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2, T3, T4, T5>).actions?.Invoke(info1, info2, info3, info4, info5);
        }
    }
    
    /// <summary>
    /// 事件触发 支持六个参数的事件（使用string作为key）
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6>(string eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6>).actions?.Invoke(info1, info2, info3, info4, info5, info6);
        }
    }
    
    // 异步事件触发方法（使用string作为key）
    public async void EventTriggerAsync<T>(string eventName, T info)
    {
        try
        {
            if (stringEventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }
    
    // 异步事件触发方法（使用string作为key）
    public async void EventTriggerAsync<T1, T2>(string eventName, T1 info1, T2 info2)
    {
        try
        {
            if (stringEventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }
    
    // 异步事件触发方法（使用string作为key）
    public async void EventTriggerAsync<T1, T2, T3>(string eventName, T1 info1, T2 info2, T3 info3)
    {
        try
        {
            if (stringEventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }
    
    // 异步事件触发方法（使用string作为key）
    public async void EventTriggerAsync<T1, T2, T3, T4>(string eventName, T1 info1, T2 info2, T3 info3, T4 info4)
    {
        try
        {
            if (stringEventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }
    
    // 异步事件触发方法（使用string作为key）
    public async void EventTriggerAsync<T1, T2, T3, T4, T5>(string eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5)
    {
        try
        {
            if (stringEventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }
    
    // 异步事件触发方法（使用string作为key）
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6>(string eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6)
    {
        try
        {
            if (stringEventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T>(E_EventType eventName, T info)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持两个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    public void EventTrigger<T1, T2>(E_EventType eventName, T1 info1, T2 info2)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2>).actions?.Invoke(info1, info2);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2>(E_EventType eventName, T1 info1, T2 info2)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持三个参数
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="eventName"></param>
    /// <param name="info1"></param>
    /// <param name="info2"></param>
    /// <param name="info3"></param>
    public void EventTrigger<T1, T2, T3>(E_EventType eventName, T1 info1, T2 info2, T3 info3)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3>).actions?.Invoke(info1, info2, info3);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3>(E_EventType eventName, T1 info1, T2 info2, T3 info3)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持四个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    public void EventTrigger<T1, T2, T3, T4>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4>).actions?.Invoke(info1, info2, info3, info4);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持五个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5>).actions?.Invoke(info1, info2, info3, info4, info5);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持六个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6>).actions?.Invoke(info1, info2, info3, info4, info5, info6);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持七个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持八个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    /// <param name="info8">第八个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7, T8>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7, info8);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7, T8>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7, info8));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持九个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    /// <param name="info8">第八个参数</param>
    /// <param name="info9">第九个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7, T8, T9>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持十个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    /// <param name="info8">第八个参数</param>
    /// <param name="info9">第九个参数</param>
    /// <param name="info10">第十个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持十一个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    /// <param name="info8">第八个参数</param>
    /// <param name="info9">第九个参数</param>
    /// <param name="info10">第十个参数</param>
    /// <param name="info11">第十一个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持十二个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    /// <param name="info8">第八个参数</param>
    /// <param name="info9">第九个参数</param>
    /// <param name="info10">第十个参数</param>
    /// <param name="info11">第十一个参数</param>
    /// <param name="info12">第十二个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持十三个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    /// <param name="info8">第八个参数</param>
    /// <param name="info9">第九个参数</param>
    /// <param name="info10">第十个参数</param>
    /// <param name="info11">第十一个参数</param>
    /// <param name="info12">第十二个参数</param>
    /// <param name="info13">第十三个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12, T13 info13)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12, info13);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12, T13 info13)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12, info13));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持十四个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    /// <param name="info8">第八个参数</param>
    /// <param name="info9">第九个参数</param>
    /// <param name="info10">第十个参数</param>
    /// <param name="info11">第十一个参数</param>
    /// <param name="info12">第十二个参数</param>
    /// <param name="info13">第十三个参数</param>
    /// <param name="info14">第十四个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12, T13 info13, T14 info14)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12, info13, info14);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12, T13 info13, T14 info14)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12, info13, info14));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持十五个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    /// <param name="info8">第八个参数</param>
    /// <param name="info9">第九个参数</param>
    /// <param name="info10">第十个参数</param>
    /// <param name="info11">第十一个参数</param>
    /// <param name="info12">第十二个参数</param>
    /// <param name="info13">第十三个参数</param>
    /// <param name="info14">第十四个参数</param>
    /// <param name="info15">第十五个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12, T13 info13, T14 info14, T15 info15)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12, info13, info14, info15);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12, T13 info13, T14 info14, T15 info15)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12, info13, info14, info15));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 事件触发 支持十六个参数的事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="info1">第一个参数</param>
    /// <param name="info2">第二个参数</param>
    /// <param name="info3">第三个参数</param>
    /// <param name="info4">第四个参数</param>
    /// <param name="info5">第五个参数</param>
    /// <param name="info6">第六个参数</param>
    /// <param name="info7">第七个参数</param>
    /// <param name="info8">第八个参数</param>
    /// <param name="info9">第九个参数</param>
    /// <param name="info10">第十个参数</param>
    /// <param name="info11">第十一个参数</param>
    /// <param name="info12">第十二个参数</param>
    /// <param name="info13">第十三个参数</param>
    /// <param name="info14">第十四个参数</param>
    /// <param name="info15">第十五个参数</param>
    /// <param name="info16">第十六个参数</param>
    public void EventTrigger<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12, T13 info13, T14 info14, T15 info15, T16 info16)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>).actions?.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12, info13, info14, info15, info16);
        }
    }

    // 异步事件触发方法
    public async void EventTriggerAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(E_EventType eventName, T1 info1, T2 info2, T3 info3, T4 info4, T5 info5, T6 info6, T7 info7, T8 info8, T9 info9, T10 info10, T11 info11, T12 info12, T13 info13, T14 info14, T15 info15, T16 info16)
    {
        try
        {
            if (eventDic.TryGetValue(eventName, out var eventInfoBase))
            {
                var eventInfo = eventInfoBase as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>;
                if (eventInfo != null && eventInfo.actions != null)
                {
                    // 使用(UniTask) 开启线程池异步执行
                    await UniTask.RunOnThreadPool(() => eventInfo.actions.Invoke(info1, info2, info3, info4, info5, info6, info7, info8, info9, info10, info11, info12, info13, info14, info15, info16));
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"异步触发事件 {eventName} 时发生异常: {e.Message}\n{e.StackTrace}");
        }
    }

    // 优先级事件触发
    public void EventTriggerWithPriority<T>(E_EventType eventName, T info)
    {
        if (eventDic.TryGetValue(eventName, out var eventInfoBase))
        {
            if (eventInfoBase is PrioritizedEventInfo<T> prioritizedEventInfo)
            {
                prioritizedEventInfo.Invoke(info);
            }
            else
            {
                LogSystem.Error($"事件 {eventName} 不是优先级事件类型，无法使用优先级触发机制");
            }
        }
    }

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T>(E_EventType eventName, Action<T> func)
    {
        //如果已经存在该事件的委托记录 直接添加方法
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T>(func));
        }
    }



    public void AddEventListener(E_EventType eventName, Action func)
    {
        //如果已经存在该事件的委托记录 直接添加方法
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo(func));
        }
    }
    
    /// <summary>
    /// 添加事件监听（使用string作为key）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T>(string eventName, Action<T> func)
    {
        //如果已经存在该事件的委托记录 直接添加方法
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T>).actions += func;
        }
        else
        {
            stringEventDic.Add(eventName, new EventInfo<T>(func));
        }
    }



    public void AddEventListener(string eventName, Action func)
    {
        //如果已经存在该事件的委托记录 直接添加方法
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo).actions += func;
        }
        else
        {
            stringEventDic.Add(eventName, new EventInfo(func));
        }
    }
    
    /// <summary>
    /// 添加事件监听 支持两个参数的事件（使用string作为key）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2>(string eventName, Action<T1, T2> func)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2>).actions += func;
        }
        else
        {
            stringEventDic.Add(eventName, new EventInfo<T1, T2>(func));
        }
    }
    
    /// <summary>
    /// 添加事件监听 支持3个参数的事件（使用string作为key）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3>(string eventName, Action<T1, T2, T3> func)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2, T3>).actions += func;
        }
        else
        {
            stringEventDic.Add(eventName, new EventInfo<T1, T2, T3>(func));
        }
    }
    
    /// <summary>
    /// 添加事件监听 支持4个参数的事件（使用string作为key）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> func)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2, T3, T4>).actions += func;
        }
        else
        {
            stringEventDic.Add(eventName, new EventInfo<T1, T2, T3, T4>(func));
        }
    }
    
    /// <summary>
    /// 添加事件监听 支持5个参数的事件（使用string作为key）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> func)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2, T3, T4, T5>).actions += func;
        }
        else
        {
            stringEventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5>(func));
        }
    }
    
    /// <summary>
    /// 添加事件监听 支持6个参数的事件（使用string作为key）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> func)
    {
        if (stringEventDic.ContainsKey(eventName))
        {
            (stringEventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6>).actions += func;
        }
        else
        {
            stringEventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持两个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2>(E_EventType eventName, Action<T1, T2> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2>(func));
        }
    }


    /// <summary>
    /// 添加事件监听 支持3个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3>(E_EventType eventName, Action<T1, T2, T3> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持4个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4>(E_EventType eventName, Action<T1, T2, T3, T4> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持5个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5>(E_EventType eventName, Action<T1, T2, T3, T4, T5> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持6个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持7个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持8个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7, T8>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7, T8>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持9个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持10个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持11个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持12个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持13个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持14个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持15个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(func));
        }
    }

    /// <summary>
    /// 添加事件监听 支持16个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(func));
        }
    }

    // 带优先级的注册方法
    public void AddEventListenerWithPriority<T>(E_EventType eventName, Action<T> func, int priority = 0)
    {
        if (!eventDic.TryGetValue(eventName, out var eventInfoBase))
        {
            var newEventInfo = new PrioritizedEventInfo<T>();
            newEventInfo.AddAction(func, priority);
            eventDic.Add(eventName, newEventInfo);
        }
        else
        {
            if (eventInfoBase is PrioritizedEventInfo<T> prioritizedEventInfo)
            {
                prioritizedEventInfo.AddAction(func, priority);
            }
            else
            {
                LogSystem.Error($"事件 {eventName} 不是优先级事件类型，无法添加带优先级的监听器");
            }
        }
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T>(E_EventType eventName, Action<T> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T>).actions -= func;
    }

    public void RemoveEventListener(E_EventType eventName, Action func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo).actions -= func;
    }
    
    public void RemoveEventListener<T>(string eventName, Action<T> func)
    {
        if (stringEventDic.ContainsKey(eventName))
            (stringEventDic[eventName] as EventInfo<T>).actions -= func;
    }

    public void RemoveEventListener(string eventName, Action func)
    {
        if (stringEventDic.ContainsKey(eventName))
            (stringEventDic[eventName] as EventInfo).actions -= func;
    }
    
    public void RemoveEventListener<T1, T2>(string eventName, Action<T1, T2> func)
    {
        if (stringEventDic.ContainsKey(eventName))
            (stringEventDic[eventName] as EventInfo<T1, T2>).actions -= func;
    }
    
    public void RemoveEventListener<T1, T2, T3>(string eventName, Action<T1, T2, T3> func)
    {
        if (stringEventDic.ContainsKey(eventName))
            (stringEventDic[eventName] as EventInfo<T1, T2, T3>).actions -= func;
    }
    
    public void RemoveEventListener<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> func)
    {
        if (stringEventDic.ContainsKey(eventName))
            (stringEventDic[eventName] as EventInfo<T1, T2, T3, T4>).actions -= func;
    }
    
    public void RemoveEventListener<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> func)
    {
        if (stringEventDic.ContainsKey(eventName))
            (stringEventDic[eventName] as EventInfo<T1, T2, T3, T4, T5>).actions -= func;
    }
    
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> func)
    {
        if (stringEventDic.ContainsKey(eventName))
            (stringEventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持两个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2>(E_EventType eventName, Action<T1, T2> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持3个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3>(E_EventType eventName, Action<T1, T2, T3> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持4个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4>(E_EventType eventName, Action<T1, T2, T3, T4> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持5个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5>(E_EventType eventName, Action<T1, T2, T3, T4, T5> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持6个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持7个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持8个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7, T8>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持9个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持10个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持11个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持12个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持13个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持14个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持15个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>).actions -= func;
    }

    /// <summary>
    /// 移除事件监听 支持16个参数的事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(E_EventType eventName, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>).actions -= func;
    }

    // 移除带优先级的事件监听器
    public void RemoveEventListenerWithPriority<T>(E_EventType eventName, Action<T> func)
    {
        if (eventDic.TryGetValue(eventName, out var eventInfoBase))
        {
            if (eventInfoBase is PrioritizedEventInfo<T> prioritizedEventInfo)
            {
                prioritizedEventInfo.RemoveAction(func);
                // 如果没有监听器了，就可以移除该事件避免内存泄露
                if (prioritizedEventInfo.prioritizedActions.Count == 0)
                {
                    eventDic.Remove(eventName);
                }
            }
            else
            {
                LogSystem.Error($"事件 {eventName} 不是优先级事件类型，无法移除带优先级的监听器");
            }
        }
        else
        {
            LogSystem.Warning($"事件 {eventName} 不存在，无法移除监听器");
        }
    }

    /// <summary>
    /// 清空事件的监听
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
    private void ClearStringEvent()
    {
        stringEventDic.Clear();
    }

    private void Clear(string eventName)
    {
        if (stringEventDic.ContainsKey(eventName))
            stringEventDic.Remove(eventName);
    }

    /// <summary>
    /// 清空指定的某一个事件的监听
    /// </summary>
    /// <param name="eventName"></param>
    public void Clear(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }
}

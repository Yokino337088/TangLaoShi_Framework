using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 公共Mono模块管理器
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr>
{
    private event Action updateEvent;
    private event Action fixedUpdateEvent;
    private event Action lateUpdateEvent;

    // 用于存储协程的字典，键为协程标识，值为协程对象
    private Dictionary<string, Coroutine> coroutineDict = new Dictionary<string, Coroutine>();

    /// <summary>
    /// 添加Update帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddUpdateListener(Action updateFun)
    {
        updateEvent += updateFun;
    }

    /// <summary>
    /// 移除Update帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveUpdateListener(Action updateFun)
    {
        updateEvent -= updateFun;
    }

    /// <summary>
    /// 添加FixedUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddFixedUpdateListener(Action updateFun)
    {
        fixedUpdateEvent += updateFun;
    }
    /// <summary>
    /// 移除FixedUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveFixedUpdateListener(Action updateFun)
    {
        fixedUpdateEvent -= updateFun;
    }

    /// <summary>
    /// 添加LateUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddLateUpdateListener(Action updateFun)
    {
        lateUpdateEvent += updateFun;
    }

    /// <summary>
    /// 移除LateUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveLateUpdateListener(Action updateFun)
    {
        lateUpdateEvent -= updateFun;
    }


    /// <summary>
    /// 开启多携程的方法
    /// </summary>
    /// <param name="routine"></param>
    /// <param name="coroutineId"></param>
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerator routine, string coroutineId)
    {
        string newCoroutineId = coroutineId;
        int count = 1;
        while (coroutineDict.ContainsKey(newCoroutineId))
        {
            newCoroutineId = $"{coroutineId}_{count}";
            count++;
        }

        Coroutine coroutine = base.StartCoroutine(routine);
        coroutineDict.Add(newCoroutineId, coroutine);
        return coroutine;
    }


    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }
}

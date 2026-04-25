using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

/// <summary>
/// 场景切换管理器 主要用于切换场景
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr() { }

    /// <summary>
    /// 当前场景索引
    /// </summary>
    public int NowSceneIndex => SceneManager.GetActiveScene().buildIndex;

    public event Action onSceneLoadStart;
    public event Action onSceneLoadComplete;

    //同步切换场景的方法
    public void LoadScene(string name, Action callBack = null)
    {
        //切换场景
        SceneManager.LoadScene(name);
        //调用回调
        callBack?.Invoke();
        callBack = null;
    }

    //异步切换场景的方法
    public async void LoadSceneAsyn(string name, Action callBack = null)
    {
        await ReallyLoadSceneAsyn(name, callBack);
    }

    public async void LoadSceneAsyn(int sceneIndex, Action callBack = null)
    {
        await ReallyLoadSceneAsyn(sceneIndex, callBack);
    }


    private async UniTask ReallyLoadSceneAsyn(string name, Action callBack)
    {
        //先清除对象池
        GOPoolMgr.Instance.ClearPool();

        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        onSceneLoadStart?.Invoke();
        //不停的在异步方法中每帧检测是否加载结束 如果加载结束就不会进这个循环每帧执行了
        while (!ao.isDone)
        {
            //可以在这里利用事件中心 每一帧将进度发送给想要得到的地方
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange, ao.progress);
            await UniTask.Yield();
        }
        //避免最后一帧直接结束了 没有同步1出去
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange, 1);
        onSceneLoadComplete?.Invoke();
        callBack?.Invoke();
        callBack = null;
    }

    private async UniTask ReallyLoadSceneAsyn(int sceneIndex, Action callBack)
    {
        //先清除对象池
        GOPoolMgr.Instance.ClearPool();

        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneIndex);
        onSceneLoadStart?.Invoke();
        //不停的在异步方法中每帧检测是否加载结束 如果加载结束就不会进这个循环每帧执行了
        while (!ao.isDone)
        {
            //可以在这里利用事件中心 每一帧将进度发送给想要得到的地方
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange, ao.progress);
            await UniTask.Yield();
        }
        //避免最后一帧直接结束了 没有同步1出去
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange, 1);
        onSceneLoadComplete?.Invoke();
        callBack?.Invoke();
        callBack = null;
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用于进行加载AB相关资源的整合 在开发中可以通过EditorResMgr去加载对应资源进行测试
/// </summary>
public class ABResMgr : BaseManager<ABResMgr>
{
    //如果是true会通过EditorResMgr去加载 如果是false会通过ABMgr AB包的形式去加载
    private bool isDebug = true;

    private ABResMgr() { }

    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T : Object
    {
#if UNITY_EDITOR
        if (isDebug)
        {
            //使用ABResourceLoader加载资源，替代EditorResMgr
            ABResourceLoader.Instance.LoadResourceAsync<T>(abName, resName, callBack, isSync);
        }
        else
        {
            ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
        }
#else
        ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
#endif
    }

    public T LoadRes<T>(string abName, string resName) where T : Object
    {
        T res;
        if (isDebug)
        {
            //使用ABResourceLoader加载资源，替代EditorResMgr
            res = ABResourceLoader.Instance.LoadResource<T>(abName, resName);
            return res as T;
        }
        else
        {
            res = ABMgr.Instance.LoadRes<T>(abName, resName);
            return res as T;
        }
    }
}

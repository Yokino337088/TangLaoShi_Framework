using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class UWQResMgr : SingletonAutoMono<UWQResMgr>
{
    /// <summary>
    /// 利用UnityWebRequest去加载资源
    /// </summary>
    /// <typeparam name="T">类型只能是string、byte[]、Texture、AssetBundle 不能是其他类型 目前不支持</typeparam>
    /// <param name="path">资源路径、要自己加上协议 http、ftp、file</param>
    /// <param name="callBack">加载成功的回调函数</param>
    /// <param name="failCallBack">加载失败的回调函数</param>
    public void LoadRes<T>(string path, UnityAction<T> callBack, UnityAction failCallBack) where T : class
    {
        StartCoroutine(ReallyLoadRes<T>(path, callBack, failCallBack));
    }

    private IEnumerator ReallyLoadRes<T>(string path, UnityAction<T> callBack, UnityAction failCallBack) where T:class
    {
        //string
        //byte[]
        //Texture
        //AssetBundle
        Type type = typeof(T);
        //用于加载的对象
        UnityWebRequest req = null;
        if (type == typeof(string) ||
            type == typeof(byte[]))
            req = UnityWebRequest.Get(path);
        else if (type == typeof(Texture))
            req = UnityWebRequestTexture.GetTexture(path);
        else if (type == typeof(AssetBundle))
            req = UnityWebRequestAssetBundle.GetAssetBundle(path);
        else
        {
            failCallBack?.Invoke();
            yield break;
        }

        yield return req.SendWebRequest();
        //如果加载成功 
        if (req.result == UnityWebRequest.Result.Success)
        {
            if (type == typeof(string))
                callBack?.Invoke(req.downloadHandler.text as T);
            else if (type == typeof(byte[]))
                callBack?.Invoke(req.downloadHandler.data as T);
            else if (type == typeof(Texture))
                callBack?.Invoke(DownloadHandlerTexture.GetContent(req) as T);
            else if (type == typeof(AssetBundle))
                callBack?.Invoke(DownloadHandlerAssetBundle.GetContent(req) as T);
        }
        else
            failCallBack?.Invoke();
        //释放UWQ对象
        req.Dispose();
    }
}

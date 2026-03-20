using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// AB包资源管理器 - 统一的资源加载入口
/// 支持编辑器模式和运行时模式的无缝切换
/// </summary>
public class ABResMgr : BaseManager<ABResMgr>
{
    //如果为true 通过ABResourceLoader去加载（编辑器模式） 如果为false 通过ABMgr AB包加载方式去加载（运行时模式）
    private bool isDebug = true;

    private ABResMgr() { }

    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T : Object
    {
#if UNITY_EDITOR
        if (isDebug)
        {
            //使用ABResourceLoader加载资源 通过EditorResMgr
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
            //使用ABResourceLoader加载资源 通过EditorResMgr
            res = ABResourceLoader.Instance.LoadResource<T>(abName, resName);
            return res as T;
        }
        else
        {
            res = ABMgr.Instance.LoadRes<T>(abName, resName);
            return res as T;
        }
    }

    #region 场景预加载功能 - 统一的预加载入口

    /// <summary>
    /// 预加载多个AB包（用于过场景时提前加载资源）
    /// 支持编辑器环境和发布环境，会自动根据当前环境选择合适的预加载方式
    /// </summary>
    /// <param name="abNames">需要预加载的AB包名称列表</param>
    /// <param name="progressCallback">进度回调，参数为当前进度(0-1)</param>
    /// <param name="completeCallback">完成回调，参数为是否全部加载成功</param>
    public void PreloadABPackages(List<string> abNames, UnityAction<float> progressCallback = null, UnityAction<bool> completeCallback = null)
    {
#if UNITY_EDITOR
        if (isDebug)
        {
            // 编辑器模式下使用ABResourceLoader进行预加载验证
            ABResourceLoader.Instance.PreloadResources(abNames, progressCallback, completeCallback);
        }
        else
        {
            // 编辑器模式下也使用ABMgr进行实际预加载（调试用）
            ABMgr.Instance.PreloadABPackages(abNames, progressCallback, completeCallback);
        }
#else
        // 发布环境下使用ABMgr进行实际预加载
        ABMgr.Instance.PreloadABPackages(abNames, progressCallback, completeCallback);
#endif
    }

    /// <summary>
    /// 预加载单个AB包及其所有资源
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <param name="progressCallback">进度回调</param>
    /// <param name="completeCallback">完成回调，返回加载的所有资源</param>
    public void PreloadABPackageWithAllAssets(string abName, UnityAction<float> progressCallback = null, UnityAction<Object[]> completeCallback = null)
    {
#if UNITY_EDITOR
        if (isDebug)
        {
            // 编辑器模式下加载所有相关资源
            ABResourceLoader.Instance.PreloadAllAssetsInPackage(abName, progressCallback, completeCallback);
        }
        else
        {
            ABMgr.Instance.PreloadABPackageWithAllAssets(abName, progressCallback, completeCallback);
        }
#else
        ABMgr.Instance.PreloadABPackageWithAllAssets(abName, progressCallback, completeCallback);
#endif
    }

    /// <summary>
    /// 取消预加载（释放预加载但未使用的资源）
    /// </summary>
    /// <param name="abNames">需要取消预加载的AB包名称列表</param>
    public void CancelPreload(List<string> abNames)
    {
#if UNITY_EDITOR
        if (isDebug)
        {
            ABResourceLoader.Instance.CancelPreload(abNames);
        }
        else
        {
            ABMgr.Instance.CancelPreload(abNames);
        }
#else
        ABMgr.Instance.CancelPreload(abNames);
#endif
    }

    /// <summary>
    /// 获取AB包预加载状态
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <returns>预加载状态</returns>
    public ABMgr.PreloadState GetPreloadState(string abName)
    {
#if UNITY_EDITOR
        if (isDebug)
        {
            // 编辑器模式下始终返回Loaded（资源已通过AssetDatabase加载）
            return ABMgr.PreloadState.Loaded;
        }
        else
        {
            return ABMgr.Instance.GetPreloadState(abName);
        }
#else
        return ABMgr.Instance.GetPreloadState(abName);
#endif
    }

    /// <summary>
    /// 预加载场景配置数据类
    /// 用于配置场景切换时需要预加载的AB包
    /// </summary>
    [System.Serializable]
    public class ScenePreloadConfig
    {
        public string SceneName;                    // 场景名称
        public List<string> ABPackagesToPreload;    // 需要预加载的AB包列表
        public bool PreloadAllAssets;               // 是否预加载包中所有资源
        public float MinPreloadTime;                // 最小预加载时间（用于显示加载界面）
    }

    /// <summary>
    /// 根据场景配置预加载AB包
    /// </summary>
    /// <param name="config">场景预加载配置</param>
    /// <param name="progressCallback">进度回调</param>
    /// <param name="completeCallback">完成回调</param>
    public void PreloadForScene(ScenePreloadConfig config, UnityAction<float> progressCallback = null, UnityAction<bool> completeCallback = null)
    {
        if (config == null || config.ABPackagesToPreload == null || config.ABPackagesToPreload.Count == 0)
        {
            LogSystem.Warning("场景预加载配置为空，无需预加载");
            completeCallback?.Invoke(true);
            return;
        }

        LogSystem.Info($"开始为场景 {config.SceneName} 预加载资源，共 {config.ABPackagesToPreload.Count} 个AB包");

        if (config.PreloadAllAssets)
        {
            // 预加载所有资源
            PreloadMultiplePackagesWithAssets(config.ABPackagesToPreload, progressCallback, completeCallback);
        }
        else
        {
            // 只预加载AB包
            PreloadABPackages(config.ABPackagesToPreload, progressCallback, completeCallback);
        }
    }

    /// <summary>
    /// 预加载多个AB包及其所有资源
    /// </summary>
    private async void PreloadMultiplePackagesWithAssets(List<string> abNames, UnityAction<float> progressCallback, UnityAction<bool> completeCallback)
    {
        int totalCount = abNames.Count;
        int currentCount = 0;
        bool allSuccess = true;

        foreach (string abName in abNames)
        {
            bool packageSuccess = false;
            await UniTask.WaitUntil(() =>
            {
                PreloadABPackageWithAllAssets(abName, (progress) =>
                {
                    float totalProgress = (currentCount + progress) / totalCount;
                    progressCallback?.Invoke(totalProgress);
                }, (assets) =>
                {
                    packageSuccess = assets != null;
                });
                return true;
            });

            if (!packageSuccess)
            {
                allSuccess = false;
            }
            currentCount++;
        }

        progressCallback?.Invoke(1f);
        completeCallback?.Invoke(allSuccess);
    }

    #endregion
}

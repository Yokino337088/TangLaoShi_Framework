using Cysharp.Threading.Tasks;
using System;
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

    /// <summary>
    /// 泛型异步加载资源（回调版本）
    /// </summary>
    public void LoadResAsync<T>(string abName, string resName, Action<T> callBack, bool isSync = false) where T : UnityEngine.Object
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

    /// <summary>
    /// 泛型异步加载资源（返回对象版本）
    /// 不需要传入回调函数，直接返回加载的资源对象
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="abName">AB包名称</param>
    /// <param name="resName">资源名称</param>
    /// <param name="isSync">是否同步加载</param>
    /// <returns>加载的资源对象</returns>
    public async UniTask<T> LoadResAsync<T>(string abName, string resName, bool isSync = false) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (isDebug)
        {
            //使用ABResourceLoader加载资源
            return await ABResourceLoader.Instance.LoadResourceAsync<T>(abName, resName, isSync);
        }
        else
        {
            return await ABMgr.Instance.LoadResAsync<T>(abName, resName, isSync);
        }
#else
        return await ABMgr.Instance.LoadResAsync<T>(abName, resName, isSync);
#endif
    }

    public T LoadRes<T>(string abName, string resName) where T : UnityEngine.Object
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
    public void PreloadABPackages(List<string> abNames, Action<float> progressCallback = null, Action<bool> completeCallback = null)
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
    public void PreloadABPackageWithAllAssets(string abName, Action<float> progressCallback = null, Action<UnityEngine.Object[]> completeCallback = null)
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
    /// 自动检测并卸载无用的AB包（引用计数为0的包）
    /// </summary>
    public void AutoUnloadUnusedABs()
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            ABMgr.Instance.AutoUnloadUnusedABs();
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需手动卸载AB包");
        }
#else
        ABMgr.Instance.AutoUnloadUnusedABs();
#endif
    }
    
    /// <summary>
    /// 启动自动检测和卸载无用AB包的定时任务
    /// </summary>
    /// <param name="intervalSeconds">检测间隔（秒）</param>
    public void StartAutoUnloadTask(float intervalSeconds = 30f)
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            ABMgr.Instance.StartAutoUnloadTask(intervalSeconds);
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需启动自动卸载任务");
        }
#else
        ABMgr.Instance.StartAutoUnloadTask(intervalSeconds);
#endif
    }
    
    /// <summary>
    /// 手动触发一次无用AB包的检测和卸载
    /// </summary>
    public void TriggerUnloadCheck()
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            ABMgr.Instance.TriggerUnloadCheck();
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需手动触发卸载检查");
        }
#else
        ABMgr.Instance.TriggerUnloadCheck();
#endif
    }
    
    /// <summary>
    /// 获取AB包的引用计数
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <returns>引用计数</returns>
    public int GetABRefCount(string abName)
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            return ABMgr.Instance.GetRefCount(abName);
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，引用计数无意义");
            return 0;
        }
#else
        return ABMgr.Instance.GetRefCount(abName);
#endif
    }
    
    /// <summary>
    /// 自动检测闲置AB包并减小引用计数
    /// 类似于GC算法，定期检查AB包的使用状态
    /// </summary>
    public void AutoReduceIdleABRefCount()
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            ABMgr.Instance.AutoReduceIdleABRefCount();
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需手动管理AB包引用计数");
        }
#else
        ABMgr.Instance.AutoReduceIdleABRefCount();
#endif
    }
    
    /// <summary>
    /// 启动自动检测闲置AB包并减小引用计数的定时任务
    /// </summary>
    /// <param name="intervalSeconds">检测间隔（秒）</param>
    /// <param name="idleThreshold">闲置时间阈值（秒）</param>
    public void StartIdleABCheckTask(float intervalSeconds = 30f, float idleThreshold = 60f)
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            ABMgr.Instance.StartIdleABCheckTask(intervalSeconds, idleThreshold);
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需启动闲置检测任务");
        }
#else
        ABMgr.Instance.StartIdleABCheckTask(intervalSeconds, idleThreshold);
#endif
    }
    
    /// <summary>
    /// 手动触发一次闲置AB包的检测和引用计数减小
    /// </summary>
    public void TriggerIdleABCheck()
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            ABMgr.Instance.TriggerIdleABCheck();
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需手动触发闲置检测");
        }
#else
        ABMgr.Instance.TriggerIdleABCheck();
#endif
    }
    
    /// <summary>
    /// 设置闲置时间阈值
    /// </summary>
    /// <param name="seconds">闲置时间阈值（秒）</param>
    public void SetIdleTimeThreshold(float seconds)
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            ABMgr.Instance.SetIdleTimeThreshold(seconds);
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，闲置时间阈值设置无意义");
        }
#else
        ABMgr.Instance.SetIdleTimeThreshold(seconds);
#endif
    }
    
    /// <summary>
    /// 更新AB包的使用时间
    /// </summary>
    /// <param name="abName">AB包名称</param>
    public void UpdateABLastUsedTime(string abName)
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            ABMgr.Instance.UpdateABLastUsedTime(abName);
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，使用时间更新无意义");
        }
#else
        ABMgr.Instance.UpdateABLastUsedTime(abName);
#endif
    }
    
    /// <summary>
    /// 获取AB包的最后使用时间
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <returns>最后使用时间</returns>
    public float GetABLastUsedTime(string abName)
    {
#if UNITY_EDITOR
        if (!isDebug)
        {
            return ABMgr.Instance.GetABLastUsedTime(abName);
        }
        else
        {
            LogSystem.Info("编辑器模式下，资源由Unity自动管理，使用时间获取无意义");
            return -1f;
        }
#else
        return ABMgr.Instance.GetABLastUsedTime(abName);
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
    public void PreloadForScene(ScenePreloadConfig config, Action<float> progressCallback = null, Action<bool> completeCallback = null)
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
    private async void PreloadMultiplePackagesWithAssets(List<string> abNames, Action<float> progressCallback, Action<bool> completeCallback)
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

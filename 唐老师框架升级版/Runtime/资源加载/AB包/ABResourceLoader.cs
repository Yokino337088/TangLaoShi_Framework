using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;

public class ABResourceLoader : BaseManager<ABResourceLoader>
{
    private Dictionary<string, ABResourceInfo> resourceInfoMap = new Dictionary<string, ABResourceInfo>();
    private string configFilePath = "ABResourceConfig.json";
    private bool isConfigLoaded = false;
    
    /// <summary>
    /// 设置配置文件路径
    /// </summary>
    /// <param name="path">配置文件路径</param>
    public void SetConfigFilePath(string path)
    {
        configFilePath = path;
        ReloadConfig();
    }
    
    /// <summary>
    /// 获取当前配置文件路径
    /// </summary>
    /// <returns>配置文件路径</returns>
    public string GetConfigFilePath()
    {
        return configFilePath;
    }
    
    /// <summary>
    /// 尝试从PathConfig.json加载配置文件路径
    /// </summary>
    private void TryLoadPathFromConfig()
    {
#if UNITY_EDITOR
        string pathConfigPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "唐老师框架迭代版/Editor/Config/PathConfig.json");
        if (System.IO.File.Exists(pathConfigPath))
        {
            try
            {
                string jsonContent = System.IO.File.ReadAllText(pathConfigPath);
                // 简单的JSON解析，提取abResourceConfigPath字段
                int startIndex = jsonContent.IndexOf("\"abResourceConfigPath\":");
                if (startIndex != -1)
                {
                    startIndex += "\"abResourceConfigPath\":".Length;
                    // 跳过空格
                    while (startIndex < jsonContent.Length && char.IsWhiteSpace(jsonContent[startIndex]))
                    {
                        startIndex++;
                    }
                    // 提取字符串值
                    if (startIndex < jsonContent.Length && jsonContent[startIndex] == '"')
                    {
                        startIndex++;
                        int endIndex = jsonContent.IndexOf('"', startIndex);
                        if (endIndex != -1)
                        {
                            string path = jsonContent.Substring(startIndex, endIndex - startIndex);
                            if (!string.IsNullOrEmpty(path))
                            {
                                configFilePath = path;
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("从PathConfig.json加载AB资源配置路径失败: " + e.Message);
            }
        }
#endif
    }
    
    private ABResourceLoader()
    {
        
        // 尝试从PathConfig.json加载配置文件路径
        TryLoadPathFromConfig();
        LoadResourceConfig();
    }
    
    /// <summary>
    /// 加载资源配置文件
    /// </summary>
    private void LoadResourceConfig()
    {
        // 如果configFilePath是绝对路径，直接使用
        if (Path.IsPathRooted(configFilePath))
        {
            if (File.Exists(configFilePath))
            {
                LoadConfigFromFile(configFilePath);
                return;
            }
        }
        else
        {
            // 尝试从streamingAssetsPath加载
            string configPath = Path.Combine(Application.streamingAssetsPath, configFilePath);
            if (File.Exists(configPath))
            {
                LoadConfigFromFile(configPath);
                return;
            }
            
            // 尝试从Assets目录加载（编辑器模式）
            // 处理configFilePath可能包含"Assets/"前缀的情况
            string configFileName = configFilePath;
            if (configFileName.StartsWith("Assets/"))
            {
                configFileName = configFileName.Substring("Assets/".Length);
            }
            configPath = Path.Combine(Application.dataPath, configFileName);
            if (File.Exists(configPath))
            {
                LoadConfigFromFile(configPath);
                return;
            }
        }
        
        LogSystem.Warning("AB资源配置文件未找到: " + configFilePath);
    }
    
    /// <summary>
    /// 从文件加载配置
    /// </summary>
    /// <param name="filePath">配置文件路径</param>
    private void LoadConfigFromFile(string filePath)
    {
        try
        {
            string jsonContent = File.ReadAllText(filePath);
            ABResourceConfig config = JsonUtility.FromJson<ABResourceConfig>(jsonContent);
            
            if (config != null && config.resources != null)
            {
                resourceInfoMap.Clear();
                foreach (var info in config.resources)
                {
                    // 使用AB包名+资源名作为键，与查找逻辑保持一致
                    string key = info.ABPackageName + "/" + info.ResourceName;
                    resourceInfoMap[key] = info;
                }
                isConfigLoaded = true;
                LogSystem.Info("AB资源配置加载成功，共加载 " + resourceInfoMap.Count + " 个资源信息");
            }
        }
        catch (System.Exception e)
        {
            LogSystem.Error("加载AB资源配置失败: " + e.Message);
        }
    }
    
    /// <summary>
    /// 根据AB包名和资源名加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="abName">AB包名称</param>
    /// <param name="resName">资源名称</param>
    /// <returns>加载的资源</returns>
    public T LoadResource<T>(string abName, string resName) where T : UnityEngine.Object
    {
        // 尝试通过配置查找资源路径
        string resourceKey = abName + "/" + resName;
        if (resourceInfoMap.TryGetValue(resourceKey, out ABResourceInfo info))
        {
            // 在编辑器模式下直接从AssetDatabase加载
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(info.ResourcePath);
#else
            // 在运行时通过AB包加载
            return ABMgr.Instance.LoadRes<T>(info.ABPackageName, resName);
#endif
        }
        
        // 如果配置中未找到，尝试通过ABMgr加载
        return ABMgr.Instance.LoadRes<T>(abName, resName);
    }
    
    /// <summary>
    /// 异步加载资源（回调版本）
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="abName">AB包名称</param>
    /// <param name="resName">资源名称</param>
    /// <param name="callback">加载完成回调</param>
    /// <param name="isSync">是否同步加载</param>
    public void LoadResourceAsync<T>(string abName, string resName, Action<T> callback, bool isSync = false) where T : UnityEngine.Object
    {
        // 尝试通过配置查找资源路径
        string resourceKey = abName + "/" + resName;
        if (resourceInfoMap.TryGetValue(resourceKey, out ABResourceInfo info))
        {
            // 在编辑器模式下直接从AssetDatabase加载
#if UNITY_EDITOR
            T resource = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(info.ResourcePath);
            callback?.Invoke(resource);
#else
            // 在运行时通过AB包加载
            ABMgr.Instance.LoadResAsync<T>(info.ABPackageName, resName, callback, isSync);
#endif
            return;
        }
        
        // 如果配置中未找到，尝试通过ABMgr加载
        //ABMgr.Instance.LoadResAsync<T>(abName, resName, callback, isSync);
    }

    /// <summary>
    /// 异步加载资源（返回对象版本）
    /// 不需要传入回调函数，直接返回加载的资源对象
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="abName">AB包名称</param>
    /// <param name="resName">资源名称</param>
    /// <param name="isSync">是否同步加载</param>
    /// <returns>加载的资源对象</returns>
    public async UniTask<T> LoadResourceAsync<T>(string abName, string resName, bool isSync = false) where T : UnityEngine.Object
    {
        // 尝试通过配置查找资源路径
        string resourceKey = abName + "/" + resName;
        if (resourceInfoMap.TryGetValue(resourceKey, out ABResourceInfo info))
        {
            // 在编辑器模式下直接从AssetDatabase加载
#if UNITY_EDITOR
            T resource = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(info.ResourcePath);
            return resource;
#else
            // 在运行时通过AB包加载
            return await ABMgr.Instance.LoadResAsync<T>(info.ABPackageName, resName, isSync);
#endif
        }
        
        // 如果配置中未找到，返回null
        return null;
    }
    
    /// <summary>
    /// 重新加载配置
    /// </summary>
    public void ReloadConfig()
    {
        isConfigLoaded = false;
        LoadResourceConfig();
    }
    
    /// <summary>
    /// 检查资源是否在配置中存在
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <param name="resName">资源名称</param>
    /// <returns>资源是否存在</returns>
    public bool ContainsResource(string abName, string resName)
    {
        string resourceKey = abName + "/" + resName;
        return resourceInfoMap.ContainsKey(resourceKey);
    }
    
    /// <summary>
    /// 获取资源信息
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <param name="resName">资源名称</param>
    /// <returns>资源信息</returns>
    public ABResourceInfo GetResourceInfo(string abName, string resName)
    {
        string resourceKey = abName + "/" + resName;
        resourceInfoMap.TryGetValue(resourceKey, out ABResourceInfo info);
        return info;
    }
    
    /// <summary>
    /// 自动检测并卸载无用的AB包（引用计数为0的包）
    /// 编辑器模式下，资源由Unity自动管理，此方法仅记录日志
    /// </summary>
    public void AutoUnloadUnusedABs()
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需手动卸载AB包");
#else
        // 非编辑器模式下，委托给ABMgr处理
        ABMgr.Instance.AutoUnloadUnusedABs();
#endif
    }
    
    /// <summary>
    /// 启动自动检测和卸载无用AB包的定时任务
    /// 编辑器模式下，资源由Unity自动管理，此方法仅记录日志
    /// </summary>
    /// <param name="intervalSeconds">检测间隔（秒）</param>
    public void StartAutoUnloadTask(float intervalSeconds = 30f)
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需启动自动卸载任务");
#else
        // 非编辑器模式下，委托给ABMgr处理
        ABMgr.Instance.StartAutoUnloadTask(intervalSeconds);
#endif
    }
    
    /// <summary>
    /// 手动触发一次无用AB包的检测和卸载
    /// 编辑器模式下，资源由Unity自动管理，此方法仅记录日志
    /// </summary>
    public void TriggerUnloadCheck()
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需手动触发卸载检查");
#else
        // 非编辑器模式下，委托给ABMgr处理
        ABMgr.Instance.TriggerUnloadCheck();
#endif
    }
    
    /// <summary>
    /// 获取AB包的引用计数
    /// 编辑器模式下，资源由Unity自动管理，引用计数无意义
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <returns>引用计数</returns>
    public int GetABRefCount(string abName)
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，引用计数无意义");
        return 0;
#else
        // 非编辑器模式下，委托给ABMgr处理
        return ABMgr.Instance.GetRefCount(abName);
#endif
    }
    
    /// <summary>
    /// 自动检测闲置AB包并减小引用计数
    /// 编辑器模式下，资源由Unity自动管理，此方法仅记录日志
    /// </summary>
    public void AutoReduceIdleABRefCount()
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需手动管理AB包引用计数");
#else
        // 非编辑器模式下，委托给ABMgr处理
        ABMgr.Instance.AutoReduceIdleABRefCount();
#endif
    }
    
    /// <summary>
    /// 启动自动检测闲置AB包并减小引用计数的定时任务
    /// 编辑器模式下，资源由Unity自动管理，此方法仅记录日志
    /// </summary>
    /// <param name="intervalSeconds">检测间隔（秒）</param>
    /// <param name="idleThreshold">闲置时间阈值（秒）</param>
    public void StartIdleABCheckTask(float intervalSeconds = 30f, float idleThreshold = 60f)
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需启动闲置检测任务");
#else
        // 非编辑器模式下，委托给ABMgr处理
        ABMgr.Instance.StartIdleABCheckTask(intervalSeconds, idleThreshold);
#endif
    }
    
    /// <summary>
    /// 手动触发一次闲置AB包的检测和引用计数减小
    /// 编辑器模式下，资源由Unity自动管理，此方法仅记录日志
    /// </summary>
    public void TriggerIdleABCheck()
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，无需手动触发闲置检测");
#else
        // 非编辑器模式下，委托给ABMgr处理
        ABMgr.Instance.TriggerIdleABCheck();
#endif
    }
    
    /// <summary>
    /// 设置闲置时间阈值
    /// 编辑器模式下，资源由Unity自动管理，此方法仅记录日志
    /// </summary>
    /// <param name="seconds">闲置时间阈值（秒）</param>
    public void SetIdleTimeThreshold(float seconds)
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，闲置时间阈值设置无意义");
#else
        // 非编辑器模式下，委托给ABMgr处理
        ABMgr.Instance.SetIdleTimeThreshold(seconds);
#endif
    }
    
    /// <summary>
    /// 更新AB包的使用时间
    /// 编辑器模式下，资源由Unity自动管理，此方法仅记录日志
    /// </summary>
    /// <param name="abName">AB包名称</param>
    public void UpdateABLastUsedTime(string abName)
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，使用时间更新无意义");
#else
        // 非编辑器模式下，委托给ABMgr处理
        ABMgr.Instance.UpdateABLastUsedTime(abName);
#endif
    }
    
    /// <summary>
    /// 获取AB包的最后使用时间
    /// 编辑器模式下，资源由Unity自动管理，此方法仅记录日志
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <returns>最后使用时间</returns>
    public float GetABLastUsedTime(string abName)
    {
#if UNITY_EDITOR
        LogSystem.Info("编辑器模式下，资源由Unity自动管理，使用时间获取无意义");
        return -1f;
#else
        // 非编辑器模式下，委托给ABMgr处理
        return ABMgr.Instance.GetABLastUsedTime(abName);
#endif
    }
    
    [System.Serializable]
    public class ABResourceInfo
    {
        public string ResourceName;
        public string ResourcePath;
        public string ABPackageName;
    }
    
    [System.Serializable]
    private class ABResourceConfig
    {
        public List<ABResourceInfo> resources;
    }

    #region 编辑器环境预加载功能

    /// <summary>
    /// 预加载资源（编辑器环境下模拟预加载行为）
    /// 在编辑器模式下，预加载实际上是验证资源是否存在并缓存资源信息
    /// </summary>
    /// <param name="abNames">AB包名称列表（在编辑器下用于查找对应目录的资源）</param>
    /// <param name="progressCallback">进度回调(0-1)</param>
    /// <param name="completeCallback">完成回调，参数为是否全部验证成功</param>
    public async void PreloadResources(List<string> abNames, Action<float> progressCallback = null, Action<bool> completeCallback = null)
    {
        bool allSuccess = await PreloadResourcesAsync(abNames, progressCallback);
        completeCallback?.Invoke(allSuccess);
    }

    /// <summary>
    /// 预加载资源的异步实现（编辑器环境）
    /// </summary>
    private async UniTask<bool> PreloadResourcesAsync(List<string> abNames, Action<float> progressCallback)
    {
        if (abNames == null || abNames.Count == 0)
        {
            LogSystem.Warning("预加载列表为空，无需加载");
            progressCallback?.Invoke(1f);
            return true;
        }

        LogSystem.Info($"编辑器模式：开始预加载验证，共 {abNames.Count} 个AB包");

        int totalCount = abNames.Count;
        int successCount = 0;
        int failCount = 0;

        for (int i = 0; i < abNames.Count; i++)
        {
            string abName = abNames[i];
            float progress = (float)(i + 1) / totalCount;

            if (string.IsNullOrEmpty(abName))
            {
                progressCallback?.Invoke(progress);
                continue;
            }

            // 在编辑器模式下，验证该AB包对应的资源是否存在
            bool exists = ValidateABPackageResources(abName);
            if (exists)
            {
                successCount++;
                LogSystem.Info($"编辑器预加载验证：AB包 {abName} 验证通过");
            }
            else
            {
                failCount++;
                LogSystem.Warning($"编辑器预加载验证：AB包 {abName} 未找到对应资源");
            }

            progressCallback?.Invoke(progress);

            // 每处理一个包等待一帧，避免阻塞
            await UniTask.Yield();
        }

        LogSystem.Info($"编辑器预加载验证完成：成功 {successCount} 个，失败 {failCount} 个");
        progressCallback?.Invoke(1f);

        return failCount == 0;
    }

    /// <summary>
    /// 验证AB包对应的资源是否存在（编辑器环境）
    /// </summary>
    private bool ValidateABPackageResources(string abName)
    {
#if UNITY_EDITOR
        // 在资源配置中查找该AB包的所有资源
        bool found = false;
        foreach (var kvp in resourceInfoMap)
        {
            if (kvp.Value.ABPackageName == abName)
            {
                found = true;
                // 验证资源文件是否存在
                if (!System.IO.File.Exists(kvp.Value.ResourcePath))
                {
                    if (!kvp.Value.ResourcePath.StartsWith("Assets/"))
                    {
                        string fullPath = System.IO.Path.Combine(Application.dataPath, kvp.Value.ResourcePath);
                        if (!System.IO.File.Exists(fullPath))
                        {
                            LogSystem.Warning($"资源文件不存在: {kvp.Value.ResourcePath}");
                        }
                    }
                }
            }
        }

        // 如果没有在配置中找到，尝试从AssetDatabase查找
        if (!found)
        {
            // 尝试查找该AB包名称对应的资源文件夹
            string[] guids = UnityEditor.AssetDatabase.FindAssets("", new[] { "Assets" });
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath(path);
                if (importer != null && importer.assetBundleName == abName)
                {
                    found = true;
                    break;
                }
            }
        }

        return found;
#else
        // 非编辑器模式下直接返回true
        return true;
#endif
    }

    /// <summary>
    /// 预加载指定AB包中的所有资源（编辑器环境）
    /// 在编辑器下会加载所有相关资源到内存中
    /// </summary>
    public async void PreloadAllAssetsInPackage(string abName, Action<float> progressCallback = null, Action<UnityEngine.Object[]> completeCallback = null)
    {
        UnityEngine.Object[] assets = await PreloadAllAssetsInPackageAsync(abName, progressCallback);
        completeCallback?.Invoke(assets);
    }

    /// <summary>
    /// 预加载AB包中所有资源的异步实现（编辑器环境）
    /// </summary>
    private async UniTask<UnityEngine.Object[]> PreloadAllAssetsInPackageAsync(string abName, Action<float> progressCallback)
    {
#if UNITY_EDITOR
        LogSystem.Info($"编辑器模式：开始预加载AB包 {abName} 中的所有资源");

        // 收集该AB包下的所有资源
        List<UnityEngine.Object> loadedAssets = new List<UnityEngine.Object>();
        List<string> resourcePaths = new List<string>();

        // 从配置中查找
        foreach (var kvp in resourceInfoMap)
        {
            if (kvp.Value.ABPackageName == abName)
            {
                resourcePaths.Add(kvp.Value.ResourcePath);
            }
        }

        // 如果没有在配置中找到，从AssetDatabase查找
        if (resourcePaths.Count == 0)
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("", new[] { "Assets" });
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath(path);
                if (importer != null && importer.assetBundleName == abName)
                {
                    resourcePaths.Add(path);
                }
            }
        }

        int totalCount = resourcePaths.Count;

        for (int i = 0; i < resourcePaths.Count; i++)
        {
            string path = resourcePaths[i];
            UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (asset != null)
            {
                loadedAssets.Add(asset);
            }

            float progress = (float)(i + 1) / totalCount;
            progressCallback?.Invoke(progress);

            // 每加载一个资源等待一帧
            if (i % 5 == 0) // 每5个资源等待一帧，避免过于频繁
            {
                await UniTask.Yield();
            }
        }

        progressCallback?.Invoke(1f);
        LogSystem.Info($"编辑器模式：AB包 {abName} 预加载完成，共加载 {loadedAssets.Count} 个资源");

        return loadedAssets.ToArray();
#else
        progressCallback?.Invoke(1f);
        return null;
#endif
    }

    /// <summary>
    /// 取消预加载（编辑器环境下无实际操作，仅记录日志）
    /// </summary>
    public void CancelPreload(List<string> abNames)
    {
        if (abNames == null || abNames.Count == 0)
            return;

        LogSystem.Info($"编辑器模式：取消预加载 {abNames.Count} 个AB包");
        // 编辑器模式下资源由Unity自动管理，无需手动释放
    }

    #endregion
}

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;

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
    public T LoadResource<T>(string abName, string resName) where T : Object
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
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="abName">AB包名称</param>
    /// <param name="resName">资源名称</param>
    /// <param name="callback">加载完成回调</param>
    /// <param name="isSync">是否同步加载</param>
    public void LoadResourceAsync<T>(string abName, string resName, UnityAction<T> callback, bool isSync = false) where T : Object
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
}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ABResourceEditorWindow : EditorWindow
{
    private static ABResourceEditorWindow window;
    private List<ABResourceInfo> resourceInfos = new List<ABResourceInfo>();
    private Vector2 scrollPosition;
    private string searchFilter = "";
    private string abPackageFilter = "";
    private bool showOnlyUnassigned = false;
    private string configFilePath;
    
    [MenuItem("唐老师工具/AB包资源管理器")]
    public static void ShowWindow()
    {
        window = GetWindow<ABResourceEditorWindow>("AB包资源管理器");
        window.minSize = new Vector2(800, 600);
    }
    
    private void OnEnable()
    {
        // 从PathConfig中读取配置文件路径
        PathConfig config = PathConfig.Load();
        configFilePath = config.abResourceConfigPath;
        RefreshResourceInfos();
    }
    
    private void OnGUI()
    {
        // 工具栏
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Button("刷新资源", EditorStyles.toolbarButton))
            {
                RefreshResourceInfos();
            }
            
            if (GUILayout.Button("保存配置", EditorStyles.toolbarButton))
            {
                SaveResourceConfig();
            }
            
            GUILayout.FlexibleSpace();
            
            GUILayout.Label("配置文件路径:", EditorStyles.toolbarButton);
            configFilePath = EditorGUILayout.TextField(configFilePath, EditorStyles.toolbarTextField, GUILayout.Width(300));
            if (GUILayout.Button("浏览", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                string newPath = EditorUtility.SaveFilePanel("选择配置文件路径", "Assets", "ABResourceConfig", "json");
                if (!string.IsNullOrEmpty(newPath))
                {
                    // 将绝对路径转换为相对于项目根目录的路径
                    if (newPath.StartsWith(Application.dataPath))
                    {
                        configFilePath = "Assets" + newPath.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        configFilePath = newPath;
                    }
                    
                    // 保存路径到PathConfig
                    SaveConfigFilePath();
                }
            }
            
            if (GUILayout.Button("保存路径", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                SaveConfigFilePath();
            }
            
            GUILayout.Space(10);
            
            searchFilter = EditorGUILayout.TextField("搜索资源", searchFilter, EditorStyles.toolbarTextField, GUILayout.Width(200));
            abPackageFilter = EditorGUILayout.TextField("搜索AB包", abPackageFilter, EditorStyles.toolbarTextField, GUILayout.Width(150));
            showOnlyUnassigned = EditorGUILayout.ToggleLeft("只显示未分配", showOnlyUnassigned, EditorStyles.toolbarButton);
        }
        GUILayout.EndHorizontal();
        
        // 资源列表
        GUILayout.BeginVertical();
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            {
                // 表头
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                {
                    GUILayout.Label("资源名称", GUILayout.Width(200));
                    GUILayout.Label("资源路径", GUILayout.Width(400));
                    GUILayout.Label("AB包名称", GUILayout.Width(150));
                    GUILayout.Label("操作", GUILayout.Width(100));
                }
                GUILayout.EndHorizontal();
                
                // 过滤资源
                var filteredResources = resourceInfos;
                if (!string.IsNullOrEmpty(searchFilter))
                {
                    filteredResources = filteredResources.Where(r => r.ResourceName.Contains(searchFilter, System.StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (!string.IsNullOrEmpty(abPackageFilter))
                {
                    filteredResources = filteredResources.Where(r => r.ABPackageName.Contains(abPackageFilter, System.StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (showOnlyUnassigned)
                {
                    filteredResources = filteredResources.Where(r => string.IsNullOrEmpty(r.ABPackageName)).ToList();
                }
                
                // 显示资源列表
                foreach (var info in filteredResources)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(info.ResourceName, GUILayout.Width(200));
                        GUILayout.Label(info.ResourcePath, GUILayout.Width(400));
                        info.ABPackageName = EditorGUILayout.TextField(info.ABPackageName, GUILayout.Width(150));
                        if (GUILayout.Button("加载", GUILayout.Width(60)))
                        {
                            LoadResource(info);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
    }
    
    private void RefreshResourceInfos()
    {
        resourceInfos.Clear();
        
        // 搜索所有可能的资源文件
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        
        foreach (string path in assetPaths)
        {
            // 过滤掉不需要的文件类型和文件夹
            if (path.StartsWith("Assets/") && !path.EndsWith(".cs") && !path.EndsWith(".meta") && !path.EndsWith(".unity"))
            {
                // 检查是否是文件夹
                if (AssetDatabase.IsValidFolder(path))
                    continue;
                
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset != null)
                {
                    string abPackageName = GetABPackageForResource(path);
                    // 只添加有AB包信息的资源
                    if (!string.IsNullOrEmpty(abPackageName))
                    {
                        ABResourceInfo info = new ABResourceInfo
                        {
                            ResourceName = asset.name,
                            ResourcePath = path,
                            ABPackageName = abPackageName
                        };
                        resourceInfos.Add(info);
                    }
                }
            }
        }
        
        // 按AB包名称排序
        resourceInfos.Sort((a, b) => a.ABPackageName.CompareTo(b.ABPackageName));
    }
    
    private string GetABPackageForResource(string resourcePath)
    {
        // 从Unity的AssetBundle配置中读取AB包信息
        AssetImporter importer = AssetImporter.GetAtPath(resourcePath);
        if (importer != null && !string.IsNullOrEmpty(importer.assetBundleName))
        {
            return importer.assetBundleName;
        }
        return "";
    }
    
    private void SaveResourceConfig()
    {
        // 过滤出有AB包信息的资源
        var resourcesWithAB = resourceInfos.Where(r => !string.IsNullOrEmpty(r.ABPackageName)).ToList();
        
        // 保存资源配置到文件
        string configPath = configFilePath;
        // 确保路径是绝对路径
        if (!Path.IsPathRooted(configPath))
        {
            configPath = Path.Combine(Application.dataPath, configPath.Substring(7)); // 移除 "Assets/" 前缀
        }
        
        string configJson = JsonUtility.ToJson(new ABResourceConfig { resources = resourcesWithAB }, true);
        File.WriteAllText(configPath, configJson);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("保存成功", $"AB包资源配置已保存，共保存 {resourcesWithAB.Count} 个资源", "确定");
    }
    
    private void LoadResource(ABResourceInfo info)
    {
        // 检查是否是文件夹
        if (AssetDatabase.IsValidFolder(info.ResourcePath))
        {
            EditorUtility.DisplayDialog("加载失败", "不能加载文件夹", "确定");
            return;
        }
        
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(info.ResourcePath);
        if (asset != null)
        {
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
            EditorUtility.DisplayDialog("加载成功", $"已加载资源: {info.ResourceName}", "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("加载失败", $"无法加载资源: {info.ResourceName}", "确定");
        }
    }
    
    /// <summary>
    /// 保存配置文件路径到PathConfig
    /// </summary>
    private void SaveConfigFilePath()
    {
        PathConfig config = PathConfig.Load();
        config.abResourceConfigPath = configFilePath;
        config.Save();
        EditorUtility.DisplayDialog("保存成功", "配置文件路径已保存到PathConfig", "确定");
    }
    
    [System.Serializable]
    public class ABResourceInfo
    {
        public string ResourceName;
        public string ResourcePath;
        public string ABPackageName;
    }
    
    [System.Serializable]
    public class ABResourceConfig
    {
        public List<ABResourceInfo> resources;
    }
}

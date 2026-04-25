using UnityEditor;
using UnityEngine;
using System.IO;

public class PathConfigWindow : EditorWindow
{
    private PathConfig config;
    private bool autoSave = true;
    
    [MenuItem("唐老师工具/路径配置管理")]
    public static void ShowWindow()
    {
        GetWindow<PathConfigWindow>("路径配置管理");
    }
    
    private void OnEnable()
    {
        // 加载配置
        config = PathConfig.Load();
    }
    
    private void OnGUI()
    {
        GUILayout.Space(10);
        
        // 标题
        GUILayout.Label("路径配置管理", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // 自动保存选项
        autoSave = EditorGUILayout.Toggle("自动保存", autoSave);
        GUILayout.Space(10);
        
        // Excel相关路径
        GUILayout.Label("Excel相关路径", EditorStyles.boldLabel);
        GUILayout.BeginVertical("box");
        
        EditorGUI.BeginChangeCheck();
        
        // Excel文件夹路径
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Excel文件夹路径:", GUILayout.Width(120));
        config.excelPath = EditorGUILayout.TextField(config.excelPath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择Excel文件夹", config.excelPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                config.excelPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        // 数据类输出路径
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("数据类路径:", GUILayout.Width(120));
        config.dataClassPath = EditorGUILayout.TextField(config.dataClassPath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择数据类文件夹", config.dataClassPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                config.dataClassPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        // 数据容器输出路径
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("容器类路径:", GUILayout.Width(120));
        config.dataContainerPath = EditorGUILayout.TextField(config.dataContainerPath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择容器类文件夹", config.dataContainerPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                config.dataContainerPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        // 二进制文件输出路径
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("二进制路径:", GUILayout.Width(120));
        config.binaryOutputPath = EditorGUILayout.TextField(config.binaryOutputPath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择二进制文件夹", config.binaryOutputPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                config.binaryOutputPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        // JSON输出路径
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("JSON路径:", GUILayout.Width(120));
        config.jsonOutputPath = EditorGUILayout.TextField(config.jsonOutputPath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择JSON文件夹", config.jsonOutputPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                config.jsonOutputPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
        GUILayout.Space(10);
        
        // 编辑器资源路径
        GUILayout.Label("编辑器资源路径", EditorStyles.boldLabel);
        GUILayout.BeginVertical("box");
        
        // 编辑器资源根路径
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("资源根路径:", GUILayout.Width(120));
        config.editorResRootPath = EditorGUILayout.TextField(config.editorResRootPath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择编辑器资源文件夹", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                // 转换为相对路径
                if (path.StartsWith(Application.dataPath))
                {
                    config.editorResRootPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    config.editorResRootPath = path;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
        GUILayout.Space(10);
        
        // 按钮区域
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("保存配置"))
        {
            config.Save();
            AssetDatabase.Refresh();
        }
        
        if (GUILayout.Button("重置默认"))
        {
            if (EditorUtility.DisplayDialog("重置默认", "确定要重置为默认路径吗？", "确定", "取消"))
            {
                config = new PathConfig();
                if (autoSave)
                {
                    config.Save();
                    AssetDatabase.Refresh();
                }
            }
        }
        
        GUILayout.EndHorizontal();
        
        // 自动保存
        if (EditorGUI.EndChangeCheck() && autoSave)
        {
            config.Save();
        }
        
        GUILayout.Space(10);
        
        // 提示信息
        GUILayout.Label("提示:", EditorStyles.boldLabel);
        GUILayout.Label("1. 修改路径后请点击保存配置");
        GUILayout.Label("2. 编辑器资源路径请使用相对路径（如: Assets/Editor/ArtRes/）");
        GUILayout.Label("3. 其他路径可以使用绝对路径");
    }
}
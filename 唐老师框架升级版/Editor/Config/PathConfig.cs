using UnityEngine;
using System.IO;

[System.Serializable]
public class PathConfig
{
    // Excel相关路径
    public string excelPath = Application.dataPath + "/唐老师框架升级版/Excel表数据/";
    public string dataClassPath = Application.dataPath + "/唐老师框架升级版/Runtime/ExcelData工具/DataClass/";
    public string dataContainerPath = Application.dataPath + "/唐老师框架升级版/Runtime/ExcelData工具/Container/";
    public string binaryOutputPath = Application.streamingAssetsPath + "/Binary/";
    public string jsonOutputPath = Application.streamingAssetsPath + "/Json/";
    
    // 编辑器资源路径
    public string editorResRootPath = "Assets/Editor/ArtRes/";
    
    // AB包资源配置文件路径
    public string abResourceConfigPath = "Assets/ABResourceConfig.json";
    
    // 配置文件路径
    private static string configFilePath = Application.dataPath + "/唐老师框架升级版/Editor/Config/PathConfig.json";
    
    // 保存配置到文件
    public void Save()
    {
        string json = JsonUtility.ToJson(this, true);
        string directory = Path.GetDirectoryName(configFilePath);
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllText(configFilePath, json);
        Debug.Log("路径配置已保存: " + configFilePath);
    }
    
    // 从文件加载配置
    public static PathConfig Load()
    {
        if (File.Exists(configFilePath))
        {
            try
            {
                string json = File.ReadAllText(configFilePath);
                PathConfig config = JsonUtility.FromJson<PathConfig>(json);
                return config;
            }
            catch (System.Exception e)
            {
                Debug.LogError("加载路径配置失败: " + e.Message);
                return new PathConfig();
            }
        }
        else
        {
            Debug.Log("路径配置文件不存在，使用默认配置");
            return new PathConfig();
        }
    }
}
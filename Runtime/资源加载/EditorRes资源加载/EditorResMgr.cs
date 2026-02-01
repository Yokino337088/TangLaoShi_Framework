using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using System.IO;


/// <summary>
/// 编辑器资源管理器
/// 注意：只有在开发时能使用该管理器加载资源 用于开发功能
/// 发布后 是无法使用该管理器的 因为它需要用到编辑器相关功能
/// </summary>
public class EditorResMgr : BaseManager<EditorResMgr>
{
    // 配置实例
    private PathConfig config;
    
    // 用于放置需要打包进AB包中的资源路径
    private string rootPath
    {
        get
        {
            if (config == null)
            {
                config = PathConfig.Load();
            }
            return config.editorResRootPath;
        }
    }

    private EditorResMgr() { }

    //1.加载单个资源的
    public T LoadEditorRes<T>(string path) where T:Object
    {
#if UNITY_EDITOR
        // 尝试直接加载路径，不添加后缀名
        string fullPath = rootPath + path;
        T res = AssetDatabase.LoadAssetAtPath<T>(fullPath);
        
        // 如果直接加载失败，尝试查找匹配的资源
        if (res == null)
        {
            // 获取路径的目录部分和资源名称部分
            string directoryPath = Path.GetDirectoryName(fullPath) ?? "";
            string resourceName = Path.GetFileName(fullPath);
            
            // 如果路径不包含目录，使用rootPath作为目录
            if (string.IsNullOrEmpty(directoryPath))
            {
                directoryPath = rootPath;
                resourceName = path;
            }
            
            // 查找指定目录下名称匹配的资源
            string[] guids = AssetDatabase.FindAssets($"{resourceName}", new[] { directoryPath });
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 尝试加载资源并检查类型是否匹配
                T tempRes = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (tempRes != null)
                {
                    res = tempRes;
                    break;
                }
            }
        }
        
        return res;
#else
        return null;
#endif
    }

    //2.加载图集相关资源的
    public Sprite LoadSprite(string path, string spriteName)
    {
#if UNITY_EDITOR
        //加载图集中的所有子资源 
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        //遍历所有子资源 得到同名图片返回
        foreach (var item in sprites)
        {
            if (spriteName == item.name)
                return item as Sprite;
        }
        return null;
#else
        return null;
#endif
    }

    //加载图集文件中的所有子图片并返回给外部
    public Dictionary<string, Sprite> LoadSprites(string path)
    {
#if UNITY_EDITOR
        Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        foreach (var item in sprites)
        {
            spriteDic.Add(item.name, item as Sprite);
        }
        return spriteDic;
#else
        return null;
#endif
    }

}


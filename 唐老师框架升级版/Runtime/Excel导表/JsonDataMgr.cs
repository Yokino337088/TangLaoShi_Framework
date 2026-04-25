// 引入系统命名空间
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using Newtonsoft.Json;
using System.Reflection; // 引入命名空间


/// <summary>
/// 跨平台 JSON 数据管理器
/// 功能：
/// 1. 支持 Android/iOS/PC 多平台
/// 2. 处理 JSON 数据的加载和保存
/// 3. 增强错误处理和日志
/// </summary>
public class JsonDataMgr : BaseManager<JsonDataMgr>
{
    // 重复主键处理方式枚举
    public enum DuplicateKeyBehavior
    {
        Overwrite,   // 覆盖
        Skip,        // 跳过
        ThrowError   // 抛异常
    }

    [Header("数据配置")]
    public DuplicateKeyBehavior duplicateKeyHandling = DuplicateKeyBehavior.Overwrite; // 重复主键处理方式
    public bool allowEmptyKeys = false; // 是否允许空主键

    // 路径定义
    private static readonly string JSON_SUB_PATH = "Json/"; // JSON 数据子目录
    private static readonly string SAVE_PATH = Application.dataPath + "/Save/"; // 存档路径
    private Dictionary<string, object> tableDic = new Dictionary<string, object>(); // 表名到容器对象的字典

    // 构造函数（单例模式，私有化）
    private JsonDataMgr()
    {

    }

    /// <summary> 获取 StreamingAssets JSON 文件路径（自动适配平台） </summary>
    private string GetJsonFilePath(string fileName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 安卓平台路径
        return Path.Combine(Application.streamingAssetsPath, JSON_SUB_PATH, fileName);
#elif UNITY_IOS && !UNITY_EDITOR
        // iOS 平台路径（需加 file:// 前缀）
        return "file://" + Path.Combine(Application.streamingAssetsPath, JSON_SUB_PATH, fileName);
#else
        // 其他平台路径
        return Path.Combine(Application.streamingAssetsPath, JSON_SUB_PATH, fileName);
#endif
    }

    /// <summary>
    /// 初始化方法（根据平台选择同步或异步加载）
    /// 记得在这里加载数据表
    /// </summary>
    public void InitData()
    {
#if UNITY_EDITOR
        // 编辑器下无需异步
        //LoadTable<T_RoleContainer, T_Role>();
#else
        // 移动平台异步加载
        MonoMgr.Instance.StartCoroutine(InitDataAsync());
#endif
    }

    /// <summary>
    /// 异步初始化数据（可扩展）
    /// 记得在这里加载数据表
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitDataAsync()
    {
        yield return null;
    }

    /// <summary>
    /// 主加载接口（根据平台选择同步或异步加载）
    /// </summary>
    /// <typeparam name="T">数据容器类</typeparam>
    /// <typeparam name="K">数据结构类</typeparam>
    public void LoadTable<T, K>()
    {
        LoadTable<T, K>(false);
    }

    /// <summary>
    /// 使用 Newtonsoft.Json 加载表格数据
    /// </summary>
    /// <typeparam name="T">数据容器类</typeparam>
    /// <typeparam name="K">数据结构类</typeparam>
    public void LoadTableWithNewtonsoft<T, K>()
    {
        LoadTable<T, K>(true);
    }

    /// <summary>
    /// 主加载接口（根据平台选择同步或异步加载）
    /// </summary>
    /// <typeparam name="T">数据容器类</typeparam>
    /// <typeparam name="K">数据结构类</typeparam>
    /// <param name="useNewtonsoft">是否使用 Newtonsoft.Json</param>
    private void LoadTable<T, K>(bool useNewtonsoft)
    {
#if UNITY_ANDROID || UNITY_IOS
        // 移动平台异步加载
        MonoMgr.Instance.StartCoroutine(LoadTableAsync<T, K>(useNewtonsoft));
#else
        // PC/编辑器同步加载
        LoadTableDesktop<T, K>(useNewtonsoft);
#endif
    }

    // Android/iOS 专用异步加载表
    private IEnumerator LoadTableAsync<T, K>(bool useNewtonsoft = false)
    {
        // 拼接文件名和路径
        string fileName = typeof(K).Name + ".json";
        string filePath = GetJsonFilePath(fileName);

        // 使用 UnityWebRequest 异步加载
        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            request.timeout = 15; // 超时时间
            yield return request.SendWebRequest(); // 发送请求

            // 检查请求结果
            if (request.result != UnityWebRequest.Result.Success)
            {
                LogSystem.Error($"加载失败: {filePath}\nError: {request.error}");
                yield break;
            }

            // 处理 JSON 数据
            ProcessJsonData<T, K>(request.downloadHandler.text, useNewtonsoft);
        }
    }

    // PC/Editor 专用同步加载表
    private void LoadTableDesktop<T, K>(bool useNewtonsoft = false)
    {
        // 拼接文件名和路径
        string fileName = typeof(K).Name + ".json";
        string filePath = GetJsonFilePath(fileName);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            LogSystem.Error($"文件不存在: {filePath}");
            return;
        }

        try
        {
            // 读取全部文本
            string jsonText = File.ReadAllText(filePath);
            // 处理 JSON 数据
            ProcessJsonData<T, K>(jsonText, useNewtonsoft);
        }
        catch (Exception ex)
        {
            LogSystem.Error($"加载失败: {filePath}\n{ex.Message}");
        }
    }

    // 统一的 JSON 数据处理核心
    /// <summary>
    /// 主键的类型必须为string,并且主键的名字必须为"id"（大小写敏感）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <param name="jsonText"></param>
    /// <param name="useNewtonsoft"></param>
    private void ProcessJsonData<T, K>(string jsonText, bool useNewtonsoft = true)
    {                                                                    
        try
        {
            Dictionary<string, K> dataDic;
            if (useNewtonsoft)
            {
                // 使用 Newtonsoft.Json 反序列化
                dataDic = JsonConvert.DeserializeObject<Dictionary<string, K>>(jsonText);
            }
            else
            {
                // 使用 LitJson 反序列化
                dataDic = JsonMapper.ToObject<Dictionary<string, K>>(jsonText);
            }
            
            Type containerType = typeof(T); // 容器类型
            object containerObj = Activator.CreateInstance(containerType); // 创建容器对象
            object dicObject = containerType.GetField("dataDic").GetValue(containerObj); // 获取字典对象

            MethodInfo containsKey = dicObject.GetType().GetMethod("ContainsKey"); // 字典是否包含键方法
            MethodInfo addMethod = dicObject.GetType().GetMethod("Add"); // 字典添加方法
            PropertyInfo indexer = dicObject.GetType().GetProperty("Item"); // 字典索引器

            int successCount = 0; // 成功计数
            Type classType = typeof(K); // 数据类类型
            FieldInfo keyField = classType.GetField("id"); // 假设主键字段名为 "id"

            // 遍历所有数据
            foreach (K dataObj in dataDic.Values)
            {
                object keyValue = keyField.GetValue(dataObj); // 获取主键值
                if (!ValidateKey(keyValue, classType, successCount + 1)) continue; // 验证主键

                bool isDuplicate = (bool)containsKey.Invoke(dicObject, new[] { keyValue }); // 是否重复
                if (isDuplicate)
                {
                    HandleDuplicateKey(dicObject, keyValue, dataObj, containerType.Name, successCount + 1, indexer); // 处理重复键
                }
                else
                {
                    addMethod.Invoke(dicObject, new[] { keyValue, dataObj }); // 添加到字典
                    successCount++;
                }
            }

            tableDic[typeof(T).Name] = containerObj; // 存入总表字典
            LogSystem.Info($"{typeof(T).Name} 加载完成: {successCount}/{dataDic.Count}");
        }
        catch (Exception ex)
        {
            LogSystem.Error($"处理 JSON 数据失败: {typeof(T).Name}\n{ex}");
        }
    }

    // 使用 Newtonsoft.Json 处理 JSON 数据
    private void ProcessNewtonsoftJsonData<T, K>(string jsonText)
    {
        ProcessJsonData<T, K>(jsonText, true);
    }

    // 验证主键值是否有效
    private bool ValidateKey(object keyValue, Type classType, int rowNum)
    {
        if (keyValue == null || (keyValue is string str && string.IsNullOrEmpty(str)))
        {
            if (!allowEmptyKeys)
            {
                LogSystem.Error($"空键值: {classType.Name} 行{rowNum}");
                return false;
            }
        }
        return true;
    }

    // 处理重复主键（根据配置选择覆盖/跳过/抛异常）
    private void HandleDuplicateKey(object dic, object key, object value, string tableName, int rowNum, PropertyInfo indexer)
    {
        string msg = $"重复键: {tableName} 行{rowNum}\n键值: {key}";
        switch (duplicateKeyHandling)
        {
            case DuplicateKeyBehavior.Overwrite:
                // 覆盖原有数据
                indexer.SetValue(dic, value, new[] { key });
                break;
            case DuplicateKeyBehavior.Skip:
                // 跳过并输出警告
                LogSystem.Warning(msg + " (已跳过)");
                break;
            case DuplicateKeyBehavior.ThrowError:
                // 抛出异常
                throw new Exception(msg);
        }
    }

    // 获取已加载的表格容器对象
    public T GetTable<T>() where T : class
    {
        string tableName = typeof(T).Name;
        if (tableDic.TryGetValue(tableName, out object table))
        {
            return table as T;
        }
        LogSystem.Error($"表不存在: {tableName}");
        return null;
    }

    // 保存对象到本地文件（可读写数据）
    public void Save(object obj, string fileName)
    {
        Save(obj, fileName, false);
    }

    /// <summary>
    /// 使用 Newtonsoft.Json 保存对象到本地文件
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fileName"></param>
    public void SaveWithNewtonsoft(object obj, string fileName)
    {
        Save(obj, fileName, true);
    }

    /// <summary>
    /// 保存对象到本地文件（可读写数据）
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fileName"></param>
    /// <param name="useNewtonsoft"></param>
    private void Save(object obj, string fileName, bool useNewtonsoft)
    {
        // 检查存档目录
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);

        string filePath = Path.Combine(SAVE_PATH, fileName + ".json");
        try
        {
            string jsonText;
            if (useNewtonsoft)
            {
                // 使用 Newtonsoft.Json 序列化
                jsonText = JsonConvert.SerializeObject(obj, Formatting.Indented);
                LogSystem.Info($"使用 Newtonsoft.Json 保存数据到: {filePath}");
            }
            else
            {
                // 使用 LitJson 序列化
                jsonText = JsonMapper.ToJson(obj);
                LogSystem.Info($"使用 LitJson 保存数据到: {filePath}");
            }
            File.WriteAllText(filePath, jsonText);
        }
        catch (Exception ex)
        {
            LogSystem.Error($"保存失败: {filePath}\n{ex.Message}");
        }
    }

    // 加载本地文件为对象（可读写数据）
    public T Load<T>(string fileName) where T : class
    {
        return Load<T>(fileName, false);
    }

    /// <summary>
    /// 使用 Newtonsoft.Json 加载本地文件为对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public T LoadWithNewtonsoft<T>(string fileName) where T : class
    {
        return Load<T>(fileName, true);
    }

    /// <summary>
    /// 加载本地文件为对象（可读写数据）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="useNewtonsoft"></param>
    /// <returns></returns>
    private T Load<T>(string fileName, bool useNewtonsoft) where T : class
    {
        string filePath = Path.Combine(SAVE_PATH, fileName + ".json");

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            LogSystem.Warning($"文件不存在: {filePath}");
            return default;
        }

        try
        {
            string jsonText = File.ReadAllText(filePath);
            if (useNewtonsoft)
            {
                // 使用 Newtonsoft.Json 反序列化
                return JsonConvert.DeserializeObject<T>(jsonText);
            }
            else
            {
                // 使用 LitJson 反序列化
                return JsonMapper.ToObject<T>(jsonText);
            }
        }
        catch (Exception ex)
        {
            LogSystem.Error($"加载失败: {filePath}\n{ex.Message}");
            return default;
        }
    }

    // 验证表格完整性（检查是否有重复主键）
    public void VerifyTableIntegrity<T>() where T : class
    {
        string tableName = typeof(T).Name;
        if (!tableDic.TryGetValue(tableName, out object container))
        {
            LogSystem.Error($"验证失败: 表 {tableName} 未加载");
            return;
        }

        var dicField = container.GetType().GetField("dataDic"); // 获取字典字段
        if (dicField == null)
        {
            LogSystem.Error($"验证失败: {tableName} 缺少 dataDic 字段");
            return;
        }

        if (!(dicField.GetValue(container) is IDictionary dic))
        {
            LogSystem.Error($"验证失败: {tableName}.dataDic 不是字典类型");
            return;
        }

        HashSet<object> keySet = new HashSet<object>(); // 用于检测重复键
        ArrayList duplicateKeys = new ArrayList(); // 存储重复键

        // 遍历所有键
        foreach (var key in dic.Keys)
        {
            if (!keySet.Add(key))
            {
                duplicateKeys.Add(key);
            }
        }
        if (duplicateKeys.Count > 0)
        {
            LogSystem.Error($"表 {tableName} 完整性验证失败 - 发现 {duplicateKeys.Count} 个重复键:");
            foreach (var key in duplicateKeys)
            {
                LogSystem.Error($"重复键: {key} (类型: {key.GetType().Name})");
            }
        }
        else
        {
            LogSystem.Info($"表 {tableName} 完整性验证通过 - 共 {dic.Count} 条记录");
        }
    }

    // 检查指定存档文件是否存在
    public bool HasSavedData(string fileName)
    {
        string filePath = Path.Combine(SAVE_PATH, fileName + ".json");
        return File.Exists(filePath);
    }

    /// <summary>
    /// 从Ab包中加载表格数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public void LoadTableFromAB<T,K>()
    {
        LoadTableFromAB<T, K>(true);
    }

    /// <summary>
    /// 使用 Newtonsoft.Json 从 Ab 包中加载表格数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public void LoadTableFromABWithNewtonsoft<T, K>()
    {
        LoadTableFromAB<T, K>(true);
    }

    /// <summary>
    /// 从Ab包中加载表格数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <param name="useNewtonsoft"></param>
    private void LoadTableFromAB<T,K>(bool useNewtonsoft = true)
    {
        string fileName = typeof(K).Name;
        LogSystem.Info($"从Ab包中加载表格数据: {fileName}");
        ABResMgr.Instance.LoadResAsync<TextAsset>("json", fileName,(obj) =>
        {
            if (obj != null)
            {
                ProcessJsonData<T, K>(obj.text, useNewtonsoft);
            }
            else
            {
                LogSystem.Error("从AB包加载json失败");
            }
        });
    }

    
}

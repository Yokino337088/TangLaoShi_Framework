// 引入系统命名空间
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// 跨平台二进制数据管理器（最终版）
/// 功能：
/// 1. 支持Android/iOS/PC多平台
/// 2. 保留原有所有接口
/// 3. 增强错误处理和日志
/// </summary>
public class BinaryDataMgr : BaseManager<BinaryDataMgr>
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
    private static readonly string BINARY_SUB_PATH = "Binary/"; // 二进制数据子目录
    private static readonly string SAVE_PATH = Application.dataPath + "/Save/"; // 存档路径
    private Dictionary<string, object> tableDic = new Dictionary<string, object>(); // 表名到容器对象的字典



    // 构造函数（单例模式，私有化）
    private BinaryDataMgr()
    {

    }


    /// <summary> 获取StreamingAssets二进制文件路径（自动适配平台） </summary>
    private string GetBinaryFilePath(string fileName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 安卓平台路径
        return Path.Combine(Application.streamingAssetsPath, BINARY_SUB_PATH, fileName);
#elif UNITY_IOS && !UNITY_EDITOR
        // iOS平台路径（需加file://前缀）
        return "file://" + Path.Combine(Application.streamingAssetsPath, BINARY_SUB_PATH, fileName);
#else
        // 其他平台路径
        return Path.Combine(Application.streamingAssetsPath, BINARY_SUB_PATH, fileName);
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
#if UNITY_ANDROID || UNITY_IOS
        // 移动平台异步加载
        MonoMgr.Instance.StartCoroutine(LoadTableAsync<T, K>());
#else
        // PC/编辑器同步加载
        LoadTableDesktop<T, K>();
#endif
    }


    // Android/iOS专用异步加载表
    private IEnumerator LoadTableAsync<T, K>()
    {
        // 拼接文件名和路径
        string fileName = typeof(K).Name + ".tang";
        string filePath = GetBinaryFilePath(fileName);

        // 使用UnityWebRequest异步加载
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

            // 处理二进制数据
            ProcessBinaryData<T, K>(request.downloadHandler.data);
        }
    }


    // PC/Editor专用同步加载表
    private void LoadTableDesktop<T, K>()
    {
        // 拼接文件名和路径
        string fileName = typeof(K).Name + ".tang";
        string filePath = GetBinaryFilePath(fileName);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            LogSystem.Error($"文件不存在: {filePath}");
            return;
        }

        try
        {
            // 读取全部字节
            byte[] bytes = File.ReadAllBytes(filePath);
            // 处理二进制数据
            ProcessBinaryData<T, K>(bytes);
        }
        catch (Exception ex)
        {
            LogSystem.Error($"加载失败: {filePath}\n{ex.Message}");
        }
    }


    // 统一的二进制数据处理核心
    private void ProcessBinaryData<T, K>(byte[] bytes)
    {
        try
        {
            int index = 0; // 当前字节索引
            int count = BitConverter.ToInt32(bytes, index); // 读取数据行数
            index += 4;

            int keyNameLength = BitConverter.ToInt32(bytes, index); // 主键名长度
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength); // 主键名
            index += keyNameLength;

            Type containerType = typeof(T); // 容器类型
            object containerObj = Activator.CreateInstance(containerType); // 创建容器对象
            object dicObject = containerType.GetField("dataDic").GetValue(containerObj); // 获取字典对象

            MethodInfo containsKey = dicObject.GetType().GetMethod("ContainsKey"); // 字典是否包含键方法
            MethodInfo addMethod = dicObject.GetType().GetMethod("Add"); // 字典添加方法
            PropertyInfo indexer = dicObject.GetType().GetProperty("Item"); // 字典索引器

            int successCount = 0; // 成功计数
            Type classType = typeof(K); // 数据类类型
            FieldInfo[] fields = classType.GetFields(); // 字段列表
            FieldInfo keyField = classType.GetField(keyName); // 主键字段

            // 遍历所有数据行
            for (int i = 0; i < count; i++)
            {
                object dataObj = ReadTableRow(bytes, ref index, classType, fields, out bool isValid); // 读取一行数据
                if (!isValid) continue; // 无效则跳过

                object keyValue = keyField.GetValue(dataObj); // 获取主键值
                if (!ValidateKey(keyValue, classType, i + 1)) continue; // 验证主键

                bool isDuplicate = (bool)containsKey.Invoke(dicObject, new[] { keyValue }); // 是否重复
                if (isDuplicate)
                {
                    HandleDuplicateKey(dicObject, keyValue, dataObj, containerType.Name, i + 1, indexer); // 处理重复键
                }
                else
                {
                    addMethod.Invoke(dicObject, new[] { keyValue, dataObj }); // 添加到字典
                    successCount++;
                }
            }

            tableDic[typeof(T).Name] = containerObj; // 存入总表字典
            LogSystem.Info($"{typeof(T).Name} 加载完成: {successCount}/{count}");
        }
        catch (Exception ex)
        {
            LogSystem.Error($"处理二进制数据失败: {typeof(T).Name}\n{ex}");
        }
    }


    // 读取一行数据并填充到对象
    private object ReadTableRow(byte[] bytes, ref int index, Type classType, FieldInfo[] fields, out bool isValid)
    {
        object dataObj = Activator.CreateInstance(classType); // 创建数据对象
        isValid = true;

        // 遍历所有字段
        foreach (FieldInfo field in fields)
        {
            try
            {
                // 读取字段值
                if (!ReadFieldValue(bytes, ref index, field, dataObj))
                {
                    isValid = false;
                    break;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error($"读取字段失败: {field.Name}\n{ex}");
                isValid = false;
            }
        }

        return isValid ? dataObj : null;
    }


    // 读取字段值并赋值到对象
    private bool ReadFieldValue(byte[] bytes, ref int index, FieldInfo field, object target)
    {
        if (field.FieldType == typeof(int))
        {
            // 读取int类型
            field.SetValue(target, BitConverter.ToInt32(bytes, index));
            index += 4;
        }
        else if (field.FieldType == typeof(float))
        {
            // 读取float类型
            field.SetValue(target, BitConverter.ToSingle(bytes, index));
            index += 4;
        }
        else if (field.FieldType == typeof(bool))
        {
            // 读取bool类型
            field.SetValue(target, BitConverter.ToBoolean(bytes, index));
            index += 1;
        }
        else if (field.FieldType == typeof(string))
        {
            // 读取string类型（先读长度再读内容）
            int length = BitConverter.ToInt32(bytes, index);
            index += 4;
            string value = Encoding.UTF8.GetString(bytes, index, length);
            field.SetValue(target, value);
            index += length;
        }
        else
        {
            // 不支持的类型
            LogSystem.Error($"不支持的类型: {field.FieldType}");
            return false;
        }
        return true;
    }


    // 验证主键值是否有效
    private bool ValidateKey(object keyValue, Type classType, int rowNum)
    {
        if (keyValue == null)
        {
            LogSystem.Error($"空键值: {classType.Name} 行{rowNum}");
            return false;
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
        // 检查存档目录
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);

        string filePath = Path.Combine(SAVE_PATH, fileName + ".tang");
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter(); // 二进制序列化器
                bf.Serialize(fs, obj); // 序列化对象
                LogSystem.Info($"数据已保存到: {filePath}");
            }
        }
        catch (Exception ex)
        {
            LogSystem.Error($"保存失败: {filePath}\n{ex.Message}");
        }
    }


    // 加载本地文件为对象（可读写数据）
    public T Load<T>(string fileName) where T : class
    {
        string filePath = Path.Combine(SAVE_PATH, fileName + ".tang");

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            LogSystem.Warning($"文件不存在: {filePath}");
            return default;
        }

        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter(); // 二进制序列化器
                return bf.Deserialize(fs) as T; // 反序列化对象
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
        string filePath = Path.Combine(SAVE_PATH, fileName + ".tang");
        return File.Exists(filePath);
    }
}
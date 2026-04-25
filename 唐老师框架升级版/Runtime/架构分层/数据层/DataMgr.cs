using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 数据管理器
/// 管理所有数据类的生命周期
/// </summary>
public class DataMgr : BaseManager<DataMgr>
{
    #region 数据结构

    /// <summary>
    /// 数据注册信息
    /// </summary>
    private class DataRegistry
    {
        public Type Type { get; set; }
        public IData Instance { get; set; }
        public bool AutoInitialize { get; set; }
        public bool AutoLoad { get; set; }
        public bool AutoSave { get; set; }
    }

    #endregion

    #region 字段

    // 数据注册表
    private Dictionary<string, DataRegistry> _dataRegistry = new Dictionary<string, DataRegistry>();
    
    // 数据实例字典
    private Dictionary<string, IData> _dataInstances = new Dictionary<string, IData>();
    
    // 数据类型映射
    private Dictionary<Type, string> _typeToIdMap = new Dictionary<Type, string>();
    
    // 数据加载顺序
    private List<string> _loadOrder = new List<string>();
    
    // 自动保存间隔
    private float _autoSaveInterval = 30f;
    private float _autoSaveTimer = 0f;

    #endregion

    #region 初始化

    private DataMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
        LogSystem.Info("DataMgr initialized");
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 注册数据类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="id">数据ID</param>
    /// <param name="autoInitialize">是否自动初始化</param>
    /// <param name="autoLoad">是否自动加载</param>
    /// <param name="autoSave">是否自动保存</param>
    public void RegisterData<T>(string id, bool autoInitialize = true, bool autoLoad = true, bool autoSave = true) where T : IData, new()
    {
        if (_dataRegistry.ContainsKey(id))
        {
            LogSystem.Warning($"Data {id} already registered");
            return;
        }

        DataRegistry registry = new DataRegistry
        {
            Type = typeof(T),
            Instance = null,
            AutoInitialize = autoInitialize,
            AutoLoad = autoLoad,
            AutoSave = autoSave
        };

        _dataRegistry[id] = registry;
        _typeToIdMap[typeof(T)] = id;
        _loadOrder.Add(id);

        LogSystem.Info($"Registered data: {id} ({typeof(T).Name})");
    }

    /// <summary>
    /// 注册数据实例
    /// </summary>
    /// <param name="data">数据实例</param>
    /// <param name="autoInitialize">是否自动初始化</param>
    /// <param name="autoLoad">是否自动加载</param>
    /// <param name="autoSave">是否自动保存</param>
    public void RegisterDataInstance(IData data, bool autoInitialize = true, bool autoLoad = true, bool autoSave = true)
    {
        if (_dataRegistry.ContainsKey(data.Id))
        {
            LogSystem.Warning($"Data {data.Id} already registered");
            return;
        }

        DataRegistry registry = new DataRegistry
        {
            Type = data.GetType(),
            Instance = data,
            AutoInitialize = autoInitialize,
            AutoLoad = autoLoad,
            AutoSave = autoSave
        };

        _dataRegistry[data.Id] = registry;
        _dataInstances[data.Id] = data;
        _typeToIdMap[data.GetType()] = data.Id;
        _loadOrder.Add(data.Id);

        LogSystem.Info($"Registered data instance: {data.Id} ({data.GetType().Name})");
    }

    /// <summary>
    /// 获取数据实例
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns>数据实例</returns>
    public T GetData<T>() where T : IData
    {
        Type type = typeof(T);
        if (_typeToIdMap.TryGetValue(type, out string id))
        {
            return (T)GetData(id);
        }
        throw new Exception($"Data type {type.Name} not registered");
    }

    /// <summary>
    /// 获取数据实例
    /// </summary>
    /// <param name="id">数据ID</param>
    /// <returns>数据实例</returns>
    public IData GetData(string id)
    {
        if (!_dataInstances.TryGetValue(id, out IData data))
        {
            data = CreateDataInstance(id);
        }
        return data;
    }

    /// <summary>
    /// 初始化所有数据
    /// </summary>
    public void InitializeAllData()
    {
        foreach (var registry in _dataRegistry.Values)
        {
            if (registry.AutoInitialize)
            {
                IData data = GetData(registry.Type);
                data.Initialize();
            }
        }
        LogSystem.Info("Initialized all data");
    }

    /// <summary>
    /// 加载所有数据
    /// </summary>
    public void LoadAllData()
    {
        foreach (string id in _loadOrder)
        {
            if (_dataRegistry.TryGetValue(id, out DataRegistry registry) && registry.AutoLoad)
            {
                IData data = GetData(id);
                data.Load();
            }
        }
        LogSystem.Info("Loaded all data");
    }

    /// <summary>
    /// 保存所有数据
    /// </summary>
    public void SaveAllData()
    {
        foreach (var registry in _dataRegistry.Values)
        {
            if (registry.AutoSave)
            {
                IData data = GetData(registry.Type);
                data.Save();
            }
        }
        LogSystem.Info("Saved all data");
    }

    /// <summary>
    /// 重置所有数据
    /// </summary>
    public void ResetAllData()
    {
        foreach (var data in _dataInstances.Values)
        {
            data.Reset();
        }
        LogSystem.Info("Reset all data");
    }

    /// <summary>
    /// 清理所有数据
    /// </summary>
    public void ClearAllData()
    {
        _dataInstances.Clear();
        LogSystem.Info("Cleared all data instances");
    }

    /// <summary>
    /// 设置自动保存间隔
    /// </summary>
    /// <param name="interval">间隔时间（秒）</param>
    public void SetAutoSaveInterval(float interval)
    {
        _autoSaveInterval = Mathf.Max(1f, interval);
        LogSystem.Info($"Set auto save interval to {_autoSaveInterval} seconds");
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 创建数据实例
    /// </summary>
    /// <param name="id">数据ID</param>
    /// <returns>数据实例</returns>
    private IData CreateDataInstance(string id)
    {
        if (!_dataRegistry.TryGetValue(id, out DataRegistry registry))
        {
            throw new Exception($"Data {id} not registered");
        }

        IData data;
        if (registry.Instance != null)
        {
            data = registry.Instance;
        }
        else
        {
            data = Activator.CreateInstance(registry.Type) as IData;
            if (data == null)
            {
                throw new Exception($"Failed to create data instance for {registry.Type.Name}");
            }
        }

        _dataInstances[id] = data;
        return data;
    }

    /// <summary>
    /// 获取数据实例
    /// </summary>
    /// <param name="type">数据类型</param>
    /// <returns>数据实例</returns>
    private IData GetData(Type type)
    {
        if (_typeToIdMap.TryGetValue(type, out string id))
        {
            return GetData(id);
        }
        throw new Exception($"Data type {type.Name} not registered");
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
        // 自动保存
        _autoSaveTimer += Time.deltaTime;
        if (_autoSaveTimer >= _autoSaveInterval)
        {
            SaveAllData();
            _autoSaveTimer = 0f;
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 检查数据是否注册
    /// </summary>
    /// <param name="id">数据ID</param>
    /// <returns>是否注册</returns>
    public bool IsDataRegistered(string id)
    {
        return _dataRegistry.ContainsKey(id);
    }

    /// <summary>
    /// 检查数据是否注册
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns>是否注册</returns>
    public bool IsDataRegistered<T>() where T : IData
    {
        return _typeToIdMap.ContainsKey(typeof(T));
    }

    /// <summary>
    /// 获取所有数据ID
    /// </summary>
    /// <returns>数据ID列表</returns>
    public List<string> GetAllDataIds()
    {
        return new List<string>(_dataRegistry.Keys);
    }

    /// <summary>
    /// 获取所有数据实例
    /// </summary>
    /// <returns>数据实例列表</returns>
    public List<IData> GetAllDataInstances()
    {
        return new List<IData>(_dataInstances.Values);
    }

    #endregion
}

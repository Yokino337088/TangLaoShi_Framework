using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TangFramework
{
    /// <summary>
    /// 内存管理器，用于临时存储数据类并在使用完毕后释放内存
    /// </summary>
    public class MemoryManager : BaseManager<MemoryManager>
    {
        /// <summary>
        /// 数据存储字典，键为数据ID，值为数据项
        /// </summary>
        private Dictionary<string, MemoryItem> _dataDict = new Dictionary<string, MemoryItem>();
        
        /// <summary>
        /// 引用计数字典，键为数据ID，值为引用计数
        /// </summary>
        private Dictionary<string, int> _refCountDict = new Dictionary<string, int>();
        
        /// <summary>
        /// 线程安全锁
        /// </summary>
        private object _lock = new object();
        
        /// <summary>
        /// 清理间隔（秒）
        /// </summary>
        private const float CLEANUP_INTERVAL = 60f;
        
        /// <summary>
        /// 最大存储时间（秒）
        /// </summary>
        private const float MAX_STORAGE_TIME = 300f;
        
        /// <summary>
        /// 是否启用自动清理
        /// </summary>
        public bool AutoCleanupEnabled { get; set; } = true;
        
        /// <summary>
        /// 取消令牌源
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="enableAutoCleanup">是否启用自动清理</param>
        public void Initialize(bool enableAutoCleanup = true)
        {
            AutoCleanupEnabled = enableAutoCleanup;
            // 启动清理任务
            if (enableAutoCleanup)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                CleanupTask(_cancellationTokenSource.Token).Forget();
            }
        }
        
        /// <summary>
        /// 存储数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="id">数据唯一标识</param>
        /// <param name="data">数据实例</param>
        /// <returns>是否存储成功</returns>
        public bool StoreData<T>(string id, T data)
        {
            lock (_lock)
            {
                if (_dataDict.ContainsKey(id))
                {
                    // 更新现有数据
                    _dataDict[id] = new MemoryItem(data, DateTime.Now);
                    _refCountDict[id] = 0;
                    return true;
                }
                else
                {
                    // 存储新数据
                    _dataDict.Add(id, new MemoryItem(data, DateTime.Now));
                    _refCountDict.Add(id, 0);
                    return true;
                }
            }
        }
        
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="id">数据唯一标识</param>
        /// <returns>数据实例，如果不存在返回默认值</returns>
        public T GetData<T>(string id)
        {
            lock (_lock)
            {
                if (_dataDict.TryGetValue(id, out var item))
                {
                    // 增加引用计数
                    _refCountDict[id]++;
                    // 更新最后访问时间
                    item.LastAccessTime = DateTime.Now;
                    return (T)item.Data;
                }
                return default;
            }
        }
        
        /// <summary>
        /// 释放数据引用
        /// </summary>
        /// <param name="id">数据唯一标识</param>
        /// <returns>是否释放成功</returns>
        public bool ReleaseData(string id)
        {
            lock (_lock)
            {
                if (_refCountDict.TryGetValue(id, out var count))
                {
                    if (count > 0)
                    {
                        _refCountDict[id]--;
                        
                        // 如果引用计数为0，检查是否需要清理
                        if (_refCountDict[id] == 0)
                        {
                            CheckAndCleanupData(id);
                        }
                        return true;
                    }
                }
                return false;
            }
        }
        
        /// <summary>
        /// 强制清理数据
        /// </summary>
        /// <param name="id">数据唯一标识</param>
        /// <returns>是否清理成功</returns>
        public bool ForceCleanupData(string id)
        {
            lock (_lock)
            {
                if (_dataDict.ContainsKey(id))
                {
                    _dataDict.Remove(id);
                    _refCountDict.Remove(id);
                    return true;
                }
                return false;
            }
        }
        
        /// <summary>
        /// 清理所有数据
        /// </summary>
        public void CleanupAll()
        {
            lock (_lock)
            {
                _dataDict.Clear();
                _refCountDict.Clear();
            }
            
            // 取消清理任务
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
        
        /// <summary>
        /// 检查并清理数据
        /// </summary>
        /// <param name="id">数据唯一标识</param>
        private void CheckAndCleanupData(string id)
        {
            if (_dataDict.TryGetValue(id, out var item))
            {
                // 检查是否超过最大存储时间
                if ((DateTime.Now - item.LastAccessTime).TotalSeconds > MAX_STORAGE_TIME)
                {
                    _dataDict.Remove(id);
                    _refCountDict.Remove(id);
                }
            }
        }
        
        /// <summary>
        /// 清理任务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        private async UniTaskVoid CleanupTask(CancellationToken cancellationToken)
        {
            try
            {
                while (AutoCleanupEnabled && !cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(CLEANUP_INTERVAL), cancellationToken: cancellationToken);
                    CleanupExpiredData();
                }
            }
            catch (OperationCanceledException)
            {
                // 任务被取消，正常退出
            }
        }
        
        /// <summary>
        /// 清理过期数据
        /// </summary>
        private void CleanupExpiredData()
        {
            lock (_lock)
            {
                List<string> keysToRemove = new List<string>();
                
                foreach (var kvp in _dataDict)
                {
                    string id = kvp.Key;
                    MemoryItem item = kvp.Value;
                    
                    // 检查是否引用计数为0且超过最大存储时间
                    if (_refCountDict.TryGetValue(id, out var count) && count == 0)
                    {
                        if ((DateTime.Now - item.LastAccessTime).TotalSeconds > MAX_STORAGE_TIME)
                        {
                            keysToRemove.Add(id);
                        }
                    }
                }
                
                // 清理过期数据
                foreach (string id in keysToRemove)
                {
                    _dataDict.Remove(id);
                    _refCountDict.Remove(id);
                }
            }
        }
        
        /// <summary>
        /// 获取当前存储的数据数量
        /// </summary>
        public int DataCount
        {
            get
            {
                lock (_lock)
                {
                    return _dataDict.Count;
                }
            }
        }
        
        /// <summary>
        /// 根据类型清理数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        public void CleanupDataByType<T>()
        {
            lock (_lock)
            {
                List<string> keysToRemove = new List<string>();
                
                foreach (var kvp in _dataDict)
                {
                    string id = kvp.Key;
                    MemoryItem item = kvp.Value;
                    
                    // 检查数据类型是否匹配
                    if (item.Data is T)
                    {
                        keysToRemove.Add(id);
                    }
                }
                
                // 清理匹配的数据
                foreach (string id in keysToRemove)
                {
                    _dataDict.Remove(id);
                    _refCountDict.Remove(id);
                }
            }
        }
        
        /// <summary>
        /// 内存数据项
        /// </summary>
        private class MemoryItem
        {
            /// <summary>
            /// 数据实例
            /// </summary>
            public object Data { get; }
            
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; }
            
            /// <summary>
            /// 最后访问时间
            /// </summary>
            public DateTime LastAccessTime { get; set; }
            
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="data">数据实例</param>
            /// <param name="createTime">创建时间</param>
            public MemoryItem(object data, DateTime createTime)
            {
                Data = data;
                CreateTime = createTime;
                LastAccessTime = createTime;
            }
        }
    }
    
    
    
    /// <summary>
    /// 内存管理工具类
    /// </summary>
    public static class MemoryUtil
    {
        /// <summary>
        /// 生成唯一ID
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns>唯一ID</returns>
        public static string GenerateUniqueId(string prefix = "")
        {
            return $"{prefix}{Guid.NewGuid().ToString()}";
        }

        /// <summary>
        /// 存储数据的扩展方法
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据实例</param>
        /// <param name="id">数据唯一标识</param>
        /// <returns>是否存储成功</returns>
        public static bool Store<T>(this T data, string id)
        {
            return MemoryManager.Instance.StoreData(id, data);
        }
        
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="id">数据唯一标识</param>
        /// <returns>数据实例</returns>
        public static T Get<T>(string id)
        {
            return MemoryManager.Instance.GetData<T>(id);
        }
        
        /// <summary>
        /// 释放数据
        /// </summary>
        /// <param name="id">数据唯一标识</param>
        /// <returns>是否释放成功</returns>
        public static bool Release(string id)
        {
            return MemoryManager.Instance.ReleaseData(id);
        }
        
        /// <summary>
        /// 清理指定类型的所有数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        public static void CleanupByType<T>()
        {
            MemoryManager.Instance.CleanupDataByType<T>();
        }
    }
}
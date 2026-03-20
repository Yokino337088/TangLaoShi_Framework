using System;
using UnityEngine;
using TangFramework;

namespace TangFramework.Examples
{
    /// <summary>
    /// 内存管理示例
    /// </summary>
    public class MemoryManagerExample : MonoBehaviour
    {
        private void Start()
        {
            // 初始化内存管理器
            MemoryManager.Instance.Initialize();
            
            // 示例1：存储和获取数据
            StoreAndGetDataExample();
            
            // 示例2：引用计数管理
            RefCountExample();
            
            // 示例3：自动清理
            AutoCleanupExample();
        }
        
        /// <summary>
        /// 存储和获取数据示例
        /// </summary>
        private void StoreAndGetDataExample()
        {
            Debug.Log("=== 存储和获取数据示例 ===");
            
            // 定义一个数据类
            PlayerData playerData = new PlayerData
            {
                Id = 1001,
                Name = "张三",
                Level = 10,
                Score = 1000
            };
            
            // 存储数据
            string playerId = "player_1001";
            bool stored = playerData.Store(playerId);
            Debug.Log($"存储数据: {stored}");
            
            // 获取数据
            PlayerData retrievedData = MemoryUtil.Get<PlayerData>(playerId);
            if (retrievedData != null)
            {
                Debug.Log($"获取数据: ID={retrievedData.Id}, Name={retrievedData.Name}, Level={retrievedData.Level}");
            }
            else
            {
                Debug.Log("获取数据失败");
            }
            
            // 释放数据
            bool released = MemoryUtil.Release(playerId);
            Debug.Log($"释放数据: {released}");
        }
        
        /// <summary>
        /// 引用计数管理示例
        /// </summary>
        private void RefCountExample()
        {
            Debug.Log("\n=== 引用计数管理示例 ===");
            
            // 存储一个服务端响应
            ServerResponse response = new ServerResponse
            {
                Code = 200,
                Message = "Success",
                Data = new { UserId = 1001, Token = "abc123" }
            };
            
            string responseId = "server_response_1";
            response.Store(responseId);
            
            // 多次获取数据（增加引用计数）
            for (int i = 0; i < 3; i++)
            {
                var data = MemoryUtil.Get<ServerResponse>(responseId);
                Debug.Log($"第{i+1}次获取数据: Code={data.Code}, Message={data.Message}");
            }
            
            // 多次释放数据（减少引用计数）
            for (int i = 0; i < 3; i++)
            {
                bool released = MemoryUtil.Release(responseId);
                Debug.Log($"第{i+1}次释放数据: {released}");
            }
            
            // 再次尝试获取数据
            var finalData = MemoryUtil.Get<ServerResponse>(responseId);
            if (finalData != null)
            {
                Debug.Log("数据仍然存在（等待自动清理）");
            }
            else
            {
                Debug.Log("数据已被清理");
            }
        }
        
        /// <summary>
        /// 自动清理示例
        /// </summary>
        private void AutoCleanupExample()
        {
            Debug.Log("\n=== 自动清理示例 ===");
            
            // 存储一个临时数据
            TempData tempData = new TempData
            {
                Id = 1,
                Value = "临时数据",
                Timestamp = DateTime.Now
            };
            
            string tempId = "temp_data_1";
            tempData.Store(tempId);
            
            Debug.Log($"存储临时数据: {tempData.Value}");
            Debug.Log($"当前存储的数据数量: {MemoryManager.Instance.DataCount}");
            
            // 释放数据（引用计数变为0）
            MemoryUtil.Release(tempId);
            Debug.Log("释放临时数据，等待自动清理...");
            
            // 注意：实际的自动清理会在60秒后执行
            Debug.Log("自动清理会在60秒后执行，超过300秒未使用的数据会被清理");
        }
        
        /// <summary>
        /// 玩家数据类
        /// </summary>
        [Serializable]
        private class PlayerData
        {
            public int Id;
            public string Name;
            public int Level;
            public int Score;
        }
        
        /// <summary>
        /// 服务端响应类
        /// </summary>
        [Serializable]
        private class ServerResponse
        {
            public int Code;
            public string Message;
            public object Data;
        }
        
        /// <summary>
        /// 临时数据类
        /// </summary>
        [Serializable]
        private class TempData
        {
            public int Id;
            public string Value;
            public DateTime Timestamp;
        }
    }
}
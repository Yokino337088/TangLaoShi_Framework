using System;
using UnityEngine;

/// <summary>
/// 数据层使用示例
/// </summary>
public class DataExample : MonoBehaviour
{
    private void Start()
    {
        // 注册数据类
        RegisterData();
        
        // 初始化所有数据
        DataMgr.Instance.InitializeAllData();
        
        // 加载所有数据
        DataMgr.Instance.LoadAllData();
        
        // 使用玩家数据
        UsePlayerData();
        
        // 保存所有数据
        DataMgr.Instance.SaveAllData();
    }

    /// <summary>
    /// 注册数据类
    /// </summary>
    private void RegisterData()
    {
        // 注册玩家数据
        DataMgr.Instance.RegisterData<PlayerData>("PlayerData");
        
        // 注册其他数据类...
        // DataMgr.Instance.RegisterData<GameData>("GameData");
        // DataMgr.Instance.RegisterData<SettingsData>("SettingsData");
    }

    /// <summary>
    /// 使用玩家数据
    /// </summary>
    private void UsePlayerData()
    {
        // 获取玩家数据
        PlayerData playerData = DataMgr.Instance.GetData<PlayerData>();
        
        // 输出初始数据
        Debug.Log($"初始数据: Level={playerData.Level}, Gold={playerData.Gold}, Diamond={playerData.Diamond}");
        Debug.Log($"初始装备: {string.Join(", ", playerData.Equipments)}");
        Debug.Log($"初始技能: {string.Join(", ", playerData.Skills)}");
        
        // 修改数据
        playerData.AddExp(150); // 添加经验，应该升级到2级
        playerData.AddGold(500);
        playerData.AddDiamond(50);
        playerData.AddEquipment("中级剑");
        playerData.AddSkill("火球术");
        playerData.AddAchievement("第一次升级");
        
        // 输出修改后的数据
        Debug.Log($"修改后数据: Level={playerData.Level}, Gold={playerData.Gold}, Diamond={playerData.Diamond}");
        Debug.Log($"修改后装备: {string.Join(", ", playerData.Equipments)}");
        Debug.Log($"修改后技能: {string.Join(", ", playerData.Skills)}");
        Debug.Log($"成就: {string.Join(", ", playerData.Achievements)}");
    }

    private void OnApplicationQuit()
    {
        // 保存所有数据
        DataMgr.Instance.SaveAllData();
    }
}

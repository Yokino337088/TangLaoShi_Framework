using System;
using System.Collections.Generic;

/// <summary>
/// 玩家数据
/// </summary>
public class PlayerData : BaseData
{
    // 玩家属性
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Gold { get; set; }
    public int Diamond { get; set; }
    
    // 玩家装备
    public List<string> Equipments { get; set; }
    
    // 玩家技能
    public List<string> Skills { get; set; }
    
    // 玩家成就
    public List<string> Achievements { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public PlayerData() : base("PlayerData", "玩家数据")
    {
        Equipments = new List<string>();
        Skills = new List<string>();
        Achievements = new List<string>();
    }

    /// <summary>
    /// 初始化时调用
    /// </summary>
    protected override void OnInitialize()
    {
        // 初始化默认值
        Level = 1;
        Exp = 0;
        Gold = 1000;
        Diamond = 100;
        
        // 添加初始装备
        Equipments.Add("新手剑");
        Equipments.Add("新手 armor");
        
        // 添加初始技能
        Skills.Add("基础攻击");
        
        LogSystem.Info("PlayerData initialized");
    }

    /// <summary>
    /// 重置时调用
    /// </summary>
    protected override void OnReset()
    {
        Level = 1;
        Exp = 0;
        Gold = 1000;
        Diamond = 100;
        Equipments.Clear();
        Skills.Clear();
        Achievements.Clear();
        
        LogSystem.Info("PlayerData reset");
    }

    /// <summary>
    /// 保存时调用
    /// </summary>
    protected override void OnSave()
    {
        // 这里可以实现数据保存逻辑
        // 例如保存到PlayerPrefs或文件
        LogSystem.Info($"PlayerData saved: Level={Level}, Gold={Gold}");
    }

    /// <summary>
    /// 加载时调用
    /// </summary>
    protected override void OnLoad()
    {
        // 这里可以实现数据加载逻辑
        // 例如从PlayerPrefs或文件加载
        LogSystem.Info("PlayerData loaded");
    }

    #region 公共方法

    /// <summary>
    /// 添加经验
    /// </summary>
    public void AddExp(int amount)
    {
        Exp += amount;
        CheckLevelUp();
    }

    /// <summary>
    /// 添加金币
    /// </summary>
    public void AddGold(int amount)
    {
        Gold += amount;
    }

    /// <summary>
    /// 添加钻石
    /// </summary>
    public void AddDiamond(int amount)
    {
        Diamond += amount;
    }

    /// <summary>
    /// 添加装备
    /// </summary>
    public void AddEquipment(string equipment)
    {
        if (!Equipments.Contains(equipment))
        {
            Equipments.Add(equipment);
        }
    }

    /// <summary>
    /// 添加技能
    /// </summary>
    public void AddSkill(string skill)
    {
        if (!Skills.Contains(skill))
        {
            Skills.Add(skill);
        }
    }

    /// <summary>
    /// 添加成就
    /// </summary>
    public void AddAchievement(string achievement)
    {
        if (!Achievements.Contains(achievement))
        {
            Achievements.Add(achievement);
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 检查升级
    /// </summary>
    private void CheckLevelUp()
    {
        int requiredExp = Level * 100;
        if (Exp >= requiredExp)
        {
            Exp -= requiredExp;
            Level++;
            LogSystem.Info($"Player level up to {Level}");
        }
    }

    #endregion
}
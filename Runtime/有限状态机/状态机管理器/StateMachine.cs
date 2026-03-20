using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态机基类 - 管理不同状态之间的转换和通信
/// 使用泛型支持自定义状态类型和AI对象类型
/// </summary>
/// <typeparam name="TStateType">状态类型枚举</typeparam>
/// <typeparam name="TFSMObj">AI对象接口类型</typeparam>
public abstract class StateMachine<TStateType, TFSMObj> where TFSMObj : class, IFSMObj
{
    // 状态字典，存储状态类型和对应的状态实例
    protected Dictionary<TStateType, BaseState<TStateType, TFSMObj>> stateDic = new Dictionary<TStateType, BaseState<TStateType, TFSMObj>>();

    // 状态字典只读属性
    public IReadOnlyDictionary<TStateType, BaseState<TStateType, TFSMObj>> StateDic => stateDic;

    // 当前状态
    private BaseState<TStateType, TFSMObj> nowState;

    // 当前状态只读属性
    public BaseState<TStateType, TFSMObj> NowState => nowState;

    // 上一次的状态类型
    public TStateType PrevStateType { get; private set; }

    // 当前状态类型
    public TStateType CurrentStateType { get; private set; }

    // AI对象状态机所控制的具体对象
    public TFSMObj AIObj { get; private set; }

    // 状态机是否启用
    public bool IsEnabled { get; set; } = true;

    // 状态转换历史记录
    private Queue<TStateType> stateHistory = new Queue<TStateType>();
    private int maxHistorySize = 10;

    /// <summary>
    /// 初始化状态机
    /// </summary>
    /// <param name="aiObj">AI对象</param>
    public StateMachine(TFSMObj aiObj)
    {
        if (aiObj == null)
        {
            LogSystem.Error("StateMachine初始化失败: AI对象不能为空");
            throw new ArgumentNullException(nameof(aiObj));
        }
        
        this.AIObj = aiObj;
    }

    /// <summary>
    /// 改变状态
    /// </summary>
    /// <param name="stateType">目标状态类型</param>
    public virtual void ChangeState(TStateType stateType)
    {
        try
        {
            if (!IsEnabled)
            {
                LogSystem.Warning("StateMachine未启用，无法改变状态");
                return;
            }

            // 检查状态是否存在
            if (!stateDic.ContainsKey(stateType))
            {
                LogSystem.Error($"StateMachine: 状态 {stateType} 不存在");
                return;
            }

            // 如果当前状态与目标状态相同，则不执行转换
            if (nowState != null && EqualityComparer<TStateType>.Default.Equals(nowState.StateType, stateType))
            {
                LogSystem.Warning($"StateMachine: 当前已经是状态 {stateType}，无需转换");
                return;
            }

            // 退出当前状态
            if (nowState != null)
            {
                PrevStateType = nowState.StateType;
                nowState.QuitState();
                LogSystem.Info($"StateMachine: 退出状态 {PrevStateType}");
            }

            // 进入新状态
            nowState = stateDic[stateType];
            CurrentStateType = nowState.StateType;
            nowState.EnterState();
            LogSystem.Info($"StateMachine: 进入状态 {CurrentStateType}");

            // 记录状态转换历史
            AddToHistory(CurrentStateType);
        }
        catch (Exception e)
        {
            LogSystem.Error($"StateMachine: 状态转换失败 - {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 更新当前状态方法
    /// </summary>
    public void UpdateState()
    {
        try
        {
            if (!IsEnabled || nowState == null)
                return;

            nowState.UpdateState();
        }
        catch (Exception e)
        {
            LogSystem.Error($"StateMachine: 状态更新失败 - {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 物理更新当前状态方法
    /// </summary>
    public void FixedUpdateState()
    {
        try
        {
            if (!IsEnabled || nowState == null)
                return;

            nowState.FixedUpdateState();
        }
        catch (Exception e)
        {
            LogSystem.Error($"StateMachine: 物理状态更新失败 - {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// Late更新当前状态方法
    /// </summary>
    public void LateUpdateState()
    {
        try
        {
            if (!IsEnabled || nowState == null)
                return;

            nowState.LateUpdateState();
        }
        catch (Exception e)
        {
            LogSystem.Error($"StateMachine: Late状态更新失败 - {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <typeparam name="TState">状态类型</typeparam>
    /// <param name="stateType">状态枚举值</param>
    public void AddState<TState>(TStateType stateType) where TState : BaseState<TStateType, TFSMObj>
    {
        try
        {
            if (stateDic.ContainsKey(stateType))
            {
                LogSystem.Warning($"StateMachine: 状态 {stateType} 已存在，无需添加");
                return;
            }

            // 使用反射创建状态实例
            TState stateInstance = Activator.CreateInstance(typeof(TState), new object[] { this }) as TState;
            if (stateInstance == null)
            {
                LogSystem.Error($"StateMachine: 创建状态 {stateType} 失败");
                return;
            }

            stateDic.Add(stateType, stateInstance);
            LogSystem.Info($"StateMachine: 添加状态 {stateType}");
        }
        catch (Exception e)
        {
            LogSystem.Error($"StateMachine: 添加状态失败 - {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 批量添加状态
    /// </summary>
    /// <param name="stateMappings">状态类型和状态实例的映射</param>
    public void AddStates(Dictionary<TStateType, Type> stateMappings)
    {
        foreach (var mapping in stateMappings)
        {
            try
            {
                if (!stateDic.ContainsKey(mapping.Key))
                {
                    var stateInstance = Activator.CreateInstance(mapping.Value, new object[] { this }) as BaseState<TStateType, TFSMObj>;
                    if (stateInstance != null)
                    {
                        stateDic.Add(mapping.Key, stateInstance);
                        LogSystem.Info($"StateMachine: 批量添加状态 {mapping.Key}");
                    }
                }
            }
            catch (Exception e)
            {
                LogSystem.Error($"StateMachine: 批量添加状态失败 - {mapping.Key}: {e.Message}");
            }
        }
    }

    /// <summary>
    /// 移除状态
    /// </summary>
    /// <param name="stateType">要移除的状态类型</param>
    public void RemoveState(TStateType stateType)
    {
        try
        {
            if (!stateDic.ContainsKey(stateType))
            {
                LogSystem.Warning($"StateMachine: 状态 {stateType} 不存在，无法移除");
                return;
            }

            // 如果当前正在使用该状态，则先退出
            if (nowState != null && EqualityComparer<TStateType>.Default.Equals(nowState.StateType, stateType))
            {
                nowState.QuitState();
                nowState = null;
                LogSystem.Info($"StateMachine: 移除当前状态 {stateType}");
            }

            stateDic.Remove(stateType);
            LogSystem.Info($"StateMachine: 移除状态 {stateType}");
        }
        catch (Exception e)
        {
            LogSystem.Error($"StateMachine: 移除状态失败 - {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 清空所有状态
    /// </summary>
    public void ClearStates()
    {
        try
        {
            // 退出当前状态
            if (nowState != null)
            {
                nowState.QuitState();
                nowState = null;
            }

            stateDic.Clear();
            stateHistory.Clear();
            LogSystem.Info("StateMachine: 清空所有状态");
        }
        catch (Exception e)
        {
            LogSystem.Error($"StateMachine: 清空状态失败 - {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 检查状态是否存在
    /// </summary>
    /// <param name="stateType">状态类型</param>
    /// <returns>状态是否存在</returns>
    public bool HasState(TStateType stateType)
    {
        return stateDic.ContainsKey(stateType);
    }

    /// <summary>
    /// 获取指定类型的状态
    /// </summary>
    /// <typeparam name="TState">状态类型</typeparam>
    /// <param name="stateType">状态枚举值</param>
    /// <returns>状态实例</returns>
    public TState GetState<TState>(TStateType stateType) where TState : BaseState<TStateType, TFSMObj>
    {
        if (stateDic.TryGetValue(stateType, out var state))
        {
            return state as TState;
        }
        return null;
    }

    /// <summary>
    /// 重置状态机
    /// </summary>
    /// <param name="initialStateType">初始状态类型</param>
    public void Reset(TStateType initialStateType)
    {
        try
        {
            ClearStates();
            ChangeState(initialStateType);
            LogSystem.Info($"StateMachine: 重置状态机到初始状态 {initialStateType}");
        }
        catch (Exception e)
        {
            LogSystem.Error($"StateMachine: 重置失败 - {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 添加状态转换历史记录
    /// </summary>
    /// <param name="stateType">状态类型</param>
    private void AddToHistory(TStateType stateType)
    {
        stateHistory.Enqueue(stateType);
        if (stateHistory.Count > maxHistorySize)
        {
            stateHistory.Dequeue();
        }
    }

    /// <summary>
    /// 获取状态转换历史记录
    /// </summary>
    /// <returns>状态历史记录</returns>
    public TStateType[] GetStateHistory()
    {
        return stateHistory.ToArray();
    }

    /// <summary>
    /// 设置状态历史记录最大数量
    /// </summary>
    /// <param name="size">最大数量</param>
    public void SetMaxHistorySize(int size)
    {
        if (size > 0)
        {
            maxHistorySize = size;
            // 清理历史记录
            while (stateHistory.Count > maxHistorySize)
            {
                stateHistory.Dequeue();
            }
        }
    }

    /// <summary>
    /// 清理状态机资源
    /// </summary>
    public virtual void Dispose()
    {
        try
        {
            ClearStates();
            AIObj = default;
            IsEnabled = false;
            LogSystem.Info("StateMachine: 清理资源");
        }
        catch (Exception e)
        {
            LogSystem.Error($"StateMachine: 清理资源失败 - {e.Message}\n{e.StackTrace}");
        }
    }
}

/// <summary>
/// 通用状态机实现
/// </summary>
/// <typeparam name="TStateType">状态类型</typeparam>
/// <typeparam name="TAIObj">AI对象类型</typeparam>
public class GenericStateMachine<TStateType, TAIObj> : StateMachine<TStateType, TAIObj> where TAIObj : class, IFSMObj
{
    public GenericStateMachine(TAIObj aiObj) : base(aiObj)
    {}
}

/// <summary>
/// 分层状态机 - 支持分层状态机
/// </summary>
/// <typeparam name="TStateType">状态类型枚举</typeparam>
/// <typeparam name="TFSMObj">AI对象接口类型</typeparam>
public class HierarchicalStateMachine<TStateType, TFSMObj> : StateMachine<TStateType, TFSMObj> where TFSMObj : class, IFSMObj
{
    // 父状态
    public BaseState<TStateType, TFSMObj> ParentState { get; set; }
    
    /// <summary>
    /// 初始化分层状态机
    /// </summary>
    /// <param name="aiObj">AI对象</param>
    /// <param name="parentState">父状态</param>
    public HierarchicalStateMachine(TFSMObj aiObj, BaseState<TStateType, TFSMObj> parentState) : base(aiObj)
    {
        ParentState = parentState;
        if (parentState != null)
        {
            parentState.SetChildStateMachine(this);
        }
    }
    
    /// <summary>
    /// 改变状态
    /// </summary>
    /// <param name="stateType">目标状态类型</param>
    public override void ChangeState(TStateType stateType)
    {
        base.ChangeState(stateType);
        
        // 通知父状态状态变化
        ParentState?.UpdateState();
    }
    
    /// <summary>
    /// 清理状态机资源
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        ParentState = null;
    }
    
    /// <summary>
    /// 获取根状态机
    /// </summary>
    /// <returns>根状态机</returns>
    public StateMachine<TStateType, TFSMObj> GetRootStateMachine()
    {
        var currentMachine = this as StateMachine<TStateType, TFSMObj>;
        while (currentMachine is HierarchicalStateMachine<TStateType, TFSMObj> hierarchicalMachine && hierarchicalMachine.ParentState != null)
        {
            currentMachine = hierarchicalMachine.ParentState.stateMachine;
        }
        return currentMachine;
    }
}

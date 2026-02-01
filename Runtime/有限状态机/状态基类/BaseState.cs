using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态基类 - 所有状态类的基类
/// 使用泛型支持自定义状态类型和AI对象类型
/// </summary>
/// <typeparam name="TStateType">状态类型枚举</typeparam>
/// <typeparam name="TFSMObj">AI对象接口类型</typeparam>
public abstract class BaseState<TStateType, TFSMObj> where TFSMObj : class, IFSMObj
{
    // 状态所属的状态机
    protected StateMachine<TStateType, TFSMObj> stateMachine;

    // 当前状态的类型
    public abstract TStateType StateType { get; }

    // AI对象的便捷访问
    protected TFSMObj AIObj => stateMachine.AIObj;

    /// <summary>
    /// 初始化状态
    /// </summary>
    /// <param name="machine">状态机实例</param>
    public BaseState(StateMachine<TStateType, TFSMObj> machine)
    {
        if (machine == null)
        {
            LogSystem.Error("BaseState初始化失败: 状态机不能为空");
            throw new ArgumentNullException(nameof(machine));
        }
        
        stateMachine = machine;
    }

    /// <summary>
    /// 进入状态时执行的逻辑
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// 退出状态时执行的逻辑
    /// </summary>
    public abstract void QuitState();

    /// <summary>
    /// 状态更新逻辑（每帧调用）
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// 物理更新逻辑（固定时间步长调用）
    /// </summary>
    public virtual void FixedUpdateState()
    {
        // 默认实现为空，子类可根据需要重写
    }

    /// <summary>
    /// Late更新逻辑（所有Update执行完毕后调用）
    /// </summary>
    public virtual void LateUpdateState()
    {
        // 默认实现为空，子类可根据需要重写
    }

    /// <summary>
    /// 切换到其他状态
    /// </summary>
    /// <param name="stateType">目标状态类型</param>
    protected void ChangeState(TStateType stateType)
    {
        stateMachine.ChangeState(stateType);
    }

    /// <summary>FSM
    /// 检查是否可以切换到指定状态
    /// </summary>
    /// <param name="stateType">目标状态类型</param>
    /// <returns>是否可以切换</returns>
    protected bool CanChangeState(TStateType stateType)
    {
        return stateMachine.HasState(stateType) && !EqualityComparer<TStateType>.Default.Equals(StateType, stateType);
    }

    /// <summary>
    /// 获取状态机实例
    /// </summary>
    /// <returns>状态机实例</returns>
    public StateMachine<TStateType, TFSMObj> GetStateMachine()
    {
        return stateMachine;
    }

    /// <summary>
    /// 获取泛型状态机实例
    /// </summary>
    /// <typeparam name="TMachine">状态机类型</typeparam>
    /// <returns>状态机实例</returns>
    public TMachine GetStateMachine<TMachine>() where TMachine : StateMachine<TStateType, TFSMObj>
    {
        return stateMachine as TMachine;
    }

    /// <summary>
    /// 获取AI对象实例
    /// </summary>
    /// <returns>AI对象实例</returns>
    public TFSMObj GetAIObj()
    {
        return AIObj;
    }

    /// <summary>
    /// 获取泛型AI对象实例
    /// </summary>
    /// <typeparam name="T">AI对象类型</typeparam>
    /// <returns>AI对象实例</returns>
    public T GetAIObj<T>() where T : class, TFSMObj
    {
        return AIObj as T;
    }

    /// <summary>
    /// 检查AI对象是否具有指定类型
    /// </summary>
    /// <typeparam name="T">检查的类型</typeparam>
    /// <returns>是否具有指定类型</returns>
    public bool IsAIObjOfType<T>() where T : class, TFSMObj
    {
        return AIObj is T;
    }

    /// <summary>
    /// 获取其他状态实例
    /// </summary>
    /// <typeparam name="TOtherState">其他状态类型</typeparam>
    /// <param name="stateType">状态类型枚举值</param>
    /// <returns>其他状态实例</returns>
    public TOtherState GetOtherState<TOtherState>(TStateType stateType) where TOtherState : BaseState<TStateType, TFSMObj>
    {
        return stateMachine.GetState<TOtherState>(stateType);
    }

    /// <summary>
    /// 状态是否激活
    /// </summary>
    public bool IsActive => stateMachine.NowState == this;

    /// <summary>
    /// 状态机是否启用
    /// </summary>
    protected bool IsStateMachineEnabled => stateMachine.IsEnabled;

    /// <summary>
    /// 清理状态资源
    /// </summary>
    public virtual void Dispose()
    {
        // 默认实现为空，子类可根据需要重写
        // 用于清理状态中使用的资源
    }
}

/// <summary>
/// 空状态实现 - 用于作为默认状态或占位状态
/// </summary>
/// <typeparam name="TStateType">状态类型枚举</typeparam>
/// <typeparam name="TAIObj">AI对象接口类型</typeparam>
public class EmptyState<TStateType, TAIObj> : BaseState<TStateType, TAIObj> where TAIObj : class, IFSMObj
{
    private readonly TStateType stateType;

    /// <summary>
    /// 当前状态类型
    /// </summary>
    public override TStateType StateType => stateType;

    /// <summary>
    /// 初始化空状态
    /// </summary>
    /// <param name="machine">状态机实例</param>
    /// <param name="stateType">状态类型</param>
    public EmptyState(StateMachine<TStateType, TAIObj> machine, TStateType stateType) : base(machine)
    {
        this.stateType = stateType;
    }

    /// <summary>
    /// 进入状态
    /// </summary>
    public override void EnterState()
    {
        LogSystem.Info($"进入空状态: {stateType}");
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void QuitState()
    {
        LogSystem.Info($"退出空状态: {stateType}");
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    public override void UpdateState()
    {
        // 空状态不执行任何逻辑
    }
}

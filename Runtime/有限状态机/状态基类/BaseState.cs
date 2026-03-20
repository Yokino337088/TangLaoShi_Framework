using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态基类 - 所有状态的基类
/// 使用泛型支持自定义状态类型和AI对象类型
/// </summary>
/// <typeparam name="TStateType">状态类型枚举</typeparam>
/// <typeparam name="TFSMObj">AI对象接口类型</typeparam>
public abstract class BaseState<TStateType, TFSMObj> where TFSMObj : class, IFSMObj
{
    // 状态所属的状态机
    public StateMachine<TStateType, TFSMObj> stateMachine;

    // 当前状态的类型
    public abstract TStateType StateType { get; }

    // AI对象的快捷访问
    protected TFSMObj AIObj => stateMachine.AIObj;
    
    // 父状态
    public BaseState<TStateType, TFSMObj> ParentState { get; set; }
    
    // 子状态机
    public StateMachine<TStateType, TFSMObj> ChildStateMachine { get; protected set; }

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
    /// 进入状态时执行的方法
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// 退出状态时执行的方法
    /// </summary>
    public abstract void QuitState();

    /// <summary>
    /// 状态更新方法（每帧调用）
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// 物理更新方法（固定时间间隔调用）
    /// </summary>
    public virtual void FixedUpdateState()
    {
        // 默认实现为空，子类可根据需要重写
        ChildStateMachine?.FixedUpdateState();
    }

    /// <summary>
    /// Late更新方法（在Update执行后调用）
    /// </summary>
    public virtual void LateUpdateState()
    {
        // 默认实现为空，子类可根据需要重写
        ChildStateMachine?.LateUpdateState();
    }

    /// <summary>
    /// 转换到其他状态
    /// </summary>
    /// <param name="stateType">目标状态类型</param>
    protected void ChangeState(TStateType stateType)
    {
        stateMachine.ChangeState(stateType);
    }

    /// <summary>
    /// 检查是否可以转换到指定状态
    /// </summary>
    /// <param name="stateType">目标状态类型</param>
    /// <returns>是否可以转换</returns>
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
    /// 获取指定类型的状态机实例
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
    /// 获取指定类型的AI对象实例
    /// </summary>
    /// <typeparam name="T">AI对象类型</typeparam>
    /// <returns>AI对象实例</returns>
    public T GetAIObj<T>() where T : class, TFSMObj
    {
        return AIObj as T;
    }

    /// <summary>
    /// 检查AI对象是否为指定类型
    /// </summary>
    /// <typeparam name="T">检查类型</typeparam>
    /// <returns>是否为指定类型</returns>
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
    /// 是否有子状态机
    /// </summary>
    public bool HasChildStateMachine => ChildStateMachine != null;
    
    /// <summary>
    /// 设置子状态机
    /// </summary>
    /// <param name="childStateMachine">子状态机</param>
    public void SetChildStateMachine(StateMachine<TStateType, TFSMObj> childStateMachine)
    {
        ChildStateMachine = childStateMachine;
    }
    
    /// <summary>
    /// 获取直接或间接的根状态机
    /// </summary>
    /// <returns>根状态机</returns>
    public StateMachine<TStateType, TFSMObj> GetRootStateMachine()
    {
        var currentMachine = stateMachine;
        while (currentMachine is HierarchicalStateMachine<TStateType, TFSMObj> hierarchicalMachine && hierarchicalMachine.ParentState != null)
        {
            currentMachine = hierarchicalMachine.ParentState.stateMachine;
        }
        return currentMachine;
    }

    /// <summary>
    /// 清理状态资源
    /// </summary>
    public virtual void Dispose()
    {
        // 默认实现为空，子类可根据需要重写
        // 释放状态中使用的资源
        ChildStateMachine?.Dispose();
        ChildStateMachine = null;
    }
}

/// <summary>
/// 分层状态 - 用于分层状态机
/// </summary>
/// <typeparam name="TStateType">状态类型枚举</typeparam>
/// <typeparam name="TFSMObj">AI对象接口类型</typeparam>
public class HierarchicalState<TStateType, TFSMObj> : BaseState<TStateType, TFSMObj> where TFSMObj : class, IFSMObj
{
    private TStateType _defaultChildStateType;
    
    /// <summary>
    /// 默认子状态类型
    /// </summary>
    public TStateType DefaultChildStateType
    {
        get => _defaultChildStateType;
        set => _defaultChildStateType = value;
    }

    public override TStateType StateType => throw new NotImplementedException();

    /// <summary>
    /// 初始化分层状态
    /// </summary>
    /// <param name="machine">状态机实例</param>
    public HierarchicalState(StateMachine<TStateType, TFSMObj> machine) : base(machine)
    {}
    
    /// <summary>
    /// 进入状态时的方法
    /// </summary>
    public override void EnterState()
    {
        // 如果有子状态机且设置了默认子状态，则进入默认子状态
        if (ChildStateMachine != null && !EqualityComparer<TStateType>.Default.Equals(_defaultChildStateType, default))
        {
            ChildStateMachine.ChangeState(_defaultChildStateType);
        }
    }
    
    /// <summary>
    /// 退出状态时的方法
    /// </summary>
    public override void QuitState()
    {
        // 退出子状态机
        ChildStateMachine?.NowState?.QuitState();
    }
    
    /// <summary>
    /// 状态更新方法（每帧调用）
    /// </summary>
    public override void UpdateState()
    {
        // 更新子状态机
        ChildStateMachine?.UpdateState();
    }
}

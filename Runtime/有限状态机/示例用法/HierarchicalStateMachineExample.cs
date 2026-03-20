using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 分层状态机示例
/// </summary>
public class HierarchicalStateMachineExample : MonoBehaviour, IFSMObj
{
    // 状态类型枚举
    public enum StateType
    {
        Idle,
        Moving,
        Attacking,
        // 分层状态
        Combat,
        Combat_Chasing,
        Combat_Attacking,
        Combat_Retreating
    }
    
    // 状态机
    private StateMachine<StateType, IFSMObj> _stateMachine;
    
    private void Start()
    {
        // 创建根状态机
        _stateMachine = new GenericStateMachine<StateType, IFSMObj>(this);
        
        // 添加基础状态
        _stateMachine.AddState<IdleState>(StateType.Idle);
        _stateMachine.AddState<MovingState>(StateType.Moving);
        
        // 添加分层状态
        _stateMachine.AddState<CombatState>(StateType.Combat);
        
        // 初始化状态机
        _stateMachine.ChangeState(StateType.Idle);
    }
    
    private void Update()
    {
        // 更新状态机
        _stateMachine.UpdateState();
    }
    
    private void FixedUpdate()
    {
        // 物理更新状态机
        _stateMachine.FixedUpdateState();
    }
    
    private void LateUpdate()
    {
        // 延迟更新状态机
        _stateMachine.LateUpdateState();
    }
    
    // 基础状态类
    public class IdleState : BaseState<StateType, IFSMObj>
    {
        public override StateType StateType => StateType.Idle;
        
        public IdleState(StateMachine<StateType, IFSMObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("进入Idle状态");
        }
        
        public override void QuitState()
        {
            Debug.Log("退出Idle状态");
        }
        
        public override void UpdateState()
        {
            // 模拟检测敌人
            if (UnityEngine.Random.value > 0.95f)
            {
                Debug.Log("检测到敌人，进入Combat状态");
                ChangeState(StateType.Combat);
            }
            // 模拟移动指令
            else if (UnityEngine.Random.value > 0.9f)
            {
                Debug.Log("收到移动指令，进入Moving状态");
                ChangeState(StateType.Moving);
            }
        }
    }
    
    public class MovingState : BaseState<StateType, IFSMObj>
    {
        public override StateType StateType => StateType.Moving;
        
        public MovingState(StateMachine<StateType, IFSMObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("进入Moving状态");
        }
        
        public override void QuitState()
        {
            Debug.Log("退出Moving状态");
        }
        
        public override void UpdateState()
        {
            // 模拟移动完成
            if (UnityEngine.Random.value > 0.95f)
            {
                Debug.Log("移动完成，进入Idle状态");
                ChangeState(StateType.Idle);
            }
            // 模拟检测敌人
            else if (UnityEngine.Random.value > 0.9f)
            {
                Debug.Log("检测到敌人，进入Combat状态");
                ChangeState(StateType.Combat);
            }
        }
    }
    
    // 分层状态类
    public class CombatState : HierarchicalState<StateType, IFSMObj>
    {
        public override StateType StateType => StateType.Combat;
        
        public CombatState(StateMachine<StateType, IFSMObj> machine) : base(machine)
        {
            // 设置默认子状态
            DefaultChildStateType = StateType.Combat_Chasing;
            
            // 创建子状态机
            var childMachine = new HierarchicalStateMachine<StateType, IFSMObj>(machine.AIObj, this);
            
            // 添加子状态
            childMachine.AddState<CombatChasingState>(StateType.Combat_Chasing);
            childMachine.AddState<CombatAttackingState>(StateType.Combat_Attacking);
            childMachine.AddState<CombatRetreatingState>(StateType.Combat_Retreating);
            
            // 设置子状态机
            SetChildStateMachine(childMachine);
        }
        
        public override void EnterState()
        {
            base.EnterState();
            Debug.Log("进入Combat状态");
        }
        
        public override void QuitState()
        {
            base.QuitState();
            Debug.Log("退出Combat状态");
        }
        
        public override void UpdateState()
        {
            base.UpdateState();
                        

            // 模拟战斗结束
            if (UnityEngine.Random.value > 0.98f)
            {
                Debug.Log("战斗结束，返回Idle状态");
                ChangeState(StateType.Idle);
            }
        }
    }
    
    // 子状态类
    public class CombatChasingState : BaseState<StateType, IFSMObj>
    {
        public override StateType StateType => StateType.Combat_Chasing;
        
        public CombatChasingState(StateMachine<StateType, IFSMObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("进入Combat_Chasing状态");
        }
        
        public override void QuitState()
        {
            Debug.Log("退出Combat_Chasing状态");
        }
        
        public override void UpdateState()
        {
            // 模拟接近敌人
            if (UnityEngine.Random.value > 0.9f)
            {
                Debug.Log("接近敌人，进入Combat_Attacking状态");
                ChangeState(StateType.Combat_Attacking);
            }
        }
    }
    
    public class CombatAttackingState : BaseState<StateType, IFSMObj>
    {
        public override StateType StateType => StateType.Combat_Attacking;
        
        public CombatAttackingState(StateMachine<StateType, IFSMObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("进入Combat_Attacking状态");
        }
        
        public override void QuitState()
        {
            Debug.Log("退出Combat_Attacking状态");
        }
        
        public override void UpdateState()
        {
            // 模拟敌人逃跑
            if (UnityEngine.Random.value > 0.8f)
            {
                Debug.Log("敌人逃跑，进入Combat_Chasing状态");
                ChangeState(StateType.Combat_Chasing);
            }
            // 模拟血量过低
            else if (UnityEngine.Random.value > 0.95f)
            {
                Debug.Log("血量过低，进入Combat_Retreating状态");
                ChangeState(StateType.Combat_Retreating);
            }
        }
    }
    
    public class CombatRetreatingState : BaseState<StateType, IFSMObj>
    {
        public override StateType StateType => StateType.Combat_Retreating;
        
        public CombatRetreatingState(StateMachine<StateType, IFSMObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("进入Combat_Retreating状态");
        }
        
        public override void QuitState()
        {
            Debug.Log("退出Combat_Retreating状态");
        }
        
        public override void UpdateState()
        {
            // 模拟撤退完成
            if (UnityEngine.Random.value > 0.9f)
            {
                Debug.Log("撤退完成，返回Idle状态");
                // 返回到根状态机的Idle状态
                GetRootStateMachine().ChangeState(StateType.Idle);
            }
        }
    }
}

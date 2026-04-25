using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态机使用示例
/// 展示如何使用泛型状态机框架创建和管理状态
/// </summary>
public class StateMachineExample : MonoBehaviour
{
    // 示例状态类型枚举
    public enum ExampleStateType
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Die
    }

    // 示例AI对象接口
    public interface IExampleAIObj : IFSMObj
    {
        void Move(Vector3 direction);
        void Attack();
        void Die();
        bool CanSeePlayer();
        bool IsAlive();
        Vector3 GetPosition();
    }

    // 示例AI对象实现
    public class ExampleAIObject : IExampleAIObj
    {
        public string Name { get; set; }
        public float Health { get; set; } = 100f;
        public Vector3 Position { get; set; } = Vector3.zero;

        public ExampleAIObject(string name)
        {
            Name = name;
        }

        public void Move(Vector3 direction)
        {
            Position += direction * Time.deltaTime * 5f;
            LogSystem.Info($"{Name} 移动到: {Position}");
        }

        public void Attack()
        {
            LogSystem.Info($"{Name} 发动攻击!");
        }

        public void Die()
        {
            Health = 0f;
            LogSystem.Info($"{Name} 死亡!");
        }

        public bool CanSeePlayer()
        {
            // 模拟是否能看到玩家
            return UnityEngine.Random.value > 0.7f;
        }

        public bool IsAlive()
        {
            return Health > 0f;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }
    }

    // 示例状态基类
    public abstract class ExampleState : BaseState<ExampleStateType, IExampleAIObj>
    {
        public ExampleState(StateMachine<ExampleStateType, IExampleAIObj> machine) : base(machine)
        {}
    }

    // 示例空闲状态
    public class IdleState : ExampleState
    {
        public override ExampleStateType StateType => ExampleStateType.Idle;

        private float idleTime = 0f;
        private float maxIdleTime = 3f;

        public IdleState(StateMachine<ExampleStateType, IExampleAIObj> machine) : base(machine)
        {}

        public override void EnterState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 进入空闲状态");
            idleTime = 0f;
        }

        public override void QuitState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 退出空闲状态");
        }

        public override void UpdateState()
        {
            idleTime += Time.deltaTime;

            if (idleTime >= maxIdleTime)
            {
                // 随机切换到巡逻状态
                ChangeState(ExampleStateType.Patrol);
            }
            else if (AIObj.CanSeePlayer())
            {
                // 如果看到玩家，切换到追逐状态
                ChangeState(ExampleStateType.Chase);
            }
            else if (!AIObj.IsAlive())
            {
                // 如果生命值为0，切换到死亡状态
                ChangeState(ExampleStateType.Die);
            }
        }
    }

    // 示例巡逻状态
    public class PatrolState : ExampleState
    {
        public override ExampleStateType StateType => ExampleStateType.Patrol;

        private Vector3 patrolDestination;

        public PatrolState(StateMachine<ExampleStateType, IExampleAIObj> machine) : base(machine)
        {}

        public override void EnterState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 进入巡逻状态");
            // 随机设置巡逻目标点
            patrolDestination = new Vector3(
                UnityEngine.Random.Range(-10f, 10f),
                0f,
                UnityEngine.Random.Range(-10f, 10f)
            );
            LogSystem.Info($"{AIObj.GetType().Name} 巡逻目标: {patrolDestination}");
        }

        public override void QuitState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 退出巡逻状态");
        }

        public override void UpdateState()
        {
            if (AIObj.CanSeePlayer())
            {
                // 如果看到玩家，切换到追逐状态
                ChangeState(ExampleStateType.Chase);
            }
            else if (!AIObj.IsAlive())
            {
                // 如果生命值为0，切换到死亡状态
                ChangeState(ExampleStateType.Die);
            }
            else
            {
                // 向巡逻目标移动
                Vector3 direction = (patrolDestination - AIObj.GetPosition()).normalized;
                AIObj.Move(direction);

                // 如果到达巡逻目标，切换到空闲状态
                if (Vector3.Distance(AIObj.GetPosition(), patrolDestination) < 1f)
                {
                    ChangeState(ExampleStateType.Idle);
                }
            }
        }
    }

    // 示例追逐状态
    public class ChaseState : ExampleState
    {
        public override ExampleStateType StateType => ExampleStateType.Chase;

        public ChaseState(StateMachine<ExampleStateType, IExampleAIObj> machine) : base(machine)
        {}

        public override void EnterState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 进入追逐状态");
        }

        public override void QuitState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 退出追逐状态");
        }

        public override void UpdateState()
        {
            if (!AIObj.CanSeePlayer())
            {
                // 如果看不到玩家，切换到巡逻状态
                ChangeState(ExampleStateType.Patrol);
            }
            else if (!AIObj.IsAlive())
            {
                // 如果生命值为0，切换到死亡状态
                ChangeState(ExampleStateType.Die);
            }
            else
            {
                // 模拟追逐玩家
                Vector3 direction = (Vector3.zero - AIObj.GetPosition()).normalized; // 假设玩家在原点
                AIObj.Move(direction * 1.5f); // 追逐速度更快

                // 如果距离足够近，切换到攻击状态
                if (Vector3.Distance(AIObj.GetPosition(), Vector3.zero) < 2f)
                {
                    ChangeState(ExampleStateType.Attack);
                }
            }
        }
    }

    // 示例攻击状态
    public class AttackState : ExampleState
    {
        public override ExampleStateType StateType => ExampleStateType.Attack;

        private float attackCooldown = 0f;
        private float maxAttackCooldown = 1f;

        public AttackState(StateMachine<ExampleStateType, IExampleAIObj> machine) : base(machine)
        {}

        public override void EnterState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 进入攻击状态");
            attackCooldown = 0f;
        }

        public override void QuitState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 退出攻击状态");
        }

        public override void UpdateState()
        {
            if (!AIObj.CanSeePlayer())
            {
                // 如果看不到玩家，切换到巡逻状态
                ChangeState(ExampleStateType.Patrol);
            }
            else if (!AIObj.IsAlive())
            {
                // 如果生命值为0，切换到死亡状态
                ChangeState(ExampleStateType.Die);
            }
            else
            {
                // 攻击冷却
                attackCooldown += Time.deltaTime;
                if (attackCooldown >= maxAttackCooldown)
                {
                    // 执行攻击
                    AIObj.Attack();
                    attackCooldown = 0f;
                }

                // 如果距离太远，切换到追逐状态
                if (Vector3.Distance(AIObj.GetPosition(), Vector3.zero) > 3f)
                {
                    ChangeState(ExampleStateType.Chase);
                }
            }
        }
    }

    // 示例死亡状态
    public class DieState : ExampleState
    {
        public override ExampleStateType StateType => ExampleStateType.Die;

        private float deathTime = 0f;
        private float maxDeathTime = 2f;

        public DieState(StateMachine<ExampleStateType, IExampleAIObj> machine) : base(machine)
        {}

        public override void EnterState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 进入死亡状态");
            AIObj.Die();
            deathTime = 0f;
        }

        public override void QuitState()
        {
            LogSystem.Info($"{AIObj.GetType().Name} 退出死亡状态");
        }

        public override void UpdateState()
        {
            deathTime += Time.deltaTime;
            if (deathTime >= maxDeathTime)
            {
                // 死亡动画结束，可以执行清理操作
                LogSystem.Info($"{AIObj.GetType().Name} 死亡动画结束");
                // 这里可以执行清理操作，比如销毁游戏对象等
            }
        }
    }

    // 状态机实例
    private StateMachine<ExampleStateType, IExampleAIObj> stateMachine;

    // AI对象实例
    private ExampleAIObject aiObject;

    private void Start()
    {
        // 创建AI对象
        aiObject = new ExampleAIObject("Enemy1");

        // 创建状态机
        stateMachine = new GenericStateMachine<ExampleStateType, IExampleAIObj>(aiObject);

        // 添加状态
        stateMachine.AddState<IdleState>(ExampleStateType.Idle);
        stateMachine.AddState<PatrolState>(ExampleStateType.Patrol);
        stateMachine.AddState<ChaseState>(ExampleStateType.Chase);
        stateMachine.AddState<AttackState>(ExampleStateType.Attack);
        stateMachine.AddState<DieState>(ExampleStateType.Die);

        // 也可以使用批量添加的方式
        // var stateMappings = new Dictionary<ExampleStateType, Type>
        // {
        //     { ExampleStateType.Idle, typeof(IdleState) },
        //     { ExampleStateType.Patrol, typeof(PatrolState) },
        //     { ExampleStateType.Chase, typeof(ChaseState) },
        //     { ExampleStateType.Attack, typeof(AttackState) },
        //     { ExampleStateType.Die, typeof(DieState) }
        // };
        // stateMachine.AddStates(stateMappings);

        // 设置状态历史记录的最大容量
        stateMachine.SetMaxHistorySize(15);

        // 初始状态设置为空闲
        stateMachine.ChangeState(ExampleStateType.Idle);

        LogSystem.Info("状态机初始化完成，开始运行");
    }

    private void Update()
    {
        // 更新状态机
        if (stateMachine != null)
        {
            stateMachine.UpdateState();
        }

        // 每5秒打印一次状态历史
        if (Time.time % 5f < Time.deltaTime)
        {
            PrintStateHistory();
        }
    }

    private void FixedUpdate()
    {
        // 物理更新状态机
        if (stateMachine != null)
        {
            stateMachine.FixedUpdateState();
        }
    }

    private void LateUpdate()
    {
        // Late更新状态机
        if (stateMachine != null)
        {
            stateMachine.LateUpdateState();
        }
    }

    private void OnDestroy()
    {
        // 清理状态机资源
        if (stateMachine != null)
        {
            stateMachine.Dispose();
        }
    }

    /// <summary>
    /// 打印状态历史记录
    /// </summary>
    private void PrintStateHistory()
    {
        if (stateMachine != null)
        {
            var history = stateMachine.GetStateHistory();
            LogSystem.Info("状态历史记录:");
            foreach (var state in history)
            {
                LogSystem.Info($"  - {state}");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using static BossStateMachineExample;


public interface IBossObj : IFSMObj
{
    // BOSS属性
    float Health { get; set; }
    float MaxHealth { get; set; }
    float EnrageThreshold { get; set; }
    bool IsEnraged { get; set; }
    bool IsVulnerable { get; set; }
    
    // 玩家相关
    Transform PlayerTransform { get; set; }
    float DetectionRange { get; set; }
    float AttackRange { get; set; }
    
    // 方法
    void TakeDamage(float damage);
    bool IsPlayerInDetectionRange();
    bool IsPlayerInAttackRange();
    void MoveTowardsPlayer();
}

public class BossStateMachine : StateMachine<BossStateType, IBossObj>
{
    public BossStateMachine(IBossObj aiObj) : base(aiObj)
    {

    }
}

public class BossController : MonoBehaviour, IBossObj
{

    // BOSS属性
    public float Health = 1000f;
    public float MaxHealth = 1000f;
    public float EnrageThreshold = 300f; // 愤怒阈值
    public bool IsEnraged { get; private set; } = false;
    public bool IsVulnerable { get; private set; } = false;

    float IBossObj.Health { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    float IBossObj.MaxHealth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    float IBossObj.EnrageThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    bool IBossObj.IsEnraged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    bool IBossObj.IsVulnerable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    Transform IBossObj.PlayerTransform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    float IBossObj.DetectionRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    float IBossObj.AttackRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    // 玩家相关
    public Transform PlayerTransform;
    public float DetectionRange = 20f;
    public float AttackRange = 5f;

    // 状态机管理器
    private BossStateMachine _stateMachine;

    private void Start()
    {
        // 创建根状态机
        _stateMachine = new BossStateMachine(this);

        //添加状态
        _stateMachine.AddState<IdleState>(BossStateType.Idle);
        _stateMachine.AddState<PatrolState>(BossStateType.Patrol);
        _stateMachine.AddState<VulnerableState>(BossStateType.Vulnerable);
        _stateMachine.AddState<DeathState>(BossStateType.Death);
        _stateMachine.AddState<CombatState>(BossStateType.Combat);
        _stateMachine.AddState<EnragedState>(BossStateType.Enraged);

        // 初始化状态机
        _stateMachine.ChangeState(BossStateType.Idle);
    }

    private void Update()
    {
        // 更新状态机
        _stateMachine.UpdateState();

        // 检查愤怒状态
        CheckEnrageState();

        // 检查死亡
        CheckDeath();
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

    /// <summary>
    /// 检查是否进入愤怒状态
    /// </summary>
    private void CheckEnrageState()
    {
        if (Health <= EnrageThreshold && !IsEnraged)
        {
            IsEnraged = true;
            Debug.Log("BOSS进入愤怒状态！");

            // 从当前状态转换到愤怒状态
            if (_stateMachine.NowState is CombatState)
            {
                _stateMachine.ChangeState(BossStateType.Enraged);
            }
        }
    }

    /// <summary>
    /// 检查是否死亡
    /// </summary>
    private void CheckDeath()
    {
        if (Health <= 0 && _stateMachine.NowState.StateType != BossStateType.Death)
        {
            _stateMachine.ChangeState(BossStateType.Death);
        }
    }

    /// <summary>
    /// 受伤
    /// </summary>
    /// <param name="damage">伤害值</param>
    public void TakeDamage(float damage)
    {
        Health = Mathf.Max(0, Health - damage);
        Debug.Log($"BOSS受到 {damage} 点伤害，当前血量: {Health}/{MaxHealth}");

        // 如果在Idle或Patrol状态，被攻击后进入战斗状态
        if (_stateMachine.NowState.StateType == BossStateType.Idle || _stateMachine.NowState.StateType == BossStateType.Patrol)
        {
            _stateMachine.ChangeState(BossStateType.Combat);
        }
    }

    /// <summary>
    /// 检查玩家是否在检测范围内
    /// </summary>
    /// <returns>是否检测到玩家</returns>
    public bool IsPlayerInDetectionRange()
    {
        if (PlayerTransform == null)
            return false;

        return Vector3.Distance(transform.position, PlayerTransform.position) <= DetectionRange;
    }

    /// <summary>
    /// 检查玩家是否在攻击范围内
    /// </summary>
    /// <returns>是否在攻击范围内</returns>
    public bool IsPlayerInAttackRange()
    {
        if (PlayerTransform == null)
            return false;

        return Vector3.Distance(transform.position, PlayerTransform.position) <= AttackRange;
    }

    /// <summary>
    /// 移动朝向玩家
    /// </summary>
    public void MoveTowardsPlayer()
    {
        if (PlayerTransform == null)
            return;

        Vector3 direction = (PlayerTransform.position - transform.position).normalized;
        transform.position += direction * 3f * Time.deltaTime;
        transform.LookAt(PlayerTransform);
    }
}

/// <summary>
/// BOSS分层状态机示例
/// 展示如何使用分层状态机管理复杂的BOSS状态
/// </summary>
public class BossStateMachineExample : MonoBehaviour, IBossObj
{
    // BOSS状态类型枚举
    public enum BossStateType
    {
        // 基础状态
        Idle,           //  idle状态
        Patrol,         // 巡逻状态
        
        // 战斗相关状态
        Combat,         // 战斗状态（分层状态）
        Combat_Chase,   // 战斗-追逐状态
        Combat_Attack,  // 战斗-攻击状态
        Combat_Evade,   // 战斗-闪避状态
        
        // 特殊状态
        Enraged,        // 愤怒状态（分层状态）
        Enraged_Attack, // 愤怒-攻击状态
        Enraged_Charge, // 愤怒-冲锋状态
        
        // 虚弱状态
        Vulnerable,     // 虚弱状态
        
        // 死亡状态
        Death
    }
    
    // 状态机
    private StateMachine<BossStateType, IBossObj> _stateMachine;
    
    // BOSS属性
    public float Health = 1000f;
    public float MaxHealth = 1000f;
    public float EnrageThreshold = 300f; // 愤怒阈值
    public bool IsEnraged { get; private set; } = false;
    public bool IsVulnerable { get; private set; } = false;
    float IBossObj.Health { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    float IBossObj.MaxHealth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    float IBossObj.EnrageThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    bool IBossObj.IsEnraged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    bool IBossObj.IsVulnerable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    Transform IBossObj.PlayerTransform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    float IBossObj.DetectionRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    float IBossObj.AttackRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    // 玩家相关
    public Transform PlayerTransform;
    public float DetectionRange = 20f;
    public float AttackRange = 5f;
    
    private void Start()
    {
        // 创建根状态机
        _stateMachine = new GenericStateMachine<BossStateType, IBossObj>(this);
        
        // 添加基础状态
        _stateMachine.AddState<IdleState>(BossStateType.Idle);
        _stateMachine.AddState<PatrolState>(BossStateType.Patrol);
        _stateMachine.AddState<VulnerableState>(BossStateType.Vulnerable);
        _stateMachine.AddState<DeathState>(BossStateType.Death);
        
        // 添加分层状态
        _stateMachine.AddState<CombatState>(BossStateType.Combat);
        _stateMachine.AddState<EnragedState>(BossStateType.Enraged);
        
        // 初始化状态机
        _stateMachine.ChangeState(BossStateType.Idle);
    }
    
    private void Update()
    {
        // 更新状态机
        _stateMachine.UpdateState();
        
        // 检查愤怒状态
        CheckEnrageState();
        
        // 检查死亡
        CheckDeath();
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
    
    /// <summary>
    /// 检查是否进入愤怒状态
    /// </summary>
    private void CheckEnrageState()
    {
        if (Health <= EnrageThreshold && !IsEnraged)
        {
            IsEnraged = true;
            Debug.Log("BOSS进入愤怒状态！");
            
            // 从当前状态转换到愤怒状态
            if (_stateMachine.NowState is CombatState)
            {
                _stateMachine.ChangeState(BossStateType.Enraged);
            }
        }
    }
    
    /// <summary>
    /// 检查是否死亡
    /// </summary>
    private void CheckDeath()
    {
        if (Health <= 0 && _stateMachine.NowState.StateType != BossStateType.Death)
        {
            _stateMachine.ChangeState(BossStateType.Death);
        }
    }
    
    /// <summary>
    /// 受伤
    /// </summary>
    /// <param name="damage">伤害值</param>
    public void TakeDamage(float damage)
    {
        Health = Mathf.Max(0, Health - damage);
        Debug.Log($"BOSS受到 {damage} 点伤害，当前血量: {Health}/{MaxHealth}");
        
        // 如果在Idle或Patrol状态，被攻击后进入战斗状态
        if (_stateMachine.NowState.StateType == BossStateType.Idle || _stateMachine.NowState.StateType == BossStateType.Patrol)
        {
            _stateMachine.ChangeState(BossStateType.Combat);
        }
    }
    
    /// <summary>
    /// 检查玩家是否在检测范围内
    /// </summary>
    /// <returns>是否检测到玩家</returns>
    public bool IsPlayerInDetectionRange()
    {
        if (PlayerTransform == null)
            return false;
        
        return Vector3.Distance(transform.position, PlayerTransform.position) <= DetectionRange;
    }
    
    /// <summary>
    /// 检查玩家是否在攻击范围内
    /// </summary>
    /// <returns>是否在攻击范围内</returns>
    public bool IsPlayerInAttackRange()
    {
        if (PlayerTransform == null)
            return false;
        
        return Vector3.Distance(transform.position, PlayerTransform.position) <= AttackRange;
    }
    
    /// <summary>
    /// 移动朝向玩家
    /// </summary>
    public void MoveTowardsPlayer()
    {
        if (PlayerTransform == null)
            return;
        
        Vector3 direction = (PlayerTransform.position - transform.position).normalized;
        transform.position += direction * 3f * Time.deltaTime;
        transform.LookAt(PlayerTransform);
    }
    
    // 基础状态类
    public class IdleState : BaseState<BossStateType, IBossObj>
    {
        public override BossStateType StateType => BossStateType.Idle;
        
        public IdleState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("BOSS进入Idle状态");
        }
        
        public override void QuitState()
        {
            Debug.Log("BOSS退出Idle状态");
        }
        
        public override void UpdateState()
        {
            var boss = (BossStateMachineExample)GetAIObj();
            
            // 检测玩家
            if (boss.IsPlayerInDetectionRange())
            {
                Debug.Log("BOSS检测到玩家，进入Combat状态");
                ChangeState(BossStateType.Combat);
            }
            // 随机切换到巡逻状态
            else if (UnityEngine.Random.value > 0.98f)
            {
                Debug.Log("BOSS开始巡逻");
                ChangeState(BossStateType.Patrol);
            }
        }
    }
    
    public class PatrolState : BaseState<BossStateType, IBossObj>
    {
        private Vector3 patrolDestination;
        private float patrolTime = 0f;
        private float maxPatrolTime = 10f;
        
        public override BossStateType StateType => BossStateType.Patrol;
        
        public PatrolState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("BOSS进入Patrol状态");
            
            // 随机巡逻目标点
            var boss = (BossStateMachineExample)GetAIObj();
            patrolDestination = boss.transform.position + new Vector3(
                UnityEngine.Random.Range(-20f, 20f),
                0,
                UnityEngine.Random.Range(-20f, 20f)
            );
            patrolTime = 0f;
        }
        
        public override void QuitState()
        {
            Debug.Log("BOSS退出Patrol状态");
        }
        
        public override void UpdateState()
        {
            var boss = (BossStateMachineExample)GetAIObj();
            
            // 检测玩家
            if (boss.IsPlayerInDetectionRange())
            {
                Debug.Log("BOSS检测到玩家，进入Combat状态");
                ChangeState(BossStateType.Combat);
                return;
            }
            
            // 移动到巡逻点
            Vector3 direction = (patrolDestination - boss.transform.position).normalized;
            boss.transform.position += direction * 2f * Time.deltaTime;
            boss.transform.LookAt(patrolDestination);
            
            // 到达巡逻点或超时，返回Idle状态
            if (Vector3.Distance(boss.transform.position, patrolDestination) < 1f || patrolTime > maxPatrolTime)
            {
                Debug.Log("BOSS巡逻结束，返回Idle状态");
                ChangeState(BossStateType.Idle);
            }
            
            patrolTime += Time.deltaTime;
        }
    }
    
    // 战斗状态（分层状态）
    public class CombatState : HierarchicalState<BossStateType, IBossObj>
    {
        public override BossStateType StateType => BossStateType.Combat;
        
        public CombatState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {
            // 设置默认子状态
            DefaultChildStateType = BossStateType.Combat_Chase;
            
            // 创建子状态机
            var childMachine = new HierarchicalStateMachine<BossStateType, IBossObj>(machine.AIObj, this);
            
            // 添加子状态
            childMachine.AddState<CombatChaseState>(BossStateType.Combat_Chase);
            childMachine.AddState<CombatAttackState>(BossStateType.Combat_Attack);
            childMachine.AddState<CombatEvadeState>(BossStateType.Combat_Evade);
            
            // 设置子状态机
            SetChildStateMachine(childMachine);
        }
        
        public override void EnterState()
        {
            base.EnterState();
            Debug.Log("BOSS进入Combat状态");
        }
        
        public override void QuitState()
        {
            base.QuitState();
            Debug.Log("BOSS退出Combat状态");
        }
        
        public override void UpdateState()
        {
            base.UpdateState();
            
            var boss = (BossStateMachineExample)GetAIObj();

            ChangeState(BossStateType.Combat_Chase);

            // 检查玩家是否离开检测范围
            if (!boss.IsPlayerInDetectionRange())
            {
                Debug.Log("玩家离开检测范围，BOSS返回Idle状态");
                ChangeState(BossStateType.Idle);
            }
            
            // 检查是否进入虚弱状态
            if (boss.Health < boss.MaxHealth * 0.5f && !boss.IsVulnerable && UnityEngine.Random.value > 0.99f)
            {
                boss.IsVulnerable = true;
                Debug.Log("BOSS进入虚弱状态");
                ChangeState(BossStateType.Vulnerable);
            }
        }
    }
    
    // 战斗子状态
    public class CombatChaseState : BaseState<BossStateType, IBossObj>
    {
        public override BossStateType StateType => BossStateType.Combat_Chase;
        
        public CombatChaseState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("BOSS进入Combat_Chase状态");
        }
        
        public override void QuitState()
        {
            Debug.Log("BOSS退出Combat_Chase状态");
        }
        
        public override void UpdateState()
        {
            var boss = (BossStateMachineExample)GetAIObj();
            
            // 移动朝向玩家
            boss.MoveTowardsPlayer();
            
            // 检查是否进入攻击范围
            if (boss.IsPlayerInAttackRange())
            {
                Debug.Log("BOSS进入攻击范围，切换到攻击状态");
                ChangeState(BossStateType.Combat_Attack);
            }
            
            // 随机闪避
            if (UnityEngine.Random.value > 0.95f)
            {
                Debug.Log("BOSS闪避");
                ChangeState(BossStateType.Combat_Evade);
            }
        }
    }
    
    public class CombatAttackState : BaseState<BossStateType, IBossObj>
    {
        private float attackCooldown = 2f;
        private float currentCooldown = 0f;
        
        public override BossStateType StateType => BossStateType.Combat_Attack;
        
        public CombatAttackState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("BOSS进入Combat_Attack状态");
            currentCooldown = 0f;
            
            // 执行攻击
            PerformAttack();
        }
        
        public override void QuitState()
        {
            Debug.Log("BOSS退出Combat_Attack状态");
        }
        
        public override void UpdateState()
        {
            var boss = (BossStateMachineExample)GetAIObj();
            
            currentCooldown += Time.deltaTime;
            
            // 攻击冷却结束
            if (currentCooldown >= attackCooldown)
            {
                // 检查是否还在攻击范围内
                if (boss.IsPlayerInAttackRange())
                {
                    PerformAttack();
                    currentCooldown = 0f;
                }
                else
                {
                    Debug.Log("玩家离开攻击范围，BOSS开始追逐");
                    ChangeState(BossStateType.Combat_Chase);
                }
            }
        }
        
        /// <summary>
        /// 执行攻击
        /// </summary>
        private void PerformAttack()
        {
            Debug.Log("BOSS发动攻击！");
            // 这里可以添加具体的攻击逻辑
        }
    }
    
    public class CombatEvadeState : BaseState<BossStateType, IBossObj>
    {
        private float evadeDuration = 1f;
        private float currentDuration = 0f;
        private Vector3 evadeDirection;
        
        public override BossStateType StateType => BossStateType.Combat_Evade;
        
        public CombatEvadeState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("BOSS进入Combat_Evade状态");
            currentDuration = 0f;
            
            // 随机闪避方向
            evadeDirection = new Vector3(
                UnityEngine.Random.Range(-1f, 1f),
                0,
                UnityEngine.Random.Range(-1f, 1f)
            ).normalized;
        }
        
        public override void QuitState()
        {
            Debug.Log("BOSS退出Combat_Evade状态");
        }
        
        public override void UpdateState()
        {
            var boss = (BossStateMachineExample)GetAIObj();
            
            // 执行闪避移动
            boss.transform.position += evadeDirection * 5f * Time.deltaTime;
            
            currentDuration += Time.deltaTime;
            
            // 闪避结束，返回追逐状态
            if (currentDuration >= evadeDuration)
            {
                ChangeState(BossStateType.Combat_Chase);
            }
        }
    }
    
    // 愤怒状态（分层状态）
    public class EnragedState : HierarchicalState<BossStateType, IBossObj>
    {
        public override BossStateType StateType => BossStateType.Enraged;
        
        public EnragedState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {
            // 设置默认子状态
            DefaultChildStateType = BossStateType.Enraged_Attack;
            
            // 创建子状态机
            var childMachine = new HierarchicalStateMachine<BossStateType, IBossObj>(machine.AIObj, this);
            
            // 添加子状态
            childMachine.AddState<EnragedAttackState>(BossStateType.Enraged_Attack);
            childMachine.AddState<EnragedChargeState>(BossStateType.Enraged_Charge);
            
            // 设置子状态机
            SetChildStateMachine(childMachine);
        }
        
        public override void EnterState()
        {
            base.EnterState();
            Debug.Log("BOSS进入Enraged状态！");
        }
        
        public override void QuitState()
        {
            base.QuitState();
            Debug.Log("BOSS退出Enraged状态");
        }
        
        public override void UpdateState()
        {
            base.UpdateState();
            
            var boss = (BossStateMachineExample)GetAIObj();
            
            // 检查玩家是否离开检测范围
            if (!boss.IsPlayerInDetectionRange())
            {
                Debug.Log("玩家离开检测范围，BOSS返回Idle状态");
                ChangeState(BossStateType.Idle);
            }
        }
    }
    
    // 愤怒子状态
    public class EnragedAttackState : BaseState<BossStateType, IBossObj>
    {
        private float attackCooldown = 1f; // 愤怒状态下攻击冷却更短
        private float currentCooldown = 0f;
        
        public override BossStateType StateType => BossStateType.Enraged_Attack;
        
        public EnragedAttackState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("BOSS进入Enraged_Attack状态");
            currentCooldown = 0f;
            
            // 执行愤怒攻击
            PerformEnragedAttack();
        }
        
        public override void QuitState()
        {
            Debug.Log("BOSS退出Enraged_Attack状态");
        }
        
        public override void UpdateState()
        {
            var boss = (BossStateMachineExample)GetAIObj();
            
            currentCooldown += Time.deltaTime;
            
            // 攻击冷却结束
            if (currentCooldown >= attackCooldown)
            {
                // 检查是否还在攻击范围内
                if (boss.IsPlayerInAttackRange())
                {
                    PerformEnragedAttack();
                    currentCooldown = 0f;
                }
                else
                {
                    // 愤怒状态下直接冲锋
                    Debug.Log("BOSS准备冲锋！");
                    ChangeState(BossStateType.Enraged_Charge);
                }
            }
        }
        
        /// <summary>
        /// 执行愤怒攻击
        /// </summary>
        private void PerformEnragedAttack()
        {
            Debug.Log("BOSS发动愤怒攻击！");
            // 这里可以添加更强大的攻击逻辑
        }
    }
    
    public class EnragedChargeState : BaseState<BossStateType, IBossObj>
    {
        private float chargeDuration = 2f;
        private float currentDuration = 0f;
        private Vector3 chargeDirection;
        
        public override BossStateType StateType => BossStateType.Enraged_Charge;
        
        public EnragedChargeState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("BOSS进入Enraged_Charge状态");
            currentDuration = 0f;
            
            var boss = (BossStateMachineExample)GetAIObj();
            // 朝向玩家冲锋
            if (boss.PlayerTransform != null)
            {
                chargeDirection = (boss.PlayerTransform.position - boss.transform.position).normalized;
            }
            else
            {
                chargeDirection = boss.transform.forward;
            }
        }
        
        public override void QuitState()
        {
            Debug.Log("BOSS退出Enraged_Charge状态");
        }
        
        public override void UpdateState()
        {
            var boss = (BossStateMachineExample)GetAIObj();
            
            // 执行冲锋
            boss.transform.position += chargeDirection * 8f * Time.deltaTime;
            boss.transform.forward = chargeDirection;
            
            currentDuration += Time.deltaTime;
            
            // 冲锋结束
            if (currentDuration >= chargeDuration)
            {
                // 检查是否击中玩家
                if (boss.IsPlayerInAttackRange())
                {
                    Debug.Log("BOSS冲锋击中玩家！");
                    // 这里可以添加冲锋伤害逻辑
                }
                
                // 返回攻击状态
                ChangeState(BossStateType.Enraged_Attack);
            }
        }
    }
    
    // 虚弱状态
    public class VulnerableState : BaseState<BossStateType, IBossObj>
    {
        private float vulnerableDuration = 5f;
        private float currentDuration = 0f;
        
        public override BossStateType StateType => BossStateType.Vulnerable;
        
        public VulnerableState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("BOSS进入Vulnerable状态");
            currentDuration = 0f;
            
            var boss = (BossStateMachineExample)GetAIObj();
            boss.IsVulnerable = true;
        }
        
        public override void QuitState()
        {
            Debug.Log("BOSS退出Vulnerable状态");
            
            var boss = (BossStateMachineExample)GetAIObj();
            boss.IsVulnerable = false;
        }
        
        public override void UpdateState()
        {
            var boss = (BossStateMachineExample)GetAIObj();
            
            // 虚弱状态下BOSS不会移动和攻击
            // 可以在这里添加视觉效果，如闪烁、减速等
            
            currentDuration += Time.deltaTime;
            
            // 虚弱状态结束
            if (currentDuration >= vulnerableDuration)
            {
                // 根据当前状态返回对应状态
                if (boss.IsEnraged)
                {
                    ChangeState(BossStateType.Enraged);
                }
                else
                {
                    ChangeState(BossStateType.Combat);
                }
            }
        }
    }
    
    // 死亡状态
    public class DeathState : BaseState<BossStateType, IBossObj>
    {
        public override BossStateType StateType => BossStateType.Death;
        
        public DeathState(StateMachine<BossStateType, IBossObj> machine) : base(machine)
        {}
        
        public override void EnterState()
        {
            Debug.Log("BOSS进入Death状态");
            
            // 这里可以添加死亡动画、效果等
        }
        
        public override void QuitState()
        {
            Debug.Log("BOSS退出Death状态");
        }
        
        public override void UpdateState()
        {
            // 死亡状态下可以添加死亡动画的更新逻辑
        }
    }
}

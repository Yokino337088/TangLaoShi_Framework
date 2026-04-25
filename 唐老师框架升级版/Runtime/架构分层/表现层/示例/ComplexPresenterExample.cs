using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 复杂表现层示例
/// 演示如何统一管理多个GameObject的脚本对象
/// </summary>
public class ComplexPresenterExample : MonoBehaviour
{
    [Header("UI元素")]
    public Text scoreText;
    public Text healthText;
    public Text timeText;
    public Slider progressSlider;
    
    [Header("游戏对象")]
    public GameObject player;
    public GameObject enemyPrefab;
    public Transform enemySpawnPoint;
    
    [Header("配置")]
    public int enemyCount = 5;
    public float spawnInterval = 2f;
    
    private List<EnemyPresenter> enemyPresenters = new List<EnemyPresenter>();
    private PlayerPresenter playerPresenter;
    private UIPresenter uiPresenter;
    private GameStatePresenter gameStatePresenter;
    
    private float spawnTimer = 0f;
    private int enemySpawned = 0;
    
    private void Start()
    {
        // 初始化表现层
        InitializePresenters();
        
        // 注册表现层到管理器
        RegisterPresenters();
        
        Debug.Log("ComplexPresenterExample initialized");
    }
    
    private void InitializePresenters()
    {
        // 初始化玩家表现层
        playerPresenter = new PlayerPresenter(player.transform);
        
        // 初始化UI表现层
        uiPresenter = new UIPresenter(scoreText, healthText, timeText, progressSlider);
        
        // 初始化游戏状态表现层
        gameStatePresenter = new GameStatePresenter();
        
        // 初始生成敌人
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }
    }
    
    private void RegisterPresenters()
    {
        // 注册表现层到Architecture
        Architecture.Instance.RegisterPresenter(playerPresenter);
        Architecture.Instance.RegisterPresenter(uiPresenter);
        Architecture.Instance.RegisterPresenter(gameStatePresenter);
        
        // 注册敌人表现层
        foreach (var enemyPresenter in enemyPresenters)
        {
            Architecture.Instance.RegisterPresenter(enemyPresenter);
        }
    }
    
    private void Update()
    {
        // 生成敌人
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && enemySpawned < 10)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
        
        // 更新UI - 从表现层获取数据
        uiPresenter.UpdateScore(playerPresenter.Score);
        uiPresenter.UpdateHealth(playerPresenter.Health);
        uiPresenter.UpdateTime(gameStatePresenter.GameTime);
        uiPresenter.UpdateProgress((float)enemySpawned / 10f);
        
        // 注意：所有表现层的Update方法现在由PresentMgr统一管理
        // 不需要在这里手动调用，PresentMgr会在Architecture.Update中统一调用
    }
    
    private void SpawnEnemy()
    {
        if (enemyPrefab == null || enemySpawnPoint == null)
            return;
        
        GameObject enemyObj = Instantiate(enemyPrefab, enemySpawnPoint.position, Quaternion.identity);
        EnemyPresenter enemyPresenter = new EnemyPresenter(enemyObj.transform, player.transform);
        enemyPresenters.Add(enemyPresenter);
        
        // 注册新生成的敌人表现层 - 这会触发Initialize和Start
        Architecture.Instance.RegisterPresenter(enemyPresenter);
        
        enemySpawned++;
    }
    
    private void OnDestroy()
    {
        // 清理表现层
        foreach (var enemyPresenter in enemyPresenters)
        {
            Architecture.Instance.UnregisterPresenter<EnemyPresenter>();
        }
        
        Architecture.Instance.UnregisterPresenter<PlayerPresenter>();
        Architecture.Instance.UnregisterPresenter<UIPresenter>();
        Architecture.Instance.UnregisterPresenter<GameStatePresenter>();
        
        Debug.Log("ComplexPresenterExample destroyed");
    }
}

/// <summary>
/// 玩家表现层
/// </summary>
public class PlayerPresenter : BasePresenter
{
    private Transform playerTransform;
    private int score = 0;
    private int health = 100;
    private float moveSpeed = 5f;
    
    public int Score => score;
    public int Health => health;
    
    public PlayerPresenter(Transform transform)
    {
        playerTransform = transform;
    }
    
    public override void Initialize()
    {
        Debug.Log("PlayerPresenter Initialize");
    }
    
    public override void Start()
    {
        Debug.Log("PlayerPresenter Start");
    }
    
    public override void Update()
    {
        // 处理玩家移动 - 这个方法会被PresentMgr统一调用
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        playerTransform.Translate(movement);
        
        // 处理玩家旋转
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            playerTransform.Rotate(0, mouseX * 180f * Time.deltaTime, 0);
        }
    }
    
    public void AddScore(int value)
    {
        score += value;
    }
    
    public void TakeDamage(int value)
    {
        health = Mathf.Max(0, health - value);
    }
}

/// <summary>
/// 敌人表现层
/// </summary>
public class EnemyPresenter : BasePresenter
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float moveSpeed = 3f;
    private float attackRange = 1f;
    private int health = 50;
    private float attackCooldown = 0f;
    private const float ATTACK_INTERVAL = 1f;
    
    public EnemyPresenter(Transform transform, Transform player)
    {
        enemyTransform = transform;
        playerTransform = player;
    }
    
    public override void Initialize()
    {
        Debug.Log("EnemyPresenter Initialize");
    }
    
    public override void Start()
    {
        Debug.Log("EnemyPresenter Start");
    }
    
    public override void Update()
    {
        // 这个方法会被PresentMgr统一调用
        if (playerTransform == null || enemyTransform == null)
            return;
        
        // 攻击冷却
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        
        // 追踪玩家
        Vector3 direction = playerTransform.position - enemyTransform.position;
        float distance = direction.magnitude;
        
        if (distance > attackRange)
        {
            // 移动向玩家
            enemyTransform.Translate(direction.normalized * moveSpeed * Time.deltaTime);
            // 看向玩家
            enemyTransform.LookAt(playerTransform);
        }
        else
        {
            // 攻击玩家
            if (attackCooldown <= 0)
            {
                AttackPlayer();
                attackCooldown = ATTACK_INTERVAL;
            }
        }
    }
    
    private void AttackPlayer()
    {
        // 尝试获取玩家表现层并造成伤害
        var playerPresenter = Architecture.Instance.GetPresenter<PlayerPresenter>();
        if (playerPresenter != null)
        {
            playerPresenter.TakeDamage(5);
            Debug.Log($"Enemy attacked player! Player health: {playerPresenter.Health}");
        }
    }
    
    public void TakeDamage(int value)
    {
        health = Mathf.Max(0, health - value);
        if (health <= 0)
        {
            // 敌人死亡
            Die();
        }
    }
    
    private void Die()
    {
        // 增加玩家分数
        var playerPresenter = Architecture.Instance.GetPresenter<PlayerPresenter>();
        if (playerPresenter != null)
        {
            playerPresenter.AddScore(10);
            Debug.Log($"Enemy died! Player score: {playerPresenter.Score}");
        }
        
        // 销毁敌人对象
        if (enemyTransform != null)
        {
            Object.Destroy(enemyTransform.gameObject);
        }
    }
}

/// <summary>
/// UI表现层
/// </summary>
public class UIPresenter : BasePresenter
{
    private Text scoreText;
    private Text healthText;
    private Text timeText;
    private Slider progressSlider;
    
    public UIPresenter(Text score, Text health, Text time, Slider progress)
    {
        scoreText = score;
        healthText = health;
        timeText = time;
        progressSlider = progress;
    }
    
    public override void Initialize()
    {
        Debug.Log("UIPresenter Initialize");
    }
    
    public override void Start()
    {
        Debug.Log("UIPresenter Start");
        UpdateScore(0);
        UpdateHealth(100);
        UpdateTime(0);
        UpdateProgress(0);
    }
    
    public override void Update()
    {
        // UI实时更新逻辑 - 这个方法会被PresentMgr统一调用
        // 注意：实际的UI更新在ComplexPresenterExample.Update中调用
        // 因为UI需要获取其他表现层的数据
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
    
    public void UpdateHealth(int health)
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + health;
        }
    }
    
    public void UpdateTime(float time)
    {
        if (timeText != null)
        {
            timeText.text = "Time: " + time.ToString("F1");
        }
    }
    
    public void UpdateProgress(float progress)
    {
        if (progressSlider != null)
        {
            progressSlider.value = progress;
        }
    }
}

/// <summary>
/// 游戏状态表现层
/// </summary>
public class GameStatePresenter : BasePresenter
{
    private float gameTime = 0f;
    
    public float GameTime => gameTime;
    
    public override void Initialize()
    {
        Debug.Log("GameStatePresenter Initialize");
    }
    
    public override void Start()
    {
        Debug.Log("GameStatePresenter Start");
    }
    
    public override void Update()
    {
        // 更新游戏时间 - 这个方法会被PresentMgr统一调用
        gameTime += Time.deltaTime;
    }
}
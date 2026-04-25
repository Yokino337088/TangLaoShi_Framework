using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI表现层示例
/// </summary>
public class UIPresenterExample : BasePresenter
{
    private Text scoreText;
    private int score;
    
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Initialize()
    {
        Debug.Log("UIPresenterExample Initialize");
        // 模拟查找UI元素
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        score = 0;
    }
    
    /// <summary>
    /// 启动
    /// </summary>
    public override void Start()
    {
        Debug.Log("UIPresenterExample Start");
        UpdateScore();
    }
    
    /// <summary>
    /// 更新
    /// </summary>
    public override void Update()
    {
        // 这里可以处理UI的实时更新
    }
    
    /// <summary>
    /// 停止
    /// </summary>
    public override void Stop()
    {
        Debug.Log("UIPresenterExample Stop");
    }
    
    /// <summary>
    /// 销毁
    /// </summary>
    public override void Dispose()
    {
        Debug.Log("UIPresenterExample Dispose");
    }
    
    /// <summary>
    /// 更新分数
    /// </summary>
    public void AddScore(int value)
    {
        score += value;
        UpdateScore();
    }
    
    /// <summary>
    /// 更新分数显示
    /// </summary>
    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
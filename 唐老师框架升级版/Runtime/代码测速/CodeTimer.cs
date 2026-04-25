using System;
using System.Diagnostics;

/// <summary>
/// 代码执行时间计时器
/// 使用Using语句块包裹代码，自动计算执行时间
/// </summary>
public class CodeTimer : IDisposable
{
    // 计时器
    private Stopwatch stopwatch;
    // 计时名称
    private string timerName;
    // 是否输出日志
    private bool outputLog;
    // 执行时间（毫秒）
    private double elapsedMilliseconds;
    
    /// <summary>
    /// 执行时间（毫秒）
    /// </summary>
    public double ElapsedMilliseconds
    {
        get { return elapsedMilliseconds; }
    }
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name">计时器名称</param>
    /// <param name="logOutput">是否输出日志</param>
    public CodeTimer(string name = "CodeTimer", bool logOutput = true)
    {
        timerName = name;
        outputLog = logOutput;
        stopwatch = new Stopwatch();
        stopwatch.Start();
        
        if (outputLog)
        {
            LogSystem.Info($"[{timerName}] 开始计时");
        }
    }
    
    /// <summary>
    /// 结束计时并获取执行时间
    /// </summary>
    /// <returns>执行时间（毫秒）</returns>
    public double Stop()
    {
        if (stopwatch.IsRunning)
        {
            stopwatch.Stop();
            elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            
            if (outputLog)
            {
                LogSystem.Info($"[{timerName}] 执行时间: {elapsedMilliseconds:F4} 毫秒");
            }
        }
        return elapsedMilliseconds;
    }
    
    /// <summary>
    /// 重置计时器
    /// </summary>
    public void Reset()
    {
        stopwatch.Reset();
        stopwatch.Start();
        
        if (outputLog)
        {
            LogSystem.Info($"[{timerName}] 重置计时");
        }
    }
    
    /// <summary>
    /// 实现IDisposable接口
    /// </summary>
    public void Dispose()
    {
        Stop();
        stopwatch = null;
    }
    
    /// <summary>
    /// 创建并启动计时器
    /// </summary>
    /// <param name="name">计时器名称</param>
    /// <param name="logOutput">是否输出日志</param>
    /// <returns>计时器实例</returns>
    public static CodeTimer Start(string name = "CodeTimer", bool logOutput = true)
    {
        return new CodeTimer(name, logOutput);
    }
}

/// <summary>
/// 代码执行时间计时器扩展
/// </summary>
public static class CodeTimerExtensions
{
    /// <summary>
    /// 测量动作的执行时间
    /// </summary>
    /// <param name="action">要执行的动作</param>
    /// <param name="name">计时器名称</param>
    /// <param name="logOutput">是否输出日志</param>
    /// <returns>执行时间（毫秒）</returns>
    public static double MeasureExecutionTime(this Action action, string name = "CodeTimer", bool logOutput = true)
    {
        using (var timer = new CodeTimer(name, logOutput))
        {
            action?.Invoke();
            return timer.ElapsedMilliseconds;
        }
    }
    
    /// <summary>
    /// 测量函数的执行时间并返回结果
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="func">要执行的函数</param>
    /// <param name="result">函数执行结果</param>
    /// <param name="name">计时器名称</param>
    /// <param name="logOutput">是否输出日志</param>
    /// <returns>执行时间（毫秒）</returns>
    public static double MeasureExecutionTime<T>(this Func<T> func, out T result, string name = "CodeTimer", bool logOutput = true)
    {
        result = default(T);
        using (var timer = new CodeTimer(name, logOutput))
        {
            if (func != null)
            {
                result = func();
            }
            return timer.ElapsedMilliseconds;
        }
    }
}

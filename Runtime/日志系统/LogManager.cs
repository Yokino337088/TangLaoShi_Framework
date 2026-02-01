using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 日志管理器
/// 负责管理游戏中的日志输出
/// 只在开发环境下打印日志，发布环境下不打印
/// </summary>
public class LogManager : BaseManager<LogManager>
{
    // 私有构造函数
    private LogManager() { }
    
    // 是否为开发环境
    private bool isDevelopmentBuild = true;
    
    // 是否启用日志文件
    private bool enableLogFile = false;
    
    // 日志文件路径
    private string logFilePath = "";
    
    // 日志等级
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }
    
    // 当前日志等级
    private LogLevel currentLogLevel = LogLevel.Debug;
    
    /// <summary>
    /// 初始化日志管理器
    /// </summary>
    public void Initialize()
    {
        // 检测是否为开发环境
        isDevelopmentBuild = Debug.isDebugBuild;
        
        // 初始化日志文件
        InitializeLogFile();
        
        Debug.Log("日志管理器初始化完成");
        LogInfo("日志管理器初始化完成");
    }
    
    /// <summary>
    /// 初始化日志文件
    /// </summary>
    private void InitializeLogFile()
    {
        if (enableLogFile && isDevelopmentBuild)
        {
            try
            {
                string logDir = Path.Combine(Application.persistentDataPath, "Logs");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                logFilePath = Path.Combine(logDir, $"log_{timestamp}.txt");
                
                using (StreamWriter writer = File.CreateText(logFilePath))
                {
                    writer.WriteLine($"=== 日志文件开始 ===");
                    writer.WriteLine($"时间: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                    writer.WriteLine($"应用版本: {Application.version}");
                    writer.WriteLine($"平台: {Application.platform}");
                    writer.WriteLine($"=== 日志内容 ===");
                }
                
                Debug.Log($"日志文件已创建: {logFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"创建日志文件失败: {e.Message}");
                enableLogFile = false;
            }
        }
    }
    
    /// <summary>
    /// 设置日志等级
    /// </summary>
    /// <param name="level">日志等级</param>
    public void SetLogLevel(LogLevel level)
    {
        currentLogLevel = level;
        LogInfo($"日志等级已设置为: {level}");
    }
    
    /// <summary>
    /// 启用或禁用日志文件
    /// </summary>
    /// <param name="enable">是否启用</param>
    public void EnableLogFile(bool enable)
    {
        enableLogFile = enable;
        if (enable)
        {
            InitializeLogFile();
        }
        LogInfo($"日志文件已{(enable ? "启用" : "禁用")}");
    }
    
    /// <summary>
    /// 打印调试日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public void LogDebug(string message, UnityEngine.Object context = null)
    {
        if (ShouldLog(LogLevel.Debug))
        {
            Debug.Log($"[DEBUG] {message}", context);
            WriteToLogFile($"[DEBUG] {message}");
        }
    }
    
    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public void LogInfo(string message, UnityEngine.Object context = null)
    {
        if (ShouldLog(LogLevel.Info))
        {
            Debug.Log($"[INFO] {message}", context);
            WriteToLogFile($"[INFO] {message}");
        }
    }
    
    /// <summary>
    /// 打印警告日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public void LogWarning(string message, UnityEngine.Object context = null)
    {
        if (ShouldLog(LogLevel.Warning))
        {
            Debug.LogWarning($"[WARNING] {message}", context);
            WriteToLogFile($"[WARNING] {message}");
        }
    }
    
    /// <summary>
    /// 打印错误日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public void LogError(string message, UnityEngine.Object context = null)
    {
        if (ShouldLog(LogLevel.Error))
        {
            Debug.LogError($"[ERROR] {message}", context);
            WriteToLogFile($"[ERROR] {message}");
        }
    }
    
    /// <summary>
    /// 打印致命错误日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public void LogFatal(string message, UnityEngine.Object context = null)
    {
        if (ShouldLog(LogLevel.Fatal))
        {
            Debug.LogError($"[FATAL] {message}", context);
            WriteToLogFile($"[FATAL] {message}");
        }
    }
    
    /// <summary>
    /// 格式化打印日志
    /// </summary>
    /// <param name="level">日志等级</param>
    /// <param name="format">格式化字符串</param>
    /// <param name="args">格式化参数</param>
    public void LogFormat(LogLevel level, string format, params object[] args)
    {
        string message = string.Format(format, args);
        switch (level)
        {
            case LogLevel.Debug:
                LogDebug(message);
                break;
            case LogLevel.Info:
                LogInfo(message);
                break;
            case LogLevel.Warning:
                LogWarning(message);
                break;
            case LogLevel.Error:
                LogError(message);
                break;
            case LogLevel.Fatal:
                LogFatal(message);
                break;
        }
    }
    
    /// <summary>
    /// 检查是否应该打印该等级的日志
    /// </summary>
    /// <param name="level">日志等级</param>
    /// <returns>是否应该打印</returns>
    private bool ShouldLog(LogLevel level)
    {
        // 只有在开发环境下才打印日志
        if (!isDevelopmentBuild)
        {
            return false;
        }
        
        // 检查日志等级
        return level >= currentLogLevel;
    }
    
    /// <summary>
    /// 写入日志到文件
    /// </summary>
    /// <param name="message">日志消息</param>
    private void WriteToLogFile(string message)
    {
        if (enableLogFile && !string.IsNullOrEmpty(logFilePath) && isDevelopmentBuild)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"写入日志文件失败: {e.Message}");
                enableLogFile = false;
            }
        }
    }
    
    /// <summary>
    /// 获取日志文件路径
    /// </summary>
    /// <returns>日志文件路径</returns>
    public string GetLogFilePath()
    {
        return logFilePath;
    }
    
    /// <summary>
    /// 清理日志管理器
    /// </summary>
    public void Cleanup()
    {
        if (enableLogFile && !string.IsNullOrEmpty(logFilePath))
        {
            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"=== 日志文件结束 ===");
                    writer.WriteLine($"时间: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"关闭日志文件失败: {e.Message}");
            }
        }
        Debug.Log("日志管理器清理完成");
    }
}

/// <summary>
/// 日志系统静态类
/// 提供静态方法方便调用
/// </summary>
public static class LogSystem
{
    /// <summary>
    /// 打印调试日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public static void Debug(string message, UnityEngine.Object context = null)
    {
        LogManager.Instance.LogDebug(message, context);
    }
    
    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public static void Info(string message, UnityEngine.Object context = null)
    {
        LogManager.Instance.LogInfo(message, context);
    }
    
    /// <summary>
    /// 打印警告日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public static void Warning(string message, UnityEngine.Object context = null)
    {
        LogManager.Instance.LogWarning(message, context);
    }
    
    /// <summary>
    /// 打印错误日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public static void Error(string message, UnityEngine.Object context = null)
    {
        LogManager.Instance.LogError(message, context);
    }
    
    /// <summary>
    /// 打印致命错误日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="context">上下文对象</param>
    public static void Fatal(string message, UnityEngine.Object context = null)
    {
        LogManager.Instance.LogFatal(message, context);
    }
    
    /// <summary>
    /// 格式化打印日志
    /// </summary>
    /// <param name="level">日志等级</param>
    /// <param name="format">格式化字符串</param>
    /// <param name="args">格式化参数</param>
    public static void Format(LogManager.LogLevel level, string format, params object[] args)
    {
        LogManager.Instance.LogFormat(level, format, args);
    }
}

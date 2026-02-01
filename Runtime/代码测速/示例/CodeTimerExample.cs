using System;
using UnityEngine;

/// <summary>
/// 代码计时工具使用示例
/// </summary>
public class CodeTimerExample : MonoBehaviour
{
    private void Start()
    {
        // 示例1：基本使用方法
        Example1_BasicUsage();
        
        // 示例2：带返回值的函数计时
        Example2_FunctionWithReturn();
        
        // 示例3：比较不同算法的执行时间
        Example3_AlgorithmComparison();
    }
    
    /// <summary>
    /// 示例1：基本使用方法
    /// </summary>
    private void Example1_BasicUsage()
    {
        LogSystem.Info("=== 示例1：基本使用方法 ===");
        
        // 使用Using语句块包裹需要计时的代码
        using (new CodeTimer("基本计时示例"))
        {
            // 模拟一些耗时操作
            for (int i = 0; i < 1000000; i++)
            {
                // 执行一些计算
                Mathf.Sqrt(i);
            }
        }
        
        LogSystem.Info("\n");
    }
    
    /// <summary>
    /// 示例2：带返回值的函数计时
    /// </summary>
    private void Example2_FunctionWithReturn()
    {
        LogSystem.Info("=== 示例2：带返回值的函数计时 ===");
        
        // 定义一个耗时函数
        Func<int> timeConsumingFunction = () =>
        {
            int sum = 0;
            for (int i = 0; i < 1000000; i++)
            {
                sum += i;
            }
            return sum;
        };
        
        // 使用扩展方法测量函数执行时间
        int result;
        double executionTime = timeConsumingFunction.MeasureExecutionTime(out result, "带返回值的函数计时");
        
        LogSystem.Info($"函数执行结果: {result}");
        LogSystem.Info($"函数执行时间: {executionTime:F4} 毫秒");
        
        LogSystem.Info("\n");
    }
    
    /// <summary>
    /// 示例3：比较不同算法的执行时间
    /// </summary>
    private void Example3_AlgorithmComparison()
    {
        LogSystem.Info("=== 示例3：比较不同算法的执行时间 ===");
        
        // 测试数据
        int[] testArray = new int[10000];
        for (int i = 0; i < testArray.Length; i++)
        {
            testArray[i] = UnityEngine.Random.Range(0, 10000);
        }
        
        // 复制数组用于不同排序算法
        int[] array1 = new int[testArray.Length];
        int[] array2 = new int[testArray.Length];
        Array.Copy(testArray, array1, testArray.Length);
        Array.Copy(testArray, array2, testArray.Length);
        
        // 测试冒泡排序
        using (new CodeTimer("冒泡排序"))
        {
            BubbleSort(array1);
        }
        
        // 测试Array.Sort
        using (new CodeTimer("Array.Sort"))
        {
            Array.Sort(array2);
        }
        
        LogSystem.Info("\n");
    }
    
    /// <summary>
    /// 冒泡排序算法
    /// </summary>
    /// <param name="array">要排序的数组</param>
    private void BubbleSort(int[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = 0; j < array.Length - i - 1; j++)
            {
                if (array[j] > array[j + 1])
                {
                    int temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }
    }
}

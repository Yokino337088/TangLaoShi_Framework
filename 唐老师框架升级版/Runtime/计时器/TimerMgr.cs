using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

/// <summary>
/// 计时器管理器 主要用于开启、停止、重置等等操作来管理计时器
/// </summary>
public class TimerMgr : BaseManager<TimerMgr>
{
    /// <summary>
    /// 用于记录当前将要创建的唯一ID的
    /// </summary>
    private int TIMER_KEY = 0;
    /// <summary>
    /// 用于存储管理所有计时器的字典容器
    /// </summary>
    private Dictionary<int, TimerItem> timerDic = new Dictionary<int, TimerItem>();
    /// <summary>
    /// 用于存储管理所有计时器的字典容器（不受Time.timeScale影响的计时器）
    /// </summary>
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>();
    /// <summary>
    /// 待移除列表
    /// </summary>
    private List<TimerItem> delList = new List<TimerItem>();

    //为了避免内存的浪费 每次while都会生成 
    //我们直接将其声明为成员变量
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime);
    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime);

    private CancellationTokenSource cts;
    private CancellationTokenSource realCts;

    /// <summary>
    /// 计时器管理器中的唯一计时用的协同程序 的间隔时间
    /// </summary>
    private const float intervalTime = 0.1f;

    private TimerMgr() 
    {
        //默认计时器就是开启的
        Start();
    }

    //开启计时器管理器的方法
    public void Start()
    {
        cts = new CancellationTokenSource();
        realCts = new CancellationTokenSource();
        StartTimingAsync(false, timerDic, cts.Token).Forget();
        StartTimingAsync(true, realTimerDic, realCts.Token).Forget();
    }

    //关闭计时器管理器的方法
    public void Stop()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
        if (realCts != null)
        {
            realCts.Cancel();
            realCts.Dispose();
            realCts = null;
        }
    }


    private async UniTask StartTimingAsync(bool isRealTime, Dictionary<int, TimerItem> timerDic, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                //100毫秒进行一次计时
                if (isRealTime)
                    await UniTask.Delay((int)(intervalTime * 1000), true, PlayerLoopTiming.Update, cancellationToken);
                else
                    await UniTask.Delay((int)(intervalTime * 1000), false, PlayerLoopTiming.Update, cancellationToken);
                
                //遍历所有的计时器 进行数据更新
                foreach (TimerItem item in timerDic.Values)
                {
                    if (!item.isRuning)
                        continue;
                    //判断计时器是否有间隔时间执行的需求
                    if(item.callBack != null)
                    {
                        //减去100毫秒
                        item.intervalTime -= (int)(intervalTime*1000);
                        //满足一次间隔时间执行
                        if(item.intervalTime <= 0)
                        {
                            //间隔一定时间 执行一次回调
                            item.callBack.Invoke();
                            //重置间隔时间
                            item.intervalTime = item.maxIntervalTime;
                        }
                    }
                    //总的时间更新
                    item.allTime -= (int)(intervalTime * 1000);
                    //计时时间到 需要执行完成回调函数
                    if(item.allTime <= 0)
                    {
                        item.overCallBack.Invoke();
                        delList.Add(item);
                    }
                }

                //移除待移除列表中的数据
                for (int i = 0; i < delList.Count; i++)
                {
                    //从字典中移除
                    timerDic.Remove(delList[i].keyID);
                    //放入缓存池中
                    GOPoolMgr.Instance.PushObj(delList[i]);
                }
                //移除结束后 清空列表
                delList.Clear();
            }
        }
        catch (OperationCanceledException)
        {
            // 任务被取消，正常退出
        }
    }

    /// <summary>
    /// 创建单个计时器
    /// </summary>
    /// <param name="isRealTime">如果是true不受Time.timeScale影响</param>
    /// <param name="allTime">总的时间 毫秒 1s=1000ms</param>
    /// <param name="overCallBack">总时间结束回调</param>
    /// <param name="intervalTime">间隔计时时间 毫秒 1s=1000ms</param>
    /// <param name="callBack">间隔计时时间结束 回调</param>
    /// <returns>返回唯一ID 用于外部控制对应计时器</returns>
    public int CreateTimer(bool isRealTime, int allTime, Action overCallBack, int intervalTime = 0, Action callBack = null)
    {
        //构建唯一ID
        int keyID = ++TIMER_KEY;
        //从缓存池取出对应的计时器
        TimerItem timerItem = GOPoolMgr.Instance.GetObj<TimerItem>();
        //初始化数据
        timerItem.InitInfo(keyID, allTime, overCallBack, intervalTime, callBack);
        //记录到字典中 进行数据更新
        if (isRealTime)
            realTimerDic.Add(keyID, timerItem);
        else
            timerDic.Add(keyID, timerItem);
        return keyID;
    }

    //移除单个计时器
    public void RemoveTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
        {
            //移除对应id计时器 放入缓存池
            GOPoolMgr.Instance.PushObj(timerDic[keyID]);
            //从字典中移除
            timerDic.Remove(keyID);
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            //移除对应id计时器 放入缓存池
            GOPoolMgr.Instance.PushObj(realTimerDic[keyID]);
            //从字典中移除
            realTimerDic.Remove(keyID);
        }
    }

    /// <summary>
    /// 重置单个计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void ResetTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].ResetTimer();
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].ResetTimer();
        }
    }

    /// <summary>
    /// 开启当个计时器 主要用于暂停后重新开始
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void StartTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRuning = true;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRuning = true;
        }
    }

    /// <summary>
    /// 停止单个计时器 主要用于暂停
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
    public void StopTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRuning = false;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRuning = false;
        }
    }
}


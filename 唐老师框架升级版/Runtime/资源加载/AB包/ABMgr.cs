using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;

//知识点
//字典
//UniTask异步操作
//AB包相关API
//委托
//lambda表达式
//单例模式基类——>观看Unity小框架视频 进行学习
public class ABMgr : BaseManager<ABMgr>
{
    //主包
    private AssetBundle mainAB = null;
    //主包依赖获取配置文件
    private AssetBundleManifest manifest = null;

    //选择存储 AB包的容器
    //AB包不能够重复加载 否则会报错
    //字典知识 用来存储 AB包对象
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
    
    //引用计数字典
    private Dictionary<string, int> abRefCount = new Dictionary<string, int>();

    /// <summary>
    /// 获取AB包加载路径 - streamingAssetsPath
    /// </summary>
    private string StreamingAssetsPath
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    /// <summary>
    /// 获取AB包加载路径 - persistentDataPath
    /// </summary>
    private string PersistentDataPath
    {
        get
        {
            return Application.persistentDataPath + "/";
        }
    }

    /// <summary>
    /// 主包名 根据平台不同 报名不同
    /// </summary>
    private string MainName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// 加载主包 和 配置文件
    /// 因为加载所有包是 都得判断 通过它才能得到依赖信息
    /// 所以写一个方法
    /// </summary>
    private void LoadMainAB()
    {
        if (mainAB == null)
        {
            // 尝试从两个路径加载主包
            mainAB = LoadAssetBundleFromMultiplePaths(MainName);
            if (mainAB != null)
            {
                manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            else
            {
                LogSystem.Error("无法从任何路径加载主包: " + MainName);
            }
        }
    }

    /// <summary>
    /// 尝试从多个路径加载AssetBundle
    /// 优先从persistentDataPath加载(可写路径)，如果不存在则从streamingAssetsPath加载(只读路径)
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <returns>加载到的AssetBundle，如果所有路径都加载失败则返回null</returns>
    private AssetBundle LoadAssetBundleFromMultiplePaths(string abName)
    {
        // 1. 首先尝试从persistentDataPath加载
        string persistentPath = PersistentDataPath + abName;
        if (File.Exists(persistentPath))
        {
            LogSystem.Info("从persistentDataPath加载AB包: " + abName);
            return AssetBundle.LoadFromFile(persistentPath);
        }

        // 2. 如果persistentDataPath不存在，则尝试从streamingAssetsPath加载
        string streamingPath = StreamingAssetsPath + abName;
        if (File.Exists(streamingPath))
        {
            LogSystem.Info("从streamingAssetsPath加载AB包: " + abName);
            return AssetBundle.LoadFromFile(streamingPath);
        }

        // 3. 如果两个路径都不存在，返回null
        LogSystem.Warning("AB包在两个路径中都不存在: " + abName);
        return null;
    }

    private AssetBundleCreateRequest LoadAssetBundleFromMultiplePathsRequest(string abName)
    {
        // 1. 首先尝试从persistentDataPath加载
        string persistentPath = PersistentDataPath + abName;
        if (File.Exists(persistentPath))
        {
            LogSystem.Info("从persistentDataPath加载AB包: " + abName);
            return AssetBundle.LoadFromFileAsync(persistentPath);
        }

        // 2. 如果persistentDataPath不存在，则尝试从streamingAssetsPath加载
        string streamingPath = StreamingAssetsPath + abName;
        if (File.Exists(streamingPath))
        {
            LogSystem.Info("从streamingAssetsPath加载AB包: " + abName);
            return AssetBundle.LoadFromFileAsync(streamingPath);
        }

        // 3. 如果两个路径都不存在，返回null
        LogSystem.Warning("AB包在两个路径中都不存在: " + abName);
        return null;
    }

    /// <summary>
    /// 异步从多个路径加载AssetBundle
    /// </summary>
    private async UniTask<AssetBundle> LoadAssetBundleFromMultiplePathsAsync(string abName)
    {
        // 1. 首先尝试从persistentDataPath加载
        string persistentPath = PersistentDataPath + abName;
        if (File.Exists(persistentPath))
        {
            LogSystem.Info("异步从persistentDataPath加载AB包: " + abName);
            var request = AssetBundle.LoadFromFileAsync(persistentPath);
            await request;
            return request.assetBundle;
        }

        // 2. 如果persistentDataPath不存在，则尝试从streamingAssetsPath加载
        string streamingPath = StreamingAssetsPath + abName;
        if (File.Exists(streamingPath))
        {
            LogSystem.Info("异步从streamingAssetsPath加载AB包: " + abName);
            var request = AssetBundle.LoadFromFileAsync(streamingPath);
            await request;
            return request.assetBundle;
        }

        // 3. 如果两个路径都不存在，返回null
        LogSystem.Warning("AB包在两个路径中都不存在: " + abName);
        return null;
    }

    /// <summary>
    /// 加载指定包的依赖包
    /// </summary>
    /// <param name="abName"></param>
    private void LoadDependencies(string abName)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                AssetBundle ab = LoadAssetBundleFromMultiplePaths(strs[i]);
                if (ab != null)
                {
                    abDic.Add(strs[i], ab);
                }
                else
                {
                    LogSystem.Error("无法加载依赖包: " + strs[i]);
                }
            }
            AddRefCount(strs[i]);
        }
    }

    /// <summary>
    /// 泛型资源同步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public T LoadRes<T>(string abName, string resName) where T : UnityEngine.Object
    {
        //加载依赖包
        LoadDependencies(abName);
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            AssetBundle ab = LoadAssetBundleFromMultiplePaths(abName);
            if (ab != null)
            {
                abDic.Add(abName, ab);
            }
            else
            {
                LogSystem.Error("无法加载AB包: " + abName);
                return null;
            }
        }
        //增加目标包的引用计数
        AddRefCount(abName);
        // 更新AB包的使用时间
        UpdateABLastUsedTime(abName);

        //得到加载出来的资源
        T obj = abDic[abName].LoadAsset<T>(resName);
        //如果是GameObject 因为GameObject 100%都是需要实例化的
        //所以我们直接实例化
        if (obj is GameObject)
            return GameObject.Instantiate(obj);
        else
            return obj;

    }



    /// <summary>
    /// 泛型异步加载资源（回调版本）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public async void LoadResAsync<T>(string abName, string resName, Action<T> callBack, bool isSync = false) where T : UnityEngine.Object
    {
        await ReallyLoadResAsync<T>(abName, resName, callBack, isSync);
    }

    /// <summary>
    /// 泛型异步加载资源（返回对象版本）
    /// 不需要传入回调函数，直接返回加载的资源对象
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="abName">AB包名称</param>
    /// <param name="resName">资源名称</param>
    /// <param name="isSync">是否同步加载</param>
    /// <returns>加载的资源对象</returns>
    public async UniTask<T> LoadResAsync<T>(string abName, string resName, bool isSync = false) where T : UnityEngine.Object
    {
        T result = null;
        await ReallyLoadResAsync<T>(abName, resName, (res) => { result = res; }, isSync);
        return result;
    }

    //正儿八经的 异步函数
    private async UniTask ReallyLoadResAsync<T>(string abName, string resName, Action<T> callBack, bool isSync) where T : UnityEngine.Object
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //还没有加载过该AB包
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if (isSync)
                {
                    AssetBundle ab = LoadAssetBundleFromMultiplePaths(strs[i]);
                    if (ab != null)
                    {
                        abDic.Add(strs[i], ab);
                    }
                    else
                    {
                        LogSystem.Error("无法同步加载依赖包: " + strs[i]);
                    }
                }
                //异步加载
                else
                {
                    //一开始异步加载 就记录 如果此时的记录中的值 是null 那证明这个ab包正在被异步加载
                    abDic.Add(strs[i], null);
                    
                    // 使用新的异步加载方法
                    AssetBundle bundle = await LoadAssetBundleFromMultiplePathsAsync(strs[i]);
                    
                    //异步加载结束后 再替换之前的null  这时 不为null 就证明加载结束了
                    abDic[strs[i]] = bundle;
                }
            }
            //就证明 字典中已经记录了一个AB包相关信息了
            else
            {
                //如果字典中记录的信息是null 那就证明正在加载中
                //我们只需要等待它加载结束 就可以继续执行后面的代码了
                while (abDic[strs[i]] == null)
                {
                    //只要发现正在加载中 就不停的等待一帧 下一帧再进行判断
                    await UniTask.Yield();
                }
            }
            //增加依赖包的引用计数
            AddRefCount(strs[i]);
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isSync)
            {
                AssetBundle ab = LoadAssetBundleFromMultiplePaths(abName);
                if (ab != null)
                {
                    abDic.Add(abName, ab);
                }
                else
                {
                    LogSystem.Error("无法同步加载目标包: " + abName);
                }
            }
            else
            {
                //一开始异步加载 就记录 如果此时的记录中的值 是null 那证明这个ab包正在被异步加载
                abDic.Add(abName, null);
                
                // 使用新的异步加载方法
                AssetBundle bundle = await LoadAssetBundleFromMultiplePathsAsync(abName);
                
                //异步加载结束后 再替换之前的null  这时 不为null 就证明加载结束了
                abDic[abName] = bundle;
            }
        }
        else
        {
            //如果字典中记录的信息是null 那就证明正在加载中
            //我们只需要等待它加载结束 就可以继续执行后面的代码了
            while (abDic[abName] == null)
            {
                //只要发现正在加载中 就不停的等待一帧 下一帧再进行判断
                await UniTask.Yield();
            }
        }
        //增加目标包的引用计数
        AddRefCount(abName);
        // 更新AB包的使用时间
        UpdateABLastUsedTime(abName);

        //同步加载AB包中的资源
        if (isSync)
        {
            //即使是同步加载 也需要使用回调函数传给外部进行使用
            if (abDic[abName] != null)
            {
                T res = abDic[abName].LoadAsset<T>(resName);
                callBack(res);
            }
            else
            {
                LogSystem.Error("AB包为空，无法加载资源: " + resName);
                callBack(null);
            }
        }
        //异步加载包中资源
        else
        {
            if (abDic[abName] != null)
            {
                var abq = abDic[abName].LoadAssetAsync<T>(resName);
                await abq;
                callBack(abq.asset as T);
            }
            else
            {
                LogSystem.Error("AB包为空，无法异步加载资源: " + resName);
                callBack(null);
            }
        }
    }

    /// <summary>
    /// Type异步加载资源（回调版本）
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public async void LoadResAsync(string abName, string resName, System.Type type, Action<UnityEngine.Object> callBack, bool isSync = false)
    {
        await ReallyLoadResAsync(abName, resName, type, callBack, isSync);
    }

    /// <summary>
    /// Type异步加载资源（返回对象版本）
    /// 不需要传入回调函数，直接返回加载的资源对象
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <param name="resName">资源名称</param>
    /// <param name="type">资源类型</param>
    /// <param name="isSync">是否同步加载</param>
    /// <returns>加载的资源对象</returns>
    public async UniTask<UnityEngine.Object> LoadResAsync(string abName, string resName, System.Type type, bool isSync = false)
    {
        UnityEngine.Object result = null;
        await ReallyLoadResAsync(abName, resName, type, (res) => { result = res; }, isSync);
        return result;
    }

    private async UniTask ReallyLoadResAsync(string abName, string resName, System.Type type, Action<UnityEngine.Object> callBack, bool isSync)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //还没有加载过该AB包
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if (isSync)
                {
                    AssetBundle ab = LoadAssetBundleFromMultiplePaths(strs[i]);
                    abDic.Add(strs[i], ab);
                }
                //异步加载
                else
                {
                    //一开始异步加载 就记录 如果此时的记录中的值 是null 那证明这个ab包正在被异步加载
                    abDic.Add(strs[i], null);
                    AssetBundle bundle = await LoadAssetBundleFromMultiplePathsAsync(strs[i]);
                    //异步加载结束后 再替换之前的null  这时 不为null 就证明加载结束了
                    abDic[strs[i]] = bundle;
                }
            }
            //就证明 字典中已经记录了一个AB包相关信息了
            else
            {
                //如果字典中记录的信息是null 那就证明正在加载中
                //我们只需要等待它加载结束 就可以继续执行后面的代码了
                while (abDic[strs[i]] == null)
                {
                    //只要发现正在加载中 就不停的等待一帧 下一帧再进行判断
                    await UniTask.Yield();
                }
            }
            //增加依赖包的引用计数
            AddRefCount(strs[i]);
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isSync)
            {
                AssetBundle ab = LoadAssetBundleFromMultiplePaths(abName);
                abDic.Add(abName, ab);
            }
            else
            {
                //一开始异步加载 就记录 如果此时的记录中的值 是null 那证明这个ab包正在被异步加载
                abDic.Add(abName, null);
                AssetBundle bundle = await LoadAssetBundleFromMultiplePathsAsync(abName);
                //异步加载结束后 再替换之前的null  这时 不为null 就证明加载结束了
                abDic[abName] = bundle;
            }
        }
        else
        {
            //如果字典中记录的信息是null 那就证明正在加载中
            //我们只需要等待它加载结束 就可以继续执行后面的代码了
            while (abDic[abName] == null)
            {
                //只要发现正在加载中 就不停的等待一帧 下一帧再进行判断
                await UniTask.Yield();
            }
        }
        //增加目标包的引用计数
        AddRefCount(abName);
        // 更新AB包的使用时间
        UpdateABLastUsedTime(abName);

        if (isSync)
        {
            UnityEngine.Object res = abDic[abName].LoadAsset(resName, type);
            callBack(res);
        }
        else
        {
            //异步加载包中资源
            var abq = abDic[abName].LoadAssetAsync(resName, type);
            await abq;
            callBack(abq.asset);
        }
    }

    /// <summary>
    /// 名字 异步加载 指定资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public async void LoadResAsync(string abName, string resName, Action<UnityEngine.Object> callBack, bool isSync = false)
    {
        await ReallyLoadResAsync(abName, resName, callBack, isSync);
    }

    private async UniTask ReallyLoadResAsync(string abName, string resName, Action<UnityEngine.Object> callBack, bool isSync)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //还没有加载过该AB包
            if (!abDic.ContainsKey(strs[i]))
            {
                //同步加载
                if (isSync)
                {
                    AssetBundle ab = LoadAssetBundleFromMultiplePaths(strs[i]);
                    abDic.Add(strs[i], ab);
                }
                //异步加载
                else
                {
                    //一开始异步加载 就记录 如果此时的记录中的值 是null 那证明这个ab包正在被异步加载
                    abDic.Add(strs[i], null);
                    AssetBundle bundle = await LoadAssetBundleFromMultiplePathsAsync(strs[i]);
                    //异步加载结束后 再替换之前的null  这时 不为null 就证明加载结束了
                    abDic[strs[i]] = bundle;
                }
            }
            //就证明 字典中已经记录了一个AB包相关信息了
            else
            {
                //如果字典中记录的信息是null 那就证明正在加载中
                //我们只需要等待它加载结束 就可以继续执行后面的代码了
                while (abDic[strs[i]] == null)
                {
                    //只要发现正在加载中 就不停的等待一帧 下一帧再进行判断
                    await UniTask.Yield();
                }
            }
            //增加依赖包的引用计数
            AddRefCount(strs[i]);
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            //同步加载
            if (isSync)
            {
                AssetBundle ab = LoadAssetBundleFromMultiplePaths(abName);
                abDic.Add(abName, ab);
            }
            else
            {
                //一开始异步加载 就记录 如果此时的记录中的值 是null 那证明这个ab包正在被异步加载
                abDic.Add(abName, null);
                AssetBundle bundle = await LoadAssetBundleFromMultiplePathsAsync(abName);
                //异步加载结束后 再替换之前的null  这时 不为null 就证明加载结束了
                abDic[abName] = bundle;
            }
        }
        else
        {
            //如果字典中记录的信息是null 那就证明正在加载中
            //我们只需要等待它加载结束 就可以继续执行后面的代码了
            while (abDic[abName] == null)
            {
                //只要发现正在加载中 就不停的等待一帧 下一帧再进行判断
                await UniTask.Yield();
            }
        }
        //增加目标包的引用计数
        AddRefCount(abName);
        // 更新AB包的使用时间
        UpdateABLastUsedTime(abName);

        if (isSync)
        {
            UnityEngine.Object obj = abDic[abName].LoadAsset(resName);
            callBack(obj);
        }
        else
        {
            //异步加载包中资源
            var abq = abDic[abName].LoadAssetAsync(resName);
            await abq;
            callBack(abq.asset);
        }
    }

    //卸载AB包的方法
    public void UnLoadAB(string name, Action<bool> callBackResult)
    {
        if (abDic.ContainsKey(name))
        {
            if (abDic[name] == null)
            {
                //代表正在异步加载 没有卸载成功
                callBackResult(false);
                return;
            }
            //使用ReleaseRes方法来释放资源，会自动处理引用计数
            ReleaseRes(name);
            //卸载成功
            callBackResult(true);
        }
    }

    //清空AB包的方法
    public void ClearAB()
    {
        //由于AB包都是异步加载了 因此在清理之前 停止所有异步操作
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        abRefCount.Clear();
        //卸载主包
        mainAB = null;
    }
    
    /// <summary>
    /// 增加AB包的引用计数
    /// </summary>
    private void AddRefCount(string abName)
    {
        if (abRefCount.ContainsKey(abName))
        {
            abRefCount[abName]++;
        }
        else
        {
            abRefCount.Add(abName, 1);
        }
        LogSystem.Info($"增加引用计数: {abName} - {abRefCount[abName]}");
    }
    
    /// <summary>
    /// 减少AB包的引用计数
    /// </summary>
    private void SubRefCount(string abName)
    {
        if (abRefCount.ContainsKey(abName))
        {
            abRefCount[abName]--;
            LogSystem.Info($"减少引用计数: {abName} - {abRefCount[abName]}");
            if (abRefCount[abName] <= 0)
            {
                if (abDic.ContainsKey(abName) && abDic[abName] != null)
                {
                    abDic[abName].Unload(false);
                    abDic.Remove(abName);
                    abRefCount.Remove(abName);
                    LogSystem.Info($"引用计数为0，卸载AB包: {abName}");
                }
            }
        }
    }
    
    /// <summary>
    /// 释放资源，减少AB包的引用计数
    /// </summary>
    public void ReleaseRes(string abName)
    {
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            SubRefCount(strs[i]);
        }
        //减少目标包的引用计数
        SubRefCount(abName);
    }
    
    /// <summary>
    /// 获取AB包的引用计数
    /// </summary>
    public int GetRefCount(string abName)
    {
        return abRefCount.ContainsKey(abName) ? abRefCount[abName] : 0;
    }
    
    /// <summary>
    /// 自动检测并卸载无用的AB包（引用计数为0的包）
    /// </summary>
    public void AutoUnloadUnusedABs()
    {
        List<string> unusedABs = new List<string>();
        
        // 找出所有引用计数为0的AB包
        foreach (var kvp in abRefCount)
        {
            if (kvp.Value <= 0)
            {
                unusedABs.Add(kvp.Key);
            }
        }
        
        // 卸载这些AB包
        foreach (string abName in unusedABs)
        {
            if (abDic.ContainsKey(abName) && abDic[abName] != null)
            {
                abDic[abName].Unload(false);
                abDic.Remove(abName);
                abRefCount.Remove(abName);
                LogSystem.Info($"自动卸载无用AB包: {abName}");
            }
        }
        
        if (unusedABs.Count > 0)
        {
            LogSystem.Info($"共自动卸载 {unusedABs.Count} 个无用AB包");
        }
        else
        {
            LogSystem.Info("没有无用的AB包需要卸载");
        }
    }
    
    /// <summary>
    /// 启动自动检测和卸载无用AB包的定时任务
    /// </summary>
    /// <param name="intervalSeconds">检测间隔（秒）</param>
    public void StartAutoUnloadTask(float intervalSeconds = 30f)
    {
        _ = AutoUnloadTaskAsync(intervalSeconds);
    }
    
    /// <summary>
    /// 自动检测和卸载无用AB包的异步任务
    /// </summary>
    private async UniTask AutoUnloadTaskAsync(float intervalSeconds)
    {
        while (true)
        {
            await UniTask.Delay((int)(intervalSeconds * 1000));
            AutoUnloadUnusedABs();
        }
    }
    
    /// <summary>
    /// 手动触发一次无用AB包的检测和卸载
    /// </summary>
    public void TriggerUnloadCheck()
    {
        AutoUnloadUnusedABs();
    }
    
    // AB包使用记录
    private Dictionary<string, float> abLastUsedTime = new Dictionary<string, float>();
    
    // 闲置时间阈值（秒）
    private float idleTimeThreshold = 60f; // 60秒
    
    /// <summary>
    /// 更新AB包的使用时间
    /// </summary>
    /// <param name="abName">AB包名称</param>
    public void UpdateABLastUsedTime(string abName)
    {
        abLastUsedTime[abName] = Time.time;
    }
    
    /// <summary>
    /// 自动检测闲置AB包并减小引用计数
    /// 类似于GC算法，定期检查AB包的使用状态
    /// </summary>
    public void AutoReduceIdleABRefCount()
    {
        List<string> idleABs = new List<string>();
        float currentTime = Time.time;
        
        // 找出所有闲置的AB包
        foreach (var kvp in abRefCount)
        {
            string abName = kvp.Key;
            int refCount = kvp.Value;
            
            // 检查是否有使用记录
            if (abLastUsedTime.TryGetValue(abName, out float lastUsedTime))
            {
                // 计算闲置时间
                float idleTime = currentTime - lastUsedTime;
                
                // 如果闲置时间超过阈值且引用计数大于0，减小引用计数
                if (idleTime > idleTimeThreshold && refCount > 0)
                {
                    idleABs.Add(abName);
                }
            }
            else
            {
                // 没有使用记录，默认认为是闲置的
                if (refCount > 0)
                {
                    idleABs.Add(abName);
                }
            }
        }
        
        // 减小闲置AB包的引用计数
        foreach (string abName in idleABs)
        {
            SubRefCount(abName);
            LogSystem.Info($"自动减小闲置AB包引用计数: {abName}");
        }
        
        if (idleABs.Count > 0)
        {
            LogSystem.Info($"共减小 {idleABs.Count} 个闲置AB包的引用计数");
        }
        else
        {
            LogSystem.Info("没有闲置的AB包需要处理");
        }
    }
    
    /// <summary>
    /// 启动自动检测闲置AB包并减小引用计数的定时任务
    /// </summary>
    /// <param name="intervalSeconds">检测间隔（秒）</param>
    /// <param name="idleThreshold">闲置时间阈值（秒）</param>
    public void StartIdleABCheckTask(float intervalSeconds = 30f, float idleThreshold = 60f)
    {
        idleTimeThreshold = idleThreshold;
        _ = IdleABCheckTaskAsync(intervalSeconds);
    }
    
    /// <summary>
    /// 自动检测闲置AB包的异步任务
    /// </summary>
    private async UniTaskVoid IdleABCheckTaskAsync(float intervalSeconds)
    {
        while (true)
        {
            await UniTask.Delay((int)(intervalSeconds * 1000));
            AutoReduceIdleABRefCount();
        }
    }
    
    /// <summary>
    /// 手动触发一次闲置AB包的检测和引用计数减小
    /// </summary>
    public void TriggerIdleABCheck()
    {
        AutoReduceIdleABRefCount();
    }
    
    /// <summary>
    /// 设置闲置时间阈值
    /// </summary>
    /// <param name="seconds">闲置时间阈值（秒）</param>
    public void SetIdleTimeThreshold(float seconds)
    {
        idleTimeThreshold = seconds;
        LogSystem.Info($"闲置时间阈值设置为: {seconds} 秒");
    }
    
    /// <summary>
    /// 获取AB包的最后使用时间
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <returns>最后使用时间</returns>
    public float GetABLastUsedTime(string abName)
    {
        if (abLastUsedTime.TryGetValue(abName, out float lastUsedTime))
        {
            return lastUsedTime;
        }
        return -1f;
    }

    /// <summary>
    /// 预加载多个AB包（用于过场景时提前加载资源）
    /// 会自动加载所有依赖包
    /// </summary>
    /// <param name="abNames">需要预加载的AB包名称列表</param>
    /// <param name="progressCallback">进度回调，参数为当前进度(0-1)</param>
    /// <param name="completeCallback">完成回调，参数为是否全部加载成功</param>
    public async void PreloadABPackages(List<string> abNames, Action<float> progressCallback = null, Action<bool> completeCallback = null)
    {
        bool allSuccess = await PreloadABPackagesAsync(abNames, progressCallback);
        completeCallback?.Invoke(allSuccess);
    }

    /// <summary>
    /// 预加载多个AB包的异步实现
    /// </summary>
    /// <param name="abNames">需要预加载的AB包名称列表</param>
    /// <param name="progressCallback">进度回调</param>
    /// <returns>是否全部加载成功</returns>
    private async UniTask<bool> PreloadABPackagesAsync(List<string> abNames, Action<float> progressCallback)
    {
        if (abNames == null || abNames.Count == 0)
        {
            LogSystem.Warning("预加载列表为空，无需加载");
            progressCallback?.Invoke(1f);
            return true;
        }

        // 加载主包
        LoadMainAB();
        if (manifest == null)
        {
            LogSystem.Error("无法加载主包Manifest，预加载失败");
            return false;
        }

        // 收集所有需要加载的AB包（包括依赖包）
        HashSet<string> allPackagesToLoad = new HashSet<string>();
        foreach (string abName in abNames)
        {
            if (string.IsNullOrEmpty(abName))
                continue;

            // 添加目标包
            allPackagesToLoad.Add(abName);

            // 获取并添加所有依赖包
            string[] dependencies = manifest.GetAllDependencies(abName);
            foreach (string dep in dependencies)
            {
                allPackagesToLoad.Add(dep);
            }
        }

        LogSystem.Info($"开始预加载，共 {allPackagesToLoad.Count} 个AB包（含依赖）");

        // 转换为列表以便索引
        List<string> packageList = new List<string>(allPackagesToLoad);
        int totalCount = packageList.Count;
        int successCount = 0;
        int failCount = 0;

        // 创建加载任务列表
        List<UniTask<bool>> loadTasks = new List<UniTask<bool>>();

        for (int i = 0; i < packageList.Count; i++)
        {
            string packageName = packageList[i];
            int index = i; // 捕获索引用于进度计算

            // 创建加载任务
            UniTask<bool> task = PreloadSingleABPackage(packageName, () =>
            {
                // 单个包加载完成时的进度回调
                float progress = (float)(index + 1) / totalCount;
                progressCallback?.Invoke(progress);
            });

            loadTasks.Add(task);
        }

        // 等待所有加载任务完成
        bool[] results = await UniTask.WhenAll(loadTasks);

        // 统计结果
        foreach (bool result in results)
        {
            if (result)
                successCount++;
            else
                failCount++;
        }

        LogSystem.Info($"预加载完成：成功 {successCount} 个，失败 {failCount} 个");

        // 确保最终进度为1
        progressCallback?.Invoke(1f);

        return failCount == 0;
    }

    /// <summary>
    /// 预加载单个AB包
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <param name="onComplete">单个包加载完成回调</param>
    /// <returns>是否加载成功</returns>
    private async UniTask<bool> PreloadSingleABPackage(string abName, Action onComplete)
    {
        try
        {
            // 检查是否已经加载
            if (abDic.ContainsKey(abName) && abDic[abName] != null)
            {
                // 已加载，增加引用计数
                AddRefCount(abName);
                LogSystem.Info($"预加载：AB包 {abName} 已存在，增加引用计数");
                onComplete?.Invoke();
                return true;
            }

            // 检查是否正在加载中
            if (abDic.ContainsKey(abName) && abDic[abName] == null)
            {
                // 正在加载中，等待加载完成
                LogSystem.Info($"预加载：AB包 {abName} 正在加载中，等待完成");
                while (abDic[abName] == null)
                {
                    await UniTask.Yield();
                }
                AddRefCount(abName);
                onComplete?.Invoke();
                return true;
            }

            // 开始异步加载
            abDic.Add(abName, null); // 标记为加载中
            LogSystem.Info($"预加载：开始加载AB包 {abName}");

            AssetBundle bundle = await LoadAssetBundleFromMultiplePathsAsync(abName);

            if (bundle != null)
            {
                abDic[abName] = bundle;
                AddRefCount(abName);
                LogSystem.Info($"预加载：AB包 {abName} 加载成功");
                onComplete?.Invoke();
                return true;
            }
            else
            {
                // 加载失败，从字典中移除
                abDic.Remove(abName);
                LogSystem.Error($"预加载：AB包 {abName} 加载失败");
                onComplete?.Invoke();
                return false;
            }
        }
        catch (System.Exception e)
        {
            LogSystem.Error($"预加载AB包 {abName} 时发生异常: {e.Message}");
            if (abDic.ContainsKey(abName) && abDic[abName] == null)
            {
                abDic.Remove(abName);
            }
            onComplete?.Invoke();
            return false;
        }
    }

    /// <summary>
    /// 预加载AB包并加载其中的所有资源（用于需要立即使用所有资源的场景）
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <param name="progressCallback">进度回调</param>
    /// <param name="completeCallback">完成回调，返回加载的所有资源</param>
    public async void PreloadABPackageWithAllAssets(string abName, Action<float> progressCallback = null, Action<UnityEngine.Object[]> completeCallback = null)
    {
        UnityEngine.Object[] assets = await PreloadABPackageWithAllAssetsAsync(abName, progressCallback);
        completeCallback?.Invoke(assets);
    }

    /// <summary>
    /// 预加载AB包并加载其中所有资源的异步实现
    /// </summary>
    private async UniTask<UnityEngine.Object[]> PreloadABPackageWithAllAssetsAsync(string abName, Action<float> progressCallback)
    {
        // 先预加载AB包
        bool success = await PreloadSingleABPackage(abName, null);
        if (!success)
        {
            LogSystem.Error($"预加载AB包 {abName} 失败，无法加载资源");
            return null;
        }

        // 加载包中所有资源
        if (abDic.ContainsKey(abName) && abDic[abName] != null)
        {
            AssetBundle bundle = abDic[abName];
            AssetBundleRequest request = bundle.LoadAllAssetsAsync();
            
            // 监听进度
            while (!request.isDone)
            {
                progressCallback?.Invoke(request.progress);
                await UniTask.Yield();
            }

            progressCallback?.Invoke(1f);
            LogSystem.Info($"AB包 {abName} 中所有资源加载完成，共 {request.allAssets.Length} 个");
            return request.allAssets;
        }

        return null;
    }

    /// <summary>
    /// 取消预加载（释放预加载但未使用的AB包）
    /// </summary>
    /// <param name="abNames">需要取消预加载的AB包名称列表</param>
    public void CancelPreload(List<string> abNames)
    {
        if (abNames == null || abNames.Count == 0)
            return;

        foreach (string abName in abNames)
        {
            if (abRefCount.ContainsKey(abName) && abRefCount[abName] > 0)
            {
                // 减少引用计数，如果为0会自动卸载
                ReleaseRes(abName);
                LogSystem.Info($"取消预加载：释放AB包 {abName}");
            }
        }
    }

    /// <summary>
    /// 获取预加载状态
    /// </summary>
    /// <param name="abName">AB包名称</param>
    /// <returns>预加载状态</returns>
    public PreloadState GetPreloadState(string abName)
    {
        if (!abDic.ContainsKey(abName))
        {
            return PreloadState.NotLoaded;
        }

        if (abDic[abName] == null)
        {
            return PreloadState.Loading;
        }

        return PreloadState.Loaded;
    }

    /// <summary>
    /// AB包预加载状态枚举
    /// </summary>
    public enum PreloadState
    {
        NotLoaded,  // 未加载
        Loading,    // 加载中
        Loaded      // 已加载
    }

    
}

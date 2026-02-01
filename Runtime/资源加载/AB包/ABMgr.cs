using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

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
        }
    }

    /// <summary>
    /// 泛型资源同步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public T LoadRes<T>(string abName, string resName) where T : Object
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
    /// 泛型异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public async void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T : Object
    {
        await ReallyLoadResAsync<T>(abName, resName, callBack, isSync);
    }

    //正儿八经的 异步函数
    private async UniTask ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync) where T : Object
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
    /// Type异步加载资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public async void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isSync = false)
    {
        await ReallyLoadResAsync(abName, resName, type, callBack, isSync);
    }

    private async UniTask ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isSync)
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

        if (isSync)
        {
            Object res = abDic[abName].LoadAsset(resName, type);
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
    public async void LoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isSync = false)
    {
        await ReallyLoadResAsync(abName, resName, callBack, isSync);
    }

    private async UniTask ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isSync)
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

        if (isSync)
        {
            Object obj = abDic[abName].LoadAsset(resName);
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
    public void UnLoadAB(string name, UnityAction<bool> callBackResult)
    {
        if (abDic.ContainsKey(name))
        {
            if (abDic[name] == null)
            {
                //代表正在异步加载 没有卸载成功
                callBackResult(false);
                return;
            }
            abDic[name].Unload(false);
            abDic.Remove(name);
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
        //卸载主包
        mainAB = null;
    }
}

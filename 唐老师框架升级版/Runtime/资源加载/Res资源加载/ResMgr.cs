using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

/// <summary>
/// 资源信息基类 主要用于里式替换原则 父类容器装子类对象
/// </summary>
public abstract class ResInfoBase 
{
    //引用计数
    public int refCount;
}

/// <summary>
/// 资源信息对象 主要用于存储资源信息 异步加载委托信息
/// </summary>
/// <typeparam name="T">资源类型</typeparam>
public class ResInfo<T> : ResInfoBase
{
    //资源
    public T asset;
    //主要用于异步加载结束后 传递资源到外部的委托
    public Action<T> callBack;
    //决定引用计数为0时 是否真正需要移除
    public bool isDel;
    

    public void AddRefCount()
    {
        ++refCount;
    }

    public void SubRefCount()
    {
        --refCount;
        if (refCount < 0)
            UnityEngine.Debug.LogError("引用计数小于0了，请检查使用和卸载是否配对执行");
    }
}


/// <summary>
/// Resources 资源加载模块管理器
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //用于存储加载过的资源或者加载中的资源的容器
    private Dictionary<string, ResInfoBase> resDic = new Dictionary<string, ResInfoBase>();

    private ResMgr() { }

    /// <summary>
    /// 同步加载Resources下资源的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> info;
        //字典中不存在资源时
        if (!resDic.ContainsKey(resName))
        {
            //直接同步加载 并且记录资源信息 到字典中 方便下次直接取出来用
            T res = Resources.Load<T>(path);
            info = new ResInfo<T>();
            info.asset = res;
            //引用计数增加
            info.AddRefCount();
            resDic.Add(resName, info);
            return res;
        }
        else
        {
            //取出字典中的记录
            info = resDic[resName] as ResInfo<T>;
            //引用计数增加
            info.AddRefCount();
            //存在异步加载 还在加载中
                if (info.asset == null)
                {
                    //直接采用同步的方式加载成功
                    T res = Resources.Load<T>(path);
                    //记录 
                    info.asset = res;
                    //还应该把那些等待着异步加载结束的委托去执行了
                    info.callBack?.Invoke(res);
                    //回调结束 清除无用的引用
                    info.callBack = null;
                    // 并使用
                    return res;
                }
            else
            {
                //如果已经加载结束 直接用
                return info.asset;
            }
        }
    }

    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径（Resources下的）</param>
    /// <param name="callBack">加载结束后的回调函数 当异步加载资源结束后才会调用</param>
    public async void LoadAsync<T>(string path, Action<T> callBack) where T: UnityEngine.Object
    {
        //资源的唯一ID，是通过 路径名_资源类型 拼接而成的
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> info;
        if (!resDic.ContainsKey(resName))
        {
            //声明一个 资源信息对象
            info = new ResInfo<T>();
            //引用计数增加
            info.AddRefCount();
            //将资源记录添加到字典中（资源还没有加载成功）
            resDic.Add(resName, info);
            //记录传入的委托函数 一会儿加载完成了 再使用
            info.callBack += callBack;
            //开启异步加载
            await ReallyLoadAsync<T>(path);
        }
        else
        {
            //从字典中取出资源信息
            info = resDic[resName] as ResInfo<T>;
            //引用计数增加
            info.AddRefCount();
            //如果资源还没有加载完 
            //意味着 还在进行异步加载
            if (info.asset == null)
                info.callBack += callBack;
            else
                callBack?.Invoke(info.asset);
        }
    }

    private async UniTask ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        //等待资源加载结束后 才会继续执行后面的代码
        await rq;

        string resName = path + "_" + typeof(T).Name;
        //资源加载结束 将资源传到外部的委托函数去进行使用
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //取出资源信息 并且记录加载完成的资源
            resInfo.asset = rq.asset as T;

            //如果发现需要删除 再去移除资源
            //引用计数为0 才真正去移除
            if (resInfo.refCount == 0)
                UnloadAsset<T>(path, resInfo.isDel, null, false);
            else
            {
                //将加载完成的资源传递出去
                resInfo.callBack?.Invoke(resInfo.asset);
                //加载完毕后 这些引用就可以清空 避免引用的占用 可能带来的潜在的内存泄漏问题
                resInfo.callBack = null;
            }
        }
    }

    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <param name="path">资源路径（Resources下的）</param>
    /// <param name="callBack">加载结束后的回调函数 当异步加载资源结束后才会调用</param>
    [Obsolete("注意：建议使用泛型加载方式，如果实在要用Type加载，一定不能和泛型加载混用去加载同类型同名资源")]
    public async void LoadAsync(string path, Type type, Action<UnityEngine.Object> callBack) 
    {
        //资源的唯一ID，是通过 路径名_资源类型 拼接而成的
        string resName = path + "_" + type.Name;
        ResInfo<UnityEngine.Object> info;
        if (!resDic.ContainsKey(resName))
        {
            //声明一个 资源信息对象
            info = new ResInfo<UnityEngine.Object>();
            //引用计数增加
            info.AddRefCount();
            //将资源记录添加到字典中（资源还没有加载成功）
            resDic.Add(resName, info);
            //记录传入的委托函数 一会儿加载完成了 再使用
            info.callBack += callBack;
            //开启异步加载
            await ReallyLoadAsync(path, type);
        }
        else
        {
            //从字典中取出资源信息
            info = resDic[resName] as ResInfo<UnityEngine.Object>;
            //引用计数增加
            info.AddRefCount();
            //如果资源还没有加载完 
            //意味着 还在进行异步加载
            if (info.asset == null)
                info.callBack += callBack;
            else
                callBack?.Invoke(info.asset);
        }
    }

    private async UniTask ReallyLoadAsync(string path, Type type)
    {
        //异步加载资源
        ResourceRequest rq = Resources.LoadAsync(path, type);
        //等待资源加载结束后 才会继续执行后面的代码
        await rq;

        string resName = path + "_" + type.Name;
        //资源加载结束 将资源传到外部的委托函数去进行使用
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //取出资源信息 并且记录加载完成的资源
            resInfo.asset = rq.asset;
            //如果发现需要删除 再去移除资源
            //引用计数为0 才真正去移除
            if (resInfo.refCount == 0)
                UnloadAsset(path, type, resInfo.isDel, null, false);
            else
            {
                //将加载完成的资源传递出去
                resInfo.callBack?.Invoke(resInfo.asset);
                //加载完毕后 这些引用就可以清空 避免引用的占用 可能带来的潜在的内存泄漏问题
                resInfo.callBack = null;
            }
        }
    }

    /// <summary>
    /// 指定卸载一个资源
    /// </summary>
    /// <param name="assetToUnload"></param>
    public void UnloadAsset<T>(string path, bool isDel = false, Action<T> callBack = null, bool isSub = true)
    {
        string resName = path + "_" + typeof(T).Name;
        //判断是否存在对应资源
        if(resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            //引用计数-1
            if(isSub)
                resInfo.SubRefCount();
            //记录 引用计数为0时  是否马上移除标签
            resInfo.isDel = isDel;
            //资源已经加载结束 
            if(resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel)
            {
                //从字典移除
                resDic.Remove(resName);
                //通过api 卸载资源
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            else if(resInfo.asset == null)//资源正在异步加载中
            {
                //MonoMgr.Instance.StopCoroutine(resInfo.coroutine);
                //resDic.Remove(resName);
                //为了保险起见 一定要让资源移除了
                //改变表示 待删除
                //resInfo.isDel = true;
                //当异步加载不想使用时 我们应该移除它的回调记录 而不是直接去卸载资源
                if (callBack != null)
                    resInfo.callBack -= callBack;

            }
        }
    }

    public void UnloadAsset(string path, Type type, bool isDel = false, Action<UnityEngine.Object> callBack = null, bool isSub = true)
    {
        string resName = path + "_" + type.Name;
        //判断是否存在对应资源
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            //引用计数-1
            if(isSub)
                resInfo.SubRefCount();
            //记录 引用计数为0时  是否马上移除标签
            resInfo.isDel = isDel;
            //资源已经加载结束 
            if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel)
            {
                //从字典移除
                resDic.Remove(resName);
                //通过api 卸载资源
                Resources.UnloadAsset(resInfo.asset);
            }
            else if (resInfo.asset == null)//资源正在异步加载中
            {
                //MonoMgr.Instance.StopCoroutine(resInfo.coroutine);
                //resDic.Remove(resName);
                //为了保险起见 一定要让资源移除了
                //改变表示 待删除
                //resInfo.isDel = true;
                //当异步加载不想使用时 我们应该移除它的回调记录 而不是直接去卸载资源
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }
    }

    /// <summary>
    /// 异步卸载对应没有使用的Resources相关的资源
    /// </summary>
    /// <param name="callBack">回调函数</param>
    public async void UnloadUnusedAssets(Action callBack)
    {
        await ReallyUnloadUnusedAssets(callBack);
    }

    private async UniTask ReallyUnloadUnusedAssets(Action callBack)
    {
        //就是在真正移除不使用的资源之前 应该把我们自己记录的那些引用计数为0 并且没有被移除记录的资源
        //移除掉
        List<string> list = new List<string>();
        foreach (string path in resDic.Keys)
        {
            if (resDic[path].refCount == 0)
                list.Add(path);
        }
        foreach (string path in list)
        {
            resDic.Remove(path);
        }

        AsyncOperation ao = Resources.UnloadUnusedAssets();
        await ao;
        //卸载完毕后 通知外部
        callBack();
    }

    /// <summary>
    /// 获取当前某个资源的引用计数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetRefCount<T>(string path)
    {
        string resName = path + "_" + typeof(T).Name;
        if(resDic.ContainsKey(resName))
        {
            return (resDic[resName] as ResInfo<T>).refCount;
        }
        return 0;
    }


    /// <summary>
    /// 清空字典
    /// </summary>
    /// <param name="callBack"></param>
    public async void ClearDic(Action callBack)
    {
        await ReallyClearDic(callBack);
    }

    private async UniTask ReallyClearDic(Action callBack)
    {
        resDic.Clear();
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        await ao;
        //卸载完毕后 通知外部
        callBack();
    }
}

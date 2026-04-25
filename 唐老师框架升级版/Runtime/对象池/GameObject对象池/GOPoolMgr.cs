using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Xml.Linq;

/// <summary>
/// 存储池子里的对象数据，管理
/// </summary>
public class PoolData
{
    //栈存储池子里的对象 记录还没有使用的对象
    private Stack<GameObject> dataStack = new Stack<GameObject>();

    //列表记录使用中的对象
    private List<GameObject> usedList = new List<GameObject>();

    //最大数量 同时存在的对象数量上限
    private int maxNum=20;

    //根节点对象 管理布局中的对象
    private GameObject rootObj;

    //获取池子是否有对象
    public int Count => dataStack.Count;

    public int UsedCount => usedList.Count;

    /// <summary>
    /// 当前使用数量和最大数量比较 小于返回true 需要实例化
    /// </summary>
    public bool NeedCreate => usedList.Count < maxNum;

    /// <summary>
    /// 初始化构造函数
    /// </summary>
    /// <param name="root">根节点，池子父，用于布局</param>
    /// <param name="name">池子容器的名字</param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        //初始化时 开启动态布局 建立父子关系
        if (GOPoolMgr.isOpenLayout)
        {
            //创建池子容器
            rootObj = new GameObject(name);
            //将池子容器添加到根节点下建立父子关系
            rootObj.transform.SetParent(root.transform);
        }

        //初始化时 外部传入的是动态创建一个的对象
        //所以应该添加到列表中 记录使用中的对象列表
        PushUsedList(usedObj);

        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            //UnityEngine.Debug.LogError("因为使用者传入的不是带有PoolObj脚本的预制体 请检查后再次使用");
            maxNum = 20;
            return;
        }
        //记录最大数量值
        maxNum = poolObj.maxNum;
    }

    /// <summary>
    /// 从池子里取对象数据
    /// </summary>
    /// <returns>需要的对象数据</returns>
    public GameObject Pop()
    {
        //取对象
        GameObject obj;

        if (Count > 0)
        {
            //从池子里的对象取来使用
            obj = dataStack.Pop();
            //既然要使用了 就应该加到使用中的对象列表
            usedList.Add(obj);
        }
        else
        {
            //取0号对象 就是最早使用时创建的对象
            obj = usedList[0];
            //先将最早使用到的对象从列表中移除
            usedList.RemoveAt(0);
            //然后再将要取出来使用的对象 应该再次添加到使用中的对象列表中
            //并把它加到最后面 表示 比较新的开始
            usedList.Add(obj);
        }

        //激活对象
        obj.SetActive(true);
        //解除父子关系
        if (GOPoolMgr.isOpenLayout)
            obj.transform.SetParent(null);

        return obj;
    }

    /// <summary>
    /// 将对象压回到池子
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj,Action<GameObject> callBack=null)
    {
        callBack?.Invoke(obj);
        //失活对象
        obj.SetActive(false);
        //添加到对应的池子容器 建立父子关系
        if (GOPoolMgr.isOpenLayout)
            obj.transform.SetParent(rootObj.transform);
        //通过栈来记录对应的对象数据
        dataStack.Push(obj);
        //对象已经不再使用了 应该从使用列表中移除
        usedList.Remove(obj);
    }


    /// <summary>
    /// 将对象压到使用中的对象列表
    /// </summary>
    /// <param name="obj"></param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

/// <summary>
/// 池对象基类，作为数据结构和方法类的基类
/// </summary>
public abstract class PoolObjectBase { }

/// <summary>
/// 用于存储数据结构类和方法类，不需要继承mono模块
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolObject<T> : PoolObjectBase where T : class
{
    public Queue<T> poolObjs = new Queue<T>();
}

/// <summary>
/// 需要复用的数据结构类、方法类，都需要继承此接口
/// </summary>
public interface IPoolObject
{
    /// <summary>
    /// 重置数据的方法
    /// </summary>
    void ResetInfo();
}

/// <summary>
/// 游戏对象(GameObject)池管理器
/// </summary>
public class GOPoolMgr : BaseManager<GOPoolMgr>
{
    //存储对象池数据的字典
    //键是对象名称，值是对象池数据
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    /// <summary>
    /// 用于存储数据结构类、方法类等非GameObject的对象池
    /// </summary>
    private Dictionary<string, PoolObjectBase> poolObjectDic = new Dictionary<string, PoolObjectBase>();

    //池根对象
    private GameObject poolObj;

    //是否开启自动布局
    public static bool isOpenLayout = false;

    private GOPoolMgr()
    {

        //如果池根对象为空且开启了布局，则创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

    }

    /// <summary>
    /// 获取对象的方法
    /// </summary>
    /// <param name="name">对象的资源名称</param>
    /// <returns>从对象池获取的对象</returns>
    public GameObject GetObj(string name)
    {
        //如果池根对象为空且开启了布局，则创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 对象池数据判断逻辑
        if (!poolDic.ContainsKey(name) ||
            (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            //动态创建对象
            //没有对象时，通过资源加载来实例化一个GameObject
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //obj = ABMgr.Instance.LoadRes<GameObject>("object", name);

            //修改实例化物体的名字，默认会带(Clone)
            //这样做的目的是方便管理
            obj.name = name;

            //创建对象池
            if (!poolDic.ContainsKey(name))
                poolDic.Add(name, new PoolData(poolObj, name, obj));
            else//实例化物体的名字，需要记录到使用中的物体列表
                poolDic[name].PushUsedList(obj);
        }
        //对象池存在，且使用中的物体数量达到上限，直接从池中取对象
        else
        {
            obj = poolDic[name].Pop();
        }

        #endregion


        #region 没有对象池时的逻辑
        ////字典里有这个池子 并且 栈里有对象 直接去取对象
        //if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
        //{
        //    //从栈中取对象，直接返回给外部使用
        //    obj = poolDic[name].Pop();
        //}
        ////否则，需要去创建
        //else
        //{
        //    //没有对象时，通过资源加载来实例化一个GameObject
        //    obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
        //    //修改实例化物体的名字，默认会带(Clone)
        //    //这样做的目的是方便管理
        //    obj.name = name;
        //}
        #endregion
        return obj;
    }

    public GameObject GetObj(string abName, string resName)
    {
        //如果池根对象为空且开启了布局，则创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 对象池数据判断逻辑
        if (!poolDic.ContainsKey(abName + resName) ||
            (poolDic[abName + resName].Count == 0 && poolDic[abName + resName].NeedCreate))
        {
            //动态创建对象
            //没有对象时，通过资源加载来实例化一个GameObject
            //obj = GameObject.Instantiate(Resources.Load<GameObject>(abName+resName));
            obj = ABMgr.Instance.LoadRes<GameObject>(abName, resName);

            //修改实例化物体的名字，默认会带(Clone)
            //这样做的目的是方便管理
            obj.name = abName + resName;

            //创建对象池
            if (!poolDic.ContainsKey(abName + resName))
                poolDic.Add(abName + resName, new PoolData(poolObj, abName + resName, obj));
            else//实例化物体的名字，需要记录到使用中的物体列表
                poolDic[abName + resName].PushUsedList(obj);
        }
        //对象池存在，且使用中的物体数量达到上限，直接从池中取对象
        else
        {
            obj = poolDic[abName + resName].Pop();
        }

        #endregion


        return obj;
    }

    public void GetObj(string abName, string resName,Action<GameObject> callBack)
    {
        //如果池根对象为空且开启了布局，则创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 对象池数据判断逻辑
        if (!poolDic.ContainsKey(abName + resName) ||
            (poolDic[abName + resName].Count == 0 && poolDic[abName + resName].NeedCreate))
        {
            //动态创建对象
            //没有对象时，通过资源加载来实例化一个GameObject
            //obj = GameObject.Instantiate(Resources.Load<GameObject>(abName+resName));
            //obj = GameObject.Instantiate(ABResMgr.Instance.LoadRes<GameObject>(abName, resName));

            ABResMgr.Instance.LoadResAsync<GameObject>(abName, resName, (abObj) =>
            {
                if (abObj != null)
                {
                    // 实例化预制体，保存实例的引用
                    obj = GameObject.Instantiate(abObj);
                    // 修改实例化物体的名字 默认会带(Clone)
                    // 这样做的目的 是方便管理
                    obj.name = abName + resName;
                    // 确保实例化的对象处于激活状态
                    obj.SetActive(true);

                    // 创建对象池数据
                    if (!poolDic.ContainsKey(abName + resName))
                        poolDic.Add(abName + resName, new PoolData(poolObj, abName + resName, obj));
                    else// 实例化物体的名字 不需要记录到使用中的物体列表
                        poolDic[abName + resName].PushUsedList(obj);

                    callBack?.Invoke(obj);
                }
                else
                {
                    Debug.LogError($"Failed to load asset: {resName} from bundle: {abName}");
                }
            });                        
        }
        //对象池存在，且使用中的物体数量达到上限，直接从池中取对象
        else
        {
            obj = poolDic[abName + resName].Pop();
            // 确保从池中取出的对象处于激活状态
            if (obj != null)
            {
                obj.SetActive(true);
            }
            callBack?.Invoke(obj);
        }

        #endregion

        

    }

    /// <summary>
    /// 获取对象的方法
    /// </summary>
    /// <param name="gameObject">游戏对象预制体</param>
    /// <returns>从对象池获取的对象</returns>
    public GameObject GetObj(GameObject gameObject)
    {
        //如果池根对象为空且开启了布局，则创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 对象池数据判断逻辑
        if (!poolDic.ContainsKey(gameObject.name) ||
            (poolDic[gameObject.name].Count == 0 && poolDic[gameObject.name].NeedCreate))
        {
            //动态创建对象
            //没有对象时，通过资源加载来实例化一个GameObject
            obj = GameObject.Instantiate(gameObject);
            //obj = ABMgr.Instance.LoadRes<GameObject>("object", name);

            //修改实例化物体的名字，默认会带(Clone)
            //这样做的目的是方便管理
            obj.name = gameObject.name;

            //创建对象池
            if (!poolDic.ContainsKey(gameObject.name))
                poolDic.Add(gameObject.name, new PoolData(poolObj, gameObject.name, obj));
            else//实例化物体的名字，需要记录到使用中的物体列表
                poolDic[gameObject.name].PushUsedList(obj);
        }
        //对象池存在，且使用中的物体数量达到上限，直接从池中取对象
        else
        {
            obj = poolDic[gameObject.name].Pop();
        }

        #endregion

        return obj;
    }

    

    /// <summary>
    /// 获取自定义的数据结构类、方法类等，不需要继承Mono模块
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns></returns>
    public T GetObj<T>(string nameSpace = "") where T : class, IPoolObject, new()
    {
        //生成池名称，格式为命名空间_类型名
        string poolName = nameSpace + "_" + typeof(T).Name;
        //查找对象池
        if (poolObjectDic.ContainsKey(poolName))
        {
            PoolObject<T> pool = poolObjectDic[poolName] as PoolObject<T>;
            //检查对象池是否有可复用的对象
            if (pool.poolObjs.Count > 0)
            {
                //从队列中取出对象并返回
                T obj = pool.poolObjs.Dequeue() as T;
                return obj;
            }
            //对象池为空
            else
            {
                //确保T有无参构造函数
                T obj = new T();
                return obj;
            }
        }
        else//未找到对象池
        {
            T obj = new T();
            return obj;
        }

    }

    /// <summary>
    /// 将对象压回到对象池
    /// </summary>
    /// <param name="obj">要压回的对象</param>
    public void PushObj(GameObject obj,Action<GameObject> callBack=null)
    {
        if(obj == null)
            return;
        
        //压入到对应池子
        poolDic[obj.name].Push(obj,callBack);
 
    }

    /// <summary>
    /// 将自定义的数据结构类、方法类等对象压回对象池
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public void PushObj<T>(T obj, string nameSpace = "") where T : class, IPoolObject
    {
        //如果要压入null对象，是不允许的
        if (obj == null)
            return;
        //生成池名称，格式为命名空间_类型名
        string poolName = nameSpace + "_" + typeof(T).Name;
        //查找对象池
        PoolObject<T> pool;
        if (poolObjectDic.ContainsKey(poolName))
            //获取对象池并压入对象
            pool = poolObjectDic[poolName] as PoolObject<T>;
        else//未找到对象池
        {
            pool = new PoolObject<T>();
            poolObjectDic.Add(poolName, pool);
        }
        //在压回池子之前 调用重置方法
        obj.ResetInfo();
        pool.poolObjs.Enqueue(obj);
    }

    /// <summary>
    /// 清空所有对象池中的对象
    /// 使用场景：需要完全重置时
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
        poolObjectDic.Clear();
    }
}
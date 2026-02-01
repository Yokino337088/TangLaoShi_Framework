using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Xml.Linq;

/// <summary>
/// 抽屉（池子中的数据）对象
/// </summary>
public class PoolData
{
    //用来存储抽屉中的对象 记录的是没有使用的对象
    private Stack<GameObject> dataStack = new Stack<GameObject>();

    //用来记录使用中的对象的 
    private List<GameObject> usedList = new List<GameObject>();

    //抽屉上限 场景上同时存在的对象的上限个数
    private int maxNum=20;

    //抽屉根对象 用来进行布局管理的对象
    private GameObject rootObj;

    //获取容器中是否有对象
    public int Count => dataStack.Count;

    public int UsedCount => usedList.Count;

    /// <summary>
    /// 进行使用中对象数量和最大容量进行比较 小于返回true 需要实例化
    /// </summary>
    public bool NeedCreate => usedList.Count < maxNum;

    /// <summary>
    /// 初始化构造函数
    /// </summary>
    /// <param name="root">柜子（缓存池）父对象</param>
    /// <param name="name">抽屉父对象的名字</param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        //开启功能时 才会动态创建 建立父子关系
        if (GOPoolMgr.isOpenLayout)
        {
            //创建抽屉父对象
            rootObj = new GameObject(name);
            //和柜子父对象建立父子关系
            rootObj.transform.SetParent(root.transform);
        }

        //创建抽屉时 外部肯定是会动态创建一个对象的
        //我们应该将其记录到 使用中的对象容器中
        PushUsedList(usedObj);

        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            //UnityEngine.Debug.LogError("请为使用缓存池功能的预设体对象挂载PoolObj脚本 用于设置数量上限");
            maxNum = 20;
            return;
        }
        //记录上限数量值
        maxNum = poolObj.maxNum;
    }

    /// <summary>
    /// 从抽屉中弹出数据对象
    /// </summary>
    /// <returns>想要的对象数据</returns>
    public GameObject Pop()
    {
        //取出对象
        GameObject obj;

        if (Count > 0)
        {
            //从没有的容器当中取出使用
            obj = dataStack.Pop();
            //现在要使用了 应该要用使用中的容器记录它
            usedList.Add(obj);
        }
        else
        {
            //取0索引的对象 代表的就是使用时间最长的对象
            obj = usedList[0];
            //并且把它从使用着的对象中移除
            usedList.RemoveAt(0);
            //由于它还要拿出去用，所以我们应该把它又记录到 使用中的容器中去 
            //并且添加到尾部 表示 比较新的开始
            usedList.Add(obj);
        }

        //激活对象
        obj.SetActive(true);
        //断开父子关系
        if (GOPoolMgr.isOpenLayout)
            obj.transform.SetParent(null);

        return obj;
    }

    /// <summary>
    /// 将物体放入到抽屉对象中
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj,UnityAction<GameObject> callBack=null)
    {
        callBack?.Invoke(obj);
        //失活放入抽屉的对象
        obj.SetActive(false);
        //放入对应抽屉的根物体中 建立父子关系
        if (GOPoolMgr.isOpenLayout)
            obj.transform.SetParent(rootObj.transform);
        //通过栈记录对应的对象数据
        dataStack.Push(obj);
        //这个对象已经不再使用了 应该把它从记录容器中移除
        usedList.Remove(obj);
    }


    /// <summary>
    /// 将对象压入到使用中的容器中记录
    /// </summary>
    /// <param name="obj"></param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

/// <summary>
/// 方便在字典当中用里式替换原则 存储子类对象
/// </summary>
public abstract class PoolObjectBase { }

/// <summary>
/// 用于存储 数据结构类 和 逻辑类 （不继承mono的）容器类
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolObject<T> : PoolObjectBase where T : class
{
    public Queue<T> poolObjs = new Queue<T>();
}

/// <summary>
/// 想要被复用的 数据结构类、逻辑类 都必须要继承该接口
/// </summary>
public interface IPoolObject
{
    /// <summary>
    /// 重置数据的方法
    /// </summary>
    void ResetInfo();
}

/// <summary>
/// 缓存池(对象池)模块 管理器
/// </summary>
public class GOPoolMgr : BaseManager<GOPoolMgr>
{
    //柜子容器当中有抽屉的体现
    //值 其实代表的就是一个 抽屉对象
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    /// <summary>
    /// 用于存储数据结构类、逻辑类对象的 池子的字典容器
    /// </summary>
    private Dictionary<string, PoolObjectBase> poolObjectDic = new Dictionary<string, PoolObjectBase>();

    //池子根对象
    private GameObject poolObj;

    //是否开启布局功能
    public static bool isOpenLayout = true;

    private GOPoolMgr()
    {

        //如果根物体为空 就创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

    }

    /// <summary>
    /// 拿东西的方法
    /// </summary>
    /// <param name="name">抽屉容器的名字</param>
    /// <returns>从缓存池中取出的对象</returns>
    public GameObject GetObj(string name)
    {
        //如果根物体为空 就创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 加入了数量上限后的逻辑判断
        if (!poolDic.ContainsKey(name) ||
            (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            //动态创建对象
            //没有的时候 通过资源加载 去实例化出一个GameObject
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //obj = ABMgr.Instance.LoadRes<GameObject>("object", name);

            //避免实例化出来的对象 默认会在名字后面加一个(Clone)
            //我们重命名过后 方便往里面放
            obj.name = name;

            //创建抽屉
            if (!poolDic.ContainsKey(name))
                poolDic.Add(name, new PoolData(poolObj, name, obj));
            else//实例化出来的对象 需要记录到使用中的对象容器中
                poolDic[name].PushUsedList(obj);
        }
        //当抽屉中有对象 或者 使用中的对象超上限了 直接去取出来用
        else
        {
            obj = poolDic[name].Pop();
        }

        #endregion


        #region 没有加入 上限时的逻辑
        ////有抽屉 并且 抽屉里 有对象 才去直接拿
        //if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
        //{
        //    //弹出栈中的对象 直接返回给外部使用
        //    obj = poolDic[name].Pop();
        //}
        ////否则，就应该去创造
        //else
        //{
        //    //没有的时候 通过资源加载 去实例化出一个GameObject
        //    obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
        //    //避免实例化出来的对象 默认会在名字后面加一个(Clone)
        //    //我们重命名过后 方便往里面放
        //    obj.name = name;
        //}
        #endregion
        return obj;
    }

    public GameObject GetObj(string abName, string resName)
    {
        //如果根物体为空 就创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 加入了数量上限后的逻辑判断
        if (!poolDic.ContainsKey(abName + resName) ||
            (poolDic[abName + resName].Count == 0 && poolDic[abName + resName].NeedCreate))
        {
            //动态创建对象
            //没有的时候 通过资源加载 去实例化出一个GameObject
            //obj = GameObject.Instantiate(Resources.Load<GameObject>(abName+resName));
            obj = ABMgr.Instance.LoadRes<GameObject>(abName, resName);

            //避免实例化出来的对象 默认会在名字后面加一个(Clone)
            //我们重命名过后 方便往里面放
            obj.name = abName + resName;

            //创建抽屉
            if (!poolDic.ContainsKey(abName + resName))
                poolDic.Add(abName + resName, new PoolData(poolObj, abName + resName, obj));
            else//实例化出来的对象 需要记录到使用中的对象容器中
                poolDic[abName + resName].PushUsedList(obj);
        }
        //当抽屉中有对象 或者 使用中的对象超上限了 直接去取出来用
        else
        {
            obj = poolDic[abName + resName].Pop();
        }

        #endregion


        return obj;
    }

    public void GetObj(string abName, string resName,Action<GameObject> callBack)
    {
        //如果根物体为空 就创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 加入了数量上限后的逻辑判断
        if (!poolDic.ContainsKey(abName + resName) ||
            (poolDic[abName + resName].Count == 0 && poolDic[abName + resName].NeedCreate))
        {
            //动态创建对象
            //没有的时候 通过资源加载 去实例化出一个GameObject
            //obj = GameObject.Instantiate(Resources.Load<GameObject>(abName+resName));
            obj = GameObject.Instantiate(ABResMgr.Instance.LoadRes<GameObject>(abName, resName));

            //避免实例化出来的对象 默认会在名字后面加一个(Clone)
            //我们重命名过后 方便往里面放
            obj.name = abName + resName;

            //创建抽屉
            if (!poolDic.ContainsKey(abName + resName))
                poolDic.Add(abName + resName, new PoolData(poolObj, abName + resName, obj));
            else//实例化出来的对象 需要记录到使用中的对象容器中
                poolDic[abName + resName].PushUsedList(obj);
        }
        //当抽屉中有对象 或者 使用中的对象超上限了 直接去取出来用
        else
        {
            obj = poolDic[abName + resName].Pop();
        }

        #endregion

        callBack?.Invoke(obj);

    }

    /// <summary>
    /// 拿东西的方法
    /// </summary>
    /// <param name="name">抽屉容器的名字</param>
    /// <returns>从缓存池中取出的对象</returns>
    public GameObject GetObj(GameObject gameObject)
    {
        //如果根物体为空 就创建
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 加入了数量上限后的逻辑判断
        if (!poolDic.ContainsKey(gameObject.name) ||
            (poolDic[gameObject.name].Count == 0 && poolDic[gameObject.name].NeedCreate))
        {
            //动态创建对象
            //没有的时候 通过资源加载 去实例化出一个GameObject
            obj = GameObject.Instantiate(gameObject);
            //obj = ABMgr.Instance.LoadRes<GameObject>("object", name);

            //避免实例化出来的对象 默认会在名字后面加一个(Clone)
            //我们重命名过后 方便往里面放
            obj.name = gameObject.name;

            //创建抽屉
            if (!poolDic.ContainsKey(gameObject.name))
                poolDic.Add(gameObject.name, new PoolData(poolObj, gameObject.name, obj));
            else//实例化出来的对象 需要记录到使用中的对象容器中
                poolDic[gameObject.name].PushUsedList(obj);
        }
        //当抽屉中有对象 或者 使用中的对象超上限了 直接去取出来用
        else
        {
            obj = poolDic[gameObject.name].Pop();
        }

        #endregion

        return obj;
    }

    //public void GetObjByAA(string resName,Action<GameObject> callBack=null)
    //{
    //    //如果根物体为空 就创建
    //    if (poolObj == null && isOpenLayout)
    //        poolObj = new GameObject("Pool");

    //    GameObject obj;

    //    #region 加入了数量上限后的逻辑判断
    //    if (!poolDic.ContainsKey(resName) ||
    //        (poolDic[resName].Count == 0 && poolDic[resName].NeedCreate))
    //    {                        
    //        AddressablesMgr.Instance.LoadAssetAsync<GameObject>(resName, (handle) =>
    //        {
    //            obj = GameObject.Instantiate(handle.Result);
    //            //避免实例化出来的对象 默认会在名字后面加一个(Clone)
    //            //我们重命名过后 方便往里面放
    //            obj.name = resName;
    //            //创建抽屉
    //            if (!poolDic.ContainsKey(resName))
    //                poolDic.Add(resName, new PoolData(poolObj, resName, obj));
    //            else//实例化出来的对象 需要记录到使用中的对象容器中
    //                poolDic[resName].PushUsedList(obj);

    //            callBack?.Invoke(obj);
    //        });
    //    }
    //    //当抽屉中有对象 或者 使用中的对象超上限了 直接去取出来用
    //    else
    //    {
    //        obj = poolDic[resName].Pop();
    //        callBack?.Invoke(obj);
    //    }

    //    #endregion

    //}

    /// <summary>
    /// 获取自定义的数据结构类和逻辑类对象 （不继承Mono的）
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns></returns>
    public T GetObj<T>(string nameSpace = "") where T : class, IPoolObject, new()
    {
        //池子的名字 是根据类的类型来决定的 就是它的类名
        string poolName = nameSpace + "_" + typeof(T).Name;
        //有池子
        if (poolObjectDic.ContainsKey(poolName))
        {
            PoolObject<T> pool = poolObjectDic[poolName] as PoolObject<T>;
            //池子当中是否有可以复用的内容
            if (pool.poolObjs.Count > 0)
            {
                //从队列中取出对象 进行复用
                T obj = pool.poolObjs.Dequeue() as T;
                return obj;
            }
            //池子当中是空的
            else
            {
                //必须保证存在无参构造函数
                T obj = new T();
                return obj;
            }
        }
        else//没有池子
        {
            T obj = new T();
            return obj;
        }

    }

    /// <summary>
    /// 往缓存池中放入对象
    /// </summary>
    /// <param name="name">抽屉（对象）的名字</param>
    /// <param name="obj">希望放入的对象</param>
    public void PushObj(GameObject obj,UnityAction<GameObject> callBack=null)
    {
        #region 因为失活 父子关系都放入了 抽屉对象中处理 所以不需要再处理这些内容了
        ////总之，目的就是要把对象隐藏起来
        ////并不是直接移除对象 而是将对象失活 一会儿再用 用的时候再激活它
        ////除了这种方式，还可以把对象放倒屏幕外看不见的地方
        //obj.SetActive(false);

        ////把失活的对象（要放入抽屉中的对象） 父对象先设置为 柜子（缓存池）根对象
        //obj.transform.SetParent(poolObj.transform);
        #endregion

        //没有抽屉 创建抽屉
        //if (!poolDic.ContainsKey(obj.name))
        //    poolDic.Add(obj.name, new PoolData(poolObj, obj.name));

        //往抽屉当中放对象
        poolDic[obj.name].Push(obj,callBack);

        ////如果存在对应的抽屉容器 直接放
        //if(poolDic.ContainsKey(name))
        //{
        //    //往栈（抽屉）中放入对象
        //    poolDic[name].Push(obj);
        //}
        ////否则 需要先创建抽屉 再放
        //else
        //{
        //    //先创建抽屉
        //    poolDic.Add(name, new Stack<GameObject>());
        //    //再往抽屉里面放
        //    poolDic[name].Push(obj);
        //}
    }

    /// <summary>
    /// 将自定义数据结构类和逻辑类 放入池子中
    /// </summary>
    /// <typeparam name="T">对应类型</typeparam>
    public void PushObj<T>(T obj, string nameSpace = "") where T : class, IPoolObject
    {
        //如果想要压入null对象 是不被允许的
        if (obj == null)
            return;
        //池子的名字 是根据类的类型来决定的 就是它的类名
        string poolName = nameSpace + "_" + typeof(T).Name;
        //有池子
        PoolObject<T> pool;
        if (poolObjectDic.ContainsKey(poolName))
            //取出池子 压入对象
            pool = poolObjectDic[poolName] as PoolObject<T>;
        else//没有池子
        {
            pool = new PoolObject<T>();
            poolObjectDic.Add(poolName, pool);
        }
        //在放入池子中之前 先重置对象的数据
        obj.ResetInfo();
        pool.poolObjs.Enqueue(obj);
    }

    /// <summary>
    /// 用于清除整个柜子当中的数据 
    /// 使用场景 主要是 切场景时
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
        poolObjectDic.Clear();
    }
}

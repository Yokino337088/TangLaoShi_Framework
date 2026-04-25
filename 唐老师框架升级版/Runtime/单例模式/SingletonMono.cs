using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载式 继承Mono的单例模式基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T>: MonoBehaviour where T:MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        //已经存在一个对应的单例模式对象了 不需要在有一个了
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this as T;
        //我们挂载继承该单例模式基类的脚本后 依附的对象过场景时就不会被移除了
        //就可以保证在游戏的整个生命周期中都存在 
        DontDestroyOnLoad(this.gameObject);
    }
}

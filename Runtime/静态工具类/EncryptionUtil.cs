using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 加密工具类 主要提供加密需求
/// </summary>
public class EncryptionUtil
{
    //1.获取随机密钥
    public static int GetRandomKey()
    {
        return Random.Range(1, 10000) + 5;
    }

    //2.加密数据
    public static int LockValue(int value, int key)
    {
        //主要采用异或加密
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        value += key;
        return value;
    }

    public static long LockValue(long value, int key)
    {
        //主要采用异或加密
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        value += key;
        return value;
    }

    //3.解密数据
    public static int UnLoackValue(int value, int key)
    {
        //有可能还没有加密过 没有初始化过的数据 直接想要获取 那么就不用解密了
        //这种时候数值肯定是0
        if (value == 0)
            return value;
        value -= key;
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        return value;
    }

    public static long UnLoackValue(long value, int key)
    {
        //有可能还没有加密过 没有初始化过的数据 直接想要获取 那么就不用解密了
        //这种时候数值肯定是0
        if (value == 0)
            return value;
        value -= key;
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        return value;
    }
}

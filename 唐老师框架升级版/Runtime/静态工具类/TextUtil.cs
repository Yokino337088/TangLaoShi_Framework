using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用于处理字符串的一些公共功能的
/// </summary>
public class TextUtil
{
    private static StringBuilder resultStr = new StringBuilder("");

    #region 字符串拆分相关
    /// <summary>
    /// 拆分字符串 返回字符串数组
    /// </summary>
    /// <param name="str">想要被拆分的字符串</param>
    /// <param name="type">拆分字符类型： 1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
    /// <returns></returns>
    public static string[] SplitStr(string str, int type = 1)
    {
        if (str == "")
            return new string[0];
        string newStr = str;
        if (type == 1)
        {
            //为了避免英文符号填成了中文符号 我们先进行一个替换
            while (newStr.IndexOf("；") != -1)
                newStr = newStr.Replace("；", ";");
            return newStr.Split(';');
        }
        else if (type == 2)
        {
            //为了避免英文符号填成了中文符号 我们先进行一个替换
            while (newStr.IndexOf("，") != -1)
                newStr = newStr.Replace("，", ",");
            return newStr.Split(',');
        }
        else if (type == 3)
        {
            return newStr.Split('%');
        }
        else if (type == 4)
        {
            //为了避免英文符号填成了中文符号 我们先进行一个替换
            while (newStr.IndexOf("：") != -1)
                newStr = newStr.Replace("：", ":");
            return newStr.Split(':');
        }
        else if (type == 5)
        {
            return newStr.Split(' ');
        }
        else if (type == 6)
        {
            return newStr.Split('|');
        }
        else if (type == 7)
        {
            return newStr.Split('_');
        }

        return new string[0];
    }

    /// <summary>
    /// 拆分字符串 返回整形数组
    /// </summary>
    /// <param name="str">想要被拆分的字符串</param>
    /// <param name="type">拆分字符类型： 1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
    /// <returns></returns>
    public static int[] SplitStrToIntArr(string str, int type = 1)
    {
        //得到拆分后的字符串数组
        string[] strs = SplitStr(str, type);
        if (strs.Length == 0)
            return new int[0];
        //把字符串数组 转换成 int数组 
        return Array.ConvertAll<string, int>(strs, (str) =>
        {
            return int.Parse(str);
        });
    }

    /// <summary>
    /// 专门用来拆分多组键值对形式的数据的 以int返回
    /// </summary>
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">组间分隔符  1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
    /// <param name="typeTwo">键值对分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
    /// <param name="callBack">回调函数</param>
    public static void SplitStrToIntArrTwice(string str, int typeOne, int typeTwo, Action<int, int> callBack)
    {
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0)
            return;
        int[] ints;
        for (int i = 0; i < strs.Length; i++)
        {
            //拆分单个道具的ID和数量信息
            ints = SplitStrToIntArr(strs[i], typeTwo);
            if (ints.Length == 0)
                continue;
            callBack.Invoke(ints[0], ints[1]);
        }
    }

    /// <summary>
    /// 专门用来拆分多组键值对形式的数据的 以string返回
    /// </summary>
    /// <param name="str">待拆分的字符串</param>
    /// <param name="typeOne">组间分隔符 1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
    /// <param name="typeTwo">键值对分隔符  1-; 2-, 3-% 4-: 5-空格 6-| 7-_ </param>
    /// <param name="callBack">回调函数</param>
    public static void SplitStrTwice(string str, int typeOne, int typeTwo, Action<string, string> callBack)
    {
        string[] strs = SplitStr(str, typeOne);
        if (strs.Length == 0)
            return;
        string[] strs2;
        for (int i = 0; i < strs.Length; i++)
        {
            //拆分单个道具的ID和数量信息
            strs2 = SplitStr(strs[i], typeTwo);
            if (strs2.Length == 0)
                continue;
            callBack.Invoke(strs2[0], strs2[1]);
        }
    }


    #endregion

    #region 数字转字符串相关
    /// <summary>
    /// 得到指定长度的数字转字符串内容，如果长度不够会在前面补0，如果长度超出，会保留原始数值
    /// </summary>
    /// <param name="value">数值</param>
    /// <param name="len">长度</param>
    /// <returns></returns>
    public static string GetNumStr(int value, int len)
    {
        //tostring中传入一个 Dn 的字符串
        //代表想要将数字转换为长度位n的字符串
        //如果长度不够 会在前面补0
        return value.ToString($"D{len}");
    }
    /// <summary>
    /// 让指定浮点数保留小数点后n位
    /// </summary>
    /// <param name="value">具体的浮点数</param>
    /// <param name="len">保留小数点后n位</param>
    /// <returns></returns>
    public static string GetDecimalStr(float value, int len)
    {
        //tostring中传入一个 Fn 的字符串
        //代表想要保留小数点后几位小数
        return value.ToString($"F{len}");
    }

    /// <summary>
    /// 将较大较长的数 转换为字符串
    /// </summary>
    /// <param name="num">具体数值</param>
    /// <returns>n亿n千万 或 n万n千 或 1000 3434 234</returns>
    public static string GetBigDataToString(int num)
    {
        //如果大于1亿 那么就显示 n亿n千万
        if (num >= 100000000)
        {
            return BigDataChange(num, 100000000, "亿", "千万");
        }
        //如果大于1万 那么就显示 n万n千
        else if (num >= 10000)
        {
            return BigDataChange(num, 10000, "万", "千");
        }
        //都不满足 就直接显示数值本身
        else
            return num.ToString();
    }

    /// <summary>
    /// 把大数据转换成对应的字符串拼接
    /// </summary>
    /// <param name="num">数值</param>
    /// <param name="company">分割单位 可以填 100000000、10000</param>
    /// <param name="bigCompany">大单位 亿、万</param>
    /// <param name="littltCompany">小单位 万、千</param>
    /// <returns></returns>
    private static string BigDataChange(int num, int company, string bigCompany, string littltCompany)
    {
        resultStr.Clear();
        //有几亿、几万
        resultStr.Append(num / company);
        resultStr.Append(bigCompany);
        //有几千万、几千
        int tmpNum = num % company;
        //看有几千万、几千
        tmpNum /= (company / 10);
        //算出来不为0
        if(tmpNum != 0)
        {
            resultStr.Append(tmpNum);
            resultStr.Append(littltCompany);
        }
        return resultStr.ToString();
    }

    #endregion

    #region 时间转换相关
    /// <summary>
    /// 秒转时分秒格式 其中时分秒可以自己传
    /// </summary>
    /// <param name="s">秒数</param>
    /// <param name="egZero">是否忽略0</param>
    /// <param name="isKeepLen">是否保留至少2位</param>
    /// <param name="hourStr">小时的拼接字符</param>
    /// <param name="minuteStr">分钟的拼接字符</param>
    /// <param name="secondStr">秒的拼接字符</param>
    /// <returns></returns>
    public static string SecondToHMS(int s, bool egZero = false, bool isKeepLen = false, string hourStr = "时", string minuteStr = "分", string secondStr = "秒")
    {
        //时间不会有负数 所以我们如果发现是负数直接归0
        if (s < 0)
            s = 0;
        //计算小时
        int hour = s / 3600;
        //计算分钟
        //除去小时后的剩余秒
        int second = s % 3600;
        //剩余秒转为分钟数
        int minute = second / 60;
        //计算秒
        second = s % 60;
        //拼接
        resultStr.Clear();
        //如果小时不为0 或者 不忽略0 
        if (hour != 0 || !egZero)
        {
            resultStr.Append(isKeepLen?GetNumStr(hour, 2):hour);//具体几个小时
            resultStr.Append(hourStr);
        }
        //如果分钟不为0 或者 不忽略0 或者 小时不为0
        if(minute != 0 || !egZero || hour != 0)
        {
            resultStr.Append(isKeepLen?GetNumStr(minute,2): minute);//具体几分钟
            resultStr.Append(minuteStr);
        }
        //如果秒不为0 或者 不忽略0 或者 小时和分钟不为0
        if(second != 0 || !egZero || hour != 0 || minute != 0)
        {
            resultStr.Append(isKeepLen?GetNumStr(second,2): second);//具体多少秒
            resultStr.Append(secondStr);
        }

        //如果传入的参数是0秒时
        if(resultStr.Length == 0)
        {
            resultStr.Append(0);
            resultStr.Append(secondStr);
        }

        return resultStr.ToString();
    }
    
    /// <summary>
    /// 秒转00:00:00格式
    /// </summary>
    /// <param name="s"></param>
    /// <param name="egZero"></param>
    /// <returns></returns>
    public static string SecondToHMS2(int s, bool egZero = false)
    {
        return SecondToHMS(s, egZero, true, ":", ":", "");
    }
    #endregion

    #region 字符串格式化相关
    /// <summary>
    /// 格式化电话号码，添加分隔符
    /// </summary>
    /// <param name="phoneNumber">电话号码</param>
    /// <param name="separator">分隔符</param>
    /// <returns>格式化后的电话号码</returns>
    public static string FormatPhoneNumber(string phoneNumber, string separator = "-")
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return string.Empty;
        
        // 移除所有非数字字符
        string cleanNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        // 格式化中国大陆手机号
        if (cleanNumber.Length == 11)
        {
            return $"{cleanNumber.Substring(0, 3)}{separator}{cleanNumber.Substring(3, 4)}{separator}{cleanNumber.Substring(7)}";
        }
        // 格式化固定电话
        else if (cleanNumber.Length >= 7)
        {
            int areaCodeLength = cleanNumber.Length - 7;
            return $"{cleanNumber.Substring(0, areaCodeLength)}{separator}{cleanNumber.Substring(areaCodeLength)}";
        }
        
        return phoneNumber;
    }
    
    /// <summary>
    /// 格式化姓名，隐藏中间字符
    /// </summary>
    /// <param name="name">姓名</param>
    /// <param name="hideChar">隐藏字符</param>
    /// <returns>格式化后的姓名</returns>
    public static string FormatName(string name, char hideChar = '*')
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;
        
        if (name.Length <= 2)
            return name;
        
        StringBuilder sb = new StringBuilder(name.Length);
        sb.Append(name[0]);
        for (int i = 1; i < name.Length - 1; i++)
        {
            sb.Append(hideChar);
        }
        sb.Append(name[name.Length - 1]);
        return sb.ToString();
    }
    
    /// <summary>
    /// 格式化银行卡号，添加空格分隔
    /// </summary>
    /// <param name="cardNumber">银行卡号</param>
    /// <returns>格式化后的银行卡号</returns>
    public static string FormatBankCardNumber(string cardNumber)
    {
        if (string.IsNullOrEmpty(cardNumber))
            return string.Empty;
        
        // 移除所有空格
        string cleanNumber = cardNumber.Replace(" ", "");
        
        // 每4位添加一个空格
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < cleanNumber.Length; i++)
        {
            if (i > 0 && i % 4 == 0)
                sb.Append(" ");
            sb.Append(cleanNumber[i]);
        }
        
        return sb.ToString();
    }
    #endregion

    #region 文本验证相关
    /// <summary>
    /// 验证是否为有效的邮箱地址
    /// </summary>
    /// <param name="email">邮箱地址</param>
    /// <returns>是否为有效邮箱</returns>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
        
        try
        {
            var mailAddress = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// 验证是否为有效的中国大陆手机号
    /// </summary>
    /// <param name="phoneNumber">手机号</param>
    /// <returns>是否为有效手机号</returns>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return false;
        
        // 移除所有非数字字符
        string cleanNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        // 中国大陆手机号格式：11位数字，以1开头
        return cleanNumber.Length == 11 && cleanNumber.StartsWith("1");
    }
    
    /// <summary>
    /// 验证是否为有效的URL
    /// </summary>
    /// <param name="url">URL地址</param>
    /// <returns>是否为有效URL</returns>
    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return false;
        
        return System.Uri.TryCreate(url, System.UriKind.Absolute, out _);
    }
    #endregion

    #region 字符串操作相关
    /// <summary>
    /// 将字符串首字母大写
    /// </summary>
    /// <param name="str">输入字符串</param>
    /// <returns>首字母大写的字符串</returns>
    public static string CapitalizeFirstLetter(string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;
        
        char[] chars = str.ToCharArray();
        chars[0] = char.ToUpper(chars[0]);
        return new string(chars);
    }
    
    /// <summary>
    /// 将字符串转换为驼峰命名
    /// </summary>
    /// <param name="str">输入字符串</param>
    /// <param name="separator">分隔符</param>
    /// <returns>驼峰命名的字符串</returns>
    public static string ToCamelCase(string str, char separator = '_')
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;
        
        string[] parts = str.Split(separator);
        if (parts.Length == 1)
            return parts[0];
        
        StringBuilder sb = new StringBuilder(parts[0].ToLower());
        for (int i = 1; i < parts.Length; i++)
        {
            sb.Append(CapitalizeFirstLetter(parts[i].ToLower()));
        }
        return sb.ToString();
    }
    
    /// <summary>
    /// 截断字符串，超出部分用省略号代替
    /// </summary>
    /// <param name="str">输入字符串</param>
    /// <param name="maxLength">最大长度</param>
    /// <returns>截断后的字符串</returns>
    public static string TruncateString(string str, int maxLength)
    {
        if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
            return str;
        
        return str.Substring(0, maxLength) + "...";
    }
    #endregion

    #region 文本加密/解密相关
    /// <summary>
    /// Base64编码字符串
    /// </summary>
    /// <param name="str">输入字符串</param>
    /// <returns>Base64编码后的字符串</returns>
    public static string Base64Encode(string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;
        
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(bytes);
    }
    
    /// <summary>
    /// Base64解码字符串
    /// </summary>
    /// <param name="base64Str">Base64编码的字符串</param>
    /// <returns>解码后的字符串</returns>
    public static string Base64Decode(string base64Str)
    {
        if (string.IsNullOrEmpty(base64Str))
            return string.Empty;
        
        try
        {
            byte[] bytes = Convert.FromBase64String(base64Str);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return string.Empty;
        }
    }
    #endregion

    #region 本地化相关
    /// <summary>
    /// 替换字符串中的占位符
    /// </summary>
    /// <param name="template">模板字符串，占位符格式为{0}, {1}等</param>
    /// <param name="args">替换参数</param>
    /// <returns>替换后的字符串</returns>
    public static string ReplacePlaceholders(string template, params object[] args)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;
        
        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }
    
    /// <summary>
    /// 根据键值对替换字符串中的占位符
    /// </summary>
    /// <param name="template">模板字符串，占位符格式为{key}</param>
    /// <param name="placeholders">占位符键值对</param>
    /// <returns>替换后的字符串</returns>
    public static string ReplaceNamedPlaceholders(string template, Dictionary<string, string> placeholders)
    {
        if (string.IsNullOrEmpty(template) || placeholders == null || placeholders.Count == 0)
            return template;
        
        string result = template;
        foreach (var kvp in placeholders)
        {
            result = result.Replace($"{{{kvp.Key}}}", kvp.Value);
        }
        return result;
    }
    #endregion

}

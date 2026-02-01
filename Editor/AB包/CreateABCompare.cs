using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CreateABCompare
{
    
    public static void CreateCompate()
    {
        //获取文件夹信息
        DirectoryInfo directory = new DirectoryInfo(Application.dataPath + "/AB/PC/");
        //获取该目录下的所有文件信息
        FileInfo[] fileInfos = directory.GetFiles();
        //用于存储信息的 字符串
        string abCompareInfo = "";

        foreach (FileInfo info in fileInfos)
        {
            //没有后缀的 才是AB包 我们只想要AB包的信息
            if (info.Extension == "")
            {
                //拼接一个AB包的信息
                abCompareInfo = info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                //用一个分隔符分开不同文件之间的信息
                abCompareInfo += "|";
            }
        }
        //因为循环完毕后 会在最后由一个 | 符号 所以 把它去掉
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);

        //存储拼接好的 AB包资源信息
        File.WriteAllText(Application.dataPath + "/ArtRes/AB/PC/ABCompareInfo.txt", abCompareInfo);
        //刷新编辑器
        AssetDatabase.Refresh();

        Debug.Log("AB包对比文件生成成功");
    }

    public static string GetMD5(string filePath)
    {
        //将文件以流的形式打开
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            //声明一个MD5对象 用于生成MD5码
            MD5 md5 = new MD5CryptoServiceProvider();
            //利用API 得到数据的MD5码 16个字节 数组
            byte[] md5Info = md5.ComputeHash(stream);

            //关闭文件流
            stream.Close();

            //把16个字节转换为 16进制 拼接成字符串 为了减小md5码的长度
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < md5Info.Length; i++)
                stringBuilder.Append(md5Info[i].ToString("x2"));

            return stringBuilder.ToString();
        }
    }
}

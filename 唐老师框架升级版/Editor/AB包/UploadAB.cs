using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;
using System.Net;

using System;

public class UploadAB 
{
    
    public static void UploadCompare()
    {
        //获取文件夹信息
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/AB/PC/");
        //获取该目录下的所有文件信息
        FileInfo[] fileInfos = directory.GetFiles();
       

        foreach (FileInfo info in fileInfos)
        {
            //没有后缀的 才是AB包 我们只想要AB包的信息
            if (info.Extension == ""||info.Extension==".txt")
            {
                FtpUploadFile(info.FullName, info.Name);
            }
        }
    }

    private async static void FtpUploadFile(string filePath, string fileName)
    {
        await Task.Run(() =>
        {
            try
            {
                //1.创建一个FTP连接 用于上传
                FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://192.168.56.1/AB/PC/" + fileName)) as FtpWebRequest;
                //2.设置一个通信凭证 这样才能上传
                NetworkCredential n = new NetworkCredential("陈粤宁", "yzh337088");
                req.Credentials = n;
                //3.其它设置
                //  设置代理为null
                req.Proxy = null;
                //  请求完毕后 是否关闭控制连接
                req.KeepAlive = false;
                //  操作命令-上传
                req.Method = WebRequestMethods.Ftp.UploadFile;
                //  指定传输的类型 2进制
                req.UseBinary = true;
                //4.上传文件
                //  ftp的流对象
                Stream upLoadStream=req.GetRequestStream();
                //  读取文件信息 写入该流对象
                using (FileStream file = File.OpenRead(filePath))
                {
                    //一点一点的上传内容
                    byte[] bytes = new byte[2048];
                    //返回值 代表读取了多少个字节
                    int contentLength=file.Read(bytes, 0, bytes.Length);
                    //循环上传文件中的数据
                    while (contentLength != 0)
                    {
                        //写入到上传流中
                        upLoadStream.Write(bytes, 0, contentLength);
                        //写完再读
                        contentLength = file.Read(bytes, 0, bytes.Length);
                    }

                    //循环完毕后 证明上传结束
                    file.Close();
                    upLoadStream.Close();
                }
                Debug.Log(fileName + "上传成功");
            }
            catch (Exception ex)
            {
                Debug.Log(fileName + "上传失败" + ex.Message);
            }
        });
    }
}

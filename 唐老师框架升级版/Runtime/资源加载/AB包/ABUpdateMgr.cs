using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

/// <summary>
/// AB包更新管理器
/// 负责自动从服务器下载更新AssetBundle资源
/// 实现了增量更新模式：AB包版本对比、增量下载等功能
/// </summary>
public class ABUpdateMgr : BaseManager<ABUpdateMgr>
{
    // 存储远程AB包信息的字典，用于与本地AB包进行对比
    private Dictionary<string, ABInfo> remoteABInfo = new Dictionary<string, ABInfo>();

    // 存储本地AB包信息的字典，用于与远程AB包进行对比
    private Dictionary<string, ABInfo> localABInfo = new Dictionary<string, ABInfo>();

    // 需要下载更新的AB包列表
    private List<string> downLoadList = new List<string>();

    // 资源服务器的IP地址
    private string serverIP = "ftp://10.20.90.73";

    private string userName = "DadivTao";
    private string password = "114514";

    /// <summary>
    /// 检查AB包更新
    /// </summary>
    /// <param name="overCallBack">检查完成后的回调函数(bool参数表示是否检查成功)</param>
    /// <param name="updateInfoCallBack">更新过程中的信息回调函数(string参数为信息内容)</param>
    /// <param name="updateSizeCallBack">检测到更新时，回调需要下载的AB包总大小的回调函数(long参数为字节大小)</param>
    public void CheckUpdate(Action<bool> overCallBack, Action<string> updateInfoCallBack, Action<float> updateSizeCallBack = null,Action<float> updatePro=null)
    {
        // 清空之前的缓存数据，确保每次检查都是全新的开始
        remoteABInfo.Clear();
        localABInfo.Clear();
        downLoadList.Clear();

        // 1.从服务器下载AB包对比文件
        DownLoadABCompareFile((isOver) =>
        {
            updateInfoCallBack("开始连接服务器");
            if (isOver)
            {
                updateInfoCallBack("对比文件下载成功");
                string remoteInfo = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
                updateInfoCallBack("解析远程对比文件");
                GetRemoteABCompareFileInfo(remoteInfo, remoteABInfo);
                updateInfoCallBack("解析远程对比文件完成");

                // 2.获取本地的AB包对比文件信息
                GetLocalABCompareFileInfo((isOverLocal) =>
                {
                    if (isOverLocal)
                    {
                        updateInfoCallBack("解析本地对比文件完成");
                        // 3.对比本地和远程AB包信息，确定需要更新的内容
                        updateInfoCallBack("开始对比");
                        foreach (string abName in remoteABInfo.Keys)
                        {
                            // 如果本地没有该AB包，添加到下载列表
                            if (!localABInfo.ContainsKey(abName))
                                downLoadList.Add(abName);
                            else
                            {
                                // 如果本地有该AB包，对比MD5值判断是否需要更新
                                if (localABInfo[abName].md5 != remoteABInfo[abName].md5)
                                    downLoadList.Add(abName);
                                // MD5不同则需要更新，从本地列表移除(剩下的本地列表就是需要删除的旧AB包)
                                localABInfo.Remove(abName);
                            }
                        }
                        updateInfoCallBack("对比完成");

                        // 计算需要下载的AB包总大小
                        long totalSize = 0;
                        foreach (string abName in downLoadList)
                        {
                            if (remoteABInfo.ContainsKey(abName))
                            {
                                totalSize += remoteABInfo[abName].size;
                            }
                        }

                        // 如果下载列表不为空，则回调下载大小信息
                        if (downLoadList.Count > 0 && updateSizeCallBack != null)
                        {
                            float mb = (totalSize / 1048576) * 2.47f;
                            updateSizeCallBack(mb);
                        }
                        updateInfoCallBack("删除过时的AB包文件");
                        // 删除本地没有在远程列表中的AB包(过时的AB包)
                        foreach (string abName in localABInfo.Keys)
                        {
                            if (File.Exists(Application.persistentDataPath + "/" + abName))
                                File.Delete(Application.persistentDataPath + "/" + abName);
                        }
                        updateInfoCallBack("删除完成，开始下载更新AB包");
                        // 下载需要更新的AB包
                        DownLoadABFile((isOverDown) =>
                        {
                            if (isOverDown)
                            {
                                // 下载完成后，将远程对比文件保存为本地对比文件
                                updateInfoCallBack("更新AB包对比文件保存为本地");
                                File.WriteAllText(Application.persistentDataPath + "/ABCompareInfo.txt", remoteInfo);
                            }
                            overCallBack(isOverDown);
                        }, updatePro);
                    }
                    else
                        overCallBack(false);
                });
            }
            else
            {
                overCallBack(false);
            }
        });
    }

    /// <summary>
    /// 下载AB包对比文件
    /// </summary>
    /// <param name="overCallBack">下载完成后的回调函数(bool参数表示是否下载成功)</param>
    public async void DownLoadABCompareFile(Action<bool> overCallBack)
    {
        bool isOver = false;
        int reDownLoadMaxNum = 5;  // 最大重试次数
        string localPath = Application.persistentDataPath;

        // 循环下载，直到成功或达到最大重试次数
        while (!isOver && reDownLoadMaxNum > 0)
        {
            // 使用Task在后台线程执行下载，避免阻塞主线程
            await Task.Run(() =>
            {
                isOver = DownLoadFile("ABCompareInfo.txt", localPath + "/ABCompareInfo_TMP.txt");
            });
            --reDownLoadMaxNum;
        }

        // 回调通知下载结果
        overCallBack?.Invoke(isOver);
    }

    /// <summary>
    /// 解析远程AB包对比文件信息
    /// </summary>
    /// <param name="info">对比文件的内容字符串</param>
    /// <param name="ABInfo">存储解析结果的字典</param>
    public void GetRemoteABCompareFileInfo(string info, Dictionary<string, ABInfo> ABInfo)
    {
        string[] strs = info.Split('|');  // 用|分割每个AB包信息
        string[] infos = null;
        for (int i = 0; i < strs.Length; i++)
        {
            infos = strs[i].Split(' ');  // 用空格分割每个AB包的具体信息
            // 将解析的信息添加到字典
            ABInfo.Add(infos[0], new ABInfo(infos[0], infos[1], infos[2]));
        }
    }

    /// <summary>
    /// 获取本地AB包对比文件信息
    /// </summary>
    /// <param name="overCallBack">获取完成后的回调函数(bool参数表示是否获取成功)</param>
    public async void GetLocalABCompareFileInfo(Action<bool> overCallBack)
    {
        // 首先尝试从持久化目录获取对比文件
        if (File.Exists(Application.persistentDataPath + "/ABCompareInfo.txt"))
        {
            bool result = await GetLocalABCOmpareFileInfoAsync("file:///" + Application.persistentDataPath + "/ABCompareInfo.txt");
            overCallBack(result);
        }
        // 如果持久化目录没有，则尝试从StreamingAssets目录获取
        else if (File.Exists(Application.streamingAssetsPath + "/ABCompareInfo.txt"))
        {
            string path =
#if UNITY_ANDROID
                Application.streamingAssetsPath;
#else
                "file:///" + Application.streamingAssetsPath;
#endif
            bool result = await GetLocalABCOmpareFileInfoAsync(path + "/ABCompareInfo.txt");
            overCallBack(result);
        }
        // 如果都没有，表示本地没有对比文件，不需要对比
        else
            overCallBack(true);
    }

    /// <summary>
    /// UniTask：异步获取本地AB包对比文件信息
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否获取成功</returns>
    private async UniTask<bool> GetLocalABCOmpareFileInfoAsync(string filePath)
    {
        // 使用UnityWebRequest获取本地文件
        UnityWebRequest req = UnityWebRequest.Get(filePath);
        await req.SendWebRequest();

        // 获取成功
        if (req.result == UnityWebRequest.Result.Success)
        {
            GetRemoteABCompareFileInfo(req.downloadHandler.text, localABInfo);
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// 下载需要更新的AB包文件
    /// </summary>
    /// <param name="overCallBack">下载完成后的回调函数(bool参数表示是否下载成功)</param>
    /// <param name="updatePro">下载进度回调函数(string参数为进度信息)</param>
    public async void DownLoadABFile(Action<bool> overCallBack, Action<float> updatePro)
    {
        string localPath = Application.persistentDataPath + "/";
        bool isOver = false;
        List<string> tempList = new List<string>();  // 临时存储下载成功的AB包
        int reDownLoadMaxNum = 5;  // 最大重试次数
        int downLoadOverNum = 0;  // 下载完成的数量
        int downLoadMaxNum = downLoadList.Count;  // 需要下载的总数量

        // 循环下载，直到所有AB包都下载完成或达到最大重试次数
        while (downLoadList.Count > 0 && reDownLoadMaxNum > 0)
        {
            for (int i = 0; i < downLoadList.Count; i++)
            {
                isOver = false;
                // 在后台线程执行下载
                await UniTask.RunOnThreadPool(() =>
                {
                    isOver = DownLoadFile(downLoadList[i], localPath + downLoadList[i]);
                });
                if (isOver)
                {
                    // 更新进度
                    updatePro((float)++downLoadOverNum / downLoadMaxNum);
                    tempList.Add(downLoadList[i]);  // 记录下载成功的AB包
                }
            }
            // 从下载列表中移除已成功下载的AB包
            for (int i = 0; i < tempList.Count; i++)
                downLoadList.Remove(tempList[i]);

            --reDownLoadMaxNum;
        }

        // 回调通知下载结果
        overCallBack(downLoadList.Count == 0);
    }

    /// <summary>
    /// 下载指定文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="localPath">本地保存路径</param>
    /// <returns>是否下载成功</returns>
    private bool DownLoadFile(string fileName, string localPath)
    {
        try
        {
            // 根据不同平台选择对应的AB包目录
            string pInfo =
#if UNITY_IOS
            "IOS";
#elif UNITY_ANDROID
            "Android";
#else
            "PC";
#endif

            // 创建FTP请求
            FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + "/AB/" + pInfo + "/" + fileName)) as FtpWebRequest;
            // 设置FTP凭证
            NetworkCredential n = new NetworkCredential(userName, password);
            req.Credentials = n;
            // 其他设置
            req.Proxy = null;
            req.KeepAlive = false;
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.UseBinary = true;  // 使用二进制传输

            // 获取响应并下载文件
            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            Stream downLoadStream = res.GetResponseStream();
            using (FileStream file = File.Create(localPath))
            {
                byte[] bytes = new byte[2048];  // 2KB缓冲区
                int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);

                // 循环读取并写入文件
                while (contentLength != 0)
                {
                    file.Write(bytes, 0, contentLength);
                    contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                }

                // 关闭流
                file.Close();
                downLoadStream.Close();

                return true;
            }
        }
        catch (Exception ex)
        {
            LogSystem.Info(fileName + "下载失败" + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// AB包信息类
    /// 存储AB包的基本信息
    /// </summary>
    public class ABInfo
    {
        public string name;  // AB包名称
        public long size;    // AB包大小
        public string md5;   // AB包的MD5哈希值

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">AB包名称</param>
        /// <param name="size">AB包大小字符串</param>
        /// <param name="md5">AB包MD5值</param>
        public ABInfo(string name, string size, string md5)
        {
            this.name = name;
            this.size = long.Parse(size);
            this.md5 = md5;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HanHanReptile
{
    public static class DownloadHelper
    {
        public static string rootPath = "G:\\";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void GetDownloadFolder(string path)
        {
            try
            {
                if (Directory.Exists(path) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("创建文件夹异常：" + ex.Message);
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">资源连接</param>
        /// <param name="fileName">重命名文件名</param>RootPath
        public static void DownloadImgFile(string url, string downloadpath)
        {
            WebClient mywebclient = new WebClient();
            try
            {
                mywebclient.DownloadFile(url, downloadpath);
                Console.WriteLine("下载成功！目录：" + downloadpath);
            }
            catch (Exception e)
            {
                Console.WriteLine("下载图片异常：" + e.Message);
            }

        }

    }
}

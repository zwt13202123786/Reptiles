using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Threading;

namespace HanHanReptile
{
    class Program
    {
        public static Dictionary<int, string> CartoonTypeList = new Dictionary<int, string>();

        static void Main(string[] args)
        {
            StartCrawl(7);

            //2、获取获取漫画实体类（这一步超过不能超过40s）
            //Cartoon cartoon = new HanHanReptile().GetCartoonChapter("http://www.1manhua.net/manhua35851.html");
            //if (cartoon != null)
            //{
            //    Console.WriteLine("线程" + Thread.CurrentThread.ManagedThreadId.ToString("00") + "：漫画【" + cartoon.Name + "】爬取成功！");
            //}

            ////3、将漫画数据写入数据库（这一步超过不能超过40s）
            //string Mess = SqlOperation.InsertWholeCartoon(cartoon, "萌系");
            //if (Mess == "1")
            //{
            //    Console.WriteLine("线程" + Thread.CurrentThread.ManagedThreadId.ToString("00") + "：漫画【" + cartoon.Name + "】添加数据库成功！");
            //}
            //else
            //{
            //    Console.WriteLine(Mess);
            //}
            //Console.ReadKey();

        }


        //时间戳(毫秒值)String转换为DateTime类型转换
        static string MillisecondToDate(long time)
        {
            string ms = (time % 1000).ToString();
            string ss = ((time / 1000) % 60).ToString();
            string mm = ((time / 1000) / 60 % 60).ToString();
            string hh = ((time / 1000) / 60 / 60 % 60).ToString();
            return hh + ":" + mm + ":" + ss + "." + ms;
        }

        /// <summary>
        /// 开始爬去任务
        /// </summary>
        /// <param name="threadNumber">线程数量</param>
        static void StartCrawl(int threadNumber)
        {
            int CartoonTypeID = 1;
            //清除添加失败漫画
            SqlOperation.ClearFailInfo();
            Console.WriteLine("清除添加失败漫画成功！");

            object lock1 = new object();
            HanHanReptile hanHanReptile = new HanHanReptile();
            string CartoonTypeName = GetCartoonType(CartoonTypeID);
            if (CartoonTypeName == null)
            {
                Console.WriteLine("漫画类型不存在！");
                return;
            }

            List<CartoonAll> CartoonAllList = hanHanReptile.GetAllCartoon(CartoonTypeID, CartoonTypeName, 10000);
            Console.WriteLine(CartoonTypeName + "分类漫画总数爬取成功！总数：" + CartoonAllList.Count);
            for (int i = 0; i < threadNumber; i++)
            {
                Thread thread = new Thread(() =>
                {
                    Console.WriteLine("线程" + Thread.CurrentThread.ManagedThreadId.ToString("00") + "：创建成功！开始爬取任务");
                    bool IsCrawl = true;
                    while (IsCrawl)
                    {
                        //1、获取漫画的地址
                        CartoonAll cartoonAll = null;
                        lock (lock1)
                        {
                            foreach (CartoonAll item in CartoonAllList)
                            {
                                if (item.CrawlState == 0)
                                {
                                    cartoonAll = item;
                                    item.CrawlState = 1;
                                    break;
                                }
                            }
                            if (cartoonAll == null)
                            {
                                IsCrawl = false;
                                continue;
                            }
                        }

                        Cartoon cartoon = null;
                        //2、获取获取漫画实体类（这一步超过不能超过40s）
                        cartoon = hanHanReptile.GetCartoonChapter(cartoonAll.Url);
                        if (cartoon != null)
                        {
                            Console.WriteLine("线程" + Thread.CurrentThread.ManagedThreadId.ToString("00") + "：漫画【" + cartoon.Name + "】爬取成功！");
                        }
                        else
                        {
                            continue;
                        }

                        //3、将漫画数据写入数据库（这一步超过不能超过40s）
                        string Mess = SqlOperation.InsertWholeCartoon(cartoon, CartoonTypeName);
                        if (Mess == "1")
                        {
                            Console.WriteLine("线程" + Thread.CurrentThread.ManagedThreadId.ToString("00") + "：漫画【" + cartoon.Name + "】添加数据库成功！");
                        }
                        else
                        {
                            Console.WriteLine(Mess);
                            continue;
                        }


                        //4、将数组中漫画的状态改为2
                        foreach (CartoonAll item in CartoonAllList)
                        {
                            if (item.Name == cartoon.Name)
                            {
                                item.CrawlState = 2;
                                break;
                            }
                        }


                        //判断时候爬取结束
                        bool IsComplete = true;
                        lock (lock1)
                        {
                            foreach (CartoonAll item in CartoonAllList)
                            {
                                if (item.CrawlState == 0)
                                {
                                    IsComplete = false;
                                    break;
                                }
                            }
                        }
                        if (IsComplete)
                        {
                            IsCrawl = false;
                            continue;
                        }
                    }
                    Console.WriteLine("线程" + Thread.CurrentThread.ManagedThreadId.ToString("00") + "爬去结束！");

                })
                {
                    Name = "线程" + i,
                    IsBackground = false,
                };
                thread.Start();
            }
            //Cartoon cartoon = hanHanReptile.GetCartoonChapter("http://www.1manhua.net/manhua37477.html");
            //List<CartoonAll> cartoonAlls = hanHanReptile.GetAllCartoon(1);
            //Console.ReadKey();
        }


        static string GetCartoonType(int typeId)
        {
            if (CartoonTypeList == null || CartoonTypeList.Count == 0)
            {
                CartoonTypeList.Add(1, "萌系");
                CartoonTypeList.Add(2, "搞笑");
                CartoonTypeList.Add(3, "格斗");
                CartoonTypeList.Add(4, "科幻");
                CartoonTypeList.Add(5, "剧情");
                CartoonTypeList.Add(6, "侦探");
                CartoonTypeList.Add(7, "竞技");
                CartoonTypeList.Add(8, "魔法");
                CartoonTypeList.Add(9, "神鬼");
                CartoonTypeList.Add(10, "校园");
                CartoonTypeList.Add(11, "惊栗");
                CartoonTypeList.Add(12, "厨艺");
                CartoonTypeList.Add(13, "伪娘");
                CartoonTypeList.Add(15, "冒险");
                CartoonTypeList.Add(19, "小说");
                CartoonTypeList.Add(20, "港漫");
                CartoonTypeList.Add(21, "耽美");
                CartoonTypeList.Add(22, "经典");
                CartoonTypeList.Add(23, "欧美");
                CartoonTypeList.Add(24, "日文");
                CartoonTypeList.Add(25, "亲情");
                if (CartoonTypeList.ContainsKey(typeId))
                {
                    return CartoonTypeList[typeId];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (CartoonTypeList.ContainsKey(typeId))
                {
                    return CartoonTypeList[typeId];
                }
                else
                {
                    return null;
                }

            }
        }
    }
}

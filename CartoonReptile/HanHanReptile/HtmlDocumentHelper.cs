using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace HanHanReptile
{
    public class HtmlDocumentHelper
    {
        public static void GetClassDocument(string xmlString)
        {
            XmlDocument doc = new XmlDocument(); //不要忘记导入System.Xml
            doc.LoadXml(xmlString);//装载xml文件
            XmlNode cVolListNode = doc.SelectSingleNode("<div class=\"cVolList\">");
            foreach (XmlNode item in cVolListNode.ChildNodes)
            {
                Console.WriteLine(item.Name);
            }
        }


        /// <summary>
        /// 将传入的方法不断进行重试，直至执行不超时
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="ms">超时时间</param>
        /// <param name="func">方法</param>
        public void LoopRetry<T>(int ms, Func<T> func, string mess)
        {
            bool IsSuccess = false;
            T checkResult;
            while (!IsSuccess)
            {
                checkResult = TimeoutCheck(ms, () =>
                {
                    return func();
                });
                if (checkResult == null)
                {
                    IsSuccess = false;
                    Console.WriteLine("线程" + Thread.CurrentThread.ManagedThreadId.ToString("00") + "：" + mess + "，再次重试！");
                }
                else
                {
                    IsSuccess = true;
                }
            }
        }



        /// <summary>
        /// 将传入的方法不断进行重试，直至执行不超时（有返回值）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ms"></param>
        /// <param name="func"></param>
        /// <param name="mess"></param>
        /// <returns></returns>
        public T LoopRetryReturn<T>(int ms, Func<T> func, string mess)
        {
            bool IsSuccess = false;
            T checkResult = default(T);
            while (!IsSuccess)
            {
                checkResult = TimeoutCheck(ms, () =>
                {
                    return func();
                });
                if (checkResult == null)
                {
                    IsSuccess = false;
                    Console.WriteLine("线程" + Thread.CurrentThread.ManagedThreadId.ToString("00") + "：" + mess + "，再次重试！");
                }
                else
                {
                    IsSuccess = true;
                }
            }
            return checkResult;
        }




        /// <summary>
        /// 判断方法执行是否超时
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="ms">等待的毫秒数量</param>
        /// <param name="func">方法</param>
        /// <returns></returns>        
        internal static T TimeoutCheck<T>(int ms, Func<T> func)
        {
            var wait = new ManualResetEvent(false);
            bool RunOK = false;
            var task = Task.Run<T>(() =>
            {
                var result = func.Invoke();
                RunOK = true;
                wait.Set();
                return result;
            });
            wait.WaitOne(ms);
            if (RunOK)
            {
                return task.Result;
            }
            else
            {
                return default(T);
            }
        }




    }
}

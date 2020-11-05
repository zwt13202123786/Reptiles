using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CartoonReptile.Common
{
    public class RequestHelper
    {

        /// <summary>
        /// 将传入的方法不断进行重试，直至执行不超时
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="time">超时时间（毫秒）</param>
        /// <param name="retrynum">重试次数</param>
        /// <param name="func">执行方法</param>
        /// <param name="msg">超时提示语句</param>
        public static void LoopRetry<T>(int time,int retrynum, Func<T> func, string msg)
        {
            T Result;
            for (int i = 0; i < retrynum; i++)
            {
                Result = TimeoutCheck(time, () =>
                {
                    return func();
                });
                if (Result == null)
                {
                    Console.WriteLine(msg);
                }
                else
                {
                    return;
                }
            }
        }



        /// <summary>
        /// 将传入的方法不断进行重试，直至执行不超时（有返回值）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="time">超时时间（毫秒）</param>
        /// <param name="retrynum">重试次数</param>
        /// <param name="func"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static T LoopRetryReturn<T>(int time, int retrynum, Func<T> func, Action action)
        {
            T Result = default(T);
            for (int i = 0; i < retrynum; i++)
            {
                Result = TimeoutCheck(time, () =>
                {
                    try
                    {
                        return func();
                    }
                    catch (Exception)
                    {
                        return default(T);
                    }
                });
                if (Result == null)
                {
                    action();
                }
                else
                {
                    break;
                }
            }
            return Result;
        }




        /// <summary>
        /// 判断方法执行是否超时
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="ms">等待的毫秒数量</param>
        /// <param name="func">方法</param>
        /// <returns></returns>        
        private static T TimeoutCheck<T>(int ms, Func<T> func)
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

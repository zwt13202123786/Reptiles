using CartoonReptile.Dao;
using CartoonReptile.Model.HanHanCartoon.Enum;
using CartoonReptile.Model.Quartz;
using CartoonReptile.Model.Quartz.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CartoonReptile.Service
{
    public class CommService
    {
        static DbContext db = new DbContext();
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="msgType"></param>
        /// <param name="msg"></param>
        public static void Log(int taskId, int msgType, string msg)
        {
            db.TaskLogDao.Insert(new TaskLogEntity()
            {
                MsgType = msgType,
                Msg = msg,
                TaskId = taskId,
                CreateTime = DateTime.Now
            });
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
        public T LoopRetryReturn<T>(int time, int retrynum, Func<T> func, Action action)
        {
            T Result = default(T);
            bool IsSuccess = false;
            while (!IsSuccess)
            {
                Result = TimeoutCheck(time, () =>
                {
                    try
                    {
                        return func();
                    }
                    catch (Exception ex)
                    {
                        Log(1, (int)MsgTypeEnum.ExceptionMsg, ex.Message);
                        return default(T);
                    }
                });
                if (Result != null) IsSuccess = true;
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

        /// <summary>
        /// 切片数据
        /// </summary>
        /// <typeparam name="T">源数据模型</typeparam>
        /// <param name="SourceData">源数据</param>
        /// <param name="PageSize">每页行数</param>
        /// <returns></returns>
        public static List<KeyValuePair<int, List<T>>> Cut<T>(List<T> SourceData, int PageSize = 100)
        {
            int totalPage = 1;
            List<KeyValuePair<int, List<T>>> pageData = new List<KeyValuePair<int, List<T>>>();
            if (SourceData.Count <= PageSize)
                pageData.Add(new KeyValuePair<int, List<T>>(1, SourceData));
            else
            {
                totalPage = (SourceData.Count + PageSize - 1) / PageSize;
                for (int pageIdex = 1; pageIdex <= totalPage; pageIdex++)
                {
                    pageData.Add(new KeyValuePair<int, List<T>>(pageIdex, SourceData.Skip((pageIdex - 1) * PageSize).Take(PageSize).ToList()));
                }
            }
            return pageData;
        }

        public static T GetEnumDescription<T>(string description) where T : Enum
        {
            System.Reflection.FieldInfo[] fields = typeof(T).GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false); 
                if (objs.Length > 0 && (objs[0] as DescriptionAttribute).Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            throw new Exception($"未能找到对应的枚举:{description}");
        }

    }
}


using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.Quartz
{
    /// <summary>
    /// 定时任务日志表
    /// </summary>
    [SugarTable("TaskLog")]
    public class TaskLogEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 定时任务Id
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// 定时任务消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 定时任务消息类型，1-正常消息，2-异常消息
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 定时任务消息创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.Quartz
{
    /// <summary>
    /// 定时任务表
    /// </summary>
    [SugarTable("Task")]
    public class TaskEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 定时任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 任务修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}

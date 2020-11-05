using CartoonReptile.Dao.BaseDao.Impl;
using CartoonReptile.Dao.HanHanCartoon;
using CartoonReptile.Model.Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Dao.Quartz.Impl
{
    public class TaskLogDao: BaseDao<TaskLogEntity>, ITaskLogDao
    {
        private SqlSugarClient db;
        public TaskLogDao(SqlSugarClient sugarClient) : base(sugarClient)
        {
            db = sugarClient;
        }
    }
}

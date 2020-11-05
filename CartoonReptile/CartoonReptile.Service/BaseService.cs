using CartoonReptile.Dao;
using CartoonReptile.Model.Quartz;
using CartoonReptile.Model.Quartz.Enum;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Service
{
    public class BaseService
    {
        ///static DbContext staticDB = new DbContext();
        public DbContext db;
        public void Log(int taskId, int msgType, string msg)
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
        /// 只允许在同一个数据库中使用事务
        /// </summary>
        /// <param name="dbEnum"></param>
        /// <param name="action"></param>
        public void Tran(DbEnum dbEnum, Action<DbContext> action)
        {
            SqlSugarClient dbClient = null;
            try
            {
                switch (dbEnum)
                {
                    case DbEnum.HanHanCartoonDB:
                        dbClient = db.HanHanClient;
                        break;
                    case DbEnum.Quartz:
                        dbClient = db.QuartzClient;
                        break;
                }
                dbClient.Ado.BeginTran();
                action(db);
                dbClient.Ado.CommitTran();
            }
            catch (Exception)
            {
                if (dbClient != null)
                    dbClient.Ado.RollbackTran();
                throw;
            }
        }
    }
}

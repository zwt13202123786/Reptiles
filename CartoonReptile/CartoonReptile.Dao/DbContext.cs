using CartoonReptile.Dao.BaseDao;
using CartoonReptile.Dao.HanHanCartoon;
using CartoonReptile.Dao.HanHanCartoon.Impl;
using CartoonReptile.Dao.Quartz;
using CartoonReptile.Dao.Quartz.Impl;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Dao
{
    public class DbContext
    {
        //各个数据库连接对象-------------------------------------------------
        private const string hanHanConnectionString = "Data Source=localhost;port=3306;Initial Catalog=hanhancartoondb;user id=root;password=root;Charset=utf8;Allow User Variables=True;Connect Timeout=60;SSL Mode=None";
        private SqlSugarClient _hanHanClient;
        public SqlSugarClient HanHanClient
        {
            get
            {
                if (_hanHanClient == null)
                {
                    _hanHanClient = new SqlSugarClient(new ConnectionConfig()
                    {
                        ConnectionString = hanHanConnectionString,
                        DbType = DbType.MySql,//设置数据库类型
                        IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                        InitKeyType = InitKeyType.SystemTable //从实体特性中读取主键自增列信息
                    });
                }
                return _hanHanClient;
            }
        }

        private const string quartzConnectionString = "Data Source=localhost;port=3306;Initial Catalog=quartz;user id=root;password=root;Charset=utf8;Allow User Variables=True;Connect Timeout=60;SSL Mode=None";
        private SqlSugarClient _quartzClient;
        public SqlSugarClient QuartzClient
        {
            get
            {
                if (_quartzClient == null)
                {
                    _quartzClient = new SqlSugarClient(new ConnectionConfig()
                    {
                        ConnectionString = quartzConnectionString,
                        DbType = DbType.MySql,//设置数据库类型
                        IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                        InitKeyType = InitKeyType.SystemTable //从实体特性中读取主键自增列信息
                    });
                }
                return _quartzClient;
            }
        }




        //hanhancartoondb数据库---------------------------------------------
        private IHanHanCartoonDao _hanHanCartoonDao;
        public IHanHanCartoonDao HanHanCartoonDao
        {
            get
            {
                if (_hanHanCartoonDao == null)
                {
                    _hanHanCartoonDao = new HanHanCartoonDao(HanHanClient);
                }
                return _hanHanCartoonDao;
            }
        }

        private IHanHanCartoonChapterDao _hanHanCartoonChapterDao;
        public IHanHanCartoonChapterDao HanHanCartoonChapterDao
        {
            get
            {
                if (_hanHanCartoonChapterDao == null)
                {
                    _hanHanCartoonChapterDao = new HanHanCartoonChapterDao(HanHanClient);
                }
                return _hanHanCartoonChapterDao;
            }
        }

        private IHanHanCartoonPieceDao _hanHanCartoonPieceDao;
        public IHanHanCartoonPieceDao HanHanCartoonPieceDao
        {
            get
            {
                if (_hanHanCartoonPieceDao == null)
                {
                    _hanHanCartoonPieceDao = new HanHanCartoonPieceDao(HanHanClient);
                }
                return _hanHanCartoonPieceDao;
            }
        }

        private IHanHanCartoonPieceImgDao _hanHanCartoonPieceImgDao;
        public IHanHanCartoonPieceImgDao HanHanCartoonPieceImgDao
        {
            get
            {
                if (_hanHanCartoonPieceImgDao == null)
                {
                    _hanHanCartoonPieceImgDao = new HanHanCartoonPieceImgDao(HanHanClient);
                }
                return _hanHanCartoonPieceImgDao;
            }
        }


        //quartz数据库
        private ITaskLogDao _taskLogDao;
        public ITaskLogDao TaskLogDao
        {
            get
            {
                if (_taskLogDao == null)
                {
                    _taskLogDao = new TaskLogDao(QuartzClient);
                }
                return _taskLogDao;
            }
        }
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Dao.BaseDao.Impl
{
    public class BaseDao<T> : IBaseDao<T> where T : class, new()
    {
        private SqlSugarClient db;

        public BaseDao(SqlSugarClient sugarClient)
        {
            db = sugarClient;
        }
        public bool Delete(int id)
        {
            return db.Deleteable<T>().In(id).ExecuteCommand() > 0;
        }

        public bool Delete(List<int> ids)
        {
            return db.Deleteable<T>().In(ids).ExecuteCommand() > 0;
        }

        public ISugarQueryable<T> ExecuteSql(string sql)
        {
            return db.SqlQueryable<T>(sql);
        }

        public ISugarQueryable<T> GetQueryable()
        {
            return db.Queryable<T>();
        }

        public T Insert(T entity)
        {
            return db.Insertable(entity).ExecuteReturnEntity();
        }

        public bool Insert(List<T> entitys)
        {
            return db.Insertable(entitys).ExecuteCommand() > 0;
        }
    }


}

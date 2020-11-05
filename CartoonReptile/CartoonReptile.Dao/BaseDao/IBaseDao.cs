using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Dao.BaseDao
{
    public interface IBaseDao<T>
    {
        T Insert(T entity);
        bool Insert(List<T> entitys);
        bool Delete(int id);
        bool Delete(List<int> ids);
        ISugarQueryable<T> ExecuteSql(string sql);
        ISugarQueryable<T> GetQueryable();
    }
}

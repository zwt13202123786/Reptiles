using CartoonReptile.Dao.BaseDao.Impl;
using CartoonReptile.Model.HanHanCartoon;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Dao.HanHanCartoon.Impl
{
    public class HanHanCartoonChapterDao : BaseDao<HanHanCartoonChapterEntity>, IHanHanCartoonChapterDao
    {
        private SqlSugarClient db;
        public HanHanCartoonChapterDao(SqlSugarClient sugarClient) : base(sugarClient)
        {
            db = sugarClient;
        }
    }
}

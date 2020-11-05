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
    public class HanHanCartoonDao : BaseDao<HanHanCartoonEntity>, IHanHanCartoonDao
    {
        private SqlSugarClient db;
        public HanHanCartoonDao(SqlSugarClient sugarClient) : base(sugarClient)
        {
            db = sugarClient;
        }

        public bool Update(HanHanCartoonEntity entity)
        {
           return db.Updateable<HanHanCartoonEntity>()
                .SetColumnsIF(true, x => x.CollectionNumber == entity.CollectionNumber)
                .SetColumnsIF(true, x => x.EvaluateFraction == entity.EvaluateFraction)
                .SetColumnsIF(true, x => x.EvaluateNumber == entity.EvaluateNumber)
                .SetColumnsIF(true, x => x.State == entity.State)
                .SetColumnsIF(true, x => x.LastSyncTime == entity.LastSyncTime)
                .SetColumnsIF(true, x => x.Synopsis == entity.Synopsis)
                .SetColumnsIF(true, x => x.UpdateTime == entity.UpdateTime)
                .SetColumnsIF(true, x => x.WebCoverImgUrl == entity.WebCoverImgUrl)
                .SetColumnsIF(true, x => x.WebPageUrl == entity.WebPageUrl)
                .Where(it => it.Id == 11).ExecuteCommand()>0;
        }
    }
}

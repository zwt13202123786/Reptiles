using CartoonReptile.Dao.BaseDao;
using CartoonReptile.Model.HanHanCartoon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Dao.HanHanCartoon
{
    public interface IHanHanCartoonDao: IBaseDao<HanHanCartoonEntity>
    {
        bool Update(HanHanCartoonEntity entity);
    }
}

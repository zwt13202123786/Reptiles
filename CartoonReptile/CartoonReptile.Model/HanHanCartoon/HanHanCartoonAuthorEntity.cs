using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.HanHanCartoon
{
    /// <summary>
    /// 汗汗漫画作者关系表
    /// </summary>
    [SugarTable("HanHanCartoonAuthor")]
    public class HanHanCartoonAuthorEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 主键Id
        /// </summary>
        public int HanHanCartoonId { get; set; }
        /// <summary>
        /// 主键Id
        /// </summary>
        public int HanHanAuthorId { get; set; }
    }
}

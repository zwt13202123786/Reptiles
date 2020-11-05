
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.HanHanCartoon
{
    /// <summary>
    /// 汗汗漫画作者表
    /// </summary>
    [SugarTable("HanHanAuthor")]
    public class HanHanAuthorEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 主键Id
        /// </summary>
        public int AuthorName { get; set; }
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Synopsis { get; set; }
    }
}

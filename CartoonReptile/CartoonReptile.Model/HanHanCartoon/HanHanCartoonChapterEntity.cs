
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.HanHanCartoon
{
    /// <summary>
    /// 汗汗漫画章节表
    /// </summary>
    [SugarTable("HanHanCartoonChapter")]
    public class HanHanCartoonChapterEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 漫画表Id
        /// </summary>
        public int HanHanCartoonId { get; set; }
        /// <summary>
        /// 章节名
        /// </summary>
        public string ChapterName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 章节创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 章节修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}

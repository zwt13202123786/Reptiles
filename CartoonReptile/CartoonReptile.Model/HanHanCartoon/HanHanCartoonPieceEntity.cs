using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.HanHanCartoon
{
    /// <summary>
    /// 汗汗漫画话表
    /// </summary>
    [SugarTable("HanHanCartoonPiece")]
    public class HanHanCartoonPieceEntity
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
        /// 章节表Id
        /// </summary>
        public int HanHanCartoonChapterId { get; set; }
        /// <summary>
        /// 话名
        /// </summary>
        public string PieceName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 话创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 话修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.HanHanCartoon
{
    /// <summary>
    /// 汗汗漫画图片表
    /// </summary>
    [SugarTable("HanHanCartoonPieceImg")]
    public class HanHanCartoonPieceImgEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 话表
        /// </summary>
        public int HanHanCartoonPieceId { get; set; }
        /// <summary>
        /// 网页地址
        /// </summary>
        public string WebPageUrl { get; set; }
        /// <summary>
        /// 图片服务器地址 
        /// </summary>
        public string ImgHost { get; set; }
        /// <summary>
        /// 网页图片地址
        /// </summary>
        public string WebImgUrl { get; set; }
        /// <summary>
        /// 本地图片地址
        /// </summary>
        public string LocalhostImgUrl { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}

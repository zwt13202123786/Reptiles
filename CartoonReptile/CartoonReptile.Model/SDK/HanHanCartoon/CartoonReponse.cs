using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.SDK.HanHanCartoon
{
    /// <summary>
    /// 汗汗漫画漫画主页返回信息
    /// </summary>
    public class CartoonReponse
    {
        /// <summary>
        /// 漫画名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 封面图片地址
        /// </summary>
        public string CoverImgUrl { get; set; } 
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 收藏数量
        /// </summary>
        public int CollectionNumber { get; set; }
        /// <summary>
        /// 评价分数
        /// </summary>
        public decimal EvaluateFraction { get; set; } 
        /// <summary>
        /// 评价人数
        /// </summary>
        public int EvaluateNumber { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Synopsis { get; set; }
        /// <summary>
        /// 漫画章节详细
        /// </summary>
        public List<ChapterReponse> ChapterList { get; set; }
    }
}

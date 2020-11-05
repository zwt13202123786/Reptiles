using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.HanHanCartoon
{
    /// <summary>
    /// 汗汗漫画主表
    /// </summary>
    [SugarTable("HanHanCartoon")]
    public class HanHanCartoonEntity
    {
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if ((obj.GetType().Equals(this.GetType())) == false) return false;
            HanHanCartoonEntity entity = (HanHanCartoonEntity)obj;
            if (entity.WebPageUrl != this.WebPageUrl ||
                entity.State != this.State ||
                entity.WebCoverImgUrl != this.WebCoverImgUrl ||
                entity.EvaluateFraction != this.EvaluateFraction ||
                entity.EvaluateNumber != this.EvaluateNumber ||
                entity.CollectionNumber != this.CollectionNumber ||
                entity.UpdateTime != this.UpdateTime)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 主键Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 漫画名
        /// </summary>
        public string CartoonName { get; set; }
        /// <summary>
        /// 网页地址
        /// </summary>
        public string WebPageUrl { get; set; }
        /// <summary>
        /// 漫画状态：0-连载中，1-已完结
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 漫画作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 漫画封面图片路径
        /// </summary>
        public string WebCoverImgUrl { get; set; }
        /// <summary>
        /// 本地漫画封面图片路径
        /// </summary>
        public string LocalhostCoverImgUrl { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public decimal EvaluateFraction { get; set; }
        /// <summary>
        /// 评分人数
        /// </summary>
        public int EvaluateNumber { get; set; }
        /// <summary>
        /// 收藏数
        /// </summary>
        public int CollectionNumber { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Synopsis { get; set; }
        /// <summary>
        /// 漫画创建时间（本地）
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 漫画更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 最后同步时间
        /// </summary>
        public DateTime LastSyncTime { get; set; }
    }
}

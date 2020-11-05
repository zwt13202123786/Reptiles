using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.SDK.HanHanCartoon
{
    public class ChapterReponse
    {
        /// <summary>
        /// HanHanCartoonPieceEntity表Id
        /// </summary>
        public int PieceId { get; set; }

        /// <summary>
        /// 番剧名
        /// </summary>
        public string ChapterName { get; set; } 

        /// <summary>
        /// 集数名
        /// </summary>
        public string PieceName { get; set; } 

        /// <summary>
        /// 番剧网页地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 网页的链接，图片的链接
        /// </summary>
        public List<PieceReponse> ImgPieceList { get; set; } //网页的链接，图片的链接
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.SDK.HanHanCartoon
{
    public class PieceReponse
    {
        /// <summary>
        /// 图片的顺序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        ///图片服务器地址
        /// </summary>

        public string ImgHost { get; set; }

        /// <summary>
        /// 图片网页地址
        /// </summary>
        public string WebSiteUrl { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImgUrl { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.SDK.HanHanCartoon
{
    public enum CrawlStateEnum
    {
        /// <summary>
        /// 未抓取
        /// </summary>
        NoCrawl = 1,

        /// <summary>
        /// 抓取中
        /// </summary>
        CrawlIng = 2,

        /// <summary>
        /// 已抓取
        /// </summary>
        EndCrawl = 3,
    }
}

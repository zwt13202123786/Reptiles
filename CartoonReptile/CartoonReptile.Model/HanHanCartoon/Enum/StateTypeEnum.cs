using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartoonReptile.Model.HanHanCartoon.Enum
{
    public enum StateTypeEnum
    {
        /// <summary>
        /// 连载：0
        /// </summary>
        [Description("连载")]
        Serial = 0,

        /// <summary>
        /// 完结：1
        /// </summary>
        [Description("完结")]
        end = 1,
    }
}

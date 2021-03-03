using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 状态的比较方式
    /// </summary>
    public enum CompareType
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal=1,
        /// <summary>
        /// 大于
        /// </summary>
        Greater=2,
        /// <summary>
        /// 小于
        /// </summary>
        Fewer=3,
        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterEqual=4,
        /// <summary>
        /// 小于等于
        /// </summary>
        FewerEqual=5
    }
}

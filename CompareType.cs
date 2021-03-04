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
        /// 值等于限制
        /// </summary>
        Equal=1,
        /// <summary>
        /// 值大于限制
        /// </summary>
        Greater=2,
        /// <summary>
        /// 值小于限制
        /// </summary>
        Fewer=3,
        /// <summary>
        /// 值大于等于限制
        /// </summary>
        GreaterEqual=4,
        /// <summary>
        /// 值小于等于限制
        /// </summary>
        FewerEqual=5
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 报警等级划分
    /// </summary>
    public enum AlertLevel
    {
        /// <summary>
        /// 无报警
        /// </summary>
        None=0,
        /// <summary>
        /// 警告
        /// </summary>
        Warm=1,
        /// <summary>
        /// 报警
        /// </summary>
        Alarm=2
    }
}

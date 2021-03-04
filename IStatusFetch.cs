using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 获取状态接口
    /// </summary>
    public interface IStatusFetch
    {
        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns>可进行数值大小比较的状态值</returns>
        IComparable FetchStatus();
    }
}

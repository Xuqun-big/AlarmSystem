using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 值比较的接口
    /// </summary>
    public interface ICompareBase
    {
        /// <summary>
        /// 做大小或等值比较
        /// </summary>
        /// <param name="curValue">要进行比较的值</param>
        /// <returns>比较是否符合要求</returns>
        bool DoCompare(IComparable curValue);
    }
}

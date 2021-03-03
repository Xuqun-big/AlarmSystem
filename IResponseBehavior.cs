using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 响应报警的接口
    /// </summary>
    public interface IResponseBehavior
    {
        /// <summary>
        /// 执行响应动作
        /// </summary>
        void OnCommand();
        /// <summary>
        /// 撤销响应动作
        /// </summary>
        void Undo();
    }
}

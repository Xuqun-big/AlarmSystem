using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 单个监控对象报警触发
    /// </summary>
    /// <param name="fetch">单个报警监控对象</param>
    /// <param name="BeforValue">报警前值</param>
    /// <param name="NewValue">报警后值</param>
    public delegate void RaiseSingleAlertTrigged(IStatusFetch fetch,IComparable NewValue);
    /// <summary>
    /// 单个报警结束事件
    /// </summary>
    /// <param name="fetch">单个报警监控对象</param>
    /// <param name="BeforValue">报警时间</param>
    /// <param name="NewValue">结束时间</param>
    public delegate void RaiseSingleAlertEnd(IStatusFetch fetch, DateTime BeforValue);
    /// <summary>
    /// 整个报警系统报警触发
    /// </summary>
    /// <param name="engine">报警系统</param>
    /// <param name="level">报警级别</param>
    public delegate void RaiseEngineAlertTrigged(AlertEngine engine,AlertLevel level);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 报警触发数据模型
    /// </summary>
    public class TriggerModel
    {
        private IStatusFetch fetch = null;
        private StatusTrigger trigger = null;
        private AlertLevel oldLevel = AlertLevel.None;
        private DateTime oldTime = DateTime.Now;
        /// <summary>
        /// 用于状态获取的对象
        /// </summary>
        public IStatusFetch Fetch { get => fetch; set => fetch = value; }
        /// <summary>
        /// 用于判断状态的对象
        /// </summary>
        public StatusTrigger Trigger { get => trigger; set => trigger = value; }
        /// <summary>
        /// 上一次的报警等级
        /// </summary>
        public AlertLevel OldLevel { get => oldLevel; set => oldLevel = value; }
        /// <summary>
        /// 上一次的报警时刻
        /// </summary>
        public DateTime OldTime { get => oldTime; set => oldTime = value; }
    }
}

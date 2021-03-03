using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 单个报警类
    /// </summary>
    public abstract class AlertEngine
    {
        #region 私有字段
        private readonly Dictionary<AlertLevel, List<IResponseBehavior>> Response = new Dictionary<AlertLevel, List<IResponseBehavior>>();
        private readonly List<IResponseBehavior> WarmResponse = new List<IResponseBehavior>();
        private readonly List<IResponseBehavior> AlarmResponse = new List<IResponseBehavior>();
        #endregion
        #region 公共方法
        /// <summary>
        /// 给指定等级添加一个报警行为
        /// </summary>
        /// <param name="level">报警等级</param>
        /// <param name="behavior">报警后要执行的行为</param>
        public void AddResponse(AlertLevel level,IResponseBehavior behavior)
        {
            if (level == AlertLevel.None)
                return;
            if (!Response[level].Contains(behavior))
                Response[level].Add(behavior);
        }
        /// <summary>
        /// 移除一个指定等级的报警行为
        /// </summary>
        /// <param name="level">报警等级</param>
        /// <param name="behavior">报警后要执行的行为</param>
        public void RemoveResponse(AlertLevel level,IResponseBehavior behavior)
        {
            if (level == AlertLevel.None)
                return;
            if (Response[level].Contains(behavior))
                Response[level].Remove(behavior);
        }
        /// <summary>
        /// 初始化报警系统
        /// </summary>
        public virtual void InitialEngine()
        {
            Response.Add(AlertLevel.Warm, WarmResponse);
            Response.Add(AlertLevel.Alarm, AlarmResponse);
        }
        /// <summary>
        /// 启动报警系统
        /// </summary>
        public abstract void StartEngine();
        /// <summary>
        /// 停止报警系统
        /// </summary>
        public abstract void StopEngine();
        public AlertEngine()
        {
            InitialEngine();
        }
        #endregion
    }
}

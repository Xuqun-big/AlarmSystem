using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 报警分级响应模型
    /// </summary>
    public class ResponseModel
    {
        private AlertLevel level = AlertLevel.None;
        private List<IResponseBehavior> responses = new List<IResponseBehavior>();
        private bool savedResponse = false;
        /// <summary>
        /// 报警等级
        /// </summary>
        public AlertLevel Level { get => level; set => level = value; }
        /// <summary>
        /// 要响应的事件
        /// </summary>
        public List<IResponseBehavior> Responses { get => responses; set => responses = value; }
        /// <summary>
        /// 此等级的报警是否已经响应过
        /// </summary>
        public bool SavedResponse { get => savedResponse; set => savedResponse = value; }

    }
}

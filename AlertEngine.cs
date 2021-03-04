using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 单个报警类
    /// 每个实例中触发的所有报警响应行为都是一致的
    /// </summary>
    public abstract class AlertEngine
    {
        #region 私有字段
        private readonly Dictionary<AlertLevel, List<IResponseBehavior>> Response = new Dictionary<AlertLevel, List<IResponseBehavior>>();
        private readonly List<IResponseBehavior> WarmResponse = new List<IResponseBehavior>();
        private readonly List<IResponseBehavior> AlarmResponse = new List<IResponseBehavior>();
        private readonly Dictionary<IStatusFetch, StatusTrigger> Trigger=new Dictionary<IStatusFetch, StatusTrigger>();
        private readonly Dictionary<IStatusFetch, AlertLevel> SavedValue = new Dictionary<IStatusFetch, AlertLevel>();
        private readonly Dictionary<IStatusFetch, DateTime> SavedTime = new Dictionary<IStatusFetch, DateTime>();
        private readonly Dictionary<AlertLevel, bool> SavedResponse = new Dictionary<AlertLevel, bool>();
        private bool engineStatue = false;
        private int milliSeconds = 0;
        private CancellationTokenSource Cancellation = new CancellationTokenSource();
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
        /// 添加一个报警触发器
        /// </summary>
        /// <param name="fetch">获取报警条件的对象</param>
        /// <param name="trigger">根据报警对象触发报警等级的对象</param>
        public void AddTrigger(IStatusFetch fetch,StatusTrigger trigger)
        {
            Trigger[fetch] = trigger;
        }
        /// <summary>
        /// 初始化报警系统
        /// </summary>
        public virtual void InitialEngine()
        {
            Response.Add(AlertLevel.Warm, WarmResponse);
            Response.Add(AlertLevel.Alarm, AlarmResponse);
            SavedResponse.Add(AlertLevel.Warm,false);
            SavedResponse.Add(AlertLevel.Alarm,false);
        }
        /// <summary>
        /// 当前报警系统的运行状态
        /// </summary>
        public virtual bool EngineStatus {
            get =>engineStatue; 
        }
        /// <summary>
        /// 单次扫描耗时ms
        /// </summary>
        public virtual int MilliSeconds {
            get=>milliSeconds;
        }
        public event RaiseSingleAlertTrigged SingleAlertTrigged;
        public event RaiseEngineAlertTrigged EngineAlertTrigged;
        public event RaiseSingleAlertEnd SingleAlertEnd;
        /// <summary>
        /// 启动报警系统
        /// </summary>
        public virtual void StartEngine()
        {
            engineStatue = true;
            Task.Factory.StartNew(() =>
            {
                while (engineStatue)
                {
                    DateTime t1 = DateTime.Now;
                    Task t = EngineTask(Cancellation);
                    Task.WaitAll(t);
                    milliSeconds = (int)(DateTime.Now - t1).TotalMilliseconds;
                    t = DoResponse();
                    Task.WaitAny(t);
                }
            });
        }
        private async Task DoResponse()
        {
            await Task.Run(()=>
            { 
                foreach (var s in Response)
                {
                    bool res = false;
                    if (SavedValue.Values.Contains(s.Key))
                    {
                        res = true;
                    }
                    if (res != SavedResponse[s.Key])
                    {
                        if (res)
                        {
                            foreach (var p in s.Value)
                                Task.Factory.StartNew(() =>
                                {
                                    p.OnCommand();
                                });
                        }
                        else
                        {
                            foreach (var p in s.Value)
                                Task.Factory.StartNew(() => {
                                    p.Undo();
                                });
                        }
                    }
                    SavedResponse[s.Key] = res;
                }
            });
        }
        private async Task EngineTask(CancellationTokenSource cancellationToken)
        {
            await Task.Run(() =>
            {
                foreach (var s in Trigger)
                {
                    var value = s.Key.FetchStatus();
                    AlertLevel level = s.Value.GetAlertLevel(value);
                    if (SavedValue.ContainsKey(s.Key))
                    {
                        if (SavedValue[s.Key] != level)
                        {
                            if (level == AlertLevel.None)
                            {
                                Task.Factory.StartNew(
                                    () => SingleAlertTrigged?.Invoke(s.Key,SavedTime[s.Key],DateTime.Now)
                                    );
                                
                            }
                            else
                            {
                                if(SavedValue[s.Key]!=AlertLevel.None)
                                    SavedTime[s.Key] = DateTime.Now;
                                Task.Factory.StartNew(()=> SingleAlertTrigged?.Invoke(s.Key, AlertLevel.None, level));
                            }
                            SavedValue[s.Key] = level;
                        }
                    }
                    else
                    {
                        SavedValue[s.Key] = level;
                        if (level != AlertLevel.None)
                        {
                            SavedTime[s.Key] = DateTime.Now;
                            Task.Factory.StartNew(() =>
                            {
                                SingleAlertTrigged?.Invoke(s.Key, AlertLevel.None, level);
                            });
                        }
                    }
                    if (cancellationToken.IsCancellationRequested)
                        break;
                }
            });
        }
        /// <summary>
        /// 停止报警系统
        /// </summary>
        public virtual void StopEngine()
        {
            engineStatue = false;
            Cancellation.Cancel();
            Cancellation = new CancellationTokenSource();
        }
        public AlertEngine()
        {
            InitialEngine();
        }
        #endregion
    }
}

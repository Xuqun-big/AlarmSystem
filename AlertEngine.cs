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
    /// 每个实例中触发的所有同一级别的报警时，所有的响应行为都是一致的
    /// </summary>
    public class AlertEngine
    {
        #region 私有字段
        //private readonly Dictionary<AlertLevel, List<IResponseBehavior>> Response = new Dictionary<AlertLevel, List<IResponseBehavior>>();
        //private readonly List<IResponseBehavior> WarmResponse = new List<IResponseBehavior>();
        //private readonly List<IResponseBehavior> AlarmResponse = new List<IResponseBehavior>();
        //private readonly Dictionary<IStatusFetch, StatusTrigger> Trigger=new Dictionary<IStatusFetch, StatusTrigger>();
        //private readonly Dictionary<IStatusFetch, AlertLevel> SavedValue = new Dictionary<IStatusFetch, AlertLevel>();
        //private readonly Dictionary<IStatusFetch, DateTime> SavedTime = new Dictionary<IStatusFetch, DateTime>();
        //private readonly Dictionary<AlertLevel, bool> SavedResponse = new Dictionary<AlertLevel, bool>();
        private readonly List<TriggerModel> triggerModels = new List<TriggerModel>();
        private readonly List<ResponseModel> responseModels = new List<ResponseModel>();
        private bool engineStatue = false;
        private int milliSeconds = 0;
        private CancellationTokenSource Cancellation = new CancellationTokenSource();
        private string name="";
        #endregion
        #region 公共方法
        /// <summary>
        /// 报警系统的名称
        /// </summary>
        public virtual string Name { get => name; set => name = value; }
        /// <summary>
        /// 给指定等级添加一个报警行为
        /// </summary>
        /// <param name="level">报警等级</param>
        /// <param name="behavior">报警后要执行的行为</param>
        public virtual void AddResponse(AlertLevel level,IResponseBehavior behavior)
        {
            if (level == AlertLevel.None)
                return;
            ResponseModel s= responseModels.FirstOrDefault(x => x.Level == level);
            if (!s.Responses.Contains(behavior))
                s.Responses.Add(behavior);
        }
        /// <summary>
        /// 移除一个指定等级的报警行为
        /// </summary>
        /// <param name="level">报警等级</param>
        /// <param name="behavior">报警后要执行的行为</param>
        public virtual void RemoveResponse(AlertLevel level,IResponseBehavior behavior)
        {
            if (level == AlertLevel.None)
                return;
            ResponseModel s = responseModels.FirstOrDefault(x => x.Level == level);
            if (s.Responses.Contains(behavior))
                s.Responses.Remove(behavior);
        }
        /// <summary>
        /// 添加一个报警触发器
        /// </summary>
        /// <param name="fetch">获取报警条件的对象</param>
        /// <param name="trigger">根据报警对象触发报警等级的对象</param>
        public virtual void AddTrigger(IStatusFetch fetch,StatusTrigger trigger)
        {
            TriggerModel s = triggerModels.FirstOrDefault(x => x.Fetch == fetch);
            if (s != null)
                s.Trigger = trigger;
            else
            {
                s = new TriggerModel() { Fetch = fetch, Trigger = trigger };
                triggerModels.Add(s);
            }
        }
        /// <summary>
        /// 移除一个报警触发器
        /// </summary>
        /// <param name="fetch">获取报警条件的对象</param>
        public virtual void RemoveTrigger(IStatusFetch fetch)
        {
            TriggerModel s = triggerModels.FirstOrDefault(x=>x.Fetch==fetch);
            if (s != null)
                triggerModels.Remove(s);
        }
        /// <summary>
        /// 初始化报警系统
        /// </summary>
        public virtual void InitialEngine()
        {
            foreach (var s in Enum.GetValues(typeof(AlertLevel)))
            {
                AlertLevel level = (AlertLevel)s;
                if (level != AlertLevel.None)
                    responseModels.Add(new ResponseModel() { Level=level});
            }
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
                    t = DoResponse();
                    Task.WaitAny(t);
                    milliSeconds = (int)(DateTime.Now - t1).TotalMilliseconds;
                }
            });
        }
        /// <summary>
        /// 根据报警等级作出报警响应的线程
        /// </summary>
        /// <returns></returns>
        private async Task DoResponse()
        {
            await Task.Run(()=>
            { 
                foreach (var s in responseModels)
                {
                    bool res = false;
                    if (triggerModels.Exists(x=>x.OldLevel==s.Level))
                    {
                        res = true;
                    }
                    if (res != s.SavedResponse)
                    {
                        if (res)
                        {
                            foreach (var p in s.Responses)
                                Task.Factory.StartNew(() =>
                                {
                                    p.OnCommand();
                                });
                            EngineAlertTrigged?.Invoke(this,s.Level);
                        }
                        else
                        {
                            foreach (var p in s.Responses)
                                Task.Factory.StartNew(() => {
                                    p.Undo();
                                });
                        }
                    }
                    s.SavedResponse = res;
                }
            });
        }
        /// <summary>
        /// 扫描所有报警触发器的线程
        /// </summary>
        /// <param name="cancellationToken">取消扫描</param>
        /// <returns></returns>
        private async Task EngineTask(CancellationTokenSource cancellationToken)
        {
            await Task.Run(() =>
            {
                foreach (var s in triggerModels)
                {
                    var value = s.Fetch.FetchStatus();
                    AlertLevel level = s.Trigger.GetAlertLevel(value);
                    if (s.OldLevel != level)
                    {
                        if (level == AlertLevel.None)
                        {
                            Task.Factory.StartNew(
                                () => SingleAlertEnd?.Invoke(s.Fetch,s.OldTime)
                                );
                        }
                        else
                        {
                            if(s.OldLevel==AlertLevel.None)
                                s.OldTime = DateTime.Now;
                            Task.Factory.StartNew(()=> SingleAlertTrigged?.Invoke(s.Fetch, value));
                        }
                        s.OldLevel = level;
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

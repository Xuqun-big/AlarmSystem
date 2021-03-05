using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hans.MV.Alarm
{
    /// <summary>
    /// 状态比较器，用于触发当前的报警等级
    /// </summary>
    public class StatusTrigger
    {
        private readonly List<AlertCompare> compares = new List<AlertCompare>();
        /// <summary>
        /// 获取当前的报警等级
        /// </summary>
        /// <param name="status">值的状态</param>
        /// <returns>报警等级</returns>
        public virtual AlertLevel GetAlertLevel(IComparable curValue)
        { 
            foreach(var s in compares)
            {
                var b =s.DoLevelCompare(curValue);
                if (b != AlertLevel.None)
                    return b;
            }
            return AlertLevel.None;
        }
        /// <summary>
        /// 添加一个范围比较参数
        /// </summary>
        /// <param name="base1">传入一个基础比较单元</param>
        /// <param name="level">比较成功后触发的报警等级</param>
        public virtual void AddParam(ICompareBase base1, AlertLevel level)
        {
            AlertCompare singleCompare = new AlertCompare();
            singleCompare.Base1 = (BaseCompare)base1;
            singleCompare.Level = level;
            compares.Add(singleCompare);
        }
        /// <summary>
        /// 添加一个范围比较参数
        /// 实现了ICompareBase的类型
        /// </summary>
        /// <param name="base1">传入第一个基础比较单元</param>
        /// <param name="base2">传入第二个基础比较单元</param>
        /// <param name="level">比较成功后触发的报警等级</param>
        public virtual void AddParam(ICompareBase base1, ICompareBase base2, AlertLevel level)
        {
            AlertCompare singleCompare = new AlertCompare();
            singleCompare.Base1 = (BaseCompare)base1;
            singleCompare.Base2 = (BaseCompare)base2;
            singleCompare.Level = level;
            compares.Add(singleCompare);
        }
        public virtual void ClearParam()
        { 
            compares.Clear();
        }
        /// <summary>
        /// 用于大小比较的基础结构体
        /// </summary>
        public struct BaseCompare:ICompareBase
        {
            public CompareType comparetype;
            public IComparable compareValue;
            public bool DoCompare(IComparable curValue)
            {
                if(curValue.GetType().Equals(compareValue.GetType()))
                    switch (comparetype) {
                        case CompareType.Equal:
                            if (curValue.CompareTo(compareValue) ==0)
                                return true;
                            break;
                        case CompareType.Greater:
                            if (curValue.CompareTo(compareValue) <0)
                                return true;
                            break;
                        case CompareType.Fewer:
                            if (curValue.CompareTo(compareValue) > 0)
                                return true;
                            break;
                        case CompareType.GreaterEqual:
                            if (curValue.CompareTo(compareValue) <= 0)
                                return true;
                            break;
                        case CompareType.FewerEqual:
                            if (curValue.CompareTo(compareValue) >= 0)
                                return true;
                            break;
                    }
                return false;
            }

        }
        /// <summary>
        /// 用于报警比较的数据结构体
        /// </summary>
        private class AlertCompare
        {
            public Nullable<BaseCompare> Base1;
            public Nullable<BaseCompare> Base2;
            public AlertLevel Level;
            public AlertLevel DoLevelCompare(IComparable curValue)
            {
                if (Base1 != null && Base2 != null)
                {
                    if (((BaseCompare)Base1).DoCompare(curValue) && ((BaseCompare)Base2).DoCompare(curValue))
                        return Level;
                }
                else if (Base1 != null)
                {
                    if (((BaseCompare)Base1).DoCompare(curValue))
                        return Level;
                }
                else if (Base2 != null)
                {
                    if (((BaseCompare)Base2).DoCompare(curValue))
                        return Level;
                }
                return AlertLevel.None;
            }
        }
    }
}

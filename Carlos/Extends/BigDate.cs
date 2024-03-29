﻿using System;
using System.Linq;
using Carlos.Exceptions;
using System.Diagnostics;
using Carlos.Enumerations;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Carlos.Extends
{
    /// <summary>
    /// 大型日期类，可以表示跨度很长的日期。
    /// </summary>
    /// <remarks>当前类用于表示一个跨度很长的日期，但是这个类并不能表示一个具体的时间，换句话说，该类的分度值为Day。另外，这个大型日期类是建立在一个自然科学的理想假设限制之上的（即地球公转和自转不不变化的情况下），就是这个类已经忽略掉了地球公转和自转的速率变化，这个变化在自然科学中而言是长久性和可持续性的，就目前而言，地球自转暂时保持在23h56m04s这个周期上，当然这些知识会涉及到天体物理学，静态宇宙模型以及相对论相关的知识，如果需要禁止忽略这些因素，可以参考地球公转和自转速率或者天体物理学和相对论等相关的百科知识。</remarks>
    [DebuggerDisplay("Date = {ToString(DateDisplayCategory.DashedSegmentation)}")]
    public class BigDate
    {
        private int _year;//当前日期的年份，范围是公元前6000至公元2147483647年。
        private byte _month;//当前日期的月份，范围是1至12。
        private byte _day;//当前月份的天，范围是1至month的天数。
        /// <summary>
        /// 构造函数，初始化一个包含了当前日期的BigDate实例。
        /// </summary>
        public BigDate()
        {
            _year = DateTime.Now.Year;
            _month = (byte)DateTime.Now.Month;
            _day = (byte)DateTime.Now.Day;
        }
        /// <summary>
        /// 构造函数，通过一个适用于CLR的DateTime类型的Ticks来初始化当前BigDate实例。
        /// </summary>
        /// <param name="clrDateTimeTicks">可用于初始化System.DateTime的Ticks（即适用于CLR的DateTime类型的Ticks）。</param>
        public BigDate(long clrDateTimeTicks)
        {
            DateTime time = new DateTime(clrDateTimeTicks);
            _year = time.Year;
            _month = (byte)time.Month;
            _day = (byte)time.Day;
        }
        /// <summary>
        /// 构造函数，通过一个指定的DateTime结构体实例来初始化当前BigDate实例。
        /// </summary>
        /// <param name="date">指定的DateTime结构体实例。</param>
        public BigDate(DateTime date)
        {
            _year = date.Year;
            _month = (byte)date.Month;
            _day = (byte)date.Day;
        }
        /// <summary>
        /// 构造函数，通过指定的年月日来初始化当前的BigDate实例。
        /// </summary>
        /// <param name="year">指定的年份，范围是公元前6000至公元2147483647年。</param>
        /// <param name="month">指定的月份，范围是1至12。</param>
        /// <param name="day">指定的天数，范围是1至month的天数。</param>
        /// <exception cref="DateOverflowException">如果当年月日仍以一个参数设置超出了范围，则将会抛出这个异常。</exception>
        public BigDate(int year, byte month, byte day)
        {
            if (year < MinimumYear && year > MaximumYear) throw new DateOverflowException("年份超出范围！");
            if (month < 1 && month > 12) throw new DateOverflowException("月份超出范围！");
            if (day < 1 && day > MaximumDays) throw new DateOverflowException("天数超出当前月份所包含天数的范围");
            _year = year;
            _month = month;
            _day = day;
        }
        /// <summary>
        /// 获取BigDate类型能表示的最大日期。
        /// </summary>
        public static BigDate MaximumDate => new BigDate(MaximumYear, 12, 31);
        /// <summary>
        /// 获取BigDate类型能表示的最小日期。
        /// </summary>
        public static BigDate MinimumDate => new BigDate(MinimumYear, 1, 1);
        /// <summary>
        /// 获取BigDate能够表示的最大年份。
        /// </summary>
        public static int MaximumYear => int.MaxValue;
        /// <summary>
        /// 获取BigDate能够表示的最小年份。
        /// </summary>
        public static int MinimumYear => -6000;
        /// <summary>
        /// 获取当前实例中指定的月份的最大天数。
        /// </summary>
        public byte MaximumDays
        {
            get
            {
                byte maxDays = 30;
                switch (Month)
                {
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        maxDays = 31;
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        maxDays = 30;
                        break;
                    case 2:
                        if (IsLeap) maxDays = 29;
                        else maxDays = 28;
                        break;
                }
                return maxDays;
            }
        }
        /// <summary>
        /// 获取或设置当前实例的年份。
        /// </summary>
        /// <exception cref="DateOverflowException">如果当年月日仍以一个参数设置超出了范围，则将会抛出这个异常。</exception>
        public int Year
        {
            get => _year;
            set
            {
                if (value < MinimumYear && value > MaximumYear) throw new DateOverflowException("年份超出范围！");
                _year = value;
            }
        }
        /// <summary>
        /// 获取或设置当前实例的月份。
        /// </summary>
        /// <exception cref="DateOverflowException">如果当年月日仍以一个参数设置超出了范围，则将会抛出这个异常。</exception>
        public byte Month
        {
            get => _month;
            set
            {
                if (value < 1 && value > 12) throw new DateOverflowException("月份超出范围！");
                _month = value;
            }
        }
        /// <summary>
        /// 获取或设置当前实例中这个月份所显示的天数。
        /// </summary>
        /// <exception cref="DateOverflowException">如果当年月日仍以一个参数设置超出了范围，则将会抛出这个异常。</exception>
        public byte Day
        {
            get => _day;
            set
            {
                if (value < 1 && value > MaximumDays) throw new DateOverflowException("天数超出当前月份所包含天数的范围");
                _day = value;
            }
        }
        /// <summary>
        /// 获取当前实例的日期是从-6000年01月01日开始的第几天。
        /// </summary>
        public ulong DaysBeginWithZero
        {
            get
            {
                int leapyear_count = 0;
                ulong days = (ulong)Math.Abs(MinimumYear) * 365 + (ulong)Year * 365;
                if (IsLeap) days += (ulong)new DateTime(2008, Month, Day).DayOfYear;
                else days += (ulong)new DateTime(2010, Month, Day).DayOfYear;
                for (int i = MinimumYear; i <= Year; i++)
                {
                    if (new BigDate(i, 1, 1).IsLeap) ++leapyear_count;
                }
                return days + (ulong)leapyear_count;
            }
        }
        /// <summary>
        /// 判定当前实例所表示的年份是否属于闰年。
        /// </summary>
        /// <returns>如果当前日期表示的年份是闰年，则返回true，否则返回false。</returns>
        public bool IsLeap => DateTime.IsLeapYear(Year);
        /// <summary>
        /// 将两个年份按照从小到大的顺序排列。
        /// </summary>
        /// <param name="diff">指定的BigDate实例。</param>
        /// <returns>若执行成功，则会返回一个有顺序的年份列表。</returns>
        public List<int> CompareOfYear(BigDate diff) => Year >= diff.Year ? new int[] { diff.Year, Year }.ToList() : new int[] { Year, diff.Year }.ToList();
        /// <summary>
        /// 将两个日期按照年份从小到大的顺序排列。
        /// </summary>
        /// <param name="diff">指定的BigDate实例。</param>
        /// <returns>若执行成功，则会返回一个有顺序的DateTime年份列表。</returns>
        public List<DateTime> CompareOfDateTime(BigDate diff)
        {
            DateTime dt01 = new DateTime(diff.Year, diff.Month, diff.Day);
            DateTime dt02 = new DateTime(Year, Month, Day);
            return Year >= diff.Year ? new DateTime[] { dt01, dt02 }.ToList() : new DateTime[] { dt02, dt01 }.ToList();
        }
        /// <summary>
        /// 获取指定范围之间的闰年年份。
        /// </summary>
        /// <param name="range">指定的范围上限或者下限。</param>
        /// <returns>如果函数无异常，则会返回一个范围内的闰年集合。</returns>
        /// <exception cref="DifferenceTooSmallException">指定的年份范围过小时，则会抛出这个异常。</exception>
        public List<int> GetLeaps(BigDate range)
        {
            List<int> leaps = new List<int>();
            if (Math.Abs(CompareOfYear(range)[0] - CompareOfYear(range)[1]) < 2)
            {
                throw new DifferenceTooSmallException("指定范围的年份差的绝对值不能小于2！");
            }
            Parallel.For(CompareOfYear(range)[0], CompareOfYear(range)[1] + 1, (index) =>
            {
                if (new BigDate(index, 1, 1).IsLeap) leaps.Add(index);
            });
            leaps.Sort();
            return leaps;
        }
        /// <summary>
        /// 计算指定日期与当前日期之间的天数差。
        /// </summary>
        /// <param name="diff">指定的与当前日期有差别的日期。</param>
        /// <returns>如果这个方法未抛出异常，则会返回两个日期之间的天数差的UInt64表示形式。</returns>
        public ulong GetDifferenceOfDay(BigDate diff) => (ulong)Math.Abs(DaysBeginWithZero - (decimal)diff.DaysBeginWithZero);
        /// <summary>
        /// 获取当前实例的指定格式的字符串。
        /// </summary>
        /// <param name="category">指定的格式，即显示方式。</param>
        /// <returns>该操作将会返回这个实例的字符串形式，但是这个字符串是一个有使用意义的字符串。</returns>
        public string ToString(DateDisplayCategory category)
        {
            string datest = category switch
            {
                DateDisplayCategory.OnlySerial => $"{Year}{Month:D2}{Day:D2}",
                DateDisplayCategory.SolidusSegmentation => $"{Year}/{Month:D2}/{Day:D2}",
                DateDisplayCategory.DashedSegmentation => $"{Year}-{Month:D2}-{Day:D2}",
                DateDisplayCategory.PointSegmentation => $"{Year}.{Month:D2}.{Day:D2}",
                DateDisplayCategory.WavyLineSegmentation => $"{Year}~{Month:D2}~{Day:D2}",
                DateDisplayCategory.ChineseSegmentation => $"{Year}年{Month:D2}月{Day:D2}日",
                _ => $"{Year}{Month:D2}{Day:D2}",
            };
            return datest;
        }
        /// <summary>
        /// 获取当前实例的字符串表达形式，其日期的年月日用指定分隔符进行分割。
        /// </summary>
        /// <param name="separator">用于分割年月日的分隔字符。</param>
        /// <returns>
        /// 该操作将会返回一个用separator字符分割的字符串形式的未格式化的大型日期，如果这个分隔字符为“-”，这将会返回YYYY-MM-DD这样的日期字符串。
        /// </returns>
        public string ToString(char separator) => ToString(separator, false);
        /// <summary>
        /// 获取当前实例的字符串表达形式，其日期的年月日用指定分隔符进行分割，并确定是否对Month和Day进行单位数补零的操作。
        /// </summary>
        /// <param name="separator">用于分割年月日的分隔字符。</param>
        /// <param name="isFormat">是否格式化Month和Day，即是否对这两个属性进行单位数补零的操作。</param>
        /// <returns>
        /// 该操作将会返回一个用separator字符分割的字符串形式的大型日期，如果这个分隔字符为“-”，这将会返回YYYY-MM-DD这样的日期字符串。
        /// </returns>
        public string ToString(char separator, bool isFormat)
        {
            if (isFormat) return $"{Year}{separator}{Month:D2}{separator}{Day:D2}";
            else return $"{Year}{separator}{Month}{separator}{Day}";
        }
        /// <summary>
        /// 获取当前类的字符串表达形式。
        /// </summary>
        /// <returns>该操作返回当前类的字符串表达形式，这个字符串是当前类的完整名称。</returns>
        public override string ToString() => GetType().FullName;
    }

}

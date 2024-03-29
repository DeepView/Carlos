﻿using System;
namespace Carlos.Extends
{
    /// <summary>
    /// 表示一个32位整数的范围结构。
    /// </summary>
    public struct Int32Range : IEquatable<Int32Range>
    {
        /// <summary>
        /// 构造函数，根据下限值、上限值、最低下限值是否存在于数学包含和最高上限是否存在于数学包含这四个参数创建一个32位整数的范围结构。
        /// </summary>
        /// <param name="lower">指定的下限值。</param>
        /// <param name="upper">指定的上限值。</param>
        /// <param name="isIncludeLower">在数学意义上，是否包含这个下限值，作为可选参数，其默认值为true。</param>
        /// <param name="isIncludeUpper">在数学意义上，是否包含这个上限值，作为可选参数，其默认值为true。</param>
        public Int32Range(int lower, int upper, bool isIncludeLower = true, bool isIncludeUpper = true)
        {
            Lower = lower;
            Upper = upper;
            IsIncludeLower = isIncludeLower;
            IsIncludeUpper = isIncludeUpper;
        }
        /// <summary>
        /// 获取或设置当前结构的下限值。
        /// </summary>
        public int Lower { get; set; }
        /// <summary>
        /// 获取或设置当前结构的上限值。
        /// </summary>
        public int Upper { get; set; }
        /// <summary>
        /// 获取或设置该范围在数学意义上是否包含了下限值。
        /// </summary>
        public bool IsIncludeLower { get; set; }
        /// <summary>
        /// 获取或设置该范围在数学意义上是否包含了上限值。
        /// </summary>
        public bool IsIncludeUpper { get; set; }
        /// <summary>
        /// 获取一个包含无符号字节数据类型的Int32Range实例。
        /// </summary>
        public static Int32Range RangeOfByte => new Int32Range(byte.MinValue, byte.MaxValue);
        /// <summary>
        /// 获取一个包含字节数据类型的Int32Range实例。
        /// </summary>
        public static Int32Range RangeOfSByte => new Int32Range(sbyte.MinValue, sbyte.MaxValue);
        /// <summary>
        /// 获取一个包含无符号16位整形数据类型的Int32Range实例。
        /// </summary>
        public static Int32Range RangeOfUInt16 => new Int32Range(ushort.MinValue, ushort.MaxValue);
        /// <summary>
        /// 获取一个包含16位整形数据类型的Int32Range实例。
        /// </summary>
        public static Int32Range RangeOfInt16 => new Int32Range(short.MinValue, short.MaxValue);
        /// <summary>
        /// 获取一个包含32位整形数据类型的Int32Range实例。
        /// </summary>
        public static Int32Range RangeOfInt32 => new Int32Range(int.MinValue, int.MaxValue);
        /// <summary>
        /// 判断参数x是否在当前结构所表示的范围内。
        /// </summary>
        /// <param name="x">需要被判断集合关系的32位整数。</param>
        /// <returns>该操作作出判断之后，如果该参数x属于当前范围，则返回true，否则返回false。</returns>
        public bool In(int x)
        {
            bool include = false;
            if (IsIncludeLower && IsIncludeUpper)
            {
                if (x >= Lower && x <= Upper) include = true;
                else include = false;
            }
            else if (!IsIncludeLower && !IsIncludeUpper)
            {
                if (x > Lower && x < Upper) include = true;
                else include = false;
            }
            else if (IsIncludeLower && !IsIncludeUpper)
            {
                if (x >= Lower && x < Upper) include = true;
                else include = false;
            }
            else if (!IsIncludeLower && IsIncludeUpper)
            {
                if (x > Lower && x <= Upper) include = true;
                else include = false;
            }
            return include;
        }
        /// <summary>
        /// 指示当前的范围结构的上下限是否符合数学规范，即上限是否大于等于下限。
        /// </summary>
        /// <returns>如果当前实例的上下限符合数学规范，则返回true，否则返回false。</returns>
        public bool IsCorrect() => Lower < Upper;
        /// <summary>
        /// 获取当前范围实例的上下限的极差绝对值。
        /// </summary>
        /// <returns>该操作将会返回一个以Int32数据类型为格式的上下限极差绝对值。</returns>
        public int GetDisparityAbs() => Math.Abs(Upper - Lower);
        /// <summary>
        /// 判断当前结构与另一个Int32Range结构是否相同。
        /// </summary>
        /// <param name="other">另一个Int32Range结构。</param>
        /// <returns>如果这两个Int32Range结构相同，则返回true，否则返回false。</returns>
        public bool Equals(Int32Range other)
        {
            bool lowerEqual = other.Lower == Lower;
            bool upperEqual = other.Upper == Upper;
            bool lowerIncEqual = other.IsIncludeLower == IsIncludeLower;
            bool upperIncEqual = other.IsIncludeUpper == IsIncludeUpper;
            return lowerEqual && upperEqual && lowerIncEqual && upperIncEqual;
        }
        /// <summary>
        /// 获取一个该实例的字符串表达形式。
        /// </summary>
        /// <returns>该操作将会返回一个在数学表达意义上的范围。</returns>
        public override string ToString()
        {
            string leftSymbol = IsIncludeLower ? "[" : "(";
            string rightSymbol = IsIncludeUpper ? "]" : ")";
            return $"x ∈ {leftSymbol} {Lower} , {Upper} {rightSymbol} , x ∈ N";
        }
    }
}

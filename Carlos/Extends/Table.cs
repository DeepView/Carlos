using System;
using System.Linq;
using System.Threading;
using Carlos.Exceptions;
using System.Collections;
using Carlos.Environments;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Carlos.Extends
{
    /// <summary>
    /// 一个由一维数组进行描述和定义的高性能二维表。
    /// </summary>
    /// <typeparam name="T">需要装填的数据的类型。</typeparam>
    public class Table<T> : ICloneable, IDisposable, IEnumerable<T>
    {
        private T[] dataContainer;                                                  //需要被操作的数据。
        private bool disposedValue;
        private CancellationTokenSource cancellationSource;
        private ParallelOptions parallelOptions;
        private readonly int processorCount = ComputerInfo.ProcessorCount();
        private readonly int minimumProcessorCount = 0x0004;
        private delegate void CloneFuncLoopBody(ref Table<T> table, long index);     //适用于当前实例Clone方法内部循环体的委托。
        /// <summary>
        /// 构造函数，创建一个默认尺寸（8x8）的二维表。
        /// </summary>
        public Table()
        {
            SetContainerSize(8, 8);
            dataContainer = new T[64];
            InitParallelOptions();
        }
        /// <summary>
        /// 构造函数，创建一个指定行数和列数的二维表。
        /// </summary>
        /// <param name="rows">指定的行数。</param>
        /// <param name="cols">指定的列数。</param>
        /// <exception cref="ArgumentOutOfRangeException">如果任意一个参数小于0，则将会抛出这个异常。</exception>
        public Table(long rows, long cols)
        {
            if (rows < 0 || cols < 0)
                throw new ArgumentOutOfRangeException("Size must greater than zero.");
            else
            {
                SetContainerSize(rows, cols);
                dataContainer = new T[Rows * Cols];
                InitParallelOptions();
            }
        }
        /// <summary>
        /// 构造函数，创建一个指定行数和列数的二维表，并在二维表中填充指定的数据。
        /// </summary>
        /// <param name="rows">指定的行数。</param>
        /// <param name="cols">指定的列数。</param>
        /// <param name="paddingValue">需要填充的数据。</param>
        /// <exception cref="ArgumentOutOfRangeException">如果任意一个参数小于0，则将会抛出这个异常。</exception>
        public Table(long rows, long cols, T paddingValue)
        {
            if (rows < 0 || cols < 0)
                throw new ArgumentOutOfRangeException("Size must greater than zero.");
            else
            {
                SetContainerSize(rows, cols);
                dataContainer = new T[Rows * Cols];
                InitParallelOptions();
                int psForSwitch = processorCount * 64;
                long len = Length;
                if (len >= psForSwitch && processorCount >= minimumProcessorCount)
                {
                    Parallel.For(
                        0,
                        len,
                        parallelOptions,
                        i => dataContainer[i] = paddingValue
                    );
                }
                else
                {
                    for (long i = 0; i < len; i++)
                        dataContainer[i] = paddingValue;
                }
            }
        }
        /// <summary>
        /// 获取当前二维表的总行数。
        /// </summary>
        public long Rows { get; private set; }
        /// <summary>
        /// 获取当前二维表的总列数。
        /// </summary>
        public long Cols { get; private set; }
        /// <summary>
        /// 获取当前二维表的行列数乘积，即当前二维表的长度。
        /// </summary>
        public long Length => dataContainer.Length;
        /// <summary>
        /// 获取或设置指定单元格的内容。
        /// </summary>
        /// <param name="row">该单元格所在的行。</param>
        /// <param name="col">该单元格所在的列。</param>
        /// <returns>该操作将会返回一个具体的单元格内容，其数据类型取决于在初始化当前实例时，指定的数据类型。</returns>
        /// <remarks>注意，在本实例中，row和col参数并不代表行列索引，而是行列位置，因此这些参数一定要大于0。</remarks>
        public T this[long row, long col]
        {
            get => dataContainer[GetIndex(row, col)];
            set => dataContainer[GetIndex(row, col)] = value;
        }
        /// <summary>
        /// 获取指定的行。
        /// </summary>
        /// <param name="row">指定行的行数，请注意，行数并非从0开始计算。</param>
        /// <returns>该操作会返回指定行的全部内容，并以数组的方式呈现。</returns>
        /// <exception cref="IndexOutOfRangeException">当行数小于等于0时，则将会抛出这个异常。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当指定的行数超出范围时，则将会抛出这个异常。</exception>
        public T[] GetRow(long row)
        {
            if (row <= 0)
                throw new IndexOutOfRangeException($"The {nameof(row)} must > 0.");
            else
            {
                if (row > Rows)
                    throw new ArgumentOutOfRangeException(nameof(row), "Index out of range.");
                else
                {
                    T[] data = new T[Cols];
                    int psForSwitch = processorCount * 64;
                    long start = GetIndex(row, 1);
                    long end = GetIndex(row, Cols) + 1;
                    if (Cols >= psForSwitch && processorCount >= minimumProcessorCount)
                    {
                        Parallel.For(
                            start,
                            end,
                            parallelOptions,
                            i =>
                            {
                                long offset_index = i - start;
                                data[offset_index] = dataContainer[i];
                            });
                    }
                    else
                    {
                        for (long i = start; i < end; i++)
                        {
                            long offset_index = i - start;
                            data[offset_index] = dataContainer[i];
                        }
                    }
                    return data;
                }
            }
        }
        /// <summary>
        /// 获取指定的列。
        /// </summary>
        /// <param name="col">指定列的列数，请注意，列数并非从0开始计算。</param>
        /// <returns>该操作会返回指定列的全部内容，并以数组的方式呈现。</returns>
        /// <exception cref="IndexOutOfRangeException">当列数小于等于0时，则将会抛出这个异常。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当指定的列数超出范围时，则将会抛出这个异常。</exception>
        public T[] GetCol(long col)
        {
            if (col <= 0)
                throw new IndexOutOfRangeException($"The {nameof(col)} must > 0.");
            else
            {
                if (col > Cols)
                    throw new ArgumentOutOfRangeException(nameof(col), "Index out of range.");
                else
                {
                    T[] data = new T[Rows];
                    long index = 0;
                    for (long i = col - 1; i < Length; i += Cols)
                    {
                        data[index] = dataContainer[i];
                        index++;
                    }
                    return data;
                }
            }
        }
        /// <summary>
        /// 在保留数据的前提下，重新设置当前二维表实例的尺寸。
        /// </summary>
        /// <param name="rows">新的总行数。</param>
        /// <param name="cols">新的总列数。</param>
        /// <exception cref="ArgumentException">如果任意一个参数小于0，则将会抛出这个异常。</exception>
        /// <exception cref="InvalidSizeException">当重新设置尺寸之后的单元格总数量超出2147483647时，则将会抛出这个异常。</exception>
        public void Resize(long rows, long cols)
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Size must greater than zero.");
            else
            {
                Func<long, long, bool> overflowDetermine = (long a, long b) =>
                {
                    bool isThrowedException = false;
                    try
                    {
                        long multiplication = a * b;
                    }
                    catch (OverflowException) { isThrowedException = true; }
                    return isThrowedException;
                };
                if (overflowDetermine.Invoke(rows, cols))
                {
                    string exmsg = $"The total number of cells cannot be greater than {long.MaxValue}(System.Int64.MaxValue).";
                    throw new InvalidSizeException(exmsg);
                }
                else
                {
                    T[] dataCache = dataContainer;
                    SetContainerSize(rows, cols);
                    dataContainer = new T[Rows * Cols];
                    long len = dataCache.Length > Length ? Length : dataCache.Length;
                    if (Length >= 0x00004000 && processorCount >= minimumProcessorCount)
                        Parallel.For(0, len, i => dataContainer[i] = dataCache[i]);
                    else
                        Array.Copy(dataCache, dataContainer, len);
                }
            }
        }
        /// <summary>
        /// 清空当前二维表中的所有数据。
        /// </summary>
        public void Clear() => dataContainer = new T[Rows * Cols];
        /// <summary>
        /// 获取指定行和指定列所对应单元格所在的索引。
        /// </summary>
        /// <param name="row">该单元格所在的行，请注意，行数并非从0开始计算。</param>
        /// <param name="col">该单元格所在的列，请注意，该参数的性质与参数row一样，即列数并非从0开始计算。</param>
        /// <returns>该操作将会返回一个long32数据类型的值，表示这个单元所对应的索引。</returns>
        public long GetIndex(long row, long col) => (row - 1) * Cols + (col - 1);
        /// <summary>
        /// 获取指定索引所对应单元格所在的行列位置。
        /// </summary>
        /// <param name="index">该单元格所对应的索引。</param>
        /// <returns>该操作将会返回一个元组数据，该数据包含了指定单元格的行列位置信息。</returns>
        public (long row, long col) GetPosition(long index)
        {
            long position1d = index + 1;
            return (position1d / Cols + 1, position1d % Cols);
        }
        /// <summary>
        /// 根据谓词过滤值序列。
        /// </summary>
        /// <param name="predicate">测试每个源元素是否满足条件的函数。</param>
        /// <returns>该操作将会返回一个符合过滤条件的数组。</returns>
        public T[] Where(Func<T, bool> predicate) => (T[])dataContainer.Where(predicate);
        /// <summary>
        /// 获取当前实例的遍历集合的枚举器。
        /// </summary>
        /// <returns>该操作会返回一个可用于迭代集合的枚举器。</returns>
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)dataContainer).GetEnumerator();
        /// <summary>
        /// 获取当前实例的IEnumerator。
        /// </summary>
        /// <returns>该操作将会返回一个IEnumerator。</returns>
        IEnumerator IEnumerable.GetEnumerator() => dataContainer.GetEnumerator();
        /// <summary>
        /// 获取当前实例的一维数组表达形式。
        /// </summary>
        /// <returns>该操作会返回一个一维数组，该数组包含了当前实例所表示的表格中的所有数据。</returns>
        public T[] ToArray() => dataContainer;
        /// <summary>
        /// 获取当前实例的二维数组表达形式。
        /// </summary>
        /// <returns>该操作会返回一个二维数组，该数组包含了当前实例所表示的表格中的所有数据。</returns>
        /// <exception cref="IndexOutOfRangeException">当转换时发生索引超限时，则将会抛出这个异常，一般情况下，这个异常不会发生。</exception>
        public T[,] ToArray2D()
        {
            T[,] array2d = new T[Rows, Cols];
            for (long i = 0; i < Rows; i++)
            {
                for (long j = 0; j < Cols; j++)
                {
                    long index = i * Cols + j;
                    if (index < Length) array2d[i, j] = dataContainer[index];
                    else throw new IndexOutOfRangeException("Index out of range.");
                }
            }
            return array2d;
        }
        /// <summary>
        /// 获取当前实例的交错数组表达形式。
        /// </summary>
        /// <returns>该操作会返回一个交错数组，该数组包含了当前实例所表示的表格中的所有数据。</returns>
        public T[][] ToJaggedArray()
        {
            T[][] jahhedArray = new T[Rows][];
            for (long i = 0; i < Rows; i++)
                jahhedArray[i] = GetRow(i + 1);
            return jahhedArray;
        }
        /// <summary>
        /// 获取该实例装填的数据的数据类型。
        /// </summary>
        /// <returns>该操作将会返回当前实例装填的数据的数据类型。</returns>
        public Type GetInsideType() => typeof(T);
        /// <summary>
        /// 创建一个深层克隆副本。
        /// </summary>
        /// <returns>该操作将会返回一个深克隆的Table&lt;T&gt;实例。</returns>
        /// <remarks>访问这个方法时不需要担心性能开销，该方法使用了多重条件判断来确保当前方法尽可能的在最短的时间内完成。</remarks>
        public object Clone()
        {
            long rows = Rows, cols = Cols;
            Table<T> copy = new Table<T>(Rows, Cols)
            {
                Rows = rows,
                Cols = cols
            };
            bool isRef = GetInsideType().IsByRef;
            long psForkCondition = 0x00002000;
            CloneFuncLoopBody loop = (ref Table<T> table, long index) =>
            {
                (long row, long col) = GetPosition(index);
                table[row, col] = this[row, col];
            };
            if (!isRef) psForkCondition = 0x00040000;
            if (Length >= psForkCondition && processorCount >= minimumProcessorCount)
                Parallel.For(0, Length, parallelOptions, i => loop(ref copy, i));
            else
                for (long i = 0; i < Length; i++) loop(ref copy, i);
            return copy;
        }
        /// <summary>
        /// 创建一个引用类型的克隆副本，即浅克隆副本。
        /// </summary>
        /// <returns>该操作将会返回一个浅克隆的Table&lt;T&gt;实例。</returns>
        public object CloneRef() => MemberwiseClone();
        /// <summary>
        /// 获取当前实例的字符串表达形式。
        /// </summary>
        /// <returns>该操作将会返回一个可读性非常高的字符串，这个字符串包含了该实例的内部元素的数据类型，总行列数，表格长度，以及该实例数据的打印字符串。</returns>
        public override string ToString()
        {
            string itstr = GetInsideType().FullName;
            string prlong = $"Table<{itstr}>:\n\tRows={Rows}, Cols={Cols}, Length={Length}\n\tElements=\n";
            if (Length > 0)
            {
                for (long i = 1; i <= Rows; i++)
                {
                    prlong += "\t\t{";
                    for (long j = 1; j <= Cols; j++)
                        prlong += $"Data {GetIndex(i, j) + 1}: {this[i, j]}; ";
                    prlong += "}\n";
                }
            }
            else prlong += $"\t**** Zero Size Table ****";
            return prlong;
        }
        /// <summary>
        /// 赋予Rows和Cols属性新的值。
        /// </summary>
        /// <param name="rows">Rows值。</param>
        /// <param name="cols">Cols值。</param>
        private void SetContainerSize(long rows, long cols)
        {
            Rows = rows;
            Cols = cols;
        }
        /// <summary>
        /// 初始化当前实例所需要的并行选项。
        /// </summary>
        private void InitParallelOptions()
        {
            cancellationSource = new CancellationTokenSource();
            parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = processorCount,
                CancellationToken = cancellationSource.Token
            };
        }
        /// <summary>
        /// 释放该对象引用的所有内存资源。
        /// </summary>
        /// <param name="disposing">用于指示是否释放托管资源。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Clear();
                    dataContainer = null;
                }
                disposedValue = true;
            }
        }
        /// <summary>
        /// 手动释放该对象引用的所有内存资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

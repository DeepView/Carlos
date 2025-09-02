using System;
using System.Linq;
using System.Threading;
using Carlos.Exceptions;
using System.Collections;
using Carlos.Environments;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
namespace Carlos.Extends
{
    /// <summary>
    /// 一个由一维数组进行描述和定义的高性能二维表。
    /// </summary>
    /// <typeparam name="T">需要装填的数据的类型。</typeparam>
    /// <remarks>
    /// 值得注意的是，当前实例的所有数据都存储在一个一维数组中，因此在访问数据时，行和列的索引并不是从0开始计算，而是从1开始计算。
    /// 如果需要提高数据筛选操作的效率，推荐先对数据集合进行哈希化预处理，该方法声明如下：
    /// <code language="cs">
    /// public void Pretreatment();
    /// </code>
    /// 另外，数据集合如果已经产生了变化，则在对数据筛选之前，建议再调用一次该方法。下面是调用该方法的一个简易示例代码：
    /// <code language="cs">
    /// public void WhereTest()
    /// {
    ///     int paddingVal = 0;
    ///     Table &lt;int&gt; table = new(2048, 2048);
    ///     Random rnd = new(78923);
    ///     Parallel.For(0, table.Rows, i =&gt;
    ///     {
    ///         (long row, long col) = table.GetPosition(i);
    ///         paddingVal = rnd.Next();
    ///         table[row, col] = paddingVal;
    ///     });
    ///     table.Pretreatment();
    ///     IEnumerable &lt;int&gt; query = table.Where(cell =&gt; cell &lt;= 65535);
    ///     Console.WriteLine($"query.Length={query.Count()}");
    /// }
    /// </code>
    /// </remarks>
    public class Table<T> : ICloneable, IDisposable, IEnumerable<T>
    {
        private T[] dataContainer;                                                  //需要被操作的数据。
        private bool disposedValue;
        private Dictionary<T, long> elementToIndexMap;
        private CancellationTokenSource cancellationSource;
        private ParallelOptions parallelOptions;
        private readonly int processorCount = ComputerInfo.ProcessorCount();        //当前计算机的逻辑处理器数量。
        private readonly int minimumProcessorCount = 0x0004;                        //用于并行选项的最小逻辑处理器数量。
        private delegate void CloneFuncLoopBody(ref Table<T> table, long index);    //适用于当前实例Clone方法内部循环体的委托。
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
        /// 构造函数，创建一个指定近似大小的二维表。
        /// </summary>
        /// <param name="approximateSize">指定的近似大小，但是实例创建成功之后，其实际大小往往大于或者等于这个近似大小。</param>
        /// <exception cref="ArgumentOutOfRangeException">当参数approximateSize指定的近似大小小于0时，则将会抛出这个异常。</exception>
        public Table(long approximateSize)
        {
            if (approximateSize < 0)
                throw new ArgumentOutOfRangeException("Size must greater than zero.");
            else if (approximateSize == 0)
                SetContainerSize(0, 0);
            else
            {
                long rows = (long)Math.Sqrt(approximateSize) + 1;
                long cols = rows;
                SetContainerSize(rows, cols);
            }
            dataContainer = new T[Rows * Cols];
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
        /// <param name="data">需要填充的数据。</param>
        /// <exception cref="ArgumentOutOfRangeException">如果任意一个参数小于0，则将会抛出这个异常。</exception>
        /// <exception cref="ArgumentException">如果data参数包含的数据超出rows和cols参数允许的范围，则将会抛出这个异常。</exception>
        public Table(long rows, long cols, T[] data)
        {
            if (rows < 0 || cols < 0)
                throw new ArgumentOutOfRangeException("Size must greater than zero.");
            else
            {
                SetContainerSize(rows, cols);
                dataContainer = new T[Rows * Cols];
                if (data.Length > Length)
                    throw new ArgumentException($"The data length {data.Length} is greater than the table length {Length}.");
                else
                    Array.Copy(data, dataContainer, data.Length);
                InitParallelOptions();
            }
        }
        /// <summary>
        /// 构造函数，创建一个指定行数和列数的二维表，并在二维表中填充指定的数据。
        /// </summary>
        /// <param name="rows">指定的行数。</param>
        /// <param name="cols">指定的列数。</param>
        /// <param name="paddingValue">需要填充的默认数据，这个数据会被填充到所有的Cell中。</param>
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
        /// 获取或设置当前二维表是否允许调整尺寸，默认为true。
        /// </summary>
        public bool Resizable { get; set; } = true;
        /// <summary>
        /// 获取当前二维表最后一次删除的元素。
        /// </summary>
        public T LastDeletedData { get; private set; }
        /// <summary>
        /// 获取或设置指定单元格的内容。
        /// </summary>
        /// <param name="index">该单元格所对应的一维索引，这个索引可以根据row和col进行转换。</param>
        /// <returns>该操作将会返回一个具体的单元格内容，其数据类型取决于在初始化当前实例时，指定的数据类型。</returns>
        public T this[long index]
        {
            get => dataContainer[index];
            set => dataContainer[index] = value;
        }
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
        /// 获取指定单元格的邻居元素。
        /// </summary>
        /// <param name="index">指定单元格所对应的索引。</param>
        /// <param name="isIncludeDiagonalItems">是否允许获取单元格对角线的邻居元素。</param>
        /// <returns>该操作将会返回一个Key为(long neighborRow, long neighborCol)元组，Value为T的词典类型，该类型存储了邻居元素的位置信息和邻居元素的值。</returns>
        /// <exception cref="IndexOutOfRangeException">当索引对应的单元格找不到时，则将会抛出这个异常。</exception>
        public Dictionary<(long neighborRow, long neighborCol), T> GetNeighbors(long index, bool isIncludeDiagonalItems = true)
        {
            if (index > dataContainer.Length - 1 || index < 0)
                throw new IndexOutOfRangeException("Index out of range.");
            else
            {
                (long row, long col) = GetPosition(index);
                return GetNeighbors(row, col, isIncludeDiagonalItems);
            }
        }
        /// <summary>
        /// 获取指定单元格的邻居元素。
        /// </summary>
        /// <param name="row">指定单元格的行号。</param>
        /// <param name="col">所在单元格的列号。</param>
        /// <param name="isIncludeDiagonalItems">是否允许获取单元格对角线的邻居元素。</param>
        /// <returns>该操作将会返回一个Key为(long neighborRow, long neighborCol)元组，Value为T的词典类型，该类型存储了邻居元素的位置信息和邻居元素的值。</returns>
        /// <exception cref="IndexOutOfRangeException">当参数row或者col对应的单元格找不到时，则将会抛出这个异常。</exception>
        public Dictionary<(long neighborRow, long neighborCol), T> GetNeighbors(long row, long col, bool isIncludeDiagonalItems = true)
        {
            var neighbors = new Dictionary<(long neighborRow, long neighborCol), T>();
            if (row <= 0 || col <= 0 || row > Rows || col > Cols)
                throw new IndexOutOfRangeException($"The {nameof(row)} or {nameof(col)} out of range.");
            else
            {
                (long n_row, long n_col) location;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        location = (row + i, col + j);
                        if (location.n_row > 0 && location.n_col > 0 && location.n_row <= Rows && location.n_col <= Cols)
                        {
                            long n_index = GetIndex(location.n_row, location.n_col);
                            neighbors[location] = dataContainer[n_index];
                        }
                    }
                }
                if (!isIncludeDiagonalItems)
                {
                    var keys = neighbors.Keys.ToList();
                    foreach (var key in keys)
                    {
                        if (Math.Abs(key.neighborRow - row) + Math.Abs(key.neighborCol - col) == 2)
                            neighbors.Remove(key);
                    }
                }
            }
            return neighbors;
        }
        /// <summary>
        /// 在保留数据的前提下，重新设置当前二维表实例的尺寸。
        /// </summary>
        /// <param name="rows">新的总行数。</param>
        /// <param name="cols">新的总列数。</param>
        /// <exception cref="ArgumentException">如果任意一个参数小于0，则将会抛出这个异常。</exception>
        /// <exception cref="InvalidSizeException">当重新设置尺寸之后的单元格总数量超出2147483647时，则将会抛出这个异常。</exception>
        /// <exception cref="InvalidOperationException">当Resizable属性值为false时，则将会抛出这个异常。</exception>
        /// <remarks>如果新的尺寸小于当前实例的尺寸，则会丢失部分数据。另外，如果当Resizable属性为false时，是无法修改尺寸的，必须将Resizable属性设置为true才能修改尺寸。</remarks>
        public void Resize(long rows, long cols)
        {
            if (!Resizable)
                throw new InvalidOperationException("The current instance is not allowed to resize.");
            else
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
        }
        /// <summary>
        /// 清空当前二维表中的所有数据。
        /// </summary>
        public void Clear() => dataContainer = new T[Rows * Cols];
        /// <summary>
        /// 在指定单元格后面插入指定的元素，同时其他元素依次后移，并将表格中最后一个元素存储到LastDeletedData属性中。
        /// </summary>
        /// <param name="data">需要被插入的元素。</param>
        /// <param name="row">指定单元格所在的行。</param>
        /// <param name="col">指定单元格所在的列。</param>
        /// <exception cref="InvalidOperationException">如果当前实例长度为0，则将会抛出这个异常。</exception>
        public void Insert(T data, long row, long col)
        {
            if (Length > 0)
            {
                LastDeletedData = dataContainer[Length - 1];
                long start = Length - 1;
                long index = GetIndex(row, col);
                for (long i = start; i > index; i--)
                    dataContainer[i] = dataContainer[i - 1];
                dataContainer[index] = data;
            }
            else throw new InvalidOperationException("Table is not 0-length.");
        }
        /// <summary>
        /// 删除指定单元格中的元素，并将删除的元素存储到LastDeletedData属性中。
        /// </summary>
        /// <param name="row">指定单元格所在的行。</param>
        /// <param name="col">指定单元格所在的列。</param>
        /// <exception cref="InvalidOperationException">如果当前实例长度为0，则将会抛出这个异常。</exception> 
        public void Remove(long row, long col)
        {
            if (Length > 0)
            {
                long index = GetIndex(row, col);
                LastDeletedData = dataContainer[index];
                for (long i = index; i < Length - 1; i++)
                    dataContainer[i] = dataContainer[i + 1];
            }
            else throw new InvalidOperationException("Table is not 0-length.");
        }
        /// <summary>
        /// 对当前实例容纳的数据进行哈希化处理。
        /// </summary>
        /// <remarks>该方法之所以将可见性设置为public，是为了让用户自行决定合适对数据进行预处理。不过在此建议，当该实例容纳的数据发生变更之后，执行诸如数据查找等方法之前，执行一次这个方法。</remarks>
        public void Pretreatment()
        {
            elementToIndexMap = new Dictionary<T, long>();
            for (long i = 0; i < Length; i++)
                elementToIndexMap[dataContainer[i]] = i;
        }

        /// <summary>
        /// 确认指定的元素是否存在，在调用该方法之前，建议先调用一次Pretreatment()方法。
        /// </summary>
        /// <param name="target">需要确认是否存在的元素。</param>
        /// <returns>该操作将会返回一个Boolean类型的数据，用于表示参数指定的元素是否存在。</returns>
        public bool Exists(T target) => elementToIndexMap.TryGetValue(target, out _);
        /// <summary>
        /// 确认指定元素在当前实例中第一次出现的位置，在调用该方法之前，建议先调用一次Pretreatment()方法。
        /// </summary>
        /// <param name="target">指定需要查找的元素。</param>
        /// <returns>该操作将会返回一个元组数据，该数据包含了指定元素所在单元格的行列位置信息，如果未查到该元素，这将会返回(-1,-1)的元组。</returns>
        public (long row, long col) PositionOf(T target)
        {
            bool exists = elementToIndexMap.TryGetValue(target, out long index);
            if (exists) return GetPosition(index);
            else return (-1, -1);
        }
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
        /// 查找指定的元素所对应的索引。
        /// </summary>
        /// <param name="target">需要查找其索引的元素。</param>
        /// <returns>如果该操作成功找到指定元素所对应的索引，则将会返回一个确切的索引值，否则会返回-1。</returns>
        /// <remarks>该方法是利用多线程的方式查找，所以当存在多个相同的元素时，该操作不一定会保证这个元素所对应的索引是唯一的，且也无法保证这个索引是最小的或者最大的。</remarks>
        public int IndexOf(T target)
        {
            int foundIndex = -1;
            var partitions = Partitioner.Create(0, dataContainer.Length);
            Parallel.ForEach(partitions, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    if (Volatile.Read(ref foundIndex) != -1)
                    {
                        loopState.Stop();
                        return;
                    }
                    if (dataContainer[i].Equals(target))
                    {
                        Interlocked.CompareExchange(ref foundIndex, i, -1);
                        loopState.Stop();
                        return;
                    }
                }
            });
            return foundIndex;
        }
        /// <summary>
        /// 根据谓词过滤值序列。
        /// </summary>
        /// <param name="predicate">测试每个源元素是否满足条件的函数。</param>
        /// <returns>该操作将会返回一个符合过滤条件的数组。</returns>
        public IEnumerable<T> Where(Func<T, bool> predicate) => dataContainer.Where(predicate);
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
        public void ToCsvFile(string fileName, char separator = ',')
        {
            using var writer = new System.IO.StreamWriter(fileName);
            for (long i = 1; i <= Rows; i++)
            {
                for (long j = 1; j <= Cols; j++)
                {
                    writer.Write(this[i, j]);
                    if (j < Cols) writer.Write(separator);
                }
                writer.WriteLine();
            }
        }
        /// <summary>
        /// 获取该实例装填的数据的数据类型。
        /// </summary>
        /// <returns>该操作将会返回当前实例装填的数据的数据类型。</returns>
        public Type GetInsideType() => typeof(T);
        /// <summary>
        /// 获取该实例装填的数据是否隶属于值类型数据类型。
        /// </summary>
        /// <returns>如果当前实例装填的数据隶属于值类型，则返回true，否则返回false。</returns>
        public bool IsValueTypeWithInside() => GetInsideType().IsValueType;
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
        /// <remarks>值得注意的是，若当前实例包含的元素数量过多，可能会导致这个方法运行时间延长，因为该方法会将所有的元素进行字符串化，然后进行文本格式化。在这种情形下，该操作会消耗一定的时间开销。</remarks>
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

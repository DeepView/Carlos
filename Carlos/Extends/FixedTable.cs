using System;
using System.Linq;
using System.Threading;
using Carlos.Enumerations;
using System.Threading.Tasks;
namespace Carlos.Extends
{
    /// <summary>
    /// 一个固定尺寸的二维表格类。
    /// </summary>
    /// <typeparam name="T">二维表格类中所存放数据的数据类型。</typeparam>
    public class FixedTable<T>
    {
        private int[] rowsDataCount;
        private FixedLengthQueue<T>[] containers;
        private readonly CancellationTokenSource cancellationSource;
        private readonly ParallelOptions parallelOptions;
        /// <summary>
        /// 构造函数，通过指定的最大行数rows和最大列数cols来创建一个固定尺寸的二维表。
        /// </summary>
        /// <param name="rows">指定的最大行数。</param>
        /// <param name="cols">指定的最大列数。</param>
        public FixedTable(int rows, int cols)
        {
            Size = (rows, cols);
            rowsDataCount = new int[rows];
            cancellationSource = new CancellationTokenSource();
            parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = cancellationSource.Token
            };
            for (int i = 0; i < rows; i++) rowsDataCount[i] = 0;
        }
        /// <summary>
        /// 获取当前二维表实例的尺寸，该属性通过一个最大行数rows和最大列数cols组成的元组进行描述。
        /// </summary>
        public (int rows, int cols) Size
        {
            get
            {
                int rows = containers.Length;
                int cols = containers[0].Length;
                return (rows, cols);
            }
            private set
            {
                containers = new FixedLengthQueue<T>[value.rows];
                for (int i = 0; i < containers.Length; i++)
                    containers[i] = new FixedLengthQueue<T>(value.cols);
            }
        }
        /// <summary>
        /// 索引器，获取指定位置的元素。
        /// </summary>
        /// <param name="row">指定的行。</param>
        /// <param name="col">指定的列。</param>
        /// <returns>索引器返回的数据是一个确切的对象，这个对象的数据类型由实例来决定。</returns>
        public T this[int row, int col]
        {
            get
            {
                ListNode<T> node = containers[row][col];
                return node.Element;
            }
        }
        /// <summary>
        /// 获取当前二维表实例的非空对象数量。
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                Parallel.For(0, Size.rows, parallelOptions, row =>
                {
                    if (rowsDataCount[row] > 0)
                    {
                        int flag = 0;
                        Parallel.For(0, Size.cols, parallelOptions, (col, status) =>
                        {
                            if (this[row, col] != null)
                            {
                                Interlocked.Increment(ref count);
                                Interlocked.Decrement(ref flag);
                            }
                            if (flag >= rowsDataCount[row]) status.Stop();
                        });
                    }
                });
                return count;
            }
        }
        /// <summary>
        /// 获取当前二维表实例是否为空表（未装填任何数据，但已经初始化的表）。
        /// </summary>
        /// <exception cref="NullReferenceException">当存储数据的容器未初始化时，则将会抛出这个异常。</exception>
        public bool IsEmpty
        {
            get
            {
                if (containers == null)
                    throw new NullReferenceException("This FixedTable object needs to be initialized.");
                else
                {
                    int emptyRowsCount = 0;
                    int len = containers.Length;
                    for (int i = 0; i < len; i++)
                        if (containers[i].IsEmpty()) emptyRowsCount++;
                    if (emptyRowsCount >= len) return true;
                    return false;
                }
            }
        }
        /// <summary>
        /// 获取当前二维表实例最后一个非空对象的位置，该位置由行row和列col组成的元组来表述。
        /// </summary>
        public (int row, int col) FinalDataLocation
        {
            get
            {
                int lastNotNullRowIndex = 0;
                int lastNotNullColIndex = 0;
                if (IsEmpty) return (row: 0, col: -1);
                else
                {
                    lastNotNullRowIndex = Array.FindLastIndex(
                        rowsDataCount,
                        item => item > 0);
                    lastNotNullColIndex = Array.FindLastIndex(
                        containers[lastNotNullRowIndex].ToArray(),
                        item => item == null);
                }
                return (row: lastNotNullRowIndex, col: lastNotNullColIndex);
            }
        }
        /// <summary>
        /// 向二维表实例最后一个非空对象后面添加一个对象。
        /// </summary>
        /// <param name="element">需要添加的对象。</param>
        /// <exception cref="InvalidOperationException">当FinalDataLocation为当前实例最后一个位置时，或者因为其他操作导致添加数据失败时，则将会抛出这个异常。</exception>
        public void Add(T element)
        {
            FixedLengthQueue<T> rowData = containers[FinalDataLocation.row];
            if (rowData.IsFull)
                if (FinalDataLocation.row < Size.rows)
                    rowData = containers[FinalDataLocation.row + 1];
                else
                    throw new InvalidOperationException("Unable to add new data.");
            bool isCompleted = rowData.Add(element);
            if (!isCompleted)
                throw new InvalidOperationException("Operation failed.");
            else
            {
                int lastNotNullRow = FinalDataLocation.row;
                object _sync_locker = new object();
                lock (_sync_locker)
                    rowsDataCount[lastNotNullRow]++;
            }
        }
        /// <summary>
        /// 移除指定位置的对象，该操作可能会引发的异常，以及异常引发的机制与该函数的重载版本<see cref="Remove(int, int)">Remove(int row, int col)</see>相同。
        /// </summary>
        /// <param name="location">被移出的对象所在的位置。</param>
        public void Remove((int row, int col) location) => Remove(location.row, location.col);
        /// <summary>
        /// 移除指定位置的对象。
        /// </summary>
        /// <param name="row">指定的行。</param>
        /// <param name="col">指定的列。</param>
        /// <exception cref="InvalidOperationException">当需要被移出的数据不存在时，或者因为其他操作导致移除数据失败时，则将会抛出这个异常。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当指定的位置超出检索范围时，则将会抛出这个异常。</exception>
        public void Remove(int row, int col)
        {
            if (this[row, col] == null)
                throw new InvalidOperationException("The object does not exist.");
            if (row > Size.rows || col > Size.cols)
                throw new ArgumentOutOfRangeException("The parameter is outside the actionable range.");
            bool isCompleted = containers[row].Remove(col);
            if (!isCompleted) throw new InvalidOperationException("Operation failed!");
            else
            {
                object _sync_locker = new object();
                lock (_sync_locker)
                    rowsDataCount[row]--;
            }
        }
        /// <summary>
        /// 将整个二维表实例的内容顺序进行反转。
        /// </summary>
        public void Reverse()
        {
            foreach (FixedLengthQueue<T> item in containers) item.Reverse();
            containers = containers.Reverse() as FixedLengthQueue<T>[];
            rowsDataCount = rowsDataCount.Reverse() as int[];
        }
        /// <summary>
        /// 将指定行的内容顺序进行反转。
        /// </summary>
        /// <param name="row">指定的行。</param>
        public void Reverse(int row) => containers[row].Reverse();
        /// <summary>
        /// 碎片整理，将零散的数据按照指定的整理模式进行重新存放。
        /// </summary>
        /// <param name="defragmentMode">指定的碎片整理模式。</param>
        /// <exception cref="InvalidOperationException">当在参数中传入一个不受代码支持的选项时，则将会抛出这个异常。</exception>
        public void Defragment(DefragmentMode defragmentMode)
        {
            FixedLengthQueue<T> queue = new FixedLengthQueue<T>(Size.cols);
            if (Count == Size.rows * Size.cols) return;
            else
            {
                switch (defragmentMode)
                {
                    case DefragmentMode.Serial:
                        FixedLengthQueue<T>[] table = new FixedLengthQueue<T>[Size.rows];
                        for (int i = 0; i < table.Length; i++)
                            table[i] = new FixedLengthQueue<T>(Size.cols);
                        for (int i = 0; i < Size.rows; i++)
                        {
                            if (rowsDataCount[i] == 0) continue;
                            else
                            {
                                for (int j = 0; j < Size.cols; j++)
                                    if (!IsNullElement(i, j))
                                        table[i][j].Element = containers[i][j].Element;
                            }
                        }
                        containers = table;
                        break;
                    case DefragmentMode.Row:
                        //TODO: 需要完善DefragmentMode.Row相关代码。
                        break;
                    case DefragmentMode.Col:
                        //TODO: 需要完善DefragmentMode.Col相关代码。
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported mode.");
                }
            }
        }
        /// <summary>
        /// 检查指定的元素是否为NULL。
        /// </summary>
        /// <param name="row">指定的行。</param>
        /// <param name="col">指定的列。</param>
        /// <returns></returns>
        public bool IsNullElement(int row, int col) => this[row, col] == null;
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Collections;

namespace Carlos.Extends.Tests
{
    [TestClass()]
    public class TableTests
    {
        [TestMethod()]
        public void TableTest()
        {
            Table<long> table = new(10, 10);
            for (long i = 0; i < table.Length; i++)
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = i;
                Console.WriteLine($"index {i}: {table[row, col]}");
            }
        }
        [TestMethod()]
        public void TableCloneTest()
        {
            Table<string> t = new(1024, 1024);
            Table<string> c = (Table<string>)t.Clone();
            Console.WriteLine($"c.Length={c.Length}");
            t[1, 1] = "I love you";
            Console.WriteLine($"v={t[1, 1]}");
            Console.WriteLine($"v={c[1, 1]}");
        }
        [TestMethod()]
        public void GetRowTest()
        {
            Table<int> table = new(7, 5);
            for (int i = 0; i < table.Length; i++)
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = i;
            }
            int[] rowElements = table.GetRow(3);
            Console.WriteLine($"row_count={table.Rows}, col_count={table.Cols}\n");
            for (int i = 0; i < rowElements.Length; i++) Console.WriteLine($"index={i}, data={rowElements[i]}");
        }
        [TestMethod()]
        public void GetColTest()
        {
            Table<int> table = new(7, 5);
            for (int i = 0; i < table.Length; i++)
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = i;
            }
            int[] colElements = table.GetCol(1);
            Console.WriteLine($"row_count={table.Rows}, col_count={table.Cols}\n");
            for (int i = 0; i < colElements.Length; i++) Console.WriteLine($"index={i}, data={colElements[i]}");
        }
        [TestMethod()]
        public void ToArray2DTest()
        {
            Table<int> table = new(7, 5);
            for (int i = 0; i < table.Length; i++)
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = i;
            }
            int[,] a2d = table.ToArray2D();
            long rows = table.Rows;
            long cols = table.Cols;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"{a2d[i, j]}\t");
                }
                Console.WriteLine($"\t// row_data_count={cols}");
            }
            Console.WriteLine("\nto_array2d_completed.");
        }
        [TestMethod()]
        public void ToJaggedArrayTest()
        {
            Table<int> table = new(7, 5);
            for (int i = 0; i < table.Length; i++)
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = i;
            }
            int[][] ja = table.ToJaggedArray();
            long rows = table.Rows;
            long cols = table.Cols;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"{ja[i][j]}\t");
                }
                Console.WriteLine($"\t// row_data_count={cols}");
            }
            Console.WriteLine("\nto_jagged_array_completed.");
        }
        [TestMethod()]
        public void NormalSum()
        {
            Table<long> table = new(40000, 40000);
            for (int i = 0; i < table.Length; i++)
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = i;
            }
            long res = table.Sum();
            Console.WriteLine($"table_length={table.Length}");
            Console.WriteLine($"table_sum_result={res}");
        }
        [TestMethod()]
        public void ParallelSum()
        {
            Table<long> table = new(40000, 40000);
            for (int i = 0; i < table.Length; i++)
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = i;
            }
            long res = table.AsParallel().Sum();
            //long res = 0;
            //Parallel.For(0, table.Length, i =>
            //{
            //    (long row, long col) = table.GetPosition(i);
            //    res += table[row, col];
            //});
            Console.WriteLine($"table_length={table.Length}");
            Console.WriteLine($"table_sum_result={res}");
        }
        [TestMethod()]
        public void CopyTest()
        {
            int size = 100000000; // 示例数组大小  
            int[] oldArray = new int[size];
            // 假设这里对oldArray进行了初始化
            // 使用Array.Copy复制数组  
            int[] newArray1 = new int[size];
            Stopwatch sw1 = Stopwatch.StartNew();
            Array.Copy(oldArray, newArray1, size);
            sw1.Stop();
            Console.WriteLine($"Array.Copy 时间: {sw1.ElapsedMilliseconds} ms");
            // 使用for循环复制数组  
            int[] newArray2 = new int[size];
            Stopwatch sw2 = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                newArray2[i] = oldArray[i];
            }
            sw2.Stop();
            Console.WriteLine($"for循环 时间: {sw2.ElapsedMilliseconds} ms");
            // 使用Parallel.For循环复制数组  
            int[] newArray3 = new int[size];
            Stopwatch sw3 = Stopwatch.StartNew();
            Parallel.For(0, size, i => newArray3[i] = oldArray[i]);
            sw3.Stop();
            Console.WriteLine($"Parallel.For循环 时间: {sw3.ElapsedMilliseconds} ms");
        }
        [TestMethod()]
        public void ResizeTest()
        {
            Table<long> table = new(16384, 16384);
            for (long i = 0; i < table.Length; i++)
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = i;
            }
            //table.Resizable = false;
            Console.WriteLine($"Length={table.Length}");
            Stopwatch stopwatch = Stopwatch.StartNew();
            table.Resize(32768, 32768);
            stopwatch.Stop();
            Console.WriteLine($"Length after resize={table.Length}");
            Console.WriteLine($"Elapsed Time = {stopwatch.ElapsedMilliseconds} ms");
        }
        [TestMethod()]
        public void ClearTest()
        {
            Table<string> table = new(5, 3, "Love");
            table.Clear();
            Console.WriteLine($"clear_completed.\n\n{table}");
        }
        [TestMethod()]
        public void WhereTest()
        {
            int paddingVal = 0;
            Table<int> table = new(2048, 2048);
            Random rnd = new(78923);
            Parallel.For(0, table.Rows, i =>
            {
                (long row, long col) = table.GetPosition(i);
                paddingVal = rnd.Next();
                table[row, col] = paddingVal;
            });
            table.Pretreatment();
            IEnumerable<int> query = table.Where(cell => cell <= 65535);
            Console.WriteLine($"query.Length={query.Count()}");
        }

        [TestMethod()]
        public void ExistsParallelTest()
        {
            //int paddingVal = 0;
            Table<int> table = new(128, 128);
            //Random rnd = new(78923);
            Parallel.For(0, table.Length, i =>
            {
                (long row, long col) = table.GetPosition(i);
                //paddingVal = rnd.Next();
                table[row, col] = (int)i;
            });
            bool isFound = table.Exists(0);
            Console.WriteLine(isFound);
        }

        [TestMethod()]
        public void ToCsvFileTest()
        {
            Table<int> table = new(3, 3);
            Parallel.For(0, table.Length, i =>
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = (int)i + 1;
            });
            table.ToCsvFile(@"D:\table_to_csv_test.csv");
        }

        [TestMethod()]
        public void GetNeighborsTest()
        {
            Table<int> table = new(100, 200);
            Parallel.For(0, table.Length, i =>
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = (int)i + 1;
            });
            var neighbors = table.GetNeighbors(1, 1, false);
            foreach (var item in neighbors)
            {
                Console.WriteLine(item);
            }
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Table<int> table = new(1000, 1000);
            Parallel.For(0, table.Length, i =>
            {
                (long row, long col) = table.GetPosition(i);
                table[row, col] = (int)i + 1;
            });
            Console.WriteLine(table.ToString());
        }
    }
}

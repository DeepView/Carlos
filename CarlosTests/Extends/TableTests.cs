using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace Carlos.Extends.Tests
{
    [TestClass()]
    public class TableTests
    {
        [TestMethod()]
        public void TableTest()
        {
            Table<int> table = new(10, 10);
            for (int i = 0; i < table.Length; i++)
            {
                (int row, int col) = table.GetPosition(i);
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
        //[TestMethod()]
        //public void TableCloneFastTest()
        //{
        //    Table<int> t = new(1024, 1024);
        //    Table<int> c = (Table<int>)t.CloneFast();
        //    Console.WriteLine($"c.Length={c.Length}");
        //    t[1, 1] = 32768;
        //    Console.WriteLine($"v={t[1, 1]}");
        //    Console.WriteLine($"v={c[1, 1]}");
        //}
        [TestMethod()]
        public void GetRowTest()
        {
            Table<int> table = new(7, 5);
            for (int i = 0; i < table.Length; i++)
            {
                (int row, int col) = table.GetPosition(i);
                table[row, col] = i;
            }
            int[] rowElements = table.GetRow(1);
            Console.WriteLine($"row_count={table.Rows}, col_count={table.Cols}\n");
            for (int i = 0; i < rowElements.Length; i++) Console.WriteLine($"index={i}, data={rowElements[i]}");
        }
        [TestMethod()]
        public void GetColTest()
        {
            Table<int> table = new(7, 5);
            for (int i = 0; i < table.Length; i++)
            {
                (int row, int col) = table.GetPosition(i);
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
                (int row, int col) = table.GetPosition(i);
                table[row, col] = i;
            }
            int[,] a2d = table.ToArray2D();
            int rows = table.Rows;
            int cols = table.Cols;
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
                (int row, int col) = table.GetPosition(i);
                table[row, col] = i;
            }
            int[][] ja = table.ToJaggedArray();
            int rows = table.Rows;
            int cols = table.Cols;
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
    }
}
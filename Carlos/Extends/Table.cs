﻿using System;
using System.Threading.Tasks;
namespace Carlos.Extends
{
    /// <summary>
    /// 一个高性能的二维表。
    /// </summary>
    /// <typeparam name="T">需要装填的数据的类型。</typeparam>
    public class Table<T>: IDisposable
    {
        private T[] dataContainer;
        private bool disposedValue;
        /// <summary>
        /// 构造函数，创建一个默认尺寸（8x8）的二维表。
        /// </summary>
        public Table()
        {
            FillContainerSize(8, 8);
            dataContainer = new T[64];
        }
        /// <summary>
        /// 构造函数，创建一个指定行数和列数的二维表。
        /// </summary>
        /// <param name="rows">指定的行数。</param>
        /// <param name="cols">指定的列数。</param>
        /// <exception cref="ArgumentOutOfRangeException">如果任意一个参数小于0，则将会抛出这个异常。</exception>
        public Table(int rows, int cols)
        {
            if (rows < 0 || cols < 0)
                throw new ArgumentOutOfRangeException("Size must greater than zero.");
            else
            {
                FillContainerSize(rows, cols);
                dataContainer = new T[Rows * Cols];
            }
        }
        /// <summary>
        /// 获取当前二维表的总行数。
        /// </summary>
        public int Rows { get; private set; }
        /// <summary>
        /// 获取当前二维表的总列数。
        /// </summary>
        public int Cols { get; private set; }
        /// <summary>
        /// 获取当前二维表的行列数乘积，即当前二维表的长度。
        /// </summary>
        public int Length => dataContainer.Length;
        /// <summary>
        /// 获取或设置指定单元格的内容。
        /// </summary>
        /// <param name="row">该单元格所在的行。</param>
        /// <param name="col">该单元格所在的列。</param>
        /// <returns>该操作将会返回一个具体的单元格内容，其数据类型取决于在初始化当前实例时，指定的数据类型。</returns>
        public T this[int row, int col]
        {
            get => dataContainer[GetIndex(row, col)];
            set => dataContainer[GetIndex(row, col)] = value;
        }
        /// <summary>
        /// 获取指定的行。
        /// </summary>
        /// <param name="row">指定行的行数。</param>
        /// <returns>该操作会返回指定行的全部内容，并以数组的方式呈现。</returns>
        /// <exception cref="ArgumentOutOfRangeException">当指定的行数超出范围时，则将会抛出这个异常。</exception>
        public T[] GetRow(int row)
        {
            if (row > Rows)
                throw new ArgumentOutOfRangeException(nameof(row), "Index out of range");
            else
            {
                T[] data = dataContainer[GetIndex(row, 1)..GetIndex(row, Cols)];
                return data;
            }
        }
        /// <summary>
        /// 获取指定的列。
        /// </summary>
        /// <param name="col">指定列的列数。</param>
        /// <returns>该操作会返回指定列的全部内容，并以数组的方式呈现。</returns>
        /// <exception cref="ArgumentOutOfRangeException">当指定的列数超出范围时，则将会抛出这个异常。</exception>
        public T[] GetCol(int col)
        {
            if (col > Cols)
                throw new ArgumentOutOfRangeException(nameof(col), "Index out of range");
            else
            {
                T[] data = new T[Rows];
                int index = 0;
                for (int i = col; i < Length; i += Cols)
                {
                    data[index] = dataContainer[i];
                    index++;
                }
                return data;
            }
        }
        /// <summary>
        /// 在保留数据的前提下，重新设置当前二维表实例的尺寸。
        /// </summary>
        /// <param name="rows">新的总行数。</param>
        /// <param name="cols">新的总列数。</param>
        /// <exception cref="ArgumentOutOfRangeException">如果任意一个参数小于0，则将会抛出这个异常。</exception>
        public void Resize(int rows, int cols)
        {
            if (rows < 0 || cols < 0)
                throw new ArgumentOutOfRangeException("Size must greater than zero.");
            else
            {
                T[] dataCache = dataContainer;
                FillContainerSize(rows, cols);
                dataContainer = new T[Rows * Cols];
                Parallel.For(0, dataContainer.Length, i => dataContainer[i] = dataCache[i]);
            }
        }
        /// <summary>
        /// 清空当前二维表中的所有数据。
        /// </summary>
        public void Clear() => dataContainer = new T[Rows * Cols];
        /// <summary>
        /// 赋予Rows和Cols属性新的值。
        /// </summary>
        /// <param name="rows">Rows值。</param>
        /// <param name="cols">Cols值。</param>
        private void FillContainerSize(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
        }
        /// <summary>
        /// 获取指定行和指定列所对应单元格所在的索引。
        /// </summary>
        /// <param name="row">该单元格所在的行。</param>
        /// <param name="col">该单元格所在的列。</param>
        /// <returns>该操作将会返回一个Int32数据类型的值，表示这个单元所对应的索引。</returns>
        private int GetIndex(int row, int col) => (row - 1) * col + (col - 1);
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

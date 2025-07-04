using System;
using System.Numerics;
using Carlos.Enumerations;
namespace Carlos.Extends
{
    /// <summary>
    /// 傅里叶变换类，提供离散傅里叶变换（DFT）和快速傅里叶变换（FFT）的实现。
    /// </summary>
    public class Fourier
    {
        /// <summary>
        /// 计算离散傅里叶变换（DFT）。
        /// </summary>
        /// <param name="input">输入信号。</param>
        /// <returns>复数数组，表示频域信号。</returns>
        public static Complex[] DFT(Complex[] input)
        {
            int N = input.Length;
            Complex[] output = new Complex[N];
            for (int k = 0; k < N; k++)
            {
                output[k] = new Complex(0, 0);
                for (int n = 0; n < N; n++)
                {
                    double angle = -2.0 * Math.PI * k * n / N;
                    output[k] += input[n] * new Complex(Math.Cos(angle), Math.Sin(angle));
                }
            }
            return output;
        }
        /// <summary>
        /// 快速傅里叶变换（FFT）。
        /// </summary>
        /// <param name="input">输入信号。</param>
        /// <param name="direction">傅里叶变换操作的方向。</param>
        /// <returns>复数数组，表示频域信号。</returns>
        public static Complex[] FFT(Complex[] input, FourierDirection direction)
        {
            int n = input.Length;
            for (int i = 0, j = 0; i < n; i++)
            {
                if (i < j) (input[i], input[j]) = (input[j], input[i]);
                int m = n >> 1;
                while (m >= 1 && j >= m)
                {
                    j -= m;
                    m >>= 1;
                }
                j += m;
            }
            for (int s = 1; s <= Math.Log(n, 2); s++)
            {
                int m = 1 << s;
                double theta = 2 * Math.PI / m * (direction == FourierDirection.Forward ? 1 : -1);
                for (int k = 0; k < m / 2; k++)
                {
                    Complex w = new Complex(Math.Cos(k * theta), Math.Sin(k * theta));
                    for (int j = k; j < n; j += m)
                    {
                        int t = j + m / 2;
                        Complex u = input[j];
                        Complex v = input[t] * w;
                        input[j] = u + v;
                        input[t] = u - v;
                    }
                }
            }
            return input;
        }
    }
}

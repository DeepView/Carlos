using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Carlos.Extends
{
    public class Fourier
    {
        /// <summary>
        /// 计算离散傅里叶变换（DFT）
        /// </summary>
        /// <param name="input">输入信号</param>
        /// <returns>复数数组，表示频域信号</returns>
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
    }
}

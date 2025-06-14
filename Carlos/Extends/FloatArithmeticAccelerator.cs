using System;
using OpenCL.Net;
using Carlos.Exceptions;
using Carlos.Enumerations;
namespace Carlos.Extends
{
    /// <summary>
    /// FloatArithmeticAccelerator类用于执行浮点数的加减乘除运算，使用OpenCL进行加速。
    /// </summary>
    public class FloatArithmeticAccelerator
    {
        private string openClSourceCode = @"  
            __kernel void add(__global const float* a, __global const float* b, __global float* result, int size) {  
                int id = get_global_id(0);  
                if (id < size)
                    //for (int i = 0; i < size; i++)
                        result[id] = a[id] + b[id];
            }  
            __kernel void subtract(__global const float* a, __global const float* b, __global float* result, int size) {  
                int id = get_global_id(0);
                if (id < size)
                    result[id] = a[id] - b[id];
            }  
            __kernel void multiply(__global const float* a, __global const float* b, __global float* result, int size) {  
                int id = get_global_id(0);  
                if (id < size)
                    result[id] = a[id] * b[id];  
            }  
            __kernel void divide(__global const float* a, __global const float* b, __global float* result, int size) {  
                int id = get_global_id(0);  
                if (id < size){
                    if (b[id] != 0.0f) {  
                        result[id] = a[id] / b[id];  
                    } else {  
                        result[id] = 0.0f; // Handle division by zero  
                    }  
                }
            }  
        ";
        private Platform[] platforms;
        private Device[] devices;
        private Context context;
        private CommandQueue queue;
        private ErrorCode error;
        private Program program;
        private Kernel kernel;
        private IMem<double> aBuffer;
        private IMem<double> bBuffer;
        private IMem<double> resBuffer;
        /// <summary>
        /// 构造函数，初始化OpenCL环境和设备。
        /// </summary>
        public FloatArithmeticAccelerator()
        {
            int index = 0;
            Init(openClSourceCode, index);
        }
        /// <summary>
        /// 构造函数，初始化OpenCL环境和设备，指定设备索引。
        /// </summary>
        /// <param name="deviceIndex">指定的设备索引。</param>
        public FloatArithmeticAccelerator(int deviceIndex) => Init(openClSourceCode, deviceIndex);
        /// <summary>
        /// 构造函数，初始化OpenCL环境和设备，使用指定的OpenCL源代码和设备索引。
        /// </summary>
        /// <param name="clSrcCode">指定的OpenCL源代码。</param>
        /// <param name="deviceIndex">指定的设备索引。</param>
        public FloatArithmeticAccelerator(string clSrcCode, int deviceIndex = 0) => Init(clSrcCode, deviceIndex);
        /// <summary>
        /// 获取或设置OpenCL源代码。
        /// </summary>
        public string OpenClSourceCode
        {
            get => openClSourceCode;
            set => openClSourceCode = value;
        }
        /// <summary>
        /// 检查OpenCL源代码是否为空，或者是否为空白字符串。
        /// </summary>
        public bool SourceCodeIsEmpty => string.IsNullOrWhiteSpace(OpenClSourceCode);
        /// <summary>
        /// 获取当前OpenCL平台列表。
        /// </summary>
        public Device[] Devices => devices;
        /// <summary>
        /// 打印当前OpenCL平台和设备信息到控制台。
        /// </summary>
        public void PrintDeviceInfos()
        {
            foreach (var platform in platforms)
            {
                Console.WriteLine("Platform: " + Cl.GetPlatformInfo(platform, PlatformInfo.Name, out _));
                devices = Cl.GetDeviceIDs(platform, DeviceType.Gpu, out _);
                if (error != ErrorCode.Success)
                {
                    Console.WriteLine("Error getting device IDs: " + error);
                    continue;
                }
                foreach (var device in devices)
                {
                    Console.WriteLine($"  Device: {Cl.GetDeviceInfo(device, DeviceInfo.Name, out _)}");
                    Console.WriteLine($"  Device Type: {Cl.GetDeviceInfo(device, DeviceInfo.Type, out _)}");
                    Console.WriteLine($"  Max Compute Units: {Cl.GetDeviceInfo(device, DeviceInfo.MaxComputeUnits, out _)}");
                    Console.WriteLine($"  Max Work Group Size: {Cl.GetDeviceInfo(device, DeviceInfo.MaxWorkGroupSize, out _)}");
                    Console.WriteLine($"  Global Memory Size: {Cl.GetDeviceInfo(device, DeviceInfo.GlobalMemSize, out _)} bytes");
                    Console.WriteLine($"  Local Memory Size: {Cl.GetDeviceInfo(device, DeviceInfo.LocalMemSize, out _)} bytes");
                    Console.WriteLine($"  Max Clock Frequency: {Cl.GetDeviceInfo(device, DeviceInfo.MaxClockFrequency, out _)} MHz");
                    Console.WriteLine($"  Driver Version: {Cl.GetDeviceInfo(device, DeviceInfo.DriverVersion, out _)}");
                    Console.WriteLine();
                }
            }
        }
        /// <summary>
        /// 构建OpenCL程序。
        /// </summary>
        /// <exception cref="InvalidOperationException">如果OpenCL代码为空，或者为空白字符串时，则将会抛出这个异常。</exception>
        /// <exception cref="OpenClException">如果发生了一切与OpenCL相关的错误，则将会抛出这个异常。</exception>
        public void Build()
        {
            if (SourceCodeIsEmpty)
                throw new InvalidOperationException("OpenCL source code is empty.");
            program = Cl.CreateProgramWithSource(context, 1, new[] { OpenClSourceCode }, null, out error);
            error = Cl.BuildProgram(program, 0, null, string.Empty, null, IntPtr.Zero);
            if (error != ErrorCode.Success)
                throw new OpenClException(error);
        }
        /// <summary>
        /// 执行基本的算术运算（加、减、乘、除）并返回结果数组。
        /// </summary>
        /// <param name="left">表达式的左值。</param>
        /// <param name="right">表达式的右值。</param>
        /// <param name="opt">指定的运算符。</param>
        /// <returns>该操作将会返回一个double数组，用于存储指定参数参与运算之后的结果。</returns>
        public double[] Execute(double[] left, double[] right, BasicArithmeticOperations opt)
        {
            double[] result = new double[right.Length];
            aBuffer = Cl.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, left, out error);
            bBuffer = Cl.CreateBuffer(context, MemFlags.ReadOnly | MemFlags.CopyHostPtr, right, out error);
            resBuffer = Cl.CreateBuffer<double>(context, MemFlags.WriteOnly, result.Length, out error);
            kernel = Cl.CreateKernel(program, EnumerationDescriptionAttribute.GetEnumDescription(opt), out error);
            Cl.SetKernelArg(kernel, 0, aBuffer);
            Cl.SetKernelArg(kernel, 1, bBuffer);
            Cl.SetKernelArg(kernel, 2, resBuffer);
            Cl.SetKernelArg(kernel, 3, right.Length);
            Cl.EnqueueNDRangeKernel(queue, kernel, 1, null, new[] { (IntPtr)right.Length }, null, 0, null, out _);
            Cl.EnqueueReadBuffer(queue, resBuffer, Bool.True, result, 0, null, out _);
            Cl.Finish(queue);
            return result;
        }
        /// <summary>
        /// 释放OpenCL资源。
        /// </summary>
        public void Dispose()
        {
            Cl.ReleaseKernel(kernel);
            Cl.ReleaseProgram(program);
            Cl.ReleaseMemObject(aBuffer);
            Cl.ReleaseMemObject(bBuffer);
            Cl.ReleaseMemObject(resBuffer);
            Cl.ReleaseCommandQueue(queue);
            Cl.ReleaseContext(context);
        }
        /// <summary>
        /// 初始化OpenCL环境和设备。
        /// </summary>
        /// <param name="clSrcCode">指定的OpenCL源代码。</param>
        /// <param name="deviceIndex">指定的设备索引。</param>
        private void Init(string clSrcCode, int deviceIndex)
        {
            OpenClSourceCode = clSrcCode;
            platforms = Cl.GetPlatformIDs(out error);
            devices = Cl.GetDeviceIDs(platforms[0], DeviceType.Gpu, out error);
            context = Cl.CreateContext(null, (uint)(deviceIndex + 1), devices, null, IntPtr.Zero, out error);
            queue = Cl.CreateCommandQueue(context, devices[deviceIndex], CommandQueueProperties.None, out error);
        }
    }
}

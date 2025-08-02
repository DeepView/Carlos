using System;
using OpenCL.Net;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Carlos.Compute
{
    /// <summary>
    /// 一个基于OpenCL的计算加速器类，用于执行GPU加速的计算任务。
    /// </summary>
    public class OpenCLAccelerator : IDisposable
    {
        private Context _context;
        private CommandQueue _commandQueue;
        private Program _program;
        private Kernel _kernel;
        private Dictionary<string, IMem> _memoryBuffers = new Dictionary<string, IMem>();
        private Device _device;
        private Platform _platform;
        /// <summary>
        /// 获取或设置当前OpenCL内核。
        /// </summary>
        public Kernel Kernel => _kernel;
        /// <summary>
        /// 获取或设置当前OpenCL命令队列。
        /// </summary>
        public CommandQueue CommandQueue => _commandQueue;
        /// <summary>
        /// 构造函数，创建一个OpenCL加速器实例，并初始化OpenCL环境。
        /// </summary>
        public OpenCLAccelerator()
        {
            InitializeOpenCL();
        }
        /// <summary>
        /// 初始化OpenCL环境，包括获取平台、设备、创建上下文和命令队列等操作。
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void InitializeOpenCL()
        {
            try
            {
                // 获取平台
                ErrorCode errorCode;
                Platform[] platforms = Cl.GetPlatformIDs(out errorCode);
                CheckError(errorCode);
                if (platforms.Length == 0)
                {
                    throw new Exception("未找到OpenCL平台");
                }
                // 获取第一个可用平台
                _platform = platforms[0];
                InfoBuffer platformNameBuffer = Cl.GetPlatformInfo(_platform, PlatformInfo.Name, out errorCode);
                string platformName = platformNameBuffer.ToString();
                Console.WriteLine("使用OpenCL平台: " + platformName);
                CheckError(errorCode);
                // 获取设备
                Device[] devices = Cl.GetDeviceIDs(_platform, DeviceType.Gpu, out errorCode);
                CheckError(errorCode);

                if (devices.Length == 0)
                {
                    devices = Cl.GetDeviceIDs(_platform, DeviceType.Cpu, out errorCode);
                    CheckError(errorCode);
                }
                if (devices.Length == 0)
                {
                    throw new Exception("未找到OpenCL设备");
                }
                // 获取第一个可用设备
                _device = devices[0];
                InfoBuffer deviceNameBuffer = Cl.GetDeviceInfo(_device, DeviceInfo.Name, out errorCode);
                string deviceName = deviceNameBuffer.ToString();
                Console.WriteLine("使用OpenCL设备: " + deviceName);
                CheckError(errorCode);
                // 创建上下文
                _context = Cl.CreateContext(null, 1, new[] { _device }, null, IntPtr.Zero, out errorCode);
                CheckError(errorCode);
                // 创建命令队列
                _commandQueue = Cl.CreateCommandQueue(_context, _device, CommandQueueProperties.None, out errorCode);
                CheckError(errorCode);
            }
            catch (Exception ex)
            {
                throw new Exception("初始化OpenCL失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 编译OpenCL内核。
        /// </summary>
        /// <param name="kernelCode">内核代码字符串。</param>
        /// <param name="kernelName">内核函数名。</param>
        public void CompileKernel(string kernelCode, string kernelName)
        {
            try
            {
                // 创建程序
                _program = Cl.CreateProgramWithSource(_context, 1, new[] { kernelCode }, null, out ErrorCode errorCode);
                CheckError(errorCode);
                // 编译程序
                errorCode = Cl.BuildProgram(_program, 0, null, null, null, IntPtr.Zero);
                CheckError(errorCode);
                // 检查编译日志
                InfoBuffer buildLogBuffer = Cl.GetProgramBuildInfo(_program, _device, ProgramBuildInfo.Log, out errorCode);
                CheckError(errorCode);
                // 创建内核
                _kernel = Cl.CreateKernel(_program, kernelName, out errorCode);
                CheckError(errorCode);
            }
            catch (Exception ex)
            {
                throw new Exception("编译OpenCL内核失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 创建OpenCL缓冲区。
        /// </summary>
        /// <param name="bufferName">缓冲区名称。</param>
        /// <param name="flags">缓冲区标志。</param>
        /// <param name="sizeInBytes">缓冲区大小(字节)。</param>
        /// <param name="data">初始化数据指针。</param>
        public void CreateBuffer(string bufferName, MemFlags flags, uint sizeInBytes, IntPtr data = default)
        {
            try
            {
                IMem buffer = Cl.CreateBuffer(
                    _context, 
                    flags, 
                    new IntPtr((long)sizeInBytes), 
                    data, 
                    out ErrorCode errorCode
                );
                CheckError(errorCode);
                _memoryBuffers[bufferName] = buffer;
            }
            catch (Exception ex)
            {
                throw new Exception($"创建缓冲区 '{{bufferName}}' 失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 设置内核参数。
        /// </summary>
        /// <param name="bufferName">缓冲区名称。</param>
        /// <param name="argIndex">参数索引。</param>
        public void SetKernelArgument(string bufferName, uint argIndex)
        {
            try
            {
                if (!_memoryBuffers.TryGetValue(bufferName, out IMem buffer))
                {
                    throw new Exception($"找不到缓冲区 '{{bufferName}}'");
                }
                ErrorCode errorCode = Cl.SetKernelArg(_kernel, argIndex, buffer);
                CheckError(errorCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"设置内核参数失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 执行OpenCL内核。
        /// </summary>
        /// <param name="globalWorkSizeX">X方向的全局工作大小。</param>
        /// <param name="globalWorkSizeY">Y方向的全局工作大小。</param>
        /// <param name="globalWorkSizeZ">Z方向的全局工作大小。</param>
        /// <param name="localWorkSizeX">X方向的本地工作大小。</param>
        /// <param name="localWorkSizeY">Y方向的本地工作大小。</param>
        /// <param name="localWorkSizeZ">Z方向的本地工作大小。</param>
        public void Execute(int globalWorkSizeX, int globalWorkSizeY = 1, int globalWorkSizeZ = 1, int localWorkSizeX = 1, int localWorkSizeY = 1, int localWorkSizeZ = 1)
        {
            try
            {
                IntPtr[] globalWorkSize =
                {
                    new IntPtr(globalWorkSizeX),
                    new IntPtr(globalWorkSizeY),
                    new IntPtr(globalWorkSizeZ)
                };
                IntPtr[] localWorkSize =
                {
                    new IntPtr(localWorkSizeX),
                    new IntPtr(localWorkSizeY),
                    new IntPtr(localWorkSizeZ)
                };
                Event kernelEvent;
                ErrorCode errorCode = Cl.EnqueueNDRangeKernel(
                    _commandQueue, 
                    _kernel, 
                    3, 
                    null, 
                    globalWorkSize, 
                    localWorkSize, 
                    0, 
                    null, 
                    out kernelEvent
                );
                CheckError(errorCode);
            }
            catch (Exception ex)
            {
                throw new Exception("执行OpenCL内核失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 从OpenCL缓冲区读取数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="bufferName">缓冲区名称。</param>
        /// <param name="elementCount">元素数量。</param>
        /// <returns>读取的数据数组。</returns>
        public T[] ReadBuffer<T>(string bufferName, int elementCount)
        {
            try
            {
                if (!_memoryBuffers.TryGetValue(bufferName, out IMem buffer))
                {
                    throw new Exception($"找不到缓冲区 '{{bufferName}}'");
                }
                int elementSize = Marshal.SizeOf(typeof(T));
                T[] data = new T[elementCount];
                GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                IntPtr dataPtr = dataHandle.AddrOfPinnedObject();
                ErrorCode errorCode = Cl.EnqueueReadBuffer(
                    _commandQueue, 
                    buffer, 
                    Bool.True, 
                    IntPtr.Zero,
                    new IntPtr(elementCount * elementSize), 
                    dataPtr, 
                    0, 
                    null, 
                    out Event readEvent
                );
                CheckError(errorCode);
                dataHandle.Free();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"读取缓冲区 '{{bufferName}}' 失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 向OpenCL缓冲区写入数据。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="bufferName">缓冲区名称。</param>
        /// <param name="data">要写入的数据数组。</param>
        public void WriteBuffer<T>(string bufferName, T[] data)
        {
            try
            {
                if (!_memoryBuffers.TryGetValue(bufferName, out IMem buffer))
                {
                    throw new Exception($"找不到缓冲区 '{{bufferName}}'");
                }
                int elementSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
                System.Runtime.InteropServices.GCHandle dataHandle = System.Runtime.InteropServices.GCHandle.Alloc(data, System.Runtime.InteropServices.GCHandleType.Pinned);
                IntPtr dataPtr = dataHandle.AddrOfPinnedObject();
                ErrorCode errorCode = Cl.EnqueueWriteBuffer(
                    _commandQueue, 
                    buffer, 
                    Bool.True, 
                    IntPtr.Zero,
                    new IntPtr(data.Length * elementSize), 
                    dataPtr, 0, 
                    null, 
                    out Event writeEvent
                );
                CheckError(errorCode);
                dataHandle.Free();
            }
            catch (Exception ex)
            {
                throw new Exception($"写入缓冲区 '{{bufferName}}' 失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 检查OpenCL错误。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        private void CheckError(ErrorCode errorCode)
        {
            if (errorCode != ErrorCode.Success)
            {
                throw new Exception($"OpenCL错误: {errorCode}");
            }
        }
        /// <summary>
        /// 释放OpenCL资源。
        /// </summary>
        public void Dispose()
        {
            if (!object.Equals(_kernel, default(Kernel)))
            {
                Cl.ReleaseKernel(_kernel);
                _kernel = default;
            }
            if (!object.Equals(_program, default(Program)))
            {
                Cl.ReleaseProgram(_program);
                _program = default;
            }
            foreach (var buffer in _memoryBuffers.Values)
            {
                if (!object.Equals(buffer, default(IMem)))
                {
                    Cl.ReleaseMemObject(buffer);
                }
            }
            _memoryBuffers.Clear();
            if (!object.Equals(_commandQueue, default(CommandQueue)))
            {
                Cl.ReleaseCommandQueue(_commandQueue);
                _commandQueue = default;
            }
            if (!object.Equals(_context, default(Context)))
            {
                Cl.ReleaseContext(_context);
                _context = default;
            }
        }
    }
}

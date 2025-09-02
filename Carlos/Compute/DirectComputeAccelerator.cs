using System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using dev = SharpDX.Direct3D11.Device;
using buffer = SharpDX.Direct3D11.Buffer;
namespace Carlos.Compute
{
    /// <summary>
    /// DirectCompute GPU加速计算类，提供了执行任意GPU加速代码的功能。
    /// </summary>
    /// <remarks>
    /// 以下代示例代码用于展示如何使用DirectComputeAccelerator类：
    /// <code language="csharp">
    /// static void Main(string[] args)
    /// {
    ///     Console.WriteLine("DirectCompute GPU 加速计算测试");
    ///     int dataSize = 10;          // 测试数据大小
    ///     float[] gpuResults = null;  // 用于存储GPU计算结果
    ///     try
    ///     {
    ///         using (var accelerator = new DirectComputeAccelerator())
    ///         {
    ///             try
    ///             {
    ///                 // 获取当前程序目录
    ///                 string currentDirectory = System.IO.Directory.GetCurrentDirectory();
    ///                 string shaderFilePath = System.IO.Path.Combine(currentDirectory, "MinimalTest.hlsl");
    ///                 Console.WriteLine("尝试读取极简着色器文件: " + shaderFilePath);
    ///                 // 检查着色器文件是否存在
    ///                 if (!System.IO.File.Exists(shaderFilePath))
    ///                 {
    ///                     Console.WriteLine("错误: 着色器文件不存在: " + shaderFilePath);
    ///                     return;
    ///                 }
    ///                 string hlslCode = System.IO.File.ReadAllText(shaderFilePath);  // 读取着色器文件内容
    ///                 Console.WriteLine("成功读取简单着色器代码，长度: " + hlslCode.Length + " 字符");
    ///                 accelerator.CompileAndLoadComputeShader(hlslCode, "CSMain", "cs_4_0");  // 编译并加载计算着色器 - 使用较低版本的着色器模型以提高兼容性
    ///                 accelerator.SetOutputBuffer&lt;float&gt;(dataSize);     // 设置输出缓冲区 (我们的简单着色器不需要输入数据)
    ///                 accelerator.Execute(dataSize);  // 执行计算 (每个线程处理一个元素)
    ///                 gpuResults = accelerator.GetResult&lt;float&gt;(dataSize);  // 获取计算结果
    ///             }
    ///             catch (Exception ex)
    ///             {
    ///                 Console.WriteLine("GPU计算出错: " + ex.Message);
    ///                 Console.WriteLine("错误详情: " + ex.ToString());
    ///                 Console.WriteLine("内部异常: " + (ex.InnerException?.ToString() ?? "无"));
    ///                 return;
    ///             }
    ///         }
    ///         // 验证结果
    ///         if (gpuResults != null)
    ///         {
    ///             Console.WriteLine("GPU计算完成，结果:");
    ///             for (int i = 0; i &lt; dataSize; i++)
    ///             {
    ///                 float expected = i * 2.0f;
    ///                 Console.WriteLine($"索引 {i}: GPU结果 = {gpuResults[i]}, 预期结果 = {expected}");
    ///             }
    ///         }
    ///     }
    ///     catch (Exception ex)
    ///     {
    ///         Console.WriteLine("程序执行出错: " + ex.Message);
    ///         Console.WriteLine("错误详情: " + ex.ToString());
    ///     }
    ///     Console.WriteLine("测试完成，按任意键退出...");
    ///     Console.ReadKey();
    /// } 
    /// </code>
    /// 其中，MinimalTest.hlsl文件内容如下：
    /// <code language="hlsl">
    /// RWStructuredBuffer&lt;float&gt; OutputBuffer : register(u0);
    /// [numthreads(1, 1, 1)]
    /// void CSMain(uint3 DTid : SV_DispatchThreadID)
    /// {
    ///     OutputBuffer[DTid.x] = DTid.x * 2.0f;
    /// }
    /// </code>
    /// </remarks>    
    public class DirectComputeAccelerator : IDisposable
    {
        private dev _device;
        private DeviceContext _context;
        private ComputeShader _computeShader;
        private ShaderResourceView _inputResourceView;
        private UnorderedAccessView _outputResourceView;
        private buffer _inputBuffer;
        private buffer _outputBuffer;
        /// <summary>
        /// 构造函数，创建一个DirectComputeAccelerator实例。
        /// </summary>
        public DirectComputeAccelerator()
        {
            // 创建Direct3D设备和上下文
            // 这里我们指定使用硬件设备，并支持计算着色器
            var deviceFlags = DeviceCreationFlags.None;
#if DEBUG
            deviceFlags |= DeviceCreationFlags.Debug;
#endif
            // 指定支持的特性级别
            var featureLevels = new[]
            {
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0
            };
            // 创建设备
            _device = new dev(
                DriverType.Hardware,
                deviceFlags,
                featureLevels);
            _context = _device.ImmediateContext;
        }
        /// <summary>
        /// 加载计算着色器字节码。
        /// </summary>
        /// <param name="shaderCode">计算着色器的字节码。</param>
        public void LoadComputeShader(byte[] shaderCode)
        {
            // 释放旧的着色器（如果存在）
            _computeShader?.Dispose();
            try
            {
                // 创建新的计算着色器
                _computeShader = new ComputeShader(_device, shaderCode);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("创建计算着色器失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 编译并加载计算着色器。
        /// </summary>
        /// <param name="hlslCode">HLSL着色器代码。</param>
        /// <param name="entryPoint">着色器入口点函数名。</param>
        /// <param name="shaderModel">着色器模型版本 (例如: "cs_5_0")。</param>
        public void CompileAndLoadComputeShader(string hlslCode, string entryPoint = "CSMAIN", string shaderModel = "cs_5_0")
        {
            try
            {
                // 使用SharpDX的EffectCompiler编译HLSL代码
                var compiledShader = SharpDX.D3DCompiler.ShaderBytecode.Compile(
                    hlslCode,
                    entryPoint,
                    shaderModel,
                    ShaderFlags.Debug | ShaderFlags.SkipOptimization
                );
                // 检查编译是否成功
                if (compiledShader.HasErrors)
                {
                    throw new Exception("着色器编译错误: " + compiledShader.Message);
                }
                // 获取编译后的字节码
                byte[] shaderBytecode = compiledShader.Bytecode;
                Console.WriteLine("成功编译着色器，字节码大小: " + shaderBytecode.Length + " 字节");
                // 加载编译后的着色器字节码
                LoadComputeShader(shaderBytecode);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("编译着色器失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 加载计算着色器。
        /// </summary>
        /// <param name="shaderFilePath">计算着色器文件路径。</param>
        public void LoadComputeShader(string shaderFilePath)
        {
            // 检查文件是否存在
            if (!System.IO.File.Exists(shaderFilePath))
            {
                throw new System.IO.FileNotFoundException("着色器文件不存在: " + shaderFilePath);
            }
            try
            {
                // 读取着色器文件内容
                byte[] shaderCode = System.IO.File.ReadAllBytes(shaderFilePath);
                Console.WriteLine("成功读取着色器文件，大小: " + shaderCode.Length + " 字节");
                LoadComputeShader(shaderCode);
            }
            catch (Exception ex)
            {
                throw new Exception("加载着色器文件失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 设置输入数据。
        /// </summary>
        /// <typeparam name="T">输入数据类型。</typeparam>
        /// <param name="inputData">输入数据数组。</param>
        public void SetInputData<T>(T[] inputData) where T : struct
        {
            // 释放旧的资源（如果存在）
            _inputBuffer?.Dispose();
            _inputResourceView?.Dispose();
            // 创建输入缓冲区
            _inputBuffer = buffer.Create<T>(_device, BindFlags.ShaderResource, inputData);
            // 创建着色器资源视图
            _inputResourceView = new ShaderResourceView(_device, _inputBuffer);
        }
        /// <summary>
        /// 设置输出缓冲区。
        /// </summary>
        /// <typeparam name="T">输出数据类型。</typeparam>
        /// <param name="outputSize">输出数据大小。</param>
        public void SetOutputBuffer<T>(int outputSize) where T : struct
        {
            // 释放旧的输出缓冲区和视图（如果存在）
            _outputBuffer?.Dispose();
            _outputResourceView?.Dispose();
            try
            {
                // 创建输出缓冲区描述
                BufferDescription outputBufferDesc = new BufferDescription
                {
                    Usage = ResourceUsage.Default,
                    SizeInBytes = outputSize * Utilities.SizeOf<T>(),
                    BindFlags = BindFlags.UnorderedAccess | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.BufferStructured,
                    StructureByteStride = Utilities.SizeOf<T>()
                };
                // 创建输出缓冲区
                _outputBuffer = new buffer(_device, outputBufferDesc);
                Console.WriteLine("成功创建输出缓冲区，大小: " + outputBufferDesc.SizeInBytes + " 字节");
                // 创建无序访问视图描述
                UnorderedAccessViewDescription uavDesc = new UnorderedAccessViewDescription
                {
                    Format = SharpDX.DXGI.Format.Unknown,
                    Dimension = UnorderedAccessViewDimension.Buffer,
                    Buffer = new UnorderedAccessViewDescription.BufferResource
                    {
                        FirstElement = 0,
                        ElementCount = outputSize
                    }
                };
                // 创建输出资源视图
                _outputResourceView = new UnorderedAccessView(_device, _outputBuffer, uavDesc);
                Console.WriteLine("成功创建无序访问视图");
            }
            catch (Exception ex)
            {
                throw new Exception("创建输出缓冲区或视图失败: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 执行计算着色器。
        /// </summary>
        /// <param name="threadGroupCountX">X方向的线程组数量。</param>
        /// <param name="threadGroupCountY">Y方向的线程组数量。</param>
        /// <param name="threadGroupCountZ">Z方向的线程组数量。</param>
        public void Execute(int threadGroupCountX, int threadGroupCountY = 1, int threadGroupCountZ = 1)
        {
            if (_computeShader == null)
                throw new InvalidOperationException("计算着色器未加载");
            if (_outputResourceView == null)
                throw new InvalidOperationException("输出缓冲区未设置");
            // 设置计算着色器
            _context.ComputeShader.Set(_computeShader);
            // 设置无序访问视图
            _context.ComputeShader.SetUnorderedAccessView(0, _outputResourceView);
            // 如果有输入资源，则设置
            if (_inputResourceView != null)
            {
                _context.ComputeShader.SetShaderResource(0, _inputResourceView);
            }
            // 执行计算着色器
            _context.Dispatch(threadGroupCountX, threadGroupCountY, threadGroupCountZ);
            // 清除绑定
            _context.ComputeShader.SetUnorderedAccessView(0, null);
            if (_inputResourceView != null)
            {
                _context.ComputeShader.SetShaderResource(0, null);
            }
        }
        /// <summary>
        /// 获取计算结果。
        /// </summary>
        /// <typeparam name="T">结果数据类型。</typeparam>
        /// <param name="resultSize">结果数据大小。</param>
        /// <returns>计算结果数组。</returns>
        public T[] GetResult<T>(int resultSize) where T : struct
        {
            if (_outputBuffer == null)
                throw new InvalidOperationException("输出缓冲区未设置或计算尚未执行");
            // 创建暂存缓冲区用于读取数据
            var stagingBuffer = new buffer(
                _device,
                _outputBuffer.Description.SizeInBytes,
                ResourceUsage.Staging,
                BindFlags.None,
                CpuAccessFlags.Read,
                ResourceOptionFlags.None,
                0
            );
            // 将数据从GPU复制到CPU
            _context.CopyResource(_outputBuffer, stagingBuffer);
            // 映射暂存缓冲区以读取数据
            var dataBox = _context.MapSubresource(stagingBuffer, 0, MapMode.Read, MapFlags.None);
            // 创建结果数组
            T[] result = new T[resultSize];
            // 复制数据到结果数组
            Utilities.Read(dataBox.DataPointer, result, 0, resultSize);
            // 取消映射
            _context.UnmapSubresource(stagingBuffer, 0);
            // 释放暂存缓冲区
            stagingBuffer.Dispose();
            return result;
        }
        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            _inputResourceView?.Dispose();
            _outputResourceView?.Dispose();
            _inputBuffer?.Dispose();
            _outputBuffer?.Dispose();
            _computeShader?.Dispose();
            _context?.Dispose();
            _device?.Dispose();
        }
    }
}

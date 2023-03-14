using System;
using System.Diagnostics;
namespace Carlos
{
    /// <summary>
    /// 用于描述代码执行所需时间的时间戳返回结果的类，基于IDisposable接口实现，如果您在控制台应用程序或者应用程序调试过程中需要使用此类，可以考虑使用IDisposable接口模式进行访问。
    /// </summary>
    /// <example>
    /// 该类的建议使用方法如下代码所示：
    /// <code>
    /// CodeExecutedTimespanResult result = new CodeExecutedTimespanResult("NeedExecutedCodes");
    /// //This is your code...
    /// result.StopMeasure();
    /// Console.WriteLine("ExecutedTime = {0}", result.ExecuteTime);
    /// Console.WriteLine("Timestamp = {0}", result.ExecutedTimespan);
    /// </code>
    /// 如果您正在控制台应用程序使用这个类，则示例代码如下所示：
    /// <code>
    /// using (CodeExecutedTimespanResult result = new CodeExecutedTimespanResult("Debug"))
    /// {
    /// //This is your code...
    /// }
    /// </code>
    /// 如果这段代码是在该类通过了DEBUG条件编译的情况下，那么上述代码会在控制台显示其运行时间，其具体实现请参考当前类的Dispose方法。
    /// </example>
    /// <remarks>虽然这个类的执行时间统计的精度可能略差于<c>Carlos.CodeHelper.ExecDuration(System.Action)</c>方法，但是误差不大，在性能不错的计算机平台上，其误差相较于其可以控制在1~10毫秒之内，不过该类更适用于调试应用程序或者代码片段的场景，如果需要在其他非调试场合下使用这个类，建议不要采用using模式。</remarks>
    public class CodeExecutedTimespanResult : IDisposable
   {
      /// <summary>
      /// 构造函数，创建一个默认的CodeExecutedTimestampResult实例。
      /// </summary>
      public CodeExecutedTimespanResult()
      {
         Stopwatch = new Stopwatch();
         ExecuteTime = DateTime.Now;
         Stopwatch.Start();
         CodeName = $"CodeExecutedTime_{ExecuteTime.Ticks}";
         ExecutedTimespan = 0;
      }
      /// <summary>
      /// 构造函数，创建一个指定所执行代码描述文本或者注释文本的CodeExecutedTimestampResult实例。
      /// </summary>
      /// <param name="codeName">所执行的代码的描述文本或者注释文本。</param>
      public CodeExecutedTimespanResult(string codeName)
      {
         Stopwatch = new Stopwatch();
         ExecuteTime = DateTime.Now;
         Stopwatch.Start();
         CodeName = codeName;
         ExecutedTimespan = 0;
      }
      /// <summary>
      /// 获取或设置当前实例所执行的代码的描述文本或者注释文本。
      /// </summary>
      public string CodeName { get; set; }
      /// <summary>
      /// 获取当前实例在执行代码时所花费的时长。
      /// </summary>
      public long ExecutedTimespan { get; private set; }
      /// <summary>
      /// 获取当前实例在代码开始执行的时候的本地时间。
      /// </summary>
      public DateTime ExecuteTime { get; private set; }
      /// <summary>
      /// 获取当前实例用于准确测量代码执行时长的实例。
      /// </summary>
      public Stopwatch Stopwatch { get; private set; }
      /// <summary>
      /// 结束测量某一段代码的执行时间间隔或者运行时长。
      /// </summary>
      public void StopMeasure()
      {
         Stopwatch.Stop();
         ExecutedTimespan = Stopwatch.ElapsedMilliseconds;
      }
      /// <summary>
      /// 手动释放该对象引用的所有内存资源。
      /// </summary>
      public void Dispose()
      {
         StopMeasure();
#if DEBUG
         Console.WriteLine("{0}'s running time is {1} milliseconds.", CodeName, ExecutedTimespan);
#endif
#if CONSOLE
         Console.Write("Press any key to continue...");
         Console.ReadKey(false);
#endif
         Stopwatch = null;
         CodeName = null;
         GC.SuppressFinalize(this);
      }
   }
}

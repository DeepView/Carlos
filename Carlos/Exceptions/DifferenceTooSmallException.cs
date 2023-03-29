using System;
using System.Runtime.Serialization;
namespace Carlos.Exceptions
{
    /// <summary>
    /// 如果指定的年份范围太小而抛出的异常。
    /// </summary>
    public class DifferenceTooSmallException : Exception
    {
        public DifferenceTooSmallException() { }
        public DifferenceTooSmallException(string message) : base(message) { }
        public DifferenceTooSmallException(string message, Exception inner) : base(message, inner) { }
        protected DifferenceTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

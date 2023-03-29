using System;
using System.Runtime.Serialization;
namespace Carlos.Exceptions
{
    /// <summary>
    /// 大型日期任意成分超出范围时而抛出的异常。
    /// </summary>
    public class DateOverflowException : Exception
    {
        public DateOverflowException() { }
        public DateOverflowException(string message) : base(message) { }
        public DateOverflowException(string message, Exception inner) : base(message, inner) { }
        protected DateOverflowException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

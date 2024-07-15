using System;
using System.Runtime.Serialization;
namespace Carlos.Exceptions
{
    /// <summary>
    /// 当尺寸设置超出合理范围或者设置无效时需要抛出的异常。
    /// </summary>
    public class InvalidSizeException : Exception
    {
        public InvalidSizeException() :base("Invalid setting of size.") { }

        public InvalidSizeException(string message) : base(message) { }

        public InvalidSizeException(string message, Exception innerException) : base(message, innerException) { }

        protected InvalidSizeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

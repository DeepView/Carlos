using System;
using System.Runtime.Serialization;
namespace Carlos.Exceptions
{
    /// <summary>
    /// 当SQL语句为NULL的时候抛出的异常。
    /// </summary>
    [Serializable]
    public class EmptySqlCommandLineException : Exception
    {
        public EmptySqlCommandLineException() : base("不允许空的SQL语句！") { }
        public EmptySqlCommandLineException(string message) : base(message) { }
        public EmptySqlCommandLineException(string message, Exception inner) : base(message, inner) { }
        protected EmptySqlCommandLineException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

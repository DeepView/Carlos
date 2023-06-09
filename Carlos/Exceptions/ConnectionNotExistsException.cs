using System;
using System.Runtime.Serialization;
namespace Carlos.Exceptions
{
    /// <summary>
    /// 数据库连接不存在或者已断开连接时而抛出的异常。
    /// </summary>
    [Serializable]
    public class ConnectionNotExistsException : Exception
    {
        public ConnectionNotExistsException() : base("数据库未连接或者连接已断开！") { }
        public ConnectionNotExistsException(string message) : base(message) { }
        public ConnectionNotExistsException(string message, Exception inner) : base(message, inner) { }
        protected ConnectionNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

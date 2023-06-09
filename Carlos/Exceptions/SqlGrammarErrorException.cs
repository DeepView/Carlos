using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Carlos.Exceptions
{
    /// <summary>
    /// 当SQL语句出现语法错误或者其他异常情况的时候而抛出的异常。
    /// </summary>
    [Serializable]
    public class SqlGrammarErrorException : Exception
    {
        public SqlGrammarErrorException() : base("SQL语法错误或者出现了其他异常！") { }
        public SqlGrammarErrorException(string message) : base(message) { }
        public SqlGrammarErrorException(string message, Exception inner) : base(message, inner) { }
        protected SqlGrammarErrorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

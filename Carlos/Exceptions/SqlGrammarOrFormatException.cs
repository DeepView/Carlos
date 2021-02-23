using System;
namespace Carlos.Exceptions
{
   /// <summary>
   /// 当SQL语句的语法或者格式出现问题时，则抛出这个异常。
   /// </summary>
   [Serializable]
   public class SqlGrammarOrFormatException : ArgumentException
   {
      public SqlGrammarOrFormatException() : base("Incorrect SQL syntax or format.") { }
      public SqlGrammarOrFormatException(string message, string argName) : base(message, argName) { }
      public SqlGrammarOrFormatException(string message) : base(message) { }
      public SqlGrammarOrFormatException(string message, Exception inner) : base(message, inner) { }
      protected SqlGrammarOrFormatException(
       System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}

using System;
namespace Carlos.Exceptions
{
   /// <summary>
   /// 无法解密文本，数据流，文件或者信道时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class CannotDecryptException : Exception
   {
      public CannotDecryptException():base("Cannot decrypt this string, data stream, file or channel.") { }
      public CannotDecryptException(string message) : base(message) { }
      public CannotDecryptException(string message, Exception inner) : base(message, inner) { }
      protected CannotDecryptException(
       System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}

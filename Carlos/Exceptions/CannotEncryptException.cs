using System;
namespace Carlos.Exceptions
{
   /// <summary>
   /// 无法加密文本，数据流，文件或者信道时需要抛出的异常。
   /// </summary>
   [Serializable]
   public class CannotEncryptException : Exception
   {
      public CannotEncryptException():base("Cannot encrypt this string, data stream, file or channel.") { }
      public CannotEncryptException(string message) : base(message) { }
      public CannotEncryptException(string message, Exception inner) : base(message, inner) { }
      protected CannotEncryptException(
       System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}

using System;
namespace Carlos.Exceptions
{
   /// <summary>
   /// 当数据类型无法被转换时需要抛出的异常。
   /// </summary>
   public class TypeCannotConvertException : Exception
   {
      public TypeCannotConvertException() : base("Cannot convert this data type.") { }
      public TypeCannotConvertException(Type type) : this($"Cannot convert this data type. [type name: {type.FullName}]") { }
      public TypeCannotConvertException(string message) : base(message) { }
      public TypeCannotConvertException(string message, Exception inner) : base(message, inner) { }
      protected TypeCannotConvertException(
       System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}

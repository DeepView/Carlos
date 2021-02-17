using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Exceptions
{

   [Serializable]
   public class CannotSendMailException : Exception
   {
      public CannotSendMailException() : base("Cannot send this mail, please try again.") { }
      public CannotSendMailException(string message) : base(message) { }
      public CannotSendMailException(string message, Exception inner) : base(message, inner) { }
      protected CannotSendMailException(
       System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}

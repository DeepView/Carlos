using System;
using OpenCL.Net;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Exceptions
{
    [System.Serializable]
    public class OpenClException : Cl.Exception
    {
        public OpenClException(ErrorCode error) : base(error)
        {
        }
    }
}

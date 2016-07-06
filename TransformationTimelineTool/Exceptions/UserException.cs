using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Exceptions
{
    public class UserException : Exception
    {
        public UserException() : base() { }
        public UserException(string message) : base(message) { }
        public UserException(string message, Exception inner) : base(message, inner) { }

        protected UserException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }

    }
}
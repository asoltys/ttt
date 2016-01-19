using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Exceptions
{
    public class SendMailException: Exception
    {
        public SendMailException()
        {

        }
        public SendMailException(string message) : base(message)
        {

        }

        public SendMailException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
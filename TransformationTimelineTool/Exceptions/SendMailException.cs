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
        public SendMailException(string message)
        {

        }

        public SendMailException(string message, Exception inner)
        {

        }
    }
}
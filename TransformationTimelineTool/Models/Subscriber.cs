using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class Subscriber
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<Initiative> Initiatives { get; set; }
    }
}
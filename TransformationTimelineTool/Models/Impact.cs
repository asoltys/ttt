using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class Impact
    {
        public int ID { get; set; }
        public int Level { get; set; }
        public virtual Initiative Initiative { get; set; }
        public virtual Branch Branch { get; set; }
    }
}
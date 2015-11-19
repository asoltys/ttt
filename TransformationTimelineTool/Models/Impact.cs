using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public enum Level
    {
        None, Low, Medium, High
    }
    public class Impact
    {
        public int ID { get; set; }
        public Level Level { get; set; }
        public virtual Initiative Initiative { get; set; }
        public virtual Branch Branch { get; set; }
    }
}
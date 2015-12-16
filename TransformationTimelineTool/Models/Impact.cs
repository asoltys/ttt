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
    public class Impact : Category
    {
        public Level Level { get; set; }
    }
}
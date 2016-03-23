using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Helpers
{
    public class EmailInitiative
    {
        public string NameE { get; set; }
        public string NameF { get; set; }
        public List<EmailActivies> ChangedActivities { get; set; }


    }

    public class EmailActivies
    {
        public string HoverE { get; set; }
        public string HoverF { get; set; }
        public DateTime Date { get; set; }

    }

    public class EmailImpacts
    {
        public string Level { get; set; }
    }
}
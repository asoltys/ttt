using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace TransformationTimelineTool.ViewModels
{
    public class BranchesData
    {

        public string Name
        {
            get
            {
                if (Thread.CurrentThread.CurrentCulture.Name == "fr")
                {
                    return NameF;
                }
                else
                {
                    return NameE;
                }
            }
        }
        public int ID { get; set; }
        public string NameE { get; set; }
        public string NameF { get; set; }
        public bool Flag { get; set; }
    }
}
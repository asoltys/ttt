using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class ImpactViewModel
    {
        public SelectList InitiativeSelect { get; set; }
        public ImpactViewModel()
        {

        }
        public int ID { get; set; }
        public int InitiativeID { get; set; }
        public Level Level { get; set; }
        public string Justification { get; set; }
        public Impact Impact { get; set; }
        public Event Event { get; set; }
        public Edit Edit { get; set; }
    }
}
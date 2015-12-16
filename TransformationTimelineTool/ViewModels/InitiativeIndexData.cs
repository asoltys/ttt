using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class InitiativeIndexData
    {
        public IEnumerable<Initiative> Initiatives { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<Impact> Impacts { get; set; }
    }
}
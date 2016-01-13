using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;

namespace TransformationTimelineTool.ViewModels
{
    public class EventData
    {
      public Edit GetLatestEdit()
        {
            return new Edit();
        }
    }
}
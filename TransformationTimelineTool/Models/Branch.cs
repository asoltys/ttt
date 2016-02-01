﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class Branch
    {
        public int ID { get; set; }
        public string NameShort { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages),
            ErrorMessageResourceName = "RegionBranchRequiredNameEnglish")]
        [Display(Name = "English Name")]
        public string NameE { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages),
           ErrorMessageResourceName = "RegionBranchRequiredNameFrench")]
        [Display(Name = "French Name")]
        public string NameF { get; set; }

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
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Impact> Impacts { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}
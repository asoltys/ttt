﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace TransformationTimelineTool.Models
{
    public class Initiative
    {
        public int ID { get; set; }
        [Display(Name = "English Name")]
        public string NameE { get; set; }
        [Display(Name = "French Name")]
        public string NameF { get; set; }
        [Display(Name = "English Description")]
        public string DescriptionE { get; set; }
        [Display(Name = "French Description")]
        public string DescriptionF { get; set; }

        [Display(Name = "Start")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime StartDate { get; set; }

        [Display(Name = "End")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Impact> Impacts { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using TransformationTimelineTool.Helpers;
using Microsoft.AspNet.Identity;


namespace TransformationTimelineTool.Models
{
    public class Comment
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 1)]
        public String Content { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public String AuthorName { get; set; }
    }
}
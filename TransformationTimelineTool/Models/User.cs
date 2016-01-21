using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransformationTimelineTool.Models
{
    public class User : IdentityUser
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }
        public string ApproverID { get; set; }
        public virtual User Approver { get; set; }
        public virtual ICollection<Initiative> Initiatives { get; set; }

    }
}
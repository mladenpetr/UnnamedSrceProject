using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SrceApplicaton.Models
{
    public class ComplexTemplateModel
    {
        public IEnumerable<TemplateViewModel> viewList { get; set; }
        public TemplateViewModel view { get; set; }
        public int jobID { get; set; }
    }
}
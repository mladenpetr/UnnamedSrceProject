//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SrceApplicaton.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Job
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Job()
        {
            this.Technician = new HashSet<Technician>();
        }
    
        public short JobID { get; set; }
        public System.TimeSpan StartingHour { get; set; }
        public System.TimeSpan EndingHour { get; set; }
        public string JobNotes { get; set; }
        public System.DateTime JobDate { get; set; }
        public Nullable<byte> TemplateID { get; set; }
        public string Color { get; set; }
        public string Title { get; set; }
    
        public virtual JobTemplates JobTemplates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Technician> Technician { get; set; }
    }
}

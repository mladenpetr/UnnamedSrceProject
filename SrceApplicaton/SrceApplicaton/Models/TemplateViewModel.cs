using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SrceApplicaton.Models
{
    public class TemplateViewModel
    {

        [Display(Name = "Broj stolica")]
        [Range(0,120,ErrorMessage = "Broj stolica mora biti manji od 120")]
        public Nullable<byte> Chairs { get; set; }

        [Display(Name = "Raspored stolica")]
        public string ChairLayout { get; set; }

        [Display(Name = "Broj stolova")]
        [Range(0, 24, ErrorMessage = "Broj stolica mora biti manji od 24")]
        public Nullable<byte> Tables { get; set; }

        [Display(Name = "Raspored stolova")]
        public string TablesLayout { get; set; }

        [Display(Name = "Dodatne napomene")]
        public string ExtraNotes { get; set; }

        [Display(Name = "Pregrada")]
        public bool Wall { get; set; }

        [Required]
        [Display(Name = "Dvorana")]
        public string Hall { get; set; }

        [Required]
        [Display(Name = "ID Posla")]
        public int jobID { get; set; }

        [Required]
        [Key]
        [Display(Name = "ID Rekonfiguracije")]
        public byte templateID { get; set; }
    }
}
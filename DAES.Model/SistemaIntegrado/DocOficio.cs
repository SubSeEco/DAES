using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.SistemaIntegrado
{
    [Table("DocOficio")]
    public class DocOficio
    {
        public DocOficio()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int DocOficioId { get; set; }

        [Display(Name = "WorkFlow")]
        public int? WorkFlowId { get; set; }

        [Display(Name = "Parrafo1")]
        [DataType(DataType.MultilineText)]
        public string Parrafo1 { get; set; }

        [Display(Name = "Parrafo2")]
        [DataType(DataType.MultilineText)]
        public string Parrafo2 { get; set; }


        [Display(Name = "Tabla")]
        public string Tabla { get; set; }

       
    }
}

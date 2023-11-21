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

        public byte[] Content { get; set; }

        public string FileName { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCreacion { get; set; } = DateTime.Now;

        [NotMapped]
        public int x { get; set; }
        [NotMapped]
        public int y { get; set; }
        public string NUMERO_REGISTRO { get; set; }
        public string DIRECCION { get; set; }
        public int ProcesoId { get; set; }
        public string ANTECEDENTES { get; set; }
        public string MATERIA { get; set; }
        public string A_DOC { get; set; }
        public string CORREO { get; set; }
        public string Parrafo3 { get; set; }
        public string DE_DOC { get; set; }
    }
}

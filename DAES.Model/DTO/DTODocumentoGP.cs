using System;
using System.ComponentModel.DataAnnotations;

namespace DAES.Model.DTO
{
    public class DTODocumentoGP
    {
        public DTODocumentoGP() { }

        [Key]
        [Display(Name = "Id Gestion Documental")]
        public int Id { get; set; }

        [Display(Name = "Id Proceso GP")]
        public string Doc_Correlativo { get; set; }

        [Display(Name = "Fecha Creación Proceso")]
        public DateTime Doc_FechaCreacion { get; set; }

        [Display(Name = "Materia")]
        public string MateriaGD { get; set; }

        public string Doc_Asunto { get; set; }

        [Display(Name = "Referencia")]
        public string ReferenciaGD { get; set; }

        [Display(Name = "Observación")]
        public string ObservacionGD { get; set; }

    }
}

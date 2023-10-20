using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.SistemaIntegrado
{
    [Table("ObservacionReforma")]
    public class ObservacionReforma
    {
        public ObservacionReforma()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        [Key]
        public int observacionRId { get; set; }

        [Display(Name = "Reforma AGAC")]
        public int? IdReformaAGAC { get; set; }

        [Display(Name = "Reforma Anterior")]
        public int? IdReformaAnterior { get; set; }

        [Display(Name = "Reforma Post")]
        public int? IdReformaPost { get; set; }

        [Display(Name = "Numero de Oficio")]
        public int? NumeroOficio { get; set; }

        [Display(Name = "¿Aprobacion?")]
        public int? AprobacionId { get; set; }
        public virtual Aprobacion Aprobacion { get; set; }

        [Display(Name = "Fecha Oficio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaOficio { get; set; }

        [Display(Name = "Fecha Deposito")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaDeposito { get; set; }

        [Display(Name = "Organización")]
        public int? OrganizacionId { get; set; }

    }
}

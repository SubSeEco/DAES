using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.SistemaIntegrado
{
    [Table("ObservacionLegal")]
    public class ObservacionLegal
    {
        public ObservacionLegal()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        [Key]
        public int observacionId { get; set; }

        [Display(Name = "Existencia legal")]
        public int? ExistenciaId { get; set; }

        [Display(Name = "Existencia legal Anterior")]
        public int? IdExistenciaAnterior { get; set; }

        [Display(Name = "Existencia legal Posterior")]
        public int? idExistenciaPost { get; set; }

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

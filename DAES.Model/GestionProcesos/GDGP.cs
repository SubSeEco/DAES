using ExpressiveAnnotations.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.GestionProcesos
{
    [Table("GD")]
    public class GDGP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int GDId { get; set; }

        [Display(Name = "Fecha creación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Fecha ingreso")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? FechaIngreso { get; set; } = DateTime.Now;

        //[Required(ErrorMessage = "Es necesario especificar el dato Materia")]
        [Display(Name = "Materia")]
        [DataType(DataType.MultilineText)]
        public string Materia { get; set; }

        [Display(Name = "Referencia")]
        [DataType(DataType.MultilineText)]
        public string Referencia { get; set; }

        [Display(Name = "Observación")]
        [DataType(DataType.MultilineText)]
        public string Observacion { get; set; }

        [Display(Name = "Ingreso externo?")]
        public bool IngresoExterno { get; set; }

        [Display(Name = "Número externo")]
        [RequiredIf("IngresoExterno", ErrorMessage = "Es necesario especificar este dato")]
        public string NumeroExterno { get; set; }

        //destino 1
        [Display(Name = "Unidad destino")]
        [RequiredIf("IngresoExterno", ErrorMessage = "Es necesario especificar este dato")]
        public string DestinoUnidadCodigo { get; set; }

        [Display(Name = "Unidad destino")]
        public string DestinoUnidadDescripcion { get; set; }

        [Display(Name = "Usuario destino")]
        public string DestinoFuncionarioEmail { get; set; }

        [Display(Name = "Usuario destino")]
        public string DestinoFuncionarioNombre { get; set; }

        //destino 2
        [Display(Name = "Agregar destino")]
        public bool SegundoDestino { get; set; }

        [Display(Name = "Unidad destino")]
        [RequiredIf("SegundoDestino", ErrorMessage = "Es necesario especificar este dato")]
        public string DestinoUnidadCodigo2 { get; set; }

        [Display(Name = "Unidad destino")]
        public string DestinoUnidadDescripcion2 { get; set; }

        [Display(Name = "Usuario destino")]
        public string DestinoFuncionarioEmail2 { get; set; }

        [Display(Name = "Usuario destino")]
        public string DestinoFuncionarioNombre2 { get; set; }

        [Display(Name = "Id Proceso")]
        public int ProcesoId { get; set; }
        public virtual ProcesoGP ProcesoGP { get; set; }
    }
}

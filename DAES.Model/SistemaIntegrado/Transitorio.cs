using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.SistemaIntegrado
{
    [Table("Transitorio")]
    public class Transitorio
    {
        public Transitorio()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int TransitorioId { get; set; }

        [Display(Name = "Organización")]
        public int? OrganizacionId { get; set; }

        [Display(Name = "Fecha Junta de Socios")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaJuntaSocios { get; set; }

        [Display(Name = "Fecha Escritura Publica")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaEscrituraPublica { get; set; }

        [Display(Name = "Lugar de la Notaría")]
        public string NombreNotaria { get; set; }

        [Display(Name = "Nombre del Notario")]
        public string DatosNotario { get; set; }

        [Display(Name = "Objeto")]
        [DataType(DataType.MultilineText)]
        public string Objeto { get; set; }

        [Display(Name = "Moneda")]
        public string Moneda { get; set; }

        [Display(Name = "Capital Inicial")]
        public int CapitalInicial { get; set; }

        [Display(Name = "Cuotas")]
        public int Cuotas { get; set; }

        [Display(Name = "Tipo Cuota")]
        public string TipoCuota { get; set; }

        [Display(Name = "Duración")]
        public string Duracion { get; set; }

        [Display(Name = "Forma Pago")]
        public string FormaPago { get; set; }
    }
}

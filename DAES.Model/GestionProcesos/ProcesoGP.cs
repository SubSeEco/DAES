using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.GestionProcesos
{
    [Table("CoreProceso")]
    public class ProcesoGP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        [Display(Name = "Id")]
        public int ProcesoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Tipo de proceso")]
        public int DefinicionProcesoId { get; set; }
        public virtual DefinicionProcesoGP DefinicionProceso { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha creación")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha vencimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaVencimiento { get; set; }

        [Display(Name = "Estado del Proceso")]
        public int EstadoProcesoId { get; set; }
        public virtual EstadoProcesoGP EstadoProcesoGP { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha término")]
        [DataType(DataType.Date)]
        public DateTime? FechaTermino { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Observacion { get; set; }

        [Display(Name = "Email autor")]
        public string Email { get; set; }

        [Display(Name = "Nombre autor")]
        public string NombreFuncionario { get; set; }

        [Display(Name = "Unidad")]
        public int? Pl_UndCod { get; set; }

        [Display(Name = "Unidad")]
        public string Pl_UndDes { get; set; }


        [NotMapped]
        [Display(Name = "Tiempo ejecución")]
        public TimeSpan Span => ((FechaTermino.HasValue ? FechaTermino.Value : DateTime.Now) - FechaCreacion);

        [NotMapped]
        [Display(Name = "Numero Solicitud")]
        public string NroSolicitud { get; set; }

        public virtual ICollection<WorkflowGP> Workflows { get; set; } = new HashSet<WorkflowGP>();
        public virtual ICollection<DocumentoGP> Documentos { get; set; } = new HashSet<DocumentoGP>();

        [Display(Name = "Justificación anulación")]
        [DataType(DataType.MultilineText)]
        public string JustificacionAnulacion { get; set; }

        [Display(Name = "Reservado?")]
        public bool Reservado { get; set; }

        [Display(Name = "Tags")]
        public string Tags { get; set; }
    }
}

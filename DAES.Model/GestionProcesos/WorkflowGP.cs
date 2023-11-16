using ExpressiveAnnotations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.GestionProcesos
{
    [Table("CoreWorkflow")]
    public class WorkflowGP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int WorkflowId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Proceso Id")]
        public int ProcesoId { get; set; }
        public virtual ProcesoGP Proceso { get; set; }

        //[Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Tarea")]
        public int DefinicionWorkflowId { get; set; }
        public virtual DefinicionWorkflowGP DefinicionWorkflowGP { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha creación")]
        public DateTime FechaCreacion { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha vencimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaVencimiento { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha término")]
        [DataType(DataType.Date)]
        public DateTime? FechaTermino { get; set; }

        [Display(Name = "Grupo")]
        public int? GrupoId { get; set; }
        public virtual GrupoGP Grupo { get; set; }

        //[RequiredIf("PermitirSeleccionarUnidadDestino && TipoAprobacionId != 3", ErrorMessage = "Es necesario especificar el dato Unidad destino")]
        [RequiredIf("PermitirSeleccionarUnidadDestino", ErrorMessage = "Es necesario especificar el dato Unidad destino")]
        [Display(Name = "Unidad")]
        public int? Pl_UndCod { get; set; }

        //[RequiredIf("PermitirSeleccionarUnidadDestino && TipoAprobacionId != 3", ErrorMessage = "Es necesario especificar el dato Funcionario destino")]
        [RequiredIf("Reservado", ErrorMessage = "Es necesario especificar el dato Funcionario Destino ya que es un proceso reservado")]
        [Display(Name = "Funcionario")]
        public string To { get; set; }

        [Display(Name = "Unidad")]
        public string Pl_UndDes { get; set; }

        [Display(Name = "Email funcionario")]
        public string Email { get; set; }

        [RequiredIf("TipoAprobacionId == 3", ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Observaciones de la tarea")]
        [DataType(DataType.MultilineText)]
        public string Observacion { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Mensaje { get; set; }

        [RequiredIf("RequiereAprobacionAlEnviar", ErrorMessage = "Es necesario especificar el dato Aprobación")]
        [Display(Name = "Tipo aprobación")]
        public int? TipoAprobacionId { get; set; }
        public virtual TipoAprobacionGP TipoAprobacionGP { get; set; }

        [Display(Name = "Terminada?")]
        public bool Terminada { get; set; }

        [Display(Name = "Anulada?")]
        public bool Anulada { get; set; }

        [Display(Name = "Tarea es personal?")]
        public bool TareaPersonal { get; set; }

        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(Name = "Nombre Funcionario")]
        public string NombreFuncionario { get; set; }

        [Display(Name = "Entidad")]
        public string Entity { get; set; }

        [Display(Name = "Entidad Id")]
        public int? EntityId { get; set; }

        public int? ToPl_UndCod { get; set; }

        public virtual ICollection<DocumentoGP> Documentos { get; set; } = new HashSet<DocumentoGP>();
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.GestionProcesos
{
    [Table("CoreDefinicionProceso")]
    public class DefinicionProcesoGP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int DefinicionProcesoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Nombre proceso")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Duración (horas)")]
        public int DuracionHoras { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Habilitado?")]
        public bool Habilitado { get; set; }

        //public virtual ICollection<DefinicionWorkflow> DefinicionWorkflows { get; set; } = new HashSet<DefinicionWorkflow>();
        public virtual ICollection<ProcesoGP> Procesos { get; set; } = new HashSet<ProcesoGP>();

        [NotMapped]
        [Display(Name = "Participantes")]
        public string Grupos { get; set; }

        [Display(Name = "Ejecutar primera tarea inmediatamente?")]
        public bool EjecutarInmediatamente { get; set; }
    }
}

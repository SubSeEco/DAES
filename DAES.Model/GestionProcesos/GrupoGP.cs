using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.GestionProcesos
{
    [Table("CoreGrupo")]
    public class GrupoGP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int GrupoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Grupo")]
        public string Nombre { get; set; }

        //public virtual ICollection<Usuario> Usuarios { get; set; } = new HashSet<Usuario>();
    }
}

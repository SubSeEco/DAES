using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.GestionProcesos
{
    [Table("CoreTipoDocumento")]
    public class TipoDocumentoGP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int TipoDocumentoId { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAES.Model.SistemaIntegrado
{
    [Table("TipoGeneral")]
    public class TipoGeneral
    {
        public TipoGeneral()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int? TipoGeneralId { get; set; }


        [Display(Name = "Tipo General")]
        public string Nombre { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAES.Model.SistemaIntegrado
{
    [Table("ExistenciaLegalAnterior")]
    public class ExistenciaAnterior
    {
        public ExistenciaAnterior()
        {
            ObservacionLegals = new List<ObservacionLegal>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int IdExistenciaAnterior { get; set; }

        [Display(Name = "Tipo Norma")]
        public int? TipoNormaId { get; set; }
        public virtual TipoNorma tipoNorma { get; set; }

        [Display(Name = "Número de Norma")]
        public int? NNorma { get; set; }

        [Display(Name = "Fecha de Norma")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FNorma { get; set; }
        
        [Display(Name = "Fecha de publicación (Diario Oficial)")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaPublicacion { get; set; }

        [Display(Name = " Autorizado Por")]
        public string Autorizado { get; set; }

        [Display(Name = "Organización")]
        public int? OrganizacionId { get; set; }
        public virtual Organizacion Organizacion { get; set; }

        [Display(Name = "Tipo Junta")]
        public int? TipoGeneralId { get; set; }
        public virtual TipoGeneral TipoGeneral { get; set; }

        [Display(Name = "Nombre notario")]
        public string DatosGeneralNotario { get; set; }
        [Display(Name = "Lugar notario")]
        public string LugarNotario { get; set; }

        [Display(Name = "Fecha Junta Socios")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaJuntaGeneral { get; set; }

        [Display(Name = "Fecha escritura")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaEscritura { get; set; }
        public virtual List<ObservacionLegal> ObservacionLegals { get; set; }
    }
}

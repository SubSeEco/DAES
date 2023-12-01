using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.GestionProcesos
{
    [Table("ProcesoDocumento")]
    public class ProcesoDocumentoDAESGP
    {
        [Key]
        public int Id { get; set; }

        public int IdFuncionario { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreFuncionario { get; set; }

        public int? IdUnidadFuncionario { get; set; }

        [StringLength(150)]
        public string NombreUnidadFuncionario { get; set; }

        public int Secuencia { get; set; }

        public bool VBSiguienteProceso { get; set; }

        public string ObsSiguienteProceso { get; set; }

        public int? IdUnidadEntidadDestino { get; set; }

        [StringLength(150)]
        public string NombreUnidadEntidadDestino { get; set; }

        public int? IdUsuarioDestino { get; set; }

        [StringLength(150)]
        public string NombreUsuarioDestino { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool? Completado { get; set; }

        public bool Activo { get; set; }

        public int Documento_Id { get; set; }

        public int TipoDestino_Id { get; set; }

        public int TipoProceso_Id { get; set; }

        public virtual DocumentoGP Documento { get; set; }

        //public virtual TipoDestino TipoDestino { get; set; }

        public virtual DefinicionProcesoGP DefinicionProcesoGP { get; set; }
    }
}

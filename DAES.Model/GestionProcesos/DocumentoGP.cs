using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.GestionProcesos
{
    [Table("CoreDocumento")]
    public class DocumentoGP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentoId { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Proceso")]
        public int ProcesoId { get; set; }
        public virtual ProcesoGP Proceso { get; set; }

        [Required(ErrorMessage = "Es necesario especificar este dato")]
        [Display(Name = "Proceso")]
        public int WorkflowId { get; set; }
        public virtual WorkflowGP Workflow { get; set; }

        [Display(Name = "Tipo documento")]
        public int? TipoDocumentoId { get; set; }
        public virtual TipoDocumentoGP TipoDocumento { get; set; }


        //Todo documento nace privado, solo cambia a publico cuando se firma para efectos de verificación 
        //[Display(Name = "Privacidad")]
        //public int? TipoPrivacidadId { get; set; }
        //public virtual TipoPrivacidad TipoPrivacidad { get; set; }


        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Autor")]
        public string Email { get; set; }

        [Display(Name = "Nombre")]
        public string FileName { get; set; }

        [Display(Name = "Extensión")]
        public string Extension { get; set; }

        [Display(Name = "URL")]
        public string URL { get; set; }

        [Display(Name = "Archivo")]
        public byte[] File { get; set; }

        [Display(Name = "Texto")]
        public string Texto { get; set; }

        [Display(Name = "Metadata")]
        public string Metadata { get; set; }

        [Display(Name = "Metadata")]
        public string Type { get; set; }

        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [Display(Name = "Firmado")]
        public bool Signed { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Display(Name = "Código de barra")]
        public byte[] BarCode { get; set; }

        [Display(Name = "Hash md5")]
        public string Md5 { get; set; }

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Tipo documento (foliador)")]
        public string TipoDocumentoFirma { get; set; }

        [Display(Name = "Requiere firma electrónica?")]
        public bool RequiereFirmaElectronica { get; set; }

        [Display(Name = "Es documento oficial?")]
        public bool EsOficial { get; set; }

        [Display(Name = "Unidad del firmante")]
        public string FirmanteUnidad { get; set; }

        [Display(Name = "Usuario firmante")]
        public string FirmanteEmail { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Código estampado?")]
        public bool CodigoEstampado { get; set; } = false;

        //[Display(Name = "Fecha de plazo")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        //public DateTime? FechaPlazo { get; set; } = DateTime.Now;

        [Display(Name = "Anulado Por: ")]
        public string AnuladoPor { get; set; }

        [Display(Name = "Fecha Anulacion")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? FechaAnulacion { get; set; }

        //[Display(Name = "Fecha Firma")]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        //public DateTime? FechaFirma { get; set; }


        #region NotMapped

        [NotMapped]
        [Display(Name = "Tiene plazo establecido")]
        public bool PlazoEstablecido { get; set; }

        [NotMapped]
        [Display(Name = "Autorizado para firmar documentos?")]
        public bool AutorizadoParaFirma { get; set; }

        [NotMapped]
        [Display(Name = "Autor del documento?")]
        public bool AutorizadoParaEliminar { get; set; }

        [NotMapped]
        public bool Selected { get; set; } = false;

        [NotMapped]
        [Display(Name = "Certificado electrónico")]
        public string SerialNumber { get; set; }

        #endregion
    }
}

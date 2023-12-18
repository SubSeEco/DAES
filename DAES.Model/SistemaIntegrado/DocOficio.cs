using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAES.Model.SistemaIntegrado
{
    [Table("DocOficio")]
    public class DocOficio
    {
        public DocOficio()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int DocOficioId { get; set; }

        [Display(Name = "WorkFlow")]
        public int? WorkFlowId { get; set; }

        [Display(Name = "Proceso")]
        public int? ProcesoId { get; set; }

        [Display(Name = "Reg:")]
        [DataType(DataType.MultilineText)]
        public string NUMERO_REGISTRO { get; set; }

        [Display(Name = "ANT:")]
        [DataType(DataType.MultilineText)]
        public string ANTECEDENTES { get; set; }

        [Display(Name = "MAT:")]
        [DataType(DataType.MultilineText)]
        public string MATERIA { get; set; }

        [Display(Name = "DE:")]
        [DataType(DataType.MultilineText)]
        public string DE_DOC { get; set; }


        [Display(Name = "A:")]
        [DataType(DataType.MultilineText)]
        public string A_DOC { get; set; }

        [Display(Name = "Direccion")]
        [DataType(DataType.MultilineText)]
        public string DIRECCION { get; set; }

        [Display(Name = "Correo")]
        [DataType(DataType.MultilineText)]
        public string CORREO { get; set; }

        [Display(Name = "2. Cuerpo del Oficio")]
        [DataType(DataType.MultilineText)]
        public string Parrafo1 { get; set; }

        [Display(Name = "Informa actualización/registro del Com. Adm./Directorio")]
        [DataType(DataType.MultilineText)]
        public string Parrafo2 { get; set; }

        [Display(Name = "Parrafo3")]
        [DataType(DataType.MultilineText)]
        public string Parrafo3 { get; set; }

        [Display(Name = "1. Antecedentes del oficio")]
        public string Tabla { get; set; }

        public byte[] Content { get; set; }

        [Display(Name = "Nombre del Archivo")]
        public string FileName { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCreacion { get; set; } = DateTime.Now;

        [NotMapped]
        public byte[] File { get; set; }

        [Display(Name = "Firmado")]
        public bool Firmado { get; set; } = false;

        [Display(Name = "Tipo documento")]
        public int? TipoDocumentoId { get; set; }

        [Display(Name = "Folio")]
        public string Folio { get; set; }

        [Display(Name = "Autores")]
        public string AUTORES { get; set; }

        [Display(Name = "Insertar “Consejo de administración” o “Directorio”.")]
        public bool TieneDirectorio { get; set; } = false;
    }
}

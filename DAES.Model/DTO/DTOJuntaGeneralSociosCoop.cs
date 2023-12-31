﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DAES.Model.SistemaIntegrado;

namespace DAES.Model.DTO
{
    public class DTOJuntaGeneralSociosCoop : DTOSolicitanteCore
    {
        public DTOJuntaGeneralSociosCoop()
        {
            Directorio = new List<DTODirectorio>();
        }

        [Required(ErrorMessage = "Es necesario especificar el dato Organización")]
        [Display(Name = "Organización")]
        public int OrganizacionId { get; set; }
        public virtual Organizacion Organizacion { get; set; }

        [Display(Name = "Tipo organización")]
        public int TipoOrganizacionId { get; set; }
        public virtual TipoOrganizacion TipoOrganizacion { get; set; }

        [Display(Name = "Estado")]
        public int? EstadoId { get; set; }
        public virtual Estado Estado { get; set; }

        [Display(Name = "Situación")]
        public int? SituacionId { get; set; }
        public virtual Situacion Situacion { get; set; }

        [Display(Name = "Rubro")]
        public int? RubroId { get; set; }
        public virtual Rubro Rubro { get; set; }

        [Display(Name = "Subrubro")]
        public int? SubRubroId { get; set; }
        public virtual SubRubro SubRubro { get; set; }

        [Display(Name = "Región")]
        public int? RegionId { get; set; }
        public virtual Region Region { get; set; }

        [Display(Name = "Comuna")]
        public int? ComunaId { get; set; }
        public virtual Comuna Comuna { get; set; }

        [Display(Name = "Número registro")]
        public string NumeroRegistro { get; set; }

        [Display(Name = "Razón social")]
        [DataType(DataType.MultilineText)]
        public string RazonSocial { get; set; }

        [Display(Name = "Sigla")]
        public string Sigla { get; set; }

        [Display(Name = "Dirección")]
        [DataType(DataType.MultilineText)]
        public string Direccion { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Es necesario especificar el dato Acta de la Junta General de Socios, debidamente autorizada ante Notario")]
        [Display(Name = "(*) Acta de la Junta General de Socios, debidamente autorizada ante Notario.")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File1 { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el dato Acta Constitutiva del Consejo de Administración, de acuerdo a lo dispuesto en el artículo 60, del reglamento de la Ley General de Cooperativas, debidamente autorizada ante Notario")]
        [Display(Name = "(*) Acta Constitutiva del Consejo de Administración, de acuerdo a lo dispuesto en el artículo 60, del reglamento de la Ley General de Cooperativas, debidamente autorizada ante Notario.")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File2 { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el dato Formalidades de convocatoria, esto es, copia del aviso de citación publicado; comprobantes de correo y(o copia del Libro de citaciones")]
        [Display(Name = "(*) Formalidades de convocatoria, esto es, copia del aviso de citación publicado; comprobantes de correo y/o copia del Libro de citaciones.")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File3 { get; set; }

        [Display(Name = "Ficha de Datos, según lo dispuesto en el artículo 2, numeral 9 de la Resolución Administrativa 1321.")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File4 { get; set; }

       
        public virtual List<DTODirectorio> Directorio { get; set; }

    }
}

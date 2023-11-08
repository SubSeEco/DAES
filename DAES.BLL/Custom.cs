using DAES.BLL.Interfaces;
using DAES.Infrastructure;
using DAES.Infrastructure.GestionDocumental;
using DAES.Infrastructure.SistemaIntegrado;
using DAES.Model.Core;
using DAES.Model.FirmaDocumento;
using DAES.Model.Sigper;
using DAES.Model.SistemaIntegrado;
using DAES.Infrastructure.Sigper;
using FluentDateTime;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.ModelBinding;
using DAES.Infrastructure.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;
using static iTextSharp.text.pdf.AcroFields;
using System.Web.Services.Description;
//using DAES.bll.Interfaces;

namespace DAES.BLL
{
    public class Custom
    {
        private SmtpClient smtpClient = new SmtpClient();
        private MailMessage emailMsg = new MailMessage();
        private GestionDocumentalContext gestionDocumentalContext = new GestionDocumentalContext();


        private void Send()
        {
            try
            {
                smtpClient.Send(emailMsg);
            }
            catch
            {
                return;
            }
        }

        public void LogAdd(Log log)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                context.Log.Add(log);
                context.SaveChanges();
            }
        }

        public Organizacion GenerarRegistro(Organizacion Organizacion)
        {

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {

                var returnlist = new List<string>();
                var configuracionsecuencia = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.SecuenciaCorrelativo);
                if (configuracionsecuencia == null)
                {
                    throw new Exception("No se puede generar el correlativo ya que no existe la configuración de secuencia.");
                }

                if (!configuracionsecuencia.Valor.IsInt())
                {
                    throw new Exception("No se puede generar correlativo ya que la configuración del valor del último correlativo no es un número entero válido.");
                }

                var organizacion = context.Organizacion.FirstOrDefault(q => q.OrganizacionId == Organizacion.OrganizacionId);
                if (organizacion == null)
                {
                    organizacion = new Organizacion();
                    organizacion.FechaCreacion = DateTime.Now;
                    organizacion.NumeroRegistro = string.Empty;
                    context.Organizacion.Add(organizacion);
                }

                organizacion.TipoOrganizacionId = Organizacion.TipoOrganizacionId;
                organizacion.RUT = Organizacion.RUT;
                organizacion.RazonSocial = Organizacion.RazonSocial;
                organizacion.Sigla = Organizacion.Sigla;
                organizacion.EstadoId = (int)Infrastructure.Enum.Estado.RolAsignado;
                organizacion.FechaActualizacion = DateTime.Now;

                var cont = 1;
                while (context.Organizacion.Any(q => q.NumeroRegistro == organizacion.NumeroRegistro || organizacion.NumeroRegistro == string.Empty))
                {
                    organizacion.NumeroRegistro = (configuracionsecuencia.Valor.ToInt() + cont).ToString();
                    cont++;
                }

                configuracionsecuencia.Valor = (configuracionsecuencia.Valor.ToInt() + cont).ToString();
                context.SaveChanges();

                return organizacion;
            }
        }

        public List<string> SupervisorUpdate(List<ActualizacionSupervisor> list)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();
                if (list == null)
                {
                    return returnValue;
                }

                foreach (var item in list)
                {
                    var supervisor = context.SupervisorAuxiliars.FirstOrDefault(q => q.SupervisorAuxiliarId == item.SupervisorAuxiliarId);
                    if (item != null)
                    {
                        supervisor.TipoPersonaJuridicaId = item.TipoPersonaJuridicaId;
                        supervisor.DomicilioLegal = item.DomicilioLegal;
                        supervisor.Telefono = item.Telefono;
                        supervisor.CorreoElectronico = item.CorreoElectronico;

                        foreach (var UpRepre in item.Representantes)
                        {
                            var repre = context.RepresentantesLegals.Find(UpRepre.RepresentanteLegalId);

                            repre.NombreCompleto = UpRepre.NombreCompleto;
                            repre.RUN = UpRepre.RUN;
                            repre.Profesion = UpRepre.Profesion;
                            repre.Domicilio = UpRepre.Domicilio;
                            repre.Nacionalidad = UpRepre.Nacionalidad;
                            if (repre.Habilitado != UpRepre.Habilitado)
                            {
                                repre.Eliminado = true;
                            }
                            else
                            {
                                repre.Eliminado = false;
                            }

                            /*else
                            {
                                new RepresentanteLegal()
                                {
                                    NombreCompleto = UpRepre.NombreCompleto,
                                    RUN = UpRepre.RUN,
                                    Profesion = UpRepre.Profesion,
                                    Domicilio = UpRepre.Domicilio,
                                    Nacionalidad = UpRepre.Nacionalidad,
                                    SupervisorAuxiliarId = supervisor.SupervisorAuxiliarId,
                                    Habilitado=true,
                                    Eliminado=false
                                };
                            }*/

                        }

                        foreach (var UpFacultada in item.Facultada)
                        {
                            var facultada = context.PersonaFacultadas.Find(UpFacultada.PersonaFacultadaId);

                            facultada.NombreCompleto = UpFacultada.NombreCompleto;
                            facultada.RUN = UpFacultada.RUN;
                            facultada.Profesion = UpFacultada.Profesion;
                            facultada.Domicilio = UpFacultada.Domicilio;
                            facultada.Nacionalidad = UpFacultada.Nacionalidad;

                            if (facultada.Habilitado != UpFacultada.Habilitado)
                            {
                                facultada.Eliminado = true;
                            }
                            else
                            {
                                facultada.Eliminado = false;
                            }
                            /*new PersonaFacultada()
                            {
                                NombreCompleto = UpFacultada.NombreCompleto,
                                RUN = UpFacultada.RUN,
                                Profesion = UpFacultada.Profesion,
                                Domicilio = UpFacultada.Domicilio,
                                Nacionalidad = UpFacultada.Nacionalidad,
                                SupervisorAuxiliarId = supervisor.SupervisorAuxiliarId
                            };*/
                        }
                    }
                    context.SaveChanges();
                }
                return returnValue;
            }
        }

        //public List<string> DirectorioUpdateDev(List<ActualizacionOrganizacionDirectorio> list)
        //{
        //    using (SistemaIntegradoContext context = new SistemaIntegradoContext())
        //    {
        //        var returnValue = new List<string>();

        //        if (list == null)
        //        {
        //            return returnValue;
        //        }

        //        context.SaveChanges();


        //        foreach (var item in list)
        //        {
        //            var directorio = context.Directorio.FirstOrDefault(q => q.DirectorioId == item.DirectorioUpdateId);
        //            if (directorio != null)
        //            {
        //                //directorio.CargoId = item.CargoId;
        //                //directorio.NombreCompleto = item.NombreCompleto;
        //                //directorio.Rut = item.Rut;
        //                //directorio.FechaInicio = item.FechaInicio;
        //                //directorio.FechaTermino = item.FechaTermino;
        //                //directorio.GeneroId = item.GeneroId;


        //            }
        //        }
        //        context.SaveChanges();
        //        return returnValue;
        //    }
        //}
        public List<string> DirectorioUpdateDev(List<ActualizacionDirectorioOrganizacion> list, int organizacionId, int? actualizacionOrganizacionId)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }

                context.SaveChanges();

                var directorio_update = context.ActualizacionDirectorioOrganizacion.Where(q => q.ActualizacionOrganizacionId == actualizacionOrganizacionId).ToList();
                var directorio_actual = context.Directorio.Where(q => q.OrganizacionId == organizacionId).ToList();
                var eliminar = false;

                foreach (var item in list/*.OrderBy(q => q.NombreCompleto)*/)
                {
                    //Se busca por el rut
                    var directorio = context.Directorio.Where(q => q.OrganizacionId == organizacionId && q.Rut == item.Rut).FirstOrDefault();
                    //Ahora se busca por el nombre del integrante de directorio
                    var directorio_n = context.Directorio.Where(q => q.OrganizacionId == organizacionId && q.NombreCompleto.ToUpper() == item.NombreCompleto.ToUpper()).FirstOrDefault();

                    //eh?
                    var dir = directorio != null ? directorio : directorio_n;

                    //Si el miembro existe, se actualiza o se mantiene
                    if (dir != null)
                    {
                        dir.CargoId = item.CargoId;
                        dir.NombreCompleto = item.NombreCompleto;
                        dir.Rut = item.Rut;
                        dir.FechaInicio = item.FechaInicio;
                        dir.FechaTermino = item.FechaTermino;
                        dir.GeneroId = item.GeneroId;
                        dir.Eliminar = false;
                    }
                    else
                    {
                        //se esta agregando un nuevo miembro
                        context.Directorio.Add(new Directorio
                        {
                            OrganizacionId = organizacionId,
                            NombreCompleto = item.NombreCompleto,
                            Rut = item.Rut,
                            FechaInicio = item.FechaInicio,
                            FechaTermino = item.FechaTermino,
                            CargoId = item.CargoId,
                            GeneroId = item.GeneroId,
                            Eliminar = false
                        });
                    }

                }
                context.SaveChanges();

                //cuando se desea eliminar
                if (directorio_update.Count() < directorio_actual.Count())
                    eliminar = true;

                if (eliminar)
                {
                    List<Directorio> directorios_del = new List<Directorio>();
                    List<ComisionLiquidadora> comision_del = new List<ComisionLiquidadora>();
                    foreach (var item in directorio_actual)
                    {
                        if (item.Eliminar)
                        {
                            directorios_del.Add(item);
                        }
                    }
                    foreach (var help in directorios_del)
                    {
                        var comision_help = context.ComisionLiquidadora.Where(q => q.DirectorioId == help.DirectorioId).FirstOrDefault();
                        if (comision_help != null)
                        {
                            comision_del.Add(comision_help);
                        }
                    }

                    //Antes de eliminar, se debe verificar su existencia dentro de la comision liquidadora y eliminarlo
                    foreach (var item in comision_del)
                    {
                        context.ComisionLiquidadora.Remove(item);
                    }


                    foreach (var item in directorios_del)
                    {
                        context.Directorio.Remove(item);
                    }

                    context.SaveChanges();
                }

                return returnValue;
            }
        }
        //public List<string> DirectorioUpdate(List<Directorio> list /*int? organizacionId*/)
        //{
        //    using (SistemaIntegradoContext context = new SistemaIntegradoContext())
        //    {
        //        var returnValue = new List<string>();

        //        if (list == null)
        //        {
        //            return returnValue;
        //        }

        //        context.SaveChanges();

        //        foreach (var item in list)
        //        {
        //            var directorio = context.Directorio.FirstOrDefault(q => q.DirectorioId == item.DirectorioUpdateId);
        //            if (directorio != null)
        //            {
        //                //directorio.CargoId = item.CargoId;
        //                //directorio.NombreCompleto = item.NombreCompleto;
        //                //directorio.Rut = item.Rut;
        //                //directorio.FechaInicio = item.FechaInicio;
        //                //directorio.FechaTermino = item.FechaTermino;
        //                //directorio.GeneroId = item.GeneroId;


        //            }
        //        }
        //        context.SaveChanges();
        //        return returnValue;
        //    }
        //}

        public List<string> DirectorioUpdate(List<Directorio> list /*int? organizacionId*/)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }

                context.SaveChanges();

                foreach (var item in list)
                {
                    var directorio = context.Directorio.FirstOrDefault(q => q.DirectorioId == item.DirectorioId);
                    if (directorio != null)
                    {
                        directorio.CargoId = item.CargoId;
                        directorio.NombreCompleto = item.NombreCompleto;
                        directorio.Rut = item.Rut;
                        directorio.FechaInicio = item.FechaInicio;
                        directorio.FechaTermino = item.FechaTermino;
                        directorio.GeneroId = item.GeneroId;
                    }
                }
                context.SaveChanges();
                return returnValue;
            }
        }
        public List<string> ModificacionUpdate(List<ModificacionEstatuto> list)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }

                foreach (var item in list)
                {
                    var modificacion = context.ModificacionEstatutos.FirstOrDefault(q => q.ModificacionEstatutoId == item.ModificacionEstatutoId);
                    if (modificacion != null)
                    {
                        modificacion.Fecha = item.Fecha;
                        modificacion.NumeroOficio = item.NumeroOficio;
                        modificacion.Vigente = item.Vigente;
                    }
                }

                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> ExistenciaUpdate(List<ExistenciaLegal> list, Organizacion or)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }


                foreach (var item in list)
                {
                    var exi = context.ExistenciaLegal.FirstOrDefault(q => q.ExistenciaId == item.ExistenciaId);
                    //var exis = context.ExistenciaLegal.Any(q => q.OrganizacionId == item.OrganizacionId);
                    if (exi != null)
                    {

                        exi.NumeroNorma = item.NumeroNorma;
                        exi.TipoNormaId = item.TipoNormaId;
                        exi.FechaNorma = item.FechaNorma;
                        exi.FechaPublicacionn = item.FechaPublicacionn;
                        exi.FechaPublic = item.FechaPublic;
                        exi.AutorizadoPor = item.AutorizadoPor;
                        exi.FechaConstitutivaSocios = item.FechaConstitutivaSocios;
                        exi.NumeroOficio = item.NumeroOficio;
                        exi.FechaOficio = item.FechaOficio;
                        exi.FechaEscrituraPublica = item.FechaEscrituraPublica;
                        exi.DatosGeneralesNotario = item.DatosGeneralesNotario;
                        exi.Fojas = item.Fojas;
                        exi.FechaInscripcion = item.FechaInscripcion;
                        exi.DatosCBR = item.DatosCBR;
                        exi.AprobacionId = item.AprobacionId;
                        exi.TipoGeneralId = item.TipoGeneralId;
                    }

                }
                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> ExistenciaAnteriorUpdate(List<ExistenciaAnterior> list)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }


                foreach (var item in list)
                {
                    var exi = context.ExistenciaLegalAnterior.FirstOrDefault(q => q.IdExistenciaAnterior == item.IdExistenciaAnterior);
                    //var exis = context.ExistenciaLegal.Any(q => q.OrganizacionId == item.OrganizacionId);
                    if (exi != null)
                    {

                        exi.TipoNormaId = item.TipoNormaId;
                        exi.NNorma = item.NNorma;
                        exi.FNorma = item.FNorma;
                        exi.FechaPublicacion = item.FechaPublicacion;
                        exi.Autorizado = item.Autorizado;
                        exi.LugarNotario = item.LugarNotario;
                        exi.DatosGeneralNotario = item.DatosGeneralNotario;
                        exi.TipoGeneralId = item.TipoGeneralId;
                        exi.FechaEscritura = item.FechaEscritura;
                        exi.FechaJuntaGeneral = item.FechaJuntaGeneral;
                    }

                }
                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> ExistenciaPostUpdate(List<ExistenciaPosterior> list)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }


                foreach (var item in list)
                {
                    var exi = context.ExistenciaPosterior.FirstOrDefault(q => q.idExistenciaPost == item.idExistenciaPost);
                    //var exis = context.ExistenciaLegal.Any(q => q.OrganizacionId == item.OrganizacionId);
                    if (exi != null)
                    {

                        exi.FechaConstitutivaSocios = item.FechaConstitutivaSocios;
                        exi.FechaEscrituraPublica = item.FechaEscrituraPublica;
                        exi.FechaPublicacionn = item.FechaPublicacionn;
                        exi.DatosGeneralesNotario = item.DatosGeneralesNotario;
                        exi.Fojas = item.Fojas;
                        exi.AnoInscripcion = item.AnoInscripcion;
                        exi.DatosCBR = item.DatosCBR;
                        exi.LugarNotario = item.LugarNotario;
                        exi.TipoGeneralId = item.TipoGeneralId;
                    }

                }
                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> SaneamientoUpdate(List<Saneamiento> list)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }

                foreach (var item in list)
                {
                    var sane = context.Saneamiento.FirstOrDefault(q => q.IdSaneamiento == item.IdSaneamiento);
                    if (sane != null)
                    {
                        sane.FechaEscrituraPublicaa = item.FechaEscrituraPublicaa;
                        sane.FechaaPublicacionDiario = item.FechaaPublicacionDiario;
                        sane.DatoGeneralesNotario = item.DatoGeneralesNotario;
                        sane.Fojass = item.Fojass;
                        sane.FechaaInscripcion = item.FechaaInscripcion;
                        sane.DatossCBR = item.DatossCBR;
                        //Sane.OrganizacionId = item.OrganizacionId;


                    }
                }
                context.SaveChanges();
                return returnValue;
            }
        }

        //public List<string> CrudMantenedoresActualizar(List<Reforma> list, List<Saneamiento> san, List<ExistenciaLegal> exis) {

        //    var returnValue = new List<string>();

        //    if (list == null || san == null || exis == null)
        //    {
        //        return returnValue;
        //    }

        //    return returnValue;
        //}

        public List<string> ReformaUpdate(List<Reforma> list, Organizacion mod)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }


                foreach (var item in list)
                {
                    var reforma = context.Reforma.FirstOrDefault(q => q.IdReforma == item.IdReforma);
                    if (reforma != null)
                    {
                        reforma.FechaReforma = item.FechaReforma;
                        reforma.FechaReformaa = item.FechaReformaa;
                        reforma.UltimaReforma = item.NumeroNormaa == null && reforma.NumeroOficio == null ? (bool)item.UltimaReformaa : item.UltimaReforma;
                        reforma.TipoNormaId = item.TipoNormaId;
                        reforma.NumeroNormaa = item.NumeroNormaa;
                        reforma.FechaNormaa = item.FechaNormaa;
                        reforma.FechaPublicacionDiario = item.FechaPublicacionDiario;
                        reforma.DatosGeneralNotario = item.DatosGeneralNotario;
                        reforma.FechaJuntaGeneral = item.FechaJuntaGeneral;
                        reforma.FechaPublicacion = item.FechaPublicacion;
                        reforma.AnoInscripcion = item.AnoInscripcion;
                        reforma.DatosCBR = item.DatosCBR;
                        reforma.FechaEscrituraPublica = item.FechaEscrituraPublica;
                        reforma.Fojas = item.Fojas;
                        reforma.AsambleaDepId = item.AsambleaDepId;
                        reforma.FechaOficio = item.FechaOficio;
                        reforma.NumeroOficio = item.NumeroOficio;
                        reforma.FechaAsambleaDep = item.FechaAsambleaDep;
                        reforma.AprobacionId = item.AprobacionId;
                        //reforma.EspaciosDocAGAC = item.EspaciosDocAGAC;
                    }
                }

                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> ReformaUpdateAG(List<ReformaAGAC> list)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }


                foreach (var item in list)
                {
                    var reforma = context.ReformaAGAC.FirstOrDefault(q => q.IdReformaAGAC == item.IdReformaAGAC);
                    if (reforma != null)
                    {
                        reforma.AsambleaDepId = item.AsambleaDepId;
                        reforma.FechaAsambleaDep = item.FechaAsambleaDep;
                        reforma.NumeroOficio = item.NumeroOficio;
                        reforma.FechaOficio = item.FechaOficio;
                        reforma.AprobacionId = item.AprobacionId;
                        reforma.TipoGeneralId = item.TipoGeneralId;
                        //reforma.EspaciosDocAGAC = item.EspaciosDocAGAC;

                    }
                }

                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> ReformaUpdateAnt(List<ReformaAnterior> list)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }


                foreach (var item in list)
                {
                    var reforma = context.ReformaAnterior.FirstOrDefault(q => q.IdReformaAnterior == item.IdReformaAnterior);
                    if (reforma != null)
                    {
                        reforma.FechaReforma = item.FechaReforma;
                        reforma.TipoNormaId = item.TipoNormaId;
                        reforma.FechaNorma = item.FechaNorma;
                        reforma.NNorma = item.NNorma;
                        reforma.FechaPublicDiario = item.FechaPublicDiario;
                        reforma.DatosNotario = item.DatosNotario;
                        reforma.EspaciosDocAnterior = item.EspaciosDocAnterior;
                        reforma.TipoGeneralId = item.TipoGeneralId;
                        reforma.AutorizadoPor = item.AutorizadoPor;
                        reforma.LugarNotario = item.LugarNotario;
                        reforma.TipoGeneralId = item.TipoGeneralId;
                        reforma.FechaEscritura = item.FechaEscritura;
                        reforma.FechaJuntaGeneral = item.FechaJuntaGeneral;
                    }
                }

                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> ReformaUpdatePost(List<ReformaPosterior> list)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                if (list == null)
                {
                    return returnValue;
                }


                foreach (var item in list)
                {
                    var reforma = context.ReformaPosterior.FirstOrDefault(q => q.IdReformaPost == item.IdReformaPost);
                    if (reforma != null)
                    {
                        reforma.FReforma = item.FReforma;
                        reforma.FechaJuntGeneralSocios = item.FechaJuntGeneralSocios;
                        reforma.FechaEscrituraPublica = item.FechaEscrituraPublica;
                        reforma.FojasNumero = item.FojasNumero;
                        reforma.AnoInscripcion = item.AnoInscripcion;
                        reforma.DatosCBR = item.DatosCBR;
                        reforma.FechaPubliDiario = item.FechaPubliDiario;
                        reforma.DatosGeneralNotario = item.DatosGeneralNotario;
                        reforma.EspaciosDoc = item.EspaciosDoc;
                        reforma.LugarNotario = item.LugarNotario;
                        reforma.TipoGeneralId = item.TipoGeneralId;
                    }
                }

                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> TransitorioUpdate(List<Transitorio> listTransitorio, Transitorio transitorio)
        {

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();
                if (listTransitorio == null)
                {
                    return returnValue;
                }
                /*if(liquidadoras == null)
                {
                    return returnValue;
                }*/


                foreach (var item in listTransitorio)
                {
                    var newTransitorio = context.Transitorio.FirstOrDefault(q => q.TransitorioId == item.TransitorioId);
                    var org = context.Organizacion.FirstOrDefault(q => q.OrganizacionId == item.OrganizacionId);
                    // not Sure

                    if (newTransitorio != null)
                    {
                        newTransitorio.FechaEscrituraPublica = item.FechaEscrituraPublica;
                        newTransitorio.FechaJuntaSocios = item.FechaJuntaSocios;
                        newTransitorio.Moneda = item.Moneda;
                        newTransitorio.Cuotas = item.Cuotas;
                        newTransitorio.TipoCuota = item.TipoCuota;
                        newTransitorio.Objeto = item.Objeto;
                        newTransitorio.CapitalInicial = item.CapitalInicial;
                        newTransitorio.Duracion = item.Duracion;
                        newTransitorio.FormaPago = item.FormaPago;
                        newTransitorio.NombreNotaria = item.NombreNotaria;
                        newTransitorio.DatosNotario = item.DatosNotario;
                        newTransitorio.TipoGeneralId = item.TipoGeneralId;
                    }
                }

                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> ObservacionLegalUpdate(List<ObservacionLegal> listObservacionLegal, ObservacionLegal ObservacionLegal)
        {

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();
                if (listObservacionLegal == null)
                {
                    return returnValue;
                }
                /*if(liquidadoras == null)
                {
                    return returnValue;
                }*/


                foreach (var item in listObservacionLegal)
                {
                    var newObservacion = context.ObservacionLegal.FirstOrDefault(q => q.observacionId == item.observacionId);
                    var org = context.Organizacion.FirstOrDefault(q => q.OrganizacionId == item.OrganizacionId);
                    // not Sure

                    if (newObservacion != null)
                    {

                        newObservacion.NumeroOficio = item.NumeroOficio;

                        newObservacion.AprobacionId = item.AprobacionId;

                        newObservacion.FechaOficio = item.FechaOficio;

                        newObservacion.FechaDeposito = item.FechaDeposito;

                        newObservacion.ExistenciaId = item.ExistenciaId;

                        newObservacion.IdExistenciaAnterior = item.IdExistenciaAnterior;

                        newObservacion.idExistenciaPost = item.idExistenciaPost;

                    }
                }

                context.SaveChanges();
                return returnValue;
            }
        }

        public List<string> ObservacionReformaUpdate(List<ObservacionReforma> listObservacionReforma, ObservacionReforma ObservacionReforma)
        {

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();
                if (listObservacionReforma == null)
                {
                    return returnValue;
                }
                /*if(liquidadoras == null)
                {
                    return returnValue;
                }*/


                foreach (var item in listObservacionReforma)
                {
                    var newObservacion = context.ObservacionReforma.FirstOrDefault(q => q.observacionRId == item.observacionRId);
                    var org = context.Organizacion.FirstOrDefault(q => q.OrganizacionId == item.OrganizacionId);
                    // not Sure

                    if (newObservacion != null)
                    {
                        newObservacion.IdReformaAGAC = item.IdReformaAGAC;

                        newObservacion.IdReformaAnterior = item.IdReformaAnterior;

                        newObservacion.IdReformaPost = item.IdReformaPost;

                        newObservacion.NumeroOficio = item.NumeroOficio;

                        newObservacion.AprobacionId = item.AprobacionId;

                        newObservacion.FechaOficio = item.FechaOficio;

                        newObservacion.FechaDeposito = item.FechaDeposito;

                    }
                }

                context.SaveChanges();
                return returnValue;
            }
        }
        public List<string> DisolucionUpdate(List<Disolucion> listDisolucion, Disolucion disolucionss, List<ComisionLiquidadora> comisionLiquidadoras)
        {

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();
                if (listDisolucion == null)
                {
                    return returnValue;
                }
                /*if(liquidadoras == null)
                {
                    return returnValue;
                }*/


                foreach (var item in listDisolucion)
                {
                    var disolucion = context.Disolucions.FirstOrDefault(q => q.DisolucionId == item.DisolucionId);
                    var org = context.Organizacion.FirstOrDefault(q => q.OrganizacionId == item.OrganizacionId);
                    var comiLiqui = context.ComisionLiquidadora.FirstOrDefault(q => q.OrganizacionId == org.OrganizacionId);
                    // not Sure

                    if (disolucion != null)
                    {
                        disolucion.TipoNormaId = item.TipoNormaId;

                        disolucion.NumeroNorma = item.NumeroNorma;

                        disolucion.FechaNorma = item.FechaNorma;

                        disolucion.NumeroFojas = item.NumeroFojas;

                        disolucion.AñoInscripcion = item.AñoInscripcion;

                        disolucion.MinistroDeFe = item.MinistroDeFe;

                        disolucion.Anterior = item.Anterior;

                        if (item.FechaPubliAnterior != null)
                        {
                            disolucion.FechaPubliccionDiarioOficial = item.FechaPubliAnterior;
                        }
                        else if (item.FechaPubliPosterior != null)
                        {
                            disolucion.FechaPubliccionDiarioOficial = item.FechaPubliPosterior;
                        }
                        else
                        {
                            disolucion.FechaPubliccionDiarioOficial = item.FechaPubliccionDiarioOficial;
                        }

                        /*disolucion.FechaPubliccionDiarioOficial = item.FechaPubliccionDiarioOficial;*/

                        disolucion.Autorizacion = item.Autorizacion;

                        if (item.FechaJuntaAnterior != null)
                        {
                            disolucion.FechaJuntaSocios = item.FechaJuntaAnterior;
                        }
                        else if (item.FechaJuntaPosterior != null)
                        {
                            disolucion.FechaJuntaSocios = item.FechaJuntaPosterior;
                        }
                        else
                        {
                            disolucion.FechaJuntaSocios = item.FechaJuntaSocios;
                        }
                        if (item.Anterior == true)
                        {
                            disolucion.ComisionAnterior = item.ComisionAnterior;
                            disolucion.ComisionPosterior = false;
                        }
                        else if (item.Anterior == false)
                        {
                            disolucion.ComisionPosterior = item.ComisionPosterior;
                            disolucion.ComisionAnterior = false;
                        }

                        disolucion.NumeroOficio = item.NumeroOficio;

                        disolucion.FechaOficio = item.FechaOficio;

                        disolucion.FechaAsambleaSocios = item.FechaAsambleaSocios;

                        disolucion.FechaEscrituraPublica = item.FechaEscrituraPublica;

                        disolucion.NombreNotaria = item.NombreNotaria;

                        disolucion.DatosNotario = item.DatosNotario;

                        disolucion.DatosCBR = item.DatosCBR;

                        disolucion.FechaComiLiquiJuntaSocios = item.FechaComiLiquiJuntaSocios;

                        disolucion.FechaInicio = item.FechaInicio;

                        disolucion.FechaTermino = item.FechaTermino;

                        foreach (var help in comisionLiquidadoras)
                        {
                            var comisionLiqui = context.ComisionLiquidadora.Where(q => q.ComisionLiquidadoraId == help.ComisionLiquidadoraId);
                            comisionLiqui.FirstOrDefault().DisolucionId = item.DisolucionId;
                            comisionLiqui.FirstOrDefault().CargoId = help.CargoId;
                            comisionLiqui.FirstOrDefault().Rut = help.Rut;
                            comisionLiqui.FirstOrDefault().GeneroId = help.GeneroId;
                            comisionLiqui.FirstOrDefault().NombreCompleto = help.NombreCompleto;
                            comisionLiqui.FirstOrDefault().FechaInicio = help.FechaInicio;
                            comisionLiqui.FirstOrDefault().FechaTermino = help.FechaTermino;
                            comisionLiqui.FirstOrDefault().EsMiembro = help.EsMiembro;
                        }
                    }
                }

                context.SaveChanges();
                return returnValue;
            }
        }

        public void SignPDF(int documentoid, int TipoOrganizacionId, string NumeroFolio)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {

                //traer documento original y firmante
                var firmante = context.Firmante.FirstOrDefault(q => q.EsActivo);
                if (firmante == null)
                {
                    throw new Exception("No se encontró un firmante habilitado.");
                }

                var documento = context.Documento.Find(documentoid);
                if (documento == null)
                {
                    throw new Exception("No se encontró el documento a firmar.");
                }

                var tipoOrganizacion = context.TipoOrganizacion.Find(TipoOrganizacionId);
                if (tipoOrganizacion == null)
                {
                    throw new Exception("No se encontró el tipo de organización.");
                }

                //crear nuevo documento
                var nuevodocumento = new Documento();
                nuevodocumento.Activo = true;
                nuevodocumento.Autor = documento.Autor;
                nuevodocumento.Descripcion = documento.Descripcion;
                nuevodocumento.Enviado = documento.Enviado;
                nuevodocumento.FechaRecordatorio = documento.FechaRecordatorio;
                nuevodocumento.FechaResolucion = documento.FechaResolucion;
                nuevodocumento.FechaValidoHasta = documento.FechaValidoHasta;
                nuevodocumento.FirmanteId = documento.FirmanteId;
                nuevodocumento.OrganizacionId = documento.OrganizacionId;
                nuevodocumento.ProcesoId = documento.ProcesoId;
                nuevodocumento.Recordatorio = documento.Recordatorio;
                nuevodocumento.Resuelto = documento.Resuelto;
                nuevodocumento.TipoDocumentoId = documento.TipoDocumentoId;
                nuevodocumento.Url = documento.Url;
                nuevodocumento.WorkflowId = documento.WorkflowId;
                nuevodocumento.TipoPrivacidadId = documento.TipoPrivacidadId;
                //nuevodocumento.NumeroFolio = NumeroFolio;
                nuevodocumento.NumeroFolio = null;
                context.Documento.Add(nuevodocumento);
                context.SaveChanges();

                nuevodocumento.Content = SignPDF(nuevodocumento.DocumentoId, nuevodocumento.NumeroFolio, documento.Content, documento.FileName, firmante, true, documento.TipoDocumentoId, tipoOrganizacion.TipoOrganizacionId);
                nuevodocumento.Firmado = true;
                nuevodocumento.FileName = "FIRMADO - " + documento.FileName;

                documento.Activo = false;

                context.SaveChanges();
            }
        }


        public byte[] CrearDocumentoConfOficio(DocOficio docofi)
        {
            #region Configurar PreDocumento
            EventoTitulos ev = new EventoTitulos();
            Font _fontTitulo = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, BaseColor.DARK_GRAY);
            Font _fontNumero = new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, BaseColor.DARK_GRAY);
            Font _fontFirmante = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.DARK_GRAY);
            Font _fontStandard = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.DARK_GRAY);
            Font _fontStandardBold = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.DARK_GRAY);

            MemoryStream memStream = new MemoryStream();
            Document doc = new Document(PageSize.LEGAL);
            PdfWriter write = PdfWriter.GetInstance(doc, memStream);
            write.PageEvent = ev;
            Chunk SaltoLinea = Chunk.NEWLINE;

            //NEW
            doc.Open();
            doc.AddTitle(docofi.Parrafo1);

            var centrar = Element.ALIGN_CENTER;
            Paragraph paragraphTITULO = new Paragraph(docofi.Parrafo1, _fontTitulo);
            paragraphTITULO.Alignment = centrar;

            var logo = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.URLImagenLogo);
            if (logo == null)
            {
                throw new Exception("No se encontró la configuración de url de rúbrica.");
            }

            if (logo != null && logo.Valor.IsNullOrWhiteSpace())
            {
                throw new Exception("La configuración de url de rúbrica es inválida.");
            }

            Image imagenLogo = Image.GetInstance(logo.Valor);
            imagenLogo.ScalePercent(20);

            PdfPTable tableHeader = new PdfPTable(3);
            tableHeader.WidthPercentage = 100f;
            tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;
            tableHeader.DefaultCell.Border = 0;

            //logo
            PdfPCell cell = new PdfPCell(imagenLogo);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BorderWidth = 0;
            cell.PaddingTop = 20;
            cell.Border = Rectangle.NO_BORDER;
            tableHeader.AddCell(cell);

            //title
            cell = new PdfPCell(new Phrase(docofi.Parrafo1, _fontTitulo));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderWidth = 0;
            cell.PaddingTop = 20;
            cell.Border = Rectangle.NO_BORDER;
            tableHeader.AddCell(cell);

            //Id
            var paragrafId = new Paragraph(string.Format("Nro Folio XX"), _fontNumero);
            paragrafId.Alignment = Element.ALIGN_RIGHT;

            var paragrafDate = new Paragraph(string.Format("{0:dd-MM-yyyy HH:mm:ss}", DateTime.Now), _fontStandard);
            paragrafDate.Alignment = Element.ALIGN_RIGHT;

            cell = new PdfPCell();
            cell.AddElement(paragrafId);
            cell.AddElement(paragrafDate);

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderWidth = 0;
            cell.PaddingTop = 20;
            cell.Border = Rectangle.NO_BORDER;
            tableHeader.AddCell(cell);

            doc.Add(tableHeader);
            doc.Add(SaltoLinea);
            doc.Add(new Paragraph());

            /*
            ////Configuracion - Parrafos / Se deben agregar todos los parrafos aqui
            string parrafo_1 = string.Format(docofi.Parrafo1 != null ? docofi.Parrafo1 : string.Empty);
            string parrafo_2 = string.Format(docofi.Parrafo2 != null ? docofi.Parrafo2 : string.Empty);
            string parrafo_3 = string.Format(docofi.Tabla != null ? docofi.Tabla : string.Empty);

            //para cuando se necesite con color
            string parrafo_fin = "Se hace presente que no se registra en nuestros archivos la cancelación de la personalidad jurídica de dicha Cooperativa."
                             + "\n" + "\n" + "Saluda atentamente a ustedes";

            //para cuando se necesite sin color
            string parrafo_final = "Saluda atentamente a ustedes.";
            

           
            Paragraph paragraphUNO = new Paragraph(parrafo_1, _fontStandard);
            paragraphUNO.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphDOS = new Paragraph(parrafo_2, _fontStandard);
            paragraphDOS.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphTabla = new Paragraph(parrafo_3, _fontStandard);
            paragraphTabla.Alignment = Element.ALIGN_JUSTIFIED;



            doc.Add(paragraphUNO);
            doc.Add(SaltoLinea);
            doc.Add(paragraphDOS);
            doc.Add(SaltoLinea);
            doc.Add(paragraphTabla);
            doc.Add(SaltoLinea);*/
            doc.Close();
            #endregion
            return memStream.ToArray();
        }
        //TODO: Se crea nuevo metodo para documento configuracion
        public byte[] CrearDocumentoConfiguracion(ConfiguracionCertificado configuracioncertificado)
        {
            #region Configurar PreDocumento
            EventoTitulos ev = new EventoTitulos();
            Font _fontTitulo = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, BaseColor.DARK_GRAY);
            Font _fontNumero = new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, BaseColor.DARK_GRAY);
            Font _fontFirmante = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.DARK_GRAY);
            Font _fontStandard = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.DARK_GRAY);
            Font _fontStandardBold = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.DARK_GRAY);

            MemoryStream memStream = new MemoryStream();
            Document doc = new Document(PageSize.LEGAL);
            PdfWriter write = PdfWriter.GetInstance(doc, memStream);
            write.PageEvent = ev;
            Chunk SaltoLinea = Chunk.NEWLINE;

            //NEW
            doc.Open();
            doc.AddTitle(configuracioncertificado.Titulo);

            var centrar = Element.ALIGN_CENTER;
            Paragraph paragraphTITULO = new Paragraph(configuracioncertificado.Titulo, _fontTitulo);
            paragraphTITULO.Alignment = centrar;

            var logo = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.URLImagenLogo);
            if (logo == null)
            {
                throw new Exception("No se encontró la configuración de url de rúbrica.");
            }

            if (logo != null && logo.Valor.IsNullOrWhiteSpace())
            {
                throw new Exception("La configuración de url de rúbrica es inválida.");
            }

            Image imagenLogo = Image.GetInstance(logo.Valor);
            imagenLogo.ScalePercent(20);

            PdfPTable tableHeader = new PdfPTable(3);
            tableHeader.WidthPercentage = 100f;
            tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;
            tableHeader.DefaultCell.Border = 0;

            //logo
            PdfPCell cell = new PdfPCell(imagenLogo);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.BorderWidth = 0;
            cell.PaddingTop = 20;
            cell.Border = Rectangle.NO_BORDER;
            tableHeader.AddCell(cell);

            //title
            cell = new PdfPCell(new Phrase(configuracioncertificado.Titulo, _fontTitulo));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderWidth = 0;
            cell.PaddingTop = 20;
            cell.Border = Rectangle.NO_BORDER;
            tableHeader.AddCell(cell);

            //Id
            var paragrafId = new Paragraph(string.Format("Nro Folio XX"), _fontNumero);
            paragrafId.Alignment = Element.ALIGN_RIGHT;

            var paragrafDate = new Paragraph(string.Format("{0:dd-MM-yyyy HH:mm:ss}", DateTime.Now), _fontStandard);
            paragrafDate.Alignment = Element.ALIGN_RIGHT;

            cell = new PdfPCell();
            cell.AddElement(paragrafId);
            cell.AddElement(paragrafDate);

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BorderWidth = 0;
            cell.PaddingTop = 20;
            cell.Border = Rectangle.NO_BORDER;
            tableHeader.AddCell(cell);

            doc.Add(tableHeader);
            doc.Add(SaltoLinea);
            doc.Add(new Paragraph());


            ////Configuracion - Parrafos / Se deben agregar todos los parrafos aqui
            string parrafo_1 = string.Format(configuracioncertificado.Parrafo1 != null ? configuracioncertificado.Parrafo1 : string.Empty);
            string parrafo_2 = string.Format(configuracioncertificado.Parrafo2 != null ? configuracioncertificado.Parrafo2 : string.Empty);
            string parrafo_3 = string.Format(configuracioncertificado.Parrafo3 != null ? configuracioncertificado.Parrafo3 : string.Empty);
            string parrafo_4 = string.Format(configuracioncertificado.Parrafo4 != null ? configuracioncertificado.Parrafo4 : string.Empty);
            string parrafo_5 = string.Format(configuracioncertificado.Parrafo5 != null ? configuracioncertificado.Parrafo5 : string.Empty);
            string parrafo1DisPos = string.Format(configuracioncertificado.Parrafo1DisPos != null ? configuracioncertificado.Parrafo1DisPos : string.Empty);
            string parrafo1DisAnt = string.Format(configuracioncertificado.Parrafo1DisAnt != null ? configuracioncertificado.Parrafo1DisAnt : string.Empty);
            string parrafo2ExAnterior = string.Format(configuracioncertificado.Parrafo2ExAnterior != null ? configuracioncertificado.Parrafo2ExAnterior : string.Empty);
            string parrafo2ExPosterior = string.Format(configuracioncertificado.Parrafo2ExPosterior != null ? configuracioncertificado.Parrafo2ExPosterior : string.Empty);
            string parrafo4ReAnterior = string.Format(configuracioncertificado.Parrafo4ReAnterior != null ? configuracioncertificado.Parrafo4ReAnterior : string.Empty);
            string parrafo4RePosterior = string.Format(configuracioncertificado.Parrafo4RePosterior != null ? configuracioncertificado.Parrafo4RePosterior : string.Empty);
            string parrafoObservacion = string.Format(configuracioncertificado.ParrafoObservacion != null ? configuracioncertificado.ParrafoObservacion : string.Empty);

            //para cuando se necesite con color
            string parrafo_fin = "Se hace presente que no se registra en nuestros archivos la cancelación de la personalidad jurídica de dicha Cooperativa."
                             + "\n" + "\n" + "Saluda atentamente a ustedes";

            //para cuando se necesite sin color
            string parrafo_final = "Saluda atentamente a ustedes.";
            #endregion

            #region Declarar / Configurar Parrafos
            Paragraph paragraphUNO = new Paragraph(parrafo_1, _fontStandard);
            paragraphUNO.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphDOS = new Paragraph(parrafo_2, _fontStandard);
            paragraphDOS.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphTRES = new Paragraph(parrafo_3, _fontStandard);
            paragraphTRES.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphCUATRO = new Paragraph(parrafo_4, _fontStandard);
            paragraphCUATRO.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphCINCO = new Paragraph(parrafo_5, _fontStandard);
            paragraphCINCO.Alignment = Element.ALIGN_JUSTIFIED;

            //Parrafos "Dinamicos de cada Organizacion"

            Paragraph paragraphUNODISANT = new Paragraph(parrafo1DisAnt, _fontStandard);
            paragraphUNODISANT.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphUNODISPOST = new Paragraph(parrafo1DisPos, _fontStandard);
            paragraphUNODISPOST.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphDOSEXANT = new Paragraph(parrafo2ExAnterior, _fontStandard);
            paragraphDOSEXANT.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphDOSEXPOST = new Paragraph(parrafo2ExPosterior, _fontStandard);
            paragraphDOSEXPOST.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphCUATROREANT = new Paragraph(parrafo4ReAnterior, _fontStandard);
            paragraphCUATROREANT.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphCUATROREPOST = new Paragraph(parrafo4RePosterior, _fontStandard);
            paragraphCUATROREPOST.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphOBSERVACION = new Paragraph(parrafoObservacion, _fontStandard);
            paragraphOBSERVACION.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphFINAL = new Paragraph(parrafo_final, _fontStandard);
            paragraphFINAL.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph paragraphFIN = new Paragraph(parrafo_fin, _fontStandard);
            paragraphFIN.Alignment = Element.ALIGN_JUSTIFIED;

            Paragraph responsable = new Paragraph("Texto para Responsable", _fontStandardBold);
            responsable.Alignment = centrar;

            Paragraph _cargo = new Paragraph("Texto para cargo de Responsable", _fontStandardBold);
            _cargo.Alignment = centrar;

            Paragraph _unidad = new Paragraph("Unidad a la que pertenece el Responsable", _fontStandardBold);
            _unidad.Alignment = centrar;

            #endregion

            //cambiar disolucion de Cooperativas usa mas parrafos
            var definicion = "";
            switch (configuracioncertificado.ConfiguracionCertificadoId)
            {
                case 1009:
                case 1008:
                    definicion = "Disolucion";
                    break;
                case 1010:
                    definicion = "DisolucionCOOP";
                    break;
                case 4:
                case 6:
                case 1:
                    definicion = "Vigencia";
                    break;
                case 17:
                case 18:
                    definicion = "VigenciaEstatutosAGAC";
                    break;
                case 16:
                    definicion = "VigenciaEstatutosCOOP";
                    break;
                case 3:
                case 2:
                case 5:
                    definicion = "VigenciaDirectorio";
                    break;
            }

            switch (definicion)
            {
                case "Disolucion":
                    doc.Add(paragraphUNO);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphOBSERVACION);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphFINAL);
                    break;
                case "DisolucionCOOP":
                    doc.Add(paragraphUNODISANT);
                    doc.Add(paragraphUNODISPOST);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphOBSERVACION);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphFINAL);
                    break;
                case "Vigencia":
                    doc.Add(paragraphUNO);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphOBSERVACION);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphFINAL);
                    break;
                case "VigenciaEstatutosAGAC":
                    doc.Add(paragraphUNO);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphDOS);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphCUATRO);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphOBSERVACION);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphFINAL);
                    break;
                case "VigenciaEstatutosCOOP":
                    doc.Add(paragraphUNO);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphTRES);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphDOSEXANT);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphDOSEXPOST);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphCUATROREANT);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphCUATROREPOST);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphOBSERVACION);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphFIN);
                    break;
                case "VigenciaDirectorio":
                    doc.Add(paragraphUNO);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphDOS);
                    doc.Add(SaltoLinea);

                    PdfPTable table = new PdfPTable(4);
                    table.WidthPercentage = 100.0f;
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.DefaultCell.BorderColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(new PdfPCell(new Phrase("Cargo", _fontStandard)));
                    table.AddCell(new PdfPCell(new Phrase("Nombre", _fontStandard)));
                    PdfPCell cellDesde = new PdfPCell(new Phrase("Desde", _fontStandardBold));
                    cellDesde.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cellDesde);
                    PdfPCell cellHasta = new PdfPCell(new Phrase("Hasta", _fontStandardBold));
                    cellHasta.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cellHasta);

                    doc.Add(table);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphOBSERVACION);
                    doc.Add(SaltoLinea);
                    doc.Add(paragraphFINAL);
                    break;
            }

            doc.Add(SaltoLinea);
            doc.Add(responsable);
            doc.Add(_cargo);
            doc.Add(_unidad);
            doc.Close();
            return memStream.ToArray();
        }


        public byte[] CrearCertificadoPDF(ConfiguracionCertificado configuracioncertificado, Organizacion organizacion, Firmante firmante, int id, int TipoDocumentoId)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                EventoTitulos ev = new EventoTitulos();
                Font _fontTitulo = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, BaseColor.DARK_GRAY);
                Font _fontTituloSegundario = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.DARK_GRAY);
                Font _fontNumero = new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, BaseColor.DARK_GRAY);
                Font _fontFirmante = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.DARK_GRAY);
                Font _fontStandard = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.DARK_GRAY);
                Font _fontVariables = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);//variables??
                Font _fontStandardBold = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.DARK_GRAY);
                Font _fontStandardBoldSubRayado = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD | Font.UNDERLINE, BaseColor.DARK_GRAY);
                Font _fontNegrita = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD);
                MemoryStream memStream = new MemoryStream();
                Document doc = new Document(PageSize.LEGAL);
                PdfWriter write = PdfWriter.GetInstance(doc, memStream);
                write.PageEvent = ev;
                Chunk SaltoLinea = Chunk.NEWLINE;
                int index = 0;
                string parrafo_uno = string.Format(configuracioncertificado.Parrafo1 != null ? configuracioncertificado.Parrafo1 : " ");
                string parrafo_dos = string.Format(configuracioncertificado.Parrafo2 != null ? configuracioncertificado.Parrafo2 : " ");
                string parrafo_tres = string.Format(configuracioncertificado.Parrafo3 != null ? configuracioncertificado.Parrafo3 : " ");
                string parrafo_cuatro = string.Format(configuracioncertificado.Parrafo4 != null ? configuracioncertificado.Parrafo4 : " ");
                string parrafo_cinco = string.Format(configuracioncertificado.Parrafo5 != null ? configuracioncertificado.Parrafo5 : " ");
                string parrafoone = string.Format(configuracioncertificado.Parrafo1 != null ? configuracioncertificado.Parrafo1 : " ");
                string parrafos = string.Format(configuracioncertificado.Parrafo1 != null ? configuracioncertificado.Parrafo1 : " ");

                var tipoOrganizacion = context.TipoOrganizacion.Find(organizacion.TipoOrganizacionId);

                doc.Open();

                doc.AddTitle(configuracioncertificado.Titulo);
                doc.AddAuthor(firmante.Nombre);

                var centrar = Element.ALIGN_CENTER;
                Paragraph paragraphTITULO = new Paragraph(configuracioncertificado.Titulo, _fontTitulo);
                paragraphTITULO.Alignment = centrar;

                Paragraph paragraphDOS = new Paragraph(parrafo_dos, _fontStandard);
                paragraphDOS.Alignment = Element.ALIGN_JUSTIFIED;

                Paragraph paragraphTRES = new Paragraph(parrafo_tres, _fontStandard);
                paragraphTRES.Alignment = Element.ALIGN_JUSTIFIED;

                Paragraph paragraphCUATRO = new Paragraph(parrafo_cuatro, _fontStandard);
                paragraphCUATRO.Alignment = Element.ALIGN_JUSTIFIED;

                Paragraph rae = new Paragraph(configuracioncertificado.Parrafo3, _fontStandard);
                rae.Alignment = Element.ALIGN_JUSTIFIED;

                Phrase PhraseUNO;
                Phrase PhraseDOS;
                Phrase PhraseDOSSuperior;
                Phrase PhraseDOSInferior;
                Phrase PhraseTRES;
                Phrase PhraseCINCO;
                Paragraph subtitulo;
                Paragraph subtitulo2 = null;
                Paragraph subtitulo3 = null;
                Paragraph comb = new Paragraph();
                Paragraph comb2 = new Paragraph();
                Paragraph comb2Superior = new Paragraph();
                Paragraph comb2Inferior = new Paragraph();
                Paragraph comb3 = new Paragraph();
                Paragraph comb4Superior = new Paragraph();
                Paragraph comb4Inferior = new Paragraph();
                Phrase PhraseCUATROSuperior;
                Phrase PhraseCUATROInferior;
                var logo = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.URLImagenLogo);
                if (logo == null)
                {
                    throw new Exception("No se encontró la configuración de url de rúbrica.");
                }

                if (logo != null && logo.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("La configuración de url de rúbrica es inválida.");
                }

                Image imagenLogo = Image.GetInstance(logo.Valor);
                imagenLogo.ScalePercent(20);

                PdfPTable tableHeader = new PdfPTable(3);
                tableHeader.WidthPercentage = 100f;
                tableHeader.DefaultCell.Border = Rectangle.NO_BORDER;
                tableHeader.DefaultCell.Border = 0;

                //logo
                PdfPCell cell = new PdfPCell(imagenLogo);
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.BorderWidth = 0;
                cell.PaddingTop = 20;
                cell.Border = Rectangle.NO_BORDER;
                tableHeader.AddCell(cell);

                //title
                string[] titulo = configuracioncertificado.Titulo.Split('#');
                Phrase PhraseTitulo;
                Paragraph tituloFinal = new Paragraph();
                index = 0;
                foreach (var element in titulo)
                {
                    if (index == 0)
                    {
                        PhraseTitulo = new Phrase(element, _fontTitulo);
                    }
                    else
                    {
                        PhraseTitulo = new Phrase(SaltoLinea + element, _fontTituloSegundario);

                    }
                    tituloFinal.Add(PhraseTitulo);
                    index++;
                }
                //cell = new PdfPCell(new Phrase(configuracioncertificado.Titulo, _fontTitulo));
                cell = new PdfPCell(new Phrase(tituloFinal));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BorderWidth = 0;
                cell.PaddingTop = 20;
                cell.Border = Rectangle.NO_BORDER;
                tableHeader.AddCell(cell);

                ////Id
                //var paragrafId = new Paragraph(string.Format("N° {0}", id), _fontNumero);
                //paragrafId.Alignment = Element.ALIGN_RIGHT;

                //var paragrafDate = new Paragraph(string.Format("Emitido el {0:dd-MM-yyyy HH:mm:ss}", DateTime.Now), _fontStandard);
                //paragrafDate.Alignment = Element.ALIGN_RIGHT;

                cell = new PdfPCell();
                //cell.AddElement(paragrafId);
                //cell.AddElement(paragrafDate);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.BorderWidth = 0;
                cell.PaddingTop = 20;
                cell.Border = Rectangle.NO_BORDER;
                tableHeader.AddCell(cell);

                doc.Add(tableHeader);
                doc.Add(new Paragraph());


                if (TipoDocumentoId == (int)Infrastructure.Enum.TipoDocumento.Articulo8voTransitorio)
                {

                    // PARRAFO 1 INI
                    var legalAnt = context.ExistenciaLegalAnterior.FirstOrDefault(q => q.OrganizacionId == organizacion.OrganizacionId);
                    var transitorio = context.Transitorio.FirstOrDefault(q => q.OrganizacionId == organizacion.OrganizacionId);
                    if (transitorio != null)
                    {
                        parrafo_uno = parrafo_uno.Replace("[RAZONSOCIAL]", organizacion.RazonSocial != null ? "#" + organizacion.RazonSocial.ToString() + "#" : "[RAZONSOCIAL]");
                        parrafo_uno = parrafo_uno.Replace("[FECHAJUNTAGENERALSOCIOS]", legalAnt.FechaJuntaGeneral != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalAnt.FechaJuntaGeneral.Value) + "#" : "[ERROR_Legal]");
                        parrafo_uno = parrafo_uno.Replace("[FECHAESCRITURA]", legalAnt.FechaEscritura != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalAnt.FechaEscritura.Value) + "#" : "[ERROR_Legal]");
                        parrafo_uno = parrafo_uno.Replace("[LUGARNOTARIO]", legalAnt.LugarNotario != null ? "#" + legalAnt.LugarNotario + "#" : "[ERROR_Legal]");
                        parrafo_uno = parrafo_uno.Replace("[DATOSGENERALESNOTARIO]", legalAnt.DatosGeneralNotario != null ? "#" + legalAnt.DatosGeneralNotario + "#" : "[ERROR_Legal]");

                        parrafo_uno = parrafo_uno.Replace("[OBJETO]", transitorio.Objeto != null ? "#" + transitorio.Objeto.ToString() + "#" : "[ERROR_Transitorio]");
                    }
                    else
                    {
                        throw new Exception(string.Format("Error al emitir, el apartado Transitorio no existe"));
                    }


                    string[] parafo1 = parrafo_uno.Split('#');
                    if (parrafo_uno.Contains("ERROR_Transitorio"))
                    {
                        throw new Exception(string.Format("Error al emitir, el apartado Transitorio no tiene todos los campos requeridos"));
                    }else if(parrafo_uno.Contains("ERROR_Legal"))
                    {
                        throw new Exception(string.Format("Error al emitir, el apartado Transitorio no tiene todos los campos requeridos"));
                    }
                    foreach (string element in parafo1)
                    {
                        if (index % 2 == 0)
                        {
                            PhraseUNO = new Phrase(element, _fontStandard);
                        }
                        else
                        {
                            PhraseUNO = new Phrase(element, _fontNegrita);
                        }
                        comb.Add(PhraseUNO);
                        index++;
                    }
                    comb.Alignment = Element.ALIGN_JUSTIFIED;
                    doc.Add(SaltoLinea);
                    doc.Add(SaltoLinea);
                    doc.Add(SaltoLinea);
                    doc.Add(SaltoLinea);

                    doc.Add(comb);
                    doc.Add(SaltoLinea);
                    // PARRAFO 1 - FIN
                    // PARRAFO 2 - INI
  
                    if (legalAnt != null)
                    {
                        parrafo_dos = parrafo_dos.Replace("[TIPONORMA]", legalAnt.tipoNorma != null ? "#" + legalAnt.tipoNorma.Nombre + "#" : "[ERROR_Legal]");
                        parrafo_dos = parrafo_dos.Replace("[NUMERONORMA]", legalAnt.NNorma != null ? "#" + legalAnt.NNorma + "#" : "[ERROR_Legal]");
                        parrafo_dos = parrafo_dos.Replace("[FECHANORMA]", legalAnt.FNorma != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalAnt.FNorma) + "#" : "[ERROR_Legal]");
                        parrafo_dos = parrafo_dos.Replace("[AUTORIZADOPOR]", legalAnt.Autorizado != null ? "#" + legalAnt.Autorizado + "#" : "[ERROR_Legal]");
                        parrafo_dos = parrafo_dos.Replace("[FECHAASAMBLEASOCIOS]", legalAnt.FechaPublicacion != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalAnt.FechaPublicacion) + "#" : "[ERROR_Legal]");
                    }
                    else
                    {
                        throw new Exception(string.Format("Error al emitir, el apartado Legal no existe"));
                    }

                    string[] parrafo2 = parrafo_dos.Split('#');
                    if (parrafo_dos.Contains("ERROR_Legal"))
                    {
                        throw new Exception(string.Format("Error al emitir, el apartado Legal no tiene todos los campos requeridos"));
                    }
                    index = 0;
                    foreach (string element in parrafo2)
                    {
                        if (index % 2 == 0)
                        {
                            PhraseDOS = new Phrase(element, _fontStandard);
                        }
                        else
                        {
                            PhraseDOS = new Phrase(element, _fontNegrita);
                        }
                        comb2.Add(PhraseDOS);
                        index++;
                    }
                    comb.Alignment = Element.ALIGN_JUSTIFIED;

                    doc.Add(comb2);
                    doc.Add(SaltoLinea);
                    // PARRAFO 2 - FIN
                    // PARRAFO 3 - INI
                    if (organizacion.ReformaAnteriors.Any())
                    {
                        var posicion = 0;
                        var cantiReforma = organizacion.ReformaAnteriors.Count();
                        foreach (var item in organizacion.ReformaAnteriors.OrderByDescending(q => q.FechaReforma))
                        {
                            posicion++;
                            var parrafoActual = parrafo_tres;
                            if (posicion == cantiReforma)
                            {
                                parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + posicion + "° #y última");
                            }
                            else
                            {
                                parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + posicion + "°" + "#");
                            }
                            parrafoActual = parrafoActual.Replace("[TIPONORMA]", item.TipoNormaId != null ? "#" + item.TipoNorma.Nombre + "#" : "[ERROR_Reforma]");
                            parrafoActual = parrafoActual.Replace("[NUMERONORMA]", item.NNorma != null ? "#" + item.NNorma + "#" : "[ERROR_Reforma]");
                            parrafoActual = parrafoActual.Replace("[FECHANORMA]", item.FechaNorma.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaNorma.Value) + "#" : "[ERROR_Reforma]");
                            parrafoActual = parrafoActual.Replace("[AUTORIZADOPOR]", item.AutorizadoPor != null ? "#" + item.AutorizadoPor + "#" : "[ERROR_Reforma]");
                            parrafoActual = parrafoActual.Replace("[FECHAPUBLICACION]", item.FechaPublicDiario != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaPublicDiario.Value) + "#" : "[ERROR_Reforma]");
                            parrafoActual = parrafoActual.Replace("[TIPOJUNTA]", item.TipoGeneralId != null ? "#" + item.TipoGeneral.Nombre + "#" : "[ERROR_Reforma]");
                            parrafoActual = parrafoActual.Replace("[FECHAJUNTAGENERALSOCIOS]", item.FechaJuntaGeneral != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaJuntaGeneral.Value) + "#" : "[ERROR_Reforma]");
                            parrafoActual = parrafoActual.Replace("[FECHAESCRITURA]", item.FechaEscritura != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaEscritura.Value) + "#" : "[ERROR_Reforma]");
                            parrafoActual = parrafoActual.Replace("[LUGARNOTARIO]", item.LugarNotario != null ? "#" + item.LugarNotario + "#" : "[ERROR_Reforma]");
                            parrafoActual = parrafoActual.Replace("[DATOGENERALNOTARIO]", item.DatosNotario != null ? "#" + item.DatosNotario + "#" : "[ERROR_Reforma]");
                            string[] parrafo3 = parrafoActual.Split('#');
                            if (parrafoActual.Contains("ERROR_Reforma"))
                            {
                                throw new Exception(string.Format("Error al emitir, el apartado Reforma no tiene todos los campos requeridos"));
                            }
                            index = 0;
                            foreach (string element in parrafo3)
                            {
                                if (index % 2 == 0)
                                {
                                    PhraseTRES = new Phrase(element, _fontStandard);
                                }
                                else
                                {
                                    PhraseTRES = new Phrase(element, _fontNegrita);
                                }
                                comb3.Add(PhraseTRES);
                                
                                index++;
                            }
                            comb3.Add(SaltoLinea);
                            comb3.Alignment = Element.ALIGN_JUSTIFIED;
                        }


                    }
                    else
                    {
                        comb3 = new Paragraph("No se registran actas complementarias, rectificatorias o modificatorias.", _fontStandard);
                    }


                    doc.Add(comb3);
                    doc.Add(SaltoLinea);
                    // PARRAFO 3 - FIN


                }

                if (TipoDocumentoId == (int)Infrastructure.Enum.TipoDocumento.Estatutos)
                {


                    if (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.Cooperativa)
                    {
                        var legalAnt = organizacion.ExistenciaAnteriors.FirstOrDefault(q => q.OrganizacionId == organizacion.OrganizacionId);
                        var legarPost = organizacion.ExistenciaPosteriors.FirstOrDefault(q => q.OrganizacionId == organizacion.OrganizacionId);
                        if (organizacion.ExistenciaAnteriors.Any()) // estatuto anterior 2003
                        {
                            // PARRAFO 1 - INI  estatuto anterior 2003                        
                            parrafo_uno = parrafo_uno.Replace("[TIPONORMA]", legalAnt.TipoNormaId != null ? "#" + legalAnt.tipoNorma.Nombre + "#" : "[ERROR_legalAnt]");
                            parrafo_uno = parrafo_uno.Replace("[NUMERONORMA]", legalAnt.NNorma != null ? "#" + legalAnt.NNorma + "#" : "[ERROR_legalAnt]");
                            parrafo_uno = parrafo_uno.Replace("[FECHANORMA]", legalAnt.FNorma != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalAnt.FNorma.Value) + "#" : "[ERROR_legalAnt]");
                            parrafo_uno = parrafo_uno.Replace("[AUTORIZADOPOR]", legalAnt.Autorizado != null ? "#" + legalAnt.Autorizado + "#" : "[ERROR_legalAnt]");
                            parrafo_uno = parrafo_uno.Replace("[FECHAPUBLICACION]", legalAnt.FechaPublicacion.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalAnt.FechaPublicacion.Value) + "#" : "[ERROR_legalAnt]");
                            parrafo_uno = parrafo_uno.Replace("[RAZONSOCIAL]", organizacion.RazonSocial != null ? "#" + organizacion.RazonSocial + "#" : "[ERROR_legalAnt]");
                            parrafo_uno = parrafo_uno.Replace("[SIGLA]", organizacion.Sigla != null ? "la que podrá usar indistintamente para todos los efectos legales el nombre de fantasía #" + organizacion.Sigla + "#," : "");//Opcional
                            parrafo_uno = parrafo_uno.Replace("[NUMEROREGISTRO]", organizacion.NumeroRegistro != null ? "#" + organizacion.NumeroRegistro + "#" : "[ERROR_legalAnt]");

                            string[] parafo1;
                            parafo1 = parrafo_uno.Split('#');
                            if (parrafo_uno.Contains("ERROR_legalAnt"))
                            {
                                throw new Exception(string.Format("Error al emitir, el apartado legal no tiene todos los campos requeridos"));
                            }

                            foreach (string element in parafo1)
                            {
                                if (index % 2 == 0)
                                {
                                    PhraseUNO = new Phrase(element, _fontStandard);
                                }
                                else
                                {
                                    PhraseUNO = new Phrase(element, _fontNegrita);
                                }
                                comb.Add(PhraseUNO);
                                index++;
                            }
                            comb.Alignment = Element.ALIGN_JUSTIFIED;
                            doc.Add(SaltoLinea);
                            doc.Add(SaltoLinea);
                            doc.Add(SaltoLinea);
                            doc.Add(SaltoLinea);
                            doc.Add(comb);
                            doc.Add(SaltoLinea);
                            //PARRAFO 1 - FIN
                            //PARRAFO 2 - INI estatuto anterior 2003
                            var ReformaVigente = true;
                            if (organizacion.ReformaPosteriors.Any())
                            {
                                var Reforma = organizacion.ReformaPosteriors.ToList().OrderByDescending(q => q.FechaPubliDiario);
                                var ReformaUlti = new ReformaPosterior();
                                foreach (var item in Reforma)
                                {
                                    if ((item.ObservacionReformas.Any() &&
                                        item.ObservacionReformas.FirstOrDefault().AprobacionId == (int)Infrastructure.Enum.Aprobacion.aprobada) ||
                                        !item.ObservacionReformas.Any() || item.ObservacionReformas.FirstOrDefault().AprobacionId == null
                                        )
                                    {
                                        ReformaUlti = item;
                                        break;
                                    }
                                }
                                if (ReformaUlti.IdReformaPost != 0) // cuenta con al menos una reforma sin obs o con obs aprobada
                                {
                                    string[] parrafo2;
                                    parrafo2 = parrafo_dos.Split('#');
                                    subtitulo2 = new Paragraph(parrafo2[0], _fontStandardBoldSubRayado);
                                    parrafo2 = parrafo2.Where(n => n != parrafo2[0]).ToArray();
                                    parrafo2[0] = parrafo2[0].Replace("[TIPOGENERAL]", ReformaUlti.TipoGeneralId != null ? "#" + ReformaUlti.TipoGeneral.Nombre + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[EXISTEREFORMA]", "#reformado#");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHAJUNTAGENERALSOCIOS]", ReformaUlti.FechaJuntGeneralSocios.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaJuntGeneralSocios.Value) + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHAESCRITURA]", ReformaUlti.FechaEscrituraPublica.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaEscrituraPublica.Value) + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[LUGARNOTARIO]", ReformaUlti.LugarNotario != null ? "#" + ReformaUlti.LugarNotario + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[DATOGENERALNOTARIO]", ReformaUlti.DatosGeneralNotario != null ? "#" + ReformaUlti.DatosGeneralNotario + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHAPUBLICACION]", ReformaUlti.FechaPubliDiario.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaPubliDiario.Value) + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[FOJASNUMERO]", ReformaUlti.FojasNumero != null ? "#" + ReformaUlti.FojasNumero + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[COMUNACBR]", ReformaUlti.DatosCBR != null ? "#" + ReformaUlti.DatosCBR + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[AÑOINSCRIPCION]", ReformaUlti.AnoInscripcion != null ? "#" + ReformaUlti.AnoInscripcion + "#" : "ERROR_Reforma");
                                    if (parrafo2[0].Contains("ERROR_Reforma"))
                                    {
                                        throw new Exception(string.Format("Error al emitir, el apartado de Reforma no tiene todos los campos requeridos"));
                                    }
                                    var aprobadoObsReforma = false;
                                    if (ReformaUlti.ObservacionReformas != null)
                                    {
                                        parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", organizacion.ObservacionReformas.FirstOrDefault().NumeroOficio != null ? "Dicha acta estatutaria se encuentra aprobada por oficio ordinario N° #" + organizacion.ObservacionReformas.FirstOrDefault().NumeroOficio + "#" : organizacion.ObservacionReformas.FirstOrDefault().FechaOficio != null ? "" : ".");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", organizacion.ObservacionReformas.FirstOrDefault().FechaOficio != null && organizacion.ObservacionReformas.FirstOrDefault().FechaOficio != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.ObservacionReformas.FirstOrDefault().FechaOficio.Value) + "#." : "");
                                        aprobadoObsReforma = true;
                                    }

                                    if (aprobadoObsReforma == false)
                                    {
                                        parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", "");
                                    }


                                    parrafo2 = parrafo2[0].Split('#');

                                    index = 0;
                                    foreach (string element in parrafo2)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseDOS = new Phrase(element, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseDOS = new Phrase(element, _fontNegrita);
                                        }
                                        comb2.Add(PhraseDOS);
                                        index++;
                                    }
                                }
                                else
                                {
                                    ReformaVigente = false;
                                }
                            }
                            if (organizacion.ReformaAnteriors.Any() && ReformaVigente == false)
                            {
                                
                                var Reforma = organizacion.ReformaAnteriors.ToList().OrderByDescending(q => q.FechaPublicDiario);
                                var ReformaUlti = new ReformaAnterior();
                                foreach (var item in Reforma)
                                {
                                    if ((item.ObservacionReformas.Any() &&
                                        item.ObservacionReformas.FirstOrDefault().AprobacionId == (int)Infrastructure.Enum.Aprobacion.aprobada) ||
                                        !item.ObservacionReformas.Any() || item.ObservacionReformas.FirstOrDefault().AprobacionId == null
                                        )
                                    {
                                        ReformaUlti = item;
                                        break;
                                    }
                                }

                                if (ReformaUlti.IdReformaAnterior != 0) //cuenta con al menos una reforma sin obs o con obs aprobada
                                {
                                    ReformaVigente = true;
                                    var ObsReforma = ReformaUlti.ObservacionReformas.FirstOrDefault();
                                    string[] parrafo2;
                                    parrafo2 = parrafo_dos.Split('#');
                                    subtitulo2 = new Paragraph(parrafo2[0], _fontStandardBoldSubRayado);
                                    parrafo2 = parrafo2.Where(n => n != parrafo2[0]).ToArray();
                                    parrafo2[0] = parrafo2[0].Replace("[TIPOGENERAL]", ReformaUlti.TipoGeneralId != null ? "#" + ReformaUlti.TipoGeneral.Nombre + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[EXISTEREFORMA]", "#reformado#");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHAJUNTAGENERALSOCIOS]", ReformaUlti.FechaJuntaGeneral.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaJuntaGeneral.Value) + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHAESCRITURA]", ReformaUlti.FechaEscritura.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaEscritura.Value) + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[LUGARNOTARIO]", ReformaUlti.LugarNotario != null ? "#" + ReformaUlti.LugarNotario + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[DATOGENERALNOTARIO]", ReformaUlti.DatosNotario != null ? "#" + ReformaUlti.DatosNotario + "#" : "ERROR_Reforma");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHAPUBLICACION]", ReformaUlti.FechaPublicDiario.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaPublicDiario.Value) + "#" : "ERROR_Reforma");
                                    if (parrafo2[0].Contains("ERROR_Reforma"))
                                    {
                                        throw new Exception(string.Format("Error al emitir, el apartado de Reforma no tiene todos los campos requeridos"));
                                    }
                                    if (ObsReforma != null)
                                    {
                                        if (ObsReforma.AprobacionId == (int)Infrastructure.Enum.Aprobacion.aprobada)
                                        {
                                            parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", ReformaUlti.TipoNormaId != null ? "#" + ReformaUlti.TipoNorma.Nombre + "#" : "ERROR_Opcional");
                                            parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", ReformaUlti.NNorma != null ? "#" + ReformaUlti.NNorma + "#" : "ERROR_Opcional");
                                            parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", ReformaUlti.FechaNorma.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaNorma.Value) + "#" : "ERROR_Opcional");
                                            parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", ReformaUlti.AutorizadoPor != null ? "#" + ReformaUlti.AutorizadoPor + "#" : "ERROR_Opcional");
                                            parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", ObsReforma.NumeroOficio != null ? "Dicha acta estatutaria se encuentra aprobada por oficio ordinario N° #" + organizacion.ObservacionReformas.FirstOrDefault().NumeroOficio + "#" : organizacion.ObservacionReformas.FirstOrDefault().FechaOficio != null ? "" : ".");
                                            parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", ObsReforma.FechaOficio != null && ObsReforma.FechaOficio != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.ObservacionReformas.FirstOrDefault().FechaOficio.Value) + "#." : "");
                                            if (parrafo2[0].Contains("ERROR_Opcional"))
                                            {
                                                parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", "");
                                                parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", "");
                                                parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", "");
                                                parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", "");
                                                parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", "");
                                                parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", "");
                                                parrafo2[0] = parrafo2[0].Replace("ERROR_Opcional", "");
                                            }
                                        }

                                    }
                                    else
                                    {
                                        parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", "");
                                    }


                                    parrafo2 = parrafo2[0].Split('#');

                                    index = 0;
                                    foreach (string element in parrafo2)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseDOS = new Phrase(element, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseDOS = new Phrase(element, _fontNegrita);
                                        }
                                        comb2.Add(PhraseDOS);
                                        index++;
                                    }
                                }
                                else
                                {
                                    ReformaVigente = false;
                                }

                            }
                            if(ReformaVigente == false) // NO TIENE REFORMAS
                            {
                                var ObsLegal = organizacion.ExistenciaAnteriors.FirstOrDefault().ObservacionLegals.FirstOrDefault();
                                string[] parrafo2;
                                parrafo2 = parrafo_tres.Split('#');
                                subtitulo2 = new Paragraph(parrafo2[0], _fontStandardBoldSubRayado);
                                parrafo2 = parrafo2.Where(n => n != parrafo2[0]).ToArray();
                                parrafo2[0] = parrafo2[0].Replace("[EXISTEREFORMA]", "#acordado#");
                                parrafo2[0] = parrafo2[0].Replace("[TIPOGENERAL]", legalAnt.TipoGeneralId != null ? "#" + legalAnt.TipoGeneral.Nombre + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[FECHAJUNTAGENERALSOCIOS]", legalAnt.FechaJuntaGeneral.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalAnt.FechaJuntaGeneral.Value) + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[FECHAESCRITURA]", legalAnt.FechaEscritura.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalAnt.FechaEscritura.Value) + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[LUGARNOTARIO]", legalAnt.LugarNotario != null ? "#" + legalAnt.LugarNotario + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[DATOGENERALNOTARIO]", legalAnt.DatosGeneralNotario != null ? "#" + legalAnt.DatosGeneralNotario + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[FECHAPUBLICACION]", legalAnt.FechaPublicacion.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalAnt.FechaPublicacion.Value) + "#" : "ERROR_Legal");
                                if (parrafo2[0].Contains("ERROR_Legal"))
                                {
                                    throw new Exception(string.Format("Error al emitir, el apartado de Legal no tiene todos los campos requeridos"));
                                }

                                if (ObsLegal != null)
                                {
                                    if (ObsLegal.AprobacionId == (int)Infrastructure.Enum.Aprobacion.aprobada)
                                    {
                                        parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", legalAnt.TipoNormaId != null ? "#" + legalAnt.tipoNorma.Nombre + "#" : "ERROR_Opcional");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", legalAnt.NNorma != null ? "#" + legalAnt.NNorma + "#" : "ERROR_Opcional");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", legalAnt.FNorma != null ? "#" + legalAnt.FNorma + "#" : "ERROR_Opcional");
                                        parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", legalAnt.Autorizado != null ? "#" + legalAnt.Autorizado + "#" : "ERROR_Opcional");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", ObsLegal.NumeroOficio != null ? "#" + ObsLegal.NumeroOficio + "#" : "ERROR_Opcional");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", ObsLegal.FechaOficio.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}#", ObsLegal.FechaOficio.Value) + "#" : "ERROR_Opcional");
                                        if (parrafo2[0].Contains("ERROR_Opcional"))
                                        {
                                            parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", "");
                                            parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", "");
                                            parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", "");
                                            parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", "");
                                            parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", "");
                                            parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", "");
                                            parrafo2[0] = parrafo2[0].Replace("ERROR_Opcional", "");
                                        }
                                    }

                                }
                                else
                                {
                                    parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", "");
                                }


                                parrafo2 = parrafo2[0].Split('#');

                                index = 0;
                                foreach (string element in parrafo2)
                                {
                                    if (index % 2 == 0)
                                    {
                                        PhraseDOS = new Phrase(element, _fontStandard);
                                    }
                                    else
                                    {
                                        PhraseDOS = new Phrase(element, _fontNegrita);
                                    }
                                    comb2.Add(PhraseDOS);
                                    index++;
                                }
                            }

                            doc.Add(subtitulo2);
                            doc.Add(SaltoLinea);
                            doc.Add(comb2);
                            doc.Add(SaltoLinea);
                            //PARRAFO 2 - FIN
                            //PARRAFO 3 - INI estatuto anterior 2003
                            if (organizacion.Saneamientos.Count() == 0 && organizacion.ReformaAnteriors.Count() == 0 && organizacion.ReformaPosteriors.Count() == 0)
                            {
                                PhraseTRES = new Phrase("No se registran actas complementarias, rectificatorias o modificatorias", _fontStandard);
                                comb3.Add(PhraseTRES);
                                doc.Add(comb3);
                                doc.Add(SaltoLinea);
                            }
                            if(organizacion.Saneamientos.Any() || organizacion.ReformaAnteriors.Any() || organizacion.ReformaPosteriors.Any())
                            {
                                var sanamientoParrafo = configuracioncertificado.Parrafo1DisAnt;
                                var saniamientoAnt = organizacion.Saneamientos.FirstOrDefault();
                                string[] parrafo3;
                                parrafo3 = sanamientoParrafo.Split('#');
                                subtitulo3 = new Paragraph(parrafo3[0], _fontStandardBoldSubRayado);
                                parrafo3 = parrafo3.Where(n => n != parrafo3[0]).ToArray();
                                if (organizacion.Saneamientos.Any())
                                {
                                    parrafo3[0] = parrafo3[0].Replace("[EXISTEREFORMA]", organizacion.ReformaAnteriors.Any() || organizacion.ReformaPosteriors.Any() ? "El acta contitutiva" : "La reforma estatutaria");
                                    parrafo3[0] = parrafo3[0].Replace("[FECHAESCRITURA]", saniamientoAnt.FechaEscrituraPublicaa.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", saniamientoAnt.FechaEscrituraPublicaa.Value) + "#" : "ERROR_Saneamiento");
                                    parrafo3[0] = parrafo3[0].Replace("[LUGARNOTARIO]", saniamientoAnt.LugarNotario != null ? "#" + saniamientoAnt.LugarNotario + "#" : "ERROR_Saneamiento");
                                    parrafo3[0] = parrafo3[0].Replace("[DATOGENERALNOTARIO]", saniamientoAnt.DatoGeneralesNotario != null ? "#" + saniamientoAnt.DatoGeneralesNotario + "#" : "ERROR_Saneamiento");
                                    parrafo3[0] = parrafo3[0].Replace("[FECHAPUBLICACION]", saniamientoAnt.FechaaPublicacionDiario.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", saniamientoAnt.FechaaPublicacionDiario.Value) + "#" : "ERROR_Saneamiento");
                                    if (parrafo3[0].Contains("ERROR_Saneamiento"))
                                    {
                                        throw new Exception(string.Format("Error al emitir, el apartado de Saneamiento no tiene todos los campos requeridos"));
                                    }
                                    parrafo3 = parrafo3[0].Split('#');
                                    index = 0;
                                    foreach (var item in parrafo3)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseTRES = new Phrase(item, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseTRES = new Phrase(item, _fontNegrita);
                                        }
                                        comb3.Add(PhraseTRES);
                                        index++;
                                    }
                                }
                                comb3.Alignment = Element.ALIGN_JUSTIFIED;
                                doc.Add(subtitulo3);
                                doc.Add(SaltoLinea);
                                if(organizacion.Saneamientos.Any())
                                {
                                    doc.Add(comb3);
                                    doc.Add(SaltoLinea);
                                }

                            }
                            //PARRAFO 3 - FIN
                            //PARRAFO 4 - INI estatuto anterior 2003

                            if (organizacion.ReformaAnteriors.Any()) // estatuto anterior 2003 reforma ante 
                            {
                                var ReformaParrafo = configuracioncertificado.Parrafo2ExAnterior;
                                var Reforma = organizacion.ReformaAnteriors.OrderBy(q => q.FechaPublicDiario);
                                string[] parrafo4Superior;
                                var IndReforma = 0;
                                var cantiReforma = organizacion.ReformaAnteriors.Count();
                                foreach (var Re in Reforma)
                                {
                                    Paragraph comb4SuperiorR = new Paragraph();
                                    Paragraph comb4InferiorR = new Paragraph();
                                    var parrafoActual = ReformaParrafo;
                                    IndReforma++;

                                    if (IndReforma == cantiReforma && organizacion.ReformaPosteriors.Any() == false)
                                    {
                                        parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + IndReforma + "° #y última");
                                    }
                                    else
                                    {
                                        parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + IndReforma + "°" + "#");
                                    }

                                    parrafoActual = parrafoActual.Replace("[TIPONORMA]", Re.TipoNormaId != null ? "#" + Re.TipoNorma.Nombre + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[NUMERONORMA]", Re.NNorma != null ? "#" + Re.NNorma + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[FECHANORMA]", Re.FechaNorma.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaNorma.Value) + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[AUTORIZADOPOR]", Re.AutorizadoPor != null ? "#" + Re.AutorizadoPor + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[FECHAPUBLICACION]", Re.FechaPublicDiario.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaPublicDiario.Value) + "#" : "ERROR_Reforma");

                                    parrafoActual = parrafoActual.Replace("[TIPOGENERAL]", Re.TipoGeneralId != null ? "#" + Re.TipoGeneral.Nombre + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[FECHAJUNTAGENERALSOCIOS]", Re.FechaJuntaGeneral != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaJuntaGeneral.Value) + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[FECHAESCRITURA]", Re.FechaEscritura.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaEscritura.Value) + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[LUGARNOTARIO]", Re.LugarNotario != null ? "#" + Re.LugarNotario + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[DATOGENERALNOTARIO]", Re.DatosNotario != null ? "#" + Re.DatosNotario + "#" : "ERROR_Reforma");
                                    parrafo4Superior = parrafoActual.Split('#');
                                    if (parrafoActual.Contains("ERROR_Reforma"))
                                    {
                                        throw new Exception(string.Format("Error al emitir, el apartado de Reforma no tiene todos los campos requeridos"));
                                    }
                                    index = 0;
                                    foreach (var item in parrafo4Superior)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseCUATROSuperior = new Phrase(item, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseCUATROSuperior = new Phrase(item, _fontNegrita);
                                        }
                                        comb4SuperiorR.Add(PhraseCUATROSuperior);
                                        index++;
                                    }

                                    comb4SuperiorR.Add(SaltoLinea);
                                    comb4SuperiorR.Add(SaltoLinea);
                                    comb4SuperiorR.Alignment = Element.ALIGN_JUSTIFIED;
                                    comb4InferiorR.Alignment = Element.ALIGN_JUSTIFIED;
                                    doc.Add(comb4SuperiorR);

                                }
                            }
                            if(organizacion.ReformaPosteriors.Any()) // estatuto anterior 2003 reforma post
                            {
                                var ReformaParrafo = configuracioncertificado.Parrafo2ExPosterior;
                                var Reforma = organizacion.ReformaPosteriors.OrderBy(q => q.FechaPubliDiario);
                                string[] parrafo4;
                                parrafo4 = ReformaParrafo.Split('#');
                                subtitulo3 = new Paragraph(parrafo4[0], _fontStandardBoldSubRayado);
                                string[] parrafo4Superior;
                                string[] parrafo4Inferior;
                                List<string> conjuntoSuperior = new List<string> { };
                                List<string> conjuntoInferior = new List<string> { };
                                var IndReforma = 0;
                                var cantiReforma = organizacion.ReformaPosteriors.Count();
                                foreach (var Re in Reforma)
                                {
                                    Paragraph comb4SuperiorR = new Paragraph();
                                    Paragraph comb4InferiorR = new Paragraph();
                                    conjuntoSuperior.Add(parrafo4[0]);
                                    var parrafoActual = conjuntoSuperior.ElementAt(IndReforma);
                                    conjuntoInferior.Add(parrafo4[1]);
                                    var parrafoActualInf = conjuntoInferior.ElementAt(IndReforma);
                                    //PARRAFO 4 Reforma - INI
                                    IndReforma++;
                                    var suma = IndReforma + organizacion.ReformaAnteriors.Count();
                                    var ObsReformaPost = organizacion.ObservacionReformas.FirstOrDefault(q => q.IdReformaPost == Re.IdReformaPost);
                                    if (IndReforma == cantiReforma)
                                    {
                                        parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + suma + "° #y última");
                                    }
                                    else
                                    {
                                        parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + suma + "°" + "#");
                                    }

                                    parrafoActual = parrafoActual.Replace("[TIPOGENERAL]", Re.TipoGeneralId != null ? "#" + Re.TipoGeneral.Nombre + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[FECHAJUNTAGENERALSOCIOS]", Re.FechaJuntGeneralSocios != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaJuntGeneralSocios.Value) + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[FECHAESCRITURA]", Re.FechaEscrituraPublica.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaEscrituraPublica.Value) + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[LUGARNOTARIO]", Re.LugarNotario != null ? "#" + Re.LugarNotario + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[DATOGENERALNOTARIO]", Re.DatosGeneralNotario != null ? "#" + Re.DatosGeneralNotario + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[FECHAPUBLICACION]", Re.FechaPubliDiario.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaPubliDiario.Value) + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[FOJASNUMERO]", Re.FojasNumero != null ? "#" + Re.FojasNumero + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[COMUNACBR]", Re.DatosCBR != null ? "#" + Re.DatosCBR + "#" : "ERROR_Reforma");
                                    parrafoActual = parrafoActual.Replace("[AÑOINSCRIPCION]", Re.AnoInscripcion != null ? "#" + Re.AnoInscripcion + "#" : "ERROR_Reforma");
                                    parrafo4Superior = parrafoActual.Split('#');
                                    if (parrafoActual.Contains("ERROR_Reforma"))
                                    {
                                        throw new Exception(string.Format("Error al emitir, el apartado de Reforma no tiene todos los campos requeridos"));
                                    }
                                    index = 0;
                                    foreach (var item in parrafo4Superior)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseCUATROSuperior = new Phrase(item, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseCUATROSuperior = new Phrase(item, _fontNegrita);
                                        }
                                        comb4SuperiorR.Add(PhraseCUATROSuperior);
                                        index++;
                                    }
                                    if (ObsReformaPost != null)
                                    {

                                        parrafoActualInf = parrafoActualInf.Replace("[NUMEROOFICIO]", ObsReformaPost.NumeroOficio != null ? "#" + ObsReformaPost.NumeroOficio + "#" : "ERROR_ObsReforma");
                                        parrafoActualInf = parrafoActualInf.Replace("[FECHAOFICIO]", ObsReformaPost.FechaOficio.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ObsReformaPost.FechaOficio.Value) + "#" : "ERROR_ObsReforma");
                                        parrafoActualInf = parrafoActualInf.Replace("[APROBACION]", ObsReformaPost.AprobacionId != null ? "#" + ObsReformaPost.Aprobacion.Nombre + "#" : "ERROR_ObsReforma");
                                        parrafo4Inferior = parrafoActualInf.Split('#');
                                        if (!parrafoActualInf.Contains("ERROR_ObsReforma"))
                                        {
                                            index = 0;
                                            foreach (var item in parrafo4Inferior)
                                            {
                                                if (index % 2 == 0)
                                                {
                                                    PhraseCUATROInferior = new Phrase(item, _fontStandard);
                                                }
                                                else
                                                {
                                                    PhraseCUATROInferior = new Phrase(item, _fontNegrita);
                                                }
                                                comb4InferiorR.Add(PhraseCUATROInferior);
                                                index++;
                                            }
                                        }

                                    }
                                    comb4SuperiorR.AddRange(comb4InferiorR);
                                    comb4SuperiorR.Add(SaltoLinea);
                                    comb4SuperiorR.Add(SaltoLinea);
                                    comb4SuperiorR.Alignment = Element.ALIGN_JUSTIFIED;
                                    comb4InferiorR.Alignment = Element.ALIGN_JUSTIFIED;
                                    doc.Add(comb4SuperiorR);
                                    //PARRAFO 4 Reforma - FIN
                                }
                            }

                            //PARRAFO 4 - FIN
                        }
                        // estatuto posterior 2003
                        else if (organizacion.ExistenciaPosteriors.FirstOrDefault(q => q.OrganizacionId == organizacion.OrganizacionId) != null)
                        {
                            // PARRAFO 1 - INI estatuto posterior 2003
                            var legalPost = organizacion.ExistenciaPosteriors.FirstOrDefault();
                            parrafo_uno = parrafo_uno.Replace("[RAZONSOCIAL]", organizacion.RazonSocial != null ? "#" + organizacion.RazonSocial + "#" : "");
                            parrafo_uno = parrafo_uno.Replace("[SIGLA]", organizacion.Sigla != null ? "la que podrá usar indistintamente para todos los efectos legales el nombre de fantasía #" + organizacion.Sigla + "#," : "");//Opcional
                            parrafo_uno = parrafo_uno.Replace("[NUMEROREGISTRO]", organizacion.NumeroRegistro != null ? "#" + organizacion.NumeroRegistro + "#" : "");

                            string[] parafo1;
                            parafo1 = parrafo_uno.Split('#');

                            foreach (string element in parafo1)
                            {
                                if (index % 2 == 0)
                                {
                                    PhraseUNO = new Phrase(element, _fontStandard);
                                }
                                else
                                {
                                    PhraseUNO = new Phrase(element, _fontNegrita);
                                }
                                comb.Add(PhraseUNO);
                                index++;
                            }
                            comb.Alignment = Element.ALIGN_JUSTIFIED;
                            doc.Add(SaltoLinea);
                            doc.Add(SaltoLinea);
                            doc.Add(SaltoLinea);
                            doc.Add(SaltoLinea);
                            doc.Add(comb);
                            doc.Add(SaltoLinea);
                            //PARRAFO 1 - FIN
                            //PARRAFO 2 - INI estatuto posterior 2003
                            var Reforma = organizacion.ReformaPosteriors.ToList().OrderByDescending(q => q.FechaPubliDiario);
                            var ReformaUlti = new ReformaPosterior();
                            foreach (var item in Reforma)
                            {
                                if((item.ObservacionReformas.Any() && 
                                    item.ObservacionReformas.FirstOrDefault().AprobacionId == (int)Infrastructure.Enum.Aprobacion.aprobada) ||
                                    !item.ObservacionReformas.Any() || item.ObservacionReformas.FirstOrDefault().AprobacionId == null
                                    )
                                {
                                    ReformaUlti = item;
                                    break;
                                }
                            }
                            if(ReformaUlti.IdReformaPost != 0) //cuenta con al menos una reforma sin obs o con obs aprobada
                            {
                                string[] parrafo2;
                                parrafo2 = parrafo_dos.Split('#');
                                subtitulo2 = new Paragraph(parrafo2[0], _fontStandardBoldSubRayado);
                                parrafo2 = parrafo2.Where(n => n != parrafo2[0]).ToArray();
                                parrafo2[0] = parrafo2[0].Replace("[TIPOGENERAL]", ReformaUlti.TipoGeneralId != null ? "#" + ReformaUlti.TipoGeneral.Nombre + "#" : "ERROR_Reforma");
                                parrafo2[0] = parrafo2[0].Replace("[EXISTEREFORMA]", "#reformado#");
                                parrafo2[0] = parrafo2[0].Replace("[FECHAJUNTAGENERALSOCIOS]", ReformaUlti.FechaJuntGeneralSocios.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaJuntGeneralSocios.Value) + "#" : "ERROR_Reforma");
                                parrafo2[0] = parrafo2[0].Replace("[FECHAESCRITURA]", ReformaUlti.FechaEscrituraPublica.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaEscrituraPublica.Value) + "#" : "ERROR_Reforma");
                                parrafo2[0] = parrafo2[0].Replace("[LUGARNOTARIO]", ReformaUlti.LugarNotario != null ? "#" + ReformaUlti.LugarNotario + "#" : "ERROR_Reforma");
                                parrafo2[0] = parrafo2[0].Replace("[DATOGENERALNOTARIO]", ReformaUlti.DatosGeneralNotario != null ? "#" + ReformaUlti.DatosGeneralNotario + "#" : "ERROR_Reforma");
                                parrafo2[0] = parrafo2[0].Replace("[FECHAPUBLICACION]", ReformaUlti.FechaPubliDiario.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaPubliDiario.Value) + "#" : "ERROR_Reforma");
                                parrafo2[0] = parrafo2[0].Replace("[FOJASNUMERO]", ReformaUlti.FojasNumero != null ? "#" + ReformaUlti.FojasNumero + "#" : "ERROR_Reforma");
                                parrafo2[0] = parrafo2[0].Replace("[COMUNACBR]", ReformaUlti.DatosCBR != null ? "#" + ReformaUlti.DatosCBR + "#" : "ERROR_Reforma");
                                parrafo2[0] = parrafo2[0].Replace("[AÑOINSCRIPCION]", ReformaUlti.AnoInscripcion != null ? "#" + ReformaUlti.AnoInscripcion + "#" : "ERROR_Reforma");
                                if (parrafo2[0].Contains("ERROR_Reforma"))
                                {
                                    throw new Exception(string.Format("Error al emitir, el apartado de Reforma no tiene todos los campos requeridos"));
                                }
                                var aprobadoObsReforma = false;
                                if (ReformaUlti.ObservacionReformas != null)
                                {
                                        parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", organizacion.ObservacionReformas.FirstOrDefault().NumeroOficio != null ? "Dicha acta estatutaria se encuentra aprobada por oficio ordinario N° #" + organizacion.ObservacionReformas.FirstOrDefault().NumeroOficio + "#" : organizacion.ObservacionReformas.FirstOrDefault().FechaOficio != null ? "" : ".");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", organizacion.ObservacionReformas.FirstOrDefault().FechaOficio != null && organizacion.ObservacionReformas.FirstOrDefault().FechaOficio != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.ObservacionReformas.FirstOrDefault().FechaOficio.Value) + "#." : "");
                                        aprobadoObsReforma = true;
                                }

                                if (aprobadoObsReforma == false)
                                {
                                    parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", "");
                                }


                                parrafo2 = parrafo2[0].Split('#');

                                index = 0;
                                foreach (string element in parrafo2)
                                {
                                    if (index % 2 == 0)
                                    {
                                        PhraseDOS = new Phrase(element, _fontStandard);
                                    }
                                    else
                                    {
                                        PhraseDOS = new Phrase(element, _fontNegrita);
                                    }
                                    comb2.Add(PhraseDOS);
                                    index++;
                                }
                            }
                            else // NO TIENE REFORMA
                            {
                                var ObsLegal = organizacion.ExistenciaPosteriors.FirstOrDefault().ObservacionLegals.FirstOrDefault();
                                var LegalPost = organizacion.ExistenciaPosteriors.FirstOrDefault();
                                string[] parrafo2;
                                parrafo2 = parrafo_dos.Split('#');
                                subtitulo2 = new Paragraph(parrafo2[0], _fontStandardBoldSubRayado);
                                parrafo2 = parrafo2.Where(n => n != parrafo2[0]).ToArray();
                                parrafo2[0] = parrafo2[0].Replace("[EXISTEREFORMA]", "#acordado#");
                                parrafo2[0] = parrafo2[0].Replace("[TIPOGENERAL]", LegalPost.TipoGeneralId != null ? "#" + LegalPost.TipoGeneral.Nombre + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[FECHAJUNTAGENERALSOCIOS]", LegalPost.FechaConstitutivaSocios.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", LegalPost.FechaConstitutivaSocios.Value) + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[FECHAESCRITURA]", LegalPost.FechaEscrituraPublica.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", LegalPost.FechaEscrituraPublica.Value) + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[LUGARNOTARIO]", LegalPost.LugarNotario != null ? "#" + LegalPost.LugarNotario + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[DATOGENERALNOTARIO]", LegalPost.DatosGeneralesNotario != null ? "#" + LegalPost.DatosGeneralesNotario + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[FECHAPUBLICACION]", LegalPost.FechaPublicacionn.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", LegalPost.FechaPublicacionn.Value) + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[FOJASNUMERO]", LegalPost.Fojas != null ? "#" + LegalPost.Fojas + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[COMUNACBR]", LegalPost.DatosCBR != null ? "#" + LegalPost.DatosCBR + "#" : "ERROR_Legal");
                                parrafo2[0] = parrafo2[0].Replace("[AÑOINSCRIPCION]", LegalPost.AnoInscripcion != null ? "#" + LegalPost.AnoInscripcion + "#" : "ERROR_Legal");
                                if (parrafo2[0].Contains("ERROR_Legal"))
                                {
                                    throw new Exception(string.Format("Error al emitir, el apartado de Legal no tiene todos los campos requeridos"));
                                }

                                if (ObsLegal != null)
                                {
                                    if (ObsLegal.AprobacionId == (int)Infrastructure.Enum.Aprobacion.aprobada)
                                    {
                                        //parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", LegalPost.TipoNormaId != null ? "Dicha acta estatutaria se encuentra aprobada por #" + LegalPost.tipoNorma.Nombre + "#" : "ERROR_Opcional");
                                        //parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", LegalPost.NNorma != null ? "#N°" + LegalPost.NNorma + "#" : "ERROR_Opcional");
                                        //parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", LegalPost.FNorma != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}#", LegalPost.FNorma) : "ERROR_Opcional");
                                        //parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", LegalPost.Autorizado != null ? "del #" + LegalPost.Autorizado + "#" : "ERROR_Opcional");
                                        parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", ObsLegal != null ? ObsLegal.NumeroOficio != null ? "por oficio ordinario N° #" + ObsLegal.NumeroOficio + "#" : "" : "");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", ObsLegal != null ? ObsLegal.FechaOficio.HasValue ? " de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}#", ObsLegal.FechaOficio.Value) + "#" : "" : "");

                                    }
                                    else
                                    {
                                        parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", "");
                                        parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", "");
                                    }

                                }
                                else
                                {
                                    //parrafo2[0] = parrafo2[0].Replace("[TIPONORMA]", "");
                                    //parrafo2[0] = parrafo2[0].Replace("[NUMERONORMA]", "");
                                    //parrafo2[0] = parrafo2[0].Replace("[FECHANORMA]", "");
                                    //parrafo2[0] = parrafo2[0].Replace("[AUTORIZADOPOR]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[NUMEROOFICIO]", "");
                                    parrafo2[0] = parrafo2[0].Replace("[FECHAOFICIO]", "");
                                }


                                parrafo2 = parrafo2[0].Split('#');

                                index = 0;
                                foreach (string element in parrafo2)
                                {
                                    if (index % 2 == 0)
                                    {
                                        PhraseDOS = new Phrase(element, _fontStandard);
                                    }
                                    else
                                    {
                                        PhraseDOS = new Phrase(element, _fontNegrita);
                                    }
                                    comb2.Add(PhraseDOS);
                                    index++;
                                }
                            }

                            doc.Add(subtitulo2);
                            doc.Add(SaltoLinea);
                            doc.Add(comb2);
                            doc.Add(SaltoLinea);
                            //PARRAFO 2 - FIN
                            //PARRAFO 3 - INI estatuto posterior 2003
                            if (organizacion.Saneamientos.Count() == 0 && organizacion.ReformaPosteriors.Count() == 0)
                            {
                                PhraseTRES = new Phrase("No se registran actas complementarias, rectificatorias o modificatorias", _fontStandard);
                                comb3.Add(PhraseTRES);
                                doc.Add(comb3);
                                doc.Add(SaltoLinea);
                            }
                            if (organizacion.Saneamientos.Any() || organizacion.ReformaPosteriors.Any())
                            {
                                var saneamientoParrafo = configuracioncertificado.Parrafo1DisPos;
                                var saneamientoAnt = organizacion.Saneamientos.FirstOrDefault();
                                string[] parrafo3;
                                parrafo3 = saneamientoParrafo.Split('#');
                                subtitulo3 = new Paragraph(parrafo3[0], _fontStandardBoldSubRayado);
                                parrafo3 = parrafo3.Where(n => n != parrafo3[0]).ToArray();
                                if (organizacion.Saneamientos.Any())
                                {
                                    parrafo3[0] = parrafo3[0].Replace("[EXISTEREFORMA]", organizacion.ReformaPosteriors.Any() ? "El acta contitutiva" : "La reforma estatutaria");
                                    parrafo3[0] = parrafo3[0].Replace("[FECHAESCRITURA]", saneamientoAnt.FechaEscrituraPublicaa.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", saneamientoAnt.FechaEscrituraPublicaa.Value) + "#" : "ERROR_Saneamiento");
                                    parrafo3[0] = parrafo3[0].Replace("[LUGARNOTARIO]", saneamientoAnt.LugarNotario != null ? "#" + saneamientoAnt.LugarNotario + "#" : "ERROR_Saneamiento");
                                    parrafo3[0] = parrafo3[0].Replace("[DATOGENERALNOTARIO]", saneamientoAnt.DatoGeneralesNotario != null ? "#" + saneamientoAnt.DatoGeneralesNotario + "#" : "ERROR_Saneamiento");
                                    parrafo3[0] = parrafo3[0].Replace("[FECHAPUBLICACION]", saneamientoAnt.FechaaPublicacionDiario.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", saneamientoAnt.FechaaPublicacionDiario.Value) + "#" : "ERROR_Saneamiento");
                                    parrafo3[0] = parrafo3[0].Replace("[FOJASNUMERO]", saneamientoAnt.Fojass != null ? "#" + saneamientoAnt.Fojass + "#" : "ERROR_Saneamiento");
                                    parrafo3[0] = parrafo3[0].Replace("[COMUNACBR]", saneamientoAnt.DatossCBR != null ? "#" + saneamientoAnt.DatossCBR + "#" : "ERROR_Saneamiento");
                                    parrafo3[0] = parrafo3[0].Replace("[AÑOINSCRIPCION]", saneamientoAnt.FechaaInscripcion != null ? "#" + saneamientoAnt.FechaaInscripcion + "#" : "ERROR_Saneamiento");
                                    if (parrafo3[0].Contains("ERROR_Saneamiento"))
                                    {
                                        throw new Exception(string.Format("Error al emitir, el apartado de Saneamiento no tiene todos los campos requeridos"));
                                    }
                                    parrafo3 = parrafo3[0].Split('#');
                                    index = 0;
                                    foreach (var item in parrafo3)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseTRES = new Phrase(item, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseTRES = new Phrase(item, _fontNegrita);
                                        }
                                        comb3.Add(PhraseTRES);
                                        index++;
                                    }
                                }
                                comb3.Alignment = Element.ALIGN_JUSTIFIED;
                                doc.Add(subtitulo3);
                                doc.Add(SaltoLinea);
                                doc.Add(comb3);
                                doc.Add(SaltoLinea);
                            }
                            //PARRAFO 3 - FIN
                            //PARRAFO 4 - INI estatuto posterior 2003

                            if (organizacion.ReformaPosteriors.Any())
                            {
                                var ReformaParrafo = configuracioncertificado.Parrafo2ExPosterior;
                                var ReformaPost = organizacion.ReformaPosteriors.OrderBy(q => q.FechaPubliDiario);
                                    string[] parrafo4;
                                    parrafo4 = ReformaParrafo.Split('#');
                                    subtitulo3 = new Paragraph(parrafo4[0], _fontStandardBoldSubRayado);
                                    string[] parrafo4Superior;
                                    string[] parrafo4Inferior;
                                    List<string> conjuntoSuperior = new List<string> { };
                                    List<string> conjuntoInferior = new List<string> { };
                                    var IndReforma = 0;
                                    var cantiReforma = organizacion.ReformaPosteriors.Count();
                                    foreach (var Re in ReformaPost)
                                    {
                                        Paragraph comb4SuperiorR = new Paragraph();
                                        Paragraph comb4InferiorR = new Paragraph();
                                        conjuntoSuperior.Add(parrafo4[0]);
                                        var parrafoActual = conjuntoSuperior.ElementAt(IndReforma);
                                        conjuntoInferior.Add(parrafo4[1]);
                                        var parrafoActualInf = conjuntoInferior.ElementAt(IndReforma);
                                        IndReforma++;
                                        var ObsReformaPost = organizacion.ReformaPosteriors.FirstOrDefault().ObservacionReformas.FirstOrDefault(q => q.IdReformaPost == Re.IdReformaPost);
                                        if (IndReforma == cantiReforma)
                                        {
                                            parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + IndReforma + "° #y última");
                                        }
                                        else
                                        {
                                            parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + IndReforma + "°" + "#");
                                        }

                                        parrafoActual = parrafoActual.Replace("[TIPOGENERAL]", Re.TipoGeneralId != null ? "#" + Re.TipoGeneral.Nombre + "#" : "ERROR_Reforma");
                                        parrafoActual = parrafoActual.Replace("[FECHAJUNTAGENERALSOCIOS]", Re.FechaJuntGeneralSocios != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaJuntGeneralSocios.Value) + "#" : "ERROR_Reforma");
                                        parrafoActual = parrafoActual.Replace("[FECHAESCRITURA]", Re.FechaEscrituraPublica.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaEscrituraPublica.Value) + "#" : "ERROR_Reforma");
                                        parrafoActual = parrafoActual.Replace("[LUGARNOTARIO]", Re.LugarNotario != null ? "#" + Re.LugarNotario + "#" : "ERROR_Reforma");
                                        parrafoActual = parrafoActual.Replace("[DATOGENERALNOTARIO]", Re.DatosGeneralNotario != null ? "#" + Re.DatosGeneralNotario + "#" : "ERROR_Reforma");
                                        parrafoActual = parrafoActual.Replace("[FECHAPUBLICACION]", Re.FechaPubliDiario.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaPubliDiario.Value) + "#" : "ERROR_Reforma");
                                        parrafoActual = parrafoActual.Replace("[FOJASNUMERO]", Re.FojasNumero != null ? "#" + Re.FojasNumero + "#" : "ERROR_Reforma");
                                        parrafoActual = parrafoActual.Replace("[COMUNACBR]", Re.DatosCBR != null ? "#" + Re.DatosCBR + "#" : "ERROR_Reforma");
                                        parrafoActual = parrafoActual.Replace("[AÑOINSCRIPCION]", Re.AnoInscripcion != null ? "#" + Re.AnoInscripcion + "#" : "ERROR_Reforma");
                                        parrafo4Superior = parrafoActual.Split('#');
                                        if (parrafoActual.Contains("ERROR_Reforma"))
                                        {
                                            throw new Exception(string.Format("Error al emitir, el apartado de Reforma no tiene todos los campos requeridos"));
                                        }
                                        index = 0;
                                        foreach (var item in parrafo4Superior)
                                        {
                                            if (index % 2 == 0)
                                            {
                                                PhraseCUATROSuperior = new Phrase(item, _fontStandard);
                                            }
                                            else
                                            {
                                                PhraseCUATROSuperior = new Phrase(item, _fontNegrita);
                                            }
                                            comb4SuperiorR.Add(PhraseCUATROSuperior);
                                            index++;
                                        }
                                        if (ObsReformaPost != null)
                                        {

                                            parrafoActualInf = parrafoActualInf.Replace("[NUMEROOFICIO]", ObsReformaPost.NumeroOficio != null ? "#" + ObsReformaPost.NumeroOficio + "#" : "ERROR_ObsReforma");
                                            parrafoActualInf = parrafoActualInf.Replace("[FECHAOFICIO]", ObsReformaPost.FechaOficio.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ObsReformaPost.FechaOficio.Value) + "#" : "ERROR_ObsReforma");
                                            parrafoActualInf = parrafoActualInf.Replace("[APROBACION]", ObsReformaPost.AprobacionId != null ? "#" + ObsReformaPost.Aprobacion.Nombre + "#" : "ERROR_ObsReforma");
                                            parrafo4Inferior = parrafoActualInf.Split('#');
                                            if (!parrafo_cinco.Contains("ERROR_ObsReforma"))
                                            {
                                            index = 0;
                                            foreach (var item in parrafo4Inferior)
                                            {
                                                if (index % 2 == 0)
                                                {
                                                    PhraseCUATROInferior = new Phrase(item, _fontStandard);
                                                }
                                                else
                                                {
                                                    PhraseCUATROInferior = new Phrase(item, _fontNegrita);
                                                }
                                                comb4InferiorR.Add(PhraseCUATROInferior);
                                                index++;
                                            }
                                        }

                                        }
                                        comb4SuperiorR.AddRange(comb4InferiorR);
                                        comb4SuperiorR.Add(SaltoLinea);
                                        comb4SuperiorR.Add(SaltoLinea);
                                        comb4SuperiorR.Alignment = Element.ALIGN_JUSTIFIED;
                                        comb4InferiorR.Alignment = Element.ALIGN_JUSTIFIED;
                                        doc.Add(comb4SuperiorR);
                                        //PARRAFO 4 Reforma - FIN
                                    }
                                
                            }
                            //PARRAFO 4 - FIN
                        }
                        if (organizacion.ExistenciaAnteriors == null && organizacion.ExistenciaPosteriors == null)
                        {
                            throw new Exception(string.Format("Error al emitir, el apartado de legal no tiene todos los campos requeridos"));
                        }
                    }
                    else //estatuto gremial
                    {
                        // PARRAFO 1 - INI estatuto gremial
                        parrafo_uno = parrafo_uno.Replace("[RAZONSOCIAL]", organizacion.RazonSocial != null ? "#" + organizacion.RazonSocial.ToString() + "#" : "[RAZONSOCIAL]");
                        parrafo_uno = parrafo_uno.Replace("[SIGLA]", organizacion.Sigla != null ? "la que podrá usar indistintamente para todos los efectos legales el nombre de fantasía #" + organizacion.Sigla + "#," : "");
                        parrafo_uno = parrafo_uno.Replace("[NUMEROREGISTRO]", organizacion.NumeroRegistro != null ? "#" + organizacion.NumeroRegistro.ToString() + "#" : "[NUMEROREGISTRO]");
                        string[] parafo1;
                        parafo1 = parrafo_uno.Split('#');


                        foreach (string element in parafo1)
                        {
                            if (index % 2 == 0)
                            {
                                PhraseUNO = new Phrase(element, _fontStandard);
                            }
                            else
                            {
                                PhraseUNO = new Phrase(element, _fontNegrita);
                            }
                            comb.Add(PhraseUNO);
                            index++;
                        }
                        comb.Alignment = Element.ALIGN_JUSTIFIED;
                        doc.Add(SaltoLinea);
                        doc.Add(SaltoLinea);
                        doc.Add(SaltoLinea);
                        doc.Add(SaltoLinea);
                        doc.Add(comb);
                        doc.Add(SaltoLinea);
                        //PARRAFO 1 - FIN

                        //PARRAFO 2 - INI estatuto gremial
                        var legalGremial = organizacion.ExistenciaLegals.FirstOrDefault();
                        parrafo_dos = parrafo_dos.Replace("[APROBACION]", legalGremial.AprobacionId != null ? "#" + legalGremial.Aprobacion.Nombre + "#" : "ERROR_Legal");
                        parrafo_dos = parrafo_dos.Replace("[FECHAASAMBLEASOCIOS]", legalGremial.FechaConstitutivaSocios.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalGremial.FechaConstitutivaSocios.Value) + "#" : "ERROR_Legal");
                        parrafo_dos = parrafo_dos.Replace("[NUMEROOFICIO]", legalGremial.ObservacionLegals.FirstOrDefault().NumeroOficio != null ? "Dicha acta estatutaria se encuentra aprobada por oficio ordinario N° #" + legalGremial.ObservacionLegals.FirstOrDefault().NumeroOficio + "#" : organizacion.ObservacionLegals.FirstOrDefault().FechaOficio != null ? "" : ".");
                        parrafo_dos = parrafo_dos.Replace("[FECHAOFICIO]", legalGremial.ObservacionLegals.FirstOrDefault().FechaOficio != null && legalGremial.ObservacionLegals.FirstOrDefault().FechaOficio != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalGremial.ObservacionLegals.FirstOrDefault().FechaOficio.Value) + "#." : "");
                        if (parrafo_dos.Contains("ERROR_Legal"))
                        {
                            throw new Exception(string.Format("Error al emitir, el apartado legal no tiene todos los campos requeridos"));
                        }
                        string[] parafo2;
                        parafo2 = parrafo_dos.Split('#');
                        index = 0;

                        foreach (var item in parafo2)
                        {
                            if (index % 2 == 0)
                            {
                                PhraseDOS = new Phrase(item, _fontStandard);
                            }
                            else
                            {
                                PhraseDOS = new Phrase(item, _fontNegrita);
                            }
                            comb2.Add(PhraseDOS);
                            index++;
                        }

                        comb2.Alignment = Element.ALIGN_JUSTIFIED;
                        doc.Add(comb2);
                        doc.Add(SaltoLinea);
                        //PARRAFO 2 - FIN

                        //PARRAFO 3 - INI estatuto gremial
                        var Reforma = organizacion.ReformaAGACs.ToList().OrderByDescending(q => q.FechaAsambleaDep);
                        var ReformaUlti = new ReformaAGAC();
                        foreach (var item in Reforma)
                        {
                            if ((item.ObservacionReformas.Any() &&
                                item.ObservacionReformas.FirstOrDefault().AprobacionId == (int)Infrastructure.Enum.Aprobacion.aprobada) ||
                                !item.ObservacionReformas.Any() || item.ObservacionReformas.FirstOrDefault().AprobacionId == null
                                )
                            {
                                ReformaUlti = item;
                                break;
                            }
                        }
                        if(ReformaUlti.IdReformaAGAC != 0)
                        {
                            var obsLegalGremial = ReformaUlti.ObservacionReformas.FirstOrDefault();
                            string[] parrafo3;
                            parrafo3 = parrafo_tres.Split('#');
                            subtitulo2 = new Paragraph(parrafo3[0], _fontStandardBoldSubRayado);
                            parrafo3 = parrafo3.Where(n => n != parrafo3[0]).ToArray();
                            parrafo3[0] = parrafo3[0].Replace("[TIPOGENERAL]", ReformaUlti.TipoGeneralId != null ? "#" + ReformaUlti.TipoGeneral.Nombre + "#" : "ERROR_Reforma");
                            parrafo3[0] = parrafo3[0].Replace("[EXISTEREFORMA]", "#reformado#");
                            parrafo3[0] = parrafo3[0].Replace("[FECHAASAMBLEASOCIOS]", ReformaUlti.FechaAsambleaDep.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ReformaUlti.FechaAsambleaDep.Value) + "#" : "ERROR_Reforma");
                            parrafo3[0] = parrafo3[0].Replace("[ESTADO]", ReformaUlti.AprobacionId != null ? "#" + ReformaUlti.Aprobacion.Nombre + "#" : "ERROR_Reforma");
                            if(ReformaUlti.ObservacionReformas.FirstOrDefault().AprobacionId == (int)Infrastructure.Enum.Aprobacion.aprobada)
                            {
                                parrafo3[0] = parrafo3[0].Replace("[NUMEROOFICIO]", obsLegalGremial.NumeroOficio != null ? "Dicha acta estatutaria se encuentra aprobada por oficio ordinario N° #" + obsLegalGremial.NumeroOficio + "#" : obsLegalGremial.FechaOficio != null ? "" : ".");
                                parrafo3[0] = parrafo3[0].Replace("[FECHAOFICIO]", obsLegalGremial.FechaOficio != null && obsLegalGremial.FechaOficio != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", obsLegalGremial.FechaOficio.Value) + "#." : "");
                            }
                            else
                            {
                                parrafo3[0] = parrafo3[0].Replace("[NUMEROOFICIO]", "");
                                parrafo3[0] = parrafo3[0].Replace("[FECHAOFICIO]", "");
                            }

                            parrafo3 = parrafo3[0].Split('#');
                            if (parrafo3[0].Contains("ERROR_Reforma"))
                            {
                                throw new Exception(string.Format("Error al emitir, el apartado de Reforma no tiene todos los campos requeridos"));
                            }
                            index = 0;
                            foreach (string element in parrafo3)
                            {
                                if (index % 2 == 0)
                                {
                                    PhraseTRES = new Phrase(element, _fontStandard);
                                }
                                else
                                {
                                    PhraseTRES = new Phrase(element, _fontNegrita);
                                }
                                comb3.Add(PhraseTRES);
                                index++;
                            }
                        }
                        else // NO TIENE REFORMA estatuto gremial
                        {
                            string[] parrafo3;
                            parrafo3 = parrafo_tres.Split('#');
                            subtitulo2 = new Paragraph(parrafo3[0], _fontStandardBoldSubRayado);
                            parrafo3 = parrafo3.Where(n => n != parrafo3[0]).ToArray();
                            parrafo3[0] = parrafo3[0].Replace("[EXISTEREFORMA]", "#acordado#");
                            parrafo3[0] = parrafo3[0].Replace("[FECHAASAMBLEASOCIOS]", legalGremial.FechaConstitutivaSocios.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", legalGremial.FechaConstitutivaSocios.Value) + "#" : "ERROR_Legal");
                            parrafo3[0] = parrafo3[0].Replace("[NUMEROOFICIO]", legalGremial.NumeroOficio != null ? "#" + legalGremial.NumeroOficio + "#" : "ERROR_Legal");
                            parrafo3[0] = parrafo3[0].Replace("[FECHAOFICIO]", legalGremial.FechaOficio.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}#", legalGremial.FechaOficio.Value) + "#" : "ERROR_Legal");
                            parrafo3 = parrafo3[0].Split('#');
                            if (parrafo3[0].Contains("ERROR_Legal"))
                            {
                                throw new Exception(string.Format("Error al emitir, el apartado de Legal no tiene todos los campos requeridos"));
                            }
                            index = 0;
                            foreach (string element in parrafo3)
                            {
                                if (index % 2 == 0)
                                {
                                    PhraseTRES = new Phrase(element, _fontStandard);
                                }
                                else
                                {
                                    PhraseTRES = new Phrase(element, _fontNegrita);
                                }
                                comb3.Add(PhraseTRES);
                                index++;
                            }
                        }
                        comb3.Alignment = Element.ALIGN_JUSTIFIED;
                        if (subtitulo2 != null)
                        {
                            doc.Add(subtitulo2);
                            doc.Add(SaltoLinea);
                            doc.Add(comb3);
                            doc.Add(SaltoLinea);
                        }
                        //PARRAFO 3 - FIN
                        //PARRAFO 4 - INI estatuto gremial

                        if (organizacion.ReformaAGACs.Any())
                        {
                            var ReformaGremial = organizacion.ReformaAGACs.OrderBy(q => q.FechaAsambleaDep);
                            string[] parrafo4;
                            parrafo4 = parrafo_cuatro.Split('#');
                            subtitulo3 = new Paragraph(parrafo4[0], _fontStandardBoldSubRayado);
                            parrafo4 = parrafo4.Where(n => n != parrafo4[0]).ToArray();
                            string[] parrafo4Superior;
                            string[] parrafo4Inferior;
                            List<string> conjuntoSuperior = new List<string> { };
                            List<string> conjuntoInferior = new List<string> { };
                            doc.Add(subtitulo3);
                            doc.Add(SaltoLinea);
                            var IndReforma = 0;
                            var cantiReforma = organizacion.ReformaAGACs.Count();
                            foreach (var Re in ReformaGremial)
                            {
                                Paragraph comb4SuperiorR = new Paragraph();
                                Paragraph comb4InferiorR = new Paragraph();
                                conjuntoSuperior.Add(parrafo4[0]);
                                var parrafoActual = conjuntoSuperior.ElementAt(IndReforma);
                                conjuntoInferior.Add(parrafo4[1]);
                                var parrafoActualInf = conjuntoInferior.ElementAt(IndReforma);
                                //PARRAFO 4 Reforma - INI
                                IndReforma++;
                                var ObsReforma = organizacion.ObservacionReformas.FirstOrDefault(q => q.IdReformaAGAC == Re.IdReformaAGAC);
                                if (IndReforma == cantiReforma)
                                {
                                    parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + IndReforma + "° #y última");
                                }
                                else
                                {
                                    parrafoActual = parrafoActual.Replace("[REFORMA]", "#" + IndReforma + "°" + "#");
                                }
                                parrafoActual = parrafoActual.Replace("[TIPOGENERAL]", Re.TipoGeneralId != null ? "#" + Re.TipoGeneral.Nombre + "#" : "ERROR_Reforma");
                                parrafoActual = parrafoActual.Replace("[FECHAASAMBLEASOCIOS]", Re.FechaAsambleaDep.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaAsambleaDep.Value) + "#" : "ERROR_Reforma");
                                parrafoActual = parrafoActual.Replace("[NUMEROOFICIO]", Re.NumeroOficio != null ? "#" + Re.NumeroOficio + "#" : "ERROR_Reforma");
                                parrafoActual = parrafoActual.Replace("[FECHAOFICIO]", Re.FechaOficio.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", Re.FechaOficio.Value) + "#" : "ERROR_Reforma");
                                parrafoActual = parrafoActual.Replace("[APROBACION]", Re.AprobacionId != null ? "#" + Re.Aprobacion.Nombre + "#" : "ERROR_Reforma");
                                parrafo4Superior = parrafoActual.Split('#');
                                if (parrafoActual.Contains("ERROR_Reforma"))
                                {
                                    throw new Exception(string.Format("Error al emitir, el apartado de Reforma no tiene todos los campos requeridos"));
                                }
                                index = 0;
                                foreach (var item in parrafo4Superior)
                                {
                                    if (index % 2 == 0)
                                    {
                                        PhraseCUATROSuperior = new Phrase(item, _fontStandard);
                                    }
                                    else
                                    {
                                        PhraseCUATROSuperior = new Phrase(item, _fontNegrita);
                                    }
                                    comb4SuperiorR.Add(PhraseCUATROSuperior);
                                    index++;
                                }
                                if (ObsReforma != null)
                                {

                                    parrafoActualInf = parrafoActualInf.Replace("[FECHADEPOSITO]", ObsReforma.FechaDeposito.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ObsReforma.FechaDeposito.Value) + "#" : "ERROR_ObsReforma");
                                    parrafoActualInf = parrafoActualInf.Replace("[NUMEROOFICIO]", ObsReforma.NumeroOficio != null ? "#" + ObsReforma.NumeroOficio + "#" : "ERROR_ObsReforma");
                                    parrafoActualInf = parrafoActualInf.Replace("[FECHAOFICIO]", ObsReforma.FechaOficio.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ObsReforma.FechaOficio.Value) + "#" : "ERROR_ObsReforma");
                                    parrafoActualInf = parrafoActualInf.Replace("[APROBACION]", ObsReforma.AprobacionId != null ? "#" + ObsReforma.Aprobacion.Nombre + "#" : "ERROR_ObsReforma");
                                    parrafo4Inferior = parrafoActualInf.Split('#');
                                    if (!parrafoActualInf.Contains("ERROR_ObsReforma"))
                                    {
                                        index = 0;
                                        foreach (var item in parrafo4Inferior)
                                        {
                                            if (index % 2 == 0)
                                            {
                                                PhraseCUATROInferior = new Phrase(item, _fontStandard);
                                            }
                                            else
                                            {
                                                PhraseCUATROInferior = new Phrase(item, _fontNegrita);
                                            }
                                            comb4InferiorR.Add(PhraseCUATROInferior);
                                            index++;
                                        }
                                    }

                                }
                                comb4SuperiorR.AddRange(comb4InferiorR);
                                comb4SuperiorR.Add(SaltoLinea);
                                comb4SuperiorR.Add(SaltoLinea);
                                comb4SuperiorR.Alignment = Element.ALIGN_JUSTIFIED;
                                comb4InferiorR.Alignment = Element.ALIGN_JUSTIFIED;
                                doc.Add(comb4SuperiorR);
                                //PARRAFO 4 Reforma - FIN
                                //PARRAFO 5 ObsReforma - INI
                                /*
                                if (ObsReforma != null)
                                {
                                    parrafo_cinco = parrafo_cinco.Replace("[FECHAJUNTAGENERALSOCIOS]", ObsReforma.FechaDeposito.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ObsReforma.FechaDeposito.Value) + "#" : "ERROR_ObsReforma");
                                    parrafo_cinco = parrafo_cinco.Replace("[NUMEROOFICIO]", ObsReforma.NumeroOficio != null ? "#" + ObsReforma.NumeroOficio + "#" : "ERROR_ObsReforma");
                                    parrafo_cinco = parrafo_cinco.Replace("[FECHAOFICIO]", ObsReforma.FechaOficio.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", ObsReforma.FechaOficio.Value) + "#" : "ERROR_ObsReforma");
                                    parrafo_cinco = parrafo_cinco.Replace("[APROBACION]", ObsReforma.AprobacionId != null ? "#" + ObsReforma.Aprobacion.Nombre + "#" : "ERROR_ObsReforma");
                                    parrafo5 = parrafo_cinco.Split('#');
                                    if (parrafo_cinco.Contains("ERROR_ObsReforma"))
                                    {
                                        throw new Exception(string.Format("Error al emitir, el apartado de Observacion Reforma no tiene todos los campos requeridos"));
                                    }
                                    index = 0;
                                    foreach (var item in parrafo5)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseCINCO = new Phrase(item, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseCINCO = new Phrase(item, _fontNegrita);
                                        }
                                        comb2.Add(PhraseCINCO);
                                        index++;
                                    }
                                }*/
                                //PARRAFO 5 ObsReforma - FIN

                            }
                        }
                        //PARRAFO 4 - FIN
                    }

                 

                }









                //CertificadoDisolucionTest = 106 | CertificadoDisolucion = 4

                if (TipoDocumentoId == (int)Infrastructure.Enum.TipoDocumento.CertificadoDisolución)
                {
                    var aux = context.Disolucions.FirstOrDefault(q => q.OrganizacionId == organizacion.OrganizacionId);
                    if (aux != null)
                    {
                        string parrafo = aux.Anterior == true ? configuracioncertificado.Parrafo1DisAnt : configuracioncertificado.Parrafo1DisPos;
                        string parrafo2 = aux.Anterior == true ? configuracioncertificado.Parrafo2ExAnterior : configuracioncertificado.Parrafo2ExPosterior;
                        parrafo = aux.Anterior == null ? configuracioncertificado.Parrafo1 : parrafo;
                        parrafo2 = aux.Anterior == null ? configuracioncertificado.Parrafo2 : parrafo2;
                        Phrase PhraseDiso;
                        Phrase PhraseDiso2;
                        Paragraph combDiso = new Paragraph();
                        Paragraph combDiso2 = new Paragraph();

                        if (aux.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.Cooperativa)
                        {
                            if (aux.Anterior == true)
                            {

                                    parrafo = parrafo.Replace("[TIPONORMA]", aux.TipoNorma.Nombre != null ? "#" + aux.TipoNorma.Nombre.ToString() + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[NUMERONORMA]", aux.NumeroNorma != null ? "#" + aux.NumeroNorma.ToString() + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[FECHANORMA]", aux.FechaNorma != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaNorma.Value) + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[AUTORIZADOPOR]", aux.Autorizacion != null ? "#" + aux.Autorizacion.ToString() + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[RAZONSOCIAL]", organizacion.RazonSocial != null ? "#" + organizacion.RazonSocial.ToString() + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[FECHAPUBLICACION]", aux.FechaPubliccionDiarioOficial != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaPubliccionDiarioOficial.Value) + "#" : "[ERROR_Disolucion]");

                                string[] diso = parrafo.Split('#');

                                    parrafo2 = parrafo2.Replace("[FECHACELEBRACION]", aux.FechaComiLiquiJuntaSocios != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaComiLiquiJuntaSocios.Value) + "#," : "");

                                    string[] diso2 = parrafo2.Split('#');
                                    if (parrafo.Contains("ERROR_Disolucion") || parrafo2.Contains("ERROR_Disolucion"))
                                    {
                                        throw new Exception(string.Format("Error al emitir, el apartado de Disolucion no tiene todos los campos requeridos"));
                                    }

                                    foreach (string element in diso)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseDiso = new Phrase(element, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseDiso = new Phrase(element, _fontNegrita);
                                        }
                                        combDiso.Add(PhraseDiso);
                                        index++;
                                    }
                                    index = 0;
                                    foreach (string element in diso2)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseDiso2 = new Phrase(element, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseDiso2 = new Phrase(element, _fontNegrita);
                                        }
                                        combDiso2.Add(PhraseDiso2);
                                        index++;
                                    }
                                    combDiso.Alignment = Element.ALIGN_JUSTIFIED;
                                    combDiso2.Alignment = Element.ALIGN_JUSTIFIED;


                            }
                            else
                            {
                                    parrafo = parrafo.Replace("[RAZONSOCIAL]", organizacion.RazonSocial != null ? "#" + organizacion.RazonSocial.ToString() + "#" : "[RAZONSOCIAL]");
                                    parrafo = parrafo.Replace("[FECHAJUNTAGENERALSOCIOS]", aux.FechaJuntaSocios != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaJuntaSocios.Value) + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[FECHAESCRITURA]", aux.FechaEscrituraPublica != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaEscrituraPublica.Value) + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[LUGARNOTARIO]", aux.NombreNotaria != null ? "#" + aux.NombreNotaria + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[DATOGENERALNOTARIO]", aux.DatosNotario != null ? "#" + aux.DatosNotario + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[FECHAPUBLICACION]", aux.FechaPubliccionDiarioOficial != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaPubliccionDiarioOficial.Value) + "#" : "[ERROR_Disolucio]");
                                    parrafo = parrafo.Replace("[FOJASNUMERO]", aux.NumeroFojas != null ? "#" + aux.NumeroFojas + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[COMUNACBR]", aux.DatosCBR != null ? "#" + aux.DatosCBR + "#" : "[ERROR_Disolucion]");
                                    parrafo = parrafo.Replace("[AÑOINSCRIPCION]", aux.AñoInscripcion != null ? "#" + aux.AñoInscripcion + "#" : "[ERROR_Disolucion]");

                                string[] diso = parrafo.Split('#');

                                    parrafo2 = parrafo2.Replace("[FECHACELEBRACION]", aux.FechaComiLiquiJuntaSocios != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaComiLiquiJuntaSocios.Value) + "#," : "");

                                string[] diso2 = parrafo2.Split('#');
                                    if (parrafo.Contains("ERROR_Disolucion") || parrafo2.Contains("ERROR_Disolucion"))
                                    {
                                        throw new Exception(string.Format("Error al emitir, el apartado de Disolucion no tiene todos los campos requeridos"));
                                    }
                                    foreach (string element in diso)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseDiso = new Phrase(element, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseDiso = new Phrase(element, _fontNegrita);
                                        }
                                        combDiso.Add(PhraseDiso);
                                        index++;
                                    }
                                    index = 0;
                                    foreach (string element in diso2)
                                    {
                                        if (index % 2 == 0)
                                        {
                                            PhraseDiso2 = new Phrase(element, _fontStandard);
                                        }
                                        else
                                        {
                                            PhraseDiso2 = new Phrase(element, _fontNegrita);
                                        }
                                        combDiso2.Add(PhraseDiso2);
                                        index++;
                                    }
                                    combDiso.Alignment = Element.ALIGN_JUSTIFIED;
                                    combDiso2.Alignment = Element.ALIGN_JUSTIFIED;


                            }
                       }
                        else if (aux.TipoOrganizacionId == (int)DAES.Infrastructure.Enum.TipoOrganizacion.AsociacionConsumidores ||
                                 aux.TipoOrganizacionId == (int)DAES.Infrastructure.Enum.TipoOrganizacion.AsociacionGremial)
                        {
                            parrafo = parrafo.Replace("[RAZONSOCIAL]", organizacion.RazonSocial != null ? "#" + organizacion.RazonSocial.ToString() + "#" : "[ERROR_Disolucion]");
                            parrafo = parrafo.Replace("[FECHAASAMBLEASOCIOS]", aux.FechaAsambleaSocios != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaJuntaSocios.Value) + "#" : "[ERROR_Disolucion]");
                                parrafo = parrafo.Replace("[FECHAESCRITURA]", aux.FechaEscrituraPublica != null ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaEscrituraPublica.Value) + "#" : "[ERROR_Disolucion]");
                                parrafo = parrafo.Replace("[LUGARNOTARIO]", aux.NombreNotaria != null ? "#" + aux.NombreNotaria + "#" : "[ERROR_Disolucion]");
                                parrafo = parrafo.Replace("[DATOGENERALNOTARIO]", aux.DatosNotario != null ? "#" + aux.DatosNotario + "#" : "[ERROR_Disolucion]");
                                parrafo = parrafo.Replace("[FECHAPUBLICACION]", aux.FechaPubliccionDiarioOficial.HasValue ? "#" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaPubliccionDiarioOficial.Value) + "#" : "[ERROR_Disolucion]");
                                parrafo = parrafo.Replace("[NUMEROOFICIO]", aux.NumeroOficio != null ? "Dicha disolución se encuentra aprobada por oficio ordinario N° #" + aux.NumeroOficio + "#" : aux.FechaOficio != null ? "" : ".");
                                parrafo = parrafo.Replace("[FECHAOFICIO]", aux.NumeroOficio != null && aux.FechaOficio != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", aux.FechaOficio.Value) + "#." : "");
                            if (parrafo.Contains("ERROR_Disolucion"))
                                {
                                    throw new Exception(string.Format("Error al emitir, el apartado de Disolucion no tiene todos los campos requeridos"));
                                }

                                string[] diso = parrafo.Split('#');

                                foreach (string element in diso)
                                {
                                    if (index % 2 == 0)
                                    {
                                        PhraseDiso = new Phrase(element, _fontStandard);
                                    }
                                    else
                                    {
                                        PhraseDiso = new Phrase(element, _fontNegrita);
                                    }
                                    combDiso.Add(PhraseDiso);
                                    index++;
                                }
                                combDiso.Alignment = Element.ALIGN_JUSTIFIED;




                        }

                        doc.Add(SaltoLinea);
                        doc.Add(SaltoLinea);
                        doc.Add(SaltoLinea);
                        doc.Add(SaltoLinea);
                        doc.Add(combDiso);
                        doc.Add(SaltoLinea);
                        doc.Add(combDiso2);
                        doc.Add(SaltoLinea);
                        if (aux.ComisionAnterior = true || aux.ComisionPosterior == true)
                        {
                            var comision = context.ComisionLiquidadora.Where(q => q.OrganizacionId == organizacion.OrganizacionId).OrderBy(q => q.Cargo.Secuencia);
                            if (comision.Any())
                            {
                                bool filaCompleta = false;
                                PdfPTable table = new PdfPTable(4);
                                table.WidthPercentage = 100.0f;
                                table.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.DefaultCell.BorderColor = BaseColor.LIGHT_GRAY;
                                    table.AddCell(new PdfPCell(new Phrase("Nombre", _fontStandardBold)));
                                table.AddCell(new PdfPCell(new Phrase("Cargo", _fontStandardBold)));
                                PdfPCell cellDesde = new PdfPCell(new Phrase("Desde", _fontStandardBold));
                                cellDesde.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.AddCell(cellDesde);
                                PdfPCell cellHasta = new PdfPCell(new Phrase("Hasta", _fontStandardBold));
                                cellHasta.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.AddCell(cellHasta);
                                table.SetWidths(new float[] { 4f, 6f, 3f, 3f });
                                foreach (var item in comision.ToList())
                                {
                                    if (item.EsMiembro)
                                    {
                                        var cargo = context.Cargo.FirstOrDefault(q => q.CargoId == item.CargoId);
                                        if (item.NombreCompleto != null && cargo.Nombre != null && aux.FechaInicio.HasValue == true && aux.FechaTermino.HasValue == true)
                                        {
                                            filaCompleta = true;
                                            break;
                                        }
                                    }
                                }
                                if (filaCompleta == true)
                                {
                                    foreach (var item in comision.ToList())
                                    {
                                        if (item.EsMiembro)
                                        {
                                            var cargo = context.Cargo.FirstOrDefault(q => q.CargoId == item.CargoId);
                                            if (cargo != null)
                                                table.AddCell(new PdfPCell(new Phrase(item.NombreCompleto, _fontStandard)));

                                            table.AddCell(new PdfPCell(new Phrase(cargo.Nombre, _fontStandard)));
                                            table.AddCell(new PdfPCell(new Phrase(aux.FechaInicio.HasValue ? string.Format("{0:dd-MM-yyyy}", aux.FechaInicio.Value) : string.Empty, _fontStandard)));
                                            table.AddCell(new PdfPCell(new Phrase(aux.FechaTermino.HasValue ? string.Format("{0:dd-MM-yyyy}", aux.FechaTermino.Value) : string.Empty, _fontStandard)));

                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception(string.Format("Error al emitir, las comisiones no estan actualizados"));

                                }
                                table.SpacingBefore = 15f;
                                doc.Add(table);
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("Error al emitir, no tiene comisiones"));
                        }
                        

                        doc.Add(SaltoLinea);
                    }


                    else
                    {
                        throw new Exception("Aviso: La Organización no cuenta con sus datos actualizados " +
                            "para una emisión de certificado inmediata. Por favor, para proceder con su requerimiento, seleccione la opción 'Certificado Disolución (Solicitar emisión)'");
                    }
                }
                /*
                if (!string.IsNullOrWhiteSpace(parrafo_dos))
                {
                    if (organizacion.FechaCelebracion.HasValue)
                    {
                        parrafo_dos = parrafo_dos.Replace("[FECHACELEBRACION]", string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.FechaCelebracion.Value));
                    }
                }
                */


                if (TipoDocumentoId == (int)Infrastructure.Enum.TipoDocumento.Vigencia ||
                    TipoDocumentoId == (int)Infrastructure.Enum.TipoDocumento.VigenciaDirectorio)
                {

                    parrafo_uno = parrafo_uno.Replace("[ROL]", organizacion.NumeroRegistro != null ? "#" + organizacion.NumeroRegistro + "#" : "[ERROR_Org]");
                    parrafo_uno = parrafo_uno.Replace("[RAZONSOCIAL]", organizacion.RazonSocial != null ? "#" + organizacion.RazonSocial + "#" : "[ERROR_Org]");
                    parrafo_uno = parrafo_uno.Replace("[SIGLA]", organizacion.Sigla != null ? "la que podrá usar indistintamente para todos los efectos legales el nombre de fantasía #" + organizacion.Sigla + "#," : "");
                    //parrafo_uno = parrafo_uno.Replace("[REGION]", organizacion.Region.Nombre != null ? "#" + organizacion.Region.Nombre + "#" : "[ERROR_Org]");
                    //parrafo_uno = parrafo_uno.Replace("[DOMICILLIOSOCIAL]", organizacion.Direccion != null ? "#" + organizacion.Direccion + "#" : "[ERROR_Org]");
                    if (parrafo_uno.Contains("ERROR_Org"))
                    {
                        throw new Exception(string.Format("Error al emitir, el apartado de Organizacion no tiene todos los campos requeridos"));
                    }
                    string[] parafo1;
                    parafo1 = parrafo_uno.Split('#');
                    index = 0;
                    foreach (string element in parafo1)
                    {
                        if (index % 2 == 0)
                        {
                            PhraseUNO = new Phrase(element, _fontStandard);
                        }
                        else
                        {
                            PhraseUNO = new Phrase(element, _fontNegrita);
                        }
                        comb.Add(PhraseUNO);
                        index++;
                    }

                    comb.Alignment = Element.ALIGN_JUSTIFIED;
                    doc.Add(SaltoLinea);
                    doc.Add(SaltoLinea);
                    doc.Add(SaltoLinea);
                    doc.Add(SaltoLinea);
                    doc.Add(comb);
                    doc.Add(SaltoLinea);
                }

                //Si se esta solicitando un certificado de vigencia de estatutos

                if (TipoDocumentoId == (int)Infrastructure.Enum.TipoDocumento.VigenciaEstatutos)
                {
                    if (organizacion.ExistenciaLegals.Any() || organizacion.Saneamientos.Any() || organizacion.ExistenciaAnteriors.Any() ||
                        organizacion.ExistenciaPosteriors.Any() || organizacion.ReformaAGACs.Any() || organizacion.ReformaAnteriors.Any() ||
                        organizacion.ReformaPosteriors.Any())
                    {


                        if (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.Cooperativa)
                        {
                            List<string> campos_parrafo1 = new List<string>();
                            foreach (var item in configuracioncertificado.Parrafo1.Replace(",", " ").Replace(".", " ").Split())
                            {
                                if (item.Contains("[") && item.Contains("]"))
                                {
                                    campos_parrafo1.Add(item);
                                }
                            }

                            for (int i = 0; i < campos_parrafo1.Count; i++)
                            {
                                var value = campos_parrafo1[i];
                                switch (value)
                                {
                                    case "[RAZONSOCIAL]":
                                        parrafo_uno = parrafo_uno.Replace(value, organizacion.RazonSocial != null ? organizacion.RazonSocial : string.Empty);
                                        break;
                                    case "[ROL]":
                                        parrafo_uno = parrafo_uno.Replace(value, organizacion.NumeroRegistro != null ? organizacion.NumeroRegistro : string.Empty);
                                        break;
                                    case "[SIGLA]":
                                        parrafo_uno = parrafo_uno.Replace(value, organizacion.Sigla != null ? organizacion.Sigla : string.Empty);
                                        break;
                                    case "[TIPOORGANIZACION]":
                                        parrafo_uno = parrafo_uno.Replace(value, organizacion.TipoOrganizacion.Nombre != null ? organizacion.TipoOrganizacion.Nombre : string.Empty);
                                        break;
                                    case "[VIGENTE]":
                                        parrafo_uno = parrafo_uno.Replace(value, organizacion.Estado.Nombre != null ? organizacion.Estado.Nombre : string.Empty);
                                        break;
                                }
                            }

                            Paragraph parrafouno = new Paragraph(parrafo_uno, _fontStandard);
                            parrafouno.Alignment = Element.ALIGN_JUSTIFIED;

                            doc.Add(SaltoLinea);
                            doc.Add(parrafouno);
                            doc.Add(SaltoLinea);

                            if (organizacion.ExistenciaPosteriors.Any() && organizacion.ExistenciaPosteriors.FirstOrDefault().FechaPublicacionn != null)
                            {
                                string parrafo = string.Format(configuracioncertificado.Parrafo2ExPosterior != null ? configuracioncertificado.Parrafo2ExPosterior : string.Empty);
                                List<string> campos_parrafo2 = new List<string>();
                                foreach (var item in parrafo.Replace(",", " ").Replace(".", " ").Split())
                                {
                                    if (item.Contains("[") && item.Contains("]"))
                                    {
                                        campos_parrafo2.Add(item);
                                    }
                                }



                                for (int i = 0; i < campos_parrafo2.Count; i++)
                                {
                                    var value = campos_parrafo2[i];
                                    switch (value)
                                    {
                                        case "[FECHACONSTITUTIVASOCIOS]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaPosteriors.FirstOrDefault().FechaConstitutivaSocios.Value != null ?
                                                                                string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.ExistenciaPosteriors.FirstOrDefault().FechaConstitutivaSocios.Value) : string.Empty);
                                            break;
                                        case "[FECHAESCRITURAPUBLICA]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaPosteriors.FirstOrDefault().FechaEscrituraPublica.Value != null ?
                                                                                string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.ExistenciaPosteriors.FirstOrDefault().FechaEscrituraPublica.Value) : string.Empty);
                                            break;
                                        case "[DATOSGENERALESNOTARIO]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaPosteriors.FirstOrDefault().DatosGeneralesNotario.ToString() != null ?
                                                                                organizacion.ExistenciaPosteriors.FirstOrDefault().DatosGeneralesNotario.ToString() : string.Empty);
                                            break;
                                        case "[FECHAPUBLICACIONN]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaPosteriors.FirstOrDefault().FechaPublicacionn.Value != null ?
                                                                                string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.ExistenciaPosteriors.FirstOrDefault().FechaPublicacionn.Value) : string.Empty);
                                            break;
                                        case "[FOJAS]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaPosteriors.FirstOrDefault().Fojas != null ?
                                                                                organizacion.ExistenciaPosteriors.FirstOrDefault().Fojas : string.Empty);
                                            break;
                                        case "[FECHAINSCRIPCION]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaPosteriors.FirstOrDefault().AnoInscripcion.ToString() != null ?
                                                                                organizacion.ExistenciaPosteriors.FirstOrDefault().AnoInscripcion.ToString() : string.Empty);
                                            break;
                                        case "[DATOSCBR]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaPosteriors.FirstOrDefault().DatosCBR.ToString() != null ?
                                                                                organizacion.ExistenciaPosteriors.FirstOrDefault().DatosCBR.ToString() : string.Empty);
                                            break;

                                    }
                                }

                                Paragraph parrafoDos = new Paragraph(parrafo, _fontStandard);
                                parrafoDos.Alignment = Element.ALIGN_JUSTIFIED;
                                doc.Add(parrafoDos);
                                doc.Add(SaltoLinea);
                            }

                            if (organizacion.ExistenciaAnteriors.Any())
                            {
                                var parrafo = string.Format(configuracioncertificado.Parrafo2ExAnterior != null ? configuracioncertificado.Parrafo2ExAnterior : string.Empty);
                                List<string> campos_parrafo2 = new List<string>();
                                foreach (var item in parrafo.Replace(",", " ").Replace(".", " ").Split())
                                {
                                    if (item.Contains("[") && item.Contains("]"))
                                    {
                                        campos_parrafo2.Add(item);
                                    }
                                }

                                for (int i = 0; i < campos_parrafo2.Count; i++)
                                {
                                    var value = campos_parrafo2[i];
                                    switch (value)
                                    {
                                        case "[TIPONORMA]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaAnteriors.FirstOrDefault().tipoNorma.Nombre.ToString() != null ?
                                                                                  organizacion.ExistenciaAnteriors.FirstOrDefault().tipoNorma.Nombre.ToString() : string.Empty);
                                            break;
                                        case "[NUMERONORMA]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaAnteriors.FirstOrDefault().NNorma.ToString() != null ?
                                                                                  organizacion.ExistenciaAnteriors.FirstOrDefault().NNorma.ToString() : string.Empty);
                                            break;
                                        case "[FECHANORMA]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaAnteriors.FirstOrDefault().FNorma.Value != null ?
                                                                                  string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.ExistenciaAnteriors.FirstOrDefault().FNorma.Value) : string.Empty);
                                            break;
                                        case "[AUTORIZADOPOR]":
                                            parrafo = parrafo.Replace(value, organizacion.ExistenciaAnteriors.FirstOrDefault().Autorizado.ToString() != null ?
                                                                                   organizacion.ExistenciaAnteriors.FirstOrDefault().Autorizado.ToString() : string.Empty);
                                            break;
                                            //case "[FECHAPUBLICACIONN]":
                                            //    parrafo = parrafo.Replace(value, organizacion.ExistenciaAnteriors.FirstOrDefault().FechaPublicacion.Value != null ?
                                            //                                           string.Format("{0:dd/MM/yyyy}", organizacion.ExistenciaAnteriors.FirstOrDefault().FechaPublicacion.Value) : string.Empty);
                                            //    break;
                                    }
                                }

                                if (organizacion.ExistenciaAnteriors.FirstOrDefault().FechaPublicacion != null)
                                {
                                    parrafo = parrafo + " con fecha de publicacion en el Diario Oficial: " + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.ExistenciaAnteriors.FirstOrDefault().FechaPublicacion.Value) + ".";
                                }
                                else
                                {
                                    parrafo = parrafo + ".";
                                }

                                Paragraph parrafox = new Paragraph(parrafo, _fontStandard);
                                parrafox.Alignment = Element.ALIGN_JUSTIFIED;
                                doc.Add(parrafox);
                                doc.Add(SaltoLinea);

                            }

                            if (organizacion.Saneamientos.Any())
                            {
                                string parrafo = string.Format(configuracioncertificado.Parrafo3 != null ? configuracioncertificado.Parrafo3 : string.Empty);
                                List<string> campos_parrafo2 = new List<string>();
                                foreach (var item in parrafo.Replace(",", " ").Replace(".", " ").Split())
                                {
                                    if (item.Contains("[") && item.Contains("]"))
                                    {
                                        campos_parrafo2.Add(item);
                                    }
                                }

                                for (int i = 0; i < campos_parrafo2.Count; i++)
                                {
                                    var value = campos_parrafo2[i];
                                    switch (value)
                                    {
                                        case "[FECHAESCRITURAPUBLICA]":
                                            parrafo = parrafo.Replace(value, organizacion.Saneamientos.FirstOrDefault().FechaEscrituraPublicaa.ToString() != null ?
                                                                                string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.Saneamientos.FirstOrDefault().FechaEscrituraPublicaa) : string.Empty);
                                            break;
                                        case "[FECHAPUBLICACIONDIARIO]":
                                            parrafo = parrafo.Replace(value, organizacion.Saneamientos.FirstOrDefault().FechaaPublicacionDiario.ToString() != null ?
                                                                                string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", organizacion.Saneamientos.FirstOrDefault().FechaaPublicacionDiario) : string.Empty);
                                            break;
                                        case "[DATOSGENERALESNOTARIO]":
                                            parrafo = parrafo.Replace(value, organizacion.Saneamientos.FirstOrDefault().DatoGeneralesNotario.ToString() != null ?
                                                                                organizacion.Saneamientos.FirstOrDefault().DatoGeneralesNotario.ToString() : string.Empty);
                                            break;
                                        case "[FOJAS]":
                                            parrafo = parrafo.Replace(value, organizacion.Saneamientos.FirstOrDefault().Fojass != null ?
                                                                                organizacion.Saneamientos.FirstOrDefault().Fojass : string.Empty);
                                            break;
                                        case "[FECHAINSCRIPCION]":
                                            parrafo = parrafo.Replace(value, organizacion.Saneamientos.FirstOrDefault().FechaaInscripcion.ToString() != null ?
                                                                                organizacion.Saneamientos.FirstOrDefault().FechaaInscripcion.ToString() : string.Empty);
                                            break;
                                        case "[DATOSCBR]":
                                            parrafo = parrafo.Replace(value, organizacion.Saneamientos.FirstOrDefault().DatossCBR.ToString() != null ?
                                                                                organizacion.Saneamientos.FirstOrDefault().DatossCBR.ToString() : string.Empty);
                                            break;
                                    }
                                }

                                Paragraph parr2 = new Paragraph(parrafo, _fontStandard);
                                parr2.Alignment = Element.ALIGN_JUSTIFIED;
                                doc.Add(parr2);
                                doc.Add(SaltoLinea);

                            }

                            if (organizacion.ReformaAnteriors.Any() == true && organizacion.ReformaAnteriors.FirstOrDefault().FechaReforma != null)
                            {
                                var contRef = 0;
                                //string parrafo = string.Format(configuracioncertificado.Parrafo4ReAnterior != null ? configuracioncertificado.Parrafo4ReAnterior : " ");
                                var fechaMayorr = organizacion.ReformaAnteriors.OrderByDescending(q => q.FechaReforma).FirstOrDefault();

                                List<string> campos_parrafo4 = new List<string>();
                                foreach (var item in configuracioncertificado.Parrafo4ReAnterior.Replace(",", " ").Replace(".", " ").Split())
                                {
                                    if (item.Contains("[") && item.Contains("]"))
                                    {
                                        campos_parrafo4.Add(item);
                                    }
                                }

                                foreach (var item in organizacion.ReformaAnteriors.ToList().OrderByDescending(q => q.FechaReforma).ToList())
                                {
                                    contRef++;
                                    string parrafo = string.Format(configuracioncertificado.Parrafo4ReAnterior != null ? configuracioncertificado.Parrafo4ReAnterior : " ");
                                    if (organizacion.ReformaAnteriors.Any() && contRef == 1)
                                    {
                                        parrafo = parrafo.Insert(0, "De acuerdo con los antecedentes registrados en esta División, la entidad presenta las siguientes reformas de estatutos:" +
                                                                           "\n" + "\n" + "REFORMA(AS) ANTERIOR(ES) AL AÑO 2003*:" + "\n" + "\n");

                                    }
                                    //else if (organizacion.ReformaAnteriors.Any() && contRef == 1 && organizacion.ReformaPosteriors.Any())
                                    //{
                                    //    parrafo = parrafo.Insert(0, "REFORMA(AS) ANTERIOR(ES) AL AÑO 2003*:" + "\n" + "\n");
                                    //}

                                    for (int i = 0; i < campos_parrafo4.Count; i++)
                                    {
                                        var value = campos_parrafo4[i];

                                        switch (value)
                                        {
                                            case "[COUNT]":
                                                parrafo = parrafo.Replace(value, contRef.ToString() + ".- ");
                                                break;
                                            case "[TIPONORMAREF]":
                                                parrafo = parrafo.Replace(value, item.TipoNorma.Nombre != null ? item.TipoNorma.Nombre : string.Empty);
                                                break;
                                            case "[NUMERONORMAREF]":
                                                parrafo = parrafo = parrafo.Replace(value, item.NNorma != null ? item.NNorma : string.Empty);
                                                break;
                                            case "[FECHANORMAREF]":
                                                parrafo = parrafo.Replace(value, item.FechaNorma.Value != null ? string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaNorma) : string.Empty);
                                                break;
                                            case "[DATOSGENERALNOTARIOREF]":
                                                parrafo = parrafo.Replace(value, item.DatosNotario != null ? item.DatosNotario : string.Empty);
                                                break;
                                            case "[FECHAPUBLICACIONDIARIOREF]":
                                                parrafo = parrafo.Replace(value, item.FechaPublicDiario.Value != null ? string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaPublicDiario) : string.Empty);
                                                break;
                                            case "[ESPACIO]":
                                                parrafo = parrafo.Replace(value, item.EspaciosDocAnterior != null ? item.EspaciosDocAnterior : string.Empty);
                                                break;
                                        }
                                    }
                                    Paragraph parro = new Paragraph(parrafo, _fontStandard);
                                    parro.Alignment = Element.ALIGN_JUSTIFIED;

                                    doc.Add(parro);
                                    doc.Add(SaltoLinea);

                                }
                            }

                            if (organizacion.ReformaPosteriors.Any())
                            {
                                var contRefPost = 0;
                                string parrafoEspacioPost = string.Format(configuracioncertificado.Parrafo5 != null ? configuracioncertificado.Parrafo5 : " ");

                                List<string> campos_parrafo3 = new List<string>();
                                foreach (var item in configuracioncertificado.Parrafo4RePosterior.Replace(",", " ").Replace(".", " ").Split())
                                {
                                    if (item.Contains("[") && item.Contains("]"))
                                    {
                                        campos_parrafo3.Add(item);
                                    }
                                }

                                foreach (var item in organizacion.ReformaPosteriors.ToList().OrderBy(q => q.FReforma).ToList())
                                {
                                    contRefPost++;
                                    string parrafo = string.Format(configuracioncertificado.Parrafo4RePosterior != null ? configuracioncertificado.Parrafo4RePosterior : " ");

                                    var fechaMayorr = organizacion.ReformaPosteriors.OrderByDescending(q => q.FReforma).FirstOrDefault();

                                    //Revisar esta parte
                                    if (organizacion.ReformaPosteriors.Any() && contRefPost == 1 && !organizacion.ReformaAnteriors.Any())
                                    {
                                        //para insertar en un indice especificado
                                        parrafo = parrafo.Insert(0, "De acuerdo con los antecedentes registrados en esta División, la entidad presenta las siguientes reformas de estatutos:" +
                                                                           "\n" + "\n" + "REFORMA(AS) POSTERIOR(ES) AL AÑO 2003*:" + "\n" + "\n");

                                    }
                                    else if (organizacion.ReformaPosteriors.Any() && contRefPost == 1 && organizacion.ReformaAnteriors.Any())
                                    {
                                        parrafo = parrafo.Insert(0, "REFORMA(AS) POSTERIOR(ES) AL AÑO 2003*:" + "\n" + "\n");
                                    }

                                    for (int i = 0; i < campos_parrafo3.Count; i++)
                                    {

                                        var value = campos_parrafo3[i];

                                        switch (value)
                                        {
                                            case "[COUNT]":
                                                parrafo = parrafo.Replace(value, contRefPost.ToString() + ".- ");
                                                break;
                                            case "[FECHAREFORMAA]":
                                                parrafo = parrafo.Replace(value, item.FechaJuntGeneralSocios.Value != null ?
                                                                          string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaJuntGeneralSocios.Value) : string.Empty);
                                                break;
                                            case "[FECHAREFORMAPOST]":
                                                parrafo = parrafo.Replace(value, item.FechaEscrituraPublica.Value != null ?
                                                                          string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaEscrituraPublica.Value) : string.Empty);
                                                break;
                                            case "[FECHAPUBLICDIARIO]":
                                                parrafo = parrafo.Replace(value, item.FechaPubliDiario.Value != null ?
                                                                          string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaPubliDiario.Value) : string.Empty);
                                                break;
                                            case "[DATOSGENERALESNOTARIO]":
                                                parrafo = parrafo.Replace(value, item.DatosGeneralNotario != null ? item.DatosGeneralNotario : string.Empty);
                                                break;
                                            case "[FOJAS]":
                                                parrafo = parrafo.Replace(value, item.FojasNumero != null ? item.FojasNumero : string.Empty);
                                                break;
                                            case "[DATOSCBRREF]":
                                                parrafo = parrafo.Replace(value, item.DatosCBR != null ? item.DatosCBR : string.Empty);
                                                break;
                                            case "[ANOINSCRIPCION]":
                                                parrafo = parrafo.Replace(value, item.AnoInscripcion != null ? item.AnoInscripcion : string.Empty);
                                                break;
                                            case "[ESPACIO]":
                                                parrafo = parrafo.Replace(value, item.EspaciosDoc != null ?
                                                                                              item.EspaciosDoc : string.Empty);
                                                break;
                                            case "[NUMEROOFICIO]":
                                                parrafo = parrafo.Replace(value, item.NumeroOficio != null ? item.NumeroOficio : string.Empty);
                                                break;
                                            case "[FECHAOFICIO]":
                                                parrafo = parrafo.Replace(value, item.FechaOficio.Value != null ?
                                                                          string.Format("{0:dd} de {0:MMMM} del {0:yyyy}", item.FechaOficio.Value) : string.Empty);
                                                break;
                                        }
                                    }

                                    Paragraph parraf = new Paragraph(parrafo, _fontStandard);
                                    parraf.Alignment = Element.ALIGN_JUSTIFIED;

                                    doc.Add(parraf);
                                    doc.Add(SaltoLinea);

                                }
                            }


                        }

                        if (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.AsociacionGremial ||
                                organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.AsociacionConsumidores)
                        {
                            parrafoone = parrafoone.Replace("[SIGLA]", organizacion.Sigla ?? string.Empty);
                            parrafoone = parrafoone.Replace("[RAZONSOCIAL]", organizacion.RazonSocial ?? string.Empty);
                            parrafoone = parrafoone.Replace("[TIPOORGANIZACION]", organizacion.TipoOrganizacion.Nombre ?? string.Empty);
                            parrafoone = parrafoone.Replace("[ROL]", organizacion.NumeroRegistro ?? string.Empty);
                            parrafoone = parrafoone.Replace("[VIGENTE]", organizacion.Estado.Nombre ?? string.Empty);

                            Paragraph parrafounoo = new Paragraph(parrafoone, _fontStandard);
                            parrafounoo.Alignment = Element.ALIGN_JUSTIFIED;
                            doc.Add(SaltoLinea);
                            doc.Add(parrafounoo);

                            if (organizacion.ExistenciaLegals.Any()) 
                            {
                                if (organizacion.ExistenciaLegals.FirstOrDefault().FechaConstitutivaSocios != null)
                                {
                                    parrafo_dos = parrafo_dos.Replace("[FECHACONSTITUTIVASOCIOS]", organizacion.ExistenciaLegals.FirstOrDefault().FechaConstitutivaSocios.HasValue ? string.Format("{0:dd} de {0:MMMM} del {0:yyyy}",organizacion.ExistenciaLegals.FirstOrDefault().FechaConstitutivaSocios.Value) : string.Empty);
                                }
                                else
                                {
                                    parrafo_dos = parrafo_dos.Replace("[FECHACONSTITUTIVASOCIOS]", string.Empty);
                                }
                                if (organizacion.ExistenciaLegals.FirstOrDefault().NumeroOficio != null)
                                {
                                    parrafo_dos = parrafo_dos.Replace("[NUMEROOFICIO]", organizacion.ExistenciaLegals.FirstOrDefault().NumeroOficio + ", " ?? string.Empty);
                                }
                                else
                                {
                                    parrafo_dos = parrafo_dos.Replace("[NUMEROOFICIO]", string.Empty);
                                }
                                if (organizacion.ExistenciaLegals.FirstOrDefault().FechaOficio != null)
                                {
                                    parrafo_dos = parrafo_dos.Replace("[FECHAOFICIO]", organizacion.ExistenciaLegals.FirstOrDefault().FechaOficio.Value.ToString("dd/MM/yyyy") ?? string.Empty);
                                }
                                else
                                {
                                    parrafo_dos = parrafo_dos.Replace("[FECHAOFICIO]", string.Empty);
                                }
                                if (organizacion.ExistenciaLegals.FirstOrDefault().Aprobacion != null && organizacion.ExistenciaLegals.FirstOrDefault().Aprobacion.Nombre != null)
                                {
                                    parrafo_dos = parrafo_dos.Replace("[APROBACION]", organizacion.ExistenciaLegals.FirstOrDefault().Aprobacion.Nombre ?? string.Empty);
                                }
                                else
                                {
                                    parrafo_dos = parrafo_dos.Replace("[APROBACION]", string.Empty);
                                }

                                Paragraph parrafoDos = new Paragraph(parrafo_dos, _fontStandard);
                                parrafoDos.Alignment = Element.ALIGN_JUSTIFIED;

                                Paragraph parrafoonee = new Paragraph(parrafoone, _fontStandard);
                                parrafoDos.Alignment = Element.ALIGN_JUSTIFIED;


                                doc.Add(SaltoLinea);
                                doc.Add(parrafoDos);
                                doc.Add(SaltoLinea);
                            }

                            if (organizacion.ReformaAGACs.Any())
                            {
                                var contRefAGAC = 0;


                                List<string> campos_parrafo4 = new List<string>();
                                foreach (var item in configuracioncertificado.Parrafo4.Replace(",", " ").Replace(".", " ").Split())
                                {
                                    if (item.Contains("[") && item.Contains("]"))
                                    {
                                        campos_parrafo4.Add(item);
                                    }
                                }

                                foreach (var item in organizacion.ReformaAGACs.ToList().OrderByDescending(q => q.FechaAsambleaDep).ToList())
                                {
                                    contRefAGAC++;
                                    string parrafo4_VE = string.Format(configuracioncertificado.Parrafo4 != null ? configuracioncertificado.Parrafo4 : " ");
                                    for (int i = 0; i < campos_parrafo4.Count; i++)
                                    {
                                        var value = campos_parrafo4[i];
                                        switch (value)
                                        {
                                            case "[COUNTREF]":
                                                parrafo4_VE = parrafo4_VE.Replace(value, contRefAGAC.ToString() + ".- ");
                                                break;
                                            case "[ASAMBLEA]":
                                                parrafo4_VE = parrafo4_VE.Replace(value, item.AsambleaDeposito.Descripcion != null ? item.AsambleaDeposito.Descripcion : string.Empty);
                                                break;
                                            case "[FECHAASAMBLEA]":
                                                parrafo4_VE = parrafo4_VE.Replace(value, item.FechaAsambleaDep.Value != null ?
                                                                                  string.Format("{0:dd/MM/yyyy}", item.FechaAsambleaDep.Value) : string.Empty);
                                                break;
                                            case "[NUMEROOFICIO]":
                                                parrafo4_VE = parrafo4_VE.Replace(value, item.NumeroOficio != null ? item.NumeroOficio : string.Empty);
                                                break;
                                            case "[APROBACION]":
                                                parrafo4_VE = parrafo4_VE.Replace(value, item.Aprobacion.Nombre != null ? item.Aprobacion.Nombre : string.Empty);
                                                break;
                                            case "[FECHAOFICIO]":
                                                parrafo4_VE = parrafo4_VE.Replace(value, item.FechaOficio.Value != null ?
                                                                                  string.Format("{0:dd/MM/yyyy}", item.FechaOficio.Value) : string.Empty);
                                                break;
                                            case "[ESPACIO]":
                                                parrafo4_VE = parrafo4_VE.Replace(value, item.EspaciosDocAGAC != null ? item.EspaciosDocAGAC : string.Empty);
                                                break;

                                        }
                                    }

                                    Paragraph parrafoss = new Paragraph(parrafo4_VE, _fontStandard);
                                    parrafoss.Alignment = Element.ALIGN_JUSTIFIED;

                                    doc.Add(parrafoss);
                                    doc.Add(SaltoLinea);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Aviso: La Organización no cuenta con sus datos actualizados " +
                        "para una emisión de certificado inmediata. Por favor, para proceder con su requerimiento, seleccione la opción 'Certificado Estatutos- Estatuto vigente actual (Solicitar emisión)'");
                    }
                }

                if (configuracioncertificado.TieneDirectorio)
                {
                    /*
                    var parrafo = string.Format(configuracioncertificado.Parrafo2 != null ? configuracioncertificado.Parrafo2 : " ");
                    string[] parafoPartido;
                    List<string> campos_parrafo2 = new List<string>();
                    foreach (var item in parrafo.Replace(",", " ").Replace(".", " ").Split())
                    {
                        if (item.Contains("[") && item.Contains("]"))
                        {
                            campos_parrafo2.Add(item);
                        }
                    }
                    for (int i = 0; i < campos_parrafo2.Count; i++)
                    {
                        var value = campos_parrafo2[i];
                        switch (value)
                        {
                            case "[FECHACELEBRACION]":
                                parrafo = parrafo.Replace(value, organizacion.FechaCelebracion.ToString() != null ?
                                                        string.Format("#" + "{0:dd/MM/yyyy}" + "#", organizacion.FechaCelebracion.Value) : string.Empty);
                                //parrafo = parrafo.Replace(value, "99/99/9999");                              
                                parafoPartido = parrafo.Split('#');
                                index = 0;

                                foreach (string element in parafoPartido)
                                {
                                    if (index % 2 == 0)
                                    {
                                        PhraseDOS = new Phrase(element, _fontStandard);
                                    }
                                    else
                                    {
                                        PhraseDOS = new Phrase(element, _fontNegrita);
                                    }
                                    comb2.Add(PhraseDOS);
                                    index++;
                                }
                                comb2.Alignment = Element.ALIGN_JUSTIFIED;
                                break;
                        }
                    }

                    Paragraph parrafito = new Paragraph(parrafo, _fontStandard);
                    parrafito.Alignment = Element.ALIGN_JUSTIFIED;
                    if (comb2 != null)
                    {
                        doc.Add(comb2);
                    }
                    else
                    {
                        doc.Add(parrafito);
                    }
                    */

                    //Esto se borra porque hay un salto de linea adicional
                    //doc.Add(SaltoLinea);
                    if (parrafo_dos != " ")
                    {
                        parrafo_dos = parrafo_dos.Replace("[FECHACELEBRACION]", organizacion.fechaasambleadirectorio != null ? "de fecha #" + string.Format("{0:dd} de {0:MMMM} del {0:yyyy}" , organizacion.fechaasambleadirectorio.Value) +"#, ": "");

                        string[] parafo2;
                        parafo2 = parrafo_dos.Split('#');
                        index = 0;
                        foreach (string element in parafo2)
                        {
                            if (index % 2 == 0)
                            {
                                PhraseDOS = new Phrase(element, _fontStandard);
                            }
                            else
                            {
                                PhraseDOS = new Phrase(element, _fontNegrita);
                            }
                            comb2.Add(PhraseDOS);
                            index++;
                        }
                        doc.Add(comb2);
                        doc.Add(SaltoLinea);
                    }
                    var directorio = context.Directorio.Where(q => q.OrganizacionId == organizacion.OrganizacionId).OrderBy(q => q.Cargo.Secuencia);
                    if (directorio.Any())
                    {
                        //Esto duplica el parrafo 2
                        //doc.Add(paragraphDOS);
                        bool filaCompleta = false;
                        if (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.Cooperativa)
                        {
                            PdfPTable table = new PdfPTable(4);
                            table.WidthPercentage = 100.0f;
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.DefaultCell.BorderColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(new PdfPCell(new Phrase("Nombre", _fontStandardBold)));
                            table.AddCell(new PdfPCell(new Phrase("Cargo", _fontStandardBold)));
                            PdfPCell cellDesde = new PdfPCell(new Phrase("Desde", _fontStandardBold));
                            cellDesde.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cellDesde);
                            PdfPCell cellHasta = new PdfPCell(new Phrase("Hasta", _fontStandardBold));
                            cellHasta.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cellHasta);
                            table.SetWidths(new float[] { 4f, 6f, 3f, 3f });
                            foreach (var item in directorio.ToList())
                            {
                                var cargo = context.Cargo.FirstOrDefault(q => q.CargoId == item.CargoId);
                                if (item.NombreCompleto != null && cargo.Nombre != null && item.FechaInicio.HasValue == true && item.FechaTermino.HasValue == true)
                                {
                                    filaCompleta = true;
                                    break;
                                }
                            }
                            if (filaCompleta == true)
                            {
                                foreach (var item in directorio.ToList())
                                {
                                    var cargo = context.Cargo.FirstOrDefault(q => q.CargoId == item.CargoId);
                                    if (cargo != null)
                                        table.AddCell(new PdfPCell(new Phrase(item.NombreCompleto, _fontStandard)));

                                    table.AddCell(new PdfPCell(new Phrase(cargo.Nombre, _fontStandard)));
                                    table.AddCell(new PdfPCell(new Phrase(item.FechaInicio.HasValue ? string.Format("{0:dd-MM-yyyy}", item.FechaInicio.Value) : string.Empty, _fontStandard)));
                                    table.AddCell(new PdfPCell(new Phrase(item.FechaTermino.HasValue ? string.Format("{0:dd-MM-yyyy}", item.FechaTermino.Value) : string.Empty, _fontStandard)));

                                }
                            }
                            else
                            {
                                throw new Exception(string.Format("Error al emitir, los directorios no estan actualizados"));

                            }


                            if (!organizacion.NotaDirectorio.IsNullOrWhiteSpace())
                            {
                                table.AddCell(new PdfPCell(new Phrase("*NOTA", _fontStandard)));
                                table.AddCell(new PdfPCell(new Phrase(organizacion.NotaDirectorio, _fontStandardBold)) { HorizontalAlignment = Element.ALIGN_JUSTIFIED });
                            }

                            table.SpacingBefore = 15f;
                            doc.Add(table);
                        }
                        else if (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.AsociacionGremial)
                        {
                            PdfPTable table = new PdfPTable(4);
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.WidthPercentage = 100f;
                            table.DefaultCell.BorderColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(new PdfPCell(new Phrase("Nombre", _fontStandardBold)));
                            table.AddCell(new PdfPCell(new Phrase("Cargo", _fontStandardBold)));
                            PdfPCell cellDesde = new PdfPCell(new Phrase("Desde", _fontStandardBold));
                            cellDesde.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cellDesde);
                            PdfPCell cellHasta = new PdfPCell(new Phrase("Hasta", _fontStandardBold));
                            cellHasta.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cellHasta);
                            table.SetWidths(new float[] { 4f, 6f, 3f, 3f });

                            foreach (var item in directorio.ToList())
                            {
                                var cargo = context.Cargo.FirstOrDefault(q => q.CargoId == item.CargoId);
                                if (item.NombreCompleto != null && cargo.Nombre != null && item.FechaInicio.HasValue == true && item.FechaTermino.HasValue == true)
                                {
                                    filaCompleta = true;
                                    break;
                                }
                            }
                            if (filaCompleta == true)
                            {
                                foreach (var item in directorio.ToList())
                                {
                                    var cargo = context.Cargo.FirstOrDefault(q => q.CargoId == item.CargoId);
                                    if (cargo != null)
                                        table.AddCell(new PdfPCell(new Phrase(item.NombreCompleto, _fontStandard)));

                                    table.AddCell(new PdfPCell(new Phrase(cargo.Nombre, _fontStandard)));
                                    table.AddCell(new PdfPCell(new Phrase(item.FechaInicio.HasValue ? string.Format("{0:dd-MM-yyyy}", item.FechaInicio.Value) : string.Empty, _fontStandard)));
                                    table.AddCell(new PdfPCell(new Phrase(item.FechaTermino.HasValue ? string.Format("{0:dd-MM-yyyy}", item.FechaTermino.Value) : string.Empty, _fontStandard)));

                                }
                            }
                            else
                            {
                                throw new Exception(string.Format("Error al emitir, los directorios no estan actualizados"));

                            }

                            if (!organizacion.NotaDirectorio.IsNullOrWhiteSpace())
                            {
                                table.AddCell(new PdfPCell(new Phrase("*NOTA", _fontStandard)));
                                table.AddCell(new PdfPCell(new Phrase(organizacion.NotaDirectorio, _fontStandardBold)) { Colspan = 3, HorizontalAlignment = Element.ALIGN_JUSTIFIED });
                            }
                            table.SpacingBefore = 15f;
                            doc.Add(table);
                        }
                        else if (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.AsociacionConsumidores)
                        {
                            PdfPTable table = new PdfPTable(4);
                            table.WidthPercentage = 100.0f;
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.DefaultCell.BorderColor = BaseColor.LIGHT_GRAY;
                            table.AddCell(new PdfPCell(new Phrase("Nombre", _fontStandardBold)));
                            table.AddCell(new PdfPCell(new Phrase("Cargo", _fontStandardBold)));
                            PdfPCell cellDesde = new PdfPCell(new Phrase("Desde", _fontStandardBold));
                            cellDesde.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cellDesde);
                            PdfPCell cellHasta = new PdfPCell(new Phrase("Hasta", _fontStandardBold));
                            cellHasta.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cellHasta);
                            table.SetWidths(new float[] { 4f, 6f, 3f, 3f });

                            foreach (var item in directorio.ToList())
                            {
                                var cargo = context.Cargo.FirstOrDefault(q => q.CargoId == item.CargoId);
                                if (item.NombreCompleto != null && cargo.Nombre != null && item.FechaInicio.HasValue == true && item.FechaTermino.HasValue == true)
                                {
                                    filaCompleta = true;
                                    break;
                                }
                            }
                            if (filaCompleta == true)
                            {
                                foreach (var item in directorio.ToList())
                                {
                                    var cargo = context.Cargo.FirstOrDefault(q => q.CargoId == item.CargoId);
                                    if (cargo != null)
                                        table.AddCell(new PdfPCell(new Phrase(item.NombreCompleto, _fontStandard)));

                                    table.AddCell(new PdfPCell(new Phrase(cargo.Nombre, _fontStandard)));
                                    table.AddCell(new PdfPCell(new Phrase(item.FechaInicio.HasValue ? string.Format("{0:dd-MM-yyyy}", item.FechaInicio.Value) : string.Empty, _fontStandard)));
                                    table.AddCell(new PdfPCell(new Phrase(item.FechaTermino.HasValue ? string.Format("{0:dd-MM-yyyy}", item.FechaTermino.Value) : string.Empty, _fontStandard)));
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format("Error al emitir, los directorios no estan actualizados"));

                            }

                            if (!organizacion.NotaDirectorio.IsNullOrWhiteSpace())
                            {
                                table.AddCell(new PdfPCell(new Phrase("*NOTA", _fontStandard)));
                                table.AddCell(new PdfPCell(new Phrase(organizacion.NotaDirectorio, _fontStandardBold)) { Colspan = 3, HorizontalAlignment = Element.ALIGN_JUSTIFIED });
                            }


                            table.SpacingBefore = 15f;
                            doc.Add(table);
                        }
                    }
                    else
                    {
                        if ((organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.AsociacionGremial) || (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.AsociacionConsumidores)) //AG y AC
                        {
                            Paragraph SinDirectorio = new Paragraph(" * A la fecha, no se registra directorio de la organización en esta División.", _fontStandard);
                            doc.Add(SaltoLinea);
                            doc.Add(SinDirectorio);
                        }
                        else if (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.Cooperativa)//Cooperativas cambia
                        {
                            Paragraph SinDirectorio = new Paragraph(" * A la fecha, no se registra consejo de administración de la organización en esta División.", _fontStandard);
                            doc.Add(SaltoLinea);
                            doc.Add(SinDirectorio);
                        }
                    }
                }

                //TODO: Aqui va el parrafo de Observacion
                var parrafo_observacion = string.Format(configuracioncertificado.ParrafoObservacion != null ? configuracioncertificado.ParrafoObservacion : " ");
                Paragraph parraOb = new Paragraph(parrafo_observacion, _fontStandard);
                doc.Add(SaltoLinea);
                doc.Add(parraOb);
                doc.Add(SaltoLinea);

                if (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.Cooperativa)
                {
                    //string orden = "Por orden del Subsecretario";
                    //Se quita salto de linea, esto porque queda muy separado del párrafo anterior
                    //doc.Add(SaltoLinea);
                    if (TipoDocumentoId == 103)
                    {
                        string orden = "Se hace presente que no se registra en nuestros archivos la cancelación de " +
                            "la personalidad jurídica de dicha Cooperativa." + "\n" + "\n" +
                            "Saluda atentamente a ustedes";

                        Paragraph porOrden = new Paragraph(orden, _fontStandard);
                        porOrden.Alignment = Element.ALIGN_JUSTIFIED;
                        doc.Add(porOrden);
                    }
                    else
                    {


                        string orden = "Saluda atentamente a ustedes.";
                        Paragraph porOrden = new Paragraph(orden, _fontStandard);
                        porOrden.Alignment = Element.ALIGN_JUSTIFIED;
                        doc.Add(porOrden);


                    }
                }

                if (organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.AsociacionGremial ||
                    organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.AsociacionConsumidores)
                {
                    //string orden = "Por orden del Subsecretario";
                    doc.Add(SaltoLinea);
                    if (TipoDocumentoId == 103)
                    {
                        string orden =
                            //"DECRETO TRAN N° 119247/1/2021, DEL 01 DE FEBRERO DE 2021." + "\n" + "\n" +
                            // "\n" + "\n" +
                            "Saluda atentamente a ustedes";

                        Paragraph porOrden = new Paragraph(orden, _fontStandard);
                        porOrden.Alignment = Element.ALIGN_JUSTIFIED;
                        doc.Add(porOrden);
                    }
                    else
                    {
                        string orden = "Saluda atentamente a ustedes.";
                        Paragraph porOrden = new Paragraph(orden, _fontStandard);
                        porOrden.Alignment = Element.ALIGN_JUSTIFIED;
                        doc.Add(porOrden);
                    }
                }
                doc.Add(SaltoLinea);
                if ((int)DAES.Infrastructure.Enum.TipoDocumento.VigenciaDirectorio == TipoDocumentoId)
                {
                    doc.Add(rae);
                }
                //imagen firma
                var configFirma = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.URLImagenRubrica);
                if (configFirma == null)
                {
                    throw new Exception("No se encontró la configuración de url de rúbrica.");
                }

                if (configFirma != null && configFirma.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("La configuración de url de rúbrica es inválida.");
                }


                Paragraph responsable = new Paragraph(firmante.Nombre, _fontStandardBold);
                responsable.Alignment = centrar;
                doc.Add(responsable);

                Paragraph _cargo = new Paragraph(firmante.Cargo, _fontStandardBold);
                _cargo.Alignment = centrar;
                doc.Add(_cargo);

                Paragraph _unidad = new Paragraph(firmante.UnidadOrganizacional, _fontStandardBold);
                _unidad.Alignment = centrar;
                doc.Add(_unidad);
                doc.Add(SaltoLinea);
                doc.Close();

                return memStream.ToArray();
            }

        }

        public void ReformaCreate(Reforma reforma, Organizacion or)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                var refo = context.Reforma.Any(q => q.IdReforma == reforma.IdReforma);
                if (refo == false)
                {
                    context.Reforma.Add(new Reforma()
                    {
                        //OrganizacionId = or.OrganizacionId,
                        FechaReforma = reforma.FechaReforma,
                        FechaReformaa = reforma.FechaReformaa,
                        UltimaReforma = reforma.UltimaReforma,
                        TipoNormaId = reforma.TipoNormaId,
                        NumeroNormaa = reforma.NumeroNormaa,
                        FechaNormaa = reforma.FechaNormaa,
                        FechaPublicacionDiario = reforma.FechaPublicacionDiario,
                        DatosGeneralNotario = reforma.DatosGeneralNotario,
                        FechaJuntaGeneral = reforma.FechaJuntaGeneral,
                        FechaPublicacion = reforma.FechaPublicacion,
                        AnoInscripcion = reforma.AnoInscripcion,
                        DatosCBR = reforma.DatosCBR,
                        FechaEscrituraPublica = reforma.FechaEscrituraPublica,
                        Fojas = reforma.Fojas,
                        AsambleaDepId = reforma.AsambleaDepId,
                        FechaOficio = reforma.FechaOficio,
                        NumeroOficio = reforma.NumeroOficio,
                        AprobacionId = reforma.AprobacionId


                    });
                    context.SaveChanges();
                }


            }
        }

        public void ReformaAGCreate(ReformaAGAC reforma, Organizacion or)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                var refo = context.ReformaAGAC.Any(q => q.IdReformaAGAC == reforma.IdReformaAGAC);
                if (refo == false)
                {
                    context.ReformaAGAC.Add(new ReformaAGAC()
                    {
                        //OrganizacionId = or.OrganizacionId,
                        AsambleaDepId = reforma.AsambleaDepId,
                        FechaAsambleaDep = reforma.FechaAsambleaDep,
                        NumeroOficio = reforma.NumeroOficio,
                        FechaOficio = reforma.FechaOficio,
                        AprobacionId = reforma.AprobacionId,
                        //EspaciosDocAGAC = reforma.EspaciosDocAGAC

                    });
                    context.SaveChanges();
                }


            }
        }
        public void ReformaPosteriorCreate(ReformaPosterior reforma, Organizacion or)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                var refo = context.ReformaPosterior.Any(q => q.IdReformaPost == reforma.IdReformaPost);
                if (refo == false)
                {
                    context.ReformaPosterior.Add(new ReformaPosterior()
                    {
                        //OrganizacionId = or.OrganizacionId,
                        FReforma = reforma.FReforma,

                        FechaJuntGeneralSocios = reforma.FechaJuntGeneralSocios,
                        FechaEscrituraPublica = reforma.FechaEscrituraPublica,
                        FojasNumero = reforma.FojasNumero,
                        AnoInscripcion = reforma.AnoInscripcion,
                        DatosCBR = reforma.DatosCBR,
                        FechaPubliDiario = reforma.FechaPubliDiario,
                        EspaciosDoc = reforma.EspaciosDoc



                    });
                    context.SaveChanges();
                }


            }
        }

        public void ReformaAnteriorCreate(ReformaAnterior reforma, Organizacion or)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                var refo = context.ReformaAnterior.Any(q => q.IdReformaAnterior == reforma.IdReformaAnterior);
                if (refo == false)
                {
                    context.ReformaAnterior.Add(new ReformaAnterior()
                    {
                        //OrganizacionId = or.OrganizacionId,
                        FechaReforma = reforma.FechaReforma,
                        TipoNormaId = reforma.TipoNormaId,
                        FechaNorma = reforma.FechaNorma,
                        NNorma = reforma.NNorma,
                        FechaPublicDiario = reforma.FechaPublicDiario,
                        DatosNotario = reforma.DatosNotario,
                        EspaciosDocAnterior = reforma.EspaciosDocAnterior,

                    });
                    context.SaveChanges();
                }


            }
        }

        public void SaneamientoCreate(Saneamiento saneamiento, Organizacion or)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                var san = context.Saneamiento.Any(q => q.IdSaneamiento == saneamiento.IdSaneamiento);
                if (san == false)
                {
                    context.Saneamiento.Add(new Saneamiento()
                    {
                        OrganizacionId = or.OrganizacionId,
                        FechaEscrituraPublicaa = saneamiento.FechaEscrituraPublicaa,
                        FechaaPublicacionDiario = saneamiento.FechaaPublicacionDiario,
                        DatoGeneralesNotario = saneamiento.DatoGeneralesNotario,
                        Fojass = saneamiento.Fojass,
                        FechaaInscripcion = saneamiento.FechaaInscripcion,
                        DatossCBR = saneamiento.DatossCBR,


                    });
                    context.SaveChanges();
                }

            }
        }

        public void ExistenciaCreate(Organizacion or, ExistenciaLegal existenciaLegals)
        {

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {


                var returnValue = new List<string>();

                var existencia = context.ExistenciaLegal.Any(q => q.ExistenciaId == or.ExistenciaLegals.FirstOrDefault().ExistenciaId);

                if (existencia == false)
                {
                    context.ExistenciaLegal.Add(new ExistenciaLegal()
                    {
                        OrganizacionId = or.OrganizacionId,
                        TipoNormaId = or.TipoNormaId,
                        NumeroNorma = existenciaLegals.NumeroNorma,
                        FechaNorma = existenciaLegals.FechaNorma,
                        FechaPublicacionn = existenciaLegals.FechaPublic != null ? existenciaLegals.FechaPublic : existenciaLegals.FechaPubli,
                        AutorizadoPor = existenciaLegals.AutorizadoPor,
                        FechaConstitutivaSocios = existenciaLegals.FechaConstitutivaSocios,
                        NumeroOficio = existenciaLegals.NumeroOficio,
                        FechaOficio = existenciaLegals.FechaOficio,
                        FechaEscrituraPublica = existenciaLegals.FechaEscrituraPublica,
                        DatosGeneralesNotario = existenciaLegals.DatosGeneralesNotario,
                        Fojas = existenciaLegals.Fojas,
                        FechaInscripcion = existenciaLegals.FechaInscripcion,
                        DatosCBR = existenciaLegals.DatosCBR,
                        AprobacionId = existenciaLegals.AprobacionId

                    });
                    context.SaveChanges();

                }



            }
        }

        public void ExistenciaAnteriorCreate(Organizacion or, ExistenciaAnterior ExiAnt)
        {

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {


                var returnValue = new List<string>();

                var existencia = context.ExistenciaLegalAnterior.Any(q => q.OrganizacionId == ExiAnt.OrganizacionId);

                if (existencia == false)
                {
                    context.ExistenciaLegalAnterior.Add(new ExistenciaAnterior()
                    {
                        OrganizacionId = or.OrganizacionId,
                        TipoNormaId = or.TipoNormaId,
                        NNorma = ExiAnt.NNorma,
                        FNorma = ExiAnt.FNorma,
                        FechaPublicacion = ExiAnt.FechaPublicacion,
                        Autorizado = ExiAnt.Autorizado


                    });
                    context.SaveChanges();

                }

            }
        }

        public void ExistenciaPostCreate(Organizacion or, ExistenciaPosterior ExiPost)
        {

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {


                var returnValue = new List<string>();

                var existencia = context.ExistenciaPosterior.Any(q => q.OrganizacionId == ExiPost.OrganizacionId);

                if (existencia == false)
                {
                    context.ExistenciaPosterior.Add(new ExistenciaPosterior()
                    {
                        OrganizacionId = or.OrganizacionId,
                        FechaConstitutivaSocios = ExiPost.FechaConstitutivaSocios,
                        FechaEscrituraPublica = ExiPost.FechaEscrituraPublica,
                        FechaPublicacionn = ExiPost.FechaPublicacionn,
                        DatosGeneralesNotario = ExiPost.DatosGeneralesNotario,
                        Fojas = ExiPost.Fojas,
                        AnoInscripcion = ExiPost.AnoInscripcion,
                        DatosCBR = ExiPost.DatosCBR



                    });
                    context.SaveChanges();

                }

            }
        }



        public byte[] SignPDF(int id, string folio, byte[] content, string nombre, Firmante firmante, bool AgregarRubrica, int TipoDocumentoId, int TipoOrganizacionId)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                //validaciones
                var UserHSM = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.UserHSM);
                if (UserHSM == null)
                {
                    throw new Exception("No se encontró la configuración de usuario de HSM.");
                }

                if (UserHSM != null && UserHSM.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("La configuración de usuario de HSM es inválida.");
                }

                var PasswordHSM = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.PasswordHSM);
                if (PasswordHSM == null)
                {
                    throw new Exception("No se encontró la configuración de usuario de HSM.");
                }

                if (PasswordHSM != null && PasswordHSM.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("La configuración de password de HSM es inválida.");
                }

                //si se requieres agregar rubrica al documento
                if (AgregarRubrica)
                {
                    var URLImagenRubrica = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.URLImagenRubrica);
                    if (URLImagenRubrica == null)
                    {
                        throw new Exception("No se encontró la configuración de url de rúbrica de firma electrónica.");
                    }

                    if (URLImagenRubrica != null && URLImagenRubrica.Valor.IsNullOrWhiteSpace())
                    {
                        throw new Exception("La configuración de url de rúbrica de firma electrónica es inválida.");
                    }

                    var configmensajeVerificacion = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.MensajeVerificacionCertificado);
                    if (configmensajeVerificacion == null)
                    {
                        throw new Exception("No existe la configuración de mensaje de verificación de certificados");
                    }

                    if (configmensajeVerificacion != null && configmensajeVerificacion.Valor.IsNullOrWhiteSpace())
                    {
                        throw new Exception("La configuración de mensaje de verificación de certificados es inválida");
                    }

                    var configmensajePie = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.MensajePieFirmaCertificado);
                    if (configmensajePie == null)
                    {
                        throw new Exception("No existe la configuración de mensaje pié de firma de certificados");
                    }

                    if (configmensajePie != null && configmensajePie.Valor.IsNullOrWhiteSpace())
                    {
                        throw new Exception("La configuración de mensaje de pié de firma de certificados es inválida");
                    }

                    Font _fontStandard = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.DARK_GRAY);
                    Font _fontStandardBold = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.DARK_GRAY);
                    Font _fontBold = new Font(Font.FontFamily.HELVETICA, 24, Font.BOLD, BaseColor.DARK_GRAY);

                    try
                    {
                        //cargar imagen rubrica
                        Image image = Image.GetInstance(URLImagenRubrica.Valor + firmante.IdFirma + ".png");

                        using (MemoryStream ms = new MemoryStream())
                        {
                            var reader = new PdfReader(content);

                            using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
                            {
                                var pdfContentFirstPage = stamper.GetOverContent(1);

                                if (!string.IsNullOrWhiteSpace(folio))
                                {
                                    Rectangle pagesize = reader.GetPageSize(1);
                                    if (pagesize != null)
                                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_RIGHT, new Phrase(string.Format("N° {0}", folio), _fontBold), 550, pagesize.Height - 95, 0);
                                    else
                                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_RIGHT, new Phrase(string.Format("N° {0}", folio), _fontBold), 550, 940, 0);
                                }

                                int delta = 150;

                                var pdfContentByte = stamper.GetOverContent(reader.NumberOfPages);
                                image.SetAbsolutePosition(180, delta);
                                image.ScalePercent(80);
                                pdfContentByte.AddImage(image);

                                ColumnText.ShowTextAligned(pdfContentByte, Element.ALIGN_CENTER, new Phrase(firmante.Nombre, _fontStandardBold), 300, delta + 50, 0);
                                ColumnText.ShowTextAligned(pdfContentByte, Element.ALIGN_CENTER, new Phrase(firmante.Cargo, _fontStandardBold), 300, delta + 40, 0);
                                ColumnText.ShowTextAligned(pdfContentByte, Element.ALIGN_CENTER, new Phrase(firmante.UnidadOrganizacional, _fontStandardBold), 300, delta + 30, 0);

                                if (id > 0)
                                {
                                    ColumnText.ShowTextAligned(pdfContentByte, Element.ALIGN_CENTER, new Phrase(configmensajePie.Valor, _fontStandard), 300, delta + 10, 0);
                                    ColumnText.ShowTextAligned(pdfContentByte, Element.ALIGN_CENTER, new Phrase(configmensajeVerificacion.Valor + " usando el código " + id, _fontStandard), 300, delta + 0, 0);
                                }
                                stamper.Close();
                            }
                            content = ms.ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error al insertar rubrica en el documento:" + ex);
                    }
                }

                //firmar electrónicamente
                try
                {
                    var header = new Infrastructure.ServiceReferenceHSM.EncabezadoRequest()
                    {
                        User = UserHSM.Valor,
                        Password = PasswordHSM.Valor,
                        NombreConfiguracion = firmante.IdFirma,
                        TipoIntercambio = "PDF",
                        FormatoDocumento = "b64",
                        RespuestaEsperada = "1"
                    };

                    var documento = new Infrastructure.ServiceReferenceHSM.DocumentoParametro()
                    {
                        Documento = Convert.ToBase64String(content),
                        NombreDocumento = nombre
                    };

                    var hsmService = new Infrastructure.ServiceReferenceHSM.WSIntercambiaDocSoapClient();
                    var response = hsmService.IntercambiaDoc(header, documento);

                    if (response.Estado.Contains("FAIL"))
                    {
                        throw new Exception("Error de firma de documento.");
                    }

                    return Convert.FromBase64String(response.Documento);
                }
                catch (Exception ex)
                {
                    LogAdd(new Log() { LogId = Guid.NewGuid(), LogTimeLocal = DateTime.Now, LogAreaAccessed = "FirmarPDF", LogTimeUtc = DateTime.UtcNow, LogDetails = "Se detectó un problema al momento de firmar el documento", LogIpAddress = Dns.GetHostName(), LogUserName = Environment.UserName, LogContent = ex.Message });
                    throw new Exception("Error de firma de documento.");
                }
            }
        }

        //public void ActualizarDirectorioA(Proceso obj)
        //{
        //    var directorios_update = obj.ActualizacionOrganizacions.FirstOrDefault().Directorio.ToList();
        //    var modelo_update = db.ActualizacionOrganizacionDirectorio.Where(q => q.DirectorioUpdateId == 65489648).ToList();
        //    foreach (var item in modelo_update)
        //    {
        //        item.DirectorioUpdateId = obj.ActualizacionOrganizacions.FirstOrDefault().ActualizacionOrganizacionId;
        //    }
        //    db.SaveChanges();
        //}

        public Proceso ProcesoStart(Proceso obj)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                if (!context.DefinicionProceso.Any(q => q.DefinicionProcesoId == obj.DefinicionProcesoId))
                {
                    throw new Exception("No se encontró la definición del proceso");
                }

                if (obj.Solicitante == null)
                {
                    throw new Exception("No se encontró el solicitante");
                }

                if (obj.Solicitante != null && obj.Solicitante.Email.IsNullOrWhiteSpace())
                {
                    throw new Exception("Debe especificar el email del solicitante");
                }

                if (obj.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.ConstitucionWeb)
                {
                    if (obj.Organizacion == null)
                    {
                        throw new Exception("No se encontró la información de la organización");
                    }
                }

                if (
                obj.DefinicionProcesoId != (int)Infrastructure.Enum.DefinicionProceso.EstudioSocioEconomicos &&
                obj.DefinicionProcesoId != (int)Infrastructure.Enum.DefinicionProceso.ConstitucionWeb &&
                obj.DefinicionProcesoId != (int)Infrastructure.Enum.DefinicionProceso.ConstitucionOP &&
                obj.DefinicionProcesoId != (int)Infrastructure.Enum.DefinicionProceso.CooperativaViviendaAbierta &&
                obj.DefinicionProcesoId != (int)Infrastructure.Enum.DefinicionProceso.IngresoSupervisorAuxiliar &&
                obj.DefinicionProcesoId != (int)Infrastructure.Enum.DefinicionProceso.ActualizacionSupervisorAuxiliar
                )
                {
                    if (!context.Organizacion.Any(q => q.OrganizacionId == obj.OrganizacionId))
                    {
                        throw new Exception("No se encontró la organización");
                    }
                }

                if (obj.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoManual || obj.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoAutomatico || obj.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.Actualizacion)
                {
                    if (obj.Solicitante != null && obj.Solicitante.Rut.IsNullOrWhiteSpace())
                    {
                        throw new Exception("Debe especificar el RUT del solicitante");
                    }

                    if (obj.Solicitante != null && obj.Solicitante.Nombres.IsNullOrWhiteSpace())
                    {
                        throw new Exception("Debe especificar el nombre del solicitante");
                    }

                    if (obj.Solicitante != null && obj.Solicitante.Apellidos.IsNullOrWhiteSpace())
                    {
                        throw new Exception("Debe especificar el apellido del solicitante");
                    }
                }

                if (obj.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoManual || obj.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoAutomatico)
                {
                    if (!obj.Documentos.Any())
                    {
                        throw new Exception("No se encontró datos del documento a emitir");
                    }

                    if (!context.Firmante.Any(q => q.EsActivo))
                    {
                        throw new Exception("No se encontró un firmante de certificado habilitado");
                    }

                    if (obj.Documentos.Any())
                    {
                        foreach (var certificado in obj.Documentos)
                        {
                            if (!context.TipoDocumento.Any(q => q.TipoDocumentoId == certificado.TipoDocumentoId))
                            {
                                throw new Exception("No se encontró el tipo de documento a emitir");
                            }
                        }
                    }
                }

                //iniciar nuevo proceso
                var proceso = new Proceso();
                proceso.DefinicionProceso = context.DefinicionProceso.FirstOrDefault(q => q.DefinicionProcesoId == obj.DefinicionProcesoId);
                proceso.FechaVencimiento = DateTime.Now.AddBusinessDays(proceso.DefinicionProceso.Duracion);
                proceso.Observacion = obj.Observacion;
                proceso.UserId = obj.UserId;
                proceso.FechaCreacion = DateTime.Now;

                proceso.Solicitante = new Solicitante()
                {
                    Nombres = obj.Solicitante.Nombres,
                    Apellidos = obj.Solicitante.Apellidos,
                    Email = obj.Solicitante.Email,
                    Rut = obj.Solicitante.Rut,
                    Fono = obj.Solicitante.Fono,
                    RegionId = obj.Solicitante.RegionId,
                    Cargo = obj.Solicitante.Cargo
                };




                //en el caso de que el proceso se origine a partir de un documento, asociar datos
                if (obj.DocumentoId > 0)
                {
                    var documento = gestionDocumentalContext.Documento.FirstOrDefault(q => q.Id == obj.DocumentoId);
                    if (documento != null)
                    {
                        proceso.DocumentoId = documento.Id;
                        proceso.Correlativo = documento.Doc_Correlativo;
                    }
                }

                //en el caso de un proceso de constitucion, asociar nueva organización
                if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.ConstitucionWeb)
                {
                    //si viene datos de una organizacion, usarlos para crea la nueva
                    if (obj.Organizacion != null)
                    {
                        proceso.Organizacion = obj.Organizacion;
                    }

                    //si no vienen datos, crear una nueva organizacion
                    if (obj.Organizacion == null)
                    {
                        proceso.Organizacion = new Organizacion()
                        {
                            FechaCreacion = DateTime.Now,
                            TipoOrganizacionId = (int)Infrastructure.Enum.TipoOrganizacion.AunNoDefinida,
                            EstadoId = (int)Infrastructure.Enum.Estado.RolAsignado,
                            NumeroSocios = 0,
                            NumeroSociosHombres = 0,
                            NumeroSociosMujeres = 0,
                            EsGeneroFemenino = false,
                            EsImportanciaEconomica = false
                        };
                    }
                }

                //en el caso de un proceso de constitucion, asociar nueva organización
                else if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.ConstitucionOP)
                {
                    //si no vienen datos, crear una nueva organizacion
                    proceso.Organizacion = new Organizacion()
                    {
                        FechaCreacion = DateTime.Now,
                        TipoOrganizacionId = (int)Infrastructure.Enum.TipoOrganizacion.AunNoDefinida,
                        EstadoId = (int)Infrastructure.Enum.Estado.RolAsignado,
                        NumeroSocios = 0,
                        NumeroSociosHombres = 0,
                        NumeroSociosMujeres = 0,
                        EsGeneroFemenino = false,
                        EsImportanciaEconomica = false
                    };
                }

                if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.IngresoSupervisorAuxiliar)
                {
                    var aux = obj.SupervisorAuxiliars.ToList();
                    var super = context.SupervisorAuxiliars.Find(aux.FirstOrDefault().SupervisorAuxiliarId);

                    foreach (var item in aux)
                    {
                        proceso.SupervisorAuxiliars.Add(super);
                        {
                            super.ProcesoId = obj.ProcesoId;
                            super.RazonSocial = item.RazonSocial;
                            super.Rut = item.Rut;
                            super.DomicilioLegal = item.DomicilioLegal;
                            super.Telefono = item.Telefono;
                            super.CorreoElectronico = item.CorreoElectronico;
                            super.TipoPersonaJuridicaId = item.TipoPersonaJuridica.TipoPersonaJuridicaId;
                            super.Aprobado = item.Aprobado;
                            super.TipoOrganizacionId = (int)DAES.Infrastructure.Enum.TipoOrganizacion.AunNoDefinida;
                        };
                        for (var i = 0; i < item.RepresentanteLegals.Count(); i++)
                        {
                            var repre = context.RepresentantesLegals.Find(item.RepresentanteLegals[i].RepresentanteLegalId);

                            //repre.SupervisorAuxiliarId = super.SupervisorAuxiliarId;

                            repre.Domicilio = item.RepresentanteLegals[i].Domicilio;
                            repre.Nacionalidad = item.RepresentanteLegals[i].Nacionalidad;
                            repre.NombreCompleto = item.RepresentanteLegals[i].NombreCompleto;
                            repre.Profesion = item.RepresentanteLegals[i].Profesion;
                            repre.RUN = item.RepresentanteLegals[i].RUN;
                        }
                        for (var i = 0; i < item.ExtractoAuxiliars.Count(); i++)
                        {
                            var extracto = context.ExtractoAuxiliars.Find(item.ExtractoAuxiliars[i].ExtractoAuxiliarId);

                            //extracto.SupervisorAuxiliarId = super.SupervisorAuxiliarId;

                            extracto.Año = item.ExtractoAuxiliars[i].Año;
                            extracto.ConservadorComercio = item.ExtractoAuxiliars[i].ConservadorComercio;
                            extracto.FechaInscripcion = item.ExtractoAuxiliars[i].FechaInscripcion;
                            extracto.FechaPubliccionDiarioOficial = item.ExtractoAuxiliars[i].FechaPubliccionDiarioOficial;
                            extracto.Foja = item.ExtractoAuxiliars[i].Foja;
                            extracto.Numero = item.ExtractoAuxiliars[i].Numero;
                            extracto.NumeroPublicacionDiarioOficial = item.ExtractoAuxiliars[i].NumeroPublicacionDiarioOficial;
                        }

                        for (var i = 0; i < item.EscrituraConstitucionModificaciones.Count(); i++)
                        {
                            var escritura = context.EscrituraConstitucions.Find(item.EscrituraConstitucionModificaciones[i].EscrituraConstitucionId);

                            //escritura.SupervisorAuxiliarId = super.SupervisorAuxiliarId;
                            escritura.Fecha = item.EscrituraConstitucionModificaciones[i].Fecha;
                            escritura.Notaria = item.EscrituraConstitucionModificaciones[i].Notaria;
                            escritura.NumeroRepertorio = item.EscrituraConstitucionModificaciones[i].NumeroRepertorio;
                        }
                        for (var i = 0; i < item.PersonaFacultadas.Count(); i++)
                        {
                            var facul = context.PersonaFacultadas.Find(item.PersonaFacultadas[i].PersonaFacultadaId);
                            //facul.SupervisorAuxiliarId = super.SupervisorAuxiliarId;
                            facul.Domicilio = item.PersonaFacultadas[i].Domicilio;
                            facul.Nacionalidad = item.PersonaFacultadas[i].Nacionalidad;
                            facul.NombreCompleto = item.PersonaFacultadas[i].NombreCompleto;
                            facul.Profesion = item.PersonaFacultadas[i].Profesion;
                            facul.RUN = item.PersonaFacultadas[i].RUN;
                        }
                    }
                }

                if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.ActualizacionSupervisorAuxiliar)
                {
                    var aux = obj.SupervisorAuxiliars.ToList();
                    var super = context.SupervisorAuxiliars.Find(aux.FirstOrDefault().SupervisorAuxiliarId);

                    foreach (var item in aux)
                    {
                        var UpSuper = context.ActualizacionSupervisors.Add(new ActualizacionSupervisor()
                        {
                            SupervisorAuxiliarId = item.SupervisorAuxiliarId,
                            Rut = item.Rut,
                            RazonSocial = item.RazonSocial,
                            CorreoElectronico = item.CorreoElectronico,
                            Aprobado = item.Aprobado,
                            Telefono = item.Telefono,
                            TipoPersonaJuridicaId = item.TipoPersonaJuridicaId,
                            DomicilioLegal = item.DomicilioLegal,
                            ProcesoId = obj.ProcesoId,
                            TipoOrganizacionId = item.TipoOrganizacionId
                        });

                        foreach (var UpRepre in item.RepresentanteLegals)
                        {
                            context.ActualizacionRepresentantes.Add(new ActualizacionRepresentante()
                            {
                                Domicilio = UpRepre.Domicilio,
                                NombreCompleto = UpRepre.NombreCompleto,
                                Nacionalidad = UpRepre.Nacionalidad,
                                Profesion = UpRepre.Profesion,
                                RUN = UpRepre.RUN,
                                RepresentanteLegalId = UpRepre.RepresentanteLegalId,
                                Habilitado = true,
                                Eliminado = false,
                                ActualizacionSupervisorId = UpSuper.ActualizacionSupervisorId
                            });
                        }

                        for (var i = 0; i < item.PersonaFacultadas.Count(); i++)
                        {
                            context.ActualizacionPersonaFacultadas.Add(new ActualizacionPersonaFacultada()
                            {
                                Domicilio = item.PersonaFacultadas[i].Domicilio,
                                Nacionalidad = item.PersonaFacultadas[i].Nacionalidad,
                                NombreCompleto = item.PersonaFacultadas[i].NombreCompleto,
                                Profesion = item.PersonaFacultadas[i].Profesion,
                                RUN = item.PersonaFacultadas[i].RUN,
                                PersonaFacultadaId = item.PersonaFacultadas[i].PersonaFacultadaId,
                                ActualizacionSupervisorId = UpSuper.ActualizacionSupervisorId,
                                Habilitado = true
                            });
                        }
                    }
                }

                //en el caso de un proceso de estudio socioeconomico 
                //if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.EstudioSocioEconomicos)
                //{

                if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.EstudioSocioEconomicos)
                {
                    foreach (var item in obj.EstudioSocioEconomicos)
                    {
                        proceso.EstudioSocioEconomicos.Add(new EstudioSocioEconomico
                        {
                            FechaCreacion = item.FechaCreacion,
                            DocumentoAdjunto = item.DocumentoAdjunto,
                            Proceso = proceso
                        });

                    }
                    //si viene datos de una organizacion, usarlos para crea la nueva
                    if (obj.Organizacion != null)
                    {
                        proceso.Organizacion = obj.Organizacion;
                    }

                    //si no vienen datos, crear una nueva organizacion
                    proceso.Organizacion = new Organizacion()
                    {

                        RazonSocial = obj.Organizacion.RazonSocial,
                        RubroId = obj.Organizacion.RubroId,
                        SubRubroId = obj.Organizacion.SubRubroId,
                        RUT = obj.Organizacion.RUT,
                        Sigla = obj.Organizacion.Sigla,
                        Fono = obj.Organizacion.Fono,
                        Email = obj.Organizacion.Email,
                        Direccion = obj.Organizacion.Direccion,
                        RegionId = obj.Organizacion.RegionId,
                        ComunaId = obj.Organizacion.ComunaId,
                        FechaCreacion = DateTime.Now,
                        SituacionId = (int)Infrastructure.Enum.Situacion.Inactiva,
                        TipoOrganizacionId = 1,
                        EstadoId = (int)Infrastructure.Enum.Estado.RolAsignado,
                        EsGeneroFemenino = false,
                        EsImportanciaEconomica = false
                    };
                }
                //en el caso de un proceso distinto de constitucion asignar organizacion seleccionada
                else
                {
                    if (obj.OrganizacionId != null)
                    {

                        proceso.Organizacion = context.Organizacion.FirstOrDefault(q => q.OrganizacionId == obj.OrganizacionId);
                    }
                }


                if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.ConstitucionWeb)
                {
                    proceso.Organizacion = new Organizacion()
                    {

                        RazonSocial = obj.Organizacion.RazonSocial,
                        RubroId = obj.Organizacion.RubroId,
                        SubRubroId = obj.Organizacion.SubRubroId,
                        RUT = obj.Organizacion.RUT,
                        Sigla = obj.Organizacion.Sigla,
                        Fono = obj.Organizacion.Fono,
                        Email = obj.Organizacion.Email,
                        Direccion = obj.Organizacion.Direccion,
                        RegionId = obj.Organizacion.RegionId,
                        ComunaId = obj.Organizacion.ComunaId,
                        FechaCreacion = DateTime.Now,
                        SituacionId = (int)Infrastructure.Enum.Situacion.Inactiva,
                        TipoOrganizacionId = 1,
                        EstadoId = (int)Infrastructure.Enum.Estado.RolAsignado,
                        EsGeneroFemenino = false,
                        EsImportanciaEconomica = false
                    };

                }


                //}
                //    else
                //{
                //    //sino pasa a crear la organización con los datos enviados.
                //    proceso.Organizacion = new Organizacion()
                //    {

                //        RazonSocial = obj.Organizacion.RazonSocial,
                //        RubroId = obj.Organizacion.RubroId,
                //        SubRubroId = obj.Organizacion.SubRubroId,
                //        RUT = obj.Organizacion.RUT,
                //        Sigla = obj.Organizacion.Sigla,
                //        Fono = obj.Organizacion.Fono,
                //        Email = obj.Organizacion.Email,
                //        Direccion = obj.Organizacion.Direccion,
                //        RegionId = obj.Organizacion.RegionId,
                //        ComunaId = obj.Organizacion.ComunaId,
                //        FechaCreacion = DateTime.Now,
                //        SituacionId = (int)Infrastructure.Enum.Situacion.Inactiva,
                //        TipoOrganizacionId = 1,
                //        EstadoId = (int)Infrastructure.Enum.Estado.RolAsignado,
                //        EsGeneroFemenino = false,
                //        EsImportanciaEconomica = false
                //    };
                //}


                //cooperativa de vivienda abierta
                //
                if (obj.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.CooperativaViviendaAbierta)
                {
                    if (obj.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.CooperativaViviendaAbierta)
                    {
                        foreach (var item in obj.CooperativaAbiertas)
                        {
                            proceso.CooperativaAbiertas.Add(new CooperativaAbierta
                            {
                                FechaCreacion = item.FechaCreacion,
                                DocumentoAdjunto = item.DocumentoAdjunto,
                                Proceso = proceso
                            });

                        }
                        //si viene datos de una organizacion, usarlos para crea la nueva
                        if (obj.Organizacion != null)
                        {
                            proceso.Organizacion = obj.Organizacion;
                        }

                        //si no vienen datos, crear una nueva organizacion
                        if (obj.Organizacion == null)
                        {
                            proceso.Organizacion = new Organizacion()
                            {
                                FechaCreacion = DateTime.Now,
                                TipoOrganizacionId = (int)Infrastructure.Enum.TipoOrganizacion.AunNoDefinida,
                                EstadoId = (int)Infrastructure.Enum.Estado.RolAsignado,
                                NumeroSocios = 0,
                                NumeroSociosHombres = 0,
                                NumeroSociosMujeres = 0,
                                EsGeneroFemenino = false,
                                EsImportanciaEconomica = false
                            };
                        }
                    }
                    else
                    {
                        proceso.Organizacion = context.Organizacion.FirstOrDefault(q => q.OrganizacionId == obj.OrganizacionId);
                    }
                }

                //en el caso de que sea certificado automático, generar pdf firmado
                if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoAutomatico)
                {
                    var doc = obj.Documentos.FirstOrDefault();
                    var tipodoc = context.TipoDocumento.FirstOrDefault(q => q.TipoDocumentoId == doc.TipoDocumentoId);

                    var documento = context.Documento.Add(new Documento()
                    {
                        Firmante = context.Firmante.FirstOrDefault(q => q.EsActivo),
                        TipoDocumentoId = doc.TipoDocumentoId,
                        Organizacion = proceso.Organizacion,
                        Proceso = proceso,
                        FechaValidoHasta = tipodoc.DiasVigencia.HasValue ? DateTime.Now.AddDays(tipodoc.DiasVigencia.Value) : (DateTime?)null,
                        TipoPrivacidadId = (int)Infrastructure.Enum.TipoPrivacidad.Publico,
                    });

                    proceso.Documentos.Add(documento);
                }

                //si el proceso trae actualizaciones de datos agregarlas
                if (obj.ActualizacionOrganizacions.Any())
                {
                    proceso.ActualizacionOrganizacions.Add(obj.ActualizacionOrganizacions.FirstOrDefault());

                }

                //si el proceso trae articulo 91, asociarlo
                if (obj.Articulo91s.Any())
                {
                    proceso.Articulo91s.Add(obj.Articulo91s.FirstOrDefault());
                }

                //asignar tareas a ejecutar
                var definicionworkflow = context.DefinicionWorkflow.Where(q => q.Habilitado && q.DefinicionProcesoId == proceso.DefinicionProceso.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId).FirstOrDefault();
                if (definicionworkflow != null)
                {
                    //si se asignó a usuario especifico cambiar, de lo contrario asignar a usuario designado en el diseño del proceso
                    var workflow = new Workflow()
                    {
                        FechaCreacion = DateTime.Now,
                        TipoAprobacionId = (int)Infrastructure.Enum.TipoAprobacion.SinAprobacion,
                        Terminada = false,
                        DefinicionWorkflow = definicionworkflow,
                        Proceso = proceso,
                        PerfilId = definicionworkflow.PerfilId,
                        UserId = !string.IsNullOrWhiteSpace(proceso.UserId) ? proceso.UserId : definicionworkflow.UserId
                    };

                    //si trae documento desde gestion documental, asociarlo
                    if (proceso.DocumentoId > 0)
                    {
                        foreach (var adjunto in gestionDocumentalContext.Adjunto.Where(q => q.IdRegistro == proceso.DocumentoId))
                        {
                            workflow.Documentos.Add(new Documento()
                            {
                                Autor = adjunto.Adj_LoginUsuario,
                                Descripcion = adjunto.Adj_Descripcion,
                                FechaCreacion = adjunto.Adj_FechaCreacion,
                                FileName = adjunto.Adj_Nombre,
                                Url = adjunto.Adj_Url,
                                TipoPrivacidadId = (int)Infrastructure.Enum.TipoPrivacidad.Publico
                            });
                        }
                    }

                    //si es un trámite en linea y viene con documentos adjuntos, asociarlos al workflow
                    if (obj != null && obj.Documentos.Any())
                    {
                        foreach (var adjunto in obj.Documentos)
                        {
                            var tipodocumento = context.TipoDocumento.FirstOrDefault(q => q.TipoDocumentoId == adjunto.TipoDocumentoId);
                            if (tipodocumento != null)
                            {
                                workflow.Documentos.Add(new Documento()
                                {
                                    Organizacion = proceso.Organizacion,
                                    Proceso = proceso,
                                    Autor = adjunto.Autor,
                                    FechaCreacion = adjunto.FechaCreacion,
                                    FileName = adjunto.FileName,
                                    Content = adjunto.Content,
                                    TipoDocumentoId = adjunto.TipoDocumentoId,
                                    NumeroOficio = adjunto.NumeroOficio,
                                    FechaSalidaOficio = adjunto.FechaSalidaOficio,
                                    TipoPrivacidadId = adjunto.TipoPrivacidadId,
                                    Periodo = adjunto.Periodo
                                });
                            }
                        }
                    }

                    //en el caso de que sea certificado manual, agregar comentario al proceso y workflow
                    if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoManual || proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoAutomatico)
                    {
                        var documento = obj.Documentos.FirstOrDefault();
                        if (documento != null)
                        {
                            var tipodocumento = context.TipoDocumento.Find(documento.TipoDocumentoId);
                            if (tipodocumento != null)
                            {
                                proceso.Observacion = tipodocumento.Nombre;
                                workflow.Observacion = tipodocumento.Nombre;
                            }
                        }
                    }

                    //agregar workflow al proceso
                    proceso.Workflows.Add(workflow);
                }

                if (proceso.DefinicionProceso.DefinicionProcesoId != (int)Infrastructure.Enum.DefinicionProceso.Fiscalizacion)
                {
                    //si no existen tareas a asignar, terminar proceso
                    if (definicionworkflow == null)
                    {
                        proceso.FechaTermino = DateTime.Now;
                        proceso.Terminada = true;
                    }
                }

                //guardar proceso
                context.Proceso.Add(proceso);
                context.SaveChanges();



                //en el caso de que sea certificado automático, generar pdf firmado
                if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoAutomatico)
                {

                    var documento = proceso.Documentos.FirstOrDefault();
                    var firmantes = db.Firmante.First(q => q.EsActivo == true);

                    //var firmanteMail = sg.GetUserByRut(/*firmantes.IdFirma*/16366481);
                    //var firEmail = "";
                    var diso = context.Disolucions.FirstOrDefault(q => q.OrganizacionId == proceso.Organizacion.OrganizacionId && q.Anterior != null);
                    if (documento.TipoDocumentoId == (int)Infrastructure.Enum.TipoDocumento.CertificadoDisolución)
                    {
                        if(proceso.Organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.Cooperativa)
                        {
                            var configuracionCertificado = context.ConfiguracionCertificado.FirstOrDefault(q => q.TipoDocumentoId == obj.TipoCertificadoId && q.TipoOrganizacionId == proceso.Organizacion.TipoOrganizacionId && q.anterior == diso.Anterior);
                            documento.Content = CrearCertificadoPDF(configuracionCertificado, proceso.Organizacion, documento.Firmante, documento.DocumentoId, documento.TipoDocumentoId);
                        }
                        else
                        {
                            var configuracionCertificado = context.ConfiguracionCertificado.FirstOrDefault(q => q.TipoDocumentoId == obj.TipoCertificadoId && q.TipoOrganizacionId == proceso.Organizacion.TipoOrganizacionId);
                            documento.Content = CrearCertificadoPDF(configuracionCertificado, proceso.Organizacion, documento.Firmante, documento.DocumentoId, documento.TipoDocumentoId);
                        }
                    }
                    else
                    {
                        if (documento.TipoDocumentoId == (int)Infrastructure.Enum.TipoDocumento.Estatutos)
                        {
                            if (proceso.Organizacion.TipoOrganizacionId == (int)Infrastructure.Enum.TipoOrganizacion.Cooperativa)
                            {
                                if (proceso.Organizacion.ExistenciaAnteriors.Any())
                                {
                                    var configuracionCertificado = context.ConfiguracionCertificado.FirstOrDefault(q => q.TipoDocumentoId == obj.TipoCertificadoId && q.TipoOrganizacionId == proceso.Organizacion.TipoOrganizacionId && q.anterior == true);
                                    documento.Content = CrearCertificadoPDF(configuracionCertificado, proceso.Organizacion, documento.Firmante, documento.DocumentoId, documento.TipoDocumentoId);
                                }
                                else if(proceso.Organizacion.ExistenciaPosteriors.Any())
                                {
                                    var configuracionCertificado = context.ConfiguracionCertificado.FirstOrDefault(q => q.TipoDocumentoId == obj.TipoCertificadoId && q.TipoOrganizacionId == proceso.Organizacion.TipoOrganizacionId && q.anterior == false);
                                    documento.Content = CrearCertificadoPDF(configuracionCertificado, proceso.Organizacion, documento.Firmante, documento.DocumentoId, documento.TipoDocumentoId);
                                }
                                else
                                {
                                    throw new Exception(string.Format("Error al emitir, el apartado de legales no existe"));

                                }
                            }
                            else
                            {
                                    var configuracionCertificado = context.ConfiguracionCertificado.FirstOrDefault(q => q.TipoDocumentoId == obj.TipoCertificadoId && q.TipoOrganizacionId == proceso.Organizacion.TipoOrganizacionId);
                                    documento.Content = CrearCertificadoPDF(configuracionCertificado, proceso.Organizacion, documento.Firmante, documento.DocumentoId, documento.TipoDocumentoId);                               
                            }

                        }
                        else
                        {
                            var configuracionCertificado = context.ConfiguracionCertificado.FirstOrDefault(q => q.TipoDocumentoId == obj.TipoCertificadoId && q.TipoOrganizacionId == proceso.Organizacion.TipoOrganizacionId);
                            documento.Content = CrearCertificadoPDF(configuracionCertificado, proceso.Organizacion, documento.Firmante, documento.DocumentoId, documento.TipoDocumentoId);
                        }
                    }




                    //documento.Content = SignPDF(documento.DocumentoId, documento.NumeroFolio, documento.Content, documento.DocumentoId.ToString(), documento.Firmante, false, documento.TipoDocumentoId, proceso.Organizacion.TipoOrganizacionId);
                    var objDoc = db.Documento.Where(q => q.DocumentoId == documento.DocumentoId).First();
                    var procesoId = documento.ProcesoId;
                    var a = SignResoAuto(documento, /*"jmontesl@economia.cl"*/firmantes.Nombre, documento.ProcesoId.Value);
                    documento.Content = a;
                    documento.FileName = string.Concat(documento.DocumentoId, ".pdf");
                    documento.Firmado = true;



                    context.SaveChanges();
                }

                //en el caso de que el proceso sea de generar el certificado de TEST
                //if (proceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.ConfiguracionCertificadoTEST)
                //{
                //    var documento = obj.Documentos.FirstOrDefault();
                //    var configuracionCertificado = context.ConfiguracionCertificado.FirstOrDefault(q => q.TipoDocumentoId == documento.TipoDocumentoId && q.TipoOrganizacionId == proceso.Organizacion.TipoOrganizacionId);
                //    documento.Content = CrearCertificadoTEST(configuracionCertificado, proceso.Organizacion, documento.Firmante, documento.DocumentoId, documento.TipoDocumentoId);
                //    documento.FileName = string.Concat(documento.DocumentoId, ".pdf");

                //    context.SaveChanges();

                //    //var documento = proceso.Documentos.FirstOrDefault() == null ? new Documento() 
                //    //                { TipoDocumentoId = (int)Infrastructure.Enum.TipoDocumento.OficioTEST } : null;
                //    //var configuracionCertificado = context.ConfiguracionCertificado.FirstOrDefault();


                //}

                //notificar al solicitante via correo el inicio del proceso
                NotificarProceso(proceso, proceso.Solicitante.Email);

                //en el caso de que sea certificado manual, notificar a certificadosaes@economia.cl
                var configcopia = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.NotificarSolicitudCertificadoManual);
                if (proceso.DefinicionProceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoManual)
                {
                    if (configcopia != null && !string.IsNullOrWhiteSpace(configcopia.Valor))
                    {
                        NotificarProceso(proceso, configcopia.Valor);
                    }
                }

                //notificar via correo la ejecución del primer workflow
                if (proceso.Workflows.Any())
                {
                    NotificarTarea(proceso.Workflows.FirstOrDefault().WorkflowId);
                }

                return proceso;
            }
        }

        public void ProcesoUpdate(Workflow w, int? DefinicionWorkflowId)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                if (w == null)
                {
                    throw new Exception("Debe especificar un workflow.");
                }

                var workflow = context.Workflow.FirstOrDefault(q => q.WorkflowId == w.WorkflowId);
                if (workflow == null)
                {
                    throw new Exception("No se encontró el workflow.");
                }

                var proceso = context.Proceso.FirstOrDefault(q => q.ProcesoId == workflow.ProcesoId);
                if (proceso == null)
                {
                    throw new Exception("No se encontró el proceso asociado al workflow.");
                }

                var definicionproceso = context.DefinicionProceso.FirstOrDefault(q => q.DefinicionProcesoId == proceso.DefinicionProcesoId);
                if (definicionproceso == null)
                {
                    throw new Exception("No se encontró la definición de proceso asociado al workflow.");
                }

                var definicionworkflowlist = context.DefinicionWorkflow.Where(q => q.Habilitado && q.DefinicionProcesoId == proceso.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId);
                if (!definicionworkflowlist.Any())
                {
                    throw new Exception("No se encontró la definición de tarea del proceso asociado al workflow.");
                }

                //terminar tarea actual
                workflow.TipoAprobacionId = w.TipoAprobacionId;
                workflow.FechaTermino = DateTime.Now;
                workflow.Observacion = w.Observacion;
                workflow.Terminada = true;

                //si la tarea es despacho de documentos, notificar a jefatura
                if (workflow.DefinicionWorkflow.TipoWorkflow.Nombre.ToUpper() == "ARCHIVAR")
                {
                    NotificarArchivo(workflow.WorkflowId);
                }

                // Si la tarea es de registro de supervisor auxiliar y es archivar documentos, cambiar estado para que pase al flujo de actualizar
                if (workflow.Proceso.DefinicionProcesoId == (int)DAES.Infrastructure.Enum.DefinicionProceso.IngresoSupervisorAuxiliar)
                {
                    var Defwork = context.DefinicionWorkflow.Find(workflow.DefinicionWorkflowId);
                    var proc = context.Proceso.Find(workflow.ProcesoId);
                    var super = context.SupervisorAuxiliars.Where(q => q.ProcesoId == proc.ProcesoId).ToList();

                    if (Defwork.TipoWorkflowId == 28)
                    {
                        foreach (var item in super)
                        {
                            item.Aprobado = true;
                        }
                        context.SaveChanges();
                    }
                }

                //si hay tareas paralelas pendientes de aprobacion, guardar y salir
                var tareasparalelas = context.Workflow.Where(q => q.WorkflowId != workflow.WorkflowId && q.WorkflowGrupoId != null && q.WorkflowGrupoId == workflow.WorkflowGrupoId);
                if (tareasparalelas.Any() && tareasparalelas.Any(q => q.TipoAprobacionId == (int)Infrastructure.Enum.TipoAprobacion.SinAprobacion))
                {
                    context.SaveChanges();
                    return;
                }

                //determinar estado de tarea(s)
                var aprobado = false;

                if (tareasparalelas.Any())
                {
                    aprobado = tareasparalelas.All(q => q.TipoAprobacionId == (int)Infrastructure.Enum.TipoAprobacion.Aprobada) && w.TipoAprobacionId == (int)Infrastructure.Enum.TipoAprobacion.Aprobada;
                }
                else
                {
                    aprobado = w.TipoAprobacionId == (int)Infrastructure.Enum.TipoAprobacion.Aprobada;
                }

                //determinar siguiente tarea en base a estado y definicion de proceso
                DefinicionWorkflow siguientedefinicionworkflow = null;
                if (DefinicionWorkflowId != null)
                {
                    siguientedefinicionworkflow = context.DefinicionWorkflow.FirstOrDefault(q => q.DefinicionWorkflowId == DefinicionWorkflowId);
                }
                else if (workflow == null)
                {
                    siguientedefinicionworkflow = definicionworkflowlist.FirstOrDefault();
                }
                else if (workflow != null && aprobado)
                {
                    siguientedefinicionworkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflow.DefinicionWorkflow.Secuencia);
                }
                else if (workflow != null && !aprobado)
                {
                    siguientedefinicionworkflow =
                        definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflow.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                }

                //Si la tarea se rechaza, se vuelve a la anterior
                if (w.TipoAprobacionId == (int)Infrastructure.Enum.TipoAprobacion.Rechazada)
                {

                }

                // si existe siguiente tarea, agregarla
                var nuevastareas = new List<Workflow>();
                if (siguientedefinicionworkflow != null)
                {
                    //si la tarea fue rechazada y la tarea anterior es asignable. buscar la persona asignada
                    if (string.IsNullOrWhiteSpace(siguientedefinicionworkflow.UserId) && !aprobado)
                    {
                        var wfa = proceso.Workflows.OrderByDescending(q => q.WorkflowId).FirstOrDefault(q => q.DefinicionWorkflowId == siguientedefinicionworkflow.DefinicionWorkflowId);
                        if (wfa != null)
                            siguientedefinicionworkflow.UserId = wfa.UserId;
                    }
                    //verificar que exista un usuario para asignar tarea
                    if (string.IsNullOrWhiteSpace(workflow.NextUserId) && string.IsNullOrWhiteSpace(siguientedefinicionworkflow.UserId))
                    {
                        throw new Exception(string.Format("La siguiente tarea '{0}' no tiene un usuario asignado. Favor revise la definición del proceso.", siguientedefinicionworkflow.TipoWorkflow.Nombre));
                    }

                    //determinar lista de usuarios de la siguiente tarea
                    var listausuarios = !string.IsNullOrWhiteSpace(workflow.NextUserId) ? workflow.NextUserId.Split(',') : siguientedefinicionworkflow.UserId.Split(',');


                    //20102022
                    var definicion_wor = db.DefinicionWorkflow.Where(q => q.DefinicionWorkflowId == w.DefinicionWorkflowId).ToList();
                    var definicion_pro = db.DefinicionProceso.Where(q => q.DefinicionProcesoId == definicion_wor.First().DefinicionProcesoId);

                    //asignar tareas a la lista de usuarios
                    foreach (var userid in listausuarios)
                    {
                        //validar usuario
                        var user = context.Users.FirstOrDefault(q => q.Id == userid);
                        if (user == null)
                        {
                            throw new Exception("No se encontró el usuario id: " + userid);
                        }

                        //crear nueva tarea
                        var nuevoworkflow = new Workflow();
                        nuevoworkflow.FechaCreacion = DateTime.Now;
                        nuevoworkflow.TipoAprobacionId = (int)Infrastructure.Enum.TipoAprobacion.SinAprobacion;
                        nuevoworkflow.Terminada = false;
                        nuevoworkflow.DefinicionWorkflow = siguientedefinicionworkflow;
                        nuevoworkflow.Proceso = proceso;
                        nuevoworkflow.PerfilId = siguientedefinicionworkflow.PerfilId;

                        //TODO: se modifica esto para hacer efectivo el cambio de destino
                        if (w.UserId != null)
                        {
                            nuevoworkflow.UserId = w.UserId;
                        }
                        else
                        {
                            nuevoworkflow.UserId = w.UserId != null ? workflow.UserId : userid;
                        }

                        //nuevoworkflow.UserId = w.UserId != null ? workflow.UserId : userid;

                        //si es mas de 1 usuario agrupar las tareas y hacerlas paralelas
                        if (listausuarios.Count() > 1)
                        {
                            nuevoworkflow.WorkflowGrupoId = workflow.WorkflowId;
                        }

                        //agregar tarea
                        proceso.Workflows.Add(nuevoworkflow);

                        //acumular tarea para notificar
                        nuevastareas.Add(nuevoworkflow);
                    }
                }

                // si no existe siguiente tarea, terminar proceso
                if (siguientedefinicionworkflow == null)
                {
                    proceso.Terminada = true;
                    proceso.FechaTermino = DateTime.Now;
                }

                //guardar cambios
                context.SaveChanges();

                //notificar de tarea via correo
                if (nuevastareas.Any())
                {
                    foreach (var item in nuevastareas)
                    {
                        NotificarTarea(item.WorkflowId);
                    }
                }
            }
        }

        public void CambioDeBandeja(Workflow w, int? DefinicionWorkflowId)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                if (w == null)
                {
                    throw new Exception("Debe especificar un workflow.");
                }

                var workflow = context.Workflow.FirstOrDefault(q => q.WorkflowId == w.WorkflowId);
                if (workflow == null)
                {
                    throw new Exception("No se encontró el workflow.");
                }

                var proceso = context.Proceso.FirstOrDefault(q => q.ProcesoId == workflow.ProcesoId);
                if (proceso == null)
                {
                    throw new Exception("No se encontró el proceso asociado al workflow.");
                }

                var definicionproceso = context.DefinicionProceso.FirstOrDefault(q => q.DefinicionProcesoId == proceso.DefinicionProcesoId);
                if (definicionproceso == null)
                {
                    throw new Exception("No se encontró la definición de proceso asociado al workflow.");
                }

                var definicionworkflowlist = context.DefinicionWorkflow.Where(q => q.Habilitado && q.DefinicionProcesoId == proceso.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId);
                if (!definicionworkflowlist.Any())
                {
                    throw new Exception("No se encontró la definición de tarea del proceso asociado al workflow.");
                }

                //terminar tarea actual
                workflow.Observacion = w.Observacion;
                workflow.UserId = w.UserId;

                //si la tarea es despacho de documentos, notificar a jefatura
                if (workflow.DefinicionWorkflow.TipoWorkflow.Nombre.ToUpper() == "ARCHIVAR")
                {
                    NotificarArchivo(workflow.WorkflowId);
                }

                //Notificar tarea a nuevo ejecutante
                NotificarTarea(w.WorkflowId);

                context.SaveChanges();
            }
        }

        public void ProcessDefinitionUpdate(int DefinicionProcesoId, string ids)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var idarray = ids.Split(',').Where(q => !string.IsNullOrWhiteSpace(q.ToString())).ToArray();
                for (int i = 0; i < idarray.Count(); i++)
                {
                    int id = int.Parse(idarray[i]);
                    var dpd = context.DefinicionWorkflow.FirstOrDefault(q => q.DefinicionWorkflowId == id);
                    dpd.Secuencia = i;
                }

                context.SaveChanges();
            }
        }

        public void WorkflowAssign(Workflow w, List<Model.DTO.DTOUser> users)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                //actualizar workflow actual
                var workflow = context.Workflow.FirstOrDefault(q => q.WorkflowId == w.WorkflowId);
                if (workflow == null)
                {
                    throw new Exception("Debe especificar un workflow.");
                }

                //asignar lista de usuarios
                workflow.NextUserId = string.Join(",", users.Where(q => q.Selected).Select(q => q.Id).ToArray());

                context.SaveChanges();
            }
        }

        public void ProcesoDelete(int id)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {

                var proceso = context.Proceso.FirstOrDefault(q => q.ProcesoId == id);
                if (proceso == null)
                    throw new Exception("No se encontró el proceso.");

                foreach (var documento in context.Documento.Where(q => q.ProcesoId == proceso.ProcesoId))
                    context.Documento.Remove(documento);

                foreach (var workflow in context.Workflow.Where(q => q.ProcesoId == id))
                    foreach (var documento in context.Documento.Where(q => q.WorkflowId == workflow.WorkflowId))
                        context.Documento.Remove(documento);

                foreach (var workflow in context.Workflow.Where(q => q.ProcesoId == id))
                    context.Workflow.Remove(workflow);

                foreach (var art91 in context.Articulo91.Where(q => q.ProcesoId == proceso.ProcesoId))
                    context.Articulo91.Remove(art91);

                foreach (var hallazgo in context.Hallazgo.Where(q => q.Fiscalizacion.ProcesoId == proceso.ProcesoId))
                    context.Hallazgo.Remove(hallazgo);

                foreach (var fiscalizacion in context.Fiscalizacion.Where(q => q.ProcesoId == proceso.ProcesoId))
                    context.Fiscalizacion.Remove(fiscalizacion);

                context.Proceso.Remove(proceso);
                context.SaveChanges();
            }
        }

        public void DefinicionProcesoDelete(int id)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var definicionproceso = context.DefinicionProceso.FirstOrDefault(q => q.DefinicionProcesoId == id);
                if (definicionproceso == null)
                {
                    throw new Exception("No se encontró la definición de proceso.");
                }

                context.Workflow.RemoveRange(context.Workflow.Where(q => q.Proceso.DefinicionProcesoId == definicionproceso.DefinicionProcesoId));
                context.Documento.RemoveRange(context.Documento.Where(q => q.Workflow.Proceso.DefinicionProcesoId == definicionproceso.DefinicionProcesoId));
                context.Proceso.RemoveRange(context.Proceso.Where(q => q.DefinicionProcesoId == definicionproceso.DefinicionProcesoId));
                context.DefinicionWorkflow.RemoveRange(context.DefinicionWorkflow.Where(q => q.DefinicionProcesoId == definicionproceso.DefinicionProcesoId));
                context.DefinicionProceso.Remove(definicionproceso);

                context.SaveChanges();
            }
        }

        public List<string> DefinicionWorkflowDeleteValidate(int id)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();
                var definicioworkflow = context.DefinicionWorkflow.FirstOrDefault(q => q.DefinicionWorkflowId == id);
                if (definicioworkflow == null)
                {
                    returnValue.Add("No se encontró la definición de workflow.");
                }

                if (definicioworkflow.Workflows.Any())
                {
                    returnValue.Add("No se puede eliminar está definición ya que existen tareas que se generaron a partir de ella.");
                }

                if (context.DefinicionWorkflow.Any(q => q.DefinicionWorkflowRechazoId == definicioworkflow.DefinicionWorkflowId))
                {
                    returnValue.Add("No se puede eliminar está definición ya que es la instancia de rechazo de otras tareas. Revise la definición de su proceso.");
                }

                context.DefinicionWorkflow.Remove(definicioworkflow);

                return returnValue;
            }
        }

        public void DefinicionWorkflowDelete(int id)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var definicioworkflow = context.DefinicionWorkflow.FirstOrDefault(q => q.DefinicionWorkflowId == id);
                if (definicioworkflow == null)
                {
                    throw new Exception("No se encontró la definición de workflow.");
                }

                context.Workflow.RemoveRange(context.Workflow.Where(q => q.DefinicionWorkflowId == definicioworkflow.DefinicionWorkflowId));
                context.DefinicionWorkflow.Remove(definicioworkflow);
                context.SaveChanges();
            }
        }

        public void WorkflowMove(int workflowid, string userid)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var workflow = context.Workflow.FirstOrDefault(q => q.WorkflowId == workflowid);
                if (workflow == null)
                {
                    throw new Exception("No se encontró el workflow.");
                }

                workflow.UserId = userid;
                workflow.Observacion = "Tarea reasignada por el administrador de sistema";

                context.SaveChanges();

                //notificar de tarea via correo
                NotificarTarea(workflow.WorkflowId);
            }
        }

        public void WorkflowDelete(int workflowid)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var workflow = context.Workflow.FirstOrDefault(q => q.WorkflowId == workflowid);
                if (workflow == null)
                {
                    throw new Exception("No se encontró el workflow.");
                }

                context.Documento.RemoveRange(context.Documento.Where(q => q.Workflow.WorkflowId == workflow.WorkflowId));
                context.Workflow.Remove(workflow);

                context.SaveChanges();
            }
        }

        public List<string> AccountDeleteValidate(string userid)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();
                var user = context.Users.FirstOrDefault(q => q.Id == userid);
                if (user == null)
                {
                    returnValue.Add("Debe especificar un usuario");
                }

                if (user.Workflows.Any(q => !q.Terminada))
                {
                    returnValue.Add("El usuario especificado no se puede eliminar ya que tiene tareas asignadas pendientes de ejecución");
                }

                return returnValue;
            }
        }

        public List<string> UpdateArticulo91(Articulo91 articulo91)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();
                var obj = context.Articulo91.FirstOrDefault(q => q.Articulo91Id == articulo91.Articulo91Id);
                if (obj != null)
                {
                    obj.OK = articulo91.OK;
                    context.SaveChanges();
                }
                return returnValue;
            }
        }

        public List<string> UpdateActaFiscalizacion(ActaFiscalizacion actaFiscalizacion)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var returnValue = new List<string>();

                var actafiscalizacion = context.ActaFiscalizacion.FirstOrDefault(q => q.ActaFiscalizacionId == actaFiscalizacion.ActaFiscalizacionId);
                if (actafiscalizacion != null)
                {
                    actafiscalizacion.CambioDireccion = actaFiscalizacion.CambioDireccion;
                    actafiscalizacion.ComunaId = actaFiscalizacion.ComunaId;
                    actafiscalizacion.DireccionActual = actaFiscalizacion.DireccionActual;
                    actafiscalizacion.Fecha = actaFiscalizacion.Fecha;
                    actafiscalizacion.FechaFiscalizacionInSitu = actaFiscalizacion.FechaFiscalizacionInSitu;
                    actafiscalizacion.FechaSalidaOficioAcreditacionRequerimientos = actaFiscalizacion.FechaSalidaOficioAcreditacionRequerimientos;
                    actafiscalizacion.GeneroGerenteId = actaFiscalizacion.GeneroGerenteId;
                    actafiscalizacion.GeneroRepresentanteLegalId = actaFiscalizacion.GeneroRepresentanteLegalId;
                    actafiscalizacion.Gerente = actaFiscalizacion.Gerente;
                    actafiscalizacion.NActaReunionFiscalizacionInSitu = actaFiscalizacion.NActaReunionFiscalizacionInSitu;
                    actafiscalizacion.NOficioAcreditacioRequerimientos = actaFiscalizacion.NOficioAcreditacioRequerimientos;
                    actafiscalizacion.ObservacionContable = actaFiscalizacion.ObservacionContable;
                    actafiscalizacion.ObservacionLegal = actaFiscalizacion.ObservacionLegal;
                    actafiscalizacion.RegionId = actaFiscalizacion.RegionId;
                    actafiscalizacion.RepresentanteLegal = actaFiscalizacion.RepresentanteLegal;
                    actafiscalizacion.RUT = actaFiscalizacion.RUT;
                    actafiscalizacion.VigenciaRepresentanteLegal = actaFiscalizacion.VigenciaRepresentanteLegal;

                    foreach (var item in actaFiscalizacion.ActaFiscalizacionFiscalizadorContables)
                    {
                        var fiscalizador = context.ActaFiscalizacionFiscalizadorContable.FirstOrDefault(q => q.ActaFiscalizacionFiscalizadorContableId == item.ActaFiscalizacionFiscalizadorContableId);
                        if (fiscalizador != null)
                        {
                            fiscalizador.Seleccionado = item.Seleccionado;
                        }
                    }

                    foreach (var item in actaFiscalizacion.ActaFiscalizacionFiscalizadorLegals)
                    {
                        var fiscalizador = context.ActaFiscalizacionFiscalizadorLegal.FirstOrDefault(q => q.ActaFiscalizacionFiscalizadorLegalId == item.ActaFiscalizacionFiscalizadorLegalId);
                        if (fiscalizador != null)
                        {
                            fiscalizador.Seleccionado = item.Seleccionado;
                        }
                    }

                    foreach (var item in actaFiscalizacion.ActaFiscalizacionHechoContables)
                    {
                        var acta = context.ActaFiscalizacionHechoContable.FirstOrDefault(q => q.ActaFiscalizacionHechoContableId == item.ActaFiscalizacionHechoContableId);
                        if (acta != null)
                        {
                            acta.Observacion = item.Observacion;
                            acta.Periodo = item.Periodo;
                            acta.Requerido = item.Requerido;
                            acta.File = item.File;

                            if (item.File != null)
                            {
                                var file = item.File;
                                var target = new MemoryStream();
                                file.InputStream.CopyTo(target);
                                acta.Archivo = target.ToArray();
                            }
                        }
                    }

                    foreach (var item in actaFiscalizacion.ActaFiscalizacionHechoLegals)
                    {
                        var acta = context.ActaFiscalizacionHechoLegal.FirstOrDefault(q => q.ActaFiscalizacionHechoLegalId == item.ActaFiscalizacionHechoLegalId);
                        if (acta != null)
                        {
                            acta.Observacion = item.Observacion;
                            acta.Requerido = item.Requerido;
                            acta.File = item.File;

                            if (item.File != null)
                            {
                                var file = item.File;
                                var target = new MemoryStream();
                                file.InputStream.CopyTo(target);
                                acta.Archivo = target.ToArray();
                            }
                        }
                    }

                    context.SaveChanges();
                }
                return returnValue;
            }
        }

        public bool NotificarProceso(Proceso proceso, string to)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                if (proceso == null)
                {
                    throw new Exception("Proceso no encontrado.");
                }

                if (string.IsNullOrWhiteSpace(to))
                {
                    throw new Exception("No se encontró el correo electrónico de notificación.");
                }

                var configcorreo = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.PlantillaCorreoNotificacionProceso);
                if (configcorreo == null)
                {
                    throw new Exception("No existe la plantilla de correos manual");
                }

                if (configcorreo != null && configcorreo.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("La plantilla de correos manuales es inválida");
                }

                var configDiasManuales = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.DiasCertificadosManuales);
                if (configDiasManuales == null)
                {
                    throw new Exception("No existe la configuración de dias de respuesta");
                }

                if (configDiasManuales != null && configDiasManuales.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("la configuración de dias de respuesta es inválida");
                }

                configcorreo.Valor = configcorreo.Valor.Replace("[Id]", proceso.ProcesoId.ToString());
                configcorreo.Valor = configcorreo.Valor.Replace("[Tramite]", proceso.DefinicionProceso.Nombre);
                configcorreo.Valor = configcorreo.Valor.Replace("[FechaCreacion]", string.Format("{0:dd-MM-yyyy HH:mm:ss}", proceso.FechaCreacion));
                configcorreo.Valor = configcorreo.Valor.Replace("[FechaVencimiento]", string.Format("{0:dd-MM-yyyy HH:mm:ss}", proceso.FechaVencimiento));
                if (proceso.Organizacion != null)
                {
                    configcorreo.Valor = configcorreo.Valor.Replace("[Organizacion]", proceso.Organizacion.RazonSocial);
                }

                emailMsg.IsBodyHtml = true;
                emailMsg.Body = configcorreo.Valor;

                var configAsunto = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.AsuntoCorreoNotificacion);
                if (configAsunto != null && !configAsunto.Valor.IsNullOrWhiteSpace())
                {
                    emailMsg.Subject = configAsunto.Valor;
                }

                emailMsg.To.Add(to);

                //si el proceso es una solicitud de certificado automático, adjuntar pdf
                if (proceso.DefinicionProcesoId == (int)Infrastructure.Enum.DefinicionProceso.SolicitudCertificadoAutomatico)
                {
                    if (proceso.Documentos.Any())
                    {
                        emailMsg.Attachments.Add(new Attachment(new MemoryStream(proceso.Documentos.FirstOrDefault().Content), proceso.Documentos.FirstOrDefault().FileName));
                    }
                }

                try
                {
                    Send();
                    LogAdd(new Log() { LogId = Guid.NewGuid(), LogTimeLocal = DateTime.Now, LogAreaAccessed = "NotificarProceso", LogTimeUtc = DateTime.UtcNow, LogDetails = "Correo de notificación de proceso enviado a " + emailMsg.To.ToString() + " correctamente.", LogIpAddress = Dns.GetHostName(), LogUserName = Environment.UserName, LogContent = emailMsg.Body });
                }
                catch (Exception ex)
                {
                    LogAdd(new Log() { LogId = Guid.NewGuid(), LogTimeLocal = DateTime.Now, LogAreaAccessed = "NotificarProceso", LogTimeUtc = DateTime.UtcNow, LogDetails = "Se detectó un problema al momento de notificar proceso a " + emailMsg.To.ToString(), LogIpAddress = Dns.GetHostName(), LogUserName = Environment.UserName, LogContent = ex.Message });
                }

                return true;
            }
        }

        public bool NotificarTarea(int workflowid)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var workflow = context.Workflow.FirstOrDefault(q => q.WorkflowId == workflowid);
                if (workflow == null)
                {
                    throw new Exception("No se encontró el workflow especificado");
                }

                var configplantillanotificaciontarea = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.PLantillaNotificacionTarea);
                if (configplantillanotificaciontarea == null)
                {
                    throw new Exception("No existe la plantilla de notificación de tareas");
                }

                if (configplantillanotificaciontarea != null && configplantillanotificaciontarea.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("La plantilla de notificación de tareas está vacia");
                }

                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Id]", workflow.WorkflowId.ToString());
                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[FechaCreacion]", workflow.FechaCreacion.ToString());
                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Tarea]", workflow.DefinicionWorkflow.TipoWorkflow.Nombre);
                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Proceso]", workflow.Proceso.DefinicionProceso.Nombre);
                if (workflow.Proceso.Organizacion != null)
                {
                    configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Registro]", workflow.Proceso.Organizacion.NumeroRegistro);
                    configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Organizacion]", workflow.Proceso.Organizacion.RazonSocial);
                }

                emailMsg.IsBodyHtml = true;
                emailMsg.Body = configplantillanotificaciontarea.Valor;
                emailMsg.To.Clear();

                var configAsunto = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.AsuntoCorreoNotificacion);
                if (configAsunto != null && !configAsunto.Valor.IsNullOrWhiteSpace())
                {
                    emailMsg.Subject = configAsunto.Valor;
                }

                if (workflow.User != null)
                {
                    foreach (var to in workflow.User.EmailNotificacionTarea.Split(';'))
                    {
                        if (!to.IsNullOrWhiteSpace())
                        {
                            emailMsg.To.Add(to);
                        }
                    }

                    try
                    {
                        Send();
                        LogAdd(new Log() { LogId = Guid.NewGuid(), LogTimeLocal = DateTime.Now, LogAreaAccessed = "NotificarTarea", LogTimeUtc = DateTime.UtcNow, LogDetails = "Correo de notificación de tarea enviado a " + emailMsg.To.ToString() + " correctamente.", LogIpAddress = Dns.GetHostName(), LogUserName = Environment.UserName, LogContent = emailMsg.Body });
                    }
                    catch (Exception ex)
                    {
                        LogAdd(new Log() { LogId = Guid.NewGuid(), LogTimeLocal = DateTime.Now, LogAreaAccessed = "NotificarTarea", LogTimeUtc = DateTime.UtcNow, LogDetails = "Se detectó un problema al momento de notificar tarea a " + emailMsg.To.ToString(), LogIpAddress = Dns.GetHostName(), LogUserName = Environment.UserName, LogContent = ex.Message });
                    }
                }

                return true;
            }
        }

        public bool NotificarArchivo(int workflowid)
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var workflow = context.Workflow.FirstOrDefault(q => q.WorkflowId == workflowid);
                if (workflow == null)
                {
                    throw new Exception("No se encontró el workflow especificado");
                }

                var configplantillanotificaciontarea = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.PLantillaNotificacionTareaArchivada);
                if (configplantillanotificaciontarea == null)
                {
                    throw new Exception("No existe la plantilla de notificación de tareas archivadas");
                }

                if (configplantillanotificaciontarea != null && configplantillanotificaciontarea.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("La plantilla de notificación de tareas archivadas está vacia");
                }

                var jefaturas = context.Users.Where(q => q.Habilitado && (q.PerfilId == (int)Infrastructure.Enum.Perfil.Jefatura || q.PerfilId == (int)Infrastructure.Enum.Perfil.Admininstrador));
                if (!jefaturas.Any())
                {
                    throw new Exception("No hay cuentas de usuario de jefatura o administradores a quienes informar");
                }

                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Id]", workflow.WorkflowId.ToString());
                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[FechaCreacion]", workflow.FechaCreacion.ToString());
                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Tarea]", workflow.DefinicionWorkflow.TipoWorkflow.Nombre);
                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Proceso]", workflow.Proceso.DefinicionProceso.Nombre);
                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Registro]", workflow.Proceso.Organizacion.NumeroRegistro);
                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[Organizacion]", workflow.Proceso.Organizacion.RazonSocial);
                configplantillanotificaciontarea.Valor = configplantillanotificaciontarea.Valor.Replace("[idproceso]", workflow.Proceso.ProcesoId.ToString());

                emailMsg.IsBodyHtml = true;
                emailMsg.Body = configplantillanotificaciontarea.Valor;
                emailMsg.To.Clear();

                var configAsunto = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.AsuntoCorreoNotificacion);
                if (configAsunto != null && !configAsunto.Valor.IsNullOrWhiteSpace())
                {
                    emailMsg.Subject = configAsunto.Valor;
                }

                foreach (var to in jefaturas.Select(q => q.Email))
                {
                    if (!to.IsNullOrWhiteSpace())
                    {
                        emailMsg.To.Add(to);
                    }
                }

                try
                {
                    Send();
                    LogAdd(new Log() { LogId = Guid.NewGuid(), LogTimeLocal = DateTime.Now, LogAreaAccessed = "NotificarTareaArchivada", LogTimeUtc = DateTime.UtcNow, LogDetails = "Correo de notificación de tarea archivada enviado a " + emailMsg.To.ToString() + " correctamente.", LogIpAddress = Dns.GetHostName(), LogUserName = Environment.UserName, LogContent = emailMsg.Body });
                }
                catch (Exception ex)
                {
                    LogAdd(new Log() { LogId = Guid.NewGuid(), LogTimeLocal = DateTime.Now, LogAreaAccessed = "NotificarTareaArchivada", LogTimeUtc = DateTime.UtcNow, LogDetails = "Se detectó un problema al momento de notificar tarea archivada a " + emailMsg.To.ToString(), LogIpAddress = Dns.GetHostName(), LogUserName = Environment.UserName, LogContent = ex.Message });
                }

                return true;
            }
        }

        public string GetNotificarProcesosPendientes()
        {
            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                var procesos = context.Proceso.Where(q => !q.Terminada).ToList().Where(q => DateTime.Now.Date > q.FechaVencimiento.Date).ToList();

                if (!procesos.Any())
                {
                    return null;
                }

                var plantillainforme = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.PlantillaInformeProcesosAtrasados);
                if (plantillainforme == null)
                {
                    throw new Exception("No existe la plantilla de informe de procesos atrasados");
                }

                if (plantillainforme != null && plantillainforme.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("La plantilla de informe de procesos atrasados está vacia");
                }

                var plantillainformeDetalle = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.PlantillaInformeProcesosAtrasadosDetalle);
                if (plantillainformeDetalle == null)
                {
                    throw new Exception("No existe la plantilla de informe de procesos atrasados");
                }

                if (plantillainformeDetalle != null && plantillainformeDetalle.Valor.IsNullOrWhiteSpace())
                {
                    throw new Exception("La plantilla de informe de procesos atrasados está vacia");
                }

                var jefaturas = context.Users.Where(q => q.Habilitado && (q.PerfilId == (int)Infrastructure.Enum.Perfil.Jefatura || q.PerfilId == (int)Infrastructure.Enum.Perfil.Admininstrador));
                if (!jefaturas.Any())
                {
                    throw new Exception("No hay cuentas de usuario de jefatura o administradores a quienes informar");
                }

                string detalle = string.Empty;

                foreach (var item in procesos)
                {
                    var tarea = item.Workflows.Where(q => !q.Terminada).OrderBy(q => q.WorkflowId);
                    if (!tarea.ToList().Any())
                    {
                        continue;
                    }

                    string temp = plantillainformeDetalle.Valor;
                    temp = temp.Replace("[Id]", item.ProcesoId.ToString());
                    temp = temp.Replace("[Tipo]", item.DefinicionProceso.Nombre);
                    temp = temp.Replace("[FechaCreacion]", string.Format("{0:dd/MM/yyyy HH:mm:ss}", item.FechaCreacion));
                    temp = temp.Replace("[FechaVencimiento]", string.Format("{0:dd/MM/yyyy HH:mm:ss}", item.FechaVencimiento));
                    temp = temp.Replace("[Organizacion]", item.Organizacion.RazonSocial);
                    temp = temp.Replace("[Registro]", item.Organizacion.NumeroRegistro);
                    temp = temp.Replace("[Tarea]", tarea != null ? tarea.FirstOrDefault().DefinicionWorkflow.TipoWorkflow.Nombre : string.Empty);
                    temp = temp.Replace("[Funcionario]", tarea != null && tarea.FirstOrDefault().User != null ? tarea.FirstOrDefault().User.UserName : string.Empty);
                    detalle = string.Concat(detalle, temp);
                }

                plantillainforme.Valor = plantillainforme.Valor.Replace("[Procesos]", procesos.Count().ToString());
                plantillainforme.Valor = plantillainforme.Valor.Replace("[Detalle]", detalle);

                emailMsg.IsBodyHtml = true;
                emailMsg.Priority = MailPriority.High;
                emailMsg.Body = plantillainforme.Valor;
                emailMsg.To.Clear();

                var configAsunto = context.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.AsuntoCorreoNotificacion);
                if (configAsunto != null && !configAsunto.Valor.IsNullOrWhiteSpace())
                {
                    emailMsg.Subject = configAsunto.Valor;
                }

                foreach (var to in jefaturas)
                {
                    if (!to.Email.IsNullOrWhiteSpace())
                    {
                        emailMsg.To.Add(to.Email);
                    }
                }

                try
                {
                    Send();
                    LogAdd(new Log() { LogId = Guid.NewGuid(), LogTimeLocal = DateTime.Now, LogAreaAccessed = "NotificarProcesosPendientes", LogTimeUtc = DateTime.UtcNow, LogDetails = "Correo de reporte de procesos pendientes enviado a " + emailMsg.To.ToString() + " correctamente.", LogIpAddress = Dns.GetHostName(), LogUserName = Environment.UserName, LogContent = emailMsg.Body });
                }
                catch (Exception ex)
                {
                    LogAdd(new Log() { LogId = Guid.NewGuid(), LogTimeLocal = DateTime.Now, LogAreaAccessed = "NotificarProcesosPendientes", LogTimeUtc = DateTime.UtcNow, LogDetails = "Se detectó un problema al momento de notificar procesos pendientes a " + emailMsg.To.ToString(), LogIpAddress = Dns.GetHostName(), LogUserName = Environment.UserName, LogContent = ex.Message });
                }

                return plantillainforme.Valor;
            }
        }

        //Nuevo metodo de firma 

        private readonly IGestionProcesos _repository;
        private readonly IHsm _hsm;
        private readonly ISIGPER _sigper;
        private readonly IEmail _email;
        private readonly IFolio _folio;
        private readonly IFile _file;

        public Custom(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHsm hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
        }
        public Custom(IGestionProcesos repository)
        {
            _repository = repository;
        }

        public Custom(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }

        public Custom()
        {
        }

        public ResponseMessage Insert(FirmaDocumento obj)
        {
            var response = new ResponseMessage();

            //validaciones

            if (obj == null)
                response.Errors.Add("No se encontró información del archivo a firmar");
            if (obj != null && string.IsNullOrWhiteSpace(obj.TipoDocumentoCodigo))
                response.Errors.Add("No se encontró el dato tipo documento");
            if (obj != null && obj.DocumentoSinFirma == null)
                response.Errors.Add("No se encontró el archivo a firmar");

            if (response.IsValid)
            {
                _repository.Create(obj);
                _repository.Save();
            }

            return response;
        }
        public ResponseMessage Edit(FirmaDocumento obj)
        {
            var response = new ResponseMessage();

            //validaciones
            if (obj == null)
                response.Errors.Add("No se encontró información del archivo a firmar");
            if (obj != null && string.IsNullOrWhiteSpace(obj.TipoDocumentoCodigo))
                response.Errors.Add("No se encontró el dato tipo documento");

            if (response.IsValid)
            {

                var firmaDocumento = _repository.GetFirst<FirmaDocumento>(q => q.FirmaDocumentoId == obj.FirmaDocumentoId);
                if (firmaDocumento != null)
                {
                    if (obj.DocumentoSinFirma != null)
                        firmaDocumento.DocumentoSinFirma = obj.DocumentoSinFirma;

                    if (obj.DocumentoSinFirmaFilename != null)
                        firmaDocumento.DocumentoSinFirmaFilename = obj.DocumentoSinFirmaFilename;

                    firmaDocumento.TipoDocumentoCodigo = obj.TipoDocumentoCodigo;
                    firmaDocumento.TipoDocumentoDescripcion = obj.TipoDocumentoDescripcion;
                    firmaDocumento.URL = obj.URL;
                    firmaDocumento.Observaciones = obj.Observaciones;
                    firmaDocumento.Firmado = false;

                    _repository.Update(firmaDocumento);
                    _repository.Save();
                }
            }

            return response;
        }
        public ResponseMessage Sign(int id, List<string> emailsFirmantes, string firmante)
        {
            var responseCaseUse = new ResponseMessage();
            var persona = new Model.Sigper.SIGPER();
            //var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
            SistemaIntegradoContext context = new SistemaIntegradoContext();

            if (id == 0)
                responseCaseUse.Errors.Add("Id de documento a firmar no encontrado");
            var documentoOriginal = context.Documento.FirstOrDefault(q => q.DocumentoId == id);

            if (documentoOriginal == null)
                responseCaseUse.Errors.Add("Documento a firmar no encontrado");
            //if (documentoOriginal != null && documentoOriginal.DocumentoSinFirma == null)
            //    responseCaseUse.Errors.Add("Documento a firmar sin contenido");

            var url_tramites_en_linea = context.Configuracion.FirstOrDefault(q => q.Nombre == nameof(Infrastructure.Enum.Configuracion.url_tramites_en_linea)); //; Util.Enum.Configuracion.url_tramites_en_linea
            if (url_tramites_en_linea == null)
                responseCaseUse.Errors.Add("No se encontró la configuración de la url de verificación de documentos");
            if (url_tramites_en_linea != null && url_tramites_en_linea.Valor.IsNullOrWhiteSpace())
                responseCaseUse.Errors.Add("No se encontró la configuración de la url de verificación de documentos");

            if (!emailsFirmantes.Any())
                responseCaseUse.Errors.Add("Debe especificar al menos un firmante");
            //if (emailsFirmantes.Any())
            //    foreach (var email in emailsFirmantes)
            //        if (!string.IsNullOrWhiteSpace(email) && !context.Rubrica.Any(q => q.Email == email && q.HabilitadoFirma))
            //            responseCaseUse.Errors.Add("No se encontró rúbrica habilitada para el firmante " + email);

            //if (string.IsNullOrEmpty(firmante))
            //    responseCaseUse.Errors.Add("Debe especificar el email del usuario firmante");

            //if (!string.IsNullOrEmpty(firmante))
            //{
            //    persona = _sigper.GetUserByEmail(firmante); 
            //    if (persona == null)
            //        responseCaseUse.Errors.Add("No se encontró usuario firmante en sistema Sigper");

            //    if (persona != null && string.IsNullOrWhiteSpace(persona.SubSecretaria))
            //        responseCaseUse.Errors.Add("No se encontró la subsecretaría del firmante");
            //}

            if (!responseCaseUse.IsValid)
                return responseCaseUse;

            //listado de id de firmantes
            var idsFirma = new List<string>();
            foreach (var email in emailsFirmantes)
            {
                var rubrica = context.Rubrica.FirstOrDefault(q => q.Email == email && q.HabilitadoFirma);
                if (rubrica != null)
                    idsFirma.Add(rubrica.IdentificadorFirma);
            }

            //si el documento ya tiene folio, no solicitarlo nuevamente
            if (string.IsNullOrWhiteSpace(documentoOriginal.NumeroFolio))
            {
                try
                {
                    //var errors = ModelState.Select(x => x.Value.Errors);
                    var _responseFolio = _folio.GetFolio(string.Join(", ", emailsFirmantes), documentoOriginal.TipoDocumentoCodigo, persona.SubSecretaria);
                    if (_responseFolio == null)
                        responseCaseUse.Errors.Add("Error al llamar el servicio externo de folio");

                    if (_responseFolio != null && _responseFolio.status == "ERROR")
                        responseCaseUse.Errors.Add(_responseFolio.error);

                    documentoOriginal.Folio = _responseFolio.folio;

                    _repository.Update(documentoOriginal);
                    _repository.Save();
                }
                catch (Exception ex)
                {
                    responseCaseUse.Errors.Add(ex.Message);
                }
            }

            if (!responseCaseUse.IsValid)
                return responseCaseUse;

            //crear nuevo documento
            var documentoFirmado = new FirmaDocumento()
            {
                Proceso = documentoOriginal.Proceso,
                Workflow = documentoOriginal.Workflow,//se debe enviar un objeto no un dato (revisar esto!)
                Fecha = DateTime.Now,
                Email = documentoOriginal.Autor,
                Signed = false,
                TipoPrivacidadId = (int)Infrastructure.Enum.TipoPrivacidad.Publico,
                TipoDocumentoId = 6,
                Folio = documentoOriginal.Folio,
            };
            _repository.Create(documentoFirmado);
            _repository.Save();

            try
            {
                //generar código QR
                var _responseQR = _file.CreateQr(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", documentoFirmado.DocumentoId));

                //firmar documento
                var _responseHSM = _hsm.SignWSDL(documentoOriginal.DocumentoSinFirma, idsFirma, documentoFirmado.DocumentoId, documentoFirmado.Folio, url_tramites_en_linea.Valor, _responseQR);

                //actualizar firma documento
                documentoOriginal.DocumentoConFirma = _responseHSM;
                documentoOriginal.DocumentoConFirmaFilename = "Firmado " + documentoOriginal.DocumentoSinFirmaFilename;
                documentoOriginal.Firmantes = string.Join(", ", idsFirma);
                documentoOriginal.Firmado = true;
                documentoOriginal.FechaFirma = DateTime.Now;
                documentoOriginal.DocumentoId = documentoFirmado.DocumentoId;

                //actualizar documento con contenido firmado
                documentoFirmado.File = _responseHSM;
                documentoFirmado.FileName = documentoOriginal.DocumentoConFirmaFilename;
                documentoFirmado.Signed = true;

                //obtener metadata del documento
                var _responseMetadata = _file.BynaryToText(documentoFirmado.File);
                if (_responseMetadata != null)
                {
                    documentoFirmado.Texto = _responseMetadata.Text;
                    documentoFirmado.Metadata = _responseMetadata.Metadata;
                    documentoFirmado.Type = _responseMetadata.Type;
                }

                //actualizar datos
                _repository.Update(documentoOriginal);
                _repository.Update(documentoFirmado);
            }
            catch (Exception ex)
            {
                //documento.Activo = false;
                //_repository.Update(documento);
                responseCaseUse.Errors.Add(ex.Message);
            }

            //guardar cambios
            _repository.Save();

            return responseCaseUse;
        }
        private SistemaIntegradoContext db = new SistemaIntegradoContext();
        private SistemaIntegradoContext context = new SistemaIntegradoContext();
        private UseCaseSigper sg = new UseCaseSigper();
        private DAES.Infrastructure.File.File fl = new DAES.Infrastructure.File.File();
        private Infrastructure.Folio.Folio folio = new Infrastructure.Folio.Folio();
        private Infrastructure.Hsm.HSM hsms = new Infrastructure.Hsm.HSM();


        public ResponseMessage SignReso(Documento obj, string email, int HorasExtrasId)
        {
            var response = new ResponseMessage();
            //var persona = new SIGPER();
            //var gp = new Infrastructure.GestionProcesos.GestionProcesos();
            // var gps = new Infrastructure.Interfaces.IGestionProcesos();

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                try
                {

                    //var documento = db.Documento.FirstOrDefault(q => q.DocumentoId == obj.DocumentoId);
                    var documento = context.Documento.FirstOrDefault(q => q.DocumentoId == obj.DocumentoId);
                    if (documento == null)
                        response.Errors.Add("Documento no encontrado");

                    if (obj.Firmado)
                        response.Errors.Add("Documento ya se encuentra firmado");

                    var rubrica = db.Rubrica.FirstOrDefault(q => q.Email == email);
                    /*old firma*/
                    //var rubrica = _repository.Get<Rubrica>(q => q.Email == email && q.HabilitadoFirma == true);
                    //string IdentificadorFirma = string.Empty;
                    //bool habilitado = false;
                    //foreach (var fir in rubrica)
                    //{
                    //    if (fir == null)
                    //        response.Errors.Add("Usuario sin información de firma electrónica");
                    //    if (fir != null && string.IsNullOrWhiteSpace(fir.IdentificadorFirma))
                    //        response.Errors.Add("Usuario no tiene identificador de firma electrónica");

                    //    if (documento.Proceso.DefinicionProcesoId == int.Parse(fir.IdProceso))
                    //    {
                    //        habilitado = true;
                    //        IdentificadorFirma = fir.IdentificadorFirma;
                    //    }

                    //    if (fir.HabilitadoFirma != true)
                    //        response.Errors.Add("Usuario no se encuentra habilitado para firmar");
                    //}
                    /**/

                    if (rubrica == null)
                        response.Errors.Add("No se encontraron firmas habilitadas para el usuario");

                    var HSMUser = db.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.UserHSM);
                    if (HSMUser == null)
                        response.Errors.Add("No se encontró la configuración de usuario de HSM.");
                    if (HSMUser != null && string.IsNullOrWhiteSpace(HSMUser.Valor))
                        response.Errors.Add("La configuración de usuario de HSM es inválida.");

                    var HSMPassword = db.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.PasswordHSM);
                    if (HSMPassword == null)
                        response.Errors.Add("No se encontró la configuración de usuario de HSM.");
                    if (HSMPassword != null && string.IsNullOrWhiteSpace(HSMPassword.Valor))
                        response.Errors.Add("La configuración de password de HSM es inválida.");

                    var url_tramites_en_linea = db.Configuracion.FirstOrDefault(q => q.Nombre == nameof(Infrastructure.Enum.Configuracion.url_tramites_en_linea));
                    if (url_tramites_en_linea == null)
                        response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");
                    if (url_tramites_en_linea != null && url_tramites_en_linea.Valor.IsNullOrWhiteSpace())
                        response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");


                    if (response.IsValid)
                    {
                        var persona = sg.GetUserByEmail(email);

                        /*se buscar la persona para determinar la subsecretaria*/
                        if (!string.IsNullOrEmpty(email))
                        {
                            if (persona == null)
                                response.Errors.Add("No se encontró usuario firmante en sistema Sigper");

                            if (persona != null && string.IsNullOrWhiteSpace(persona.SubSecretaria))
                                response.Errors.Add("No se encontró la subsecretaría del firmante");
                        }

                        /*Se busca proceso para determinar tipo de documento*/
                        string TipoDocto = "OTRO";
                        var result = documento.TipoDocumento.Nombre.ToString();
                        var a = result;
                        switch (result)
                        {
                            case "Resoluciones Ministeriales Exentas":
                                TipoDocto = "RMEX";
                                break;

                            case "Resoluciones Administrativas Exentas":
                                TipoDocto = "RAEX";
                                break;
                            case "Cartas":
                                TipoDocto = "CART";
                                break;
                            case "Memorandos":
                                TipoDocto = "MEMO";
                                break;
                            case "Circulares":
                                TipoDocto = "CIRC";
                                break;
                            case "Oficios":
                                TipoDocto = "OFIC";
                                break;
                            case var pa when a.Contains("Certificado"):
                                TipoDocto = "CERT";
                                break;
                            default:
                                TipoDocto = "OTRO";
                                break;
                        }
                        var proceso = db.Proceso.FirstOrDefault(q => q.ProcesoId == documento.ProcesoId);


                        //listado de id de firmantes
                        var idsFirma = new List<string>();
                        idsFirma.Add(rubrica.IdentificadorFirma);

                        //generar código QR
                        byte[] qr = fl.CreateQr(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", documento.DocumentoId));

                        //si el documento ya tiene folio no solicitarlo nuevamente
                        if (string.IsNullOrWhiteSpace(documento.Folio))
                        {

                            try
                            {

                                var folios = folio.GetFolio(string.Join(", ", email), TipoDocto, persona.SubSecretaria);
                                if (folios == null)
                                    response.Errors.Add("Servicio de folio no entregó respuesta");

                                if (folios != null && folios.status == "ERROR")
                                    response.Errors.Add(folios.error);

                                documento.Folio = folios.folio;
                                documento.File = documento.Content;


                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                response.Errors.Add(ex.Message);
                            }
                        }


                        var docto = hsms.Sign(documento.File, idsFirma, documento.DocumentoId, documento.Folio, url_tramites_en_linea.Valor, qr);
                        documento.Content = docto;
                        //documento.Signed = true;
                        documento.Firmado = true;

                        context.SaveChanges();

                    }
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }

            }


            return response;
        }

        public byte[] SignResoAuto(Documento obj, string email, int HorasExtrasId)
        {
            var response = new ResponseMessage();
            //var persona = new SIGPER();
            //var gp = new Infrastructure.GestionProcesos.GestionProcesos();
            // var gps = new Infrastructure.Interfaces.IGestionProcesos();

            using (SistemaIntegradoContext context = new SistemaIntegradoContext())
            {
                try
                {

                    //var documento = db.Documento.FirstOrDefault(q => q.DocumentoId == obj.DocumentoId);
                    var documento = context.Documento.FirstOrDefault(q => q.DocumentoId == obj.DocumentoId);
                    if (documento == null)
                        response.Errors.Add("Documento no encontrado");

                    if (obj.Firmado)
                        response.Errors.Add("Documento ya se encuentra firmado");

                    var rubrica = context.Rubrica.FirstOrDefault(q => q.IdentificadorFirma == email);
                    /*old firma*/
                    //var rubrica = _repository.Get<Rubrica>(q => q.Email == email && q.HabilitadoFirma == true);
                    //string IdentificadorFirma = string.Empty;
                    //bool habilitado = false;
                    //foreach (var fir in rubrica)
                    //{
                    //    if (fir == null)
                    //        response.Errors.Add("Usuario sin información de firma electrónica");
                    //    if (fir != null && string.IsNullOrWhiteSpace(fir.IdentificadorFirma))
                    //        response.Errors.Add("Usuario no tiene identificador de firma electrónica");

                    //    if (documento.Proceso.DefinicionProcesoId == int.Parse(fir.IdProceso))
                    //    {
                    //        habilitado = true;
                    //        IdentificadorFirma = fir.IdentificadorFirma;
                    //    }

                    //    if (fir.HabilitadoFirma != true)
                    //        response.Errors.Add("Usuario no se encuentra habilitado para firmar");
                    //}
                    /**/

                    if (rubrica == null)
                        response.Errors.Add("No se encontraron firmas habilitadas para el usuario");

                    var HSMUser = db.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.UserHSM);
                    if (HSMUser == null)
                        response.Errors.Add("No se encontró la configuración de usuario de HSM.");
                    if (HSMUser != null && string.IsNullOrWhiteSpace(HSMUser.Valor))
                        response.Errors.Add("La configuración de usuario de HSM es inválida.");

                    var HSMPassword = db.Configuracion.FirstOrDefault(q => q.ConfiguracionId == (int)Infrastructure.Enum.Configuracion.PasswordHSM);
                    if (HSMPassword == null)
                        response.Errors.Add("No se encontró la configuración de usuario de HSM.");
                    if (HSMPassword != null && string.IsNullOrWhiteSpace(HSMPassword.Valor))
                        response.Errors.Add("La configuración de password de HSM es inválida.");

                    var url_tramites_en_linea = db.Configuracion.FirstOrDefault(q => q.Nombre == nameof(Infrastructure.Enum.Configuracion.url_tramites_en_linea));
                    if (url_tramites_en_linea == null)
                        response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");
                    if (url_tramites_en_linea != null && url_tramites_en_linea.Valor.IsNullOrWhiteSpace())
                        response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");


                    if (response.IsValid)
                    {
                        var persona = sg.GetUserByEmail(rubrica.Email);

                        /*se buscar la persona para determinar la subsecretaria*/
                        if (!string.IsNullOrEmpty(email))
                        {
                            if (persona == null)
                                response.Errors.Add("No se encontró usuario firmante en sistema Sigper");

                            if (persona != null && string.IsNullOrWhiteSpace(persona.SubSecretaria))
                                response.Errors.Add("No se encontró la subsecretaría del firmante");
                        }

                        /*Se busca proceso para determinar tipo de documento*/
                        string TipoDocto = "OTRO";
                        var result = documento.TipoDocumento.Nombre.ToString();
                        var a = result;
                        switch (result)
                        {
                            case "Resoluciones Ministeriales Exentas":
                                TipoDocto = "RMEX";
                                break;
                            case "Resoluciones Administrativas Exentas":
                                TipoDocto = "RAEX";
                                break;
                            case "CARTAS":
                                TipoDocto = "CART";
                                break;
                            case "Memorandos":
                                TipoDocto = "MEMO";
                                break;
                            case "Circulares":
                                TipoDocto = "CIRC";
                                break;
                            case "Oficios":
                                TipoDocto = "OFIC";
                                break;
                            case var pa when a.Contains("Certificado"):
                                TipoDocto = "CERT";
                                break;
                            default:
                                TipoDocto = "OTRO";
                                break;
                        }
                        var proceso = db.Proceso.FirstOrDefault(q => q.ProcesoId == documento.ProcesoId);


                        //listado de id de firmantes
                        var idsFirma = new List<string>();
                        idsFirma.Add(rubrica.IdentificadorFirma);

                        //generar código QR
                        byte[] qr = fl.CreateQr(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", documento.DocumentoId));

                        //si el documento ya tiene folio no solicitarlo nuevamente
                        if (string.IsNullOrWhiteSpace(documento.Folio))
                        {

                            try
                            {

                                var folios = folio.GetFolio(string.Join(", ", rubrica.Email), TipoDocto, persona.SubSecretaria);
                                if (folios == null)
                                    response.Errors.Add("Servicio de folio no entregó respuesta");

                                if (folios != null && folios.status == "ERROR")
                                    response.Errors.Add(folios.error);

                                documento.Folio = folios.folio;
                                documento.Content = obj.Content;


                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                response.Errors.Add(ex.Message);
                            }
                        }

                        var docto = hsms.Sign(documento.Content, idsFirma, documento.DocumentoId, documento.Folio, url_tramites_en_linea.Valor, qr);
                        documento.Content = docto;
                        //documento.Signed = true;
                        documento.Firmado = true;
                        response.Documento = documento.Content;
                        context.SaveChanges();

                    }
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }

            }


            return response.Documento;
        }

    }
}
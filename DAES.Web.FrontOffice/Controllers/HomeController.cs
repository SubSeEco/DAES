﻿using DAES.Web.FrontOffice.Helper;
using DAES.Web.FrontOffice.Models;
using System.Web.Mvc;

namespace DAES.Web.FrontOffice.Controllers
{
    [Audit]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GPHSA()
        {
            Global.CurrentClaveUnica.ClaveUnicaRequestAutorization.controller = "GPHSA";
            Global.CurrentClaveUnica.ClaveUnicaRequestAutorization.method = "Index";
            return Redirect();
        }        

        private ActionResult Redirect()
        {
            //Global.CurrentClaveUnica.ClaveUnicaRequestAutorization.controller = "GPHSA";
            //Global.CurrentClaveUnica.ClaveUnicaRequestAutorization.method = "Index";
            //activar en desarrollo, bypass de clave única
            //Global.CurrentClaveUnica.ClaveUnicaUser = new ClaveUnicaUser();
            //Global.CurrentClaveUnica.ClaveUnicaUser.name = new Name
            //{
            //    nombres = new System.Collections.Generic.List<string> { "DESA", "DESA" },
            //    apellidos = new System.Collections.Generic.List<string> { "DESA", "DESA" }
            //};
            //Global.CurrentClaveUnica.ClaveUnicaUser.RolUnico = new RolUnico
            //{
            //    numero = 17957898,
            //    DV = "0",
            //    tipo = "RUN"
            //};
            //return RedirectToAction(Global.CurrentClaveUnica.ClaveUnicaRequestAutorization.method, Global.CurrentClaveUnica.ClaveUnicaRequestAutorization.controller);

            //Sin bypass
            return Redirect(Global.CurrentClaveUnica.ClaveUnicaRequestAutorization.uri);
        }
    }
}
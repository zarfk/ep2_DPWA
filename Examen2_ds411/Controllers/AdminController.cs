using Examen2_ds411.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examen2_ds411.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin

        examen2ds31Entities contexto = new examen2ds31Entities();

        public ActionResult Cuenta()
        {
            //Vista de crear cuentas y tabla de registro
            if (System.Web.HttpContext.Current.Session["rol"] == null)
            {
                //Si no ha iniciado sesion lo manda al login
                return RedirectToAction("Login", "Login");
            }
            else
            {   //Si ha iniciado secion, pero no es admin lo manda la vista estado de cuenta
                if (System.Web.HttpContext.Current.Session["rol"]?.ToString() != "administrador")
                {
                    return RedirectToAction("EstadoCuenta", "Cliente");
                }
                else
                {
                    // Si es administrador
                    return View();
                }

            }
        }
    

    }
}
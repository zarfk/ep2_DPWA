using Examen2_ds411.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examen2_ds411.Controllers
{
    
    public class LoginController : Controller
    {
        // GET: Login
        examen2ds31Entities contexto = new examen2ds31Entities();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(cliente c)
        {
            cliente C_existente = contexto.cliente.FirstOrDefault(x => x.cod_cliente == c.cod_cliente && x.nit == c.nit);

            if (C_existente != null)
            {
                ViewBag.ins = "true";
                System.Web.HttpContext.Current.Session["rol"] = C_existente.rol;
                System.Web.HttpContext.Current.Session["cod_cliente"] = C_existente.cod_cliente;
                
                //Segun el usuario 

                if (System.Web.HttpContext.Current.Session["rol"]?.ToString() == "administrador")
                {
                    //Cambia a la que sera tu vista principal
                    return RedirectToAction("Cuenta", "Admin");
                }
                else
                {
                    return RedirectToAction("EstadoCuenta", "Cliente");

                }
            }
            else
            {
                ViewBag.ins = "false";
                return View();
            }
            
        }

        //Cerrar Secion
        public ActionResult CerrarSesion()
        {
            System.Web.HttpContext.Current.Session["rol"] = null;
            System.Web.HttpContext.Current.Session["cod_cliente"] = null;

            return RedirectToAction("login");
        }

    }
}
using Examen2_ds411.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examen2_ds411.Controllers
{
    public class CuentasController : Controller
    {
        examen2ds31Entities contexto = new examen2ds31Entities();
        // GET: Cuentas

        //Vista accedidad por administrador para crear cuentas
        //Creacion de cuentas y tabla de registros

        public ActionResult Cuenta()
        {
            //Vista de crear cuentas y tabla de registro
            if (System.Web.HttpContext.Current.Session["rol"] == null )
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                if (System.Web.HttpContext.Current.Session["rol"]?.ToString() != "administrador")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Si no es administrador
                    return View();
                }

            }
        }
        public ActionResult EstadoCuenta()   
        {
            int cod_cliente = int.Parse(System.Web.HttpContext.Current.Session["cod_cliente"].ToString());
            cuenta c = contexto.cuenta.FirstOrDefault(x => x.cod_cliente == cod_cliente);
            return View(c);
        }

    }
}
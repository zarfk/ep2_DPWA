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

        // Obtener listas de clientes y cuentas

        [HttpGet]
        public JsonResult GetClientes()
        {
            List<cliente> data = contexto.cliente.ToList();
            List<cliente_str> data2 = new List<cliente_str>();

            foreach (cliente item in data)
            {
                cliente_str temp = new cliente_str();
                temp.cod_cliente = item.cod_cliente;
                temp.nombre_cliente = item.nombre_cliente;
                temp.nit = item.nit;
                temp.rol = item.rol;
                data2.Add(temp);
            }
            return Json(new {resultado = data2}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCuenta(int cod_cliente)
        {
            List<cuenta> data = contexto.cuenta.Where(x => x.cod_cliente == cod_cliente).ToList();
            List<cuenta_str> data2 = new List<cuenta_str>();

            foreach (cuenta item in data)
            {
                cuenta_str temp = new cuenta_str();
                temp.ncta = item.ncta;
                temp.saldo = item.saldo;
                temp.cod_cliente = item.cod_cliente;
                data2.Add(temp);
            }
            return Json(new { resultado = data2 }, JsonRequestBehavior.AllowGet);
        }

        // Crear clientes y cuentas de clientes

        [HttpPost]
        public JsonResult CrearCliente(cliente c)
        {
            try
            {
                contexto.cliente.Add(c);
                contexto.SaveChanges();
                return Json("Cliente creado!");
            }
            catch(Exception e)
            {
                return Json(e.Message);
            }
        }

        [HttpPost]
        public JsonResult CrearCuenta(cuenta c)
        {
            try
            {
                contexto.cuenta.Add(c);
                contexto.SaveChanges();
                return Json("Cuenta creada!");
            }catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        // Structs de cliente y cuenta

        struct cliente_str
        {
            public int cod_cliente { get; set; }
            public string nombre_cliente { get; set; }
            public string nit { get; set; }
            public string rol { get; set; }
        }
        struct cuenta_str
        {
            public int ncta { get; set; }
            public double? saldo { get; set; }
            public int? cod_cliente { get; set; }
        }
    }
}
using Examen2_ds411.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examen2_ds411.Controllers
{
    public class ClienteController : Controller
    {
        examen2ds31Entities contexto = new examen2ds31Entities();
        // GET: Cliente
        
        public ActionResult EstadoCuenta()
        {
            if (System.Web.HttpContext.Current.Session["rol"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {   
                //Si es admin lo mando a la vista de Admin
                //Lo cambias por la que sera tu vista principal

                if (System.Web.HttpContext.Current.Session["rol"]?.ToString() == "administrador")
                {
                    return RedirectToAction("Cuenta", "Admin");
                }
                else
                {
                    return View();
                }

            }

        }

        public JsonResult GetEstadoCuenta()
        {
            if (Session["cod_cliente"] == null)
                return Json(new { error = "Sesión expirada o no válida." }, JsonRequestBehavior.AllowGet);


            int cod_cliente = int.Parse(System.Web.HttpContext.Current.Session["cod_cliente"].ToString());

            var cuenta = contexto.cuenta.FirstOrDefault(c => c.cod_cliente == cod_cliente);
            var cliente = contexto.cliente.FirstOrDefault(c => c.cod_cliente == cod_cliente);


            if (cuenta == null || cliente == null)
            {
                return Json(new { error = "Cuenta o cliente no encontrado." }, JsonRequestBehavior.AllowGet);
            }

            var transacciones = contexto.transacciones
                .Where(t => t.ncta == cuenta.ncta)
                .Select(t => new Transaccion
                {
                    tipo = t.tipo,
                    monto = t.monto.Value,
                    fecha = t.fecha.Value

                }).ToList();

            EstadoDeCuenta estado = new EstadoDeCuenta
            {
                nombreCliente = cliente.nombre_cliente,
                cod_cliente = cliente.cod_cliente,
                nit = cliente.nit,
                numeroCuenta = cuenta.ncta,
                saldo = cuenta.saldo.Value,
                Transacciones = transacciones
            };

            return Json(new { resultado = estado }, JsonRequestBehavior.AllowGet);
        }

        //Estas estructuras las voy a modificar con la logica que un usuario puede
        //tener varias cuentas, pero te pueden servir de referencia

        //Estructuras: 
        public class Transaccion
        {
            public string tipo { get; set; }
            public double monto { get; set; }
            public DateTime fecha { get; set; }
        }

        public class EstadoDeCuenta
        {

            public string nombreCliente { get; set; }
            public int cod_cliente { get; set; }
            public string nit { get; set; }
            public int numeroCuenta { get; set; }
            public double saldo { get; set; }

            public List<Transaccion> Transacciones { get; set; }

        }

    }

}

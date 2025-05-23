﻿using Examen2_ds411.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 using static Examen2_ds411.Controllers.ClienteController; 
namespace Examen2_ds411.Controllers
{
    public class ClienteController : Controller
    {
        examen2ds31Entities contexto = new examen2ds31Entities();

        // GET: Cliente
        public ActionResult EstadoCuenta()
        {
            try
            {
                if (TempData["msj"] != null)
                {
                    ViewBag.msjT = TempData["msj"];
                }

                if (TempData["error_general"] != null)
                {
                    ViewBag.msjError = TempData["error_general"];
                }


                if (System.Web.HttpContext.Current.Session["rol"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
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
            catch (Exception ex)
            {
                TempData["mjs"] = "ErrorData";
                return RedirectToAction("Login", "Login"); 
            }
            // Fin: Bloque try-catch
        }

        [HttpGet]
        public ActionResult TransaccionCliente(string tipo, string n)
        {
            try
            {
                if (Session["rol"] == null)
                    return RedirectToAction("Login", "Login");

                if (Session["rol"]?.ToString() == "administrador")
                    return RedirectToAction("Cuenta", "Admin");

                int ncta = int.Parse(n); 
                int cant_t = contexto.transacciones.Count(); 

                TempData["Tipo"] = tipo;
                TempData["ncta"] = n;
                TempData["cod_transac"] = cant_t + 1; 

                return View();
            }
            catch (FormatException)
            {
                TempData["error_general"] = "Número de cuenta proporcionado no válido.";
                return RedirectToAction("EstadoCuenta");
            }
            catch (Exception ex)
            {
                TempData["msj"] = "ErrorData";
                return RedirectToAction("EstadoCuenta"); 
            }

        }

        [HttpPost]
        public ActionResult GuardarTransaccion(transacciones t)
        {
            try
            {
                if (Session["rol"] == null)
                    return RedirectToAction("Login", "Login");

                if (Session["rol"]?.ToString() == "administrador")
                    return RedirectToAction("Cuenta", "Admin");

                int cod_cliente = int.Parse(System.Web.HttpContext.Current.Session["cod_cliente"].ToString());
                cliente cl = contexto.cliente.FirstOrDefault(c => c.cod_cliente == cod_cliente); // Consulta BD

                cuenta ct = contexto.cuenta.FirstOrDefault(c => c.ncta == t.ncta && c.cod_cliente == cod_cliente);

                if (ct == null)
                {
                    TempData["msj"] = "ErrorCuenta"; 
                    return RedirectToAction("EstadoCuenta");
                }

                
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                int transaccionesHoy = contexto.transacciones
                    .Where(t_db => t_db.fecha.HasValue &&
                                   t_db.fecha.Value >= today &&
                                   t_db.fecha.Value < tomorrow)
                    .Join(contexto.cuenta,
                          t_db => t_db.ncta,
                          c_db => c_db.ncta,
                          (t_db, c_db) => new { Transaction = t_db, Cuenta = c_db })
                    .Where(joined => joined.Cuenta.cod_cliente == cod_cliente)
                    .Count(); 

                int limiteDiario = 10;

                if (transaccionesHoy >= limiteDiario)
                {
                    TempData["msj"] = "limiteDiario"; 
                    return RedirectToAction("EstadoCuenta");
                }

                t.fecha = DateTime.Now;

                if (t.tipo == "deposito")
                {
                    contexto.transacciones.Add(t);
                    ct.saldo += t.monto;
                    contexto.SaveChanges(); 
                    TempData["msj"] = "deposito";
                }
                else if (t.tipo == "retiro") 
                {
                    if (ct.saldo == null || ct.saldo < t.monto)
                    {
                        TempData["msj"] = "SaldoIn";
                    }
                    else
                    {
                        contexto.transacciones.Add(t);
                        ct.saldo -= t.monto;
                        contexto.SaveChanges(); 
                        TempData["msj"] = "retiro";
                    }
                }
                
                return RedirectToAction("EstadoCuenta");
            }
            
            catch (Exception ex)
            {
                TempData["msj"] = "ErrorData";
                return RedirectToAction("EstadoCuenta"); 
            }

        }

        public JsonResult GetEstadoCuenta()
        {
            try
            {
                if (Session["cod_cliente"] == null)
                    return Json(new { error = "Sesión expirada o no válida." }, JsonRequestBehavior.AllowGet);

                int cod_cliente = int.Parse(System.Web.HttpContext.Current.Session["cod_cliente"].ToString());

                var cliente = contexto.cliente.FirstOrDefault(c => c.cod_cliente == cod_cliente);

                var cuentas = contexto.cuenta
                   .Where(c => c.cod_cliente == cod_cliente)
                   .Select(c => new CuentaCliente
                   {
                       saldo = c.saldo.Value, 
                       numeroCuenta = c.ncta,
                       Transacciones = contexto.transacciones
                           .Where(t => t.ncta == c.ncta)
                           .Select(t => new Transaccion
                           {
                               tipo = t.tipo,
                               monto = t.monto.Value, 
                               fecha = t.fecha 
                           }).ToList() 
                   }).ToList(); 

                if (cuentas == null || cliente == null)
                {
                    return Json(new { error = "Ocurrio un error al encontrar los datos del cliente o cuentas." }, JsonRequestBehavior.AllowGet);
                }

                EstadoDeCuenta estado = new EstadoDeCuenta
                {
                    nombreCliente = cliente.nombre_cliente,
                    cod_cliente = cliente.cod_cliente,
                    nit = cliente.nit,
                    Cuentas = cuentas
                };

                return Json(new { resultado = estado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Ocurrió un error inesperado al obtener los datos del estado de cuenta." }, JsonRequestBehavior.AllowGet);
            }

        }

        public class Transaccion 
        {
            public string tipo { get; set; }
            public double monto { get; set; }
            public DateTime? fecha { get; set; }
        }
        public class CuentaCliente
        { 
            public double saldo { get; set; }
            public int numeroCuenta { get; set; }
            public List<Transaccion> Transacciones { get; set; }
        }

        public class EstadoDeCuenta 
        {
            public string nombreCliente { get; set; }
            public int cod_cliente { get; set; }
            public string nit { get; set; }
            public List<CuentaCliente> Cuentas { get; set; }
        }
    }
}
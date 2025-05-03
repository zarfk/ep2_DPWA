using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examen2_ds411.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.Session["rol"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                ViewBag.Message = "Your application description page.";
                return View();
            }
        }

        public ActionResult About()
        {
            if (System.Web.HttpContext.Current.Session["rol"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                ViewBag.Message = "Your application description page.";
                return View();
            }
        }

        public ActionResult Contact()
        {
            if (System.Web.HttpContext.Current.Session["rol"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                ViewBag.Message = "Your application description page.";
                return View();
            }
        }
    }
}
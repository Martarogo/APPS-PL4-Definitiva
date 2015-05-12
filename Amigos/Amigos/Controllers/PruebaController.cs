using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amigos.Controllers
{
    public class PruebaController : Controller
    {
        //
        // GET: /Prueba/
        public ActionResult Index()
        {
            return View();
        }

        public String Hola (String valor, int veces)
        //http://localhost:54321/prueba/Hola?valor=Marta&veces=10
        {
            String repetido=null;
            for (int i = 1; i <= veces; i++)         
            {
                repetido = repetido+valor;
            }
            return repetido;
        }

        public ActionResult Adios(string valor, int veces)
        {
            ViewBag.valor = valor;
            ViewBag.veces = veces;

            return View();
        }
    }
}
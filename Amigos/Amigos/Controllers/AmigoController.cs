using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Amigos.DataAccessLayed;
using Amigos.Models;
using Amigos.Helpers;

namespace Amigos.Controllers
{
    public class AmigoController : BaseController
    {
        private AmigoDBContext db = new AmigoDBContext();
        private readonly String FORMATERROR="#formaterror";

        // GET: Amigo
        public ActionResult Index()
        {
            return View(db.Amigos.ToList());
        }

        // GET: Amigo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Amigo amigo = db.Amigos.Find(id);
            if (amigo == null)
            {
                return HttpNotFound();
            }
            return View(amigo);
        }

        // GET: Amigo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Amigo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,name,longi,lati")] Amigo amigo)
        {
            if (ModelState.IsValid)
            {
                amigo.lati = decimalFormat(amigo.lati);
                amigo.longi = decimalFormat(amigo.longi);
                if (amigo.lati == FORMATERROR || amigo.longi == FORMATERROR)
                {
                    RedirectToAction("Create");
                }
                else
                {
                    db.Amigos.Add(amigo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                } 
            }
            return View(amigo);
        }
        
        //GET Amigo/Distance
        public ActionResult Distance()
        {
            return View();
        }

        //POST: Amigo/Distance
        [HttpPost]
        public ActionResult Distance([Bind(Include = "ID,name,longi,lati")] Amigo ami, string distance)
        {
            ami.longi = decimalFormat(ami.longi);
            ami.lati = decimalFormat(ami.lati);
            if(Convert.ToDouble(distance)<0||Math.Abs(Convert.ToDouble(ami.lati))>90||Math.Abs(Convert.ToDouble(ami.longi))>90){
                return View();
            }else{
                return RedirectToAction("Closest", new { lati=ami.lati, longi=ami.longi, radium=distance});
            }
        }

        //GET: Amigo/Closest
        public ActionResult Closest(string lati, string longi, string radium)
        {
            Amigo me = new Amigo();
            me.lati = lati;
            me.longi = longi;
            if (radium.Contains(",")) radium = radium.Replace(",",".");
            double maxDistance = Convert.ToDouble(radium);

            List<Amigo> lista = new List<Amigo>();

            foreach (Amigo friend in db.Amigos)
            {
                if (me.getDistance(friend) <= maxDistance)
                {
                    lista.Add(friend);
                }
            }
            return View(lista.ToList());
        }

        // GET: Amigo/Edit/5
        public ActionResult Edit(int? id)//La interrogación indica que puede ser null
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Amigo amigo = db.Amigos.Find(id);
            if (amigo == null)
            {
                return HttpNotFound();
            }
            return View(amigo);
        }

        // POST: Amigo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,name,longi,lati")] Amigo amigo)
        {
            if (ModelState.IsValid)
            //Este condicional sirve para determinar si los datos recibidos son válidos para crear un nuevo objeto e insertarlo en la base de datos.
            {
                amigo.lati=decimalFormat(amigo.lati);
                amigo.longi = decimalFormat(amigo.longi);
                if (amigo.lati == FORMATERROR || amigo.longi == FORMATERROR)
                {
                    RedirectToAction("Edit");
                }
                else
                {
                    db.Entry(amigo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(amigo);
        }

        // GET: Amigo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Amigo amigo = db.Amigos.Find(id);
            if (amigo == null)
            {
                return HttpNotFound();
            }
            return View(amigo);
        }

        // POST: Amigo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Amigo amigo = db.Amigos.Find(id);
            db.Amigos.Remove(amigo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private String decimalFormat(String n)
        {
            if (n.Contains(",")){
                if (n.Contains("."))
                {
                    return FORMATERROR;
                }
                n=n.Replace(",", "."); 
            }
            try
            {
                double aux=Convert.ToDouble(n);
                if (Math.Abs(aux) > 90)
                {
                    return FORMATERROR;
                }
                return aux.ToString();
            }catch(FormatException){
                return FORMATERROR;
            }
        }
    }
}

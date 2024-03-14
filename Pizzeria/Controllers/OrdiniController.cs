using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Pizzeria.Models;

namespace Pizzeria.Controllers
{
    [Authorize(Roles ="Admin")]
    public class OrdiniController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Ordini
        public ActionResult Index()
        {
            var ordini = db.Ordini.Include(o => o.Utenti).OrderByDescending(o => o.DataOrdine);
            return View(ordini.ToList());
        }

        public ActionResult isEvaso(int id)
        {
            Ordini ordine = db.Ordini.Find(id);
            ordine.isEvaso = true;
            db.Entry(ordine).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Ordini/Details/5
        public ActionResult isntEvaso(int id)
        {
            Ordini ordine = db.Ordini.Find(id);
            ordine.isEvaso = false;
            db.Entry(ordine).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public PartialViewResult Details(int? id)
        {
            var Dettagli = db.Dettagli.Include(o => o.Ordini)
                .Include(o => o.Prodotti)
                .Where(o => o.idOrdine_FK == id);

            return PartialView("_Details", Dettagli.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult OrdiniUtente()
        {
            var userId = db.Utenti.FirstOrDefault(u => u.Email == User.Identity.Name).idUtente;
            var ordini = db.Ordini.Include(o => o.Utenti)
                .Include(o => o.Dettagli)
                .Where(o => o.idUtente_FK == userId)
                .OrderByDescending(o => o.DataOrdine);
            return View(ordini.ToList());
        }

        

        public async Task<ActionResult> GetNumeroOrdini()
        {
            // Conta il numero di ordini nel database
            int totale = await db.Ordini.Where(o => o.isEvaso == true).CountAsync();

            // Restituisce il conteggio come risposta JSON
            return Json(totale, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> IncassatoPerGiorno(DateTime data)
        {

            // Calcola l'incasso per il giorno specificato
            decimal incasso = await db.Ordini
                .Where(o => o.DataOrdine.Year == data.Year && o.DataOrdine.Month == data.Month && o.DataOrdine.Day == data.Day)
                .SumAsync(o => o.Totale);

            // Restituisce l'incasso come risposta JSON
            return Json(incasso, JsonRequestBehavior.AllowGet);
        }
    }
}

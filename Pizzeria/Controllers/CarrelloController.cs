using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pizzeria.Models;

namespace Pizzeria.Controllers
{
    public class CarrelloController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Carrello
        public ActionResult Index()
        {
            var cart = Session["cart"] as List<Prodotti>;
            if (cart == null || !cart.Any()) // Check if the cart is empty
            {
               return RedirectToAction("Index", "Prodotti");
            }
            return View(cart);
        }



        // GET: Carrello/Delete/5
        public ActionResult Delete(int? id)
        {
            var cart = Session["cart"] as List<Prodotti>;
            if (cart != null)
            {
                var productToRemove = cart.FirstOrDefault(p => p.idProdotto == id);
                if (productToRemove != null)
                {
                    cart.Remove(productToRemove);
                }
            }

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
        [HttpPost]
        public ActionResult Ordina(string note, string indirizzo)
        {
            ModelDbContext db = new ModelDbContext();
            var userId = db.Utenti.FirstOrDefault(u => u.Email == User.Identity.Name).idUtente;

            var cart = Session["cart"] as List<Prodotti>;
            if (cart != null && cart.Any()) // Check if the cart is not empty
            {
                // Create a new order
                Ordini newOrder = new Ordini();
                newOrder.DataOrdine = DateTime.Now;
                newOrder.isEvaso = false;
                newOrder.idUtente_FK = userId;
                newOrder.Indirizzo = indirizzo;
                newOrder.Totale = cart.Sum(p => p.Prezzo);
                newOrder.Note = note;

                // Save the order to the database
                db.Ordini.Add(newOrder);
                db.SaveChanges();

                // Create a new order detail for each product in the cart
                foreach (var product in cart)
                {
                    Dettagli newDetail = new Dettagli();
                    newDetail.idOrdine_FK = newOrder.idOrdine;
                    newDetail.idProdotto_FK = product.idProdotto;
                    newDetail.Quantita = 1;

                    // Save the order detail to the database
                    db.Dettagli.Add(newDetail);
                    db.SaveChanges();
                }

                // Clear the cart
                cart.Clear();
            }

            return RedirectToAction("Index", "Prodotti");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Models;

namespace Bidc_Union_Management.Controllers
{
    public class PhanDoanController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        //
        // GET: /PhanDoan/

        public ActionResult Index()
        {
            ViewBag.phandoan = "active";
            List<PhandoanModels> dsPhanDoan = new List<PhandoanModels>();
            foreach (PhanDoan item in db.PhanDoans.OrderBy(a=>a.Name).ToList())
            {
                ChiDoan cd = db.ChiDoans.Find(item.ChiDoanId);
                dsPhanDoan.Add(new PhandoanModels() { Id=item.Id,Name=item.Name,ChiDoan=cd.Name,CreateDate=(DateTime)item.CreateDate});
            }
            return View(dsPhanDoan);
        }

        //
        // GET: /PhanDoan/Details/5

        public ActionResult Details(Guid id = new Guid())
        {
            ViewBag.phandoan = "active";
            PhanDoan phandoan = db.PhanDoans.Find(id);
            ChiDoan chidoan = db.ChiDoans.Find(phandoan.ChiDoanId);
            if (chidoan==null)
            {
                ViewBag.chidoan = "";
            }
            else
            {
                ViewBag.chidoan = chidoan.Name;
            }            
            if (phandoan == null)
            {
                return HttpNotFound();
            }
            return View(phandoan);
        }

        //
        // GET: /PhanDoan/Create

        public ActionResult Create()
        {
            List<ChiDoan> dschidoan = db.ChiDoans
                                    .OrderBy(a => a.Name)
                                    .ToList();
            List<SelectListItem> listchidoans = new List<SelectListItem>();
            foreach (var item in dschidoan)
            {
                listchidoans.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.listchidoans = listchidoans;
            ViewBag.phandoan = "active";
            return View();
        }

        //
        // POST: /PhanDoan/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhanDoan phandoan)
        {
            ViewBag.phandoan = "active";
            if (ModelState.IsValid)
            {
                phandoan.Id = Guid.NewGuid();
                phandoan.CreateDate = DateTime.Now;
                db.PhanDoans.Add(phandoan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(phandoan);
        }

        //
        // GET: /PhanDoan/Edit/5

        public ActionResult Edit(Guid id = new Guid())
        {
            ViewBag.phandoan = "active";

            PhanDoan phandoan = db.PhanDoans.Find(id);
            if (phandoan == null)
            {
                return HttpNotFound();
            }
            List<ChiDoan> dschidoan = db.ChiDoans
                                    .OrderBy(a => a.Name)
                                    .ToList();
            List<SelectListItem> listchidoans = new List<SelectListItem>();
            foreach (var item in dschidoan)
            {
                if (item.Id==phandoan.ChiDoanId)
                {
                    listchidoans.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    listchidoans.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()                        
                    });
                }                
            }
            ViewBag.listchidoans = listchidoans;
            return View(phandoan);
        }

        //
        // POST: /PhanDoan/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhanDoan phandoan)
        {
            ViewBag.phandoan = "active";
            if (ModelState.IsValid)
            {
                db.Entry(phandoan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phandoan);
        }

        //
        // GET: /PhanDoan/Delete/5

        public ActionResult Delete(Guid id = new Guid())
        {
            ViewBag.phandoan = "active";
            PhanDoan phandoan = db.PhanDoans.Find(id);
            ChiDoan chidoan = db.ChiDoans.Find(phandoan.ChiDoanId);
            if (chidoan == null)
            {
                ViewBag.chidoan = "";
            }
            else
            {
                ViewBag.chidoan = chidoan.Name;
            }
            if (phandoan == null)
            {
                return HttpNotFound();
            }
            return View(phandoan);
        }

        //
        // POST: /PhanDoan/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewBag.phandoan = "active";
            PhanDoan phandoan = db.PhanDoans.Find(id);
            db.PhanDoans.Remove(phandoan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
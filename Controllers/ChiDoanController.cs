using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Filters;
using Bidc_Union_Management.Models;

namespace Bidc_Union_Management.Controllers
{
    [InitializeSimpleMembership]
    public class ChiDoanController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        //
        // GET: /ChiDoan/

        public ActionResult Index()
        {            
            ViewBag.chidoan = "active";
            return View(db.ChiDoans.ToList());
        }

        //
        // GET: /ChiDoan/Details/5

        public ActionResult Details(Guid id = new Guid())
        {
            ViewBag.chidoan = "active";
            ChiDoan chidoan = db.ChiDoans.Find(id);
            if (chidoan == null)
            {
                return HttpNotFound();
            }
            return View(chidoan);
        }

        //
        // GET: /ChiDoan/Create

        public ActionResult Create()
        {
            ViewBag.chidoan = "active";
            return View();
        }

        //
        // POST: /ChiDoan/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChiDoan chidoan)
        {
            ViewBag.chidoan = "active";
            if (ModelState.IsValid)
            {
                chidoan.Id = Guid.NewGuid();
                chidoan.CreateDate = DateTime.Now;
                db.ChiDoans.Add(chidoan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(chidoan);
        }

        //
        // GET: /ChiDoan/Edit/5

        public ActionResult Edit(Guid id = new Guid())
        {
            ViewBag.chidoan = "active";
            ChiDoan chidoan = db.ChiDoans.Find(id);
            if (chidoan == null)
            {
                return HttpNotFound();
            }
            return View(chidoan);
        }

        //
        // POST: /ChiDoan/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChiDoan chidoan)
        {
            ViewBag.chidoan = "active";
            if (ModelState.IsValid)
            {
                db.Entry(chidoan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chidoan);
        }

        //
        // GET: /ChiDoan/Delete/5

        public ActionResult Delete(Guid id = new Guid())
        {
            ViewBag.chidoan = "active";
            ChiDoan chidoan = db.ChiDoans.Find(id);
            if (chidoan == null)
            {
                return HttpNotFound();
            }
            return View(chidoan);
        }

        //
        // POST: /ChiDoan/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewBag.chidoan = "active";
            ChiDoan chidoan = db.ChiDoans.Find(id);
            db.ChiDoans.Remove(chidoan);
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
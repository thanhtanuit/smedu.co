using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Models;
using System.Web.Security;

namespace Bidc_Union_Management.Controllers
{
    //[Authorize(Roles = "Inputter")]
    public class NhiemKyController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        //
        // GET: /NhiemKy/

        public ActionResult Index()
        {
            ViewBag.nhiemky = "active";
            string[] roles = Roles.GetRolesForUser();
            return View(db.NhiemKies.OrderByDescending(a=>a.CreateDate).ToList());
        }

        //
        // GET: /NhiemKy/Details/5

        public ActionResult Details(Guid id = new Guid())
        {
            ViewBag.nhiemky = "active";
            NhiemKy nhiemky = db.NhiemKies.Find(id);
            if (nhiemky == null)
            {
                return HttpNotFound();
            }
            return View(nhiemky);
        }

        //
        // GET: /NhiemKy/Create

        public ActionResult Create()
        {
            ViewBag.nhiemky = "active";
            return View();
        }

        //
        // POST: /NhiemKy/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NhiemKy nhiemky)
        {
            ViewBag.nhiemky = "active";
            if (ModelState.IsValid)
            {
                nhiemky.Id = Guid.NewGuid();
                nhiemky.CreateDate = DateTime.Now;
                db.NhiemKies.Add(nhiemky);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nhiemky);
        }

        //
        // GET: /NhiemKy/Edit/5

        public ActionResult Edit(Guid id = new Guid())
        {
            ViewBag.nhiemky = "active";
            NhiemKy nhiemky = db.NhiemKies.Find(id);
            if (nhiemky == null)
            {
                return HttpNotFound();
            }
            return View(nhiemky);
        }

        //
        // POST: /NhiemKy/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NhiemKy nhiemky)
        {
            ViewBag.nhiemky = "active";
            if (ModelState.IsValid)
            {
                db.Entry(nhiemky).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nhiemky);
        }

        //
        // GET: /NhiemKy/Delete/5

        public ActionResult Delete(Guid id)
        {
            ViewBag.nhiemky = "active";
            NhiemKy nhiemky = db.NhiemKies.Find(id);
            if (nhiemky == null)
            {
                return HttpNotFound();
            }
            return View(nhiemky);
        }

        //
        // POST: /NhiemKy/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewBag.nhiemky = "active";
            NhiemKy nhiemky = db.NhiemKies.Find(id);
            db.NhiemKies.Remove(nhiemky);
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
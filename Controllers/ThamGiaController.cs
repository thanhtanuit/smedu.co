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
    public class ThamGiaController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        //
        // GET: /ThamGia/

        public ActionResult Index()
        {
            ViewBag.thamgia = "active";
            List<HoatDong> dsThamGia = db.HoatDongs.OrderByDescending(a => a.CreateDate).ToList();
            List<ChiTietThamGia> result = new List<ChiTietThamGia>();
            foreach (var item in dsThamGia)
            {                              
                ChiTietThamGia cttg = new ChiTietThamGia();
                cttg.Id = item.Id;
                cttg.Name = item.Name;
                cttg.CreateDate = (DateTime)item.CreateDate;
                List<ThamGia> dsthamgia = db.ThamGias.Where(a => a.HoatDongId == item.Id).ToList();
                cttg.SL = dsthamgia.Count;
                result.Add(cttg);
            }

            return View(result);
        }

        //
        // GET: /ThamGia/Details/5

        public ActionResult Details(Guid id = new Guid())
        {
            ViewBag.thamgia = "active";
            HoatDong hoatdong = db.HoatDongs.Find(id);
            List<DoanVien> dsdoanvien = new List<DoanVien>();
            List<ThamGia> dsthamgia = db.ThamGias.Where(a => a.HoatDongId == id).OrderBy(a=>a.UserName).ToList();
            foreach (var item in dsthamgia)
            {
                List<DoanVien> doanvien = db.DoanViens.Where(a => a.UserName == item.UserName).ToList();
                if (doanvien.Count>0)
                {
                    dsdoanvien.Add(doanvien[0]);
                }
            }
            ViewBag.hoatdong = hoatdong;
            return View(dsdoanvien);
        }

        //
        // GET: /ThamGia/Create

        public ActionResult Create()
        {
            ViewBag.thamgia = "active";
            List<HoatDong> hoatdongs = db.HoatDongs.OrderByDescending(a => a.CreateDate).ToList();
            List<DoanVien> doanviens = db.DoanViens.OrderBy(a => a.UserName).ToList();
            List<SelectListItem> listhoatdongs = new List<SelectListItem>();
            foreach (var item in hoatdongs)
            {
                listhoatdongs.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                }); 
            }
            ViewBag.listhoatdongs = listhoatdongs;

            List<SelectListItem> listdoanviens = new List<SelectListItem>();
            foreach (var item in doanviens)
            {
                listdoanviens.Add(new SelectListItem
                {
                    Text = item.UserName,
                    Value = item.UserName
                });
            }
            ViewBag.listdoanviens = listdoanviens;
            
            return View();
        }

        //
        // POST: /ThamGia/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ThamGia thamgia)
        {
            ViewBag.thamgia = "active";
            if (ModelState.IsValid)
            {
                string hoatdongid = Request.Form["hoatdongid"];
                string doanvien = Request.Form["doanvienid"];
                thamgia.Id = Guid.NewGuid();
                thamgia.UserName = doanvien;
                thamgia.CreateDate = DateTime.Now;
                db.ThamGias.Add(thamgia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thamgia);
        }

        //
        // GET: /ThamGia/Edit/5

        public ActionResult Edit(Guid id = new Guid())
        {
            ViewBag.thamgia = "active";
            ThamGia thamgia = db.ThamGias.Find(id);
            if (thamgia == null)
            {
                return HttpNotFound();
            }
            return View(thamgia);
        }

        //
        // POST: /ThamGia/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ThamGia thamgia)
        {
            ViewBag.thamgia = "active";
            if (ModelState.IsValid)
            {
                db.Entry(thamgia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thamgia);
        }

        //
        // GET: /ThamGia/Delete/5

        public ActionResult Delete(Guid id = new Guid())
        {
            ViewBag.thamgia = "active";
            ThamGia thamgia = db.ThamGias.Find(id);
            if (thamgia == null)
            {
                return HttpNotFound();
            }
            return View(thamgia);
        }

        //
        // POST: /ThamGia/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewBag.thamgia = "active";
            ThamGia thamgia = db.ThamGias.Find(id);
            db.ThamGias.Remove(thamgia);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class ChiTietThamGia
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int SL { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
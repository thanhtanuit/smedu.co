using Bidc_Union_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Bidc_Union_Management.Filters;
using PagedList;

namespace Bidc_Union_Management.Controllers
{
    [InitializeSimpleMembership]
    [Authorize(Roles = "Viewer,Admin,Inputter,Authoriser")]
    public class HomeController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        public ActionResult Index()
        {
            return Redirect("/Administrator/Index");
        }

        public ActionResult ListHoatDong(int? page)
        {
            ViewBag.hoatdong = "active";
            DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);
            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<HoatDong> dshoatdong = db.HoatDongs
                                      .Where(a => a.Branch == dv.Branch&a.Status==2)
                                     .OrderByDescending(a => a.CreateDate)
                                     .ToList();

            IPagedList<HoatDong> hds = dshoatdong.ToPagedList(pageIndex, pageSize);
            return View(hds);
        }

        public ActionResult ListCongVan(int? page)
        {
            ViewBag.congvan = "active";
            DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<CongVan> datas = db.CongVans
                                    .Where(a => a.Branch == dv.Branch&a.Status==2)
                                    .OrderByDescending(a => a.CreateDate)
                                     .ToList();

            IPagedList<CongVan> hds = datas.ToPagedList(pageIndex, pageSize);
            return View(hds);
        }
        //public ActionResult Hoatdong()
        //{
        //    List<HoatDong> dshoatdong = db.HoatDongs
        //                               .OrderByDescending(a => a.CreateDate)
        //                               .ToList();
        //    ViewBag.dshoatdong = dshoatdong;
        //    return View();
        //}

        //public ActionResult hoatdongdetail(Guid id)
        //{
        //    ViewBag.hoatdong = "active";
        //    HoatDong hoatdong = db.HoatDongs.Find(id);
        //    return View(hoatdong);
        //}

        //public ActionResult Tintuc()
        //{
        //    List<New> dsnew = db.News
        //                    .OrderByDescending(a => a.CreateDate)
        //                    .ToList();
        //    ViewBag.dsnew = dsnew;
        //    return View();
        //}

        //public ActionResult tintucdetail(Guid id)
        //{
        //    New tintuc = db.News.Find(id);
        //    return View(tintuc);
        //}

        //public ActionResult congvan()
        //{
        //    List<CongVan> dscongvan = db.CongVans
        //                       .OrderByDescending(a => a.CreateDate)
        //                       .ToList();
        //    ViewBag.dscongvan = dscongvan;
        //    return View();
        //}

        //public ActionResult danhsachdoanvien()
        //{
        //    return View();
        //}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Filters;
using Bidc_Union_Management.Models;

namespace Bidc_Union_Management.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Authorize(Roles = "Viewer,Admin,Inputter,Authoriser")]
    public class AdministratorController : Controller
    {
        //
        // GET: /Administrator/

        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        public ActionResult Index()
        {
            DoanVien dv=db.DoanViens.SingleOrDefault(a=>a.UserName==User.Identity.Name);
            List<HoatDong> dshoatdong = db.HoatDongs
                                     .Where(a => a.Branch == dv.Branch&a.Status==2)
                                     .OrderByDescending(a => a.CreateDate)
                                     .Take(4)
                                     .ToList();

            List<HoatDong> dshoatdong1 = new List<HoatDong>();
            List<HoatDong> dshoatdong2 = new List<HoatDong>();

            int i = 0;
            foreach (HoatDong item in dshoatdong)
            {
                if (i%2==0)
                {
                    dshoatdong1.Add(item);
                }
                else
                {
                    dshoatdong2.Add(item);
                }
                i++;
            }


            List<CongVan> dscongvan = db.CongVans
                                .Where(a => a.Branch == dv.Branch&a.Status==2)
                                .OrderByDescending(a => a.CreateDate)
                                .Take(3)
                                .ToList();

            ViewBag.dshoatdong1 = dshoatdong1;
            ViewBag.dshoatdong2 = dshoatdong2;
            //ViewBag.dsnew = dsnew;
            ViewBag.dscongvan = dscongvan;

            return View();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Models;
using System.IO;
using System.Configuration;
using Bidc_Union_Management.Filters;

namespace Bidc_Union_Management.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class NewController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        //
        // GET: /News/

        public ActionResult Index()
        {
            ViewBag.tintuc = "active";
            return View(db.News.Where(a=>a.Status!=3)
                        .OrderByDescending(a => a.CreateDate)
                        .OrderBy(a => a.Status).ToList());
        }

        public ActionResult DeleteAuthorise()
        {
            ViewBag.tintuc = "active";
            return View(db.News.Where(a=>a.Status==3)
                        .OrderByDescending(a => a.CreateDate)
                        .OrderBy(a => a.Status).ToList());
        }

        //
        // GET: /News/Details/5

        public ActionResult Details(Guid id = new Guid())
        {
            ViewBag.tintuc = "active";
            New news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        //
        // GET: /News/Create

        public ActionResult Authorise(Guid id = new Guid())
        {
            ViewBag.tintuc = "active";
            New news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        [HttpPost, ActionName("Authorise")]
        [ValidateAntiForgeryToken]
        public ActionResult AuthoriseConfirmed(Guid id)
        {
            ViewBag.tintuc = "active";
            New news = db.News.Find(id);
            news.Status = 2;            
            db.SaveChanges();
            return Redirect("/New/index");
        }

        public ActionResult Create()
        {
            ViewBag.tintuc = "active";
            return View();
        }

        //
        // POST: /News/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(New news)
        {
            ViewBag.tintuc = "active";
            if (ModelState.IsValid)
            {
                news.Id = Guid.NewGuid();
                news.Inputter = User.Identity.Name.ToLower();
                news.CreateDate = DateTime.Now;

                var file = Request.Files[0];
                if (file.ContentLength>0)
                {
                    HttpPostedFileBase hpf = file as HttpPostedFileBase;
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(hpf.FileName);
                    string path = "~/FileUpLoad/";
                    bool exists = System.IO.Directory.Exists(Server.MapPath(path));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(path));
                    string pathfile = Path.Combine(Server.MapPath(path), fileName);
                    DateTime createdate = DateTime.Now;
                    file.SaveAs(pathfile);
                    news.ImagesNew = fileName;
                    news.Status = 1;
                }
                else
                {
                    news.ImagesNew = "logo.png";
                }                

                db.News.Add(news);
                db.SaveChanges();
                return Redirect("/New/index");
            }

            return View(news);
        }

        //
        // GET: /News/Edit/5

        public ActionResult Edit(Guid id = new Guid())
        {
            ViewBag.tintuc = "active";
            New news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        //
        // POST: /News/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(New news)
        {
            ViewBag.tintuc = "active";
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;

                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    HttpPostedFileBase hpf = file as HttpPostedFileBase;
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(hpf.FileName);
                    string path = "~/FileUpLoad/";
                    bool exists = System.IO.Directory.Exists(Server.MapPath(path));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(path));
                    string pathfile = Path.Combine(Server.MapPath(path), fileName);
                    DateTime createdate = DateTime.Now;
                    file.SaveAs(pathfile);
                    news.ImagesNew = fileName;                   
                }

                news.Status = 1;
                db.SaveChanges();
                return Redirect("/New/index");
            }
            return View(news);
        }

        public ActionResult Restore(Guid id = new Guid())
        {
            ViewBag.tintuc = "active";
            New news = db.News.Find(id);
            news.Status = 2;
            if (news == null)
            {
                return HttpNotFound();
            }
            db.SaveChanges();
            return Redirect("/New/index");
        }

        //
        // GET: /News/Delete/5

        public ActionResult Delete(Guid id = new Guid())
        {
            ViewBag.tintuc = "active";
            New news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        //
        // POST: /News/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewBag.tintuc = "active";
            New news = db.News.Find(id);
            if (news.Status==1 || news.Status==3)
            {
                db.News.Remove(news);
                db.SaveChanges();
            }
            else if (news.Status==2)
            {
                news.Status = 3;
                db.SaveChanges();
            }
           
            return Redirect("/New/index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
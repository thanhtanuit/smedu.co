using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Models;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Bidc_Union_Management.Filters;
using System.Configuration;



namespace Bidc_Union_Management.Controllers
{
    [InitializeSimpleMembership]
    [Authorize(Roles = "Inputter,Authoriser")]
    public class CongVanController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        //
        // GET: /CongVan/

        public ActionResult Index()
        {
            ViewBag.congvan = "active";
            DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);            
            return View(db.CongVans
                .Where(a=>a.Branch==dv.Branch)
                .OrderByDescending(a => a.CreateDate).ToList());
        }

        //
        // GET: /CongVan/Details/5

        public ActionResult Details(Guid id = new Guid())
        {
            ViewBag.congvan = "active";
            CongVan congvan = db.CongVans.Find(id);
            if (congvan == null)
            {
                return HttpNotFound();
            }
            return View(congvan);
        }

        public ActionResult Authorise(Guid id = new Guid())
        {
            ViewBag.congvan = "active";
            CongVan congvan = db.CongVans.Find(id);
            if (congvan == null)
            {
                return HttpNotFound();
            }
            return View(congvan);
        }

        public ActionResult AuthoriseConfirmed(Guid id)
        {
            ViewBag.tintuc = "active";
            CongVan congvan = db.CongVans.Find(id);
            congvan.Status = 2;
            congvan.Authorise = User.Identity.Name;
            db.SaveChanges();

            //Send mail
            string ListUserName=congvan.ListUserName;
            foreach (string item in ListUserName.Split(','))
            {
                try
                {
                    SendEmail(congvan.Name,item, congvan.Link);
                }
                catch (Exception)
                {
                    
                }                
            }
            return Redirect("/CongVan/index");
        }

        //
        // GET: /CongVan/Create

        public ActionResult Create()
        {
            ViewBag.congvan = "active";
            List<SelectListItem> loaicongvan = new List<SelectListItem>();
            loaicongvan.Add(new SelectListItem { Text = "Công văn đến", Value = "1" });
            loaicongvan.Add(new SelectListItem { Text = "Công văn đi", Value = "2" });
            ViewBag.loaicongvan = loaicongvan;

            ViewBag.dsdoanvien = db.DoanViens.OrderBy(a => a.Email).ToList();

            return View();
        }

        //
        // POST: /CongVan/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CongVan congvan)
        {
            ViewBag.congvan = "active";
            List<SelectListItem> loaicongvan = new List<SelectListItem>();
            loaicongvan.Add(new SelectListItem { Text = "Công văn đến", Value = "1" });
            loaicongvan.Add(new SelectListItem { Text = "Công văn đi", Value = "2" });

            ViewBag.loaicongvan = loaicongvan;
            ViewBag.dsdoanvien = db.DoanViens.OrderBy(a => a.Email).ToList();
            string listuser = "";
            if (ModelState.IsValid)
            {
                congvan.Id = Guid.NewGuid();
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    HttpPostedFileBase hpf = file as HttpPostedFileBase;
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(hpf.FileName);
                    string path = "~/FileUpLoad/CongVan/";
                    bool exists = System.IO.Directory.Exists(Server.MapPath(path));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(path));
                    string pathfile = Path.Combine(Server.MapPath(path), fileName);
                    DateTime createdate = DateTime.Now;
                    file.SaveAs(pathfile);
                    congvan.Link = fileName;
                }
                else
                {
                    ViewBag.error = "Chưa chọn file";
                    return View(congvan);
                }

                //-------------Check and send mail ----------------------
                string kt = Request.Form["sendmail"];
                if (kt == "1")
                {
                    List<DoanVien> dsdoanvien = db.DoanViens.ToList();
                    foreach (DoanVien item in dsdoanvien)
                    {
                        if (listuser == "")
                        {
                            listuser = item.Email;
                        }
                        else
                        {
                            listuser = listuser + "," + item.Email;
                        }
                    }
                }

                // Chi gui cho mot so nguoi
                string checksomeone = Request.Form["sendsomemail"];
                if (checksomeone == "2")
                {
                    string dsmail = Request.Form["fname"];
                    if (dsmail != "" & dsmail != null)
                    {
                        listuser = dsmail;
                    }
                }

                //------------- End check and send mail -----------------
                DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);
                congvan.Branch = dv.Branch;
                congvan.Status = 1;
                congvan.Type = int.Parse(Request.Form["Type"]);
                congvan.CreateDate = DateTime.Now;
                congvan.ListUserName = listuser;
                congvan.Inputter = User.Identity.Name;                
                db.CongVans.Add(congvan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(congvan);
        }

        //
        // GET: /CongVan/Edit/5

        public ActionResult Edit(Guid id = new Guid())
        {
            ViewBag.congvan = "active";
            CongVan congvan = db.CongVans.Find(id);
            bool cvden = false;
            bool cvdi = false;
            if (congvan.Type == 1)
            {
                cvden = true;
            }
            if (congvan.Type == 2)
            {
                cvdi = true;
            }
            List<SelectListItem> loaicongvan = new List<SelectListItem>();
            loaicongvan.Add(new SelectListItem { Text = "Công văn đến", Value = "1", Selected = cvden });
            loaicongvan.Add(new SelectListItem { Text = "Công văn đi", Value = "2", Selected = cvdi });
            ViewBag.loaicongvan = loaicongvan;

            if (congvan == null)
            {
                return HttpNotFound();
            }
            return View(congvan);
        }

        //
        // POST: /CongVan/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CongVan congvan)
        {
            ViewBag.congvan = "active";
            if (ModelState.IsValid)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    HttpPostedFileBase hpf = file as HttpPostedFileBase;
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(hpf.FileName);
                    string path = "~/FileUpLoad/CongVan/";
                    bool exists = System.IO.Directory.Exists(Server.MapPath(path));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(path));
                    string pathfile = Path.Combine(Server.MapPath(path), fileName);
                    DateTime createdate = DateTime.Now;
                    file.SaveAs(pathfile);
                    congvan.Link = fileName;
                }

                db.Entry(congvan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(congvan);
        }

        //
        // GET: /CongVan/Delete/5

        public ActionResult Delete(Guid id = new Guid())
        {
            ViewBag.congvan = "active";
            CongVan congvan = db.CongVans.Find(id);
            if (congvan == null)
            {
                return HttpNotFound();
            }
            return View(congvan);
        }

        //
        // POST: /CongVan/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewBag.congvan = "active";
            CongVan congvan = db.CongVans.Find(id);
            db.CongVans.Remove(congvan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void SendEmail(string name, string email, string linkattachment)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("info@bidc.vn");
            mail.To.Add(email);
            mail.Subject = name;
            string html = "Mọi người xem công văn mới tại đây: <a href='http://localhost:3636/FileUpload/CongVan/" + linkattachment + "'>" + name + "</a>";
            mail.Body = html;
            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            string path = "~/FileUpLoad/CongVan/" + linkattachment;
            //making attachment
            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(Server.MapPath(path));
            mail.Attachments.Add(attachment);

            //Gmail Port
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationSettings.AppSettings["email"], ConfigurationSettings.AppSettings["password"]);

            //You can specifiy below line of code either in web.config file or as below.
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
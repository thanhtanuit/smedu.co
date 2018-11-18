using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Models;

namespace Bidc_Union_Management.Controllers
{
        [Authorize(Roles = "Inputter,Authoriser")]
    public class SendMailController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        //
        // GET: /SendMail/

        public ActionResult Index()
        {
            ViewBag.sendmail = "active";
            DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);            
            return View(db.SendMails.Where(a => (a.Status == 1 || a.Status == 2)&a.Branch==dv.Branch)
                          .OrderByDescending(a => a.CreateDate)
                          .ToList());
        }

        public ActionResult DeleteAuthorise()
        {
            ViewBag.sendmail = "active";
            DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);            
            return View(db.SendMails.Where(a => a.Status == 3&a.Branch==dv.Branch).ToList());
        }

        //
        // GET: /SendMail/Details/5

        public ActionResult Details(Guid id = new Guid())
        {
            ViewBag.sendmail = "active";
            SendMail sendmail = db.SendMails.Find(id);
            if (sendmail == null)
            {
                return HttpNotFound();
            }
            return View(sendmail);
        }

        public ActionResult Authorise(Guid id = new Guid())
        {
            ViewBag.sendmail = "active";
            SendMail sendmail = db.SendMails.Find(id);
            if (sendmail == null)
            {
                return HttpNotFound();
            }
            return View(sendmail);
        }

        public ActionResult ConfirmAuthorise(Guid id = new Guid())
        {
            ViewBag.sendmail = "active";
            SendMail sendmail = db.SendMails.Find(id);
            if (sendmail == null)
            {
                return HttpNotFound();
            }

            if (sendmail.Status==1)
            {
                if (sendmail.ListUserName != "")
                {
                    string[] listmail = sendmail.ListUserName.Split(',');
                    foreach (string item in listmail)
                    {
                        string path = Server.MapPath("~/FileUpLoad/CongVan/" + sendmail.Attactment);
                        if (sendmail.Attactment == null || sendmail.Attactment == "")
                        {
                            path = "";
                        }
                        ObjectSendMail.SendEmail(sendmail.Subject, item, path, sendmail.Contents);
                    }
                }
                sendmail.Status = 2;
                sendmail.Authorise = User.Identity.Name;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /SendMail/Create

        public ActionResult Create()
        {
            ViewBag.sendmail = "active";
            ViewBag.dsdoanvien = db.DoanViens.OrderBy(a => a.Email).ToList();
            return View(new SendMail());
        }

        //
        // POST: /SendMail/Create

        [HttpPost]
        public ActionResult Create(SendMail model)
        {
            ViewBag.sendmail = "active";
            ViewBag.dsdoanvien = db.DoanViens.OrderBy(a => a.Email).ToList();

            if (ModelState.IsValid)
            {
                model.Contents = model.Contents.Replace("\n", "<br/>");
                ViewBag.dsdoanvien = db.DoanViens.OrderBy(a => a.Email).ToList();
                model.Id = Guid.NewGuid();
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
                    model.Attactment = fileName;
                }
        
                //-------------Check and send mail ----------------------
                string kt = Request.Form["sendmail"];
                string listsavemail = "";
                if (kt == "1")
                {
                    List<DoanVien> dsdoanvien = db.DoanViens.ToList();
                    foreach (DoanVien item in dsdoanvien)
                    {
                        try
                        {
                            if (listsavemail == "")
                            {
                                listsavemail += item.Email;
                            }
                            else
                            {
                                listsavemail += "," + item.Email;
                            }                          
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                // Chi gui cho mot so nguoi
                string checksomeone = Request.Form["sendsomemail"];
                if (checksomeone == "2")
                {
                    string dsmail = Request.Form["fname"];
                    listsavemail = dsmail;                  
                }

                //------------- End check and send mail -----------------
                model.Status = 1;
                model.ListUserName = listsavemail;
                DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);
                model.Branch = dv.Branch;
                model.CreateDate = DateTime.Now;
                model.Inputter = User.Identity.Name;
                db.SendMails.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Chưa nhập thông tin";
                return View(model);
            }

            return View(model);
        }

        //
        // GET: /SendMail/Delete/5

        public ActionResult Delete(Guid id = new Guid())
        {
            ViewBag.sendmail = "active";
            SendMail sendmail = db.SendMails.Find(id);
            if (sendmail == null)
            {
                return HttpNotFound();
            }
            return View(sendmail);
        }

        //
        // POST: /SendMail/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewBag.sendmail = "active";
            SendMail sendmail = db.SendMails.Find(id);
            db.SendMails.Remove(sendmail);
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
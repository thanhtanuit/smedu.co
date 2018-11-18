using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Models;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Security;

namespace Bidc_Union_Management.Controllers
{
    [Authorize]
    public class HoatDongController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        //
        // GET: /HoatDong/

        public ActionResult Index()
        {
            ViewBag.hoatdong = "active";
            if (Roles.IsUserInRole("Viewer"))
            {
                DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);
                return View(db.HoatDongs
                    .Where(a => a.Status == 2 & a.Branch == dv.Branch)
                    .OrderByDescending(a => a.CreateDate).ToList());
            }
            else
            {
                DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);
                return View(db.HoatDongs
                    .Where(a => a.Status != 3 & a.Branch == dv.Branch)
                    .OrderByDescending(a => a.CreateDate).ToList());
            }

        }

        public ActionResult DeleteAuthorise()
        {
            ViewBag.hoatdong = "active";
            DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);
            return View(db.HoatDongs
               .Where(a => a.Status == 3 & a.Branch == dv.Branch)
               .OrderByDescending(a => a.CreateDate).ToList());
        }

        //
        // GET: /HoatDong/Details/5

        public ActionResult Details(Guid id = new Guid())
        {
            DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);
            ViewBag.hoatdong = "active";
            ViewBag.HoatDongId = id;

            HoatDong hoatdong = db.HoatDongs.Find(id);
            if (hoatdong == null)
            {
                return HttpNotFound();
            }
            List<Bidc_Union_Management.Models.Attachment> dsattachment = db.Attachments.Where(a => a.HoatDongId == id)
                                            .OrderByDescending(a => a.CreateDate)
                                            .ToList();
            ViewBag.dsattachment = dsattachment;
            List<Comment> dscomment = db.Comments.Where(a => a.HoatDongId == id)
                                            .OrderByDescending(a => a.CreateDate)
                                            .ToList();
            ViewBag.dscomment = dscomment;

            List<Notification> dsnotification = db.Notifications.Where(a => a.HoatDongId == id)
                                            .OrderByDescending(a => a.CreateDate)
                                            .ToList();
            List<DoanVien> dsdoanvien = db.DoanViens
                                        .Where(a=>a.Branch==dv.Branch)
                                        .OrderBy(a => a.UserName).ToList();
            List<DoanVien> dschuathem = new List<DoanVien>();
            List<DoanVien> dsdathem = new List<DoanVien>();
            //foreach (Notification item in dsnotification)
            //{
            //    DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == item.UserName);
            //    dsdathem.Add(dv);
            //}

            foreach (DoanVien item in dsdoanvien)
            {
                List<DoanVien> dscheck = dsdathem.Where(a => a.UserName == item.UserName).ToList();
                if (dscheck.Count == 0)
                {
                    dschuathem.Add(item);
                }
            }

            ViewBag.dschuathem = dschuathem;
            ViewBag.dsdathem = dsdathem;

            List<SelectListItem> listdoanviens = new List<SelectListItem>();
            foreach (var item in dschuathem)
            {
                listdoanviens.Add(new SelectListItem
                {
                    Text = item.FullName,
                    Value = item.UserName
                });
            }
            ViewBag.listdoanviens = listdoanviens;

            List<History> dshistory = db.Histories.Where(a => a.HoatDongId == id).ToList();
            ViewBag.dshistory = dshistory;

            if (Roles.IsUserInRole("Inputter") || Roles.IsUserInRole("Authoriser"))
            {
                ViewBag.allow = "1";
            }

            List<ThamGia> dsthamgia = db.ThamGias
                                      .Where(a => a.HoatDongId == id)
                                      .ToList();

            List<DoanVien> dschuathamgia = new List<DoanVien>();
         
            foreach (DoanVien item in dsdoanvien)
            {
                List<ThamGia> check = dsthamgia.Where(a => a.UserName == item.UserName).ToList();
                if (check.Count==0)
                {
                    dschuathamgia.Add(item);
                }               
            }

            List<DoanVien> DoanVienThamGia = new List<DoanVien>();
            foreach (ThamGia item in dsthamgia)
            {
                DoanVien dv1 = db.DoanViens.SingleOrDefault(a => a.UserName == item.UserName);
                if (dv1 != null)
                {
                    DoanVienThamGia.Add(dv1);
                }
            }

            ViewBag.DoanVienThamGia = DoanVienThamGia;
            ViewBag.dsthamgia = dsthamgia;
            ViewBag.dschuathamgia = dschuathamgia;



            return View(hoatdong);
        }

        public ActionResult Authorise(Guid id = new Guid())
        {
            ViewBag.hoatdong = "active";
            HoatDong hoatdong = db.HoatDongs.Find(id);
            if (hoatdong == null)
            {
                return HttpNotFound();
            }
            return View(hoatdong);
        }

        [HttpPost, ActionName("Authorise")]
        [ValidateAntiForgeryToken]
        public ActionResult AuthoriseConfirmed(Guid id)
        {
            ViewBag.hoatdong = "active";
            HoatDong hoatdong = db.HoatDongs.Find(id);
            hoatdong.Status = 2;
            db.SaveChanges();
            return Redirect("/HoatDong/index");
        }

        //
        // GET: /HoatDong/Create

        public ActionResult Create()
        {
            ViewBag.hoatdong = "active";
            return View();
        }

        //
        // POST: /HoatDong/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(HoatDong hoatdong)
        {
            ViewBag.hoatdong = "active";
            if (ModelState.IsValid)
            {
                hoatdong.Id = Guid.NewGuid();
                hoatdong.Inputter = User.Identity.Name.ToLower();
                hoatdong.CreateDate = DateTime.Now;

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
                    hoatdong.Images = fileName;
                }
                else
                {
                    hoatdong.Images = "logo.png";
                }

                DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == User.Identity.Name);
                hoatdong.Branch = dv.Branch;
                hoatdong.Status = 1;

                db.HoatDongs.Add(hoatdong);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hoatdong);
        }

        //
        // GET: /HoatDong/Edit/5

        public ActionResult Edit(Guid id = new Guid())
        {
            ViewBag.hoatdong = "active";
            HoatDong hoatdong = db.HoatDongs.Find(id);
            if (hoatdong == null)
            {
                return HttpNotFound();
            }
            return View(hoatdong);
        }

        //
        // POST: /HoatDong/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(HoatDong hoatdong)
        {
            ViewBag.hoatdong = "active";
            if (ModelState.IsValid)
            {
                db.Entry(hoatdong).State = EntityState.Modified;

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
                    hoatdong.Images = fileName;
                }
                db.SaveChanges();
                AddHistory(hoatdong.Id, "Chỉnh sửa nội dung", User.Identity.Name);

                return RedirectToAction("Index");
            }
            return View(hoatdong);
        }

        public ActionResult Restore(Guid id = new Guid())
        {
            ViewBag.hoatdong = "active";
            HoatDong hoatdong = db.HoatDongs.Find(id);
            hoatdong.Status = 2;
            if (hoatdong == null)
            {
                return HttpNotFound();
            }
            db.SaveChanges();
            return Redirect("/HoatDong/index");
        }

        //
        // GET: /HoatDong/Delete/5

        public ActionResult Delete(Guid id = new Guid())
        {
            ViewBag.hoatdong = "active";
            HoatDong hoatdong = db.HoatDongs.Find(id);
            if (hoatdong == null)
            {
                return HttpNotFound();
            }
            return View(hoatdong);
        }

        //
        // POST: /HoatDong/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewBag.hoatdong = "active";

            HoatDong hoatdong = db.HoatDongs.Find(id);
            if (hoatdong.Status == 1 || hoatdong.Status == 3)
            {
                db.HoatDongs.Remove(hoatdong);
                db.SaveChanges();
            }
            else if (hoatdong.Status == 2)
            {
                hoatdong.Status = 3;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddComment(ObjectComment objectcomment)
        {
            if (objectcomment.content != null & objectcomment.content != "")
            {
                Comment comment = new Comment();
                comment.Id = Guid.NewGuid();
                comment.UserName = User.Identity.Name;
                comment.HoatDongId = Guid.Parse(objectcomment.HoatDongId);
                comment.CreateDate = DateTime.Now;
                comment.Detail = objectcomment.content;
                db.Comments.Add(comment);
                db.SaveChanges();

                AddHistory(Guid.Parse(objectcomment.HoatDongId), "Bình luận", User.Identity.Name);
                //UpdateChange(Guid.Parse(objectcomment.HoatDongId));
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult AddDoanvien(ObjectComment objectcomment)
        {
            if (objectcomment.content != null && objectcomment.content != "")
            {
                Notification notification = new Notification();
                notification.Id = Guid.NewGuid();
                notification.UserName = objectcomment.content;
                notification.HoatDongId = Guid.Parse(objectcomment.HoatDongId); ;
                notification.CreateDate = DateTime.Now;
                db.Notifications.Add(notification);
                db.SaveChanges();
            }


            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult RemoveNotification(ObjectComment objectcomment)
        {
            Guid HoatDongId = Guid.Parse(objectcomment.HoatDongId);
            List<Notification> dsNoti = db.Notifications
                                    .Where(a => a.HoatDongId == HoatDongId
                                        & a.UserName == objectcomment.content)
                                        .ToList();
            if (dsNoti.Count > 0)
            {
                Notification notification = dsNoti[0];
                db.Notifications.Remove(notification);
                db.SaveChanges();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult AddThamGia(ObjectThamgia objectthamgia)
        {
            Guid HoatDongId = Guid.Parse(objectthamgia.HoatDongId);
            ThamGia tg = db.ThamGias.SingleOrDefault(a => a.UserName == objectthamgia.UserName
                                    & a.HoatDongId == HoatDongId);
            if (tg==null)
            {
                tg = new ThamGia();
                tg.Id = Guid.NewGuid();
                tg.CreateDate = DateTime.Now;
                tg.UserName = objectthamgia.UserName;
                tg.HoatDongId = Guid.Parse(objectthamgia.HoatDongId);
                db.ThamGias.Add(tg);
                db.SaveChanges();

            }
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult RemoveThamGia(ObjectThamgia objectthamgia)
        {
            Guid HoatDongId = Guid.Parse(objectthamgia.HoatDongId);
            ThamGia tg = db.ThamGias.SingleOrDefault(a => a.UserName == objectthamgia.UserName
                                    & a.HoatDongId == HoatDongId);
            if (tg != null)
            {                
                db.ThamGias.Remove(tg);
                db.SaveChanges();
            }
            return Json(new { success = true });
        }

        public string GetComment(Guid Id)
        {
            string kq = "";
            kq += "<ul>";
            List<Comment> dscomment = db.Comments.Where(a => a.HoatDongId == Id)
                                    .OrderByDescending(a => a.CreateDate)
                                    .ToList();
            foreach (Comment item in dscomment)
            {
                kq += "<li class='list-act-hom-con'>";
                DateTime dt = (DateTime)item.CreateDate;
                kq += "<i class='fa fa-clock-o' aria-hidden='true'></i>";
                kq += "<h4><span>" + dt.ToString("dd/MM/yyyy HH:mm") + "</span> " + item.UserName + "</h4>";
                kq += item.Detail.Trim();
                kq += "</li>";
            }
            kq += "</ul>";
            return kq;
        }

        public string Getattachments(Guid Id)
        {
            string kq = "";
            kq += "<table class='table table-bordered'>";
            kq += "<thead>";
            kq += "<tr>";
            kq += "<th>File Name</th>";
            kq += "<th>Size</th>";

            kq += "<th>Creator</th>";
            kq += "<th>Created</th>";
            kq += "</tr>";
            kq += "</thead>";
            kq += "<tbody>";
            List<Bidc_Union_Management.Models.Attachment> dsattachment = db.Attachments.Where(a => a.HoatDongId == Id)
                                    .OrderByDescending(a => a.CreateDate)
                                    .ToList();
            foreach (Bidc_Union_Management.Models.Attachment item in dsattachment)
            {
                DateTime dt = (DateTime)item.CreateDate;
                kq += "<tr>";
                kq += "<td><a href='/FileUpload/Attachments/" + item.Link + "' target='_blank' >" + item.Description + "</a></td>";
                kq += "<td>" + item.Size + "</td>";
                kq += "<td>" + item.UserName + "</td>";
                kq += "<td>" + dt.ToString("dd/MM/yyyy HH:mm") + "</td>";
                kq += "</tr>";
            }
            kq += "</table>";
            return kq;
        }

        public string GetNotification(Guid Id)
        {
            List<Notification> dsnotification = db.Notifications.Where(a => a.HoatDongId == Id)
                                                      .OrderByDescending(a => a.CreateDate)
                                                      .ToList();
            List<DoanVien> dsdoanvien = db.DoanViens.OrderBy(a => a.UserName).ToList();
            List<DoanVien> dsdathem = new List<DoanVien>();
            foreach (Notification item in dsnotification)
            {
                DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == item.UserName);
                dsdathem.Add(dv);
            }

            string kq = "<table class='table table-bordered'>";
            kq += "<tbody>";
            foreach (DoanVien item in dsdathem)
            {
                kq += "<tr>";
                kq += "<td>";
                kq += "<a href='Javascript:void(0)' onclick=removenotification('" + item.UserName + "')><i aria-hidden='true' class='fa fa-trash-o'></i></a>";
                kq += "</td>";
                kq += "<td>" + item.FullName + "</td>";
                kq += "</tr>";
            }
            kq += "</tbody>";
            kq += "</table>";
            return kq;
        }

        public string GetHistory(Guid Id)
        {
            List<History> dshistory = db.Histories.Where(a => a.HoatDongId == Id)
                                                      .OrderByDescending(a => a.CreateDate)
                                                      .ToList();
            string kq = "<table class='table table-bordered'>";
            kq += "<thead>";
            kq += "<tr>";
            kq += "<th>Ngày</th>";
            kq += "<th>User</th>";
            kq += "<th>Mô tả</th>";
            kq += "</tr>";
            kq += "</thead>";
            kq += "<tbody>";
            foreach (History item in dshistory)
            {
                DateTime dt = (DateTime)item.CreateDate;
                kq += "<tr>";
                kq += "<td>" + dt.ToString("dd/MM/yyyy HH:mm") + "</td>";
                kq += "<td>" + item.UserName + "</td>";
                kq += "<td>" + item.ItemChanged + "</td>";
                kq += "</tr>";
            }
            kq += "</tbody>";
            kq += "</table>";
            return kq;
        }

        public string AddHistory(Guid HoatDongId, string content, string username)
        {
            History history = new History();
            history.Id = Guid.NewGuid();
            history.UserName = username;
            history.CreateDate = DateTime.Now;
            history.HoatDongId = HoatDongId;
            history.ItemChanged = content;
            db.Histories.Add(history);
            db.SaveChanges();

            return "Sucess";
        }

        [HttpPost]
        public ActionResult UploadFiles(Guid HoatDongId, string Description)
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {

                        HttpPostedFileBase file = files[i];
                        string fname = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string fnamesavedb = fname;

                        // Get the complete folder path and store the file inside it.  
                        if (!Directory.Exists(Server.MapPath("~/FileUpload/Attachments/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/FileUpload/Attachments/"));
                        }
                        fname = Path.Combine(Server.MapPath("~/FileUpload/Attachments/"), fname);
                        file.SaveAs(fname);

                        Bidc_Union_Management.Models.Attachment attachment = new Bidc_Union_Management.Models.Attachment();
                        attachment.Id = Guid.NewGuid();
                        attachment.HoatDongId = HoatDongId;
                        attachment.Description = Description;
                        attachment.Link = fnamesavedb;
                        attachment.Name = file.FileName;
                        if (Description == null || Description == "")
                        {
                            attachment.Description = file.FileName;
                        }
                        attachment.UserName = User.Identity.Name;
                        attachment.Size = SizeToString(file.ContentLength);
                        attachment.CreateDate = DateTime.Now;
                        db.Attachments.Add(attachment);
                        db.SaveChanges();

                    }

                    AddHistory(HoatDongId, "Thêm File đính kèm", User.Identity.Name);
                    // Returns message that successfully uploaded  
                    return Json("Thêm file thành công");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        public void UpdateChange(Guid HoatDongId)
        {
            List<Notification> dsNotification = db.Notifications.Where(a => a.HoatDongId == HoatDongId).ToList();
            foreach (Notification item in dsNotification)
            {
                DoanVien dv = db.DoanViens.SingleOrDefault(a => a.UserName == item.UserName);
                if (dv != null)
                {
                    try
                    {
                        SendEmail(dv.UserName, dv.Email, "");
                    }
                    catch (Exception)
                    {


                    }
                }
            }
        }

        public void SendEmail(string name, string email, string linkattachment)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("info@bidc.vn");
            mail.To.Add(email);
            mail.Subject = name;
            string html = "Cập nhập hoạt động thay đổi";
            mail.Body = html;
            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            string path = "~/FileUpLoad/CongVan/" + linkattachment;
            //making attachment
            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(Server.MapPath(path));
            mail.Attachments.Add(attachment);

            //Gmail Port
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("thanhtanuit@gmail.com", "0633763614@");

            //You can specifiy below line of code either in web.config file or as below.
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }

        public string SizeToString(double Size)
        {
            int quotient = 0;
            double dSize = Size;
            string unit = "Byte";

            while (dSize > 1024.0)
            {
                dSize /= 1024.0;
                quotient++;
            }

            switch (quotient)
            {
                case 1: unit = "KB"; break;
                case 2: unit = "MB"; break;
                case 3: unit = "GB"; break;
                case 4: unit = "TB"; break;
            }

            if (quotient != 0)
            {
                return string.Format("{0:F2} {1}", dSize, unit);
            }

            return string.Format("{0} {1}", dSize, unit);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class ObjectComment
    {
        public string HoatDongId { get; set; }
        public string content { get; set; }
    }

    public class ObjectThamgia
    {
        public string HoatDongId { get; set; }
        public string UserName { get; set; }
    }
}
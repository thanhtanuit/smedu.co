using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Models;
using WebMatrix.WebData;
using Bidc_Union_Management.Filters;
using System.IO;
using System.Web.Security;

namespace Bidc_Union_Management.Controllers
{
    [InitializeSimpleMembership]
    public class DoanVienController : Controller
    {
        private Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        //
        // GET: /DoanVien/

        public ActionResult Index()
        {
            ViewBag.doanvien = "active";
            List<DoanvienModels> dsdoanvien = new List<DoanvienModels>();
            foreach (DoanVien item in db.DoanViens.Where(a=>a.UserName!="admin").OrderByDescending(a => a.CreateDate))
            {
                ChucVu cv = db.ChucVus.Find(item.ChucVuId);
                ChiDoan cd = db.ChiDoans.Find(item.ChiDoanId);
                PhanDoan pd = db.PhanDoans.Find(item.PhanDoanId);
                if (pd==null)
                {
                    pd = new PhanDoan();
                }
                if (cd==null)
                {
                    cd = new ChiDoan();
                }
                dsdoanvien.Add(new DoanvienModels() { Id=item.Id, 
                    UserName=item.UserName, 
                    FullName=item.FullName, 
                    Email=item.Email,
                    Chucvu=cv.Name, 
                    Chidoan=cd.Name, 
                    Phandoan=pd.Name });
            }
            return View(dsdoanvien);
        }

        public ActionResult TreeList()
        {
            ViewBag.doanvien = "active";

            List<ChidoanModel> dschidoan = new List<ChidoanModel>();
            List<ChiDoan> listcd = db.ChiDoans.OrderBy(a => a.Name).ToList();
            foreach (ChiDoan item in listcd)
            {
                List<PhandoanModels> dsphandoan = new List<PhandoanModels>();
                List<PhanDoan> listpd = db.PhanDoans.Where(a => a.ChiDoanId == item.Id).ToList();
                foreach (PhanDoan pd in listpd)
                {
                    List<DoanvienModels> dsdoanvien = new List<DoanvienModels>();
                    List<DoanVien> listdv = db.DoanViens.Where(a => a.PhanDoanId == pd.Id).ToList();
                    foreach (DoanVien dv in listdv)
                    {
                        dsdoanvien.Add(new DoanvienModels() { Id=dv.Id,
                                        UserName=dv.UserName,
                                        FullName=dv.FullName,
                                        Email=dv.Email,
                                        Chucvu=GetChucvu((int)dv.ChucVuId)
                                        });
                    }

                    dsphandoan.Add(new PhandoanModels() { Id = pd.Id, Name = pd.Name, dsdoanvien = dsdoanvien });
                }

                dschidoan.Add(new ChidoanModel() { Id=item.Id,
                            Name=item.Name,
                            CreateDate=(DateTime)item.CreateDate,
                            dsphandoan=dsphandoan}
                            );
            }
            ViewBag.chidoans = dschidoan;
            return View();
        }

        //
        // GET: /DoanVien/Details/5

        public ActionResult Details(Guid id = new Guid())
        {
            ViewBag.doanvien = "active";
            DoanVien doanvien = db.DoanViens.Find(id);
            ChucVu cv = db.ChucVus.Find(doanvien.ChucVuId);
            ChiDoan cd = db.ChiDoans.Find(doanvien.ChiDoanId);
            PhanDoan pd = db.PhanDoans.Find(doanvien.PhanDoanId);
            if (doanvien == null)
            {
                return HttpNotFound();
            }
            DoanvienModels model = new DoanvienModels();
            model.Id = doanvien.Id;
            model.Email = doanvien.Email;
            model.Chucvu = cv.Name;
            model.Chidoan = cd.Name;
            model.Phandoan = pd.Name;
            model.CreateDate = (DateTime)doanvien.CreateDate;
            model.FullName = doanvien.FullName;
            model.UserName = doanvien.UserName;
            model.Country = doanvien.Country;
            model.Avatar = doanvien.Avatar;
            if (model.Birthday!=null)
	        {
                model.Birthday = (DateTime)doanvien.Birthday;
	        }

            string roleArray = "";
            string[] listroles = Roles.GetRolesForUser(doanvien.UserName);
            foreach (string item in listroles)
            {
                if (roleArray == "")
                {
                    roleArray = roleArray + item.ToLower();
                }
                else
                {
                    roleArray = roleArray + "," + item.ToLower();
                }
            }
            string roleadmin = "";
            string roleinput = "";
            string roleauthorise = "";
            string roleview = "";
            if (roleArray.Contains("admin"))
            {
                roleadmin = "checked";
            }

            if (roleArray.Contains("authoriser"))
            {
                roleauthorise = "checked";
            }

            if (roleArray.Contains("inputter"))
            {
                roleinput = "checked";
            }

            if (roleArray.Contains("viewer"))
            {
                roleview = "checked";
            }
            ViewBag.roleadmin = roleadmin;
            ViewBag.roleauthorise = roleauthorise;
            ViewBag.roleinput = roleinput;
            ViewBag.roleview = roleview;

            return View(model);
        }

        //
        // GET: /DoanVien/Create

        public ActionResult Create()
        {
            List<ChucVu> chucvus = db.ChucVus.ToList();
            List<SelectListItem> quyenhans = new List<SelectListItem>();
            foreach (var item in chucvus)
            {
                quyenhans.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.quyenhans = quyenhans;

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

            List<PhanDoan> dsphandoan = db.PhanDoans
                                    .OrderBy(a => a.Name)
                                    .ToList();
            List<SelectListItem> listphandoans = new List<SelectListItem>();
            foreach (var item in dsphandoan)
            {
                listphandoans.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.listphandoans = listphandoans;

            ViewBag.doanvien = "active";
            return View();
        }

        //
        // POST: /DoanVien/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DoanVien doanvien)
        {
            ViewBag.doanvien = "active";
            string roleArray = Request.Form["fname"];
            string roleadmin = "";
            string roleinput = "";
            string roleauthorise = "";
            string roleview = "";
            if (roleArray.Contains("admin"))
            {
                roleadmin = "checked";
            }

            if (roleArray.Contains("authoriser"))
            {
                roleauthorise = "checked";
            }

            if (roleArray.Contains("inputter"))
            {
                roleinput = "checked";
            }

            if (roleArray.Contains("viewer"))
            {
                roleview = "checked";
            }
            ViewBag.roleadmin = roleadmin;
            ViewBag.roleauthorise = roleauthorise;
            ViewBag.roleinput = roleinput;
            ViewBag.roleview = roleview;           

            List<ChucVu> chucvus = db.ChucVus.ToList();
            List<SelectListItem> quyenhans = new List<SelectListItem>();
            foreach (var item in chucvus)
            {
                quyenhans.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.quyenhans = quyenhans;

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

            List<PhanDoan> dsphandoan = db.PhanDoans
                                    .OrderBy(a => a.Name)
                                    .ToList();
            List<SelectListItem> listphandoans = new List<SelectListItem>();
            foreach (var item in dsphandoan)
            {
                listphandoans.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.listphandoans = listphandoans;
            if (ModelState.IsValid)
            {                
                if (doanvien.UserName!=null&doanvien.Password!=null)
                {
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
                        doanvien.Avatar = fileName;
                    }
                    else
                    {
                        doanvien.Avatar = "avatar.jpg";
                    }

                    try
                    {
                        WebSecurity.CreateUserAndAccount(doanvien.UserName, doanvien.Password);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.error = "Tên đăng nhập đã tồn tại";
                        return View(doanvien);
                    }

                    ChiDoan cd = db.ChiDoans.SingleOrDefault(a => a.Id == doanvien.ChiDoanId);
                    doanvien.Branch = cd.Branch;
                    doanvien.Id = Guid.NewGuid();
                    doanvien.CreateDate = DateTime.Now;                    
                    db.DoanViens.Add(doanvien);
                    db.SaveChanges();
                    
                    if (roleArray != "")
                    {
                        Roles.AddUserToRoles(doanvien.UserName, roleArray.Split(','));
                    }
                  
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Tên đăng nhập hoặc mật khẩu không được trống";
                    return View(doanvien);
                }             
            }

            return View(doanvien);
        }

        //
        // GET: /DoanVien/Edit/5

        public ActionResult Edit(Guid id = new Guid())
        {
            ViewBag.doanvien = "active";
            DoanVien doanvien = db.DoanViens.Find(id);
            if (doanvien == null)
            {
                return HttpNotFound();
            }
            string roleArray = "";
            string[] listroles = Roles.GetRolesForUser(doanvien.UserName);
            foreach (string item in listroles)
            {
                if (roleArray=="")
                {
                    roleArray=roleArray+item.ToLower();
                }
                else
                {
                    roleArray = roleArray + "," + item.ToLower();
                }
            }
            string roleadmin = "";
            string roleinput = "";
            string roleauthorise = "";
            string roleview = "";
            if (roleArray.Contains("admin"))
            {
                roleadmin = "checked";
            }

            if (roleArray.Contains("authoriser"))
            {
                roleauthorise = "checked";
            }

            if (roleArray.Contains("inputter"))
            {
                roleinput = "checked";
            }

            if (roleArray.Contains("viewer"))
            {
                roleview = "checked";
            }
            ViewBag.roleadmin = roleadmin;
            ViewBag.roleauthorise = roleauthorise;
            ViewBag.roleinput = roleinput;
            ViewBag.roleview = roleview;
       
            bool cv = false;
            bool cd = false;
            bool pd = false;
            List<ChucVu> chucvus = db.ChucVus.ToList();
            List<SelectListItem> quyenhans = new List<SelectListItem>();
            foreach (var item in chucvus)
            {
                if (item.Id==doanvien.ChucVuId)
                {
                    cv = true;
                }
                else
                {
                    cv = false;
                }
                quyenhans.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString(),
                    Selected=cv
                });
            }

            ViewBag.quyenhans = quyenhans;

            List<ChiDoan> dschidoan = db.ChiDoans
                                    .OrderBy(a => a.Name)
                                    .ToList();
            List<SelectListItem> listchidoans = new List<SelectListItem>();
            foreach (var item in dschidoan)
            {
                if (item.Id==doanvien.ChiDoanId)
                {
                    cd = true;
                }
                else
                {
                    cd = false;
                }
                listchidoans.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString(),
                    Selected=cd
                });
            }
            ViewBag.listchidoans = listchidoans;

            List<PhanDoan> dsphandoan = db.PhanDoans
                                    .OrderBy(a => a.Name)
                                    .ToList();
            List<SelectListItem> listphandoans = new List<SelectListItem>();
            foreach (var item in dsphandoan)
            {
                if (item.Id==doanvien.PhanDoanId)
                {
                    pd = true;
                }
                else
                {
                    pd = false;
                }
                listphandoans.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString(),
                    Selected=pd
                });
            }
            ViewBag.listphandoans = listphandoans;
            return View(doanvien);
        }

        //
        // POST: /DoanVien/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DoanVien doanvien)
        {
            ViewBag.doanvien = "active";

            string roleArray = Request.Form["fname"];
            string roleadmin = "";
            string roleinput = "";
            string roleauthorise = "";
            string roleview = "";
            if (roleArray.Contains("admin"))
            {
                roleadmin = "checked";
            }

            if (roleArray.Contains("authoriser"))
            {
                roleauthorise = "checked";
            }

            if (roleArray.Contains("inputter"))
            {
                roleinput = "checked";
            }

            if (roleArray.Contains("viewer"))
            {
                roleview = "checked";
            }
            ViewBag.roleadmin = roleadmin;
            ViewBag.roleauthorise = roleauthorise;
            ViewBag.roleinput = roleinput;
            ViewBag.roleview = roleview;

            if (ModelState.IsValid)
            {
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
                    doanvien.Avatar = fileName;
                }

                ChiDoan cd = db.ChiDoans.SingleOrDefault(a => a.Id == doanvien.ChiDoanId);
                doanvien.Branch = cd.Branch;

                db.Entry(doanvien).State = EntityState.Modified;              
                db.SaveChanges();

                // Remove all roles
                string[] listroles = Roles.GetRolesForUser(doanvien.UserName);
                if (listroles.Count()>0)
                {
                    Roles.RemoveUserFromRoles(doanvien.UserName, listroles);
                }                

                // Add roles
                if (roleArray != "")
                {
                    Roles.AddUserToRoles(doanvien.UserName, roleArray.Split(','));
                }

                return RedirectToAction("Index");
            }
            return View(doanvien);
        }

        //
        // GET: /DoanVien/Delete/5

        public ActionResult Delete(Guid id = new Guid())
        {
            ViewBag.doanvien = "active";
            DoanVien doanvien = db.DoanViens.Find(id);
            ChucVu cv = db.ChucVus.Find(doanvien.ChucVuId);
            ChiDoan cd = db.ChiDoans.Find(doanvien.ChiDoanId);
            PhanDoan pd = db.PhanDoans.Find(doanvien.PhanDoanId);
            if (doanvien == null)
            {
                return HttpNotFound();
            }
            DoanvienModels model = new DoanvienModels();
            model.Id = doanvien.Id;
            model.Email = doanvien.Email;
            model.Chucvu = cv.Name;
            model.Chidoan = cd.Name;
            model.Phandoan = pd.Name;
            model.CreateDate = (DateTime)doanvien.CreateDate;
            model.FullName = doanvien.FullName;
            model.UserName = doanvien.UserName;
            model.Country = doanvien.Country;
            model.Avatar = doanvien.Avatar;
            if (model.Birthday != null)
            {
                model.Birthday = (DateTime)doanvien.Birthday;
            }

            string roleArray = "";
            string[] listroles = Roles.GetRolesForUser(doanvien.UserName);
            foreach (string item in listroles)
            {
                if (roleArray == "")
                {
                    roleArray = roleArray + item.ToLower();
                }
                else
                {
                    roleArray = roleArray + "," + item.ToLower();
                }
            }
            string roleadmin = "";
            string roleinput = "";
            string roleauthorise = "";
            string roleview = "";
            if (roleArray.Contains("admin"))
            {
                roleadmin = "checked";
            }

            if (roleArray.Contains("authoriser"))
            {
                roleauthorise = "checked";
            }

            if (roleArray.Contains("inputter"))
            {
                roleinput = "checked";
            }

            if (roleArray.Contains("viewer"))
            {
                roleview = "checked";
            }
            ViewBag.roleadmin = roleadmin;
            ViewBag.roleauthorise = roleauthorise;
            ViewBag.roleinput = roleinput;
            ViewBag.roleview = roleview;

            return View(model);
        }

        public string GetChucvu(int chucvuid)
        {
            Bidc_Union_ManagementEntities db = new Bidc_Union_Management.Models.Bidc_Union_ManagementEntities();
            ChucVu cv = db.ChucVus.Find(chucvuid);
            if (cv == null)
            {
                return "";
            }
            return cv.Name;
        }

        //
        // POST: /DoanVien/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ViewBag.doanvien = "active";
            DoanVien doanvien = db.DoanViens.Find(id);
            db.DoanViens.Remove(doanvien);
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
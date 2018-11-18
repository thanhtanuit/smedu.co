using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Filters;
using Bidc_Union_Management.Models;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

namespace Bidc_Union_Management.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Authorize(Roles = "Viewer,Admin,Authoriser,Inputter")]
    public class SettingAccountController : Controller
    {
        //
        // GET: /SettingAccount/

        Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();

        public ActionResult Index()
        {
            ViewBag.doanvien = "active";
            DoanVien doanvien = db.DoanViens.SingleOrDefault(a=>a.UserName==User.Identity.Name);
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
            model.Branch = doanvien.Branch;
            if (model.Birthday != null)
            {
                model.Birthday = (DateTime)doanvien.Birthday;
            }

            return View(model);      
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Đổi mật khẩu không thành công");
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        ModelState.AddModelError("", "Đổi mật khẩu thành công");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đổi mật khẩu không thành công");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Đổi mật khẩu không thành công");
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        ModelState.AddModelError("", "Đổi mật khẩu thành công");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

          public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

    }
}

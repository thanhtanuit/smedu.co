using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bidc_Union_Management.Models;
using Bidc_Union_Management.ViewModel;
using System.Web.Security;
using Bidc_Union_Management.ViewModel;
using Bidc_Union_Management.Filters;

namespace Bidc_Union_Management.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ManageUsersController : Controller
    {
        //
        // GET: /ManageUsers/

        public ActionResult UsersWithRoles()
        {
            ViewBag.phanquyen = "active";
            UsersContext context = new UsersContext();
            List<UserProfile> dsuser = context.UserProfiles.ToList();
            List<Users_in_Role_ViewModel> usersWithRoles = new List<ViewModel.Users_in_Role_ViewModel>();
            string[] admins = Roles.GetUsersInRole("Admin");
            string[] authorisers = Roles.GetUsersInRole("Authoriser");
            string[] inputters = Roles.GetUsersInRole("Inputter");
            foreach (string item in admins)
            {
                usersWithRoles.Add(new Users_in_Role_ViewModel() { Username = item, Role = "Admin" });
            }

            foreach (string item in authorisers)
            {
                usersWithRoles.Add(new Users_in_Role_ViewModel() { Username = item, Role = "Authoriser" });
            }

            foreach (string item in inputters)
            {
                usersWithRoles.Add(new Users_in_Role_ViewModel() { Username = item, Role = "Inputter" });
            }
            
            return View(usersWithRoles);
        }

        public ActionResult Create()
        {
            ViewBag.phanquyen = "active";
            return View();
        }

        [HttpPost]
        public ActionResult Create(RolesDV model)
        {
            ViewBag.phanquyen = "active";
            Bidc_Union_ManagementEntities db = new Bidc_Union_ManagementEntities();
            List<DoanVien> dv = db.DoanViens.Where(a => a.UserName == model.UserName).ToList();
            if (dv.Count==0)
            {
                ViewBag.error="Người dùng không tồn tại";
                return View(model);
            }

            if (!Roles.RoleExists(model.RoleName))
            {
                ViewBag.error = "Quyền không tồn tại";
                return View(model);
            }

            if (Roles.IsUserInRole(model.UserName,model.RoleName))
            {
                ViewBag.error = "Người dùng đã được gán quyền";
                return View(model);
            }

            Roles.AddUserToRole(model.UserName, model.RoleName);
            return Redirect("/ManageUsers/UsersWithRoles");
        }

        public ActionResult DeleteRoles(string username, string roles)
        {
            ViewBag.phanquyen = "active";
            if (Roles.IsUserInRole(username, roles))
            {
                Roles.RemoveUserFromRole(username, roles);
            }

            return Redirect("/ManageUsers/UsersWithRoles");
        }

    }
}

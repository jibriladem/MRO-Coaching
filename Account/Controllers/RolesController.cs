using MROCoatching.Abstractions.Identity;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using MROCoatching.DataObjects.ViewModel.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Controllers
{
    [Area("Account")]
    [DisplayName("AccountMangement")]
    //[GroundServicePlanningAuthorizationFilter]
    public class RolesController : Controller
    {
        private ApplicationDbContext _db;
        readonly IETRoleManager etRoleManager;
        readonly IETUserManager etUserManager;

        public RoleManager<ApplicationRole> RoleManager { get; private set; }

        public RolesController(IETRoleManager _etRoleManager, ApplicationDbContext db, RoleManager<ApplicationRole> _roleManager, IETUserManager _userManager)
        {
            _db = db;
            etRoleManager = _etRoleManager;
            RoleManager = _roleManager;
            etUserManager = _userManager;
        }

        //
        // GET: /Roles/Index
        [HttpGet]
        [DisplayName("AccountMangement")]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Index()
        {
            var rolesList = new List<RoleViewModel>();

            foreach (var role in _db.Roles.ToList())
            {
                var roleModel = new RoleViewModel((ApplicationRole)role);
                rolesList.Add(roleModel);
            }

            if (TempData["SuccessAlertMessage"] != null)
            {
                ViewBag.SuccessAlertMessage = TempData["SuccessAlertMessage"];
                TempData["SuccessAlertMessage"] = null;
            }

            if (TempData["FailureAlertMessage"] != null)
            {
                ViewBag.FailureAlertMessage = TempData["FailureAlertMessage"];
                TempData["FailureAlertMessage"] = null;
            }
            return View(rolesList);
        }

        //
        // GET: /Roles/Create
        [HttpGet]
        [DisplayName("AccountMangement")]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Create(string message = "")
        {
            ViewBag.Privileges = _db.ApplicationPrivileges.OrderBy(p => p.Action).ToList();
            //ViewBag.Roles = new SelectList(_db.Roles.Select(x => x.Name).Distinct());
            ViewBag.Message = message;
            return View();
        }

        //
        // POST: /Roles/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AdmiLteAuthorizationFilter]
        public async Task<ActionResult> Create([Bind("RoleName,Description")] RoleViewModel model)
        {
            string message = "That role name has already been used";
            string messagePrivilege = "The role must have at least one corresponding privilege";
            string pri = Request.Form["privilege"];
            if (ModelState.IsValid && pri != null)
            {
                if (await etRoleManager.RoleExists(model.RoleName))
                    return View(message);
                else
                {
                    if (await etRoleManager.CreateRole(model.RoleName, model.Description))
                    {
                        var role = _db.Roles.First(r => r.Name == model.RoleName);
                        string[] privileges = pri.Split(',');
                        foreach (var item in privileges)
                        {
                            _db.ApplicationRolePrivileges.Add(new ApplicationRolePrivilege { RoleId = role.Id, PrivilegeId = item });
                        }
                        _db.SaveChanges();
                    }
                    TempData["SuccessAlertMessage"] = "Role successfully inserted.";
                    return RedirectToAction("Index", "Roles", new { area = "Account" });
                }
            }
            return View(messagePrivilege);
        }

        //
        // GET: /Roles/Edit
        [HttpGet]
        [DisplayName("AccountMangement")]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Edit(string name)
        {
            string selected = "";
            var role = _db.Roles.First(r => r.Name == name);
            List<ApplicationRolePrivilege> p = _db.ApplicationRolePrivileges.Where(x => x.RoleId == role.Id).ToList();
            foreach (var item in p)
            {
                selected += item.PrivilegeId + ",";
            }

            ViewBag.Selected = selected;
            ViewBag.Privileges = _db.ApplicationPrivileges.OrderBy(pr => pr.Action).ToList();

            var model = new RoleViewModel
            {
                RoleName = role.Name,
                Description = role.Description
            };
            return View(model);
        }

        //
        // POST: /Roles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Edit(RoleViewModel model)
        {
            if (model != null)
            {
                string privilege = Request.Form["privilege"];
                if (privilege != null)
                {
                    var role = _db.Roles.FirstOrDefault(x => x.Name == model.RoleName);

                    if (role != null)
                    {
                        List<ApplicationRolePrivilege> p = _db.ApplicationRolePrivileges.Where(x => x.RoleId == role.Id).ToList();
                        _db.ApplicationRolePrivileges.RemoveRange(p);
                        string[] roles = privilege.Split(',');
                        foreach (var item in roles)
                        {
                            ApplicationRolePrivilege pri = new ApplicationRolePrivilege();
                            pri.PrivilegeId = item;
                            pri.RoleId = role.Id;
                            _db.ApplicationRolePrivileges.Add(pri);
                        }
                        _db.SaveChanges();

                        if (!string.IsNullOrEmpty(model.Description) && !string.Equals(model.Description, role.Description, System.StringComparison.OrdinalIgnoreCase))
                        {
                            role.Description = model.Description;
                            _db.Entry(role).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                        TempData["SuccessAlertMessage"] = "Role successfully updated.";
                        return RedirectToAction("Index", "Roles", new { area = "Account" });
                    }
                }
            }
            return View(model);
        }

        //
        // GET: /Roles/Delete        
        [HttpGet]
        [DisplayName("AccountMangement")]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Delete(string name)
        {
            RoleViewModel model;
            if (name == null)
            {
                return RedirectToAction("BadRequest", "Errors");
            }
            var role = _db.Roles.First(r => r.Name == name);
            //var exist = role.Users.FirstOrDefault(u => u.RoleId == role.Id);

            var exist = _db.UserRoles.FirstOrDefault(u => u.RoleId == role.Id);

            if (exist != null)
            {
                TempData["FailureAlertMessage"] = "You cannot delete this role. It is given to some users.";
                return RedirectToAction("Index", "Roles", new { area = "Account" });
            }
            else
            {
                model = new RoleViewModel((ApplicationRole)role);
            }
            return View(model);
        }

        //
        // POST: /Roles/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[AdmiLteAuthorizationFilter]
        public async Task<ActionResult> DeleteConfirmed(string name)
        {
            var role = _db.Roles.First(r => r.Name == name);
            var privilege = _db.ApplicationRolePrivileges.Where(p => p.RoleId == role.Id);

            foreach (var item in privilege)
                _db.ApplicationRolePrivileges.Remove(item);

            await etUserManager.DeleteRole(role.Id);

            TempData["SuccessAlertMessage"] = "Role successfully deleted.";

            return RedirectToAction("Index", "Roles", new { area = "Account" });
        }
    }
}
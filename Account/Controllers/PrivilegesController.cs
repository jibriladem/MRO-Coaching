using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MROCoatching.Web.Controllers
{
    [Area("Account")]
    [DisplayName("AccountMangement")]
    // [GroundServicePlanningAuthorizationFilter]
    public class PrivilegesController : Controller
    {
        private ApplicationDbContext db;
        //private readonly IServiceProvider _serviceProvider;
        public PrivilegesController(ApplicationDbContext _db/*, IServiceProvider _serviceProvider*/)
        {
            db = _db;
            //serviceProvider = _serviceProvider;
        }
        //
        // GET: /Privileges/Index
        [DisplayName("AccountMangement")]
        [HttpGet]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Index()
        {
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

            return View(db.ApplicationPrivileges.OrderBy(p => p.Action).ToList());
        }

        //
        // GET: /Privileges/Create
        [HttpGet]
        [DisplayName("AccountMangement")]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Create(string message = "")
        {
            ViewBag.Message = message;
            return View();
        }

        //
        // POST: /Privileges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Create([Bind("Id,Action,Description")] ApplicationPrivilege model)
        {
            string message = "That privilege name has already been used";
            if (ModelState.IsValid)
            {
                ApplicationPrivilege pre = db.ApplicationPrivileges.Find(model.Id);
                if (pre != null)
                {
                    return View(message);
                }
                else
                {
                    model.Id = Guid.NewGuid().ToString();
                    db.ApplicationPrivileges.Add(model);
                    db.SaveChanges();
                    TempData["SuccessAlertMessage"] = "Privilege has been successfully created.";
                    return RedirectToAction("Index", "Privileges", new { area = "Account" });
                }
            }
            return View();
        }

        //
        // GET: /Privileges/Edit
        [HttpGet]
        [DisplayName("AccountMangement")]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Edit(string paction)
        {
            if (paction == null)
            {
                return RedirectToAction("BadRequest", "Errors");
            }
            ApplicationPrivilege previlege = db.ApplicationPrivileges.First(p => p.Action == paction);
            if (previlege == null)
            {
                return RedirectToAction("NotFound", "Errors");
            }
            return View(previlege);
        }

        //
        // POST: /Privileges/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Edit([Bind("Id,Action,Description")] ApplicationPrivilege model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessAlertMessage"] = "Privilege has been successfully updated.";
                return RedirectToAction("Index", "Privileges", new { area = "Account" });
            }
            return View(model);
        }
        //
        // GET: /Roles/Delete        
        [HttpGet]
        [DisplayName("AccountMangement")]
        //[AdmiLteAuthorizationFilter]
        public ActionResult Delete(string paction)
        {
            if (paction == null)
            {
                return RedirectToAction("BadRequest", "Errors");
            }
            ApplicationPrivilege privilege = db.ApplicationPrivileges.First(r => r.Action == paction);
            return View(privilege);
        }

        //
        // POST: /Roles/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[AdmiLteAuthorizationFilter]
        public ActionResult DeleteConfirmed(string paction)
        {
            ApplicationPrivilege privilege = db.ApplicationPrivileges.First(p => p.Action == paction);
            List<ApplicationRolePrivilege> rolePrivilege =
                db.ApplicationRolePrivileges.Where(rp => rp.PrivilegeId == privilege.Id).ToList();
            db.ApplicationPrivileges.Remove(privilege);
            db.ApplicationRolePrivileges.RemoveRange(rolePrivilege);
            db.SaveChanges();
            TempData["SuccessAlertMessage"] = "Privilege has been successfully deleted.";
            return RedirectToAction("Index", "Privileges", new { area = "Account" });
        }
    }
}

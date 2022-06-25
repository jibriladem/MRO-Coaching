using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.BasicTransactions;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.BasicTransactions.Controllers
{
    [Area("BasicTransactions")]
    public class PairupplansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.BasicTransactions.Pairupplans _pairs;
        private readonly UserManager<ApplicationUser> _userManager;
        public PairupplansController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _pairs = new DataObjects.Models.BasicTransactions.Pairupplans(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Pairupplans.ToListAsync().ConfigureAwait(false));
        }
        // GET: BasicTransactions/Pairupplans/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sections = await _context.Pairupplans.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (sections == null)
            {
                return NotFound();
            }

            return View(sections);
        }
        // GET: BasicTransactions/Pairupplans/Create
        public IActionResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pairupplans pairupplans)
        {
            if (ModelState.IsValid)
            {
                _pairs.Fullname = pairupplans.Fullname;
                _pairs.Regnumber = pairupplans.Regnumber;
                _pairs.CurrentPosition = pairupplans.CurrentPosition;
                _pairs.LastPromotiondate = pairupplans.LastPromotiondate;
                _pairs.NextPromotiondate = pairupplans.NextPromotiondate;
                _pairs.Pltype = pairupplans.Pltype;
                _pairs.Pllevel = pairupplans.Pllevel;
                _pairs.Coachingstartdate = pairupplans.Coachingstartdate;
                _pairs.Coachingenddate = pairupplans.Coachingenddate;
                _pairs.Coachproposedmgr = pairupplans.Coachproposedmgr;
                _pairs.Coachid = pairupplans.Coachid;
                _pairs.Coachname = pairupplans.Coachname;
                _pairs.Pltype = pairupplans.Pltype;
                _pairs.Pltype = pairupplans.Pltype;
                _pairs.Pltype = pairupplans.Pltype;
                _pairs.Assessorid = pairupplans.Assessorid;
                _pairs.Assessorname = pairupplans.Assessorname;
                _pairs.Remark = pairupplans.Remark;
                _pairs.Status = pairupplans.Status;
                _pairs.ERNAM = User.Identity.Name;
                _pairs.AENAM = User.Identity.Name;
                _pairs.ERDAT = DateTime.Now;
                _pairs.AEDAT = DateTime.Now;
                var exist = await _pairs.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _pairs.Save().ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }
                return new JsonResult(new DatabaseOperationResponse
                {
                    Status = OperationStatus.Exist,
                    Message = "Record Already Exists"
                });
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: BasicTransactions/Pairupplans/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Employees.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: BasicTransactions/Pairupplans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Pairupplans pairupplans)
        {
            if (ModelState.IsValid)
            {
                _pairs.Id = pairupplans.Id;
                _pairs.Fullname = pairupplans.Fullname;
                _pairs.Regnumber = pairupplans.Regnumber;
                _pairs.CurrentPosition = pairupplans.CurrentPosition;
                _pairs.LastPromotiondate = pairupplans.LastPromotiondate;
                _pairs.NextPromotiondate = pairupplans.NextPromotiondate;
                _pairs.Pltype = pairupplans.Pltype;
                _pairs.Pllevel = pairupplans.Pllevel;
                _pairs.Coachingstartdate = pairupplans.Coachingstartdate;
                _pairs.Coachingenddate = pairupplans.Coachingenddate;
                _pairs.Coachproposedmgr = pairupplans.Coachproposedmgr;
                _pairs.Coachid = pairupplans.Coachid;
                _pairs.Coachname = pairupplans.Coachname;
                _pairs.Pltype = pairupplans.Pltype;
                _pairs.Pltype = pairupplans.Pltype;
                _pairs.Pltype = pairupplans.Pltype;
                _pairs.Assessorid = pairupplans.Assessorid;
                _pairs.Assessorname = pairupplans.Assessorname;
                _pairs.Remark = pairupplans.Remark;
                _pairs.Status = pairupplans.Status;
                _pairs.ERNAM = pairupplans.ERNAM;
                _pairs.AENAM = User.Identity.Name;
                _pairs.ERDAT = pairupplans.ERDAT;
                _pairs.AEDAT = DateTime.Now;
                var result = await _pairs.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: BasicTransactions/Pairupplans/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var pairs = await _context.Pairupplans.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _pairs.Id = pairs.Id;
                var result = await _pairs.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }

    }
}

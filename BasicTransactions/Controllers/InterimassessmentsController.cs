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
    public class InterimassessmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.BasicTransactions.Interimassessments _interims;
        private readonly UserManager<ApplicationUser> _userManager;
        public InterimassessmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _interims = new DataObjects.Models.BasicTransactions.Interimassessments(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Interimassessments.ToListAsync().ConfigureAwait(false));
        }
        // GET: BasicTransactions/Interimassessments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sections = await _context.Interimassessments.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (sections == null)
            {
                return NotFound();
            }

            return View(sections);
        }
        // GET: BasicTransactions/Interimassessments/Create
        public IActionResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Interimassessments interimassessments)
        {
            if (ModelState.IsValid)
            {
                _interims.Intname = interimassessments.Intname;
                _interims.Asstarger = interimassessments.Asstarger;
                _interims.Excuted = interimassessments.Excuted;
                _interims.Remark = interimassessments.Remark;
                _interims.Status = interimassessments.Status;
                _interims.Remark = interimassessments.Remark;
                _interims.ERNAM = User.Identity.Name;
                _interims.AENAM = User.Identity.Name;
                _interims.ERDAT = DateTime.Now;
                _interims.AEDAT = DateTime.Now;
                var exist = await _interims.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _interims.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: BasicTransactions/Interimassessments/Edit/5
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
        // POST: BasicTransactions/Interimassessments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Interimassessments interimassessments)
        {
            if (ModelState.IsValid)
            {
                _interims.Id = interimassessments.Id;
               _interims.Intname = interimassessments.Intname;
               _interims.Asstarger = interimassessments.Asstarger;
               _interims.Excuted = interimassessments.Excuted;
               _interims.Remark = interimassessments.Remark;
               _interims.Status = interimassessments.Status;
               _interims.Remark = interimassessments.Remark;
               _interims.ERNAM = interimassessments.ERNAM;
               _interims.AENAM = User.Identity.Name;
               _interims.ERDAT = interimassessments.ERDAT;
                _interims.AEDAT = DateTime.Now;
                var result = await _interims.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: BasicTransactions/Interimassessments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var interims = await _context.Interimassessments.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _interims.Id = interims.Id;
                var result = await _interims.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }


    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AssessingTable;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.AssessingMaster.Controllers
{
    [Area("AssessingMaster")]
    public class AssquestionlimitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.AssessingTable.Assquestionlimits _category;
        private readonly UserManager<ApplicationUser> _userManager;
        public AssquestionlimitsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _category = new DataObjects.Models.AssessingTable.Assquestionlimits(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(await _context.Assquestionlimits.ToListAsync().ConfigureAwait(false));
        }
        // GET: AssessingMaster/Asscategories/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Assquestionlimits.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }

            return View(costcnt);
        }
        // GET: AssessingMaster/Asscategories/Create
        public IActionResult Create()
        {
            ViewData["Assessmenttype"] = _context.Types.Where(c => c.Status == "Y").Select(L => new SelectListItem//PL Types
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assquestionlimits asscategories)
        {
            if (ModelState.IsValid)
            {
                _category.Assessmenttype = asscategories.Assessmenttype;
                _category.Assessmentdesc = asscategories.Assessmentdesc;
                //_category.Percentages = asscategories.Percentages;
                _category.Questioncnt = asscategories.Questioncnt;
                _category.ERNAM = User.Identity.Name;
                _category.AENAM = User.Identity.Name;
                _category.ERDAT = DateTime.Now;
                _category.AEDAT = DateTime.Now;
                var exist = await _category.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _category.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: AssessingMaster/Asscategories/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Assessmenttype"] = _context.Types.Where(c => c.Status == "Y").Select(L => new SelectListItem//PL Types
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Assquestionlimits.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: AssessingMaster/Asscategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Assquestionlimits asscategories)
        {
            if (ModelState.IsValid)
            {
                _category.Id = asscategories.Id;
                _category.Assessmenttype = asscategories.Assessmenttype;
                _category.Assessmentdesc = asscategories.Assessmentdesc;
                //_category.Percentages = asscategories.Percentages;
                _category.Questioncnt = asscategories.Questioncnt;
                _category.ERNAM = asscategories.ERNAM;
                _category.AENAM = User.Identity.Name;
                _category.ERDAT = asscategories.ERDAT;
                _category.AEDAT = DateTime.Now;
                var result = await _category.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: AssessingMaster/Asscategories/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var cat = await _context.Assquestionlimits.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _category.Id = cat.Id;
                var result = await cat.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
    }
}

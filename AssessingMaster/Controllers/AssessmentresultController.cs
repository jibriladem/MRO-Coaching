using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AssessingTable;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.AssessingMaster.Controllers
{
    [Area("AssessingMaster")]
    public class AssessmentresultController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.AssessingTable.Assessmentresult _types;
        private readonly UserManager<ApplicationUser> _userManager;
        public AssessmentresultController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _types = new DataObjects.Models.AssessingTable.Assessmentresult(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(await _context.Asstypes.ToListAsync().ConfigureAwait(false));
        }
        // GET: AssessingMaster/Asstypes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Asstypes.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }

            return View(costcnt);
        }
        // GET: AssessingMaster/Asstypes/Create
        public IActionResult Create()
        {
            var getassessmentqns = _context.Questionaries.AsNoTracking().ToList();
            return View(getassessmentqns);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assessmentresult asstypes)
        {
            if (ModelState.IsValid)
            {
                //_types.Assname = asstypes.Assname;
                //_types.Assdescriptions = asstypes.Assdescriptions;
                //_types.Passmark = asstypes.Passmark;
                _types.Status = (asstypes.checkStatus == true) ? "Y" : "N"; 
                _types.ERNAM = User.Identity.Name;
                _types.AENAM = User.Identity.Name;
                _types.ERDAT = DateTime.Now;
                _types.AEDAT = DateTime.Now;
                var exist = await _types.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _types.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: AssessingMaster/Asstypes/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Asstypes.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: AssessingMaster/Asstypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Assessmentresult asstypes)
        {
            if (ModelState.IsValid)
            {
                _types.Id = asstypes.Id;
                //_types.Assname = asstypes.Assname;
                //_types.Assdescriptions = asstypes.Assdescriptions;
                //_types.Passmark = asstypes.Passmark;
                _types.Status = (asstypes.checkStatus == true) ? "Y" : "N";
                _types.ERNAM = asstypes.ERNAM;
                _types.AENAM = User.Identity.Name;
                _types.ERDAT = asstypes.ERDAT;
                _types.AEDAT = DateTime.Now;
                var result = await _types.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: AssessingMaster/Asstypes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var typ = await _context.Asstypes.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _types.Id = typ.Id;
                var result = await typ.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }

    }
}

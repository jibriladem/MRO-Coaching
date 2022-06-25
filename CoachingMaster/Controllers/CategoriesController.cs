using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.CoachingTable;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.CoachingMaster.Controllers
{
    [Area("CoachingMaster")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.CoachingTable.Categories _category;
        private readonly UserManager<ApplicationUser> _userManager;
        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _category = new DataObjects.Models.CoachingTable.Categories(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(await _context.Categories.ToListAsync().ConfigureAwait(false));
        }
        // GET: CoachingMaster/Categories/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }

            return View(costcnt);
        }
        // GET: CoachingMaster/Categories/Create
        public IActionResult Create()
        {
            ViewData["Pltypes"] = _context.Types.Select(L => new SelectListItem//PL Types
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            ViewData["Forwhom"] = _context.Types.Select(L => new SelectListItem
            {
                Text = L.Costcenter,
                Value = L.Costcenter,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Pltypes,Comptcode,Descriptions,ERNAM,AENAM,ERDAT,AEDAT")] Categories categories)
        {
            if (ModelState.IsValid)
            {
                _category.Pltypes = categories.Pltypes;
                _category.Comptcode = categories.Comptcode;
                _category.Descriptions = categories.Descriptions;
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
        // GET: CoachingMaster/Categories/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Pltypes"] = _context.Types.Select(L => new SelectListItem//PL Types
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            ViewData["Forwhom"] = _context.Types.Select(L => new SelectListItem
            {
                Text = L.Costcenter,
                Value = L.Costcenter,
            }).Distinct();
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Categories.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: CoachingMaster/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Categories categories)
        {
            if (ModelState.IsValid)
            {
                _category.Id = categories.Id;
                _category.Pltypes = categories.Pltypes;
                _category.Comptcode = categories.Comptcode;
                _category.Descriptions = categories.Descriptions;
                _category.ERNAM = categories.ERNAM;
                _category.AENAM = User.Identity.Name;
                _category.ERDAT = categories.ERDAT;
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
        // GET: CoachingMaster/Categories/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _category.Id = category.Id;
                var result = await category.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        public async Task<IActionResult> Getdept(string Dept)
        {
            if (string.IsNullOrEmpty(Dept))
            {
                return NotFound();
            }
            var pl1 = await _context.Departments.Where(con => con.Deptcode == Dept).FirstOrDefaultAsync();
            if (pl1 == null)
            {

            }
            else
            {
                pl1._dbContext = null;
            }
            return new JsonResult(pl1);
        }
        public async Task<IActionResult> Getpositions(string Pltypes)
        {
            if (string.IsNullOrEmpty(Pltypes))
            {
                return NotFound();
            }
            var pl1 = await _context.Positions.Where(con => con.Postid == Pltypes).FirstOrDefaultAsync();
            if (pl1 == null)
            {

            }
            else
            {
                pl1._dbContext = null;
            }
            return new JsonResult(pl1);
        }
    }
}

using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class MenusController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.AccountMaster.Menus _menus;
        private readonly UserManager<ApplicationUser> _userManager;

        public MenusController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _menus = new DataObjects.Models.AccountMaster.Menus(context);
            _userManager = userManager;
        }


        // GET: Account/Menus
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Menus.Include(m => m.ParentMenuId);
            return View(await applicationDbContext.ToListAsync().ConfigureAwait(false));
        }

        // GET: Account/Menus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menus = await _context.Menus
                .Include(m => m.ParentMenuId)
                .FirstOrDefaultAsync(m => m.MenuId == id).ConfigureAwait(false);
            if (menus == null)
            {
                return NotFound();
            }

            return View(menus);
        }

        // GET: Account/Menus/Create
        public IActionResult Create()
        {
            ViewData["ParentId"] = new SelectList(_context.Menus, "MenuId", "Name");
            return View();
        }

        // POST: Account/Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Menus menus)
        {
            if (ModelState.IsValid)
            {
                _menus.MenuId = menus.MenuId;
                _menus.Name = menus.Name;
                _menus.Icon = menus.Icon;
                _menus.Url = menus.Url;
                _menus.ParentId = menus.ParentId;
                _menus.Privilages = menus.Privilages;
                _menus.Description = menus.Description;
                _menus.ACTIND = menus.ACTIND;

                _menus.ERNAM = User.Identity.Name;
                _menus.ERDAT = DateTime.Now;

                var exist = _menus.Exist();
                if (!exist)
                {
                    var result = _menus.Save() as DatabaseOperationResponse;
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
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => !string.IsNullOrEmpty(e.ErrorMessage) ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }

        // GET: Account/Menus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menus = await _context.Menus.FindAsync(id).ConfigureAwait(false);
            if (menus == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(_context.Menus, "MenuId", "Name", menus.ParentId);
            ViewData["Status"] = Enum.GetValues(typeof(RecordStatus)).Cast<RecordStatus>();
            return View(menus);
        }

        // POST: Account/Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Menus menus)
        {
            if (ModelState.IsValid)
            {
                _menus.MenuId = menus.MenuId;
                _menus.Name = menus.Name;
                _menus.Icon = menus.Icon;
                _menus.Url = menus.Url;
                _menus.ParentId = menus.ParentId;
                _menus.Privilages = menus.Privilages;
                _menus.Description = menus.Description;
                _menus.ACTIND = menus.ACTIND;
                _menus.ERNAM = menus.ERNAM;
                _menus.ERDAT = menus.ERDAT;

                _menus.AENAM = User.Identity.Name;
                _menus.AEDAT = DateTime.Now;

                var result = _menus.Update() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => !string.IsNullOrEmpty(e.ErrorMessage) ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }

        // GET: Account/Menus/Delete/5
        public IActionResult Delete(long? id)
        {
            var _menus = _context.Menus.Where(s => s.MenuId == id).FirstOrDefault();
            var result = _menus.Delete() as DatabaseOperationResponse;
            return new JsonResult(result);
        }
        public IActionResult Remove(long? id)
        {
            var _menus = _context.Menus.Where(s => s.MenuId == id).FirstOrDefault();
            _menus.ACTIND = RecordStatus.Deleted;
            var result = _menus.Update() as DatabaseOperationResponse;
            return new JsonResult(result);
        }
    }
}

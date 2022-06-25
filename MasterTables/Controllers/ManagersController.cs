using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.MasterTables;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.MasterTables.Controllers
{
    [Area("MasterTables")]
    public class ManagersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.MasterTables.Managers _managers;
        private readonly UserManager<ApplicationUser> _userManager;
        public ManagersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _managers = new DataObjects.Models.MasterTables.Managers(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Managers.ToListAsync().ConfigureAwait(false));
        }
        // GET: MasterTables/Managers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var managers = await _context.Managers.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (managers == null)
            {
                return NotFound();
            }

            return View(managers);
        }
        // GET: MasterTables/Managers/Create
        public IActionResult Create()
        {
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status1 == "Y" && con.Status == "Y").Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Postid"] = _context.Positions.Select(L => new SelectListItem//Position Code
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Department Code
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Managers managers)
        {
            if (ModelState.IsValid)
            {
                _managers.EmployeeId = managers.EmployeeId;
                _managers.Employeename = managers.Employeename;
                _managers.Status3 = managers.Status3;
                _managers.Costcentercode = managers.Costcentercode;
                _managers.Costcentername = managers.Costcentername;
                _managers.Postid = managers.Postid;
                _managers.Postname = managers.Postname;
                _managers.Deptcode = managers.Deptcode;
                _managers.Deptname = managers.Deptname;
                _managers.Divisions = managers.Divisions;
                _managers.Fromdate = managers.Fromdate;
                _managers.Toodate = managers.Toodate;
                _managers.ERNAM = User.Identity.Name;
                _managers.AENAM = User.Identity.Name;
                _managers.ERDAT = DateTime.Now;
                _managers.AEDAT = DateTime.Now;
                var exist = await _managers.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _managers.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: General/Managers/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status1 == "Y" && con.Status == "Y").Select(L => new SelectListItem//Employee Id
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Postid"] = _context.Positions.Select(L => new SelectListItem//Positionr Code
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Department Code
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();

            if (id == null)
            {
                return NotFound();
            }

            var manage = await _context.Managers.FindAsync(id).ConfigureAwait(false);
            if (manage == null)
            {
                return NotFound();
            }
            else
            {
                if (manage.Costcentername == null)
                {
                    manage.Costcentername = _context.Costcenters.Where(con => con.Costcentercode == manage.Costcentercode).Select(con => con.Costcentername).FirstOrDefault();
                }
                if (manage.Postname == null)
                {
                    manage.Postname = _context.Positions.Where(con => con.Postid == manage.Postid).Select(con => con.Postname).FirstOrDefault();
                }
                if (manage.Deptname == null)
                {
                    manage.Deptname = _context.Departments.Where(con => con.Deptcode == manage.Deptcode).Select(con => con.Deptname).FirstOrDefault();
                }
            }
            return View(manage);
        }
        // POST: MasterTables/Managers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Managers managers)
        {
            if (ModelState.IsValid)
            {
                _managers.Id = managers.Id;
                _managers.EmployeeId = managers.EmployeeId;
                _managers.Employeename = managers.Employeename;
                _managers.Status3 = managers.Status3;
                _managers.Costcentercode = managers.Costcentercode;
                _managers.Costcentername = managers.Costcentername;
                _managers.Postid = managers.Postid;
                _managers.Postname = managers.Postname;
                _managers.Deptcode = managers.Deptcode;
                _managers.Deptname = managers.Deptname;
                _managers.Divisions = managers.Divisions;
                _managers.Fromdate = managers.Fromdate;
                _managers.Toodate = managers.Toodate;
                _managers.ERNAM = managers.ERNAM;
                _managers.AENAM = User.Identity.Name;
                _managers.ERDAT = managers.ERDAT;
                _managers.AEDAT = DateTime.Now;
                var result = await _managers.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: MasterTables/Managers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var account = await _context.Managers.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _managers.Id = account.Id;
                var result = await _managers.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] Managers enteredFiles)
        {
            if (enteredFiles.Files != null)
            {
                using (StreamReader reader1 = new StreamReader(enteredFiles.Files.OpenReadStream()))//, Encoding.UTF8
                {
                    var data = reader1.ReadToEndAsync();
                    string[] strings = data.Result.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);
                    var vals = 0;
                    var managers = new List<Managers>();
                    while (strings.Length > vals)
                    {
                        if (vals == 0)
                        {
                            vals++;
                            continue;
                        }
                        else
                        {
                            var values = strings[vals].Split(",");
                            if (values != null)
                            {
                                managers.Add(new Managers()
                                {
                                    EmployeeId = values[0],
                                    Employeename = values[1],
                                    Status3 = values[2],
                                    Costcentercode = values[3],
                                    Postid = values[4],
                                    Postname = values[5],
                                    Deptcode = values[6],
                                    Divisions = values[7],
                                    Fromdate = Convert.ToDateTime(values[8]),
                                    Toodate = Convert.ToDateTime(values[9]),
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        vals++;
                    }
                    if (managers.Count > 0)
                    {
                        var result = await _managers.SaveList(managers) as DatabaseOperationResponse;
                        return new JsonResult(result);
                    }
                }
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => !string.IsNullOrEmpty(e.ErrorMessage) ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }

    }
}

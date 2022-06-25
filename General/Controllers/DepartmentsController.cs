using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.General;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.Master.General.Controllers
{
    [Area("General")]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.General.Departments _dept;
        private readonly UserManager<ApplicationUser> _userManager;
        public DepartmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _dept = new DataObjects.Models.General.Departments(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Departments.ToListAsync().ConfigureAwait(false));
        }
        // GET: General/Departments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Departments.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }

            return View(costcnt);
        }
        // GET: General/Departments/Create
        public IActionResult Create()
        {
            ViewData["Costcentercode"] = _context.Costcenters.Where(con => con.Status == "Y").Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();

            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Departments departments)
        {
            if (ModelState.IsValid)
            {
                _dept.Deptcode = departments.Deptcode;
                _dept.Deptname = departments.Deptname;
                _dept.Divisions = departments.Divisions;
                //_dept.Divisiondept = departments.Divisiondept;
                _dept.Costcentercode = departments.Costcentercode;
                _dept.Reportsto = departments.Reportsto;
                _dept.Costcentername = departments.Costcentername;
                _dept.Fromdate = departments.Fromdate;
                _dept.Toodate = departments.Toodate;
                _dept.ERNAM = User.Identity.Name;
                _dept.AENAM = User.Identity.Name;
                _dept.ERDAT = DateTime.Now;
                _dept.AEDAT = DateTime.Now;
                var exist = await _dept.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _dept.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: General/Departments/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Costcentercode"] = _context.Costcenters.Where(con => con.Status == "Y").Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Departments.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: General/Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Departments departments)
        {
            if (ModelState.IsValid)
            {
                _dept.Id = departments.Id;
                _dept.Deptcode = departments.Deptcode;
                _dept.Deptname = departments.Deptname;
                _dept.Divisions = departments.Divisions;
                _dept.Costcentercode = departments.Costcentercode;
                _dept.Reportsto = departments.Reportsto;
                _dept.Costcentername = departments.Costcentername;
                _dept.Fromdate = departments.Fromdate;
                _dept.Toodate = departments.Toodate;
                _dept.ERNAM = departments.ERNAM;
                _dept.AENAM = User.Identity.Name;
                _dept.ERDAT = departments.ERDAT;
                _dept.AEDAT = DateTime.Now;
                var result = await _dept.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: OtherMaster/GEN1043/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var account = await _context.Departments.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                if (account != null)
                {
                    var sect = _context.Sections.AsNoTracking().Where(con => con.Deptcode == account.Deptcode).FirstOrDefault();
                    if (sect != null)
                    {
                        return new JsonResult(new DatabaseOperationResponse
                        {
                            Status = OperationStatus.Exist,
                            Message = "Can't Delete this Department, It is in use in Section Master Table"
                        });
                    }
                    else
                    {
                        _dept.Id = account.Id;
                        var result = await _dept.Delete().ConfigureAwait(false) as DatabaseOperationResponse;
                        return new JsonResult(result);
                    }
                }
                return new JsonResult(new DatabaseOperationResponse
                {
                    Status = OperationStatus.NOT_OK,
                    ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
                });
            }
            return new JsonResult(null);
        }
        public async Task<IActionResult> Getdepartments(string Deptcode)
        {
            if (string.IsNullOrEmpty(Deptcode))
            {
                return NotFound();
            }
            var dept = _context.Departments.Where(con => con.Deptcode == Deptcode).FirstOrDefault();

            if (dept == null)
            {
                return NotFound();
            }
            else
            {
                dept._dbContext = null;
            }
            return new JsonResult(dept);
        }
        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] Departments enteredFiles)
        {
            if (enteredFiles.Files != null)
            {
                using (StreamReader reader1 = new StreamReader(enteredFiles.Files.OpenReadStream()))//, Encoding.UTF8
                {
                    var data = reader1.ReadToEndAsync();
                    string[] strings = data.Result.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);
                    var vals = 0;
                    var departments = new List<Departments>();
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
                                departments.Add(new Departments()
                                {
                                    Deptcode = values[0],
                                    Deptname = values[1],
                                    Divisions = values[2],
                                    Costcentercode = values[3],
                                    Costcentername = values[4],
                                    Fromdate = Convert.ToDateTime(values[5]),
                                    Toodate = Convert.ToDateTime(values[6]),
                                    Reportsto = values[7],
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        vals++;
                    }
                    if (departments.Count > 0)
                    {
                        var result = await _dept.SaveList(departments) as DatabaseOperationResponse;
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

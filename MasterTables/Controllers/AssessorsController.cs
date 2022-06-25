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
    public class AssessorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.MasterTables.Assessors _assessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public AssessorsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _assessor = new DataObjects.Models.MasterTables.Assessors(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Assessors.ToListAsync().ConfigureAwait(false));
            //return View();
        }
        // GET: MasterTables/Assessors/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sections = await _context.Assessors.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (sections == null)
            {
                return NotFound();
            }

            return View(sections);
        }
        // GET: MasterTables/Assessors/Create
        public IActionResult Create()
        {
            ViewData["Costcentercode"] = _context.Costcenters.Where(con => con.Status == "Y").Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y").Select(L => new SelectListItem//Employee Id
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Postid"] = _context.Positions.Where(con => con.Status == "Y").Select(L => new SelectListItem//Positions
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Departments
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assessors assessors)
        {
            if (ModelState.IsValid)
            {
                _assessor.EmployeeId = assessors.EmployeeId;
                _assessor.Employeename = assessors.Employeename;
                _assessor.Costcentercode = assessors.Costcentercode;
                _assessor.Costcentername = assessors.Costcentername;
                _assessor.Postid = assessors.Postid;
                _assessor.Postname = assessors.Postname;
                _assessor.Deptname = assessors.Deptname;
                _assessor.Deptname = assessors.Deptname;
                _assessor.Assessortrn = assessors.Assessortrn;
                _assessor.Assessorauth = assessors.Assessorauth;
                _assessor.Fromdate = assessors.Fromdate;
                _assessor.Toodate = assessors.Toodate;
                _assessor.ERNAM = User.Identity.Name;
                _assessor.AENAM = User.Identity.Name;
                _assessor.ERDAT = DateTime.Now;
                _assessor.AEDAT = DateTime.Now;
                var exist = await _assessor.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _assessor.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: General/Employees/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Costcentercode"] = _context.Costcenters.Where(con => con.Status == "Y").Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y").Select(L => new SelectListItem//Employee Id
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Postid"] = _context.Positions.Where(con => con.Status == "Y").Select(L => new SelectListItem//Positions
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Departments
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();
            if (id == null)
            {
                return NotFound();
            }
            var getass = _context.Assessors.Where(c => c.Id == id).FirstOrDefault();
            //var costcnt = await _context.Employees.FindAsync(id).ConfigureAwait(false);
            if (getass == null)
            {
                return NotFound();
            }
            return View(getass);
        }
        // POST: MasterTables/Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Assessors assessors)
        {
            if (ModelState.IsValid)
            {
                _assessor.Id = assessors.Id;
                _assessor.EmployeeId = assessors.EmployeeId;
                _assessor.EmployeeId = assessors.EmployeeId;
                _assessor.Employeename = assessors.Employeename;
                _assessor.Costcentercode = assessors.Costcentercode;
                _assessor.Costcentername = assessors.Costcentername;
                _assessor.Postid = assessors.Postid;
                _assessor.Postname = assessors.Postname;
                _assessor.Deptname = assessors.Deptname;
                _assessor.Deptname = assessors.Deptname;
                _assessor.Assessortrn = assessors.Assessortrn;
                _assessor.Assessorauth = assessors.Assessorauth;
                _assessor.Fromdate = assessors.Fromdate;
                _assessor.Toodate = assessors.Toodate;
                _assessor.ERNAM = assessors.ERNAM;
                _assessor.AENAM = User.Identity.Name;
                _assessor.ERDAT = assessors.ERDAT;
                _assessor.AEDAT = DateTime.Now;
                var result = await _assessor.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: MasterTables/Employees/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var account = await _context.Assessors.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _assessor.Id = account.Id;
                var result = await _assessor.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] Assessors enteredFiles)
        {
            if (enteredFiles.Files != null)
            {
                using (StreamReader reader1 = new StreamReader(enteredFiles.Files.OpenReadStream()))//, Encoding.UTF8
                {
                    var data = reader1.ReadToEndAsync();
                    string[] strings = data.Result.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);
                    var vals = 0;
                    var assessors = new List<Assessors>();
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
                                assessors.Add(new Assessors()
                                {
                                    EmployeeId = values[0],
                                    Employeename = values[1],
                                    Costcentercode = values[2],
                                    //Costcentername = values[3],
                                    Postid = values[3],
                                    Postname = values[4],
                                    Deptcode = values[5],
                                    Assessortrn = values[6],
                                    Assessorauth = values[7],
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
                    if (assessors.Count > 0)
                    {
                        var result = await _assessor.SaveList(assessors) as DatabaseOperationResponse;
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

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
    public class CoachesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.MasterTables.Coaches _coaches;
        private readonly UserManager<ApplicationUser> _userManager;
        public CoachesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _coaches = new DataObjects.Models.MasterTables.Coaches(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Coaches.ToListAsync().ConfigureAwait(false));
            //return View();
        }
        // GET: MasterTables/Coaches/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sections = await _context.Coaches.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (sections == null)
            {
                return NotFound();
            }

            return View(sections);
        }
        // GET: MasterTables/Employees/Create
        public IActionResult Create()
        {
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["EmployeeId"] = _context.Employees.Where(c => c.Status == "Y").Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
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
        public async Task<IActionResult> Create(Coaches coaches)
        {
            if (ModelState.IsValid)
            {
                _coaches.EmployeeId = coaches.EmployeeId;
                _coaches.Employeename = coaches.Employeename;
                _coaches.Costcentercode = coaches.Costcentercode;
                _coaches.Costcentername = coaches.Costcentername;
                _coaches.Deptcode = coaches.Deptcode;
                _coaches.Deptname = coaches.Deptname;
                if (coaches.Training != null)
                {
                    if (coaches.Training == "C")
                    {
                        _coaches.Classroomcoach = "Y";
                        _coaches.Onlinecoach = "N";
                    }
                    else if (coaches.Training == "O")
                    {
                        _coaches.Classroomcoach = "N";
                        _coaches.Onlinecoach = "Y";
                    }
                }
                else
                {
                    return new JsonResult(new DatabaseOperationResponse
                    {
                        Status = OperationStatus.Exist,
                        Message = "Please Select Training Type"
                    });
                }
                //_coaches.Onlinecoach = coaches.Onlinecoach;
                //_coaches.Classroomcoach = coaches.Classroomcoach;
                _coaches.Coachauth = coaches.Coachauth;
                _coaches.Fromdate = coaches.Fromdate;
                _coaches.Toodate = coaches.Toodate;
                _coaches.ERNAM = User.Identity.Name;
                _coaches.AENAM = User.Identity.Name;
                _coaches.ERDAT = DateTime.Now;
                _coaches.AEDAT = DateTime.Now;
                var exist = await _coaches.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _coaches.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: General/Coaches/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["EmployeeId"] = _context.Employees.Where(c => c.Status == "Y").Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
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
            var getval = _context.Coaches.Where(c => c.Id == id).FirstOrDefault();
            //var costcnt = await _context.Employees.FindAsync(id).ConfigureAwait(false);
            if (getval == null)
            {
                return NotFound();
            }
            return View(getval);
        }
        // POST: MasterTables/Coaches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Coaches coaches)
        {
            if (ModelState.IsValid)
            {
                _coaches.Id = coaches.Id;
                _coaches.EmployeeId = coaches.EmployeeId;
                _coaches.EmployeeId = coaches.EmployeeId;
                _coaches.Employeename = coaches.Employeename;
                _coaches.Costcentercode = coaches.Costcentercode;
                _coaches.Costcentername = coaches.Costcentername;
                _coaches.Deptcode = coaches.Deptcode;
                _coaches.Deptname = coaches.Deptname;
                _coaches.Onlinecoach = coaches.Onlinecoach;
                _coaches.Classroomcoach = coaches.Classroomcoach;
                _coaches.Coachauth = coaches.Coachauth;
                _coaches.Fromdate = coaches.Fromdate;
                _coaches.Toodate = coaches.Toodate;
                _coaches.ERNAM = coaches.ERNAM;
                _coaches.AENAM = User.Identity.Name;
                _coaches.ERDAT = coaches.ERDAT;
                _coaches.AEDAT = DateTime.Now;
                var result = await _coaches.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: MasterTables/Coaches/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var account = await _context.Coaches.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _coaches.Id = account.Id;
                var result = await _coaches.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        // GET: MasterTables/Employees/Upload
        //[HttpPost]
        public IActionResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] Coaches enteredFiles)
        {
            if (enteredFiles.Files != null)
            {
                using (StreamReader reader1 = new StreamReader(enteredFiles.Files.OpenReadStream()))//, Encoding.UTF8
                {
                    var data = reader1.ReadToEndAsync();
                    string[] strings = data.Result.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);
                    var vals = 0;
                    var coaches = new List<Coaches>();
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
                                coaches.Add(new Coaches()
                                {
                                    EmployeeId = values[0],
                                    Employeename = values[1],
                                    Costcentercode = values[2],
                                    //Costcentername = values[3],
                                    Deptcode = values[3],
                                    Onlinecoach = values[4],
                                    Classroomcoach = values[5],
                                    Coachauth = values[6],
                                    Fromdate = Convert.ToDateTime(values[7]),
                                    Toodate = Convert.ToDateTime(values[8]),
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        vals++;
                    }
                    if (coaches.Count > 0)
                    {
                        var result = await _coaches.SaveList(coaches) as DatabaseOperationResponse;
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

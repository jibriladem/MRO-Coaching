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
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.Master.MasterTables
{
    [Area("MasterTables")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.MasterTables.Employees _employee;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmployeesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _employee = new DataObjects.Models.MasterTables.Employees(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Employees.Where(c => c.Status == "Y").ToListAsync().ConfigureAwait(false));
            //return View(await _context.Employees.ToListAsync().ConfigureAwait(false));
            //return View();
        }
        // GET: MasterTables/Employees/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sections = await _context.Employees.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
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
            ViewData["PositionId"] = _context.Positions.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employees employees)
        {
            if (ModelState.IsValid)
            {
                _employee.EmployeeId = employees.EmployeeId;
                _employee.Employeename = employees.Employeename;
                _employee.Costcentercode = employees.Costcentercode;
                _employee.Costcentername = employees.Costcentername;
                _employee.Status = employees.Status;
                _employee.Reportsto = employees.Reportsto;
                _employee.Seniordate = employees.Seniordate;
                _employee.PositionId = employees.PositionId;
                _employee.Positiondesc = employees.Positiondesc;
                _employee.Promotiondate = employees.Promotiondate;
                _employee.NextPromdate = employees.NextPromdate;
                //_employee.status1 = (employees.checkStatus1 == true) ? "Y" : "N"; ;
                //_employee.status2 = (employees.checkStatus2 == true) ? "Y" : "N";
                if (employees.Status1 != null)
                {
                    _employee.Status1 = employees.Status1;
                }
                else
                {
                    _employee.Status1 = "N";
                }
                if (employees.Status2 != null)
                {
                    _employee.Status2 = employees.Status2;
                }
                else
                {
                    _employee.Status2 = "N";
                }
                _employee.Empgroup = employees.Empgroup;
                _employee.Fromdate = employees.Fromdate;
                _employee.Toodate = employees.Toodate;
                //_employee.status = (employees.checkStatus == true) ? "Y" : "N"; ;
                _employee.ERNAM = User.Identity.Name;
                _employee.AENAM = User.Identity.Name;
                _employee.ERDAT = DateTime.Now;
                _employee.AEDAT = DateTime.Now;
                if (_employee.PositionId != null)
                {
                    var getnext = _context.Positions.Where(c => c.Postid == _employee.PositionId && c.Costcentercode == _employee.Costcentercode).FirstOrDefault();
                    if (getnext != null)
                    {
                        //select next positions
                        getnext.Positionnbr = getnext.Positionnbr + 1;
                        var getnex1 = _context.Positions.Where(c => c.Costcentercode == _employee.Costcentercode && c.Positionnbr == getnext.Positionnbr).FirstOrDefault();
                        if (getnex1 != null)
                        {
                            _employee.Nextpl = getnex1.Postid;
                        }
                        else
                        {
                            _employee.Nextpl = "";
                        }
                    }
                }
                var exist = await _employee.Exist(); //for all  
                if (!exist)
                {
                    var result = await _employee.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["PositionId"] = _context.Positions.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();

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
        // POST: MasterTables/Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Employees employees)
        {
            if (ModelState.IsValid)
            {
                _employee.Id = employees.Id;
                _employee.EmployeeId = employees.EmployeeId;
                _employee.Employeename = employees.Employeename;
                _employee.Costcentercode = employees.Costcentercode;
                _employee.Reportsto = employees.Reportsto;
                _employee.Costcentername = employees.Costcentername;
                _employee.Seniordate = employees.Seniordate;
                _employee.PositionId = employees.PositionId;
                _employee.Positiondesc = employees.Positiondesc;
                _employee.Promotiondate = employees.Promotiondate;
                _employee.NextPromdate = employees.NextPromdate;
                if (employees.Status1 != null)
                {
                    _employee.Status1 = employees.Status1;
                }
                else
                {
                    _employee.Status1 = "N";
                }
                if (employees.Status2 != null)
                {
                    _employee.Status2 = employees.Status2;
                }
                else
                {
                    _employee.Status2 = "N";
                }
                _employee.Empgroup = employees.Empgroup;
                _employee.Fromdate = employees.Fromdate;
                _employee.Toodate = employees.Toodate;
                //_employee.status = (employees.checkStatus == true) ? "Y" : "N";
                _employee.Status = employees.Status;
                _employee.ERNAM = employees.ERNAM;
                _employee.AENAM = User.Identity.Name;
                _employee.ERDAT = employees.ERDAT;
                _employee.AEDAT = DateTime.Now;
                var result = await _employee.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: MasterTables/Employees/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        [HttpGet]
        public async Task<JsonResult> Delete(long? id)
        {
            if (id != 0)
            {
                var account = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _employee.Id = account.Id;
                var result = await _employee.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        // GET: MasterTables/Employees/Upload
        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] Employees enteredFiles)
        {
            if (enteredFiles.Files != null)
            {
                using (StreamReader reader1 = new StreamReader(enteredFiles.Files.OpenReadStream()))//, Encoding.UTF8
                {
                    var data = reader1.ReadToEndAsync();
                    string[] strings = data.Result.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);
                    //string[] lines = data1.Split(new[] { Environment.NewLine },
                    //        StringSplitOptions.None);
                    var a = 0;
                    var employees = new List<Employees>();
                    while (strings.Length > a)
                    {
                        if (a == 0)
                        {
                            a++;
                            continue;
                        }
                        else
                        {
                            var values = strings[a].Split(",");
                            if (values != null)
                            {
                                employees.Add(new Employees()
                                {
                                    EmployeeId = values[0],//empls.EmployeeId,
                                    Employeename = values[1],//empls.Employeename,
                                    Costcentercode = values[2],//empls.Costcentercode,
                                    Costcentername = values[3],//empls.Costcentername,
                                    Seniordate = Convert.ToDateTime(values[4]),//empls.Seniordate,
                                    PositionId = values[5],//empls.PositionId,
                                    Positiondesc = values[6],//empls.Positiondesc,
                                    Promotiondate = Convert.ToDateTime(values[7]),//empls.Promotiondate,
                                    NextPromdate = Convert.ToDateTime(values[8]),//empls.NextPromdate,
                                    Status = "Y",
                                    Empgroup = "P",
                                    Status1 = values[9],//empls.status1,
                                    Status2 = values[10],//empls.status2,
                                    Fromdate = Convert.ToDateTime(values[11]),//empls.Fromdate,
                                    Toodate = Convert.ToDateTime(values[12]),//empls.Toodate,
                                    Reportsto = values[13],
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        a++;
                    }
                    if (employees.Count > 0)
                    {
                        var result = await _employee.SaveList(employees) as DatabaseOperationResponse;
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
        public async Task<IActionResult> Getemployee(string EmployeeId)
        {
            if (string.IsNullOrEmpty(EmployeeId))
            {
                return NotFound();
            }
            var employees = await _context.Employees.Where(con => con.EmployeeId == EmployeeId).FirstOrDefaultAsync();

            if (employees == null)
            {
                return NotFound();
            }
            else
            {
                employees._dbContext = null;
            }
            return new JsonResult(employees);
        }
    }
}
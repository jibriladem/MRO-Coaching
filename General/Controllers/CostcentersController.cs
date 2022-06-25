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
    public class CostcentersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.General.Costcenters _costconter;
        private readonly UserManager<ApplicationUser> _userManager;
        public CostcentersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _costconter = new DataObjects.Models.General.Costcenters(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(await _context.Costcenters.ToListAsync().ConfigureAwait(false));
        }
        // GET: General/Costcenters/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Costcenters.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }

            return View(costcnt);
        }
        // GET: General/Costcenters/Create
        public IActionResult Create()
        {
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();

            ViewData["Sectcode"] = _context.Sections.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Sectcode,
                Value = L.Sectcode,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Costcenters costCenter)
        {
            if (ModelState.IsValid)
            {
                _costconter.Costcentercode = costCenter.Costcentercode;
                _costconter.Costcentername = costCenter.Costcentername;
                //_costconter.Status = (costCenter.chk_Status == true) ? "Y" : "N";
                _costconter.Costcenterdesc = costCenter.Costcenterdesc;
                _costconter.Fromdate = costCenter.Fromdate;
                _costconter.Toodate = costCenter.Toodate;
                _costconter.ERNAM = User.Identity.Name;
                _costconter.AENAM = User.Identity.Name;
                _costconter.ERDAT = DateTime.Now;
                _costconter.AEDAT = DateTime.Now;
                if (_costconter.Toodate > DateTime.Now)
                {
                    _costconter.Status = "Y";
                }
                else
                {
                    _costconter.Status = "N";
                }
                var exist = await _costconter.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _costconter.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: General/Costcenters/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();
            ViewData["Sectcode"] = _context.Sections.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Sectcode,
                Value = L.Sectcode,
            }).Distinct();
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Costcenters.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: General/Costcenters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Costcenters costCenter)
        {
            if (ModelState.IsValid)
            {
                _costconter.Id = costCenter.Id;
                _costconter.Costcentercode = costCenter.Costcentercode;
                _costconter.Costcentername = costCenter.Costcentername;
                _costconter.Status = (costCenter.chk_Status == true) ? "Y" : "N";
                _costconter.Costcenterdesc = costCenter.Costcenterdesc;
                _costconter.Fromdate = costCenter.Fromdate;
                _costconter.Toodate = costCenter.Toodate;
                _costconter.ERNAM = costCenter.ERNAM;
                _costconter.AENAM = User.Identity.Name;
                _costconter.ERDAT = costCenter.ERDAT;
                _costconter.AEDAT = DateTime.Now;
                var result = await _costconter.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: General/Costcenters/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var account = await _context.Costcenters.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                //check from department
                if (account != null)
                {
                    var dept = _context.Departments.AsNoTracking().Where(con => con.Costcentercode == account.Costcentercode).FirstOrDefault();
                    if (dept != null)
                    {
                        return new JsonResult(new DatabaseOperationResponse
                        {
                            Status = OperationStatus.Exist,
                            Message = "Can't Delete this Cost Center, It is in use in Department Master Table"
                        });
                    }
                    else
                    {
                        //check from section
                        var sect = _context.Sections.AsNoTracking().Where(con => con.Costcentercode == account.Costcentercode).FirstOrDefault();
                        if (sect != null)
                        {
                            return new JsonResult(new DatabaseOperationResponse
                            {
                                Status = OperationStatus.Exist,
                                Message = "Can't Delete this Cost Center, It is in use in Section Master Table"
                            });
                        }
                        else
                        {
                            //check from sub section
                            var teams = _context.TeamLeader.AsNoTracking().Where(con => con.Costcentercode == account.Costcentercode).FirstOrDefault();
                            if (teams != null)
                            {
                                return new JsonResult(new DatabaseOperationResponse
                                {
                                    Status = OperationStatus.Exist,
                                    Message = "Can't Delete this Cost Center, It is in use in Sub Section Master Table"
                                });
                            }
                            else
                            {
                                // check from employee
                                var empl = _context.Employees.AsNoTracking().Where(con => con.Costcentercode == account.Costcentercode).FirstOrDefault();
                                if (empl != null)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.Exist,
                                        Message = "Can't Delete this Cost Center, It is in use in Employee Master Table"
                                    });
                                }
                                else
                                {
                                    //check from assessors
                                    var ass = _context.Assessors.AsNoTracking().Where(con => con.Costcentercode == account.Costcentercode).FirstOrDefault();
                                    if (ass != null)
                                    {
                                        return new JsonResult(new DatabaseOperationResponse
                                        {
                                            Status = OperationStatus.Exist,
                                            Message = "Can't Delete this Cost Center, It is in use in Assessors Master Table"
                                        });
                                    }
                                    else
                                    {
                                        //check from coach
                                        var coach = _context.Coaches.AsNoTracking().Where(con => con.Costcentercode == account.Costcentercode).FirstOrDefault();
                                        if (coach != null)
                                        {
                                            return new JsonResult(new DatabaseOperationResponse
                                            {
                                                Status = OperationStatus.Exist,
                                                Message = "Can't Delete this Cost Center, It is in use in Coaches Master Table"
                                            });
                                        }
                                        else
                                        {
                                            //check from Manager
                                            var mgr = _context.Managers.AsNoTracking().Where(con => con.Costcentercode == account.Costcentercode).FirstOrDefault();
                                            if (mgr != null)
                                            {
                                                return new JsonResult(new DatabaseOperationResponse
                                                {
                                                    Status = OperationStatus.Exist,
                                                    Message = "Can't Delete this Cost Center, It is in use in Managers Master Table"
                                                });
                                            }
                                            else
                                            {
                                                _costconter.Id = account.Id;
                                                var result = await _costconter.Delete().ConfigureAwait(false) as DatabaseOperationResponse;
                                                return new JsonResult(result);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
            //return new JsonResult(null);
        }
        public async Task<IActionResult> Getcostcenter(string Costcentercode)
        {
            //ViewBag["Plname"] = _context.Positions.Where(c => c.Costcentercode == Costcentercode).Select(L => new SelectListItem
            //{
            //    Text = L.Postname,
            //    Value = L.Postid,
            //}).Distinct();
            if (string.IsNullOrEmpty(Costcentercode))
            {
                return NotFound();
            }
            var costcnt = await _context.Costcenters.Where(con => con.Costcentercode == Costcentercode).FirstOrDefaultAsync().ConfigureAwait(false);

            if (costcnt == null)
            {
                return NotFound();
            }
            else
            {
                costcnt._dbContext = null;
            }
            //else
            //{
            //    if (costcnt.Deptcode == null)
            //    {
            //        costcnt.Deptcode = _context.Departments.Where(c => c.Costcentercode == costcnt.Costcentercode).Select(c => c.Deptcode).FirstOrDefault();
            //        if (costcnt.Deptcode == null)
            //        {
            //            var section = _context.Sections.Where(c => c.Costcentercode == costcnt.Costcentercode).Select(c => c.Deptcode).FirstOrDefault();
            //            if (section != null)
            //            {
            //                costcnt.Deptcode = _context.Departments.Where(c => c.Deptcode == section).Select(c => c.Deptcode).FirstOrDefault();
            //            }
            //            else if (costcnt.Deptcode == null)
            //            {
            //                var section1 = _context.TeamLeader.Where(c => c.Costcentercode == costcnt.Costcentercode).Select(c => c.Sectcode).FirstOrDefault();
            //                if (section1 != null)
            //                {
            //                    var section2 = _context.Sections.Where(c => c.Sectcode == section1).Select(c => c.Deptcode).FirstOrDefault();
            //                    if (section2 != null)
            //                    {
            //                        costcnt.Deptcode = _context.Departments.Where(c => c.Deptcode == section2).Select(c => c.Deptcode).FirstOrDefault();
            //                    }
            //                }
            //                else
            //                {

            //                }
            //            }
            //        }
            //    }
            //    costcnt._dbContext = null;
            //}
            return new JsonResult(costcnt);
        }
        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] Costcenters enteredFiles)
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
                    var costcenters = new List<Costcenters>();
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
                                costcenters.Add(new Costcenters()
                                {
                                    Costcentercode = values[0],
                                    Costcentername = values[1],
                                    Costcenterdesc = values[2],
                                    Fromdate = Convert.ToDateTime(values[3]),
                                    Toodate = Convert.ToDateTime(values[4]),
                                    Status = "Y",
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        a++;
                    }
                    if (costcenters.Count > 0)
                    {
                        var result = await _costconter.SaveList(costcenters) as DatabaseOperationResponse;
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

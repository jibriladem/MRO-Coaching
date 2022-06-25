using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.BasicTransactions;
using MROCoatching.DataObjects.Models.General;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.BasicTransactions.Controllers
{
    [Area("BasicTransactions")]
    public class CoachactionplansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.BasicTransactions.Coachactionplans _actionplans;
        private readonly DataObjects.Models.BasicTransactions.Coaching _coaching;
        private readonly UserManager<ApplicationUser> _userManager;
        public CoachactionplansController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _actionplans = new DataObjects.Models.BasicTransactions.Coachactionplans(context);
            _coaching = new DataObjects.Models.BasicTransactions.Coaching(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Coaching.ToListAsync().ConfigureAwait(false));
        }
        // GET: BasicTransactions/Coachactionplans/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sections = await _context.Coachactionplans.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (sections == null)
            {
                return NotFound();
            }

            return View(sections);
        }
        // GET: BasicTransactions/Assessors/Create
        public IActionResult Create()
        {
            ////ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            ////{
            ////    Text = L.Costcentername,
            ////    Value = L.Costcentercode,
            ////}).Distinct();
            ////ViewData["EmployeeId"] = _context.Employees.Select(L => new SelectListItem//Cost Center Code
            ////{
            ////    Text = L.Employeename,
            ////    Value = L.EmployeeId,
            ////}).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coachactionplans coachactionplans)
        {
            if (ModelState.IsValid)
            {
                _actionplans.Fleet = coachactionplans.Fleet;
                _actionplans.Tailnumber = coachactionplans.Tailnumber;
                _actionplans.Taskcode = coachactionplans.Taskcode;
                _actionplans.Taskdesc = coachactionplans.Taskdesc;
                _actionplans.plandate = coachactionplans.plandate;
                _actionplans.Excuted = coachactionplans.Excuted;
                _actionplans.Remark = coachactionplans.Remark;
                _actionplans.ERNAM = User.Identity.Name;
                _actionplans.AENAM = User.Identity.Name;
                _actionplans.ERDAT = DateTime.Now;
                _actionplans.AEDAT = DateTime.Now;
                var exist = await _actionplans.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _actionplans.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: BasicTransactions/Coachactionplans/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentername,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["EmployeeId"] = _context.Employees.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Employeename,
                Value = L.EmployeeId,
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
        // POST: BasicTransactions/Coachactionplans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Coachactionplans coachactionplans)
        {
            if (ModelState.IsValid)
            {
                _actionplans.Id = coachactionplans.Id;
                _actionplans.Fleet = coachactionplans.Fleet;
                _actionplans.Tailnumber = coachactionplans.Tailnumber;
                _actionplans.Taskcode = coachactionplans.Taskcode;
                _actionplans.Taskdesc = coachactionplans.Taskdesc;
                _actionplans.plandate = coachactionplans.plandate;
                _actionplans.Excuted = coachactionplans.Excuted;
                _actionplans.Remark = coachactionplans.Remark;
                _actionplans.ERNAM = coachactionplans.ERNAM;
                _actionplans.AENAM = User.Identity.Name;
                _actionplans.ERDAT = coachactionplans.ERDAT;
                _actionplans.AEDAT = DateTime.Now;
                var result = await _actionplans.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: BasicTransactions/Coachactionplans/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var actionpl = await _context.Coachactionplans.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _actionplans.Id = actionpl.Id;
                var result = await _actionplans.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        public async Task<IActionResult> Actionplan(string Employeeid, string Plcode, string Pllevel)
        {
            if (Employeeid == null)
            {
                return NotFound();
            }
            var getalldata = _context.Coaching.Where(c => c.EmployeeId == Employeeid && c.Actionpln == "N" && c.Plcode == Plcode && c.Pllevel == Pllevel).ToList();
            if (getalldata.Count > 0)
            {
                foreach (var datas in getalldata)
                {
                    datas.Coachingstart = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == datas.EmployeeId && c.Plname == datas.Plcode && c.Pllevel == datas.Pllevel).Select(con => con.Coachingstartdate).FirstOrDefault();
                    datas.Coachingend = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == datas.EmployeeId && c.Plname == datas.Plcode && c.Pllevel == datas.Pllevel).Select(con => con.Coachingenddate).FirstOrDefault();
                    datas.Whenassess = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == datas.EmployeeId && c.Plname == datas.Plcode && c.Pllevel == datas.Pllevel).Select(con => con.Whentoassess).FirstOrDefault();
                }
            }
            return View(getalldata);
        }
        [HttpPost]
        public async Task<IActionResult> Actionplan(Coaching coaching)
        {
            if (coaching.Coachings.Count > 0)
            {
                var coachlist = new List<Coaching>();
                foreach (var val in coaching.Coachings)
                {
                    var vals = _context.Coaching.AsNoTracking().Where(c => c.Taskcode == val.Taskcode && c.ID == val.ID).FirstOrDefault();
                    if (vals != null)
                    {
                        var getrangedate = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == vals.EmployeeId).FirstOrDefault();
                        if (getrangedate.Coachingstartdate <= val.Actionfrm && getrangedate.Coachingenddate >= val.Actiontoo)
                        {
                            coachlist.Add(new Coaching()
                            {
                                ID = vals.ID,
                                EmployeeId = vals.EmployeeId,
                                Employeename = vals.Employeename,
                                Coachid = vals.Coachid,
                                Coachname = vals.Coachname,
                                Traineeapproval = vals.Traineeapproval,
                                Coachapproval = vals.Coachapproval,
                                Dateapproved = vals.Dateapproved,
                                Status = vals.Status,
                                ATAREFCODE = vals.ATAREFCODE,
                                TSFNCODE = vals.TSFNCODE,
                                Coachingstart = vals.Coachingstart,
                                Coachingend = vals.Coachingend,
                                Plcode = vals.Plcode,
                                Pllevel = vals.Pllevel,
                                Plname = vals.Plname,
                                Actionpln = "Y",
                                Actionfrm = val.Actionfrm,
                                Actiontoo = val.Actiontoo,
                                Taskcode = vals.Taskcode,
                                Taskdesc = vals.Taskdesc,
                                ERDAT = vals.ERDAT,
                                ERNAM = vals.ERNAM,
                                AEDAT = DateTime.Now,
                                AENAM = User.Identity.Name,
                            });
                        }
                        else
                        {
                            return new JsonResult(new DatabaseOperationResponse
                            {
                                Status = OperationStatus.Exist,
                                Message = "Action Date Must Between Coaching Start and End Date!"
                            });
                        }
                    }
                }
                if (coachlist.Count > 0)
                {
                    var result = await _coaching.Updatelist(coachlist).ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        public IActionResult Reactionplan(string Employeeid, long? ID, string Plcode, string Pllevel)
        {
            if (Employeeid == null && ID == 0)
            {
                return NotFound();
            }
            var getdata = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == Employeeid && c.Plcode == Plcode && c.Pllevel == Pllevel && c.Actionpln == "Y").ToList();
            if (getdata.Count > 0)
            {
                getdata = getdata.OrderBy(c => c.Actionfrm).ThenBy(c => c.Actiontoo).ToList();
                var getdatas = getdata.GroupBy(c => new { c.Actionfrm, c.Actiontoo }).OrderBy(c => c.Key.Actionfrm).ToList();
                return View(getdata);
            }
            return View(getdata);
        }

        public async Task<IActionResult> Replan(string Employeeid, long? ID, string Plcode, string Pllevel, DateTime Actionfrm, DateTime Actiontoo)
        {
            if (Employeeid == null && ID == 0)
            {
                return NotFound();
            }
            var getdata = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == Employeeid && c.Plcode == Plcode && c.Pllevel == Pllevel && c.Actionpln == "Y" && c.Actionfrm == Actionfrm && c.Actiontoo == Actiontoo).ToList();
            if (getdata.Count > 0)
            {
                foreach (var datas in getdata)
                {
                    datas.Coachingstart = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == datas.EmployeeId && c.Plname == datas.Plcode && c.Pllevel == datas.Pllevel).Select(con => con.Coachingstartdate).FirstOrDefault();
                    datas.Coachingend = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == datas.EmployeeId && c.Plname == datas.Plcode && c.Pllevel == datas.Pllevel).Select(con => con.Coachingenddate).FirstOrDefault();
                    datas.Whenassess = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == datas.EmployeeId && c.Plname == datas.Plcode && c.Pllevel == datas.Pllevel).Select(con => con.Whentoassess).FirstOrDefault();
                }
            }
            return View(getdata);
        }

        [HttpPost]
        public async Task<IActionResult> Replan(Coaching coaching)
        {
            if (coaching.Coachings.Count > 0)
            {
                var coachlist = new List<Coaching>();
                foreach (var val in coaching.Coachings)
                {
                    var vals = _context.Coaching.AsNoTracking().Where(c => c.ID == val.ID).FirstOrDefault();
                    if (vals != null)
                    {
                        coachlist.Add(new Coaching()
                        {
                            ID = vals.ID,
                            EmployeeId = vals.EmployeeId,
                            Employeename = vals.Employeename,
                            Coachid = vals.Coachid,
                            Coachname = vals.Coachname,
                            Traineeapproval = vals.Traineeapproval,
                            Coachapproval = vals.Coachapproval,
                            Dateapproved = vals.Dateapproved,
                            Status = vals.Status,
                            ATAREFCODE = vals.ATAREFCODE,
                            TSFNCODE = vals.TSFNCODE,
                            Coachingstart = vals.Coachingstart,
                            Coachingend = vals.Coachingend,
                            Plcode = vals.Plcode,
                            Pllevel = vals.Pllevel,
                            Plname = vals.Plname,
                            Actionpln = "Y",
                            Actionfrm = val.Actionfrm,
                            Actiontoo = val.Actiontoo,
                            Taskcode = vals.Taskcode,
                            Taskdesc = vals.Taskdesc,
                            ERDAT = vals.ERDAT,
                            ERNAM = vals.ERNAM,
                            AEDAT = DateTime.Now,
                            AENAM = User.Identity.Name,
                        });
                    }
                }
                if (coachlist.Count > 0)
                {
                    var result = await _coaching.Updatelist(coachlist).ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        public IActionResult Actiongraph(string? Employeeid, string? Plcode, string? Plname)
        {
            var nottaken = 0;
            var taken = 0;

            if (Employeeid != null && Plcode != null && Plname != null)
            {
                var getall = _context.Coaching.Where(c => c.EmployeeId == Employeeid && c.Plcode == Plcode && c.Plname == Plname).ToList();
                var getall1 = getall.GroupBy(x => new { x.Actionpln }).Select(p => p.FirstOrDefault()).Select(p => new Coaching
                {
                    EmployeeId = p.EmployeeId,
                    Employeename = p.Employeename,
                    Plcode = p.Plcode,
                    Plname = p.Plname,
                    Pllevel = p.Pllevel,
                    Status = p.Status,
                    ATAREFCODE = p.ATAREFCODE,
                    TSFNCODE = p.TSFNCODE,
                    Actionfrm = p.Actionfrm,
                    Actionpln = p.Actionpln,
                    Actiontoo = p.Actiontoo,
                }).ToList();
                foreach (var val in getall)
                {
                  
                    if (val.Actionpln == "N")
                    {
                        nottaken = nottaken + 1;
                    }
                    else if (val.Actionpln == "Y")
                    {
                        taken = taken + 1;
                    }
                }
                List<Wholedata> valgrp = new List<Wholedata>()
                {
                    new Wholedata(){key="Action Plan not Taken",value=nottaken},
                    new Wholedata(){key="Action Plan Taken",value=taken},
                };

                Dashboard getdashboard = new Dashboard()
                {
                    Wholedatas = valgrp,
                };
                return View(getdashboard);
            }
            else
            {
                var getall = _context.Coaching.ToList();
                var getall1 = getall.GroupBy(x => new { x.Actionpln }).Select(p => p.FirstOrDefault()).Select(p => new Coaching
                {
                    EmployeeId = p.EmployeeId,
                    Employeename = p.Employeename,
                    Plcode = p.Plcode,
                    Plname = p.Plname,
                    Pllevel = p.Pllevel,
                    Status = p.Status,
                    Traineeapproval = p.Traineeapproval,
                    Coachapproval = p.Coachapproval,
                    ATAREFCODE = p.ATAREFCODE,
                    TSFNCODE = p.TSFNCODE,
                    Actionfrm = p.Actionfrm,
                    Actionpln = p.Actionpln,
                    Actiontoo = p.Actiontoo,
                }).ToList();
                foreach (var val in getall)
                {
                    if (val.Actionpln == "N")
                    {
                        nottaken = nottaken + 1;
                    }
                    else if (val.Actionpln == "Y")
                    {
                        taken = taken + 1;
                    }
                }
                List<Wholedata> valgrp = new List<Wholedata>()
                {
                    new Wholedata(){key="Action Plan Not Taken",value=nottaken},
                    new Wholedata(){key="Action Plan Taken",value=taken},
                };
                Dashboard getdashboard = new Dashboard()
                {
                    Wholedatas = valgrp,
                };
                return View(getdashboard);
            }
        }
        //public List<Wholedata> Completedtask()
        //{
        //    var nottaken = 0;
        //    var taken = 0;
        //    var getall = _context.Coaching.ToList();
        //    var getall1 = getall.GroupBy(x => new { x.Actionpln }).Select(p => p.FirstOrDefault()).Select(p => new Coaching
        //    {
        //        EmployeeId = p.EmployeeId,
        //        Employeename = p.Employeename,
        //        Plcode = p.Plcode,
        //        Plname = p.Plname,
        //        Pllevel = p.Pllevel,
        //        Status = p.Status,
        //        Traineeapproval = p.Traineeapproval,
        //        Coachapproval = p.Coachapproval,
        //        ATAREFCODE = p.ATAREFCODE,
        //        TSFNCODE = p.TSFNCODE,
        //        Actionfrm = p.Actionfrm,
        //        Actionpln = p.Actionpln,
        //        Actiontoo = p.Actiontoo,
        //    }).ToList();
        //    foreach (var val in getall)
        //    {
        //        if (val.Actionpln == "N")
        //        {
        //            nottaken = nottaken + 1;
        //        }
        //        else if (val.Actionpln == "Y")
        //        {
        //            taken = taken + 1;
        //        }
        //    }
        //    List<Wholedata> valgrp = new List<Wholedata>()
        //        {
        //            new Wholedata(){key="Action Plan Not Taken",value=nottaken},
        //            new Wholedata(){key="Action Plan Taken",value=taken},
        //        };

        //    return valgrp;
        //}
        //public List<Wholedata> Completedtasks(string Employeeid, string Plcode, string Plname)
        //{
        //    var nottaken = 0;
        //    var taken = 0;
        //    var getall = _context.Coaching.Where(c => c.EmployeeId == Employeeid && c.Plcode == Plcode && c.Plname == Plname).ToList();
        //    var getall1 = getall.GroupBy(x => new { x.Actionpln }).Select(p => p.FirstOrDefault()).Select(p => new Coaching
        //    {
        //        EmployeeId = p.EmployeeId,
        //        Employeename = p.Employeename,
        //        Plcode = p.Plcode,
        //        Plname = p.Plname,
        //        Pllevel = p.Pllevel,
        //        Status = p.Status,
        //        ATAREFCODE = p.ATAREFCODE,
        //        TSFNCODE = p.TSFNCODE,
        //        Actionfrm = p.Actionfrm,
        //        Actionpln = p.Actionpln,
        //        Actiontoo = p.Actiontoo,
        //    }).ToList();
        //    foreach (var val in getall)
        //    {
        //        if (val.Actionpln == "N")
        //        {
        //            nottaken = nottaken + 1;
        //        }
        //        else if (val.Actionpln == "Y")
        //        {
        //            taken = taken + 1;
        //        }
        //    }
        //    List<Wholedata> valgrp = new List<Wholedata>()
        //        {
        //            new Wholedata(){key="Action Plan not Taken",value=nottaken},
        //            new Wholedata(){key="Action Plan Taken",value=taken},
        //        };

        //    return valgrp;

        //}

    }
}
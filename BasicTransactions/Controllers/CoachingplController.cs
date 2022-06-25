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
    public class CoachingplController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Coaching _coaching;
        private readonly UserManager<ApplicationUser> _userManager;
        public CoachingplController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _coaching = new Coaching(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Coaching.Where(C => C.Actionpln == "Y" /*&& C.Actionfrm <= DateTime.Now && C.Actiontoo >= DateTime.Now*/).ToListAsync().ConfigureAwait(false));
        }
        //public async Task<IActionResult> Indexs()
        //{
        //    //if (TempData["SuccessMessage"] != null)
        //    //{
        //    //    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
        //    //}
        //    return View();
        //    //return View(await _context.Coaching.Where(C => C.Actionpln == "Y" && C.Actionfrm <= DateTime.Now && C.Actiontoo >= DateTime.Now).ToListAsync().ConfigureAwait(false));
        //}

        [HttpGet]
        public IActionResult Evaluate(string Employeeid, string sender, string Plcode, string Plname)
        {
            var getemp = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == Employeeid).FirstOrDefault();
            if (getemp != null)
            {
                ViewData["Coachid"] = _context.Coachingpairupsdtl.Where(c => c.EmployeeId == Employeeid && c.Position == getemp.Position && c.Plname == getemp.Plname).Select(L => new SelectListItem
                {
                    Text = L.Coachname,
                    Value = L.Coachid

                }).Distinct();

            }
            if (Employeeid == null)
            {
                return NotFound();
            }
            var getevaluation = _context.Coaching.Where(c => c.EmployeeId == Employeeid && c.Actionpln == "Y" /*&& c.Actionfrm <= DateTime.Now && c.Actiontoo >= DateTime.Now*/ && c.Plcode == Plcode && c.Plname == Plname).ToList();
            if (getevaluation.Count > 0)
            {
                getevaluation.Sort((s1, s2) =>
                {
                    int compare = s1.Taskcode.CompareTo(s2.Taskcode);
                    if (compare != 0) return compare;
                    return compare;
                });
                //check whether all coaching are signed or not
                foreach (var val in getevaluation)
                {
                    if (sender == "Coach")
                    {
                        val.sender = sender;
                    }
                    else if (sender == "Assessor")
                    {
                        val.sender = sender;
                    }
                }
            }
            else
            {
                getevaluation = _context.Coaching.Where(c => c.EmployeeId == Employeeid && c.Actionpln == "Y" && c.Plcode == Plcode && c.Plname == Plname).ToList();
                if (getevaluation.Count > 0)
                {
                    getevaluation.Sort((s1, s2) =>
                    {
                        int compare = s1.Taskcode.CompareTo(s2.Taskcode);
                        if (compare != 0) return compare;
                        return compare;
                    });
                    foreach (var val in getevaluation)
                    {
                        if (sender == "Coach")
                        {
                            val.sender = sender;
                        }
                        else if (sender == "Assessor")
                        {
                            val.sender = sender;
                        }
                    }
                }
            }
            return View(getevaluation);
        }
        public async Task<IActionResult> Takeaction(string Employeeid, string trainee, string coaches, DateTime dates, string Taskcode, string Atachapter, string Tsfncode, string Status)
        {
            var getlist = new List<Coaching>();
            if (Employeeid == null && trainee == null && coaches == null && dates == null && Taskcode == null)
            {

            }
            else
            {
                if (dates == null)
                {
                    return new JsonResult(new DatabaseOperationResponse
                    {
                        Status = OperationStatus.ERROR,
                        Message = "Approval Date is Mandatory!"
                    });
                }
                else
                {
                    getlist = _context.Coaching.AsNoTracking().Where(con => con.EmployeeId == Employeeid && con.Taskcode == Taskcode).ToList();
                    if (getlist.Count > 0)
                    {
                        if (trainee != null && coaches == null)//when Trainee check or take action
                        {
                            foreach (var val in getlist)
                            {
                                //check whether Trainee ID, User ID and Approval ID is the same or not!
                                if (Employeeid != User.Identity.Name || Employeeid != trainee || trainee != User.Identity.Name)
                                {
                                    var value = "Errorrrrrrrrrrrrrrr";
                                }

                                val.Traineeapproval = trainee;
                                val.Dateapproved = dates;
                                val.Status = "T";
                                val.TSFNCODE = Tsfncode;
                                val.ATAREFCODE = Atachapter;
                                val.AENAM = User.Identity.Name;
                                val.AEDAT = DateTime.Now;
                            }
                            var result = await _coaching.Updatelist(getlist).ConfigureAwait(false) as DatabaseOperationResponse;
                            return new JsonResult(result);
                        }
                        else //When Coach check or take action
                        {
                            if (Status == "A")
                            {
                                foreach (var val in getlist)
                                {
                                    if (coaches != null)
                                    {
                                        val.Coachid = coaches;
                                        val.Coachname = _context.Coachingpairupsdtl.AsNoTracking().Where(c => c.Coachid == coaches).Select(c => c.Coachname).FirstOrDefault();
                                    }
                                    val.Coachapproval = coaches;
                                    val.Dateapproved = dates;
                                    val.Status = "C";
                                    val.TSFNCODE = Tsfncode;
                                    val.ATAREFCODE = Atachapter;
                                    val.Taskcount = val.Taskcount;
                                    val.AENAM = User.Identity.Name;
                                    val.AEDAT = DateTime.Now;
                                }

                            }
                            else if (Status == "R")
                            {

                            }
                            var result = await _coaching.Updatelist(getlist).ConfigureAwait(false) as DatabaseOperationResponse;
                            return new JsonResult(result);
                        }

                    }
                }
            }
            return new JsonResult(null);
        }
        public IActionResult Graphs(string Employeeid)
        {
            Dashboard getdashboard = new Dashboard()
            {
                //Menuss = allMenuConstract,
                Wholedatas = getwhole(Employeeid),
                //Completedtask = getcomp(),
                //Notcompleted = getnotcmp(),
            };
            return View(getdashboard);
        }
        public IActionResult Allgraphs(string Employeeid, string Plcode, string Plname)
        {
            if (Employeeid != null && Plcode != null && Plname != null)
            {

                Dashboard getdashboard = new Dashboard()
                {
                    Wholedatas = Completedtasks(Employeeid, Plcode, Plname),
                };
                return View(getdashboard);
            }
            else
            {

                Dashboard getdashboard = new Dashboard()
                {
                    Wholedatas = Completedtask(),
                };
                return View(getdashboard);
            }
        }

        public List<Wholedata> getwhole(string Employeeid)
        {
            var comp = 0;
            var onpr = 0;
            var notst = 0;
            var getall = _context.Coaching.Where(c => c.EmployeeId == Employeeid).ToList();
            var getall1 = getall.GroupBy(c => c.Taskcode).ToList();
            foreach (var item in getall1)//_context.Coaching.Where(c => c.EmployeeId == Employeeid).ToList()
            {
                var Status = _context.Coaching.Where(c => c.Taskcode == item.Key && c.EmployeeId == Employeeid).Select(c => c.Status).FirstOrDefault();
                if (Status == "C")
                {
                    comp = comp + 1;
                }
                else if (Status == "T")
                {
                    onpr = onpr + 1;
                }
                else
                {
                    notst = notst + 1;
                }
            }
            List<Wholedata> valgrp = new List<Wholedata>()
                {
                    new Wholedata(){key="Completed",value=comp},
                    new Wholedata(){key="Ongoing",value=onpr},
                    new Wholedata(){key="Notdone",value=notst},
                };

            return valgrp;
        }
        public List<Wholedata> Completedtask()
        {
            var comp = 0;
            var onpr = 0;
            var notst = 0;
            var getall = _context.Coaching.Where(c => c.Actionpln == "Y").ToList();
            var getall1 = getall.GroupBy(x => new { x.Status}).Select(p => p.FirstOrDefault()).Select(p => new Coaching
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
                if (val.Status == "C")
                {
                    comp = comp + 1;
                }
                else if (val.Status == "T")
                {
                    onpr = onpr + 1;
                }
                else if (val.Status == "N")
                {
                    notst = notst + 1;
                }
            }
            List<Wholedata> valgrp = new List<Wholedata>()
                {
                    new Wholedata(){key="Completed",value=comp},
                    new Wholedata(){key="Ongoing",value=onpr},
                    new Wholedata(){key="Notdone",value=notst},
                };

            return valgrp;
        }
        public List<Wholedata> Completedtasks(string Employeeid, string Plcode, string Plname)
        {
            var comp = 0;
            var onpr = 0;
            var notst = 0;
            var getall = _context.Coaching.Where(c => c.Actionpln == "Y" && c.EmployeeId == Employeeid && c.Plcode == Plcode && c.Plname == Plname).ToList();
            var getall1 = getall.GroupBy(x => new { x.Status }).Select(p => p.FirstOrDefault()).Select(p => new Coaching
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
                if (val.Status == "C")
                {
                    comp = comp + 1;
                }
                else if (val.Status == "T")
                {
                    onpr = onpr + 1;
                }
                else if (val.Status == "N")
                {
                    notst = notst + 1;
                }
            }
            List<Wholedata> valgrp = new List<Wholedata>()
                {
                    new Wholedata(){key="Completed",value=comp},
                    new Wholedata(){key="Ongoing",value=onpr},
                    new Wholedata(){key="Notdone",value=notst},
                };

            return valgrp;

        }
        public IActionResult Takeactions(string Employeeid, string Taskcode, string Status, long? ID, string Taskdesc)
        {
            if (Employeeid == null || ID == null || Taskcode == null)
            {
                return NotFound();
            }
            var getemp = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == Employeeid).FirstOrDefault();
            if (getemp != null)
            {
                ViewData["Coachid"] = _context.Coachingpairupsdtl.Where(c => c.EmployeeId == Employeeid && c.Position == getemp.Position && c.Plname == getemp.Plname).Select(L => new SelectListItem
                {
                    Text = L.Coachname,
                    Value = L.Coachid

                }).Distinct();

            }
            var getdetails = _context.Coaching.AsNoTracking().Where(c => c.ID == ID).FirstOrDefault();
            if (getdetails != null)
            {
                getdetails.Taskname = _context.Taskcode.AsNoTracking().Where(c => c.Keyword == getdetails.Taskcode).Select(c => c.Descriptions).FirstOrDefault();
            }
            return View(getdetails);
        }
        [HttpPost]
        public async Task<IActionResult> Takeactions(Coaching coaching, string Status, string Reasons)
        {
            var getalldata = _context.Coaching.AsNoTracking().Where(c => c.ID == coaching.ID).FirstOrDefault();

            if (getalldata != null)
            {
                if (Status != null)
                {
                    if (Status == "N")//Take Action for Trainee
                    {
                        if (getalldata.EmployeeId == null)
                        {
                            return new JsonResult(new DatabaseOperationResponse
                            {
                                Status = OperationStatus.ERROR,
                                Message = "Trainee Approval ID is Mandatory"
                            });
                        }
                        else if (coaching.TSFNCODE == null)
                        {
                            return new JsonResult(new DatabaseOperationResponse
                            {
                                Status = OperationStatus.Exist,
                                Message = "Please write TSFN or any Appropriate Reasons! "
                            });
                        }
                        else if (coaching.ATAREFCODE == null)
                        {
                            return new JsonResult(new DatabaseOperationResponse
                            {
                                Status = OperationStatus.Exist,
                                Message = "Please Write ATA Chapter or any Appropriate Reasons!"
                            });
                        }
                        else
                        {
                            getalldata.Traineeapproval = getalldata.EmployeeId;
                            getalldata.ATAREFCODE = coaching.ATAREFCODE;
                            getalldata.TSFNCODE = coaching.TSFNCODE;
                            getalldata.Dateapproved = coaching.Dateapproved;
                            getalldata.AEDAT = DateTime.Now;
                            getalldata.AENAM = User.Identity.Name;
                            getalldata.Status = "T";
                        }
                    }
                    else if (Status == "A")//Take Action for Coaches
                    {
                        if (coaching.Coachapproval == null)
                        {
                            return new JsonResult(new DatabaseOperationResponse
                            {
                                Status = OperationStatus.Exist,
                                Message = "Coach Approval ID is Mandatory"
                            });
                        }
                        else
                        {
                            getalldata.Coachapproval = coaching.Coachapproval;
                            getalldata.Coachid = coaching.Coachapproval;
                            getalldata.Coachname = _context.Employees.AsNoTracking().Where(c => c.EmployeeId == getalldata.Coachid).Select(c => c.Employeename).FirstOrDefault();
                            getalldata.AEDAT = DateTime.Now;
                            getalldata.Dateapproved = coaching.Dateapproved;
                            getalldata.AENAM = User.Identity.Name;
                            getalldata.Status = "C";
                        }
                    }
                    else if (Status == "W")//Take Action for Coaches with Remark
                    {
                        if (coaching.Coachapproval == null)
                        {
                            return new JsonResult(new DatabaseOperationResponse
                            {
                                Status = OperationStatus.Exist,
                                Message = "Coach Approval ID is Mandatory"
                            });
                        }
                        else
                        {
                            getalldata.Coachapproval = coaching.Coachapproval;
                            getalldata.Coachid = coaching.Coachapproval;
                            getalldata.Coachname = _context.Employees.AsNoTracking().Where(c => c.EmployeeId == getalldata.Coachid).Select(c => c.Employeename).FirstOrDefault();
                            getalldata.AEDAT = DateTime.Now;
                            getalldata.Dateapproved = coaching.Dateapproved;
                            getalldata.AENAM = User.Identity.Name;
                            getalldata.Status = "C";
                            getalldata.Reasons = Reasons;
                        }
                    }
                    else if (Status == "R")//Take Action for Coaches Rejection
                    {

                    }
                }
                var result = await getalldata.Update1().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        public IActionResult Getdetails(string Employeeid, string Status, long? ID)
        {
            if (Employeeid == null)
            {
                return NotFound();
            }
            var getdtlval = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == Employeeid && c.ID == ID).ToList();

            return View(getdtlval);
        }
        public IActionResult Reasonsfor(string Employeeid, long? ID, string Coaches)
        {
            if (Employeeid == null || ID == 0)
            {
                return NotFound();
            }
            var getempdata = _context.Coaching.Where(c => c.ID == ID && c.EmployeeId == Employeeid).FirstOrDefault();
            if (getempdata != null)
            {
                getempdata.Coachapproval = Coaches;
            }
            return View(getempdata);
        }
    }
}

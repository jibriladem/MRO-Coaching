using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.BasicTransactions;
using MROCoatching.DataObjects.Models.CoachingTable;
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
    public class CoachannualplansController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.BasicTransactions.Coachannualplans _annualnplans;
        private readonly DataObjects.Models.BasicTransactions.Coachingpairupshdr _coachinghdr;
        private readonly DataObjects.Models.BasicTransactions.Coachingpairupsdtl _coachingdtl;
        private readonly UserManager<ApplicationUser> _userManager;
        public CoachannualplansController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _annualnplans = new DataObjects.Models.BasicTransactions.Coachannualplans(context);
            _coachinghdr = new DataObjects.Models.BasicTransactions.Coachingpairupshdr(context);
            _coachingdtl = new DataObjects.Models.BasicTransactions.Coachingpairupsdtl(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var getuser = User.Identity.Name;
            if (getuser == "admin@MRO.com")
            {
                if (TempData["SuccessMessage"] != null)
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                }
                //return View();
                var getall = _context.Coachingpairupshdr.Where(c => c.Approve1 == "R" || c.Approve1 == "Y").ToList();
                if (getall.Count > 0)
                {
                    foreach (var dat in getall)
                    {
                        var coachnbr = _context.Coachingpairupsdtl.Where(c => c.EmployeeId == dat.EmployeeId && c.Position == dat.Position && c.Plname == dat.Plname).ToList();
                        //var asschnbr = _context.Coachingpairupsdtl.Where(c => c.EmployeeId == dat.EmployeeId && c.Position == dat.Position).Select(x => x.Assessorid).ToList();// && c.Position == dat.Position && c.Pllevel == dat.Pllevel
                        dat.nbrofcoach = coachnbr.GroupBy(c => c.Coachid).Count();// _context.Coachingpairupsdtl.Where(c => c.EmployeeId == dat.EmployeeId && c.Position == dat.Position && c.Pllevel == dat.Pllevel).Count();
                                                                                  //dat.nbrofass = asschnbr.Count();// _context.Coachingpairupsdtl.Where(c => c.EmployeeId == dat.EmployeeId && c.Position == dat.Position && c.Pllevel == dat.Pllevel).Select(x => x.Assessorid).Count();
                                                                                  //coachnbr.Sort((s1, s2) =>
                                                                                  //{
                                                                                  //    int compare = s1.Coachid.CompareTo(s2.Coachid);
                                                                                  //    //if (compare != 0) return compare;
                                                                                  //    //compare = s1.ACCCOD.CompareTo(s2.ACCCOD);
                                                                                  //    return compare;
                                                                                  //});
                        dat.nbrofass = coachnbr.GroupBy(c => c.Assessorid).Count();
                    }
                }
                return View(getall);
                //return View(await _context.Coachingpairupshdr.Where(con => con.Approve1 == "R" || con.Approve1 == "Y").ToListAsync().ConfigureAwait(false));
            }
            else
            {
                var getcurrentuser = _context.Employees.Where(c => c.EmployeeId == getuser).FirstOrDefault();
                if (getcurrentuser != null)
                {
                    if (TempData["SuccessMessage"] != null)
                    {
                        ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                    }
                    return View(await _context.Coachingpairupshdr.Where(con => con.Approve1 == "R" || con.Approve1 == "Y" && con.Costcentercode == getcurrentuser.Costcentercode).ToListAsync().ConfigureAwait(false));
                }
                return View();
            }
        }
        public async Task<IActionResult> Prepare()
        {
            return View(await _context.Coachedon.ToListAsync().ConfigureAwait(false));

        }
        // GET: BasicTransactions/Coachannualplans/Details/5
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
        // GET: BasicTransactions/Coachannualplans/Create
        public IActionResult Create()
        {
            var getuser = User.Identity.Name;
            ViewBag.Roles = new SelectList(_context.Roles.Select(x => x.Name).Distinct());
            var getcurrentuser = _context.Employees.Where(c => c.EmployeeId == getuser).FirstOrDefault();
            if (getcurrentuser != null)
            {
                if (getcurrentuser.Status1 == "Y")
                {
                    //Populate All data
                    ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y" && con.Status1 == "N" && con.Costcentercode == getcurrentuser.Costcentercode).Select(L => new SelectListItem//Employee ID
                    {
                        Text = L.EmployeeId,
                        Value = L.EmployeeId,
                    }).Distinct();
                }
                else
                {
                    //See only self data
                    ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y" && con.Status1 == "N").Select(L => new SelectListItem//Employee ID
                    {
                        Text = L.EmployeeId,
                        Value = L.EmployeeId,
                    }).Distinct();
                }
            }
            else
            {
                if (getuser == "admin@MRO.com")
                {
                    ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y" && con.Status1 == "N").Select(L => new SelectListItem//Employee ID
                    {
                        Text = L.EmployeeId,
                        Value = L.EmployeeId,
                    }).Distinct();
                }
            }
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["Coachid"] = _context.Coaches.Select(L => new SelectListItem//Caoch ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Assessorid"] = _context.Assessors.Select(L => new SelectListItem//Assessor ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Plname"] = _context.Types.Select(L => new SelectListItem
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();

            return PartialView();
        }
        public IActionResult Pcreate()
        {
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y" && con.Status1 == "N" && (!string.IsNullOrEmpty(con.Nextpl) || con.Nextpl != null)).Select(L => new SelectListItem//Employee ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Coachid"] = _context.Coaches.Select(L => new SelectListItem//Caoch ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Assessorid"] = _context.Assessors.Select(L => new SelectListItem//Assessor ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Plname"] = _context.Positions.Select(L => new SelectListItem
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        // [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Coachannualplans coachannualplans)
        public async Task<IActionResult> Create(Coachingpairupshdr coachingpairupshdr)
        {
            //var role = new SelectList(_context.Roles.Select(x => x.Name).Distinct());
            var Coaching = new List<Coachingpairupsdtl>();

            var Coaches = new List<Coachingpairupsdtl>();
            var Assessors = new List<Coachingpairupsdtl>();
            if (coachingpairupshdr.Coachingpairupsdtl.Count > 0 && coachingpairupshdr.Coachingpairupsdtl2.Count > 0)
            {
                Coaches = coachingpairupshdr.Coachingpairupsdtl;
                Assessors = coachingpairupshdr.Coachingpairupsdtl2;
                _coachinghdr.EmployeeId = coachingpairupshdr.EmployeeId;
                _coachinghdr.Employeename = coachingpairupshdr.Employeename;
                _coachinghdr.Position = coachingpairupshdr.Position;
                _coachinghdr.Costcentercode = coachingpairupshdr.Costcentercode;
                _coachinghdr.Costcenterdesc = coachingpairupshdr.Costcenterdesc;
                //_coachinghdr.Lastpromotiondate = coachingpairupshdr.Lastpromotiondate;
                //_coachinghdr.Nextpromotiondate = coachingpairupshdr.Nextpromotiondate;
                _coachinghdr.Coachingstartdate = coachingpairupshdr.Coachingstartdate;
                _coachinghdr.Coachingenddate = coachingpairupshdr.Coachingenddate;
                _coachinghdr.Whentoassess = coachingpairupshdr.Whentoassess;
                _coachinghdr.Plname = coachingpairupshdr.Plname;
                _coachinghdr.Pllevel = coachingpairupshdr.Pllevel;
                _coachinghdr.Status = "Y";
                _coachinghdr.Approve1 = "R";
                _coachinghdr.Approve2 = "N";
                _coachinghdr.Approve3 = "N";
                _coachinghdr.ERDAT = DateTime.Now;
                _coachinghdr.ERNAM = User.Identity.Name;
                _coachinghdr.AEDAT = DateTime.Now;
                _coachinghdr.AENAM = User.Identity.Name;

                var exists1 = await _coachinghdr.Exist().ConfigureAwait(false);
                if (!exists1)
                {
                    //Looping for Assessors
                    foreach (var assessor in Assessors)
                    {
                        var ass = _context.Assessors.Where(con => con.EmployeeId == assessor.Assessorid).FirstOrDefault();
                        if (ass == null)
                        {
                            return new JsonResult(new DatabaseOperationResponse
                            {
                                Status = OperationStatus.Exist,
                                Message = "There is no Assessor with such ID in Master Table!"
                            });
                        }
                        else
                        {
                            //Looping for Coaches
                            foreach (var coach in Coaches)
                            {
                                if (coachingpairupshdr.EmployeeId == coach.Coachid)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.ERROR,
                                        Message = "Individual Employee can't Coach him/her self!"
                                    });
                                }
                                else if (assessor.Assessorid == coach.Coachid)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.ERROR,
                                        Message = "Individual Employee can't be both Assessor and Coach!"
                                    });
                                }
                                else if (assessor.Assessorid == coachingpairupshdr.EmployeeId)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.ERROR,
                                        Message = "Individual Employee can't Assess him/her Self!"
                                    });
                                }
                                else
                                {
                                    if (coachingpairupshdr.Coachingstartdate >= coachingpairupshdr.Coachingenddate)
                                    {
                                        return new JsonResult(new DatabaseOperationResponse
                                        {
                                            Status = OperationStatus.ERROR,
                                            Message = "Coaching Start Date Must Less than Coaching End Date!"
                                        });
                                    }
                                    else if (coachingpairupshdr.Coachingenddate >= coachingpairupshdr.Whentoassess)
                                    {
                                        return new JsonResult(new DatabaseOperationResponse
                                        {
                                            Status = OperationStatus.ERROR,
                                            Message = "Coaching End Date Must Less than Assessment Date!"
                                        });
                                    }
                                    else if (coachingpairupshdr.Coachingstartdate >= coachingpairupshdr.Whentoassess)
                                    {
                                        return new JsonResult(new DatabaseOperationResponse
                                        {
                                            Status = OperationStatus.ERROR,
                                            Message = "Coaching Start Date Must Less than Assessment Date!"
                                        });
                                    }
                                    else if (coachingpairupshdr.Coachingenddate >= coachingpairupshdr.Whentoassess)
                                    {
                                        return new JsonResult(new DatabaseOperationResponse
                                        {
                                            Status = OperationStatus.ERROR,
                                            Message = "Coaching End Date Must Less than Assessment Date!"
                                        });
                                    }
                                    else if (coachingpairupshdr.Coachingstartdate >= coachingpairupshdr.Whentoassess)
                                    {
                                        return new JsonResult(new DatabaseOperationResponse
                                        {
                                            Status = OperationStatus.ERROR,
                                            Message = "Coaching Start Date Must Less than Assessment Date!"
                                        });
                                    }
                                    else
                                    {
                                        Coaching.Add(new Coachingpairupsdtl()
                                        {
                                            EmployeeId = coach.EmployeeId,
                                            Coachid = coach.Coachid,
                                            Coachname = coach.Coachname,
                                            Costcenter1 = coach.Costcenter1,
                                            Assessorid = assessor.Assessorid,
                                            Assessorname = assessor.Assessorname,
                                            Costcenter2 = assessor.Costcenter2,
                                            Pllevel = coach.Pllevel,
                                            Position = coach.Position,
                                            Plname = coach.Plname,
                                            Status = "Y",
                                            ERDAT = DateTime.Now,
                                            ERNAM = User.Identity.Name,
                                            AEDAT = DateTime.Now,
                                            AENAM = User.Identity.Name,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    foreach (var dtl in coachingpairupshdr.Coachingpairupsdtl)
                    {
                        //if (dtl.Assessorid != null)
                        //{
                        //    var ass = _context.Assessors.Where(con => con.EmployeeId == dtl.Assessorid).FirstOrDefault();
                        //    if (ass == null)
                        //    {
                        //        return new JsonResult(new DatabaseOperationResponse
                        //        {
                        //            Status = OperationStatus.Exist,
                        //            Message = "There is no Assessor with such ID in Master Table!"
                        //        });
                        //    }
                        //    else
                        //    {
                        //        if (dtl.Coachid == dtl.Assessorid)
                        //        {
                        //            return new JsonResult(new DatabaseOperationResponse
                        //            {
                        //                Status = OperationStatus.ERROR,
                        //                Message = "Individual Employee can't be both Assessor and Coach!"
                        //            });
                        //        }
                        //        else if (dtl.Coachid == dtl.EmployeeId)
                        //        {
                        //            return new JsonResult(new DatabaseOperationResponse
                        //            {
                        //                Status = OperationStatus.ERROR,
                        //                Message = "Individual Employee can't Coach him/her self!"
                        //            });
                        //        }
                        //        else if (coachingpairupshdr.EmployeeId == dtl.Assessorid)
                        //        {
                        //            return new JsonResult(new DatabaseOperationResponse
                        //            {
                        //                Status = OperationStatus.ERROR,
                        //                Message = "Individual Employee can't Assess him/her Self!"
                        //            });
                        //        }
                        //        else
                        //        {
                        //            if (coachingpairupshdr.Coachingstartdate >= coachingpairupshdr.Coachingenddate)
                        //            {
                        //                return new JsonResult(new DatabaseOperationResponse
                        //                {
                        //                    Status = OperationStatus.ERROR,
                        //                    Message = "Coaching Start Date Must Less than Coaching End Date!"
                        //                });
                        //            }
                        //            else if (coachingpairupshdr.Coachingenddate >= coachingpairupshdr.Whentoassess)
                        //            {
                        //                return new JsonResult(new DatabaseOperationResponse
                        //                {
                        //                    Status = OperationStatus.ERROR,
                        //                    Message = "Coaching End Date Must Less than Assessment Date!"
                        //                });
                        //            }
                        //            else if (coachingpairupshdr.Coachingstartdate >= coachingpairupshdr.Whentoassess)
                        //            {
                        //                return new JsonResult(new DatabaseOperationResponse
                        //                {
                        //                    Status = OperationStatus.ERROR,
                        //                    Message = "Coaching Start Date Must Less than Assessment Date!"
                        //                });
                        //            }
                        //            else
                        //            {
                        //                var exist = _context.Coachingpairupshdr.Where(c => c.EmployeeId == coachingpairupshdr.EmployeeId &&
                        //                                                                   c.Plname == coachingpairupshdr.Plname &&
                        //                                                                   c.Position == coachingpairupshdr.Position).FirstOrDefault();
                        //                var exist1 = await _annualnplans.Exist().ConfigureAwait(false);
                        //                if (!exist1)
                        //                {
                        //                    if (coachingpairupshdr.Coachingstartdate != null && coachingpairupshdr.Coachingenddate != null)
                        //                    {
                        //                        //Get count of date
                        //                        var getdate = coachingpairupshdr.Coachingenddate - coachingpairupshdr.Coachingstartdate;
                        //                        var getcurrdays = getdate.Days;
                        //                    }
                        //                    Coaching.Add(new Coachingpairupsdtl()
                        //                    {
                        //                        EmployeeId = dtl.EmployeeId,
                        //                        Coachid = dtl.Coachid,
                        //                        Coachname = dtl.Coachname,
                        //                        Costcenter1 = dtl.Costcenter1,
                        //                        Assessorid = dtl.Assessorid,
                        //                        Assessorname = dtl.Assessorname,
                        //                        Costcenter2 = dtl.Costcenter2,
                        //                        Pllevel = dtl.Pllevel,
                        //                        Position = dtl.Position,
                        //                        Plname = dtl.Plname,
                        //                        Status = "Y",
                        //                        ERDAT = DateTime.Now,
                        //                        ERNAM = User.Identity.Name,
                        //                        AEDAT = DateTime.Now,
                        //                        AENAM = User.Identity.Name,
                        //                    });

                        //                }
                        //                else
                        //                {
                        //                    var msg = coachingpairupshdr.Employeename + " with (" + coachingpairupshdr.Position + ") Position " + "has recorder before on (" + coachingpairupshdr.Plname + ") PL";
                        //                    return new JsonResult(new DatabaseOperationResponse
                        //                    {
                        //                        Status = OperationStatus.Exist,
                        //                        Message = msg
                        //                    });
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }

                }
                else
                {
                    return new JsonResult(new DatabaseOperationResponse
                    {
                        Status = OperationStatus.Exist,
                        Message = "Record is Exist!"
                    });
                }


                if (Coaching.Count > 0)
                {
                    _coachinghdr.Coachingpairupsdtl = Coaching;
                    var result = await _coachinghdr.Save().ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: BasicTransactions/Coachannualplans/Edit/5
        public async Task<IActionResult> Edit(string Employeeid, string position, string plname)
        {

            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y" && con.Status1 == "N").Select(L => new SelectListItem//Employee ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Coachid"] = _context.Coaches.Select(L => new SelectListItem//Coach ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Assessorid"] = _context.Assessors.Select(L => new SelectListItem//Assessor ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Plname"] = _context.Types.Select(L => new SelectListItem
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            if (Employeeid == null)
            {
                return NotFound();
            }

            var getvalue = _context.Coachingpairupshdr.Where(con => con.EmployeeId == Employeeid && con.Position == position && con.Plname == plname).FirstOrDefault();
            if (getvalue == null)
            {
                return NotFound();
            }
            else
            {
                //getvalue.Coaching = _context.Coaching.Where(c => c.EmployeeId == Employeeid).ToList();
                //getvalue.Coaches = _context.Coachannualplans.Where(c => c.EmployeeId == Employeeid).ToList();
                //getvalue.Assessors = _context.Coachannualplans.Where(c => c.EmployeeId == Employeeid).ToList();
                getvalue.Coachingpairupsdtl = _context.Coachingpairupsdtl.Where(con => con.EmployeeId == getvalue.EmployeeId && con.Position == getvalue.Position && con.Plname == getvalue.Plname).ToList();
                if (getvalue.Position != null)
                {
                    getvalue.Postname = _context.Positions.Where(c => c.Postid == getvalue.Position).Select(c => c.Postname).FirstOrDefault();
                }
                if (getvalue.Coachingpairupsdtl.Count > 0)
                {
                    foreach (var val in getvalue.Coachingpairupsdtl)
                    {
                        val.Coscentername1 = _context.Costcenters.Where(c => c.Costcentercode == val.Costcenter1).Select(c => c.Costcentername).FirstOrDefault();
                        //val.Coscentername2 = _context.Costcenters.Where(c => c.Costcentercode == val.Costcenter2).Select(c => c.Costcentername).FirstOrDefault();
                    }
                }
                getvalue.Coachingpairupsdtl2 = _context.Coachingpairupsdtl.Where(con => con.EmployeeId == getvalue.EmployeeId && con.Position == getvalue.Position && con.Plname == getvalue.Plname).ToList();
                if (getvalue.Coachingpairupsdtl2.Count > 0)
                {
                    foreach (var val in getvalue.Coachingpairupsdtl2)
                    {
                        val.Coscentername2 = _context.Costcenters.Where(c => c.Costcentercode == val.Costcenter2).Select(c => c.Costcentername).FirstOrDefault();
                    }
                    getvalue.Coachingpairupsdtl2 = getvalue.Coachingpairupsdtl2.GroupBy(x => new { x.Assessorid, x.Costcenter2 }).Select(x => x.FirstOrDefault()).Select(c => new Coachingpairupsdtl
                    {
                        Assessorid = c.Assessorid,
                        Assessorname = c.Assessorname,
                        Costcenter2 = c.Costcenter2,
                        Coscentername2 = c.Coscentername2,
                        Ids = c.Ids,
                    }).ToList();
                }

            }
            return View(getvalue);
        }
        // POST: BasicTransactions/Coachannualplans/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Coachingpairupshdr coachingpairupshdr)
        {
            var Coaching = new List<Coachingpairupsdtl>();
            var Coaches = new List<Coachingpairupsdtl>();
            var Assessors = new List<Coachingpairupsdtl>();
            if (coachingpairupshdr.Coachingpairupsdtl.Count > 0)
            {
                Coaches = coachingpairupshdr.Coachingpairupsdtl;
                Assessors = coachingpairupshdr.Coachingpairupsdtl2;
                _coachinghdr.Id = coachingpairupshdr.Id;
                _coachinghdr.EmployeeId = coachingpairupshdr.EmployeeId;
                _coachinghdr.Employeename = coachingpairupshdr.Employeename;
                _coachinghdr.Position = coachingpairupshdr.Position;
                _coachinghdr.Costcentercode = coachingpairupshdr.Costcentercode;
                _coachinghdr.Costcenterdesc = coachingpairupshdr.Costcenterdesc;
                //_coachinghdr.Lastpromotiondate = coachingpairupshdr.Lastpromotiondate;
                //_coachinghdr.Nextpromotiondate = coachingpairupshdr.Nextpromotiondate;
                _coachinghdr.Coachingstartdate = coachingpairupshdr.Coachingstartdate;
                _coachinghdr.Coachingenddate = coachingpairupshdr.Coachingenddate;
                _coachinghdr.Whentoassess = coachingpairupshdr.Whentoassess;
                _coachinghdr.Plname = coachingpairupshdr.Plname;
                _coachinghdr.Pllevel = coachingpairupshdr.Pllevel;
                _coachinghdr.Status = coachingpairupshdr.Status;
                _coachinghdr.Approve1 = coachingpairupshdr.Approve1;
                _coachinghdr.Approve2 = coachingpairupshdr.Approve2;
                _coachinghdr.Approve3 = coachingpairupshdr.Approve3;
                _coachinghdr.ERDAT = coachingpairupshdr.ERDAT;
                _coachinghdr.ERNAM = coachingpairupshdr.ERNAM;
                _coachinghdr.AEDAT = DateTime.Now;
                _coachinghdr.AENAM = User.Identity.Name;
                //Looping for Assessors
                foreach (var assessor in Assessors)
                {
                    var ass = _context.Assessors.Where(con => con.EmployeeId == assessor.Assessorid).FirstOrDefault();
                    if (ass == null)
                    {
                        return new JsonResult(new DatabaseOperationResponse
                        {
                            Status = OperationStatus.Exist,
                            Message = "There is no Assessor with such ID in Master Table!"
                        });
                    }
                    else
                    {
                        //Looping for Coaches
                        foreach (var coach in Coaches)
                        {
                            if (coachingpairupshdr.EmployeeId == coach.Coachid)
                            {
                                return new JsonResult(new DatabaseOperationResponse
                                {
                                    Status = OperationStatus.ERROR,
                                    Message = "Individual Employee can't Coach him/her self!"
                                });
                            }
                            else if (assessor.Assessorid == coach.Coachid)
                            {
                                return new JsonResult(new DatabaseOperationResponse
                                {
                                    Status = OperationStatus.ERROR,
                                    Message = "Individual Employee can't be both Assessor and Coach!"
                                });
                            }
                            else if (assessor.Assessorid == coachingpairupshdr.EmployeeId)
                            {
                                return new JsonResult(new DatabaseOperationResponse
                                {
                                    Status = OperationStatus.ERROR,
                                    Message = "Individual Employee can't Assess him/her Self!"
                                });
                            }
                            else
                            {
                                if (coachingpairupshdr.Coachingstartdate >= coachingpairupshdr.Coachingenddate)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.ERROR,
                                        Message = "Coaching Start Date Must Less than Coaching End Date!"
                                    });
                                }
                                else if (coachingpairupshdr.Coachingenddate >= coachingpairupshdr.Whentoassess)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.ERROR,
                                        Message = "Coaching End Date Must Less than Assessment Date!"
                                    });
                                }
                                else if (coachingpairupshdr.Coachingstartdate >= coachingpairupshdr.Whentoassess)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.ERROR,
                                        Message = "Coaching Start Date Must Less than Assessment Date!"
                                    });
                                }
                                else if (coachingpairupshdr.Coachingenddate >= coachingpairupshdr.Whentoassess)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.ERROR,
                                        Message = "Coaching End Date Must Less than Assessment Date!"
                                    });
                                }
                                else if (coachingpairupshdr.Coachingstartdate >= coachingpairupshdr.Whentoassess)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.ERROR,
                                        Message = "Coaching Start Date Must Less than Assessment Date!"
                                    });
                                }
                                else
                                {
                                    Coaching.Add(new Coachingpairupsdtl()
                                    {
                                        Ids = coachingpairupshdr.Id,
                                        EmployeeId = coach.EmployeeId,
                                        Coachid = coach.Coachid,
                                        Coachname = coach.Coachname,
                                        Costcenter1 = coach.Costcenter1,
                                        Assessorid = assessor.Assessorid,
                                        Assessorname = assessor.Assessorname,
                                        Costcenter2 = assessor.Costcenter2,
                                        Pllevel = coach.Pllevel,
                                        Position = coach.Position,
                                        Plname = coach.Plname,
                                        Status = "Y",
                                        ERDAT = DateTime.Now,
                                        ERNAM = User.Identity.Name,
                                        AEDAT = DateTime.Now,
                                        AENAM = User.Identity.Name,
                                    });
                                }
                            }
                        }
                    }
                }
                if (Coaching.Count > 0)
                {
                    _coachinghdr.Coachingpairupsdtl = Coaching;
                    var result = await _coachinghdr.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }
                //foreach (var dtl in coachingpairupshdr.Coachingpairupsdtl)
                //{
                //    if (dtl.Assessorid != null)
                //    {
                //        var ass = _context.Assessors.Where(con => con.EmployeeId == dtl.Assessorid).FirstOrDefault();
                //        if (ass == null)
                //        {
                //            return new JsonResult(new DatabaseOperationResponse
                //            {
                //                Status = OperationStatus.Exist,
                //                Message = "There is no Assessor with such ID in Master Table!"
                //            });
                //        }
                //        else
                //        {
                //            if (dtl.Coachid == dtl.Assessorid)
                //            {
                //                return new JsonResult(new DatabaseOperationResponse
                //                {
                //                    Status = OperationStatus.ERROR,
                //                    Message = "Individual Employee can't be both Assessor and Coach!"
                //                });
                //            }
                //            else if (dtl.Coachid == dtl.EmployeeId)
                //            {
                //                return new JsonResult(new DatabaseOperationResponse
                //                {
                //                    Status = OperationStatus.ERROR,
                //                    Message = "Individual Employee can't Coached by self!"
                //                });
                //            }
                //            else if (coachingpairupshdr.EmployeeId == dtl.Assessorid)
                //            {
                //                return new JsonResult(new DatabaseOperationResponse
                //                {
                //                    Status = OperationStatus.ERROR,
                //                    Message = "Individual Employee can't Assessed by Self!"
                //                });
                //            }
                //            else
                //            {
                //                if (coachingpairupshdr.Position == coachingpairupshdr.Plname)
                //                {
                //                    return new JsonResult(new DatabaseOperationResponse
                //                    {
                //                        Status = OperationStatus.ERROR,
                //                        Message = "You are Trying to Assign Same Position with before one!"
                //                    });
                //                }
                //                //if (coachingpairupshdr.Lastpromotiondate == coachingpairupshdr.Nextpromotiondate)
                //                //{
                //                //    return new JsonResult(new DatabaseOperationResponse
                //                //    {
                //                //        Status = OperationStatus.ERROR,
                //                //        Message = "Last Promotion Date Must Different From Next Promotion!"
                //                //    });
                //                //}
                //                //else if (coachingpairupshdr.Lastpromotiondate >= coachingpairupshdr.Nextpromotiondate)
                //                //{
                //                //    return new JsonResult(new DatabaseOperationResponse
                //                //    {
                //                //        Status = OperationStatus.ERROR,
                //                //        Message = "Next Promotion Date must Greater than! Last Promotion Date"
                //                //    });
                //                //}
                //                else
                //                {
                //                    //var exist = _context.Coachingpairupshdr.Where(c => c.EmployeeId == coachingpairupshdr.EmployeeId &&
                //                    //                                                   c.Plname == coachingpairupshdr.Plname &&
                //                    //                                                   c.Position == coachingpairupshdr.Position).FirstOrDefault();
                //                    var exist1 = await _annualnplans.Exist().ConfigureAwait(false);
                //                    if (!exist1)
                //                    {
                //                        if (coachingpairupshdr.Coachingstartdate != null && coachingpairupshdr.Coachingenddate != null)
                //                        {
                //                            //Get count of date
                //                            var getdate = coachingpairupshdr.Coachingenddate - coachingpairupshdr.Coachingstartdate;
                //                            var getcurrdays = getdate.Days;
                //                        }
                //                        Coaching.Add(new Coachingpairupsdtl()
                //                        {
                //                            Ids = dtl.Ids,
                //                            EmployeeId = dtl.EmployeeId,
                //                            Coachid = dtl.Coachid,
                //                            Coachname = dtl.Coachname,
                //                            Costcenter1 = dtl.Costcenter1,
                //                            Assessorid = dtl.Assessorid,
                //                            Assessorname = dtl.Assessorname,
                //                            Costcenter2 = dtl.Costcenter2,
                //                            Pllevel = dtl.Pllevel,
                //                            Position = dtl.Position,
                //                            Plname = dtl.Plname,
                //                            Status = "Y",
                //                            ERDAT = DateTime.Now,
                //                            ERNAM = User.Identity.Name,
                //                            AEDAT = DateTime.Now,
                //                            AENAM = User.Identity.Name,
                //                        });

                //                    }
                //                    else
                //                    {
                //                        var msg = coachingpairupshdr.Employeename + " with (" + coachingpairupshdr.Position + ") Position " + "has recorder before on (" + coachingpairupshdr.Plname + ") PL";
                //                        return new JsonResult(new DatabaseOperationResponse
                //                        {
                //                            Status = OperationStatus.Exist,
                //                            Message = msg
                //                        });
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: BasicTransactions/Coachannualplans/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var pairups = await _context.Coachingpairupshdr.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                if (pairups != null)
                {
                    var getdetails = _context.Coachingpairupsdtl.AsNoTracking().Where(c => c.EmployeeId == pairups.EmployeeId && c.Position == pairups.Position && c.Plname == pairups.Plname).ToList();
                    if (getdetails.Count > 0)
                    {
                        foreach (var value in getdetails)
                        {
                            _coachingdtl.Ids = value.Ids;
                        }
                        var result1 = await _coachinghdr.Deletelist(getdetails) as DatabaseOperationResponse;
                        if (result1.Status == OperationStatus.SUCCESS)
                        {
                            _coachinghdr.Id = pairups.Id;

                            var result2 = await _coachinghdr.Delete() as DatabaseOperationResponse;
                            return new JsonResult(result2);
                        }
                    }
                }
                //var annualpl = await _context.Coachannualplans.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                //_annualnplans.Id = annualpl.Id;
                //var result = await _annualnplans.Delete() as DatabaseOperationResponse;
                //return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        public IActionResult Getcoaches(string EmployeeId, string Plname, string Position)
        {
            var getcoach = new List<Coachingpairupsdtl>();
            if (string.IsNullOrEmpty(EmployeeId))
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(EmployeeId) && !string.IsNullOrEmpty(Position) && !string.IsNullOrEmpty(Plname))
            {
                getcoach = _context.Coachingpairupsdtl.Where(con => con.EmployeeId == EmployeeId && con.Position == Position && con.Plname == Plname).ToList();
            }
            //ViewBag.getcoach = getcoach;
            return View(getcoach);
        }
        public IActionResult Getassessors(string EmployeeId, string Plname, string Position)
        {
            var getcoach = new List<Coachingpairupsdtl>();
            if (string.IsNullOrEmpty(EmployeeId))
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(EmployeeId) && !string.IsNullOrEmpty(Position) && !string.IsNullOrEmpty(Plname))
            {
                getcoach = _context.Coachingpairupsdtl.Where(con => con.EmployeeId == EmployeeId && con.Position == Position && con.Plname == Plname).ToList();
            }
            //ViewBag.getcoach = getcoach;
            return View(getcoach);
        }
        public IActionResult Assignpr(string EmployeeId, string Employeename)
        {
            var getitems = new List<Items>();
            var Coachings = new List<Coachannualplans>();
            if (string.IsNullOrEmpty(EmployeeId))
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(EmployeeId) || !string.IsNullOrEmpty(Employeename))
            {
                ViewData["Coachid"] = _context.Coachannualplans.Where(c => c.EmployeeId == EmployeeId).Select(L => new SelectListItem
                {
                    Text = L.Coachname,
                    Value = L.Coachid
                }).Distinct();
                var getvalue = _context.Coachannualplans.Where(con => con.Employeename == Employeename && con.EmployeeId == EmployeeId).FirstOrDefault();
                if (getvalue != null)
                {
                    getitems = _context.Items.Where(c => c.Plcode == getvalue.Plname).ToList();
                }
                return View(getitems);
            }
            return View();
        }
        public async Task<IActionResult> Approve(string EmployeeId, string Employeename, string Position, string Plname)
        {

            ViewData["Plname"] = _context.Positions.Select(L => new SelectListItem
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();
            ViewData["Coachid"] = _context.Coaches.Select(L => new SelectListItem//Caoch ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            if (EmployeeId == null)
            {
                return NotFound();
            }
            var details = new List<Coachingpairupshdr>();
            details = _context.Coachingpairupshdr.Where(con => con.EmployeeId == EmployeeId && ((con.Approve1 == "R" || con.Approve1 == "Y" || con.Approve1 == "C") && con.Position == Position && con.Plname == Plname)).ToList();
            if (details == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var val in details)
                {
                    val.Items = _context.Items.Where(c => c.Plcode == val.Plname).ToList();
                    val.Coachingpairupsdtl = _context.Coachingpairupsdtl.Where(c => c.EmployeeId == val.EmployeeId && c.Position == val.Position && c.Plname == val.Plname).ToList();
                }
            }
            return View(details);
        }
        public async Task<IActionResult> Approves(string EmployeeId, string Employeename, string status, string Plname, string Position, string Remarks)
        {
            if (EmployeeId != null && Employeename != null && status != null)
            {
                var getdata = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == EmployeeId && c.Position == Position && c.Plname == Plname).FirstOrDefault();
                if (getdata != null)
                {
                    var getitems = _context.Items.AsNoTracking().Where(c => c.Plcode == Plname && c.Pltypes == getdata.Pllevel).ToList();
                    if (getitems.Count == 0)
                    {
                        //raise error
                        return new JsonResult(new DatabaseOperationResponse
                        {
                            Message = "There is no PL Items for this Positions, Please Either correct your Positions or Check PL Items ",
                            Status = OperationStatus.ERROR
                        });
                    }
                    else
                    {
                        if (status == "A")//First Approval
                        {
                            getdata.Approve1 = "Y";
                            getdata.Approve2 = "N";
                            getdata.Approve3 = "N";
                            getdata.AEDAT = DateTime.Now;
                            getdata.AENAM = User.Identity.Name;
                        }
                        else if (status == "RE")//Second Approval
                        {
                            getdata.Approve1 = "Y";
                            getdata.Approve2 = "R";
                            getdata.Approve3 = "R";
                            getdata.AEDAT = DateTime.Now;
                            getdata.AENAM = User.Identity.Name;
                        }
                        else if (status == "REM" && !string.IsNullOrEmpty(Remarks))//First Approval with Remark 
                        {
                            getdata.Approve1 = "Y";
                            getdata.Approve2 = "N";
                            getdata.Approve3 = "N";
                            getdata.Remarks = Remarks;
                            getdata.AEDAT = DateTime.Now;
                            getdata.AENAM = User.Identity.Name;
                        }
                        var result = await _coachinghdr.Approve(getdata).ConfigureAwait(false) as DatabaseOperationResponse;
                        return new JsonResult(result);
                    }
                    //else if (status == "R")
                    //{
                    //    getdata.Approve1 = "C";
                    //    getdata.AEDAT = DateTime.Now;
                    //    getdata.AENAM = User.Identity.Name;
                    //}
                }
            }
            return View();
        }
        public IActionResult Results(string Employeeid, string Position, string Plname)
        {
            var getemp = _context.Coachingpairupshdr.Where(c => c.EmployeeId == Employeeid && c.Position == Position && c.Plname == Plname).ToList();
            return View(getemp);
        }
        public IActionResult Createlst()
        {
            var getuser = User.Identity.Name;
            ViewBag.Roles = new SelectList(_context.Roles.Select(x => x.Name).Distinct());
            var getcurrentuser = _context.Employees.Where(c => c.EmployeeId == getuser).FirstOrDefault();
            if (getcurrentuser != null)
            {
                if (getcurrentuser.Status1 == "Y")
                {
                    //Populate All data
                    ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y" && con.Status1 == "N" && con.Costcentercode == getcurrentuser.Costcentercode).Select(L => new SelectListItem//Employee ID
                    {
                        Text = L.EmployeeId,
                        Value = L.EmployeeId,
                    }).Distinct();
                }
                else
                {
                    //See only self data
                    ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y" && con.Status1 == "N").Select(L => new SelectListItem//Employee ID
                    {
                        Text = L.EmployeeId,
                        Value = L.EmployeeId,
                    }).Distinct();
                }
            }
            else
            {
                if (getuser == "admin@MRO.com")
                {
                    ViewData["EmployeeId"] = _context.Employees.Where(con => con.Status == "Y" && con.Status1 == "N").Select(L => new SelectListItem//Employee ID
                    {
                        Text = L.EmployeeId,
                        Value = L.EmployeeId,
                    }).Distinct();
                }
            }
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["Coachid"] = _context.Coaches.Select(L => new SelectListItem//Caoch ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Assessorid"] = _context.Assessors.Select(L => new SelectListItem//Assessor ID
            {
                Text = L.EmployeeId,
                Value = L.EmployeeId,
            }).Distinct();
            ViewData["Plname"] = _context.Types.Select(L => new SelectListItem
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> Createlst(Coachingpairupshdr coachingpairupshdr)
        {
            var Trainee = new List<Coachingpairupshdr>();
            var Coaches = new List<Coachingpairupsdtl>();
            var Assessors = new List<Coachingpairupsdtl>();
            if (coachingpairupshdr.actualpls.Count > 0 && coachingpairupshdr.Coachingpairupsdtl.Count > 0 && coachingpairupshdr.Coachingpairupsdtl2.Count > 0)
            {
                Trainee = coachingpairupshdr.actualpls;
                Coaches = coachingpairupshdr.Coachingpairupsdtl;
                Assessors = coachingpairupshdr.Coachingpairupsdtl2;
                foreach (var assess in Assessors)
                {
                    var ass = _context.Assessors.Where(con => con.EmployeeId == assess.Assessorid).FirstOrDefault();
                    if (ass == null)
                    {
                        return new JsonResult(new DatabaseOperationResponse
                        {
                            Status = OperationStatus.Exist,
                            Message = "There is no Assessor with such ID in Master Table!"
                        });
                    }
                    else
                    {
                        foreach (var coach in Coaches)
                        {
                            if (assess.Assessorid == coach.Coachid)
                            {
                                return new JsonResult(new DatabaseOperationResponse
                                {
                                    Status = OperationStatus.Exist,
                                    Message = "Single Employee can't be both Assessor and Coach!"
                                });
                            }
                            foreach (var trainee in Trainee)
                            {
                                var getempdata = _context.Employees.AsNoTracking().Where(c => c.EmployeeId == trainee.EmployeeId && c.PositionId == trainee.Position).FirstOrDefault();
                                if (getempdata == null)
                                {
                                    return new JsonResult(new DatabaseOperationResponse
                                    {
                                        Status = OperationStatus.ERROR,
                                        Message = "There is Employee with Such ID in Master Table!"
                                    });
                                }
                                else
                                {
                                    //trainee.Lastpromotiondate = getempdata.Promotiondate;
                                    //trainee.Nextpromotiondate = getempdata.NextPromdate;
                                    if (trainee.EmployeeId == assess.Assessorid)
                                    {
                                        return new JsonResult(new DatabaseOperationResponse
                                        {
                                            Status = OperationStatus.ERROR,
                                            Message = "Individual Employee can't Assess him/her self!"
                                        });
                                    }
                                    else if (coach.Coachid == trainee.EmployeeId)
                                    {
                                        return new JsonResult(new DatabaseOperationResponse
                                        {
                                            Status = OperationStatus.ERROR,
                                            Message = "Individual Employee can't Coach him/her self!"
                                        });
                                    }
                                    else if (trainee.Position == trainee.Plname)
                                    {
                                        return new JsonResult(new DatabaseOperationResponse
                                        {
                                            Status = OperationStatus.ERROR,
                                            Message = "You are Trying to Assign Same Position with before one!"
                                        });
                                    }
                                    //else if (trainee.Lastpromotiondate == trainee.Nextpromotiondate)
                                    //{
                                    //    return new JsonResult(new DatabaseOperationResponse
                                    //    {
                                    //        Status = OperationStatus.ERROR,
                                    //        Message = "Last Promotion Date Must Different From Next Promotion!"
                                    //    });
                                    //}
                                    //else if (trainee.Lastpromotiondate >= trainee.Nextpromotiondate)
                                    //{
                                    //    return new JsonResult(new DatabaseOperationResponse
                                    //    {
                                    //        Status = OperationStatus.ERROR,
                                    //        Message = "Next Promotion Date must Greater than! Last Promotion Date"
                                    //    });
                                    //}
                                    else
                                    {

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
        }
        public async Task<IActionResult> Remarks(long? Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }
            var getdata = await _context.Coachingpairupshdr.AsNoTracking().Where(c => c.Id == Id).FirstOrDefaultAsync().ConfigureAwait(false);
            return View(getdata);
        }
        public IActionResult Coachinggraphs(string Employeeid, string Plcode, string Plname)
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
        public List<Wholedata> Completedtask()
        {
            var notstr = 0;
            var ready = 0;
            var firstapp = 0;
            var firstappr = 0;
            var firstrej = 0;
            //var firstrejt = 0;
            var sectapp = 0;
            var sectappr = 0;
            var sectrej = 0;
            var getall = _context.Coachingpairupshdr.Where(c => c.Status == "Y").ToList();
            var getall1 = getall.GroupBy(x => new { x.Status }).Select(p => p.FirstOrDefault()).Select(p => new Coachingpairupshdr
            {
                EmployeeId = p.EmployeeId,
                Employeename = p.Employeename,
                Plname = p.Plname,
                Pllevel = p.Pllevel,
                Status = p.Status,
                ATAREFCODE = p.ATAREFCODE,
                TSFNCODE = p.TSFNCODE,
                Approve1 = p.Approve1,
                Approve2 = p.Approve2,
                Approve3 = p.Approve3,
            }).ToList();
            foreach (var val in getall)
            {
                if (val.Approve1 == "N" && val.Approve2 == "N" && val.Approve3 == "N")//Not at All
                {
                    notstr = notstr + 1;
                }
                else if (val.Approve1 == "R" && val.Approve2 == "N" && val.Approve3 == "N")// Ready for Approval by Manager
                {
                    ready = ready + 1;
                }
                else if (val.Approve1 == "Y" && val.Approve2 == "N" && val.Approve3 == "N")//Approved by Manager
                {
                    firstapp = firstapp + 1;
                }
                else if (val.Approve1 == "Y" && val.Approve2 == "Y" && val.Approve3 == "N")//Approved by HR
                {
                    firstappr = firstappr + 1;
                }
                else if (val.Approve1 == "Y" && val.Approve2 == "R" && val.Approve3 == "N")//Rejected by HR
                {
                    firstrej = firstrej + 1;
                }
                //else if (val.Approve1 == "Y" && val.Approve2 == "R" && val.Approve3 == "N")//Rejected by HR
                //{
                //    firstrejt = firstrejt + 1;
                //}
                else if (val.Approve1 == "Y" && val.Approve2 == "R" && val.Approve3 == "R")//Approved again by Manager
                {
                    sectapp = sectapp + 1;
                }
                else if (val.Approve1 == "Y" && val.Approve2 == "R" && val.Approve3 == "Y")//Approved again by HR
                {
                    sectappr = sectappr + 1;
                }
                else if (val.Approve1 == "R" && val.Approve2 == "R" && val.Approve3 == "R")//Rejected again by Manager
                {
                    sectrej = sectrej + 1;
                }
            }
            List<Wholedata> valgrp = new List<Wholedata>()
                {
                    new Wholedata(){key="Not Started",value=notstr},
                    new Wholedata(){key="Ready",value=ready},
                    new Wholedata(){key="Approved by Sec. Manager",value=firstapp},
                    new Wholedata(){key="Approved by MRO HR",value=firstappr},
                    new Wholedata(){key="Rejected by MRO HR",value=firstrej},
                    new Wholedata(){key="Approved Again bySec. Manager",value=sectapp},
                    new Wholedata(){key="Approved Again by MRO HR",value=sectappr},
                    new Wholedata(){key="Rejected Again by MRO HR",value=firstrej},
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
    }
}

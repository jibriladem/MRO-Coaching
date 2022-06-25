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
    public class AssessingplController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.BasicTransactions.Coaching _coaching;
        private readonly DataObjects.Models.BasicTransactions.Coachingpairupshdr _coachinghdr;
        private readonly DataObjects.Models.BasicTransactions.Practicalassessments _assessments;
        private readonly DataObjects.Models.BasicTransactions.Pracchapterdesc _chapters;
        private readonly DataObjects.Models.BasicTransactions.Pracmaintcddesc _tsfnno;
        private readonly DataObjects.Models.BasicTransactions.Practaskcodedesc _taskcode;
        private readonly UserManager<ApplicationUser> _userManager;
        public AssessingplController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _coaching = new DataObjects.Models.BasicTransactions.Coaching(context);
            _coachinghdr = new DataObjects.Models.BasicTransactions.Coachingpairupshdr(context);
            _assessments = new DataObjects.Models.BasicTransactions.Practicalassessments(context);
            _chapters = new DataObjects.Models.BasicTransactions.Pracchapterdesc(context);
            _tsfnno = new DataObjects.Models.BasicTransactions.Pracmaintcddesc(context);
            _taskcode = new DataObjects.Models.BasicTransactions.Practaskcodedesc(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var getdatas = _context.Coaching.Where(c => c.Actionpln == "Y").ToList();
            if (getdatas.Count > 0)
            {
                var getassdate = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == getdatas.FirstOrDefault().EmployeeId).Select(c => c.Whentoassess).FirstOrDefault();
                var getdata = getdatas.GroupBy(c => c.EmployeeId).ToList();
            }
            return View(getdatas);
        }
        public async Task<IActionResult> Indexs(string Employeeid, long? ID, string Position, string Plname)
        {
            if (Employeeid == null || ID == 0)
            {
                return NotFound();
            }
            var getdata = _context.Coaching.Where(c => c.EmployeeId == Employeeid && c.Status == "C").FirstOrDefault();
            if (getdata != null)
            {
                var getdatas = _context.Coachingpairupshdr
                    .Where(c => c.EmployeeId == getdata.EmployeeId && c.Position == Position && c.Plname == Plname).ToList();
                if (getdatas.Count > 0)
                {
                    var getqnsamt = _context.Assquestionlimits.AsNoTracking().Where(c => c.Assessmenttype == getdatas.FirstOrDefault().Plname).Select(c => c.Questioncnt).FirstOrDefault();
                    if (getqnsamt != null)
                    {
                        foreach (var item in getdatas)
                        {
                            item.getcompleted = 0;
                            item.getwarning = 0;
                            item.getrejected = 0;
                            item.Coachingpairupsdtl = _context.Coachingpairupsdtl.AsNoTracking().Where(c => c.EmployeeId == Employeeid && c.Position == Position && c.Plname == Plname).ToList();
                            item.Items = _context.Items.AsNoTracking().Where(c => c.Plcode == Plname && c.Assesment == "Y").ToList();
                            item.Coaching = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == item.EmployeeId).ToList();
                            item.getqnsamt = getqnsamt;
                            if (item.Items.Count > 0)
                            {
                                foreach (var item1 in item.Items)
                                {
                                    item1.Selected = item.Coaching.Where(c => c.Taskcode == item1.Taskcode && c.Taskcount == Convert.ToInt32(item1.Taskcount)).Select(c => c.Assaction).FirstOrDefault();
                                    if (item1.Selected == "ACC")
                                    {
                                        item.getcompleted = 1 + item.getcompleted;
                                    }
                                    else if (item1.Selected == "ACR")
                                    {
                                        item.getwarning = item.getwarning + 1;
                                    }
                                    else if (item1.Selected == "REJ")
                                    {
                                        item.getwarning = item.getrejected + 1;
                                    }
                                    item1.ATAREFCODE = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == item.EmployeeId && c.Taskcode == item1.Taskcode && c.Taskcount == item1.Taskcount).Select(c => c.ATAREFCODE).FirstOrDefault();
                                    item1.TSFNCODE = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == item.EmployeeId && c.Taskcode == item1.Taskcode && c.Taskcount == item1.Taskcount).Select(c => c.TSFNCODE).FirstOrDefault();
                                }
                            }
                        }
                        if (getdatas.FirstOrDefault().Items.Count > 0)
                        {
                            var itemlst = getdatas.FirstOrDefault().Items.GroupBy(c => c.Taskcode);
                            ViewData["Taskcode"] = itemlst.Select(L => new SelectListItem
                            {
                                Text = L.Key,
                                Value = L.Key

                            }).Distinct();
                        }
                        return View(getdatas);

                    }
                    else
                    {
                        return new JsonResult(new DatabaseOperationResponse
                        {
                            Status = OperationStatus.ERROR,
                            Message = "There is no Standard Questions amount in Master Table, Please Set the Standard!"
                        });
                    }
                }
                else
                {
                    return new JsonResult(new DatabaseOperationResponse
                    {
                        Status = OperationStatus.ERROR,
                        Message = "There is no Pairups data with such criteria!"
                    });
                }
            }
            else
            {
                return new JsonResult(new DatabaseOperationResponse
                {
                    Status = OperationStatus.ERROR,
                    Message = "There is not Data with such criteria!"
                });
            }
            //return View();
            //return View(await _context.Coaching.Where(C => C.Actionpln == "Y" && C.Actionfrm <= DateTime.Now && C.Actiontoo >= DateTime.Now).ToListAsync().ConfigureAwait(false));
        }
        [HttpGet]
        public IActionResult Assessing(string Employeeid, long? ID, string Position, string Plname)
        {
            if (Employeeid == null || ID == 0)
            {
                return NotFound();
            }
            //var getpairuphdr = new List<Coachingpairupshdr>();
            //var getpairupdtl = new List<Coachingpairupsdtl>();
            //var getitems = new List<Items>();
            //var getcoaching = new List<Coaching>();
            //var getcoachings = new List<Coaching>();

            //getcoaching = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == Employeeid && c.Status == "C").ToList();
            //if (getcoaching.Count > 0)
            //{
            //    getpairuphdr = _context.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == Employeeid && c.Position == Position && c.Plname == Plname).ToList();
            //    if (getpairuphdr.Count > 0)
            //    {
            //        getpairupdtl = _context.Coachingpairupsdtl.AsNoTracking().Where(c => c.EmployeeId == Employeeid && c.Position == Position && c.Plname == Plname).ToList();
            //        if (getpairupdtl.Count > 0)
            //        {
            //            getitems = _context.Items.AsNoTracking().Where(c => c.Plcode == Plname && c.Assesment == "Y").ToList();
            //            if (getitems.Count > 0)
            //            {
            //                foreach (var item in getitems)
            //                {
            //                    foreach (var coach in getcoaching)
            //                    {
            //                        if (item.Taskcode == coach.Taskcode && item.Taskcount == coach.Taskcount)
            //                        {
            //                            getcoachings.Add(new Coaching()
            //                            {
            //                                EmployeeId = coach.EmployeeId,
            //                                Employeename = coach.Employeename,
            //                                Coachid = coach.Coachid,
            //                                Coachname = coach.Coachname,
            //                                Traineeapproval = coach.Traineeapproval,
            //                                Coachapproval = coach.Coachapproval,
            //                                Dateapproved = coach.Dateapproved,
            //                                Taskcode = coach.Taskcode,
            //                                Taskdesc = coach.Taskdesc,
            //                                Actionfrm = coach.Actionfrm,
            //                                Actiontoo = coach.Actiontoo,
            //                                Actionpln = coach.Actionpln,
            //                                Status = coach.Status,
            //                                ATAREFCODE = coach.ATAREFCODE,
            //                                TSFNCODE = coach.TSFNCODE,
            //                                Reasons = coach.Reasons,
            //                                ID = coach.ID,
            //                            });
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //var Getall = new Assessmentsview
            //{
            //    Items = getitems,
            //    Coaching = getcoachings,
            //    Coachingpairupsdtls = getpairupdtl,
            //    Coachingpairupshdrs = getpairuphdr,
            //};
            var getdata = _context.Coaching.Where(c => c.EmployeeId == Employeeid && c.Status == "C").FirstOrDefault();
            if (getdata != null)
            {
                var getdatas = _context.Coachingpairupshdr
                    .Where(c => c.EmployeeId == getdata.EmployeeId && c.Position == Position && c.Plname == Plname).ToList();
                if (getdatas.Count > 0)
                {
                    var getqnsamt = _context.Assquestionlimits.AsNoTracking().Where(c => c.Assessmenttype == getdatas.FirstOrDefault().Plname).Select(c => c.Questioncnt).FirstOrDefault();
                    if (getqnsamt != null)
                    {
                        foreach (var item in getdatas)
                        {
                            item.Coachingpairupsdtl = _context.Coachingpairupsdtl.AsNoTracking().Where(c => c.EmployeeId == Employeeid && c.Position == Position && c.Plname == Plname).ToList();
                            item.Items = _context.Items.AsNoTracking().Where(c => c.Plcode == Plname && c.Assesment == "Y").ToList();
                            item.getqnsamt = getqnsamt;
                            if (item.Items.Count > 0)
                            {
                                foreach (var item1 in item.Items)
                                {
                                    item1.ATAREFCODE = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == item.EmployeeId && c.Taskcode == item1.Taskcode && c.Taskcount == item1.Taskcount).Select(c => c.ATAREFCODE).FirstOrDefault();
                                    item1.TSFNCODE = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == item.EmployeeId && c.Taskcode == item1.Taskcode && c.Taskcount == item1.Taskcount).Select(c => c.TSFNCODE).FirstOrDefault();
                                }
                            }
                            //item.ATAREFCODE = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == item.EmployeeId).Select(c => c.ATAREFCODE).FirstOrDefault();
                            //item.TSFNCODE = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == item.EmployeeId).Select(c => c.TSFNCODE).FirstOrDefault();
                        }
                        return View(getdatas);

                    }
                    else
                    {
                        return new JsonResult(new DatabaseOperationResponse
                        {
                            Status = OperationStatus.ERROR,
                            Message = "There is no Standard Questions amount in Master Table, Please Set the Standard!"
                        });
                    }
                }
                else
                {
                    return new JsonResult(new DatabaseOperationResponse
                    {
                        Status = OperationStatus.ERROR,
                        Message = "There is no Pairups data with such criteria!"
                    });
                }
            }
            else
            {
                return new JsonResult(new DatabaseOperationResponse
                {
                    Status = OperationStatus.ERROR,
                    Message = "There is not Data with such criteria!"
                });
            }
            // return View();
        }
        [HttpPost]
        public async Task<IActionResult> Views(List<Items> coachings)
        {
            var getlist = new List<Coaching>();
            var getlists = new List<Coaching>();
            if (coachings.Count > 0)
            {
                foreach (var items in coachings)
                {
                    var getdata = _context.Coaching.AsNoTracking().Where(c => c.Taskcode == items.Taskcode && c.Taskcount == items.Taskcount).FirstOrDefault();
                    if (getdata != null)
                    {
                        getlist.Add(new Coaching()
                        {
                            ID = getdata.ID,
                            EmployeeId = getdata.EmployeeId,
                            Employeename = getdata.Employeename,
                            Coachapproval = getdata.Coachapproval,
                            Coachid = getdata.Coachid,
                            Coachname = getdata.Coachname,
                            Actionfrm = getdata.Actionfrm,
                            Actiontoo = getdata.Actiontoo,
                            Actionpln = getdata.Actionpln,
                            Taskcode = items.Taskcode,
                            Taskdesc = items.Description,
                            ATAREFCODE = items.ATAREFCODE,
                            TSFNCODE = items.TSFNCODE,
                            Taskcount = items.Taskcount,
                            Plcode = getdata.Plcode,
                            Pllevel = getdata.Pllevel,
                            Plname = getdata.Plname,
                            Dateapproved = getdata.Dateapproved,
                            ERDAT = DateTime.Now,
                            ERNAM = User.Identity.Name,
                            AEDAT = DateTime.Now,
                            AENAM = User.Identity.Name,
                        });
                    }
                }
                if (getlist.Count > 0)
                {

                }
            }
            return View(getlist);
        }
        [HttpGet]
        public IActionResult Evaluationdtl(string Employeeid, string sender, string Plcode, string Plname, string Pllevel)
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
            var getevaluation = _context.Coaching.Where(c => c.EmployeeId == Employeeid && c.Actionpln == "Y" && c.Plcode == Plcode && c.Plname == Plname && c.Pllevel == Pllevel/* && c.Actionfrm <= DateTime.Now && c.Actiontoo >= DateTime.Now*/).ToList();
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
            return View(getevaluation);
        }
        public IActionResult Assessments(long Id, string Taskcode, string EmployeeId, long? Taskcount)
        {
            var getdata = _context.Coaching.AsNoTracking().Where(c => c.ID == Id && c.EmployeeId == EmployeeId && c.Taskcode == Taskcode && c.Taskcount == Taskcount).FirstOrDefault();
            if (getdata != null)
            {
                getdata.Taskname = _context.Taskcode.AsNoTracking().Where(c => c.Keyword == getdata.Taskcode).Select(c => c.Descriptions).FirstOrDefault();
            }
            return View(getdata);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Assessments(Coaching coaching, string status)
        {
            if (coaching.Plcode != null && coaching.Traineeapproval != null)
            {
                //check exist
                var exist = _context.Practicalassessments.AsNoTracking().Where(c => c.Plcode == coaching.Plcode && c.Employeeid == coaching.Traineeapproval).FirstOrDefault();
                if (exist != null)
                {
                    _assessments.Id = exist.Id;
                    _assessments.Employeeid = exist.Employeeid;
                    _assessments.Assessorid = exist.Assessorid;
                    _assessments.Assessorname = exist.Assessorname;
                    _assessments.Plcode = exist.Plcode;
                    _assessments.Taskcount = exist.Taskcount + ',' + coaching.Taskcount;
                    _assessments.Trnseqnumber = exist.Trnseqnumber + 1;
                    _assessments.Taskgrpcode = exist.Taskgrpcode + ',' + coaching.Taskcode;
                    _assessments.Refgrpcode = exist.Refgrpcode + ',' + coaching.ATAREFCODE;
                    _assessments.Mntgrpcode = exist.Mntgrpcode + ',' + coaching.TSFNCODE;
                }
                else
                {
                    _assessments.Employeeid = coaching.Traineeapproval;
                    //_assessments.Assessorid = coaching.Assessorid;
                    //_assessments.Assessorname = exist.Assessorname;
                    _assessments.Trnseqnumber = 1;
                    _assessments.Plcode = coaching.Plcode;
                    _assessments.Taskgrpcode = coaching.Taskcode;
                    _assessments.Refgrpcode = coaching.ATAREFCODE;
                    _assessments.Mntgrpcode = coaching.TSFNCODE;
                    _assessments.Dateassessed = DateTime.Now;

                }
                _taskcode.Taskode = coaching.Taskcode;
                _taskcode.Taskdesc = coaching.Taskdesc;
                _taskcode.Taskcount = coaching.Taskcount;
                _taskcode.Plcode = coaching.Plcode;
                _taskcode.Employeeid = coaching.Traineeapproval;
                _chapters.Employeeid = coaching.Traineeapproval;
                _chapters.Plcode = coaching.Plcode;
                _chapters.Referencenbr = coaching.ATAREFCODE;
                var ind = coaching.ATAREFCODE.Substring(0, 2);
                coaching.Actionfrm = coaching.Dateapproved;

                _chapters.Referencedesc = _context.ATAChapters.AsNoTracking().Where(c => c.ATA_Chapter == ind).Select(c => c.Chapter_Title).FirstOrDefault();

                var getcoaching = _context.Coaching.AsNoTracking().Where(c => c.EmployeeId == _assessments.Employeeid && c.Plcode == coaching.Plcode && c.Taskcode == coaching.Taskcode && c.Taskcount == Convert.ToInt32(coaching.Taskcount)).FirstOrDefault();
                if (getcoaching != null)
                {
                    getcoaching.Assaction = status;
                }
                var result = await _assessments.Save(_assessments, _chapters, _taskcode, _tsfnno, getcoaching).ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }

            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        public IActionResult Assessmentgraph(string Employeeid, string Plcode, string Plname)
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
            var trainee = 0;
            var coaches = 0;
            var getall = _context.Coaching.ToList().Where(c => c.Actionpln == "Y");
            var getall1 = getall.GroupBy(x => new { x.Status }).Select(p => p.FirstOrDefault()).Select(p => new Coaching
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
                if (val.Status == "N" /*&& val.Traineeapproval == "" && val.Coachapproval == "N"*/)//Not Started
                {
                    notstr = notstr + 1;
                }
                else if (val.Status == "T" /*&& val.Traineeapproval != "" && val.Coachapproval == ""*/)// Trainee Signed
                {
                    trainee = trainee + 1;
                }
                else if (val.Status == "C" /*&& val.Traineeapproval != "" && val.Coachapproval != ""*/)//Both Coaches and Trainee Signed
                {
                    coaches = coaches + 1;
                }
            }
            List<Wholedata> valgrp = new List<Wholedata>()
                {
                    new Wholedata(){key="Not Started",value=notstr},
                    new Wholedata(){key="Trainee Signed",value=trainee},
                    new Wholedata(){key="Task Completed",value=coaches},
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
        [HttpGet]
        public IActionResult Finalassessment()
        {
            var getassessmentqns = _context.Questionaries.AsNoTracking().ToList();
            return View(getassessmentqns);
        }
    }
}

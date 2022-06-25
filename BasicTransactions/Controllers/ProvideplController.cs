using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class ProvideplController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Coachannualplans _annualplans;
        private readonly Coachingpairupshdr _coachinghdr;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProvideplController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _annualplans = new Coachannualplans(context);
            _coachinghdr = new Coachingpairupshdr(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coachingpairupshdr.Where(con => con.Whentoassess >= DateTime.Now && con.Approve1 == "Y" /*con.Approve1 == "Y" && con.Approve2 == "N"*/).ToListAsync().ConfigureAwait(false));
        }
        public async Task<IActionResult> Indexs(string status)
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            if (status != null)
            {
                if (status == "ALL")
                {
                    var getdata = _context.Coachingpairupshdr.Where(c => c.Approve1 == "Y").ToList();
                    if (getdata.Count > 0)
                    {

                    }
                    //return View(getdata);
                    return View(await _context.Coachingpairupshdr.Where(con => con.Approve1 == "Y" && (con.Approve2 == "N" || con.Approve2 == "R" || con.Approve2 == "Y")).ToListAsync().ConfigureAwait(false));
                }
                else if (status == "ACC")
                {
                    return View(await _context.Coachingpairupshdr.Where(con => con.Approve1 == "Y" && con.Approve2 != "Y").ToListAsync().ConfigureAwait(false));
                }
                else if (status == "REJ")
                {
                    return View(await _context.Coachingpairupshdr.Where(con => con.Approve1 == "Y" && con.Approve2 != "R").ToListAsync().ConfigureAwait(false));
                }
            }
            else
            {
                return View();
            }
            //var getalldata = _context.Coachingpairupshdr.Where(con => con.Approve1 == "Y" && con.Approve2 != "R").ToList();
            //if (getalldata.Count > 0)
            //{
            //    foreach (var lst in getalldata)
            //    {
            //        lst.Coachingpairupsdtl = _context.Coachingpairupsdtl.Where(c => c.EmployeeId == lst.EmployeeId && c.Position == lst.Position && c.Plname == lst.Plname).ToList();
            //        lst.Items = _context.Items.Where(c => c.Plcode == lst.Plname).ToList();
            //    }
            //}
            return View();
        }
        [HttpGet]
        public IActionResult Reasons(string Employeeid, string Position, string Plname)
        {
            var getemp = _context.Coachingpairupshdr.Where(c => c.EmployeeId == Employeeid && c.Position == Position && c.Plname == Plname).FirstOrDefault();
            return View(getemp);
        }
        public IActionResult Edit(string Employeeid, string Position, string Plname)
        {
            if (Employeeid == null)
            {
                return NotFound();
            }
            var details = new List<Coachingpairupshdr>();
            details = _context.Coachingpairupshdr.Where(con => con.EmployeeId == Employeeid && con.Approve1 == "Y" && con.Position == Position && con.Plname == Plname).ToList();
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
                //details._dbContext = null;
                //details.Items = _context.Items.Where(c => c.Plcode == details.Plname).ToList();
            }
            //return View(await _context.Coachannualplans.Where(c => c.Approve1 == "Y").ToListAsync().ConfigureAwait(false));
            return View(details);
        }
        public async Task<IActionResult> Approves(string Employeeid, string Position, string Plname, string status)
        {
            if (Employeeid != null)
            {
                var getcoach = new List<Items>();
                var getcoaching = new List<Coachingpairupshdr>();
                var dtl = new List<Coachingpairupsdtl>();
                var items = new List<Coaching>();
                var reason = "Checked and Approved by ( " + User.Identity.Name + ") on (" + DateTime.Now + " )";
                var coachannualplans = _context.Coachingpairupshdr.AsNoTracking().Where(con => con.EmployeeId == Employeeid && con.Position == Position && con.Plname == Plname).FirstOrDefault();
                if (coachannualplans != null)
                {
                    if (status == "A")
                    {
                        coachannualplans.Approve2 = "Y";
                        coachannualplans.AEDAT = DateTime.Now;
                        coachannualplans.AENAM = User.Identity.Name;
                        coachannualplans.Coachingpairupsdtl = dtl;
                        coachannualplans.Reasons = reason;
                    }
                    var result = await coachannualplans.Approves().ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }
            }
            return new JsonResult(null);
        }
        public async Task<IActionResult> Cancel(string Employeeid)
        {
            if (Employeeid != null)
            {
                //var getcoach = new List<Items>();
                var coachannualplans = await _context.Coachannualplans.AsNoTracking().Where(con => con.EmployeeId == Employeeid).ToListAsync();
                if (coachannualplans.Count > 0)
                {
                    foreach (var val in coachannualplans)
                    {
                        val.Approve2 = "N";
                        val.AEDAT = DateTime.Now;
                        val.AENAM = User.Identity.Name;
                    }
                    var result = await _annualplans.SaveList1(coachannualplans).ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }
            }
            return new JsonResult(null);
        }
        public IActionResult Getdocument(string EmployeeId)
        {
            var task = new List<Taskcode>();
            var task1 = new List<Taskcode>();
            var getdoc = _context.Detailinfo.ToList();
            var getpluser = _context.Plusers.ToList();
            if (EmployeeId != null)
            {
                var getpaired = _context.Coachingpairupshdr.Where(c => c.EmployeeId == EmployeeId).FirstOrDefault();
                if (getpaired != null)
                {
                    var getpl = _context.Items.Where(c => c.Plcode == getpaired.Plname).ToList();
                    if (getpl.Count > 0)
                    {
                        foreach (var val in getpl)
                        {
                            var gettaskdesc = _context.Taskcode.Where(c => c.Keyword == val.Taskcode).FirstOrDefault();
                            if (gettaskdesc != null)
                            {
                                task.Add(new Taskcode()
                                {
                                    Keyword = gettaskdesc.Keyword,
                                    Descriptions = gettaskdesc.Descriptions,
                                    Divisions = gettaskdesc.Divisions,
                                });
                            }
                        }
                    }
                }
            }
            if (task.Count > 0)
            {
                task.Sort((s1, s2) =>
                {
                    var compare = s1.Keyword.CompareTo(s2.Keyword);
                    return compare;
                });
                task1 = task.GroupBy(x => x.Keyword).Select(x => x.FirstOrDefault()).ToList(); // Filter and Delete Duplicate record
            }
            var Getall = new GeneralInfoView
            {
                Detailinfos = getdoc,
                Taskcodes = task1,
                Plusers = getpluser
            };
            return View(Getall);
        }
        [HttpPost]
        public async Task<IActionResult> Reasons(Coachingpairupshdr coachingpairupshdr)
        {
            if (coachingpairupshdr.Reasons != null)
            {
                var getalldata = _context.Coachingpairupshdr.Where(c => c.EmployeeId == coachingpairupshdr.EmployeeId && c.Position == coachingpairupshdr.Position && c.Plname == coachingpairupshdr.Plname).FirstOrDefault();
                if (getalldata != null)
                {
                    getalldata.Reasons = coachingpairupshdr.Reasons;
                    getalldata.Approve1 = "Y";
                    getalldata.Approve2 = "R";
                    getalldata.Approve3 = "N";
                    getalldata.AEDAT = DateTime.Now;
                    getalldata.AENAM = User.Identity.Name;
                    var result = await getalldata.Reject().ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }
            }
            else
            {
                return new JsonResult(new DatabaseOperationResponse
                {
                    Status = OperationStatus.Exist,
                    Message = "Reason for Cancellation is Mandatory!"
                });
            }
            return new JsonResult(null);
        }
    }
}

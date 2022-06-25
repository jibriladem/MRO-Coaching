using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.BasicTransactions;
using MROCoatching.DataObjects.Models.General;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.Dashboards.Controllers
{
    [Area("Dashboards")]
    public class DashboardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.BasicTransactions.Coachactionplans _actionplans;
        private readonly DataObjects.Models.BasicTransactions.Coaching _coaching;
        //private readonly DataObjects.Models.BasicTransactions.Coachingpairupshdr _coachinghdr;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _actionplans = new DataObjects.Models.BasicTransactions.Coachactionplans(context);
            _coaching = new DataObjects.Models.BasicTransactions.Coaching(context);
            //_coachinghdr = new DataObjects.Models.BasicTransactions.Coachingpairupshdr(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            //var nottaken = 0;
            //var taken = 0;
            //var getall = _context.Coaching.ToList();
            //var getall1 = getall.GroupBy(x => new { x.Actionpln }).Select(p => p.FirstOrDefault()).Select(p => new Coaching
            //{
            //    EmployeeId = p.EmployeeId,
            //    Employeename = p.Employeename,
            //    Plcode = p.Plcode,
            //    Plname = p.Plname,
            //    Pllevel = p.Pllevel,
            //    Status = p.Status,
            //    Traineeapproval = p.Traineeapproval,
            //    Coachapproval = p.Coachapproval,
            //    ATAREFCODE = p.ATAREFCODE,
            //    TSFNCODE = p.TSFNCODE,
            //    Actionfrm = p.Actionfrm,
            //    Actionpln = p.Actionpln,
            //    Actiontoo = p.Actiontoo,
            //}).ToList();
            //foreach (var val in getall)
            //{
            //    DateTime? mnth = val.Actionfrm;
            //    var mnth1 = mnth;
            //    if (val.Actionpln == "N")
            //    {
            //        nottaken = nottaken + 1;
            //    }
            //    else if (val.Actionpln == "Y")
            //    {
            //        taken = taken + 1;
            //    }
            //}
            //List<Wholedata> valgrp = new List<Wholedata>()
            //    {
            //        new Wholedata(){key="Action Plan Not Taken",value=nottaken},
            //        new Wholedata(){key="Action Plan Taken",value=taken},
            //    };

            //Dashboard getdashboard = new Dashboard()
            //{
            //    Wholedatas = valgrp,
            //};
            //string? Employeeid, string? Plcode, string? Plname

            List<object> data = new List<object>();
            data.Add(GetAnualPlan());
            return View(data);
        }

       
        public JsonResult GetAnualPlan()
        {
            var nottaken = 0;
            var taken = 0;
            //if (Employeeid != null && Plcode != null && Plname != null)
            //{
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
            //        //string? month = Convert.ToString(val.Actionfrm);
            //        //Console.WriteLine(month);
            //        ////month = month[0, 2];
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
            //    {
            //        new Wholedata(){key="Action Plan not Taken",value=nottaken},
            //        new Wholedata(){key="Action Plan Taken",value=taken},
            //    };

            //    Dashboard getdashboard = new Dashboard()
            //    {

            //        Wholedatas = valgrp,
            //    };
            //    return Json(getdashboard);
            //}

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
            return Json(getdashboard);

        }
        //public IActionResult Actiongraph(string? Employeeid, string? Plcode, string? Plname)
        //{
        //    var nottaken = 0;
        //    var taken = 0;
        //    if (Employeeid != null && Plcode != null && Plname != null)
        //    {
        //        var getall = _context.Coaching.Where(c => c.EmployeeId == Employeeid && c.Plcode == Plcode && c.Plname == Plname).ToList();
        //        var getall1 = getall.GroupBy(x => new { x.Actionpln }).Select(p => p.FirstOrDefault()).Select(p => new Coaching
        //        {
        //            EmployeeId = p.EmployeeId,
        //            Employeename = p.Employeename,
        //            Plcode = p.Plcode,
        //            Plname = p.Plname,
        //            Pllevel = p.Pllevel,
        //            Status = p.Status,
        //            ATAREFCODE = p.ATAREFCODE,
        //            TSFNCODE = p.TSFNCODE,
        //            Actionfrm = p.Actionfrm,
        //            Actionpln = p.Actionpln,
        //            Actiontoo = p.Actiontoo,
        //        }).ToList();
        //        foreach (var val in getall)
        //        {
        //            //string? month = Convert.ToString(val.Actionfrm);
        //            //Console.WriteLine(month);
        //            ////month = month[0, 2];
        //            if (val.Actionpln == "N")
        //            {
        //                nottaken = nottaken + 1;
        //            }
        //            else if (val.Actionpln == "Y")
        //            {
        //                taken = taken + 1;
        //            }
        //        }
        //        List<Wholedata> valgrp = new List<Wholedata>()
        //        {
        //            new Wholedata(){key="Action Plan not Taken",value=nottaken},
        //            new Wholedata(){key="Action Plan Taken",value=taken},
        //        };

        //        Dashboard getdashboard = new Dashboard()
        //        {

        //            Wholedatas = valgrp,
        //        };
        //        return View(getdashboard);
        //    }
        //    else
        //    {
        //        var getall = _context.Coaching.ToList();
        //        var getall1 = getall.GroupBy(x => new { x.Actionpln }).Select(p => p.FirstOrDefault()).Select(p => new Coaching
        //        {
        //            EmployeeId = p.EmployeeId,
        //            Employeename = p.Employeename,
        //            Plcode = p.Plcode,
        //            Plname = p.Plname,
        //            Pllevel = p.Pllevel,
        //            Status = p.Status,
        //            Traineeapproval = p.Traineeapproval,
        //            Coachapproval = p.Coachapproval,
        //            ATAREFCODE = p.ATAREFCODE,
        //            TSFNCODE = p.TSFNCODE,
        //            Actionfrm = p.Actionfrm,
        //            Actionpln = p.Actionpln,
        //            Actiontoo = p.Actiontoo,
        //        }).ToList();
        //        foreach (var val in getall)
        //        {
        //            if (val.Actionpln == "N")
        //            {
        //                nottaken = nottaken + 1;
        //            }
        //            else if (val.Actionpln == "Y")
        //            {
        //                taken = taken + 1;
        //            }
        //        }
        //        List<Wholedata> valgrp = new List<Wholedata>()
        //        {
        //            new Wholedata(){key="Action Plan Not Taken",value=nottaken},
        //            new Wholedata(){key="Action Plan Taken",value=taken},
        //        };

        //        Dashboard getdashboard = new Dashboard()
        //        {
        //            Wholedatas = valgrp,
        //        };
        //        return View(getdashboard);
        //    }
        //}
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

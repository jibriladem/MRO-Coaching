using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.BasicTransactions.Controllers
{
    [Area("BasicTransactions")]
    public class MyTeamsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.MasterTables.Employees _employees;
        private readonly DataObjects.Models.General.Costcenters _costcenters;
        private readonly DataObjects.Models.General.TeamLeader _teams;
        private readonly DataObjects.Models.General.Departments _depts;
        private readonly DataObjects.Models.BasicTransactions.Coachingpairupshdr _coachinghdr;
        private readonly UserManager<ApplicationUser> _userManager;
        public MyTeamsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _costcenters = new DataObjects.Models.General.Costcenters(context);
            _teams = new DataObjects.Models.General.TeamLeader(context);
            _depts = new DataObjects.Models.General.Departments(context);
            _coachinghdr = new DataObjects.Models.BasicTransactions.Coachingpairupshdr(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}

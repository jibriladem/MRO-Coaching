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

namespace MROCoatching.Web.Areas.General.Controllers
{
    [Area("General")]
    public class TeamLeadersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.General.TeamLeader _teams;
        private readonly UserManager<ApplicationUser> _userManager;
        public TeamLeadersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _teams = new DataObjects.Models.General.TeamLeader(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.TeamLeader.ToListAsync().ConfigureAwait(false));
        }
        // GET: General/TeamLeader/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sections = await _context.TeamLeader.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (sections == null)
            {
                return NotFound();
            }

            return View(sections);
        }
        // GET: General/TeamLeader/Create
        public IActionResult Create()
        {
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["Sectcode"] = _context.Sections.Where(con => con.Subsec == "Y").Select(L => new SelectListItem//Section Code
            {
                Text = L.Sectcode,
                Value = L.Sectcode,
            }).Distinct();
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Department Code
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();

            return PartialView();
        }
        // GET: General/TeamLeader/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamLeader teams)
        {
            if (ModelState.IsValid)
            {
                //_teams.Teamcode = teams.Teamcode;
                //_teams.Teamname = teams.Teamname;
                _teams.Sectcode = teams.Sectcode;
                _teams.Sectname = teams.Sectname;
                _teams.Costcentercode = teams.Costcentercode;
                _teams.Reportsto = teams.Reportsto;
                _teams.Costcentername = teams.Costcentername;
                if (_teams.Costcentercode == null)
                {
                    _teams.Costcentercode = _context.Sections.Where(con => con.Sectcode == _teams.Sectcode).Select(con => con.Costcentercode).FirstOrDefault();
                    _teams.Costcentername = _context.Sections.Where(con => con.Sectcode == _teams.Sectcode).Select(con => con.Costcentername).FirstOrDefault();
                }
                _teams.Fromdate = teams.Fromdate;
                _teams.Toodate = teams.Toodate;
                _teams.ERNAM = User.Identity.Name;
                _teams.AENAM = User.Identity.Name;
                _teams.ERDAT = DateTime.Now;
                _teams.AEDAT = DateTime.Now;
                var exist = await _teams.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _teams.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: General/TeamLeader/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["Sectcode"] = _context.Sections.Where(con => con.Subsec == "Y").Select(L => new SelectListItem//Section Code
            {
                Text = L.Sectcode,
                Value = L.Sectcode,
            }).Distinct();
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Department Code
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.TeamLeader.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: General/TeamLeader/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, TeamLeader teams)
        {
            if (ModelState.IsValid)
            {
                _teams.Id = teams.Id;
                //_teams.Teamcode = teams.Teamcode;
                //_teams.Teamname = teams.Teamname;
                _teams.Sectcode = teams.Sectcode;
                _teams.Sectname = teams.Sectname;
                _teams.Costcentercode = teams.Costcentercode;
                _teams.Reportsto = teams.Reportsto;
                _teams.Costcentername = teams.Costcentername;
                _teams.Fromdate = teams.Fromdate;
                _teams.Toodate = teams.Toodate;
                _teams.ERNAM = teams.ERNAM;
                _teams.AENAM = User.Identity.Name;
                _teams.ERDAT = teams.ERDAT;
                _teams.AEDAT = DateTime.Now;
                var result = await _teams.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: General/Sections/Delete/5
        public async Task<JsonResult> Delete(long? id)
        {
            if (id != 0)
            {
                var team = await _context.TeamLeader.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _teams.Id = team.Id;
                var result = await _teams.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] Sections enteredFiles)
        {
            if (enteredFiles.Files != null)
            {
                using (StreamReader reader1 = new StreamReader(enteredFiles.Files.OpenReadStream()))//, Encoding.UTF8
                {
                    var data = reader1.ReadToEndAsync();
                    string[] strings = data.Result.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);
                    var vals = 0;
                    var teamLeaders = new List<TeamLeader>();
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
                                teamLeaders.Add(new TeamLeader()
                                {
                                    //Teamcode = values[0],
                                    //Teamname = values[1],
                                    Sectcode = values[0],
                                    Sectname = values[1],
                                    Costcentercode = values[2],
                                    Costcentername = values[3],
                                    Fromdate = Convert.ToDateTime(values[4]),
                                    Toodate = Convert.ToDateTime(values[5]),
                                    Reportsto = values[6],
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        vals++;
                    }
                    if (teamLeaders.Count > 0)
                    {
                        var result = await _teams.SaveList(teamLeaders) as DatabaseOperationResponse;
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
        public async Task<IActionResult> Getsections(string Sectcode)
        {
            if (string.IsNullOrEmpty(Sectcode))
            {
                return NotFound();
            }
            var sect = await _context.Sections.Where(con => con.Sectcode == Sectcode).FirstOrDefaultAsync().ConfigureAwait(false);

            if (sect == null)
            {
                return NotFound();
            }
            else
            {
                sect._dbContext = null;
            }
            return new JsonResult(sect);
        }
    }
}

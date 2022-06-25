using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.CoachingTable;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.CoachingMaster.Controllers
{
    [Area("CoachingMaster")]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.CoachingTable.Items _items;
        private readonly UserManager<ApplicationUser> _userManager;
        public ItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _items = new DataObjects.Models.CoachingTable.Items(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(await _context.Items.ToListAsync().ConfigureAwait(false));
        }
        // GET: CoachingMaster/Items/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Items.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }

            return View(costcnt);
        }
        // GET: CoachingMaster/Items/Create
        public IActionResult Create()
        {
            ViewData["Pltype"] = _context.Types.Select(L => new SelectListItem//Category
            {
                Text = L.Pllevel,
                Value = L.Pllevel,
            }).Distinct();
            ViewData["Taskcode"] = _context.Taskcode.Where(c => c.Status == "Y").Select(L => new SelectListItem//Task Code
            {
                Text = L.Keyword,
                Value = L.Keyword,
            }).Distinct();
            ViewData["Plcode"] = _context.Types.Where(c => c.Status == "Y").Select(L => new SelectListItem//Task Code
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            ViewData["Refnumber"] = _context.ATAChapters.Where(c => c.Status == "Y").Select(L => new SelectListItem//Task Code
            {
                Text = L.Chapter_Title,
                Value = L.ATA_Chapter,
            }).Distinct();
            var user = User.Identity.Name;
            if (user != null)
            {
                var getemp = _context.Employees.Where(c => c.EmployeeId == user && c.Status == "Y" && c.Status1 == "Y").FirstOrDefault();
                if (getemp != null)
                {
                    var getdept = _context.Departments.Where(c => c.Costcentercode == getemp.Costcentercode && c.Divisions == "MRO").FirstOrDefault();
                    if (getdept != null)
                    {
                        ViewData["Forreference"] = _context.Departments.Where(c => c.Costcentercode == getemp.Costcentercode && c.Divisions == "MRO").Select(L => new SelectListItem
                        {
                            Text = L.Deptcode,
                            Value = L.Deptcode,
                        }).Distinct();
                    }
                }
            }
            else
            {
                if (user == "admin@MRO.com")
                {
                    ViewData["Forreference"] = _context.Departments.Select(L => new SelectListItem
                    {
                        Text = L.Deptcode,
                        Value = L.Deptcode,
                    }).Distinct();
                }
            }
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Items items)
        {
            if (ModelState.IsValid)
            {
                _items.Refnumber = items.Refnumber;
                _items.Plcode = items.Plcode;
                _items.Taskcode = items.Taskcode;
                _items.Pltypes = items.Pltypes;
                _items.Assesment = items.Assesment;
                _items.Status = items.Status;
                _items.Description = items.Description;
                _items.ERNAM = User.Identity.Name;
                _items.AENAM = User.Identity.Name;
                _items.ERDAT = DateTime.Now;
                _items.AEDAT = DateTime.Now;
                if (_items.Plcode != null && _items.Taskcode != null)
                {


                    var getmax = _context.Items.AsNoTracking().Where(c => c.Plcode == _items.Plcode && c.Taskcode == _items.Taskcode).Select(c => c.Taskcount).ToList();
                    if (getmax.Count == 0)
                    {
                        _items.Taskcount = 1;
                    }
                    else
                    {
                        var getMax = getmax.Max();
                        _items.Taskcount = getMax + 1;
                    }


                }
                var result = await _items.Save().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
                //var exist = await _items.Exist().ConfigureAwait(false);
                //if (!exist)
                //{
                //    var result = await _items.Save().ConfigureAwait(false) as DatabaseOperationResponse;
                //    return new JsonResult(result);
                //}
                //return new JsonResult(new DatabaseOperationResponse
                //{
                //    Status = OperationStatus.Exist,
                //    Message = "Record Already Exists"
                //});
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: CoachingMaster/Items/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Category"] = _context.Categories.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Pltypes,
                Value = L.Pltypes,
            }).Distinct();
            ViewData["Taskcode"] = _context.Taskcode.Where(c => c.Status == "Y").Select(L => new SelectListItem//Task Code
            {
                Text = L.Keyword,
                Value = L.Keyword,
            }).Distinct();
            ViewData["Plcode"] = _context.Types.Where(c => c.Status == "Y").Select(L => new SelectListItem//Task Code
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            ViewData["Refnumber"] = _context.ATAChapters.Where(c => c.Status == "Y").Select(L => new SelectListItem//Task Code
            {
                Text = L.Chapter_Title,
                Value = L.ATA_Chapter,
            }).Distinct();

            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Items.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: General/Costcenters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Items items)
        {
            if (ModelState.IsValid)
            {
                _items.Id = items.Id;
                _items.Refnumber = items.Refnumber;
                _items.Taskcode = items.Taskcode;
                _items.Plcode = items.Plcode;
                _items.Pltypes = items.Pltypes;
                _items.Taskcount = items.Taskcount;
                _items.Description = items.Description;
                _items.ERNAM = items.ERNAM;
                _items.AENAM = User.Identity.Name;
                _items.ERDAT = items.ERDAT;
                _items.AEDAT = DateTime.Now;
                var result = await _items.Update().ConfigureAwait(false) as DatabaseOperationResponse;
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
                var item = await _context.Items.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _items.Id = item.Id;
                var result = await item.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        public IActionResult Gettask(string Taskcode)
        {
            //var getcoach = new List<Coachannualplans>();
            if (string.IsNullOrEmpty(Taskcode))
            {
                return NotFound();
            }
            //if (!string.IsNullOrEmpty(Taskcode))
            //{
            var gettask = _context.Taskcode.Where(con => con.Keyword == Taskcode).FirstOrDefault();
            //}
            //ViewBag.getcoach = getcoach;
            if (gettask != null)
            {
                gettask._dbContext = null;
            }
            return new JsonResult(gettask);
            //return View(gettask);
        }
        public IActionResult Getposition(string Plcode)
        {
            if (string.IsNullOrEmpty(Plcode))
            {
                return NotFound();
            }
            var getposition = _context.Positions.Where(con => con.Postid == Plcode).FirstOrDefault();
            if (getposition != null)
            {
                getposition._dbContext = null;
            }
            return new JsonResult(getposition);
        }

        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] Items enteredFiles)
        {
            if (enteredFiles.Files != null)
            {
                using (StreamReader reader1 = new StreamReader(enteredFiles.Files.OpenReadStream()))
                {
                    var data = reader1.ReadToEndAsync();
                    string[] strings = data.Result.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);
                    var a = 0;
                    var task = "";
                    var pl = "";
                    long taskcnt = 0;
                    var items = new List<Items>();
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

                                if (values[1] != null && values[3] != null)
                                {
                                    if ((task == "" && pl == "") || (task != values[1]))
                                    {
                                        taskcnt = 0;
                                        var getmax = _context.Items.AsNoTracking().Where(c => c.Plcode == values[1] && c.Taskcode == values[3]).Select(c => c.Taskcount).ToList();
                                        if (getmax.Count == 0)
                                        {
                                            taskcnt = 1;
                                        }
                                        else
                                        {
                                            var getMax = getmax.Max();
                                            taskcnt = getMax + 1;
                                        }
                                        task = values[1];
                                        pl = values[3];
                                    }
                                    else
                                    {
                                        taskcnt = taskcnt + 1;
                                        task = values[1];
                                        pl = values[3];
                                    }

                                }
                                items.Add(new Items()
                                {
                                    Refnumber = values[0],
                                    Taskcode = values[1],
                                    Description = values[2],
                                    Plcode = values[3],
                                    Pltypes = values[4],
                                    Status = "Y",
                                    Assesment = "N",
                                    Taskcount = taskcnt,
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        a++;
                    }
                    if (items.Count > 0)
                    {
                        var result = await _items.SaveList(items) as DatabaseOperationResponse;
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
        public async Task<IActionResult> Getatachapter(string ATA_Chapter)
        {
            var Getata = new List<ATAChapters>();
            if (ATA_Chapter == null)
            {
                return NotFound();
            }
            //var getata = await _context.ATAChapters.FindAsync(ATA_Chapter).ConfigureAwait(false);
            Getata = _context.ATAChapters.Where(c => c.ATA_Chapter == ATA_Chapter).ToList();
            if (Getata.Count == 0)
            {
                return NotFound();
            }
            return View(Getata);
        }
    }
}

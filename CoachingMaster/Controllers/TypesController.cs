using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin, Manager, HR, Coach")]
    public class TypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.CoachingTable.Types _types;
        private readonly DataObjects.Models.CoachingTable.Items _items;
        private readonly UserManager<ApplicationUser> _userManager;
        public TypesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _types = new DataObjects.Models.CoachingTable.Types(context);
            _items = new DataObjects.Models.CoachingTable.Items(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(await _context.Types.ToListAsync().ConfigureAwait(false));
        }
        // GET: CoachingingMaster/Types/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Types.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }

            return View(costcnt);
        }
        // GET: AssessingMaster/Types/Create
        public IActionResult Create()
        {
            ViewData["Plname"] = _context.Positions.Select(L => new SelectListItem//Department Code
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();
            ViewData["Costcenter"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();

            return PartialView();
        }
        [HttpGet]
        public IActionResult CreateAssasement()
        {
            //ViewData["Plcode"] = _context.Types.Select(L => new SelectListItem//Cost Center Code
            //{
            //    Text = L.Pldiscreption,
            //    Value = L.Plcode,
            //}).Distinct();
            ViewData["Plname"] = _context.Types.Where(c => c.Status == "Y").Select(L => new SelectListItem//Task Code
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            ViewData["Taskcode"] = _context.Taskcode.Where(c => c.Status == "Y").Select(L => new SelectListItem//Task Code
            {
                Text = L.Keyword,
                Value = L.Keyword,
            }).Distinct();
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssasement(List<Items> items)
        {
            var items1 = new List<Items>();
            var items2 = new List<Items>();
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    items1 = _context.Items.AsNoTracking().Where(c => c.Plcode == item.Plcode && c.Taskcode == item.Taskcode).ToList();
                    if (items1.Count > 0)
                    {
                        foreach (var val in items1)
                        {
                            items2.Add(new Items()
                            {
                                Assesment = "Y",
                                Plcode = val.Plcode,
                                Taskcode = val.Taskcode,
                                AEDAT = DateTime.Now,
                                AENAM = User.Identity.Name,
                            });
                        }
                    }
                }

                if (items2.Count > 0)
                {
                    var result = await _types.Update1(items2).ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }
            }
            return View();
            //return new JsonResult(new DatabaseOperationResponse
            //{
            //    Status = OperationStatus.NOT_OK,
            //    ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            //});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Types types)
        {
            if (ModelState.IsValid)
            {

                _types.Plcode = types.Plcode;
                _types.Pldiscreption = types.Pldiscreption;
                _types.Model = types.Model;
                _types.Costcenter = types.Costcenter;
                _types.Plversion = types.Plversion;
                _types.Pllevel = types.Pllevel;
                _types.Revisiondate = types.Revisiondate;
                _types.Effectiveto = types.Effectiveto;
                _types.Originaldate = types.Originaldate;
                _types.Documentno = types.Documentno;
                _types.Status = "Y";// (types.checkStatus == true) ? "Y" : "N";
                _types.ERNAM = User.Identity.Name;
                _types.AENAM = User.Identity.Name;
                _types.ERDAT = DateTime.Now;
                _types.AEDAT = DateTime.Now;
                var exist = await _types.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    if (types.Originaldate >= types.Effectiveto)
                    {
                        return new JsonResult(new DatabaseOperationResponse
                        {
                            Status = OperationStatus.ERROR,
                            Message = "Original Date Must Be Less Than From Effective To !"
                        });
                    }
                    else
                    {
                        var result = await _types.Save().ConfigureAwait(false) as DatabaseOperationResponse;
                        return new JsonResult(result);
                    }
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
        // GET: CoachingingMaster/Types/Edit/5
        public async Task<IActionResult> Edit(long id)
        {

            ViewData["Plname"] = _context.Positions.Select(L => new SelectListItem//Department Code
            {
                Text = L.Postid,
                Value = L.Postid,
            }).Distinct();
            ViewData["Costcenter"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();

            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Types.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: CoachingingMaster/Types/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Types types)
        {
            if (ModelState.IsValid)
            {
                _types.Id = types.Id;
                _types.Plcode = types.Plcode;
                _types.Pldiscreption = types.Pldiscreption;
                _types.Model = types.Model;
                _types.Costcenter = types.Costcenter;
                _types.Plversion = types.Plversion;
                _types.Pllevel = types.Pllevel;
                _types.Revisiondate = types.Revisiondate;
                _types.Effectiveto = types.Effectiveto;
                _types.Originaldate = types.Originaldate;
                _types.Documentno = types.Documentno;
                _types.Status = (types.checkStatus == true) ? "Y" : "N";
                _types.ERNAM = types.ERNAM;
                _types.AENAM = User.Identity.Name;
                _types.ERDAT = types.ERDAT;
                _types.AEDAT = DateTime.Now;

                if (types.Originaldate >= types.Effectiveto)
                {
                    return new JsonResult(new DatabaseOperationResponse
                    {
                        Status = OperationStatus.ERROR,
                        Message = "Original Date Must Be Less Than From Effective To !"
                    });
                }
                else
                {
                    var result = await _types.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                    return new JsonResult(result);
                }

            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload([FromForm] Types enteredFiles)
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
                    var types = new List<Types>();
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
                                types.Add(new Types()
                                {
                                    Plcode = values[0],
                                    Pldiscreption = values[1],
                                    Model = values[2],
                                    Costcenter = values[3],
                                    Plversion = values[4],
                                    Revisiondate = Convert.ToDateTime(values[5]),
                                    Effectiveto = Convert.ToDateTime(values[6]),
                                    Originaldate = Convert.ToDateTime(values[7]),
                                    Documentno = values[8],
                                    Pllevel = values[9],
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
                    if (types.Count > 0)
                    {
                        var result = await _types.SaveList(types) as DatabaseOperationResponse;
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
        // GET: CoachingingMaster/Categories/Delete/5

        [HttpGet]
        public async Task<JsonResult> Delete(long id)
        {
            if (id != 0)
            {
                var types = await _context.Types.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _types.Id = types.Id;
                var result = await types.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
                //return RedirectToAction(nameof(Index));
            }
            return new JsonResult(null);
        }
        public IActionResult Getpldesc(string Plcode)
        {
            if (string.IsNullOrEmpty(Plcode))
            {
                return NotFound();
            }
            var getposition = _context.Types.Where(con => con.Plcode == Plcode).FirstOrDefault();
            if (getposition != null)
            {
                getposition._dbContext = null;
            }
            return new JsonResult(getposition);
        }
    }
}

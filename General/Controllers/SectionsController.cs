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

namespace MROCoatching.Web.Areas.Master.General.Controllers
{
    [Area("General")]
    public class SectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.General.Sections _sections;
        private readonly UserManager<ApplicationUser> _userManager;
        public SectionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _sections = new DataObjects.Models.General.Sections(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Sections.ToListAsync().ConfigureAwait(false));
        }
        // GET: General/Sections/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sections = await _context.Sections.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (sections == null)
            {
                return NotFound();
            }

            return View(sections);
        }
        // GET: General/Sections/Create
        public IActionResult Create()
        {
            ViewData["Costcentercode"] = _context.Costcenters.Where(con => con.Status == "Y").Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            ViewData["Deptcode"] = _context.Departments.Select(L => new SelectListItem//Department Code
            {
                Text = L.Deptcode,
                Value = L.Deptcode,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sections sections)
        {
            if (ModelState.IsValid)
            {
                _sections.Sectcode = sections.Sectcode;
                _sections.Sectname = sections.Sectname;
                _sections.Costcentercode = sections.Costcentercode;
                _sections.Reportsto = sections.Reportsto;
                _sections.Costcentername = sections.Costcentername;
                _sections.Divisiondept = sections.Divisiondept;
                _sections.Deptcode = sections.Deptcode;
                _sections.Deptname = sections.Deptname;
                if (sections.Subsec != null)
                {
                    _sections.Subsec = sections.Subsec;
                }
                else
                {
                    _sections.Subsec = "N";
                }

                _sections.Fromdate = sections.Fromdate;
                _sections.Toodate = sections.Toodate;
                _sections.ERNAM = User.Identity.Name;
                _sections.AENAM = User.Identity.Name;
                _sections.ERDAT = DateTime.Now;
                _sections.AEDAT = DateTime.Now;
                var exist = await _sections.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _sections.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: General/Sections/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Costcentercode"] = _context.Costcenters.Where(con => con.Status == "Y").Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
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

            var costcnt = await _context.Sections.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: General/Sections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Sections sections)
        {
            if (ModelState.IsValid)
            {
                _sections.Id = sections.Id;
                _sections.Sectcode = sections.Sectcode;
                _sections.Sectname = sections.Sectname;
                _sections.Costcentercode = sections.Costcentercode;
                _sections.Reportsto = sections.Reportsto;
                _sections.Costcentername = sections.Costcentername;
                _sections.Divisiondept = sections.Divisiondept;
                _sections.Deptcode = sections.Deptcode;
                _sections.Deptname = sections.Deptname;
                _sections.Subsec = sections.Subsec;
                _sections.Fromdate = sections.Fromdate;
                _sections.Toodate = sections.Toodate;
                _sections.ERNAM = sections.ERNAM;
                _sections.AENAM = User.Identity.Name;
                _sections.ERDAT = sections.ERDAT;
                _sections.AEDAT = DateTime.Now;
                var result = await _sections.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: General/Sections/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var sect = await _context.Sections.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _sections.Id = sect.Id;
                var result = await _sections.Delete() as DatabaseOperationResponse;
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
                    var sections = new List<Sections>();
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
                                sections.Add(new Sections()
                                {
                                    Sectcode = values[0],
                                    Sectname = values[1],
                                    Costcentercode = values[2],
                                    Costcentername = values[3],
                                    Divisiondept = values[4],
                                    Deptcode = values[5],
                                    Deptname = values[6],
                                    Fromdate = Convert.ToDateTime(values[7]),
                                    Toodate = Convert.ToDateTime(values[8]),
                                    Reportsto = values[9],
                                    Subsec = "Y",
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        vals++;
                    }
                    if (sections.Count > 0)
                    {
                        var result = await _sections.SaveList(sections) as DatabaseOperationResponse;
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

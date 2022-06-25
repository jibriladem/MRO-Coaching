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
    public class PositionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.General.Positions _position;
        private readonly UserManager<ApplicationUser> _userManager;
        public PositionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _position = new DataObjects.Models.General.Positions(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View(await _context.Positions.ToListAsync().ConfigureAwait(false));
        }
        // GET: General/Positions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Positions.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }

            return View(costcnt);
        }
        // GET: General/Positions/Create
        public IActionResult Create()
        {
            ViewData["Costcentercode"] = _context.Costcenters.Where(s => s.Status == "Y" && s.Toodate > DateTime.Now).Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Positions positions)
        {
            if (ModelState.IsValid)
            {
                _position.Postid = positions.Postid;
                _position.Postname = positions.Postname;
                _position.Pllevel = positions.Pllevel;
                _position.Costcentercode = positions.Costcentercode;
                _position.Costcentername = positions.Costcentername;
                _position.Fromdate = positions.Fromdate;
                _position.Toodate = positions.Toodate;
                _position.ERNAM = User.Identity.Name;
                _position.AENAM = User.Identity.Name;
                _position.ERDAT = DateTime.Now;
                _position.AEDAT = DateTime.Now;
                if (_position.Postid != null)
                {
                    var getposn = _context.Positions.Where(c => c.Costcentercode == _position.Costcentercode).FirstOrDefault();
                    if (getposn != null)
                    {
                        _position.Positionnbr = getposn.Positionnbr + 1;
                    }
                    else
                    {
                        _position.Positionnbr = 1;
                    }
                }
                var exist = await _position.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _position.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: General/Positions/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Costcentercode"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Positions.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: General/Positions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Positions positions)
        {
            if (ModelState.IsValid)
            {
                _position.Id = positions.Id;
                _position.Postid = positions.Postid;
                _position.Positionnbr = positions.Positionnbr;
                _position.Postname = positions.Postname;
                _position.Pllevel = positions.Pllevel;
                _position.Costcentercode = positions.Costcentercode;
                _position.Costcentername = positions.Costcentername;
                _position.Fromdate = positions.Fromdate;
                _position.Toodate = positions.Toodate;
                _position.ERNAM = positions.ERNAM;
                _position.AENAM = User.Identity.Name;
                _position.ERDAT = positions.ERDAT;
                _position.AEDAT = DateTime.Now;
                var result = await _position.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: General/Positions/Delete/5
        public async Task<JsonResult> Delete(long? id)
        {
            if (id != 0)
            {
                var account = await _context.Positions.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _position.Id = account.Id;
                var result = await _position.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] Positions enteredFiles)
        {
            if (enteredFiles.Files != null)
            {
                using (StreamReader reader1 = new StreamReader(enteredFiles.Files.OpenReadStream()))//, Encoding.UTF8
                {
                    var data = reader1.ReadToEndAsync();
                    string[] strings = data.Result.Split(Environment.NewLine,
                            StringSplitOptions.RemoveEmptyEntries);
                    var vals = 0;
                    var positions = new List<Positions>();
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
                                positions.Add(new Positions()
                                {
                                    Postid = values[0],
                                    Postname = values[1],
                                    Pllevel = values[2],
                                    Costcentercode = values[3],
                                    Costcentername = values[4],
                                    Status = "Y",
                                    Fromdate = Convert.ToDateTime(values[5]),
                                    Toodate = Convert.ToDateTime(values[6]),
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        vals++;
                    }
                    if (positions.Count > 0)
                    {
                        var result = await _position.SaveList(positions) as DatabaseOperationResponse;
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
        public async Task<IActionResult> GetPositions(string PositionId)
        {
            if (string.IsNullOrEmpty(PositionId))
            {
                return NotFound();
            }
            var post = await _context.Positions.Where(con => con.Postid == PositionId).FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }
            else
            {
                post._dbContext = null;
            }
            return new JsonResult(post);
        }
    }
}

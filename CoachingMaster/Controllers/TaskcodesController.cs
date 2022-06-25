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

namespace MROCoatching.Web.Areas.CoachingMaster.Controllers
{
    [Area("CoachingMaster")]
    public class TaskcodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.General.Taskcode _taskcode;
        private readonly UserManager<ApplicationUser> _userManager;
        public TaskcodesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _taskcode = new DataObjects.Models.General.Taskcode(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(await _context.Taskcode.ToListAsync().ConfigureAwait(false));
        }
        // GET: CoachingMaster/Taskcode/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskgrp = await _context.Taskcode.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (taskgrp == null)
            {
                return NotFound();
            }

            return View(taskgrp);
        }
        public IActionResult Create()
        {
            ViewData["Costcenter"] = _context.Costcenters.Select(L => new SelectListItem//Cost Center Code
            {
                Text = L.Costcentercode,
                Value = L.Costcentercode,
            }).Distinct();

            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Taskcode taskcode)
        {
            if (ModelState.IsValid)
            {
                _taskcode.Keyword = taskcode.Keyword;
                _taskcode.Descriptions = taskcode.Descriptions;
                _taskcode.Divisions = "MRO";
                _taskcode.Status = "Y";
                _taskcode.ERNAM = User.Identity.Name;
                _taskcode.ERDAT = DateTime.Now;
                _taskcode.AEDAT = DateTime.Now;
                _taskcode.AENAM = User.Identity.Name;
                var exist = await _taskcode.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _taskcode.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        public async Task<IActionResult> Edit(long id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskcode = await _context.Taskcode.FindAsync(id).ConfigureAwait(false);
            if (taskcode == null)
            {
                return NotFound();
            }
            return View(taskcode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Taskcode taskcode)
        {
            if (ModelState.IsValid)
            {
                _taskcode.Id = taskcode.Id;
                _taskcode.Keyword = taskcode.Keyword;
                _taskcode.Descriptions = taskcode.Descriptions;
                _taskcode.Divisions = "MRO";
                _taskcode.Status = "Y";
                _taskcode.ERNAM = User.Identity.Name;
                _taskcode.ERDAT = DateTime.Now;
                _taskcode.AEDAT = DateTime.Now;
                _taskcode.AENAM = User.Identity.Name;
                var result = await _taskcode.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
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
        public async Task<IActionResult> Upload([FromForm] Taskcode enteredFiles)
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
                    var taskcode = new List<Taskcode>();
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
                                taskcode.Add(new Taskcode()
                                {
                                    Keyword = values[0],
                                    Descriptions = values[1],
                                    Divisions = "MRO",
                                    ERDAT = DateTime.Now,
                                    ERNAM = User.Identity.Name,
                                    AEDAT = DateTime.Now,
                                    AENAM = User.Identity.Name,
                                });
                            }
                        }
                        a++;
                    }
                    if (taskcode.Count > 0)
                    {
                        var result = await _taskcode.SaveList(taskcode) as DatabaseOperationResponse;
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
        [HttpGet]
        public async Task<JsonResult> Delete(long id)
        {
            if (id != 0)
            {
                var taskcode = await _context.Taskcode.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _taskcode.Id = taskcode.Id;
                var result = await taskcode.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        public async Task<IActionResult> GetTaskcode(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return NotFound();
            }
            var taskcode = await _context.Taskcode.Where(con => con.Keyword == keyword).FirstOrDefaultAsync().ConfigureAwait(true);

            if (taskcode == null)
            {
                return NotFound();
            }
            else
            {
                taskcode._dbContext = null;
            }
            return new JsonResult(taskcode);
        }
    }
}

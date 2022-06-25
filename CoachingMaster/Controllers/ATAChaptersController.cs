using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class ATAChaptersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.CoachingTable.ATAChapters _taskgroup;
        private readonly UserManager<ApplicationUser> _userManager;
        public ATAChaptersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _taskgroup = new DataObjects.Models.CoachingTable.ATAChapters(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(await _context.ATAChapters.ToListAsync().ConfigureAwait(false));
        }
        // GET: CoachingMaster/Taskgroup/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskgrp = await _context.ATAChapters.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (taskgrp == null)
            {
                return NotFound();
            }

            return View(taskgrp);
        }
        public IActionResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ATAChapters taskgroup)
        {
            if (ModelState.IsValid)
            {
                _taskgroup.ATA_Chapter = taskgroup.ATA_Chapter;
                _taskgroup.Chapter_Title = taskgroup.Chapter_Title;
                _taskgroup.Taskdiv = "MRO";
                _taskgroup.Status = "Y";
                _taskgroup.ERNAM = User.Identity.Name;
                _taskgroup.ERDAT = DateTime.Now;
                _taskgroup.AENAM = User.Identity.Name;
                _taskgroup.AEDAT = DateTime.Now;
                var exist = await _taskgroup.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _taskgroup.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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

            var taskgrp = await _context.ATAChapters.FindAsync(id).ConfigureAwait(false);
            if (taskgrp == null)
            {
                return NotFound();
            }
            return View(taskgrp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (long id, ATAChapters taskgroup)
        {
            if (ModelState.IsValid)
            {
                _taskgroup.Id = taskgroup.Id;
                _taskgroup.ATA_Chapter = taskgroup.ATA_Chapter;
                _taskgroup.Chapter_Title = taskgroup.Chapter_Title;
                _taskgroup.Taskdiv = "MRO";
                _taskgroup.Status = "Y";
                _taskgroup.ERNAM = taskgroup.ERNAM;
                _taskgroup.ERDAT = taskgroup.ERDAT;
                _taskgroup.AENAM = User.Identity.Name;
                _taskgroup.AEDAT = DateTime.Now;
                var result = await _taskgroup.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var taskgrp = await _context.ATAChapters.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _taskgroup.Id = taskgrp.Id;
                var result = await taskgrp.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }
        [HttpGet]
        public PartialViewResult Upload()
        {
            return PartialView();
        }
        public async Task<IActionResult> Upload([FromForm] ATAChapters enteredFiles)
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
                    var atachapter = new List<ATAChapters>();
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
                                atachapter.Add(new ATAChapters()
                                {
                                    ATA_Chapter = values[0],
                                    Chapter_Title = values[1],
                                    Taskdiv = "MRO",
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
                    if (atachapter.Count > 0)
                    {
                        var result = await _taskgroup.SaveList(atachapter).ConfigureAwait(false) as DatabaseOperationResponse;
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
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AssessingTable;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.AssessingMaster.Controllers
{
    [Area("AssessingMaster")]
    public class QuestionariesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataObjects.Models.AssessingTable.Questionaries _questions;
        private readonly UserManager<ApplicationUser> _userManager;
        public QuestionariesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _questions = new DataObjects.Models.AssessingTable.Questionaries(context);
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(await _context.Questionaries.ToListAsync().ConfigureAwait(false));
        }
        // GET: AssessingMaster/Questionaries/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Questionaries.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }

            return View(costcnt);
        }
        // GET: AssessingMaster/Questionaries/Create
        public IActionResult Create()
        {
            ViewData["Asscatgcode"] = _context.Types.Select(L => new SelectListItem//PL Types
            {
                Text = L.Plcode,
                Value = L.Plcode,
            }).Distinct();
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Questionaries questionaries)
        {
            if (questionaries.Asscatgcode != null && questionaries.Assseqdesc != null)
            {
                var getmaxtyp = 0; var getmaxseq = 0;
                _questions.Asscatgcode = questionaries.Asscatgcode;
                _questions.Assseqdesc = questionaries.Assseqdesc;
                _questions.Assquestions = questionaries.Assquestions;
                _questions.Status = (questionaries.checkStatus == true) ? "Y" : "N";
                _questions.ERNAM = User.Identity.Name;
                _questions.AENAM = User.Identity.Name;
                _questions.ERDAT = DateTime.Now;
                _questions.AEDAT = DateTime.Now;
                var gettypseq = _context.Questionaries.AsNoTracking().Where(c => c.Asscatgcode == _questions.Asscatgcode && c.Assseqdesc == _questions.Assseqdesc).Select(c => c.Asstypseqnbr).ToList();
                if (gettypseq.Count > 0)
                {
                    getmaxtyp = (int)gettypseq.Max();
                    _questions.Asstypseqnbr = getmaxtyp;
                }
                else
                {
                    gettypseq = _context.Questionaries.AsNoTracking().Select(c => c.Asstypseqnbr).ToList();
                    if (gettypseq.Count > 0)
                    {
                        _questions.Asstypseqnbr = gettypseq.Max() + 1;
                    }
                    else
                    {
                        _questions.Asstypseqnbr = 1;
                    }
                }
                var getqnsseq = _context.Questionaries.AsNoTracking().Where(c => c.Asscatgcode == _questions.Asscatgcode && c.Assseqdesc == _questions.Assseqdesc && c.Asstypseqnbr == _questions.Asstypseqnbr).Select(c => c.Assquesseq).ToList();
                if (getqnsseq.Count > 0)
                {
                    getmaxseq = Convert.ToInt16(value: getqnsseq.Max());
                    _questions.Assquesseq = getmaxseq + 1;
                }
                else
                {
                    _questions.Assquesseq = 1;
                }
                var exist = await _questions.Exist().ConfigureAwait(false);
                if (!exist)
                {
                    var result = await _questions.Save().ConfigureAwait(false) as DatabaseOperationResponse;
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
        // GET: AssessingMaster/Questionaries/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["Qnscategory"] = _context.Assquestionlimits.Select(L => new SelectListItem//PL Types
            {
                Text = L.Assessmenttype,
                Value = L.Assessmenttype,
            }).Distinct();
            if (id == null)
            {
                return NotFound();
            }

            var costcnt = await _context.Questionaries.FindAsync(id).ConfigureAwait(false);
            if (costcnt == null)
            {
                return NotFound();
            }
            return View(costcnt);
        }
        // POST: AssessingMaster/Questionaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Questionaries questionaries)
        {
            if (ModelState.IsValid)
            {
                _questions.Id = questionaries.Id;
                //_questions.Qnscategory = questionaries.Qnscategory;
                //_questions.Questions = questionaries.Questions;
                //_questions.Answers = questionaries.Answers;
                _questions.Status = (questionaries.checkStatus == true) ? "Y" : "N";
                _questions.ERNAM = questionaries.ERNAM;
                _questions.AENAM = User.Identity.Name;
                _questions.ERDAT = questionaries.ERDAT;
                _questions.AEDAT = DateTime.Now;
                var result = await _questions.Update().ConfigureAwait(false) as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(new DatabaseOperationResponse
            {
                Status = OperationStatus.NOT_OK,
                ErrorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage != "" ? e.ErrorMessage : e.Exception.Message).ToList()
            });
        }
        // GET: AssessingMaster/Questionaries/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id != 0)
            {
                var ques = await _context.Questionaries.AsNoTracking().FirstOrDefaultAsync(con => con.Id == id).ConfigureAwait(false);
                _questions.Id = ques.Id;
                var result = await ques.Delete() as DatabaseOperationResponse;
                return new JsonResult(result);
            }
            return new JsonResult(null);
        }

    }
}

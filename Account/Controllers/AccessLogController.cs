using MROCoatching.DataObjects.Data.Context;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;

namespace MROCoatching.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class AccessLogController : Controller
    {
        ApplicationDbContext context;

        public AccessLogController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [DisplayName("AccountMangement")]
        [HttpGet]
        public ActionResult Index()
        {
            var accessLogs = context.AccessLogs.ToList();

            return View(accessLogs);
        }
    }
}
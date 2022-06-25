using MROCoatching.DataObjects.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;

namespace MROCoatching.Web.Areas.Account.Controllers
{
    [Area("Account")]
    [DisplayName("Operational")]
    public class NotificationController : Controller
    {
        ApplicationDbContext context;

        public NotificationController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [DisplayName("Operational")]
        // GET: Notification
        public ActionResult Index()
        {
            var notifications = context.Notifications.Include(n => n.ReceiverUser).Include(n => n.SenderUser).ToList();

            return View(notifications);
        }
    }
}
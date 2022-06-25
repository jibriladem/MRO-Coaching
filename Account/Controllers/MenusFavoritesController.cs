using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.Web.Areas.Account.Controllers
{
    [Area("Account")]
    public class MenusFavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenusFavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/MenusFavorites
        public async Task<IActionResult> Index()
        {
            return View(await _context.MenusFavorite.ToListAsync());
        }

        // GET: Account/MenusFavorites/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menusFavorite = await _context.MenusFavorite
                .FirstOrDefaultAsync(m => m.id == id);
            if (menusFavorite == null)
            {
                return NotFound();
            }

            return View(menusFavorite);
        }

        // GET: Account/MenusFavorites/Create
        public IActionResult Create()
        {
            ViewData["MenuName"] = new SelectList(_context.Menus.Where(s => !string.IsNullOrEmpty(s.Url)), "MenuId", "Name");

            return View();
        }

        // POST: Account/MenusFavorites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenusFavorite menusFavorite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menusFavorite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(menusFavorite);
        }

        // GET: Account/MenusFavorites/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["MenuName"] = new SelectList(_context.Menus, "MenuId", "Name");
            var menusFavorite = await _context.MenusFavorite.FindAsync(id);
            if (menusFavorite == null)
            {
                return NotFound();
            }
            return View(menusFavorite);
        }

        // POST: Account/MenusFavorites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, MenusFavorite menusFavorite)
        {
            if (id != menusFavorite.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menusFavorite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenusFavoriteExists(menusFavorite.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(menusFavorite);
        }

        // GET: Account/MenusFavorites/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menusFavorite = await _context.MenusFavorite
                .FirstOrDefaultAsync(m => m.id == id);
            if (menusFavorite == null)
            {
                return NotFound();
            }

            return View(menusFavorite);
        }

        // POST: Account/MenusFavorites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var menusFavorite = await _context.MenusFavorite.FindAsync(id);
            _context.MenusFavorite.Remove(menusFavorite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenusFavoriteExists(long id)
        {
            return _context.MenusFavorite.Any(e => e.id == id);
        }
    }
}

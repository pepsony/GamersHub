using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GamersHub.Data;
using GamersHub.Models;

namespace GamersHub.Controllers
{
    public class UserGamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserGamesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserGames
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userGames = await _context.UserGames
                .Where(ug => ug.UserId == user.Id)
                .Include(ug => ug.Game)
                .ToListAsync();

            return View(userGames);
        }

        // GET: UserGames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userGame = await _context.UserGames
                .Include(u => u.Game)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

            if (userGame == null)
                return NotFound();

            return View(userGame);
        }

        // GET: UserGames/Create
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Title");
            return View();
        }

        // POST: UserGames/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameId,Status")] UserGame userGame)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var alreadyExists = await _context.UserGames
                .AnyAsync(ug => ug.UserId == user.Id && ug.GameId == userGame.GameId);

            if (alreadyExists)
                ModelState.AddModelError("", "This game is already in your library.");

            if (ModelState.IsValid)
            {
                userGame.UserId = user.Id;
                _context.Add(userGame);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Title", userGame.GameId);
            return View(userGame);
        }

        // GET: UserGames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(ug => ug.Id == id && ug.UserId == user.Id);

            if (userGame == null)
                return NotFound();

            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Title", userGame.GameId);
            return View(userGame);
        }

        // POST: UserGames/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GameId,Status")] UserGame userGame)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var existing = await _context.UserGames
                .FirstOrDefaultAsync(ug => ug.Id == id && ug.UserId == user.Id);

            if (existing == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    existing.GameId = userGame.GameId;
                    existing.Status = userGame.Status;
                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserGameExists(userGame.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Title", userGame.GameId);
            return View(userGame);
        }

        // GET: UserGames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userGame = await _context.UserGames
                .Include(ug => ug.Game)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

            if (userGame == null)
                return NotFound();

            return View(userGame);
        }

        // POST: UserGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(ug => ug.Id == id && ug.UserId == user.Id);

            if (userGame != null)
                _context.UserGames.Remove(userGame);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserGameExists(int id)
        {
            return _context.UserGames.Any(e => e.Id == id);
        }
    }
}

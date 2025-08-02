using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamersHub.Data;
using GamersHub.Models;

namespace GamersHub.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamesController(ApplicationDbContext context)
        {
            _context = context;
        }           

        // GET: Games
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var gamesQuery = _context.Games
                .Include(g => g.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                gamesQuery = gamesQuery.Where(g => g.Title.Contains(searchString));
            }

            var games = await gamesQuery.ToListAsync();
            return View(games);
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,GenreId")] Game game, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    var extension = Path.GetExtension(ImageFile.FileName);
                    var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                    var path = Path.Combine("wwwroot/images/games", newFileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    game.ImagePath = $"/images/games/{newFileName}";
                }

                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", game.GenreId);
            return View(game);
        }


        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", game.GenreId);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,GenreId")] Game game, IFormFile? ImageFile)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            var gameToUpdate = await _context.Games.FindAsync(id);
            if (gameToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    gameToUpdate.Title = game.Title;
                    gameToUpdate.ReleaseDate = game.ReleaseDate;
                    gameToUpdate.GenreId = game.GenreId;

                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                        var extension = Path.GetExtension(ImageFile.FileName);
                        var newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        var path = Path.Combine("wwwroot/images/games", newFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        gameToUpdate.ImagePath = $"/images/games/{newFileName}";
                    }

                    _context.Update(gameToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists((int)game.Id))
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

            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", game.GenreId);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}

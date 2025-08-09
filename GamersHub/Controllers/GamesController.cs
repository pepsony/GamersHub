using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamersHub.Data;
using GamersHub.Models;
using GamersHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GamersHub.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGameService _gameService;
        private readonly ApplicationDbContext _context; // only for genres

        //This is for 500 error test!!!
        public IActionResult ThrowError()
        {
            throw new Exception("Test 500 error page");
        }
        // To check that in the field add: localhost:7234/Games/ThrowError in Production state

        public GamesController(IGameService gameService, ApplicationDbContext context)
        {
            _gameService = gameService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString, int? genreId, int page = 1, int pageSize = 6)
        {
            var genres = await _context.Genres.ToListAsync();
            ViewBag.Genres = new SelectList(genres, "Id", "Name");
            ViewBag.SearchString = searchString;
            ViewBag.SelectedGenre = genreId;

            var (games, totalPages) = await _gameService.GetGamesAsync(searchString, genreId, page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(games);
        }

        public async Task<IActionResult> Details(int id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null) return NotFound();
            return View(game);
        }

        [Authorize]
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Game game, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                await _gameService.CreateAsync(game, imageFile);
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", game.GenreId);
            return View(game);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null) return NotFound();
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", game.GenreId);
            return View(game);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Game game, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                await _gameService.UpdateAsync(id, game, imageFile);
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", game.GenreId);
            return View(game);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null) return NotFound();
            return View(game);
        }

        [Authorize, HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _gameService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}

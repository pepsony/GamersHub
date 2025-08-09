using System.Threading.Tasks;
using GamersHub.Data;
using GamersHub.Models;
using GamersHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GamersHub.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        // GET: Reviews
        public async Task<IActionResult> Index(string? searchString, int? genreId)
        {
            var reviews = await _reviewService.GetAllReviewsAsync(searchString, genreId);

            // Get genres for filter dropdown - for this, still access DbContext via UserManager or inject DbContext if needed
            // But better to inject a service to get genres or use ViewComponent
            // For simplicity, assume genres loaded here from context:
            var context = HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var genres = context != null
                ? await context.Genres.ToListAsync()
                : new List<Genre>();

            ViewBag.Genres = new SelectList(genres, "Id", "Name");
            ViewBag.SearchString = searchString;
            ViewBag.SelectedGenre = genreId;

            return View(reviews);
        }

        // GET: Reviews/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var review = await _reviewService.GetReviewByIdAsync(id.Value);

            if (review == null) return NotFound();

            return View(review);
        }

        // GET: Reviews/Create
        [Authorize]
        public IActionResult Create()
        {
            // Populate games dropdown list
            var games = HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) is ApplicationDbContext ctx
                ? ctx.Games.ToList()
                : new List<Game>();

            ViewData["GameId"] = new SelectList(games, "Id", "Title");
            return View();
        }

        // POST: Reviews/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameId,Content,Rating")] Review review)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (!ModelState.IsValid)
            {
                ViewData["GameId"] = new SelectList(GetGamesForDropdown(), "Id", "Title", review.GameId);
                return View(review);
            }

            bool created = await _reviewService.CreateReviewAsync(review, userId);

            if (!created)
            {
                ModelState.AddModelError("", "You have already reviewed this game.");
                ViewData["GameId"] = new SelectList(GetGamesForDropdown(), "Id", "Title", review.GameId);
                return View(review);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var review = await _reviewService.GetReviewByIdAsync(id.Value);
            if (review == null) return NotFound();

            return View(review);
        }

        // POST: Reviews/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,GameId,Content,Rating,CreatedAt")] Review review)
        {
            if (id != review.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["GameId"] = new SelectList(GetGamesForDropdown(), "Id", "Title", review.GameId);
                ViewData["UserId"] = new SelectList(GetUsersForDropdown(), "Id", "UserName", review.UserId);
                return View(review);
            }

            var updated = await _reviewService.UpdateReviewAsync(review);
            if (!updated) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var review = await _reviewService.GetReviewByIdAsync(id.Value);
            if (review == null) return NotFound();

            return View(review);
        }

        // POST: Reviews/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Helpers to get dropdown data
        private IEnumerable<Game> GetGamesForDropdown()
        {
            return HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) is ApplicationDbContext ctx
                ? ctx.Games.ToList()
                : new List<Game>();
        }

        private IEnumerable<ApplicationUser> GetUsersForDropdown()
        {
            return HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) is ApplicationDbContext ctx
                ? ctx.Users.ToList()
                : new List<ApplicationUser>();
        }
    }
}

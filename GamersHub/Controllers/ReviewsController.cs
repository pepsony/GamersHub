using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamersHub.Data;
using GamersHub.Models;
using Microsoft.AspNetCore.Identity;

namespace GamersHub.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {   

            var reviews = _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.User);

            return View(await reviews.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            return review == null ? NotFound() : View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Title");
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GameId,Content,Rating")] Review review)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Ensure the user is authenticated
            }

            // Check for duplicate review
            bool alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.GameId == review.GameId);

            if (alreadyReviewed)
            {
                ModelState.AddModelError("", "You have already reviewed this game.");
            }

            if (ModelState.IsValid)
            {
                review.UserId = userId;
                review.CreatedAt = DateTime.UtcNow;

                _context.Add(review);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Something went wrong while saving the review.");
            }

            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Title", review.GameId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Game)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound();

            // No longer need these selectLists for our read-only fields
            //ViewData["GameId"] = new SelectList(_context.Games, "Id", "Title", review.GameId);
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", review.UserId);
            
            return View(review);
        }

       

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,GameId,Content,Rating,CreatedAt")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["GameId"] = new SelectList(_context.Games, "Id", "Title", review.GameId);
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", review.UserId);
                return View(review);
            }

            try
            {
                var existingReview = await _context.Reviews.FindAsync(id);
                if (existingReview == null)
                {
                    return NotFound();
                }

                // Update all relevant properties
                existingReview.UserId = review.UserId;
                existingReview.GameId = review.GameId;
                existingReview.Content = review.Content;
                existingReview.Rating = review.Rating;
                existingReview.CreatedAt = review.CreatedAt;

                _context.Update(existingReview);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(review.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.Game)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            return review == null ? NotFound() : View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}

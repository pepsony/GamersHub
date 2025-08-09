using System.Threading.Tasks;
using GamersHub.Models;
using GamersHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamersHub.Controllers
{
    public class PlatformsController : Controller
    {
        private readonly IPlatformService _platformService;

        public PlatformsController(IPlatformService platformService)
        {
            _platformService = platformService;
        }

        // GET: Platforms
        public async Task<IActionResult> Index()
        {
            var platforms = await _platformService.GetAllAsync();
            return View(platforms);
        }

        // GET: Platforms/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var platform = await _platformService.GetByIdAsync(id.Value);
            if (platform == null) return NotFound();

            return View(platform);
        }

        // GET: Platforms/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Platforms/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Platform platform)
        {
            if (ModelState.IsValid)
            {
                await _platformService.CreateAsync(platform);
                return RedirectToAction(nameof(Index));
            }
            return View(platform);
        }

        // GET: Platforms/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var platform = await _platformService.GetByIdAsync(id.Value);
            if (platform == null) return NotFound();

            return View(platform);
        }

        // POST: Platforms/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Platform platform)
        {
            if (id != platform.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _platformService.UpdateAsync(platform);
                }
                catch
                {
                    if (!await _platformService.ExistsAsync(platform.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(platform);
        }

        // GET: Platforms/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var platform = await _platformService.GetByIdAsync(id.Value);
            if (platform == null) return NotFound();

            return View(platform);
        }

        // POST: Platforms/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _platformService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

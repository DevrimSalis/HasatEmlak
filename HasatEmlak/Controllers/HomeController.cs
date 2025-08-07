// Controllers/HomeController.cs
using HasatEmlak.Data;
using HasatEmlak.Models;
using HasatEmlak.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HasatEmlak.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Öne çýkan ilanlarý getir
            var featuredProperties = await _context.Properties
                .Include(p => p.PropertyImages)
                .Include(p => p.Category)
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedDate)
                .Take(6)
                .ToListAsync();

            // En son ilanlarý getir
            var latestProperties = await _context.Properties
                .Include(p => p.PropertyImages)
                .Include(p => p.Category)
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedDate)
                .Take(8)
                .ToListAsync();

            // Ýstatistikler
            var stats = new
            {
                TotalProperties = await _context.Properties.CountAsync(p => p.IsActive),
                TotalForSale = await _context.Properties.CountAsync(p => p.IsActive && p.CategoryId == 1),
                TotalForRent = await _context.Properties.CountAsync(p => p.IsActive && p.CategoryId == 2),
                TotalAgents = 1 // Sadece siz varsýnýz
            };

            // Kategoriler
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            ViewBag.FeaturedProperties = featuredProperties;
            ViewBag.LatestProperties = latestProperties;
            ViewBag.Stats = stats;
            ViewBag.Categories = categories;

            return View();
        }

        public async Task<IActionResult> About()
        {
            var stats = new
            {
                TotalProperties = await _context.Properties.CountAsync(p => p.IsActive),
                TotalAgents = 1, // Sadece siz varsýnýz
                TotalCities = await _context.Cities.CountAsync(c => c.IsActive),
                YearsOfExperience = DateTime.Now.Year - 2020 // Kuruluþ yýlýnýza göre ayarlayýn
            };

            ViewBag.Stats = stats;
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactRequest model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.IsRead = false;

                _context.ContactRequests.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Mesajýnýz baþarýyla gönderildi. En kýsa sürede size geri dönüþ yapacaðýz.";
                return RedirectToAction("Contact");
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
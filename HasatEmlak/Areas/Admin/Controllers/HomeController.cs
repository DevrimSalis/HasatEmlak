// Areas/Admin/Controllers/HomeController.cs
using HasatEmlak.Data;
using HasatEmlak.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new AdminDashboardViewModel
            {
                TotalProperties = await _context.Properties.CountAsync(),
                ActiveProperties = await _context.Properties.CountAsync(p => p.IsActive),
                TotalAgents = 1, // Sadece siz
                TotalUsers = 1, // Sadece siz
                UnreadMessages = await _context.ContactRequests.CountAsync(c => !c.IsRead),
                PropertiesThisMonth = await _context.Properties
                    .CountAsync(p => p.CreatedDate.Month == DateTime.Now.Month && p.CreatedDate.Year == DateTime.Now.Year),

                TotalSalesValue = await _context.Properties
                    .Where(p => p.IsActive && p.CategoryId == 1) // Satılık
                    .SumAsync(p => p.Price),

                AveragePropertyPrice = await _context.Properties
                    .Where(p => p.IsActive)
                    .AverageAsync(p => p.Price)
            };

            // Son 6 aylık istatistikler
            var monthlyStats = new List<MonthlyStats>();
            for (int i = 5; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var count = await _context.Properties
                    .CountAsync(p => p.CreatedDate.Month == date.Month && p.CreatedDate.Year == date.Year);

                var totalValue = await _context.Properties
                    .Where(p => p.CreatedDate.Month == date.Month && p.CreatedDate.Year == date.Year)
                    .SumAsync(p => p.Price);

                monthlyStats.Add(new MonthlyStats
                {
                    Month = date.ToString("MMM yyyy"),
                    PropertyCount = count,
                    TotalValue = totalValue
                });
            }
            dashboardData.MonthlyStats = monthlyStats;

            // Kategori istatistikleri
            var categories = await _context.Categories.ToListAsync();
            var categoryStats = new List<CategoryStats>();
            var totalProps = await _context.Properties.CountAsync(p => p.IsActive);

            foreach (var category in categories)
            {
                var count = await _context.Properties.CountAsync(p => p.IsActive && p.CategoryId == category.Id);
                categoryStats.Add(new CategoryStats
                {
                    CategoryName = category.Name,
                    Count = count,
                    Percentage = totalProps > 0 ? (decimal)count / totalProps * 100 : 0
                });
            }
            dashboardData.CategoryStats = categoryStats;

            // Şehir istatistikleri
            var cityStats = await _context.Properties
                .Where(p => p.IsActive)
                .GroupBy(p => p.City.Name)
                .Select(g => new CityStats
                {
                    CityName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(c => c.Count)
                .Take(5)
                .ToListAsync();
            dashboardData.CityStats = cityStats;

            return View(dashboardData);
        }

        // AJAX endpoints
        [HttpGet]
        public async Task<IActionResult> GetPropertyCount()
        {
            var count = await _context.Properties.CountAsync(p => p.IsActive);
            return Json(count);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _context.ContactRequests.CountAsync(c => !c.IsRead);
            return Json(count);
        }

        // Quick stats for widgets
        [HttpGet]
        public async Task<IActionResult> GetQuickStats()
        {
            var stats = new
            {
                TotalProperties = await _context.Properties.CountAsync(),
                ActiveProperties = await _context.Properties.CountAsync(p => p.IsActive),
                FeaturedProperties = await _context.Properties.CountAsync(p => p.IsActive && p.IsFeatured),
                UnreadMessages = await _context.ContactRequests.CountAsync(c => !c.IsRead),
                PropertiesToday = await _context.Properties.CountAsync(p => p.CreatedDate.Date == DateTime.Today),
                PropertiesThisWeek = await _context.Properties.CountAsync(p => p.CreatedDate >= DateTime.Now.AddDays(-7)),
                PropertiesThisMonth = await _context.Properties.CountAsync(p => p.CreatedDate.Month == DateTime.Now.Month)
            };

            return Json(stats);
        }
    }
}
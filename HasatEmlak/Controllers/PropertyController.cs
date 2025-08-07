// Controllers/PropertyController.cs
using HasatEmlak.Data;
using HasatEmlak.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Controllers
{
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // İlan listesi
        public async Task<IActionResult> Index(SearchViewModel search, int page = 1)
        {
            int pageSize = 12;
            var query = _context.Properties
                .Include(p => p.PropertyImages)
                .Include(p => p.Category)
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Neighborhood)
                .Where(p => p.IsActive);

            // Filtreleme
            if (search.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == search.CategoryId.Value);

            if (search.PropertyTypeId.HasValue)
                query = query.Where(p => p.PropertyTypeId == search.PropertyTypeId.Value);

            if (search.CityId.HasValue)
                query = query.Where(p => p.CityId == search.CityId.Value);

            if (search.DistrictId.HasValue)
                query = query.Where(p => p.DistrictId == search.DistrictId.Value);

            if (search.MinPrice.HasValue)
                query = query.Where(p => p.Price >= search.MinPrice.Value);

            if (search.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= search.MaxPrice.Value);

            if (search.MinArea.HasValue)
                query = query.Where(p => p.Area >= search.MinArea.Value);

            if (search.MaxArea.HasValue)
                query = query.Where(p => p.Area <= search.MaxArea.Value);

            if (search.RoomCount.HasValue)
                query = query.Where(p => p.RoomCount == search.RoomCount.Value);

            if (!string.IsNullOrEmpty(search.Keywords))
            {
                var keywords = search.Keywords.ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(keywords) ||
                    p.Description.ToLower().Contains(keywords) ||
                    p.Address.ToLower().Contains(keywords));
            }

            // Sıralama
            switch (search.SortBy?.ToLower())
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "date_asc":
                    query = query.OrderBy(p => p.CreatedDate);
                    break;
                case "area_desc":
                    query = query.OrderByDescending(p => p.Area);
                    break;
                default:
                    query = query.OrderByDescending(p => p.CreatedDate);
                    break;
            }

            var totalCount = await query.CountAsync();
            var properties = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ViewBag'ler için dropdown verilerini hazırla
            await PrepareViewBags();

            var viewModel = new PropertyListViewModel
            {
                Properties = properties,
                SearchModel = search,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                TotalCount = totalCount
            };

            return View(viewModel);
        }

        // İlan detayı
        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties
                .Include(p => p.PropertyImages.OrderBy(pi => pi.DisplayOrder))
                .Include(p => p.Category)
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Neighborhood)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (property == null)
                return NotFound();

            // Benzer ilanlar
            var similarProperties = await _context.Properties
                .Include(p => p.PropertyImages)
                .Include(p => p.Category)
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Where(p => p.IsActive && p.Id != id &&
                           (p.CategoryId == property.CategoryId ||
                            p.PropertyTypeId == property.PropertyTypeId ||
                            p.DistrictId == property.DistrictId))
                .OrderByDescending(p => p.CreatedDate)
                .Take(4)
                .ToListAsync();

            ViewBag.SimilarProperties = similarProperties;

            return View(property);
        }

        // AJAX - İlçeleri getir
        [HttpGet]
        public async Task<IActionResult> GetDistricts(int cityId)
        {
            var districts = await _context.Districts
                .Where(d => d.CityId == cityId && d.IsActive)
                .OrderBy(d => d.Name)
                .Select(d => new { id = d.Id, name = d.Name })
                .ToListAsync();

            return Json(districts);
        }

        // AJAX - Mahalleleri getir
        [HttpGet]
        public async Task<IActionResult> GetNeighborhoods(int districtId)
        {
            var neighborhoods = await _context.Neighborhoods
                .Where(n => n.DistrictId == districtId && n.IsActive)
                .OrderBy(n => n.Name)
                .Select(n => new { id = n.Id, name = n.Name })
                .ToListAsync();

            return Json(neighborhoods);
        }

        private async Task PrepareViewBags()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            ViewBag.PropertyTypes = await _context.PropertyTypes
                .Where(pt => pt.IsActive)
                .OrderBy(pt => pt.DisplayOrder)
                .ToListAsync();

            ViewBag.Cities = await _context.Cities
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
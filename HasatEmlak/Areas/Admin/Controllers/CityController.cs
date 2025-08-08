// Areas/Admin/Controllers/LocationController.cs
using HasatEmlak.Data;
using HasatEmlak.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Location
        public async Task<IActionResult> Index()
        {
            var cities = await _context.Cities
                .Include(c => c.Districts)
                .ThenInclude(d => d.Neighborhoods)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(cities);
        }

        #region City Operations

        [HttpPost]
        public async Task<IActionResult> CreateCity(string name, string plateCode)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new { success = false, message = "Şehir adı gereklidir!" });
            }

            var existingCity = await _context.Cities.FirstOrDefaultAsync(c => c.Name == name);
            if (existingCity != null)
            {
                return Json(new { success = false, message = "Bu şehir zaten mevcut!" });
            }

            var city = new City
            {
                Name = name,
                PlateCode = plateCode,
                IsActive = true
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Şehir başarıyla eklendi!", cityId = city.Id });
        }

        [HttpPost]
        public async Task<IActionResult> EditCity(int id, string name, string plateCode)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return Json(new { success = false, message = "Şehir bulunamadı!" });
            }

            city.Name = name;
            city.PlateCode = plateCode;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Şehir başarıyla güncellendi!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.Include(c => c.Districts).FirstOrDefaultAsync(c => c.Id == id);
            if (city == null)
            {
                return Json(new { success = false, message = "Şehir bulunamadı!" });
            }

            if (city.Districts.Any())
            {
                return Json(new { success = false, message = "Bu şehirde ilçeler bulunmaktadır!" });
            }

            var hasProperties = await _context.Properties.AnyAsync(p => p.CityId == id);
            if (hasProperties)
            {
                return Json(new { success = false, message = "Bu şehirde ilanlar bulunmaktadır!" });
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Şehir başarıyla silindi!" });
        }

        #endregion

        #region District Operations

        [HttpPost]
        public async Task<IActionResult> CreateDistrict(int cityId, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new { success = false, message = "İlçe adı gereklidir!" });
            }

            var city = await _context.Cities.FindAsync(cityId);
            if (city == null)
            {
                return Json(new { success = false, message = "Şehir bulunamadı!" });
            }

            var existingDistrict = await _context.Districts
                .FirstOrDefaultAsync(d => d.CityId == cityId && d.Name == name);
            if (existingDistrict != null)
            {
                return Json(new { success = false, message = "Bu ilçe zaten mevcut!" });
            }

            var district = new District
            {
                Name = name,
                CityId = cityId,
                IsActive = true
            };

            _context.Districts.Add(district);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "İlçe başarıyla eklendi!", districtId = district.Id });
        }

        [HttpPost]
        public async Task<IActionResult> EditDistrict(int id, string name)
        {
            var district = await _context.Districts.FindAsync(id);
            if (district == null)
            {
                return Json(new { success = false, message = "İlçe bulunamadı!" });
            }

            district.Name = name;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "İlçe başarıyla güncellendi!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDistrict(int id)
        {
            var district = await _context.Districts.Include(d => d.Neighborhoods).FirstOrDefaultAsync(d => d.Id == id);
            if (district == null)
            {
                return Json(new { success = false, message = "İlçe bulunamadı!" });
            }

            if (district.Neighborhoods.Any())
            {
                return Json(new { success = false, message = "Bu ilçede mahalleler bulunmaktadır!" });
            }

            var hasProperties = await _context.Properties.AnyAsync(p => p.DistrictId == id);
            if (hasProperties)
            {
                return Json(new { success = false, message = "Bu ilçede ilanlar bulunmaktadır!" });
            }

            _context.Districts.Remove(district);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "İlçe başarıyla silindi!" });
        }

        #endregion

        #region Neighborhood Operations

        [HttpPost]
        public async Task<IActionResult> CreateNeighborhood(int districtId, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new { success = false, message = "Mahalle adı gereklidir!" });
            }

            var district = await _context.Districts.FindAsync(districtId);
            if (district == null)
            {
                return Json(new { success = false, message = "İlçe bulunamadı!" });
            }

            var existingNeighborhood = await _context.Neighborhoods
                .FirstOrDefaultAsync(n => n.DistrictId == districtId && n.Name == name);
            if (existingNeighborhood != null)
            {
                return Json(new { success = false, message = "Bu mahalle zaten mevcut!" });
            }

            var neighborhood = new Neighborhood
            {
                Name = name,
                DistrictId = districtId,
                IsActive = true
            };

            _context.Neighborhoods.Add(neighborhood);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Mahalle başarıyla eklendi!", neighborhoodId = neighborhood.Id });
        }

        [HttpPost]
        public async Task<IActionResult> EditNeighborhood(int id, string name)
        {
            var neighborhood = await _context.Neighborhoods.FindAsync(id);
            if (neighborhood == null)
            {
                return Json(new { success = false, message = "Mahalle bulunamadı!" });
            }

            neighborhood.Name = name;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Mahalle başarıyla güncellendi!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNeighborhood(int id)
        {
            var neighborhood = await _context.Neighborhoods.FindAsync(id);
            if (neighborhood == null)
            {
                return Json(new { success = false, message = "Mahalle bulunamadı!" });
            }

            var hasProperties = await _context.Properties.AnyAsync(p => p.NeighborhoodId == id);
            if (hasProperties)
            {
                return Json(new { success = false, message = "Bu mahallede ilanlar bulunmaktadır!" });
            }

            _context.Neighborhoods.Remove(neighborhood);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Mahalle başarıyla silindi!" });
        }

        #endregion

        #region AJAX Helpers

        [HttpGet]
        public async Task<IActionResult> GetDistrictsByCity(int cityId)
        {
            var districts = await _context.Districts
                .Where(d => d.CityId == cityId && d.IsActive)
                .OrderBy(d => d.Name)
                .Select(d => new { id = d.Id, name = d.Name })
                .ToListAsync();

            return Json(districts);
        }

        [HttpGet]
        public async Task<IActionResult> GetNeighborhoodsByDistrict(int districtId)
        {
            var neighborhoods = await _context.Neighborhoods
                .Where(n => n.DistrictId == districtId && n.IsActive)
                .OrderBy(n => n.Name)
                .Select(n => new { id = n.Id, name = n.Name })
                .ToListAsync();

            return Json(neighborhoods);
        }

        #endregion
    }
}
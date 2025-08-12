// Areas/Admin/Controllers/PropertyTypeController.cs
using HasatEmlak.Data;
using HasatEmlak.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PropertyTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var propertyTypes = await _context.PropertyTypes
                .OrderBy(pt => pt.DisplayOrder)
                .ToListAsync();
            return View(propertyTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string description, string iconClass, int displayOrder)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new { success = false, message = "Emlak türü adı gereklidir!" });
            }

            try
            {
                var propertyType = new PropertyType
                {
                    Name = name,
                    Description = description,
                    IconClass = iconClass ?? "fas fa-building",
                    DisplayOrder = displayOrder > 0 ? displayOrder : await _context.PropertyTypes.CountAsync() + 1,
                    IsActive = true
                };

                _context.PropertyTypes.Add(propertyType);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Emlak türü başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                // Detaylı hata logu için
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, string description, string iconClass, int displayOrder)
        {
            var propertyType = await _context.PropertyTypes.FindAsync(id);
            if (propertyType == null)
            {
                return Json(new { success = false, message = "Emlak türü bulunamadı!" });
            }

            propertyType.Name = name;
            propertyType.Description = description;
            propertyType.IconClass = iconClass;
            propertyType.DisplayOrder = displayOrder;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Emlak türü başarıyla güncellendi!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var propertyType = await _context.PropertyTypes.FindAsync(id);
            if (propertyType == null)
            {
                return Json(new { success = false, message = "Emlak türü bulunamadı!" });
            }

            var hasProperties = await _context.Properties.AnyAsync(p => p.PropertyTypeId == id);
            if (hasProperties)
            {
                return Json(new { success = false, message = "Bu emlak türüne ait ilanlar bulunmaktadır!" });
            }

            _context.PropertyTypes.Remove(propertyType);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Emlak türü başarıyla silindi!" });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var propertyType = await _context.PropertyTypes.FindAsync(id);
            if (propertyType == null)
            {
                return Json(new { success = false, message = "Emlak türü bulunamadı!" });
            }

            propertyType.IsActive = !propertyType.IsActive;
            await _context.SaveChangesAsync();

            var status = propertyType.IsActive ? "aktif" : "pasif";
            return Json(new { success = true, message = $"Emlak türü {status} yapıldı!" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(List<int> ids)
        {
            try
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    var propertyType = await _context.PropertyTypes.FindAsync(ids[i]);
                    if (propertyType != null)
                    {
                        propertyType.DisplayOrder = i + 1;
                    }
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sıralama başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Sıralama güncellenirken hata oluştu!" });
            }
        }
    }
}
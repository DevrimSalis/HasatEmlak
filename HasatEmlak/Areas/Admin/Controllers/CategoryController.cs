// Areas/Admin/Controllers/CategoryController.cs
using HasatEmlak.Data;
using HasatEmlak.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string description, string iconClass)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new { success = false, message = "Kategori adı gereklidir!" });
            }

            var category = new Category
            {
                Name = name,
                Description = description,
                IconClass = iconClass ?? "fas fa-tag",
                IsActive = true,
                DisplayOrder = await _context.Categories.CountAsync() + 1
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Kategori başarıyla eklendi!" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string name, string description, string iconClass)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }

            category.Name = name;
            category.Description = description;
            category.IconClass = iconClass;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Kategori başarıyla güncellendi!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }

            var hasProperties = await _context.Properties.AnyAsync(p => p.CategoryId == id);
            if (hasProperties)
            {
                return Json(new { success = false, message = "Bu kategoriye ait ilanlar bulunmaktadır!" });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Kategori başarıyla silindi!" });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı!" });
            }

            category.IsActive = !category.IsActive;
            await _context.SaveChangesAsync();

            var status = category.IsActive ? "aktif" : "pasif";
            return Json(new { success = true, message = $"Kategori {status} yapıldı!" });
        }
    }
}
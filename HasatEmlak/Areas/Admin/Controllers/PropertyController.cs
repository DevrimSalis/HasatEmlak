// Areas/Admin/Controllers/PropertyController.cs
using HasatEmlak.Data;
using HasatEmlak.Models.Entities;
using HasatEmlak.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PropertyController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Property
        public async Task<IActionResult> Index(string search, int? categoryId, int? cityId, bool? isActive, int page = 1)
        {
            int pageSize = 20;
            var query = _context.Properties
                .Include(p => p.Category)
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.PropertyImages)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) ||
                                        p.Description.Contains(search) ||
                                        p.Address.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (cityId.HasValue)
            {
                query = query.Where(p => p.CityId == cityId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var properties = await query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ViewBag data for filters
            await PrepareViewBagDataSafe();

            var viewModel = new PropertyListViewModel
            {
                Properties = properties,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                TotalCount = totalCount,
                SearchModel = new SearchViewModel
                {
                    Keywords = search,
                    CategoryId = categoryId,
                    CityId = cityId
                }
            };

            ViewBag.IsActive = isActive;
            return View(viewModel);
        }

        // GET: Admin/Property/Create
        public async Task<IActionResult> Create()
        {
            await PrepareViewBagDataSafe();
            return View(new PropertyCreateViewModel());
        }

        // POST: Admin/Property/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyCreateViewModel model)
        {
            try
            {
                // Debug log
                Console.WriteLine($"=== CREATE DEBUG ===");
                Console.WriteLine($"Received {model.Images?.Count ?? 0} images");

                // Duplicate image kontrolü
                if (model.Images != null && model.Images.Any())
                {
                    var uniqueImages = model.Images
                        .Where(img => img != null && img.Length > 0)
                        .GroupBy(img => new {
                            Name = img.FileName?.ToLower(),
                            Size = img.Length,
                            Type = img.ContentType
                        })
                        .Select(g => g.First())
                        .ToList();

                    if (uniqueImages.Count != model.Images.Count)
                    {
                        Console.WriteLine($"Removed {model.Images.Count - uniqueImages.Count} duplicate images");
                        model.Images = uniqueImages;
                    }

                    foreach (var img in model.Images)
                    {
                        Console.WriteLine($"Image: {img.FileName} - {img.Length} bytes - {img.ContentType}");
                    }
                }

                if (!ModelState.IsValid)
                {
                    await PrepareViewBagDataSafe();
                    TempData["Error"] = "Lütfen tüm zorunlu alanları doldurun!";
                    return View(model);
                }

                var property = new Property
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    Area = model.Area,
                    RoomCount = model.RoomCount,
                    BathroomCount = model.BathroomCount,
                    FloorNumber = model.FloorNumber,
                    TotalFloors = model.TotalFloors,
                    BuildingAge = model.BuildingAge,
                    Address = model.Address,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    IsFeatured = model.IsFeatured,
                    CategoryId = model.CategoryId,
                    PropertyTypeId = model.PropertyTypeId,
                    CityId = model.CityId,
                    DistrictId = model.DistrictId,
                    NeighborhoodId = model.NeighborhoodId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Property saved with ID: {property.Id}");

                // Handle image uploads
                if (model.Images != null && model.Images.Any())
                {
                    Console.WriteLine($"Processing {model.Images.Count} images for property {property.Id}");
                    await HandleImageUploadsSafe(property.Id, model.Images);
                }

                TempData["Success"] = "İlan başarıyla oluşturuldu!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create: {ex}");
                TempData["Error"] = $"İlan kaydedilirken hata oluştu: {ex.Message}";
                await PrepareViewBagDataSafe();
                return View(model);
            }
        }

        // GET: Admin/Property/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _context.Properties
                .Include(p => p.PropertyImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                TempData["Error"] = "İlan bulunamadı!";
                return RedirectToAction(nameof(Index));
            }

            var model = new PropertyCreateViewModel
            {
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                Area = property.Area,
                RoomCount = property.RoomCount,
                BathroomCount = property.BathroomCount,
                FloorNumber = property.FloorNumber,
                TotalFloors = property.TotalFloors,
                BuildingAge = property.BuildingAge,
                Address = property.Address,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                IsFeatured = property.IsFeatured,
                CategoryId = property.CategoryId,
                PropertyTypeId = property.PropertyTypeId,
                CityId = property.CityId,
                DistrictId = property.DistrictId,
                NeighborhoodId = property.NeighborhoodId
            };

            await PrepareViewBagDataSafe();
            ViewBag.PropertyId = property.Id;
            ViewBag.ExistingImages = property.PropertyImages;
            return View(model);
        }

        // POST: Admin/Property/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var property = await _context.Properties.FindAsync(id);
                if (property == null)
                {
                    TempData["Error"] = "İlan bulunamadı!";
                    return RedirectToAction(nameof(Index));
                }

                // Update property
                property.Title = model.Title;
                property.Description = model.Description;
                property.Price = model.Price;
                property.Area = model.Area;
                property.RoomCount = model.RoomCount;
                property.BathroomCount = model.BathroomCount;
                property.FloorNumber = model.FloorNumber;
                property.TotalFloors = model.TotalFloors;
                property.BuildingAge = model.BuildingAge;
                property.Address = model.Address;
                property.Latitude = model.Latitude;
                property.Longitude = model.Longitude;
                property.IsFeatured = model.IsFeatured;
                property.CategoryId = model.CategoryId;
                property.PropertyTypeId = model.PropertyTypeId;
                property.CityId = model.CityId;
                property.DistrictId = model.DistrictId;
                property.NeighborhoodId = model.NeighborhoodId;
                property.UpdatedDate = DateTime.Now;

                _context.Properties.Update(property);
                await _context.SaveChangesAsync();

                // Handle new image uploads
                if (model.Images != null && model.Images.Any())
                {
                    await HandleImageUploadsSafe(property.Id, model.Images);
                }

                TempData["Success"] = "İlan başarıyla güncellendi!";
                return RedirectToAction(nameof(Index));
            }

            await PrepareViewBagDataSafe();
            ViewBag.PropertyId = id;
            var existingImages = await _context.PropertyImages.Where(pi => pi.PropertyId == id).ToListAsync();
            ViewBag.ExistingImages = existingImages;
            return View(model);
        }

        // GET: Admin/Property/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Category)
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.District)
                .Include(p => p.Neighborhood)
                .Include(p => p.PropertyImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                TempData["Error"] = "İlan bulunamadı!";
                return RedirectToAction(nameof(Index));
            }

            return View(property);
        }

        // POST: Admin/Property/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties
                .Include(p => p.PropertyImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return Json(new { success = false, message = "İlan bulunamadı!" });
            }

            try
            {
                // Delete images from file system
                foreach (var image in property.PropertyImages)
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "İlan başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Silme işlemi sırasında hata oluştu!" });
            }
        }

        // POST: Admin/Property/ToggleStatus
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool status)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return Json(new { success = false, message = "İlan bulunamadı!" });
            }

            property.IsActive = status;
            property.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            var statusText = status ? "aktif" : "pasif";
            return Json(new { success = true, message = $"İlan başarıyla {statusText} yapıldı!" });
        }

        // POST: Admin/Property/ToggleFeatured
        [HttpPost]
        public async Task<IActionResult> ToggleFeatured(int id, bool featured)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return Json(new { success = false, message = "İlan bulunamadı!" });
            }

            property.IsFeatured = featured;
            property.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            var featuredText = featured ? "öne çıkarıldı" : "öne çıkarılandan kaldırıldı";
            return Json(new { success = true, message = $"İlan başarıyla {featuredText}!" });
        }

        // POST: Admin/Property/BulkAction
        [HttpPost]
        public async Task<IActionResult> BulkAction(string action, int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return Json(new { success = false, message = "Lütfen en az bir ilan seçin!" });
            }

            try
            {
                var properties = await _context.Properties.Where(p => ids.Contains(p.Id)).ToListAsync();

                switch (action.ToLower())
                {
                    case "activate":
                        foreach (var property in properties)
                        {
                            property.IsActive = true;
                            property.UpdatedDate = DateTime.Now;
                        }
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = $"{properties.Count} ilan aktif yapıldı!" });

                    case "deactivate":
                        foreach (var property in properties)
                        {
                            property.IsActive = false;
                            property.UpdatedDate = DateTime.Now;
                        }
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = $"{properties.Count} ilan pasif yapıldı!" });

                    case "delete":
                        _context.Properties.RemoveRange(properties);
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = $"{properties.Count} ilan silindi!" });

                    default:
                        return Json(new { success = false, message = "Geçersiz işlem!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında hata oluştu!" });
            }
        }

        // POST: Admin/Property/DeleteImage
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _context.PropertyImages.FindAsync(imageId);
            if (image == null)
            {
                return Json(new { success = false, message = "Resim bulunamadı!" });
            }

            try
            {
                // Delete from file system
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _context.PropertyImages.Remove(image);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Resim başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Resim silinirken hata oluştu!" });
            }
        }

        // GET: Admin/Property/GetRecentProperties
        [HttpGet]
        public async Task<IActionResult> GetRecentProperties()
        {
            var properties = await _context.Properties
                .Include(p => p.Category)
                .Include(p => p.PropertyImages)
                .OrderByDescending(p => p.CreatedDate)
                .Take(10)
                .Select(p => new
                {
                    id = p.Id,
                    title = p.Title,
                    address = p.Address,
                    price = p.Price,
                    categoryId = p.CategoryId,
                    categoryName = p.Category.Name,
                    isActive = p.IsActive,
                    createdDate = p.CreatedDate,
                    imagePath = p.PropertyImages.FirstOrDefault() != null ? p.PropertyImages.FirstOrDefault().ImagePath : null
                })
                .ToListAsync();

            return Json(properties);
        }

        // GET: Admin/Property/GetDistricts
        [HttpGet]
        public async Task<IActionResult> GetDistricts(int cityId)
        {
            try
            {
                var districts = await _context.Districts
                    .Where(d => d.CityId == cityId && d.IsActive)
                    .OrderBy(d => d.Name)
                    .Select(d => new { id = d.Id, name = d.Name })
                    .ToListAsync();

                return Json(districts);
            }
            catch (Exception ex)
            {
                return Json(new { error = "İlçeler getirilemedi", message = ex.Message });
            }
        }

        // GET: Admin/Property/GetNeighborhoods
        [HttpGet]
        public async Task<IActionResult> GetNeighborhoods(int districtId)
        {
            try
            {
                var neighborhoods = await _context.Neighborhoods
                    .Where(n => n.DistrictId == districtId && n.IsActive)
                    .OrderBy(n => n.Name)
                    .Select(n => new { id = n.Id, name = n.Name })
                    .ToListAsync();

                return Json(neighborhoods);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Mahalleler getirilemedi", message = ex.Message });
            }
        }

        #region Private Methods

        // Güvenli ViewBag preparation
        private async Task PrepareViewBagDataSafe()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                var propertyTypes = await _context.PropertyTypes
                    .Where(pt => pt.IsActive)
                    .OrderBy(pt => pt.DisplayOrder)
                    .ToListAsync();

                var cities = await _context.Cities
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                // Güvenli ViewBag assignment
                ViewBag.Categories = categories ?? new List<Category>();
                ViewBag.PropertyTypes = propertyTypes ?? new List<PropertyType>();
                ViewBag.Cities = cities ?? new List<City>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PrepareViewBagDataSafe: {ex}");

                // Hata durumunda boş listeler
                ViewBag.Categories = new List<Category>();
                ViewBag.PropertyTypes = new List<PropertyType>();
                ViewBag.Cities = new List<City>();
            }
        }

        // Güvenli image upload
        private async Task HandleImageUploadsSafe(int propertyId, List<IFormFile> images)
        {
            if (images == null || !images.Any())
                return;

            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "properties");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Duplicate dosyaları engelle - multiple criteria
            var processedFiles = new HashSet<string>();
            var displayOrder = await _context.PropertyImages
                .Where(pi => pi.PropertyId == propertyId)
                .CountAsync();

            // Unique images sadece
            var uniqueImages = images
                .Where(img => img != null && img.Length > 0)
                .GroupBy(img => new {
                    Name = img.FileName?.ToLower(),
                    Size = img.Length,
                    Type = img.ContentType
                })
                .Select(g => g.First())
                .ToList();

            Console.WriteLine($"Original images: {images.Count}, Unique images: {uniqueImages.Count}");

            foreach (var image in uniqueImages)
            {
                try
                {
                    // Double check for duplicates
                    var fileHash = $"{image.FileName?.ToLower()}_{image.Length}_{image.ContentType}";
                    if (processedFiles.Contains(fileHash))
                    {
                        Console.WriteLine($"Skipping duplicate: {image.FileName}");
                        continue;
                    }
                    processedFiles.Add(fileHash);

                    // Unique filename
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Ensure unique file path
                    while (System.IO.File.Exists(filePath))
                    {
                        fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        filePath = Path.Combine(uploadPath, fileName);
                    }

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    Console.WriteLine($"Saved file: {fileName}");

                    // Save to database
                    var propertyImage = new PropertyImage
                    {
                        PropertyId = propertyId,
                        ImagePath = $"images/properties/{fileName}",
                        AltText = $"Property {propertyId} Image",
                        IsMainImage = displayOrder == 0,
                        DisplayOrder = displayOrder,
                        CreatedDate = DateTime.Now
                    };

                    _context.PropertyImages.Add(propertyImage);
                    displayOrder++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing image {image.FileName}: {ex.Message}");
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"Saved {uniqueImages.Count} images to database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving images to database: {ex}");
                throw;
            }
        }

        #endregion
    }
}
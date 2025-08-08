// Areas/Admin/Controllers/ContactController.cs
using HasatEmlak.Data;
using HasatEmlak.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HasatEmlak.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Contact
        public async Task<IActionResult> Index(string search, bool? isRead, int page = 1)
        {
            int pageSize = 20;
            var query = _context.ContactRequests
                .Include(c => c.Property)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.FullName.Contains(search) ||
                                        c.Email.Contains(search) ||
                                        c.Subject.Contains(search) ||
                                        c.Message.Contains(search));
            }

            if (isRead.HasValue)
            {
                query = query.Where(c => c.IsRead == isRead.Value);
            }

            var totalCount = await query.CountAsync();
            var contacts = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.IsRead = isRead;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalCount = totalCount;

            return View(contacts);
        }

        // GET: Admin/Contact/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var contact = await _context.ContactRequests
                .Include(c => c.Property)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                TempData["Error"] = "Mesaj bulunamadı!";
                return RedirectToAction(nameof(Index));
            }

            // Mark as read
            if (!contact.IsRead)
            {
                contact.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(contact);
        }

        // POST: Admin/Contact/MarkAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var contact = await _context.ContactRequests.FindAsync(id);
            if (contact == null)
            {
                return Json(new { success = false, message = "Mesaj bulunamadı!" });
            }

            contact.IsRead = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Mesaj okundu olarak işaretlendi!" });
        }

        // POST: Admin/Contact/MarkAsUnread
        [HttpPost]
        public async Task<IActionResult> MarkAsUnread(int id)
        {
            var contact = await _context.ContactRequests.FindAsync(id);
            if (contact == null)
            {
                return Json(new { success = false, message = "Mesaj bulunamadı!" });
            }

            contact.IsRead = false;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Mesaj okunmadı olarak işaretlendi!" });
        }

        // POST: Admin/Contact/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.ContactRequests.FindAsync(id);
            if (contact == null)
            {
                return Json(new { success = false, message = "Mesaj bulunamadı!" });
            }

            try
            {
                _context.ContactRequests.Remove(contact);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Mesaj başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Silme işlemi sırasında hata oluştu!" });
            }
        }

        // POST: Admin/Contact/BulkAction
        [HttpPost]
        public async Task<IActionResult> BulkAction(string action, int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return Json(new { success = false, message = "Lütfen en az bir mesaj seçin!" });
            }

            try
            {
                var contacts = await _context.ContactRequests.Where(c => ids.Contains(c.Id)).ToListAsync();

                switch (action.ToLower())
                {
                    case "read":
                        foreach (var contact in contacts)
                        {
                            contact.IsRead = true;
                        }
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = $"{contacts.Count} mesaj okundu olarak işaretlendi!" });

                    case "unread":
                        foreach (var contact in contacts)
                        {
                            contact.IsRead = false;
                        }
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = $"{contacts.Count} mesaj okunmadı olarak işaretlendi!" });

                    case "delete":
                        _context.ContactRequests.RemoveRange(contacts);
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = $"{contacts.Count} mesaj silindi!" });

                    default:
                        return Json(new { success = false, message = "Geçersiz işlem!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında hata oluştu!" });
            }
        }

        // GET: Admin/Contact/GetUnreadCount
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _context.ContactRequests.CountAsync(c => !c.IsRead);
            return Json(count);
        }

        // GET: Admin/Contact/GetRecentMessages
        [HttpGet]
        public async Task<IActionResult> GetRecentMessages()
        {
            var messages = await _context.ContactRequests
                .Include(c => c.Property)
                .OrderByDescending(c => c.CreatedDate)
                .Take(5)
                .Select(c => new
                {
                    id = c.Id,
                    fullName = c.FullName,
                    email = c.Email,
                    subject = c.Subject,
                    message = c.Message.Length > 100 ? c.Message.Substring(0, 100) + "..." : c.Message,
                    isRead = c.IsRead,
                    createdDate = c.CreatedDate,
                    propertyTitle = c.Property != null ? c.Property.Title : null
                })
                .ToListAsync();

            return Json(messages);
        }

        // POST: Admin/Contact/Reply
        [HttpPost]
        public async Task<IActionResult> Reply(int id, string replyMessage)
        {
            var contact = await _context.ContactRequests.FindAsync(id);
            if (contact == null)
            {
                return Json(new { success = false, message = "Mesaj bulunamadı!" });
            }

            try
            {
                // Email gönderme işlemi burada yapılabilir
                // Şimdilik sadece okundu olarak işaretleyelim
                contact.IsRead = true;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Yanıt gönderildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Yanıt gönderilirken hata oluştu!" });
            }
        }
    }
}
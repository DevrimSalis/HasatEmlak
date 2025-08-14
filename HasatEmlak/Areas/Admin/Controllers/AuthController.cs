using Microsoft.AspNetCore.Mvc;

namespace HasatEmlak.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            // Eğer session'da login varsa direkt home'a yönlendir
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Basit admin giriş (güvenlik için daha sonra geliştirilebilir)
            if (username == "admin" && password == "dDKm4Bhu9MzohwNs")
            {
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                HttpContext.Session.SetString("AdminUsername", username);
                TempData["Success"] = "Başarıyla giriş yaptınız!";
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Başarıyla çıkış yaptınız!";
            return RedirectToAction("Login");
        }
    }
}
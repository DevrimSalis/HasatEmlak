using Microsoft.AspNetCore.Mvc;

namespace HasatEmlak.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

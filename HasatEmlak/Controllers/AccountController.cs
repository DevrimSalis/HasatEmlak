using Microsoft.AspNetCore.Mvc;

namespace HasatEmlak.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

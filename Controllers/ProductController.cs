using Microsoft.AspNetCore.Mvc;

namespace Tast_two_API.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Uam.LabHelpDesk.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
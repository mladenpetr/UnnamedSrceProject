using System.Web.Mvc;
using SrceApplicaton.Models;

namespace SrceApplicaton.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AnotherLink()
        {
            return View("Index");
        }
    }
}

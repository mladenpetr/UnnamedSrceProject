using System.Web.Mvc;
using SrceApplicaton.Models;
using System.Net;

namespace SrceApplicaton.Controllers
{
    public class HomeController : Controller
    {
        private SrceAppDatabase1Entities db = new SrceAppDatabase1Entities();

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

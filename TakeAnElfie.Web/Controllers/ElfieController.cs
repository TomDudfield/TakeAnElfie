using System.Web.Mvc;
using TakeAnElfie.Web.Models;

namespace TakeAnElfie.Web.Controllers
{
    public class ElfieController : Controller
    {
        public ActionResult Index()
        {
            return View(new UserModel());
        }

        public ActionResult Camera()
        {
            return View(new CameraModel());
        }
    }
}
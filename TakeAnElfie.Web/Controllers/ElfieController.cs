using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TakeAnElfie.Web.Controllers
{
    public class ElfieController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Camera()
        {
            return View();
        }
    }
}
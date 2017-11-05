using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace AIMS_DB_IATI.Web.Controllers
{
    public class HomeController : Controller
    {
        ActionResult Index() {
            return View();
        }
    }
}

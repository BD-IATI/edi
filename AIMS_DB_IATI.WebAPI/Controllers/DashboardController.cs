using AIMS_DB_IATI.WebAPI.Models.IATIImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AIMS_DB_IATI.WebAPI.Controllers
{

    public class DashboardController : ApiController
    {
        public DashboardModel GetDashboardData()
        {

            return new DashboardModel { LastDownloadDate = DateTime.Now.AddDays(-50) };
        }
    }
}

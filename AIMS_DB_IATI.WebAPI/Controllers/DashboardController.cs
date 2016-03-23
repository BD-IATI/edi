using AIMS_BD_IATI.DAL;
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
        public DashboardModel GetDashboardData(string dp)
        {
            var dashboardModel = new DashboardModel();
            dashboardModel.LastDownloadDate = new AimsDbIatiDAL().GetLastDownloadDate(dp);
            dashboardModel.DelegatedActivities = new AimsDbIatiDAL().GetDelegatedActivities(dp);


            return dashboardModel;
        }
    }
}

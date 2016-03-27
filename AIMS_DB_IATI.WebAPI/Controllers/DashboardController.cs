using AIMS_BD_IATI.DAL;
using AIMS_DB_IATI.WebAPI.Models;
using AIMS_DB_IATI.WebAPI.Models.IATIImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AIMS_BD_IATI.Library;

namespace AIMS_DB_IATI.WebAPI.Controllers
{

    public class DashboardController : ApiController
    {
        public DashboardModel GetDashboardData(string dp)
        {
            var dashboardModel = new DashboardModel();
            dashboardModel.LastDownloadDate = new AimsDbIatiDAL().GetLastDownloadDate(dp);
            dashboardModel.DelegatedActivities = new AimsDbIatiDAL().GetDelegatedActivities(dp);
            dashboardModel.NewActivityCount = new AimsDbIatiDAL().GetNewActivityCount(dp);

            dashboardModel.DelegatedActivities.ForEach(f => f.AssignedOrgName = Sessions.FundSources.Find(k => k.IATICode == f.AssignedOrgId).n().Name);

            return dashboardModel;
        }
    }
}

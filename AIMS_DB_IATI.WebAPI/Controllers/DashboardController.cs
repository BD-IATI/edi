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
        AimsDAL aimsDAL = new AimsDAL();
        AimsDbIatiDAL aimsDbIatiDAL = new AimsDbIatiDAL();

        public DashboardModel GetDashboardData(string dp)
        {
            Sessions.activitiesContainer.DP = dp;
            Sessions.DP = dp;

            var dashboardModel = new DashboardModel();
            dashboardModel.LastDownloadDate = aimsDbIatiDAL.GetLastDownloadDate(dp);
            dashboardModel.NewActivityCount = aimsDbIatiDAL.GetNewActivityCount(dp);
            dashboardModel.MappedActivityCount = aimsDbIatiDAL.GetMappedActivityCount(dp);
            dashboardModel.TotalActivityCount = aimsDbIatiDAL.GetTotalActivityCount(dp);

            dashboardModel.DelegatedActivities = aimsDbIatiDAL.GetDelegatedActivities(dp);
            dashboardModel.DelegatedActivities.ForEach(f => f.AssignedOrgName = Sessions.FundSources.Find(k => k.IATICode == f.AssignedOrgId).n().Name);

            #region trust fund and cofinance projects

            Sessions.CFnTFModel = aimsDbIatiDAL.GetAssignActivities(dp,true);
            Sessions.CFnTFModel = new CFnTFController().SubmitAssignedActivities(Sessions.CFnTFModel.AssignedActivities);
            dashboardModel.CFnTFModel = Sessions.CFnTFModel;

            #endregion

            dashboardModel.addLogs(aimsDbIatiDAL.GetLastDayLogs(dp));

            return dashboardModel;
        }
    }
}

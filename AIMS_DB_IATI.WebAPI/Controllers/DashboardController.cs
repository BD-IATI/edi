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
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;

namespace AIMS_DB_IATI.WebAPI.Controllers
{

    public class DashboardController : ApiController
    {
        AimsDAL aimsDAL = new AimsDAL();
        AimsDbIatiDAL aimsDbIatiDAL = new AimsDbIatiDAL();

        [AcceptVerbs("GET", "POST")]
        public DashboardModel GetDashboardData(DPLookupItem dp)
        {
            if (dp == null) return null;

            //Sessions.Clear();
            string dpId = dp.ID;
            //Sessions.DP.ID = dpId;
            Sessions.DP = dp;

            var dashboardModel = new DashboardModel();
            dashboardModel.LastDownloadDate = aimsDbIatiDAL.GetLastDownloadDate(dpId);
            dashboardModel.NewActivityCount = aimsDbIatiDAL.GetNewActivityCount(dpId);
            dashboardModel.MappedActivityCount = aimsDbIatiDAL.GetMappedActivityCount(dpId);
            dashboardModel.AssignedActivityCount = aimsDbIatiDAL.GetAssignedActivityCount(dpId);
            dashboardModel.TotalActivityCount = aimsDbIatiDAL.GetTotalActivityCount(dpId);

            dashboardModel.DelegatedActivities = aimsDbIatiDAL.GetDelegatedActivities(dpId);
            dashboardModel.DelegatedActivities.ForEach(f => f.AssignedOrgName = Sessions.FundSources?.Find(k => k.IATICode == f.AssignedOrgId)?.Name);

            #region trust fund and cofinance projects

            Sessions.CFnTFModel = aimsDbIatiDAL.GetAssignActivities(dpId, true);
            Sessions.CFnTFModel = new CFnTFController().SubmitAssignedActivities(Sessions.CFnTFModel.AssignedActivities);
            dashboardModel.CFnTFModel = Sessions.CFnTFModel;

            #endregion

            dashboardModel.addLogs(aimsDbIatiDAL.GetLastDayLogs(dpId));

            return dashboardModel;
        }

        [AcceptVerbs("GET", "POST")]
        public string DownloadDataFromIATI(DPLookupItem dp)
        {

            return "";
        }

        [AcceptVerbs("GET", "POST")]
        public string CheckSession(DPLookupItem dp)
        {
            Sessions.DP = dp;
            return Sessions.CurrentStage;
        }
        [AcceptVerbs("GET", "POST")]
        public string RestartSession(DPLookupItem dp)
        {
            //Sessions.DP = null;
            Sessions.Clear();
            return "";
        }

        [AcceptVerbs("GET", "POST")]
        public List<ActivityModel> RecallDelegatedActivity(ActivityModel da)
        {
            aimsDbIatiDAL.RecallDelegatedActivity(da);

            var das = aimsDbIatiDAL.GetDelegatedActivities(Sessions.DP.ID);
            das.ForEach(f => f.AssignedOrgName = Sessions.FundSources?.Find(k => k.IATICode == f.AssignedOrgId)?.Name);
            return das;
        }
    }
}

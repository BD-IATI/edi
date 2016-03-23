using AIMS_BD_IATI.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    public class DashboardModel
    {
        public DateTime? LastDownloadDate { get; set; }

        public List<AIMS_BD_IATI.DAL.AimsDbIatiDAL.ActivityModel> DelegatedActivities { get; set; }
    }
}
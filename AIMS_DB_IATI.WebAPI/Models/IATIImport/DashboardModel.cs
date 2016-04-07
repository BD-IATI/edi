using AIMS_BD_IATI.DAL;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    public class DashboardModel
    {
        public DateTime? LastDownloadDate { get; set; }

        public List<AimsDbIatiDAL.ActivityModel> DelegatedActivities { get; set; }

        public int NewActivityCount { get; set; }

        public CFnTFModel CFnTFModel { get; set; }

        private List<Log> Logs { get; set; }

        public List<Log> AddedNewActivityLogs { get { return Logs.FindAll(f => f.LogType == (int)LogType.AddedNewActivity); } }
        public List<Log> FinancialDataMismatchedLogs { get { return Logs.FindAll(f => f.LogType == (int)LogType.FinancialDataMismathed); } }

        public List<Log> OtherLogs { get { return Logs.FindAll(f => f.LogType != (int)LogType.AddedNewActivity && f.LogType != (int)LogType.FinancialDataMismathed ); } }

        public int TotalActivityCount { get; set; }

        public void addLogs(List<Log> logs)
        {
            Logs = logs;
        }
    }
}
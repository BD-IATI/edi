using AIMS_BD_IATI.DAL;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoreLinq;
namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    public class DashboardModel
    {
        public DateTime? LastDownloadDate { get; set; }

        public List<ActivityModel> DelegatedActivities { get; set; }

        public int NewActivityCount { get; set; }
        public int MappedActivityCount { get; set; }
        public int AssignedActivityCount { get; set; }

        public CFnTFModel CFnTFModel { get; set; }

        private List<Log> Logs { get; set; }

        public List<Log> AddedNewActivityLogs { get { return Logs.Where(f => f.LogType == (int)LogType.AddedNewActivity).DistinctBy(d => d.IatiIdentifier).ToList(); } }
        public List<Log> FinancialDataMismatchedLogs { get { return Logs.Where(f => f.LogType == (int)LogType.FinancialDataMismathed).DistinctBy(d=>d.IatiIdentifier).ToList(); } }

        public List<Log> OtherLogs { get { return Logs.FindAll(f => f.LogType != (int)LogType.AddedNewActivity && f.LogType != (int)LogType.FinancialDataMismathed ); } }

        public int TotalActivityCount { get; set; }

        public void addLogs(List<Log> logs)
        {
            Logs = logs;
        }
    }
}
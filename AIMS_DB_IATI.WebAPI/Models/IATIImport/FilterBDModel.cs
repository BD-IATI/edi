using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    [Serializable]
    public class FilterBDModel
    {
        public List<iatiactivityModel> iatiActivities { get; set; }
    }

    [Serializable]
    public class iOrgs
    {
        public List<participatingorg> Orgs { get; set; }
        public List<FundSourceLookupItem> FundSources { get; set; }
        public List<LookupItem> ExecutingAgencyTypes { get; internal set; }
        public object ExecutingAgencies { get; internal set; }
    }

}
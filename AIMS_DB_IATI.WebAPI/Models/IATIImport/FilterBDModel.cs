using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoreLinq;
namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    [Serializable]
    public class FilterBDModel
    {
        public List<iatiactivityModel> iatiActivities { get; set; }
        public List<participatingorg> AllExtendingOrgs
        {
            get
            {
                var eOrgs = new List<participatingorg>();
                foreach (var activity in iatiActivities)
                {
                    var participatingOrgs = activity.ExtendingOrgs ?? new List<participatingorg>();

                    eOrgs.AddRange(participatingOrgs);

                }

                var distictOrgs = eOrgs.DistinctBy(l => l.Name).OrderBy(o => o.Name).ToList();


                return distictOrgs;
            }
        }

    }

    [Serializable]
    public class iOrgs
    {
        public List<participatingorg> Orgs { get; set; }
        public List<ExecutingAgencyLookupItem> FundSources { get; set; }
        public List<LookupItem> ExecutingAgencyTypes { get; internal set; }
        public List<ExecutingAgencyLookupItem> ExecutingAgencies { get; internal set; }
    }

}
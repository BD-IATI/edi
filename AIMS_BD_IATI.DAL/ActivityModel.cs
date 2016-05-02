using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.DAL
{
    /// <summary>
    /// same as Activity table in AIMS_DB_IATI database
    /// </summary>
    [Serializable]
    public class ActivityModel
    {
        public int Id { get; set; }
        public string OrgId { get; set; }
        public string IatiIdentifier { get; set; }
        public string IatiActivity { get; set; }
        public Nullable<System.DateTime> DownloadDate { get; set; }
        public string IatiActivityPrev { get; set; }
        public Nullable<System.DateTime> DownloadDatePrev { get; set; }
        public Nullable<int> Hierarchy { get; set; }
        public Nullable<int> ParentHierarchy { get; set; }
        public string AssignedOrgId { get; set; }
        public string AssignedOrgName { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> MappedProjectId { get; set; }
        public Nullable<int> MappedTrustFundId { get; set; }


        public iatiactivity iatiActivity { get; set; }
    }
}

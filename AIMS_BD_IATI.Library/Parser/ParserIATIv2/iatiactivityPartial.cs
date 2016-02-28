using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AIMS_BD_IATI.Library.Parser.ParserIATIv2
{
    public class IatiIgnoreAttribute : Attribute
    {

    }
    public class iatiactivityContainer
    {
        public iatiactivityContainer()
        {
            iatiActivities = new List<iatiactivity>();
            NewProjects = new List<iatiactivity>();
        }
        public List<iatiactivity> iatiActivities { get; set; }
        public List<iatiactivity> NewProjects { get; set; }

        public bool HasRelatedActivity { get { return iatiActivities.Exists(e => e.relatedIatiActivities.Count > 0); } }

    }
    public partial class iatiactivity
    {
        public iatiactivity()
        {
            relatedIatiActivities = new List<iatiactivity>();
            MatchedProjects = new List<iatiactivity>();
        }
        [XmlIgnore]
        public List<iatiactivity> relatedIatiActivities { get; set; }
        [XmlIgnore]
        public List<iatiactivity> MatchedProjects { get; set; }
        [XmlIgnore]
        public string SelectedHierarchy { get; set; }
        [XmlIgnore]
        public string AidType { get; set; }

        [XmlIgnore]
        public decimal PercentToBD
        {
            get
            {
                var rc = recipientcountry.FirstOrDefault(f => f.code == "BD");
                return rc == null ? 0 : rc.percentage;
            }
        }

        [XmlIgnore]
        private bool? isRelevant;
        [XmlIgnore]
        public bool? IsRelevant
        {
            //ToDo add AidType criteria
            get
            {
                return isRelevant ?? PercentToBD >= 20 && activitystatus.code == "2";
            }
            set
            {
                isRelevant = value;
            }
        }

        [XmlIgnore]
        public DateTime PlannedStartDate
        {
            get
            {
                var sdate = activitydate.n().FirstOrDefault(f => f.type == "1");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //1
        [XmlIgnore]
        public DateTime ActualStartDate
        {
            get
            {
                var sdate = activitydate.n().FirstOrDefault(f => f.type == "2");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //2
        [XmlIgnore]
        public DateTime PlannedEndDate
        {
            get
            {
                var sdate = activitydate.n().FirstOrDefault(f => f.type == "3");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //3
        [XmlIgnore]
        public DateTime ActualEndDate
        {
            get
            {
                var sdate = activitydate.n().FirstOrDefault(f => f.type == "4");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //4

    }

    public partial class defaultaidtype
    {
        [XmlIgnore]
        public string name
        {
            get
            {
                if (code == "A01") return "General budget support";
                else if (code == "A02") return "Sector budget support";
                else if (code == "B01") return "Core support to NGOs, other private bodies, PPPs and research institutes";
                else if (code == "B02") return "Core contributions to multilateral institutions";
                else if (code == "B03") return "Contributions to specific-purpose programmes and funds managed by international organisations (multilateral, INGO)";
                else if (code == "B04") return "Basket funds/pooled funding";
                else if (code == "C01") return "Project-type interventions";
                else if (code == "D01") return "Donor country personnel";
                else if (code == "D02") return "Other technical assistance";
                else if (code == "E01") return "Scholarships/training in donor country";
                else if (code == "E02") return "Imputed student costs";
                else if (code == "F01") return "Debt relief";
                else if (code == "G01") return "Administrative costs not included elsewhere";
                else if (code == "H01") return "Development awareness";
                else if (code == "H02") return "Refugees in donor countries";
                else
                    return "";
            }

        }
    }
    public partial class activitystatus
    {
        [XmlIgnore]
        public string name
        {
            get
            {
                if (code == "1") return "Pipeline/identification";
                else if (code == "2") return "Implementation";
                else if (code == "3") return "Completion";
                else if (code == "4") return "Post-completion";
                else if (code == "5") return "Cancelled";
                else if (code == "6") return "Suspended";
                else
                    return "";
            }

        }
    }
}



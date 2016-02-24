using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.Library.Parser.ParserIATIv2
{
    public class iatiactivityContainer
    {
        public List<iatiactivity> iatiActivities { get; set; }
        public bool HasRelatedActivity { get { return iatiActivities.Count > 0; } }

    }
    public partial class iatiactivity
    {
        public List<iatiactivity> relatedIatiActivities { get; set; }
        public string SelectedHierarchy { get; set; }

        public decimal PercentToBD
        {
            get
            {
                var rc = recipientcountry.FirstOrDefault(f => f.code == "BD");
                return rc == null ? 0 : rc.percentage;
            }
        }
    }

    public partial class defaultaidtype
    {
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
}



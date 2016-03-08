using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    public class GeneralPreferencesModel
    {
        public string OrgId { get; set; }
        public string Field { get; set; }
        public string AIMSValue { get; set; }
        public string IATIValue { get; set; }
        public string Source { get; set; }
        public bool IsSourceAims
        {
            get
            {
                return Source == "AIMS" ? true : false;
            }
            set
            {
                Source = value ? "AIMS" : "IATI";
            }

        }
    }

    public class ProjectPreferenceModel : GeneralPreferencesModel
    {
        public string IATIIdentifier { get; set; }

    }
}
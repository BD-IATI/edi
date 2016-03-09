using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    public class ProjectFieldMapModel
    {
        public string OrgId { get; set; }
        public iatiactivity iatiActivity { get; set; }
        public iatiactivity aimsProject { get; set; }
        public List<FieldMap> Fields { get; set; }
        string financialDataSource;
        public string FinancialDataSource { get {return financialDataSource??"IATI" ;} set { financialDataSource = value; } }

        public ProjectFieldMapModel()
        {
            Fields = new List<FieldMap>();
        }
    }
    public class FieldMap
    {
        public string Field { get; set; }
        string source;
        public string Source { get { return source ?? "IATI"; } set { source = value; } }
        public string AIMSValue { get; set; }
        public string IATIValue { get; set; }

    }



    public class GeneralPreferencesModel
    {
        public string OrgId { get; set; }
        public string Field { get; set; }
        public string Source { get; set; }
        public string AIMSValue { get; set; }
        public string IATIValue { get; set; }
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
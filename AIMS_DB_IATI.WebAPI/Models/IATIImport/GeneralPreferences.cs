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
        //string financialDataSource;
        //public string FinancialDataSource { get {return financialDataSource??"IATI" ;} set { financialDataSource = value; } }

        public ProjectFieldMapModel(iatiactivity _iatiActivity, iatiactivity _aimsProject)
        {
            Fields = new List<FieldMap>();
            iatiActivity  = _iatiActivity;
            aimsProject = _aimsProject;

            if (iatiActivity != null && aimsProject != null)
            {
                Fields.Add(new FieldMap
                {
                    Field = "title",
                    AIMSValue = aimsProject.Title,
                    IATIValue = iatiActivity.Title,
                });
                Fields.Add(new FieldMap
                {
                    Field = "description",
                    AIMSValue = aimsProject.Description,
                    IATIValue = iatiActivity.Description,
                });
                Fields.Add(new FieldMap
                {
                    Field = "activitystatus",
                    AIMSValue = aimsProject.ActivityStatus,
                    IATIValue = iatiActivity.ActivityStatus,
                });

                Fields.Add(new FieldMap
                {
                    Field = "transaction",
                    AIMSValue = new { TotalCommitment = aimsProject.TotalCommitment, TotalDisbursment = aimsProject.TotalDisbursment },
                    IATIValue = new { TotalCommitment = iatiActivity.TotalCommitment, TotalDisbursment = iatiActivity.TotalDisbursment },
                });


            }

        }
    }

    public class FieldMap
    {
        public string Field { get; set; }
        string source;
        public string Source { get { return source ?? "IATI"; } set { source = value; } }
        public object AIMSValue { get; set; }
        public object IATIValue { get; set; }

    }

}
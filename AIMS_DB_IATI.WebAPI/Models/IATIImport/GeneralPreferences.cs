using AIMS_BD_IATI.DAL;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AIMS_BD_IATI.Library;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    public class ProjectFieldMapModel
    {
        //public string OrgId { get; set; }
        public bool IsManuallyMapped { get; set; }
        public iatiactivity iatiActivity { get; set; }
        public iatiactivity aimsProject { get; set; }
        public List<FieldMap> Fields { get; set; }
        public List<FieldMap> TransactionFields { get; set; }
        public ProjectFieldMapModel()
        {
            Fields = new List<FieldMap>();
            TransactionFields = new List<FieldMap>();
        }
        public ProjectFieldMapModel(iatiactivity _iatiActivity, iatiactivity _aimsProject)
            : this()
        {
            iatiActivity = _iatiActivity;
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

                //Transactions-------------------------------
                //TransactionFields.Add(new FieldMap
                //{
                //    Field = "transaction",
                //    AIMSValue = new { TotalCommitment = aimsProject.TotalCommitment, TotalDisbursment = aimsProject.TotalDisbursment },
                //    IATIValue = new { TotalCommitment = iatiActivity.TotalCommitment, TotalDisbursment = iatiActivity.TotalDisbursment },
                //});
                TransactionFields.Add(new FieldMap
                {
                    Field = "commitment",
                    AIMSValue = aimsProject.TotalCommitment,
                    IATIValue = iatiActivity.TotalCommitment
                });
                TransactionFields.Add(new FieldMap
                {
                    Field = "disbursment",
                    AIMSValue = aimsProject.TotalDisbursment,
                    IATIValue = iatiActivity.TotalDisbursment
                });
                TransactionFields.Add(new FieldMap
                {
                    Field = "planned-disbursment",
                    AIMSValue = aimsProject.TotalPlannedDisbursment,
                    IATIValue = iatiActivity.TotalPlannedDisbursment
                });
            }

        }

        public ProjectFieldMapModel(iatiactivity _iatiActivity, iatiactivity _aimsProject,
            List<FieldMappingPreferenceGeneral> generalPreferences)
            : this(_iatiActivity, _aimsProject)
        {
            foreach (var preference in generalPreferences)
            {
                Fields.Find(f => f.Field == preference.FieldName).n().IsSourceIATI = preference.IsSourceIATI;
            }
        }

        public ProjectFieldMapModel(iatiactivity _iatiActivity, iatiactivity _aimsProject,
            List<FieldMappingPreferenceActivity> activityPreferences)
            : this(_iatiActivity, _aimsProject)
        {
            foreach (var preference in activityPreferences)
            {
                Fields.Find(f => f.Field == preference.FieldName).n().IsSourceIATI = preference.IsSourceIATI;
            }
        }
    }

    public class FieldMap
    {
        public string Field { get; set; }
        public bool IsSourceIATI { get; set; }
        public object AIMSValue { get; set; }
        public object IATIValue { get; set; }
        public FieldMap()
        {
            IsSourceIATI = true;
        }
    }

}
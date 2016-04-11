using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.DAL
{
    public class ImportLogic
    {
        public static List<iatiactivity> LoadH1ActivitiesWithChild(List<iatiactivity> iatiActivities)
        {
            var H1Activities = iatiActivities.FindAll(f => f.n().hierarchy == 1);

            foreach (var H1Activity in H1Activities)
            {
                #region populate child activities
                H1Activity.childActivities.Clear();
                if (H1Activity.relatedactivity != null)
                {
                    foreach (var ra in H1Activity.relatedactivity.Where(r => r.type == "2"))
                    {
                        //load related activities
                        var ha = iatiActivities.Find(f => f.IatiIdentifier == ra.@ref);

                        if (ha != null)
                        {
                            H1Activity.childActivities.Add(ha);
                        }
                    }
                }
                #endregion

                #region To Resolve participating org
                var participatingOrgs = H1Activity.participatingorg.n().Where(w => w.role == "4").ToList();
                if (participatingOrgs.Count > 0)
                {
                    ///iOrgs.AddRange(participatingOrgs);
                }
                else if (H1Activity.childActivities.Count > 0)
                {
                    participatingorg dominatingParticipatingorg = null;
                    decimal highestCommitment = 0;
                    foreach (var relatedActivity in H1Activity.childActivities) // for h2Acts
                    {
                        participatingOrgs = relatedActivity.participatingorg.n().Where(w => w.role == "4").ToList();
                        ///iOrgs.AddRange(participatingOrgs);

                        //getting dominating participating org
                        var tc = relatedActivity.TotalCommitment;
                        if (tc > highestCommitment)
                        {
                            highestCommitment = tc;
                            dominatingParticipatingorg = participatingOrgs.FirstOrDefault();
                        }
                    }

                    //set dominating participating org to h1activity
                    if (dominatingParticipatingorg != null)
                    {
                        List<participatingorg> participatingorgs = H1Activity.participatingorg.n().ToList();
                        participatingorgs.Add(dominatingParticipatingorg);
                        H1Activity.participatingorg = participatingorgs.ToArray();
                    }

                }
                #endregion
            }
            return H1Activities;
        }

        public static List<iatiactivity> LoadH2ActivitiesWithParent(List<iatiactivity> iatiActivities)
        {
            var H2Activities = iatiActivities.FindAll(f => f.n().hierarchy == 2);

            foreach (var H2Activity in H2Activities)
            {
                #region To Resolve participating org
                var participatingOrgs = H2Activity.participatingorg.n().Where(w => w.role == "4").ToList();
                if (participatingOrgs.Count > 0)
                {
                    ///iOrgs.AddRange(participatingOrgs);
                }
                else if (H2Activity.HasParentActivity)
                {
                    var pa = H2Activity.relatedactivity.First(r => r.type == "1");
                    var pact = iatiActivities.Find(f => f.IatiIdentifier == pa.@ref);

                    if (pact != null)
                    {
                        participatingOrgs = pact.participatingorg.n().Where(w => w.role == "4").ToList();

                        ///iOrgs.AddRange(participatingOrgs);

                        //if child activity does not have implementing org then set it from parant activity
                        if (H2Activity.participatingorg != null)
                            participatingOrgs.AddRange(H2Activity.participatingorg);
                        H2Activity.participatingorg = participatingOrgs.ToArray();
                    }
                }
                #endregion


            }
            return H2Activities;
        }
        public static void SetFieldMappingPreferences(List<ProjectFieldMapModel> projectFieldMapModel, ProjectFieldMapModel generalPreferences)
        {
            //set general or activity preferences
            foreach (var mapModel in projectFieldMapModel)
            {

                var activityPreference = new AimsDbIatiDAL().GetFieldMappingPreferenceActivity(mapModel.iatiActivity.IatiIdentifier);

                var fields = mapModel.Fields.ToList();
                fields.AddRange(mapModel.TransactionFields);

                foreach (var field in fields)
                {
                    //get GetFieldMappingPreferenceActivity for this field
                    var activityFieldSource = activityPreference.Find(f => f.FieldName == field.Field);
                    if (activityFieldSource != null)
                    {
                        field.IsSourceIATI = activityFieldSource.IsSourceIATI;
                    }
                    else // apply general preferences
                    {
                        var generalFieldSource = generalPreferences.Fields.Find(f => f.Field == field.Field);
                        if (generalFieldSource != null)
                            field.IsSourceIATI = generalFieldSource.IsSourceIATI;

                    }
                }


            }
        }

        public static List<iatiactivity> MergeProjects(List<ProjectFieldMapModel> matchedProjects)
        {
            var margedProjects = new List<iatiactivity>();

            foreach (var matchedProject in matchedProjects)
            {
                matchedProject.aimsProject.FundSourceIDnIATICode = matchedProject.iatiActivity.FundSourceIDnIATICode;
                matchedProject.aimsProject.iatiidentifier = matchedProject.iatiActivity.iatiidentifier;

                foreach (var field in matchedProject.Fields)
                {
                    if (field.IsSourceIATI)
                    {
                        if (field.Field == IatiFields.Title)
                        {
                            matchedProject.aimsProject.Title = matchedProject.iatiActivity.Title;
                        }
                        else if (field.Field == IatiFields.Description)
                        {
                            matchedProject.aimsProject.Description = matchedProject.iatiActivity.Description;
                        }

                    }
                }

                var trns = new List<transaction>();
                var planDis = new List<planneddisbursement>();
                foreach (var field in matchedProject.TransactionFields)
                {
                    if (field.Field == IatiFields.Commitment)
                    {
                        if (field.IsSourceIATI)
                        {
                            trns.AddRange(matchedProject.iatiActivity.Commitments);
                            matchedProject.aimsProject.IsCommitmentIncluded = true;
                        }
                        else
                            trns.AddRange(matchedProject.aimsProject.Commitments);
                    }
                    else if (field.Field == IatiFields.Disbursment)
                    {
                        if (field.IsSourceIATI)
                        {
                            trns.AddRange(matchedProject.iatiActivity.Disbursments);
                            matchedProject.aimsProject.IsDisbursmentIncluded = true;
                        }
                        else
                            trns.AddRange(matchedProject.aimsProject.Disbursments);
                    }
                    else if (field.Field == IatiFields.PlannedDisbursment)
                    {
                        if (field.IsSourceIATI)
                        {
                            planDis.AddRange(matchedProject.iatiActivity.PlannedDisbursments);
                            matchedProject.aimsProject.IsPlannedDisbursmentIncluded = true;

                        }
                        else
                            planDis.AddRange(matchedProject.aimsProject.PlannedDisbursments);
                    }
                }

                matchedProject.aimsProject.transaction = trns.ToArray();
                matchedProject.aimsProject.planneddisbursement = planDis.ToArray();
                margedProjects.Add(matchedProject.aimsProject);
            }
            return margedProjects;
        }

    }

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
                    Field = IatiFields.Title,
                    AIMSValue = aimsProject.Title,
                    IATIValue = iatiActivity.Title,
                });
                Fields.Add(new FieldMap
                {
                    Field = IatiFields.Description,
                    AIMSValue = aimsProject.Description,
                    IATIValue = iatiActivity.Description,
                });
                Fields.Add(new FieldMap
                {
                    Field = IatiFields.Activitystatus,
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
                    Field = IatiFields.Commitment,
                    AIMSValue = aimsProject.TotalCommitmentThisDPOnly,
                    IATIValue = iatiActivity.TotalCommitmentThisDPOnly
                });
                TransactionFields.Add(new FieldMap
                {
                    Field = IatiFields.Disbursment,
                    AIMSValue = aimsProject.TotalDisbursmentThisDPOnly,
                    IATIValue = iatiActivity.TotalDisbursmentThisDPOnly
                });
                TransactionFields.Add(new FieldMap
                {
                    Field = IatiFields.PlannedDisbursment,
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
                TransactionFields.Find(f => f.Field == preference.FieldName).n().IsSourceIATI = preference.IsSourceIATI;
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

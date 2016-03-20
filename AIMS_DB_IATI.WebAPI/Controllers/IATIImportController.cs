using AIMS_BD_IATI.DAL;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MoreLinq;
using AIMS_DB_IATI.WebAPI.Models.IATIImport;

namespace AIMS_BD_IATI.WebAPI.Controllers
{
    [RoutePrefix("api/IATIImport")]
    [Authorize]
    public class IATIImportController : ApiController
    {
        public iatiactivityContainer s_activitiesContainer
        {
            get
            {
                return HttpContext.Current.Session["iatiactivityContainer"] == null ?
                    null
                    : (iatiactivityContainer)HttpContext.Current.Session["iatiactivityContainer"];
            }
            set { HttpContext.Current.Session["iatiactivityContainer"] = value; }
        }
        public HeirarchyModel s_heirarchyModel
        {
            get
            {
                return HttpContext.Current.Session["HeirarchyModel"] == null ?
                    null
                    : (HeirarchyModel)HttpContext.Current.Session["HeirarchyModel"];
            }
            set { HttpContext.Current.Session["HeirarchyModel"] = value; }
        }

        public ProjectFieldMapModel s_GeneralPreferences
        {
            get
            {
                return HttpContext.Current.Session["GeneralPreferences"] == null ?
                    null
                    : (ProjectFieldMapModel)HttpContext.Current.Session["GeneralPreferences"];
            }
            set { HttpContext.Current.Session["GeneralPreferences"] = value; }
        }

        public ProjectMapModel s_ProjectMapModel
        {
            get
            {
                return HttpContext.Current.Session["ProjectMapModel"] == null ?
                    null
                    : (ProjectMapModel)HttpContext.Current.Session["ProjectMapModel"];
            }
            set { HttpContext.Current.Session["ProjectMapModel"] = value; }
        }

        public List<FundSourceLookupItem> s_FundSources
        {
            get
            {
                return HttpContext.Current.Session["FundSources"] == null ?
                    new List<FundSourceLookupItem>()
                    : (List<FundSourceLookupItem>)HttpContext.Current.Session["FundSources"];
            }
            set { HttpContext.Current.Session["FundSources"] = value; }
        }


        [HttpGet]
        public List<DPLookupItem> GetFundSources()
        {
            return new AimsDAL().GetFundSourcesDropdownData();
        }

        [HttpGet]
        public List<FundSourceLookupItem> GetAllFundSources()
        {
            s_FundSources = new AimsDAL().GetAllFundSources();
            return s_FundSources;
        }


        [AcceptVerbs("GET", "POST")]
        public HeirarchyModel GetHierarchyData(DPLookupItem dp)
        {
            bool isDPChanged = s_activitiesContainer.n().DP != dp.ID;

            if (isDPChanged)
            {
                s_heirarchyModel = new HeirarchyModel();

                s_activitiesContainer = new AimsDbIatiDAL().GetActivities(dp.ID);

                if (s_activitiesContainer.HasRelatedActivity)
                {
                    var H1Acts = s_activitiesContainer.iatiActivities.FindAll(f => f.hierarchy == 1);
                    var H2Acts = s_activitiesContainer.iatiActivities.FindAll(f => f.hierarchy == 2);

                    var AimsProjects = s_activitiesContainer.AimsProjects;

                    var matchedH1 = (decimal)(from a in AimsProjects
                                              join i in H1Acts on a.IatiIdentifier equals i.IatiIdentifier
                                              select a).Count();

                    var matchedH2 = (decimal)(from a in AimsProjects
                                              join i in H2Acts on a.IatiIdentifier equals i.IatiIdentifier
                                              select a).Count();


                    s_heirarchyModel.H1Percent = H1Acts.Count > 0 ? Math.Round((decimal)(matchedH1 / H1Acts.Count) * 100, 2) : 0;
                    s_heirarchyModel.H2Percent = H2Acts.Count > 0 ? Math.Round((decimal)(matchedH2 / H2Acts.Count) * 100, 2) : 0;



                    #region Populate relatedActivities of the first activity as sample data
                    var parentActivities = s_activitiesContainer.iatiActivities.FindAll(x => x.hierarchy == 1);
                    foreach (var pa in parentActivities)
                    {
                        if (pa.relatedactivity != null)
                        {
                            foreach (var ra in pa.relatedactivity.Where(r => r.type == "2"))
                            {
                                //load related activities
                                var ha = s_activitiesContainer.iatiActivities.Find(f => f.iatiidentifier.Value == ra.@ref);

                                if (ha != null)
                                {
                                    pa.relatedIatiActivities.Add(ha);
                                }
                            }
                            s_heirarchyModel.SampleIatiActivity = pa;
                            break; //we have to show only one hierarchycal project as a sample
                        }
                    }
                    #endregion
                    s_heirarchyModel.SelectedHierarchy = 1;
                }
                else
                {
                    s_heirarchyModel = null;
                }
            }
            return s_heirarchyModel;
        }

        [HttpPost]
        public FilterBDModel SubmitHierarchy(HeirarchyModel heirarchyModel)
        {
            var returnResult = new FilterBDModel();

            if (heirarchyModel == null)
            {
                returnResult.iatiActivities = s_activitiesContainer.iatiActivities;
            }
            else
            {
                returnResult.iatiActivities = s_activitiesContainer.iatiActivities.FindAll(f => f.n().hierarchy == heirarchyModel.n().SelectedHierarchy);

                if (heirarchyModel.SelectedHierarchy == 1)
                {
                    foreach (var pa in returnResult.iatiActivities)
                    {
                        pa.relatedIatiActivities.Clear();
                        if (pa.relatedactivity != null)
                        {
                            foreach (var ra in pa.relatedactivity.Where(r => r.type == "2"))
                            {
                                //load related activities
                                var ha = s_activitiesContainer.iatiActivities.Find(f => f.iatiidentifier.Value == ra.@ref);

                                if (ha != null)
                                {
                                    pa.relatedIatiActivities.Add(ha);
                                }
                            }
                        }
                    }
                }

            }

            return returnResult;
        }

        [HttpPost]
        public object GetAllImplementingOrg(FilterBDModel filterDBModel)
        {
            if (filterDBModel != null)
                s_activitiesContainer.iatiActivities = filterDBModel.iatiActivities;

            var iOrgs = new List<participatingorg>();
            foreach (var activity in s_activitiesContainer.RelevantActivities)
            {
                var h1Acts = activity.participatingorg.n().Where(w => w.role == "4").ToList();
                if (h1Acts.Count > 0)
                {
                    iOrgs.AddRange(h1Acts);
                }
                else
                {
                    participatingorg dominatingParticipatingorg = null;
                    decimal highestCommitment = 0;
                    foreach (var relatedActivity in activity.relatedIatiActivities) // for h2Acts
                    {
                        var h2Acts = relatedActivity.participatingorg.n().Where(w => w.role == "4").ToList();
                        iOrgs.AddRange(h2Acts);

                        //getting dominating participating org
                        var tc = relatedActivity.TotalCommitment;
                        if (tc > highestCommitment)
                        {
                            highestCommitment = tc;
                            dominatingParticipatingorg = h2Acts.FirstOrDefault();
                        }
                    }

                    //set dominating participating org to h1activity
                    if (dominatingParticipatingorg != null)
                    {
                        List<participatingorg> participatingorgs = activity.participatingorg.n().ToList();
                        participatingorgs.Add(dominatingParticipatingorg);
                        activity.participatingorg = participatingorgs.ToArray();
                    }

                }
            }



            return new
            {
                Orgs = iOrgs.DistinctBy(l => l.@ref).OrderBy(o => o.@ref),
                FundSources = GetAllFundSources()
            };
        }

        [HttpPost]
        public List<iatiactivity> FilterDP(List<participatingorg> _iOrgs)
        {

            var iOrgs = new List<participatingorg>();
            s_activitiesContainer.RelevantActivities.ForEach(e => iOrgs.AddRange(e.participatingorg.n().Where(w => w.role == "4").ToList()));

            foreach (var iOrg in _iOrgs)
            {
                iOrgs.FindAll(f => f.@ref == iOrg.@ref).ForEach(e => e.FundSourceIDnIATICode = iOrg.FundSourceIDnIATICode);
            }

            return s_activitiesContainer.RelevantActivities;
        }

        [AcceptVerbs("GET", "POST")]
        public int? UpdateActivity(List<iatiactivity> activities)
        {
            return new AimsDbIatiDAL().AssignActivities(activities);
        }

        [AcceptVerbs("GET", "POST")]
        public ProjectMapModel SubmitActivities(List<iatiactivity> relevantActivies)
        {
            if (relevantActivies == null)
                relevantActivies = s_activitiesContainer.RelevantActivities;

            iatiactivity.FundSources = s_FundSources;

            var ProjectsOwnedByOther = relevantActivies.FindAll(f => f.IATICode != s_activitiesContainer.DP);

            relevantActivies.RemoveAll(f => f.IATICode != s_activitiesContainer.DP);

            var AimsProjects = new AimsDAL().GetAIMSDataInIATIFormat(s_activitiesContainer.n().DP);

            var MatchedProjects = (from i in relevantActivies
                                   from a in AimsProjects.Where(k => i.iatiidentifier.Value.EndsWith(k.iatiidentifier.Value))
                                   orderby i.iatiidentifier.Value

                                   select i).ToList();

            //for showing mathced projects side by side And field mapping later
            var MatchedProjects2 = (from i in relevantActivies
                                    from a in AimsProjects.Where(k => i.iatiidentifier.Value.EndsWith(k.iatiidentifier.Value))
                                    orderby i.iatiidentifier.Value
                                    select new ProjectFieldMapModel(i, a)
                                    ).ToList();

            var IatiActivityNotInAims = relevantActivies.Except(MatchedProjects).ToList();


            var AimsProjectNotInIati = AimsProjects.ExceptBy(MatchedProjects, f => f.iatiidentifier.Value).ToList();

            s_ProjectMapModel = new ProjectMapModel
            {
                MatchedProjects = MatchedProjects2,
                IatiActivitiesNotInAims = IatiActivityNotInAims,
                AimsProjectsNotInIati = AimsProjectNotInIati,
                NewProjectsToAddInAims = new List<iatiactivity>(),
                ProjectsOwnedByOther = ProjectsOwnedByOther
            };
            return s_ProjectMapModel;
        }

        [HttpGet]
        public ProjectFieldMapModel GetGeneralPreferences()
        {
            var savedPreferences = new AimsDbIatiDAL().GetFieldMappingPreferenceGeneral();

            var returnModel = (from a in s_ProjectMapModel.MatchedProjects
                               select new ProjectFieldMapModel(a.iatiActivity, a.aimsProject, savedPreferences)).FirstOrDefault();

            if (returnModel == null)
            {
                List<FieldMap> flds = new List<FieldMap>();
                foreach (var item in savedPreferences)
                {
                    FieldMap fld = new FieldMap
                    {
                        Field = item.FieldName,
                        IsSourceIATI = item.IsSourceIATI,
                        AIMSValue = "No matched projects",
                        IATIValue = "No matched activities"
                    };
                    flds.Add(fld);
                }
                returnModel = new ProjectFieldMapModel { Fields = flds };
            }
            s_GeneralPreferences = returnModel;

            return returnModel;
        }

        [AcceptVerbs("GET", "POST")]
        public int? SaveGeneralPreferences(ProjectFieldMapModel generalPreferences)
        {
            if (generalPreferences == null) return null;
            s_GeneralPreferences = generalPreferences;

            List<FieldMappingPreferenceGeneral> entities = new List<FieldMappingPreferenceGeneral>();

            foreach (var fieldMap in generalPreferences.Fields)
            {
                var entity = new FieldMappingPreferenceGeneral
                {
                    FieldName = fieldMap.Field,
                    OrgId = s_activitiesContainer.DP,
                    IsSourceIATI = fieldMap.IsSourceIATI
                };


                entities.Add(entity);
            }

            return new AimsDbIatiDAL().SaveFieldMappingPreferenceGeneral(entities);
        }

        [AcceptVerbs("GET", "POST")]
        public int? SaveActivityPreferences(ProjectFieldMapModel activityPreferences)
        {
            if (activityPreferences == null) return null;

            List<FieldMappingPreferenceActivity> entities = new List<FieldMappingPreferenceActivity>();

            foreach (var fieldMap in activityPreferences.Fields)
            {
                var entity = new FieldMappingPreferenceActivity
                {
                    FieldName = fieldMap.Field,
                    IATIIdentifier = activityPreferences.iatiActivity.IatiIdentifier,
                    //ProjectId = activityPreferences.aimsProject.ProjectId,
                    IsSourceIATI = fieldMap.IsSourceIATI
                };


                entities.Add(entity);
            }

            return new AimsDbIatiDAL().SaveFieldMappingPreferenceActivity(entities);
        }

        [HttpPost]
        public ProjectMapModel GetProjectsToMap(ProjectFieldMapModel GeneralPreference)
        {
            if (GeneralPreference != null)
                s_GeneralPreferences = GeneralPreference;

            foreach (var mapModel in s_ProjectMapModel.MatchedProjects)
            {
                var activityPreference = new AimsDbIatiDAL().GetFieldMappingPreferenceActivity(mapModel.iatiActivity.IatiIdentifier);
                foreach (var field in mapModel.Fields)
                {
                    //get GetFieldMappingPreferenceActivity for this field
                    var activityFieldSource = activityPreference.Find(f => f.FieldName == field.Field);
                    if (activityFieldSource != null)
                    {
                        field.IsSourceIATI = activityFieldSource.IsSourceIATI;
                    }
                    else // apply general preferences
                    {
                        var generalFieldSource = s_GeneralPreferences.Fields.Find(f => f.Field == field.Field);
                        if (generalFieldSource != null)
                            field.IsSourceIATI = generalFieldSource.IsSourceIATI;

                    }
                }

            }


            return new ProjectMapModel
            {
                MatchedProjects = s_ProjectMapModel.MatchedProjects,
                IatiActivitiesNotInAims = null,
                AimsProjectsNotInIati = null,
                NewProjectsToAddInAims = null,
                ProjectsOwnedByOther = null
            };
        }

        [HttpGet]
        public object GetAssignedActivities(string dp)
        {
            //var projects = new AimsDAL().GetProjects(dp);
            var projects = new AimsDAL().GetAIMSDataInIATIFormat(dp);
            var assignedActivities = new AimsDbIatiDAL().GetAssignActivities(dp);
            var trustFunds = new AimsDAL().GetTrustFunds(dp);
            return new
            {
                AssignedActivities = assignedActivities,
                Projects = projects,
                TrustFunds = trustFunds
            };
        }

        [HttpGet]
        public List<transaction> GetTrustFundDetails(int trustFundId)
        {
            return new AimsDAL().GetTrustFundDetails(trustFundId);

        }
        [AcceptVerbs("GET", "POST")]
        public int? ImportProjects(ProjectMapModel projectMapModel)
        {
            var matchedProjects = projectMapModel.MatchedProjects;

            var margedProjects = new List<iatiactivity>();

            foreach (var matchedProject in matchedProjects)
            {
                foreach (var field in matchedProject.Fields)
                {
                    if (field.IsSourceIATI)
                    {
                        if (field.Field == "title")
                        {
                            matchedProject.aimsProject.Title = matchedProject.iatiActivity.Title;
                        }
                        if (field.Field == "description")
                        {
                            matchedProject.aimsProject.Description = matchedProject.iatiActivity.Description;
                        }
                    }
                }
                margedProjects.Add(matchedProject.aimsProject);
            }

            return new AimsDAL().UpdateProjects(margedProjects);
        }

    }


}


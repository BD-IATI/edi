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
using AIMS_DB_IATI.WebAPI.Models;

namespace AIMS_BD_IATI.WebAPI.Controllers
{
    [RoutePrefix("api/IATIImport")]
    [Authorize]
    public class IATIImportController : ApiController
    {
        AimsDAL aimsDAL = new AimsDAL();
        AimsDbIatiDAL aimsDbIatiDAL = new AimsDbIatiDAL();

        void SetStatics()
        {
            iatiactivity.FundSources = Sessions.FundSources;
        }

        [HttpGet]
        public List<LookupItem> GetExecutingAgencyTypes()
        {
            Sessions.ExecutingAgencyTypes = aimsDAL.GetExecutingAgencyTypes();

            return Sessions.ExecutingAgencyTypes;
        }

        [HttpGet]
        public List<DPLookupItem> GetFundSources()
        {
            Sessions.FundSources = aimsDAL.GetAllFundSources();

            return aimsDAL.GetFundSources(Sessions.UserId);
        }

        [HttpGet]
        public List<FundSourceLookupItem> GetAllFundSources()
        {
            return Sessions.FundSources;
        }

        [AcceptVerbs("GET", "POST")]
        public HeirarchyModel GetHierarchyData(DPLookupItem dp)
        {
            //bool isDPChanged = Sessions.activitiesContainer.n().DP != dp.n().ID;

            //if (isDPChanged)
            //{

            Sessions.activitiesContainer = aimsDbIatiDAL.GetNotMappedActivities(dp.ID);

            Sessions.heirarchyModel = CalculateHierarchyMatching();

            //}
            return Sessions.heirarchyModel;
        }

        private static HeirarchyModel CalculateHierarchyMatching()
        {
            Sessions.heirarchyModel = new HeirarchyModel();
            if (Sessions.activitiesContainer.HasChildActivity)
            {
                var H1Acts = Sessions.activitiesContainer.iatiActivities.FindAll(f => f.hierarchy == 1);
                var H2Acts = Sessions.activitiesContainer.iatiActivities.FindAll(f => f.hierarchy == 2);

                var AimsProjects = Sessions.activitiesContainer.AimsProjects;

                var matchedH1 = (decimal)(GetMatchedProjects(H1Acts, AimsProjects)).Count();

                var matchedH2 = (decimal)(GetMatchedProjects(H2Acts, AimsProjects)).Count();


                Sessions.heirarchyModel.H1Percent = H1Acts.Count > 0 ? Math.Round((decimal)(matchedH1 / H1Acts.Count) * 100, 2) : 0;
                Sessions.heirarchyModel.H2Percent = H2Acts.Count > 0 ? Math.Round((decimal)(matchedH2 / H2Acts.Count) * 100, 2) : 0;



                #region Populate relatedActivities of the first activity as sample data
                var parentActivities = Sessions.activitiesContainer.iatiActivities.FindAll(x => x.hierarchy == 1);
                foreach (var pa in parentActivities)
                {
                    if (pa.relatedactivity != null)
                    {
                        foreach (var ra in pa.relatedactivity.Where(r => r.type == "2"))
                        {
                            //load related activities
                            var ha = Sessions.activitiesContainer.iatiActivities.Find(f => f.IatiIdentifier == ra.@ref);

                            if (ha != null)
                            {
                                pa.childActivities.Add(ha);
                            }
                        }
                        Sessions.heirarchyModel.SampleIatiActivity = pa;
                        break; //we have to show only one hierarchycal project as a sample
                    }
                }
                #endregion

                Sessions.heirarchyModel.SelectedHierarchy = Sessions.heirarchyModel.H1Percent > Sessions.heirarchyModel.H2Percent ? 1 : 2;
            }
            else
            {
                Sessions.heirarchyModel = null;
            }

            return Sessions.heirarchyModel;
        }

        [AcceptVerbs("GET", "POST")]
        public FilterBDModel SubmitHierarchy(HeirarchyModel heirarchyModel)
        {
            var returnResult = new FilterBDModel();

            if (heirarchyModel == null)
            {
                returnResult.iatiActivities = Sessions.activitiesContainer.iatiActivities;
            }
            else
            {

                if (heirarchyModel.SelectedHierarchy == 1)
                {
                    returnResult.iatiActivities = ImportLogic.LoadH1ActivitiesWithChild(Sessions.activitiesContainer.iatiActivities);
                }
                else
                {
                    returnResult.iatiActivities = ImportLogic.LoadH2ActivitiesWithParent(Sessions.activitiesContainer.iatiActivities);
                }

            }

            returnResult.iatiActivities = returnResult.iatiActivities.OrderByDescending(k => k.IsRelevant).ToList();

            return returnResult;
        }

        [AcceptVerbs("GET", "POST")]
        public iOrgs GetAllImplementingOrg(FilterBDModel filterDBModel)
        {

            var managingDPs = GetAllFundSources();

            var iOrgs = new List<participatingorg>();
            foreach (var activity in Sessions.activitiesContainer.iatiActivities)
            {
                var participatingOrgs = activity.ImplementingOrgs;

                iOrgs.AddRange(participatingOrgs);

            }

            var distictOrgs = iOrgs.DistinctBy(l => l.narrative.n(0).Value).OrderBy(o => o.narrative.n(0).Value);

            var exAgencies = aimsDAL.GetExecutingAgencies();

            foreach (var org in distictOrgs)
            {
                //check for matching managing DP from AIMS
                var managingDP = !string.IsNullOrWhiteSpace(org.@ref) ? managingDPs.FirstOrDefault(q => q.IATICode != null && q.IATICode.n().Contains(org.@ref)) : null;

                //if not found, set to Current DP
                if (managingDP == null)
                    managingDP = managingDPs.FirstOrDefault(q => q.IATICode != null && q.IATICode.n().Contains(Sessions.DP.ID));

                //Add selected value
                org.FundSourceIDnIATICode = managingDP == null ? "" : managingDP.IDnIATICode;

                //try to set executing agency
                ExecutingAgencyLookupItem agencyGuessed = null;
                int minDistance = 1000;
                foreach (var agency in exAgencies)
                {
                    int distance = Levenshtein.iLD(org.Name, agency.Name);
                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        agencyGuessed = agency;
                    }

                }
                if (minDistance < 7 && agencyGuessed != null)
                {
                    org.AllID = agencyGuessed.AllID;
                    org.ExecutingAgencyTypeId = agencyGuessed.ExecutingAgencyTypeId;
                }
            }

            if (filterDBModel != null)
                Sessions.activitiesContainer.iatiActivities = filterDBModel.iatiActivities;




            return new iOrgs
            {
                Orgs = distictOrgs.ToList(),
                FundSources = managingDPs,
                ExecutingAgencyTypes = aimsDAL.GetExecutingAgencyTypes(),
                ExecutingAgencies = exAgencies

            };
        }

        [AcceptVerbs("GET", "POST")]
        public List<iatiactivity> FilterDP(List<participatingorg> _iOrgs)
        {

            var projectsImpOrgs = new List<participatingorg>();
            Sessions.activitiesContainer.RelevantActivities.ForEach(e => projectsImpOrgs.AddRange(e.ImplementingOrgs));

            foreach (var iOrg in _iOrgs)
            {
                projectsImpOrgs.FindAll(f => f.@ref == iOrg.@ref).ForEach(e => e.FundSourceIDnIATICode = iOrg.FundSourceIDnIATICode);
            }

            return Sessions.activitiesContainer.RelevantActivities;
        }

        [AcceptVerbs("GET", "POST")]
        public int? AssignActivities(List<iatiactivity> activities)
        {
            return aimsDbIatiDAL.AssignActivities(activities);
        }

        [AcceptVerbs("GET", "POST")]
        public ProjectMapModel SubmitActivities(List<iatiactivity> relevantActivies)
        {
            if (relevantActivies == null)
                relevantActivies = Sessions.activitiesContainer.RelevantActivities;

            SetStatics();//since we have no access to session at library project, so we pass it in a static variables

            var ProjectsOwnedByOther = relevantActivies.FindAll(f => f.IATICode != Sessions.DP.ID);

            relevantActivies.RemoveAll(f => f.IATICode != Sessions.DP.ID);

            var AimsProjects = Sessions.activitiesContainer.AimsProjects;

            var MatchedProjects = (GetMatchedProjects(relevantActivies, AimsProjects)).ToList();

            //for showing mathced projects side by side And field mapping later
            var MatchedProjects2 = (from i in relevantActivies
                                    from a in AimsProjects.Where(k => i.IatiIdentifier.Replace("-", "").EndsWith(k.IatiIdentifier.Replace("-", "")))
                                    orderby i.IatiIdentifier
                                    select new ProjectFieldMapModel(i, a)
                                    ).ToList();

            var IatiActivityNotInAims = relevantActivies.Except(MatchedProjects).ToList();


            var AimsProjectNotInIati = AimsProjects.ExceptBy(MatchedProjects, f => f.IatiIdentifier).ToList();

            Sessions.ProjectMapModel = new ProjectMapModel
            {
                MatchedProjects = MatchedProjects2,
                IatiActivitiesNotInAims = IatiActivityNotInAims,
                AimsProjectsNotInIati = AimsProjectNotInIati,
                NewProjectsToAddInAims = new List<iatiactivity>(),
                ProjectsOwnedByOther = ProjectsOwnedByOther
            };
            return Sessions.ProjectMapModel;
        }

        private static IEnumerable<iatiactivity> GetMatchedProjects(List<iatiactivity> relevantActivies, List<iatiactivity> AimsProjects)
        {
            return from i in relevantActivies
                   from a in AimsProjects.Where(k => i.IatiIdentifier.Replace("-", "").EndsWith(k.IatiIdentifier.Replace("-", "")))
                   orderby i.IatiIdentifier

                   select i;
        }

        [AcceptVerbs("GET", "POST")]
        public bool SubmitManualMatching(ProjectMapModel projectMapModel)
        {
            Sessions.ProjectMapModel.AimsProjectsNotInIati = projectMapModel?.AimsProjectsNotInIati;

            Sessions.ProjectMapModel.MatchedProjects.RemoveAll(r => r.IsManuallyMapped);

            //add manually matched projects
            foreach (var project in Sessions.ProjectMapModel?.AimsProjectsNotInIati)
            {
                if (project.MatchedProjects.Count > 0)
                {
                    Sessions.ProjectMapModel.MatchedProjects.Add(new ProjectFieldMapModel(project.MatchedProjects.First(), project) { IsManuallyMapped = true });
                }
            }

            foreach (var project in projectMapModel?.NewProjectsToAddInAims)
            {
                project.IsCommitmentIncluded = true;
                project.IsDisbursmentIncluded = true;
                project.IsPlannedDisbursmentIncluded = true;

                Sessions.ProjectMapModel.NewProjectsToAddInAims.Add(project);
            }

            return true;
        }

        [HttpGet]
        public ProjectFieldMapModel GetGeneralPreferences()
        {
            var savedPreferences = aimsDbIatiDAL.GetFieldMappingPreferenceGeneral(Sessions.DP.ID);

            var returnModel = (from a in Sessions.ProjectMapModel.n().MatchedProjects.n()
                               select new ProjectFieldMapModel(a.iatiActivity, a.aimsProject, savedPreferences)).FirstOrDefault();

            if (returnModel == null)
            {
                returnModel = new ProjectFieldMapModel(Sessions.activitiesContainer.n().RelevantActivities.n(0), new iatiactivity(), savedPreferences);
                //foreach (var item in returnModel.Fields)
                //{
                //    item.AIMSValue = "Not found in AIMS";
                //}
            }
            Sessions.GeneralPreferences = returnModel;

            return returnModel;
        }

        [AcceptVerbs("GET", "POST")]
        public int? SaveGeneralPreferences(ProjectFieldMapModel generalPreferences)
        {
            if (generalPreferences == null) return null;
            Sessions.GeneralPreferences = generalPreferences;

            List<FieldMappingPreferenceGeneral> entities = new List<FieldMappingPreferenceGeneral>();

            var fields = generalPreferences.Fields;
            fields.AddRange(generalPreferences.TransactionFields);

            foreach (var fieldMap in fields)
            {
                var entity = new FieldMappingPreferenceGeneral
                {
                    FieldName = fieldMap.Field,
                    OrgId = Sessions.DP.ID,
                    IsSourceIATI = fieldMap.IsSourceIATI
                };


                entities.Add(entity);
            }

            return aimsDbIatiDAL.SaveFieldMappingPreferenceGeneral(entities);
        }

        [AcceptVerbs("GET", "POST")]
        public int? SaveActivityPreferences(ProjectFieldMapModel activityPreferences)
        {
            if (activityPreferences == null) return null;

            List<FieldMappingPreferenceActivity> entities = new List<FieldMappingPreferenceActivity>();

            var fields = activityPreferences.Fields;
            fields.AddRange(activityPreferences.TransactionFields);

            foreach (var fieldMap in fields)
            {
                var entity = new FieldMappingPreferenceActivity
                {
                    FieldName = fieldMap.Field,
                    IatiIdentifier = activityPreferences.iatiActivity.IatiIdentifier,
                    //ProjectId = activityPreferences.aimsProject.ProjectId,
                    IsSourceIATI = fieldMap.IsSourceIATI
                };


                entities.Add(entity);
            }

            return aimsDbIatiDAL.SaveFieldMappingPreferenceActivity(entities);
        }

        [HttpPost]
        public ProjectMapModel GetProjectsToMap(ProjectFieldMapModel GeneralPreference)
        {
            if (GeneralPreference != null)
                Sessions.GeneralPreferences = GeneralPreference;
            else
                Sessions.GeneralPreferences = GetGeneralPreferences();

            if (Sessions.ProjectMapModel.MatchedProjects.IsEmpty() && Sessions.ProjectMapModel.NewProjectsToAddInAims.IsEmpty())
            {

                Sessions.activitiesContainer = aimsDbIatiDAL.GetAllActivities(Sessions.DP.ID);
                var heirarchyModel = CalculateHierarchyMatching();

                var filterBDModel = SubmitHierarchy(heirarchyModel);
                var iOrgs = GetAllImplementingOrg(filterBDModel);
                var relevantActivities = FilterDP(iOrgs.Orgs);
                var projectMapModel = SubmitActivities(relevantActivities);

            }

            ImportLogic.SetFieldMappingPreferences(Sessions.ProjectMapModel.MatchedProjects, Sessions.GeneralPreferences);

            return new ProjectMapModel
            {
                MatchedProjects = Sessions.ProjectMapModel.MatchedProjects,
                IatiActivitiesNotInAims = null,
                AimsProjectsNotInIati = null,
                NewProjectsToAddInAims = Sessions.ProjectMapModel.NewProjectsToAddInAims,
                ProjectsOwnedByOther = null
            };
        }


        [AcceptVerbs("GET", "POST")]
        public int? ImportProjects(ProjectMapModel projectMapModel)
        {
            var matchedProjects = projectMapModel.MatchedProjects;

            var margedProjects = ImportLogic.MergeProjects(matchedProjects);

            foreach (var item in projectMapModel.NewProjectsToAddInAims)
            {
                item.FundSourceIDnIATICode = Sessions.DP.IDnIATICode;
                margedProjects.Add(item);

            }

            return aimsDAL.UpdateProjects(margedProjects, Sessions.UserId);
        }


        [AcceptVerbs("GET", "POST")]
        public ProjectFieldMapModel GetMatchedProjectByIatiIdentifier(string iatiIdentifier)
        {
            ProjectFieldMapModel ProjectFieldMapModel = aimsDbIatiDAL.GetTransactionMismatchedActivity(iatiIdentifier);

            return ProjectFieldMapModel;
        }

    }

    public class iOrgs
    {
        public List<participatingorg> Orgs { get; set; }
        public List<FundSourceLookupItem> FundSources { get; set; }
        public List<LookupItem> ExecutingAgencyTypes { get; internal set; }
        public object ExecutingAgencies { get; internal set; }
    }


}


using AIMS_BD_IATI.DAL;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MoreLinq;
using AIMS_DB_IATI.WebAPI.Models.IATIImport;
using AIMS_DB_IATI.WebAPI.Models;
using System.Reflection;

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
            if (Sessions.FundSources.Count == 0)
                Sessions.FundSources = aimsDAL.GetAllFundSources();

            iatiactivity.FundSources = Sessions.FundSources;
        }
        public IATIImportController()
        {
            SetStatics();//since we have no access to session at library project, so we pass it in a static variables

        }
        #region Dropdown Load
        [HttpGet]
        public List<LookupItem> GetExecutingAgencyTypes()
        {
            Sessions.ExecutingAgencyTypes = aimsDAL.GetExecutingAgencyTypes();

            return Sessions.ExecutingAgencyTypes;
        }

        [HttpGet]
        public List<DPLookupItem> GetFundSources()
        {
            return aimsDAL.GetFundSources(Sessions.UserId);
        }

        [HttpGet]
        public List<ExecutingAgencyLookupItem> GetAllFundSources()
        {
            return Sessions.FundSources;
        }
        #endregion

        #region HierarchyData
        [AcceptVerbs("GET", "POST")]
        public HeirarchyModel GetHierarchyData(DPLookupItem dp)
        {
            //bool isDPChanged = Sessions.activitiesContainer??.DP != dp?.ID;

            //if (isDPChanged)
            //{
            dp = dp ?? Sessions.DP;

            Sessions.activitiesContainer = aimsDbIatiDAL.GetNotMappedActivities(Sessions.DP.ID);

            var returnResult = CalculateHierarchyMatching();
            //}
            Sessions.heirarchyModel = returnResult;

            return returnResult;
        }

        private static HeirarchyModel CalculateHierarchyMatching()
        {
            var returnResult = new HeirarchyModel();
            if (Sessions.activitiesContainer?.HasChildActivity == true)
            {
                var H1Acts = Sessions.activitiesContainer?.iatiActivities.FindAll(f => f.hierarchy == 1);
                var H2Acts = Sessions.activitiesContainer?.iatiActivities.FindAll(f => f.hierarchy == 2);

                var AimsProjects = Sessions.activitiesContainer?.AimsProjects;

                var matchedH1 = (decimal)(GetMatchedProjects(H1Acts, AimsProjects)).Count();

                var matchedH2 = (decimal)(GetMatchedProjects(H2Acts, AimsProjects)).Count();


                returnResult.H1Percent = H1Acts.Count > 0 ? Math.Round((decimal)(matchedH1 / H1Acts.Count) * 100, 2) : 0;
                returnResult.H2Percent = H2Acts.Count > 0 ? Math.Round((decimal)(matchedH2 / H2Acts.Count) * 100, 2) : 0;



                #region Populate relatedActivities of the first activity as sample data
                var parentActivities = Sessions.activitiesContainer?.iatiActivities.FindAll(x => x.hierarchy == 1);
                foreach (var pa in parentActivities)
                {
                    if (pa.relatedactivity != null)
                    {
                        foreach (var ra in pa.relatedactivity.Where(r => r.type == "2"))
                        {
                            //load related activities
                            var ha = Sessions.activitiesContainer?.iatiActivities.Find(f => f.IatiIdentifier == ra.@ref);

                            if (ha != null)
                            {
                                pa.childActivities.Add(ha);
                            }
                        }
                        returnResult.SampleIatiActivity = pa;
                        break; //we have to show only one hierarchycal project as a sample
                    }
                }
                #endregion

                returnResult.SelectedHierarchy = returnResult.H1Percent >= returnResult.H2Percent ? 1 : 2;

                Sessions.CurrentStage = Stage.Hierarchy;

            }
            else
            {
                returnResult = null;
                Sessions.CurrentStage = Stage.FilterBD;
            }

            return returnResult;
        }
        #endregion


        #region FilterBD
        [AcceptVerbs("GET", "POST")]
        public FilterBDModel SubmitHierarchy(HeirarchyModel heirarchyModel)
        {
            if (Sessions.activitiesContainer.IsHierarchyLoaded == true)
                return null;

            var returnResult = new FilterBDModel();
            heirarchyModel = heirarchyModel ?? Sessions.heirarchyModel;
            if (heirarchyModel == null)
            {
                returnResult.iatiActivities = ToMinifiedIatiActivitiesModel(Sessions.activitiesContainer?.iatiActivities);
            }
            else
            {

                if (heirarchyModel.SelectedHierarchy == 1)
                {
                    Sessions.activitiesContainer.iatiActivities = ImportLogic.LoadH1ActivitiesWithChild(Sessions.activitiesContainer?.iatiActivities);
                    returnResult.iatiActivities = ToMinifiedIatiActivitiesModel(Sessions.activitiesContainer.iatiActivities, false, true);
                }
                else
                {
                    Sessions.activitiesContainer.iatiActivities = ImportLogic.LoadH2ActivitiesWithParent(Sessions.activitiesContainer?.iatiActivities);
                    returnResult.iatiActivities = ToMinifiedIatiActivitiesModel(Sessions.activitiesContainer.iatiActivities);
                }
                Sessions.activitiesContainer.IsHierarchyLoaded = true;
            }

            returnResult.iatiActivities = returnResult.iatiActivities.OrderByDescending(k => k.IsRelevant).ToList();

            Sessions.CurrentStage = Stage.FilterBD;
            Sessions.heirarchyModel = heirarchyModel;
            Sessions.filterBDModel = returnResult;
            return returnResult;
        }

        #endregion

        #region FilterDP
        [AcceptVerbs("GET", "POST")]
        public iOrgs GetAllImplementingOrg(FilterBDModel filterDBModel)
        {
            if (filterDBModel != null)
            {
                Sessions.filterBDModel.iatiActivities = filterDBModel.iatiActivities;
                UpdateActivities(filterDBModel.iatiActivities, Sessions.activitiesContainer.iatiActivities);
            }

            //actual method begins here
            var managingDPs = GetAllFundSources();

            var iOrgs = new List<participatingorg>();
            foreach (var activity in Sessions.activitiesContainer?.RelevantActivities)
            {
                var participatingOrgs = activity.ImplementingOrgs ?? new List<participatingorg>();

                iOrgs.AddRange(participatingOrgs);

            }

            var distictOrgs = iOrgs.DistinctBy(l => l.Name).OrderBy(o => o.Name);

            var exAgencies = aimsDAL.GetExecutingAgencies();

            foreach (var org in distictOrgs)
            {
                //check for matching managing DP from AIMS
                var managingDP = !string.IsNullOrWhiteSpace(org.@ref) ? managingDPs.FirstOrDefault(q => q.IATICode != null && q.IATICode.Contains(org.@ref)) : null;

                //Add selected value
                org.AllID = managingDP == null ? Sessions.DP.AllID : managingDP.AllID;

            }


            Sessions.CurrentStage = Stage.FilterDP;

            var returtResult = new iOrgs
            {
                Orgs = distictOrgs.ToList(),
                FundSources = managingDPs,
                ExecutingAgencyTypes = aimsDAL.GetExecutingAgencyTypes(),
                ExecutingAgencies = exAgencies

            };
            Sessions.iOrgs = returtResult;
            return returtResult;
        }

        [AcceptVerbs("GET", "POST")]
        public List<iatiactivityModel> FilterDP(List<participatingorg> _iOrgs)
        {
            if (_iOrgs == null) _iOrgs = Sessions.iOrgs.Orgs;
            var relevantActivities = Sessions.activitiesContainer?.RelevantActivities;
            var projectsImpOrgs = new List<participatingorg>();
            relevantActivities?.ForEach(e =>
            {
                if (e.ImplementingOrgs != null) projectsImpOrgs.AddRange(e.ImplementingOrgs);
                e.AllID = null;
            });

            foreach (var iOrg in _iOrgs)
            {
                projectsImpOrgs.FindAll(f => f.@ref == iOrg.@ref).ForEach(e =>
                {
                    e.AllID = iOrg.AllID;
                    var aimsName = Sessions.iOrgs.ExecutingAgencies.FirstOrDefault(f => f.AllID != Sessions.DP.AllID && f.AllID == iOrg.AllID)?.Name;
                    e.AimsName = aimsName;
                });
                projectsImpOrgs.FindAll(f => f.Name == iOrg.Name).ForEach(e =>
                {
                    e.AllID = iOrg.AllID;

                    var aimsName = Sessions.iOrgs.ExecutingAgencies.FirstOrDefault(f =>f.AllID != Sessions.DP.AllID &&  f.AllID == iOrg.AllID)?.Name;
                    e.AimsName = aimsName;
                });
            }

            relevantActivities?.ForEach(e =>
            {
                if (string.IsNullOrWhiteSpace(e.AllID) || !Sessions.iOrgs.FundSources.Exists(d => d.AllID == e.AllID)) e.AllID = Sessions.DP.AllID;

            });

            Sessions.CurrentStage = Stage.FilterDP;
            Sessions.activitiesContainer.iatiActivities = relevantActivities;
            return ToMinifiedIatiActivitiesModel(relevantActivities);
        }

        [AcceptVerbs("GET", "POST")]
        public int? AssignActivities(List<iatiactivityModel> activities)
        {
            return aimsDbIatiDAL.AssignActivities(activities);
        }

        #endregion

        #region ShowProjects
        [AcceptVerbs("GET", "POST")]
        public ProjectMapModelMinified SubmitActivities(List<iatiactivityModel> relevantActivies)
        {
            if (relevantActivies != null)
            {
                UpdateActivities(relevantActivies, Sessions.activitiesContainer?.RelevantActivities);
            }
            var relevantActiviesSession = Sessions.activitiesContainer?.RelevantActivities;

            //SetStatics();//since we have no access to session at library project, so we pass it in a static variables

            var ProjectsOwnedByOther = relevantActiviesSession.FindAll(f => f.IATICode != Sessions.DP.ID);

            relevantActiviesSession.RemoveAll(f => f.IATICode != Sessions.DP.ID);

            var AimsProjects = Sessions.activitiesContainer?.AimsProjects;

            var MatchedProjects = (GetMatchedProjects(relevantActiviesSession, AimsProjects)).ToList();

            //for showing mathced projects side by side And field mapping later
            var MatchedProjects2 = (from i in relevantActiviesSession
                                    from a in AimsProjects.Where(k => i.IatiIdentifier.Replace("-", "").EndsWith(k.IatiIdentifier.Replace("-", "")) || i.IatiIdentifier.Contains(k.IatiIdentifier))
                                    orderby i.IatiIdentifier
                                    select new ProjectFieldMapModel(i, a)
                                    ).ToList();

            var IatiActivityNotInAims = relevantActiviesSession.Except(MatchedProjects).ToList();


            var AimsProjectNotInIati = AimsProjects
                .ExceptBy(MatchedProjects, f => f.IatiIdentifier)
                .ExceptBy(ProjectsOwnedByOther, f => f.IatiIdentifier)
                .ToList();

            var returnResult = new ProjectMapModel
            {
                MatchedProjects = MatchedProjects2,
                IatiActivitiesNotInAims = IatiActivityNotInAims,
                AimsProjectsNotInIati = AimsProjectNotInIati,
                NewProjectsToAddInAims = new List<iatiactivity>(),
                ProjectsOwnedByOther = ProjectsOwnedByOther
            };
            Sessions.ProjectMapModel = returnResult;

            Sessions.CurrentStage = Stage.ShowProjects;

            var r = ToMinifiedProjectMapModel(returnResult);

            r.AimsProjectsDrpSrc = (from p in AimsProjects
                                    select new LookupItem
                                    {
                                        ID = p.ProjectId,
                                        Name = p.Title
                                    }).ToList();
            return r;
        }

        #endregion

        #region Match
        [AcceptVerbs("GET", "POST")]
        public bool SubmitManualMatchingUsingDropdown(ProjectMapModelMinified projectMapModel)
        {
            if (projectMapModel != null)
            {
                UpdateActivities(projectMapModel.IatiActivitiesNotInAims, Sessions.ProjectMapModel.IatiActivitiesNotInAims);

                //Sessions.ProjectMapModel.AimsProjectsNotInIati = projectMapModel.AimsProjectsNotInIati;

                Sessions.ProjectMapModel.MatchedProjects.RemoveAll(r => r.IsManuallyMapped);

            }

            //actual method starts here :)

            //add manually matched projects
            var aimsProjects = Sessions.activitiesContainer?.AimsProjects;
            var iatiActivities = Sessions.ProjectMapModel?.IatiActivitiesNotInAims;
            foreach (var project in aimsProjects)
            {
                var mappedActivityCount = iatiActivities.Count(c => c.ProjectId == project.ProjectId);
                if (mappedActivityCount == 1)
                {
                    var m = new ProjectFieldMapModel(iatiActivities.Find(c => c.ProjectId == project.ProjectId), project, false) { IsManuallyMapped = true };

                    Sessions.ProjectMapModel.MatchedProjects.Add(m);

                }
                else if (mappedActivityCount > 1)
                {
                    iatiactivity groupedActivity = MergeToSingleActivity(iatiActivities.FindAll(c => c.ProjectId == project.ProjectId));

                    Sessions.ProjectMapModel.MatchedProjects.Add(new ProjectFieldMapModel(groupedActivity, project, false) { IsManuallyMapped = true, IsGrouped = true });

                }

            }

            //foreach (var activity in Sessions.ProjectMapModel?.IatiActivitiesNotInAims.Where(w => w.ProjectId > 0))
            //{
            //    var project = aimsProjects.Find(f => f.ProjectId == activity.ProjectId);

            //    if (project != null)
            //    {
            //        Sessions.ProjectMapModel.MatchedProjects.Add(new ProjectFieldMapModel(activity, project) { IsManuallyMapped = true });
            //    }
            //}

            foreach (var activity in Sessions.ProjectMapModel?.IatiActivitiesNotInAims.Where(w => w.ProjectId == -2))
            {
                activity.IsCommitmentIncluded = true;
                activity.IsDisbursmentIncluded = true;
                activity.IsPlannedDisbursmentIncluded = true;

                Sessions.ProjectMapModel.NewProjectsToAddInAims.Add(activity);

            }

            return true;
        }

        private static iatiactivity MergeToSingleActivity(List<iatiactivity> iatiActivities)
        {
            var groupedActivity = new iatiactivity();

            var trns = new List<transaction>();
            var bgts = new List<budget>();
            var plnDis = new List<planneddisbursement>();

            foreach (var activity in iatiActivities)
            {
                trns.AddRange(activity.transaction);
                bgts.AddRange(activity.budget);
                plnDis.AddRange(activity.planneddisbursement);
            }
            groupedActivity.transaction = trns.ToArray();
            groupedActivity.budget = bgts.ToArray();
            groupedActivity.planneddisbursement = plnDis.ToArray();

            return groupedActivity;
        }

        //unused method > used for drag and drop before
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

        #endregion

        #region Preferences
        [HttpGet]
        public ProjectFieldMapModel GetGeneralPreferences()
        {
            var savedPreferences = aimsDbIatiDAL.GetFieldMappingPreferenceGeneral(Sessions.DP.ID);

            var returnModel = (from a in Sessions.ProjectMapModel?.MatchedProjects ?? new List<ProjectFieldMapModel>()
                               select new ProjectFieldMapModel(a.iatiActivity, a.aimsProject, savedPreferences)).FirstOrDefault();

            if (returnModel == null)
            {
                returnModel = new ProjectFieldMapModel(Sessions.activitiesContainer?.RelevantActivities.n(0), new iatiactivity(), savedPreferences);
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
        #endregion

        #region ReviewAdjustment
        [HttpPost]
        public ProjectMapModelMinified GetProjectsToMap(ProjectFieldMapModel GeneralPreference)
        {
            Sessions.GeneralPreferences = GeneralPreference ?? Sessions.GeneralPreferences ?? GetGeneralPreferences();

            if (Sessions.ProjectMapModel.MatchedProjects.IsEmpty() && Sessions.ProjectMapModel.NewProjectsToAddInAims.IsEmpty())
            {

                Sessions.activitiesContainer = aimsDbIatiDAL.GetMappedActivities(Sessions.DP.ID);
                var heirarchyModel = CalculateHierarchyMatching();

                var filterBDModel = SubmitHierarchy(heirarchyModel);
                var iOrgs = GetAllImplementingOrg(filterBDModel);
                var relevantActivities = FilterDP(iOrgs.Orgs);
                var projectMapModel = SubmitActivities(relevantActivities);

            }

            ImportLogic.SetFieldMappingPreferences(Sessions.ProjectMapModel.MatchedProjects, Sessions.GeneralPreferences);

            Sessions.CurrentStage = Stage.ReviewAdjustment;
            var returnResult = new ProjectMapModel
            {
                MatchedProjects = Sessions.ProjectMapModel.MatchedProjects,
                IatiActivitiesNotInAims = null,
                AimsProjectsNotInIati = null,
                NewProjectsToAddInAims = Sessions.ProjectMapModel.NewProjectsToAddInAims,
                ProjectsOwnedByOther = null
            };

            Sessions.ProjectsToMap = returnResult;



            return ToMinifiedProjectMapModel(returnResult);
        }

        [HttpPost]
        public bool UnlinkProject(ProjectFieldMapModelMinified matchedProject)
        {
            if (matchedProject != null)
            {
                var matchedProjects = Sessions.ProjectMapModel.MatchedProjects;

                matchedProjects.RemoveAll(r => r.aimsProject.ProjectId == matchedProject.aimsProject.ProjectId && r.iatiActivity.IatiIdentifier == matchedProject.iatiActivity.IatiIdentifier);

                aimsDbIatiDAL.UnMapActivity(matchedProject.iatiActivity.IatiIdentifier);
            }
            return true;
        }

        #endregion

        #region Import
        [AcceptVerbs("GET", "POST")]
        public int? ImportProjects(ProjectMapModelMinified projectMapModel)
        {
            var matchedProjects = Sessions.ProjectMapModel.MatchedProjects;

            foreach (var mp in matchedProjects)
            {
                var v = projectMapModel.MatchedProjects.Find(f => f.iatiActivity.IatiIdentifier == mp.iatiActivity.IatiIdentifier);

                mp.Fields = v.Fields;
                mp.TransactionFields = v.TransactionFields;

            }

            //actual method starts here :)
            var margedProjects = ImportLogic.MergeProjects(matchedProjects);

            foreach (var item in Sessions.ProjectsToMap?.NewProjectsToAddInAims)
            {
                item.AllID = Sessions.DP.AllID;
                margedProjects.Add(item);

            }

            Sessions.Clear();
            return aimsDAL.UpdateProjects(margedProjects, Sessions.UserId,false);
        }

        #endregion

        [AcceptVerbs("GET", "POST")]
        public ProjectFieldMapModel GetMatchedProjectByIatiIdentifier(string iatiIdentifier)
        {
            ProjectFieldMapModel ProjectFieldMapModel = aimsDbIatiDAL.GetTransactionMismatchedActivity(iatiIdentifier);

            return ProjectFieldMapModel;
        }

        [AcceptVerbs("GET", "POST")]
        public int? UpdateTransactionByForce(Log log)
        {
            ProjectFieldMapModel ProjectFieldMapModel = aimsDbIatiDAL.GetTransactionMismatchedActivity(log.IatiIdentifier);

            var margedProjects = ImportLogic.MergeProjects(new List<ProjectFieldMapModel> { ProjectFieldMapModel });

            log.IsActive = false;
            aimsDbIatiDAL.UpdateLog(log);
            return aimsDAL.UpdateProjects(margedProjects, Sessions.UserId, false);
        }

        [AcceptVerbs("GET", "POST")]
        public int? SetIgnoreActivity(Log log)
        {
            log.IsActive = false;
            aimsDbIatiDAL.UpdateLog(log);
            return aimsDbIatiDAL.SetIgnoreActivity(log.IatiIdentifier);
        }
        private static IEnumerable<iatiactivity> GetMatchedProjects(List<iatiactivity> relevantActivies, List<iatiactivity> AimsProjects)
        {
            Sessions.CurrentStage = Stage.MatchProjects;

            return from i in relevantActivies
                   let isHierarchy2 = i.hierarchy == 2
                   from a in AimsProjects.Where(k => i.IatiIdentifier.Replace("-", "").EndsWith(k.IatiIdentifier.Replace("-", "")) ||
                   (isHierarchy2 ? false : i.IatiIdentifier.Contains(k.IatiIdentifier)))
                   orderby i.IatiIdentifier

                   select i;
        }

        private static void UpdateActivities(List<iatiactivityModel> clientActivities, List<iatiactivity> sessionActivities)
        {
            foreach (var activity in sessionActivities)
            {
                var ra = clientActivities.Find(f => f.IatiIdentifier == activity.IatiIdentifier);
                if (ra != null)
                {
                    activity.IsRelevant = ra.IsRelevant;
                    activity.ProjectId = ra.ProjectId;
                    activity.MappedProjectId = ra.MappedProjectId;
                    activity.MappedTrustFundId = ra.MappedTrustFundId;
                    activity.AllID = ra.AllID;
                    //var clientProperties = typeof(iatiactivity).GetProperties(BindingFlags.SetProperty).Where(w => w.GetCustomAttribute(typeof(Newtonsoft.Json.JsonIgnoreAttribute)) == null);

                    //foreach (PropertyInfo clientProperty in clientProperties)
                    //{
                    //    clientProperty.SetValue(activity, clientProperty.GetValue(ra));
                    //}
                }
            }
        }

        public List<iatiactivityModel> ToMinifiedIatiActivitiesModel(List<iatiactivity> iatiActivities, bool includeTransactions = false, bool includeChilds = false, bool includeMatched = false)
        {
            return iatiActivities == null ? null : (from i in iatiActivities
                                                    select ToMinifiedIatiActivityModel(i, includeTransactions, includeChilds, includeMatched)).ToList();
        }

        public iatiactivityModel ToMinifiedIatiActivityModel(iatiactivity iatiActivity, bool includeTransactions = false, bool includeChilds = false, bool includeMatched = false)
        {
            return new iatiactivityModel
            {
                IsDataSourceAIMS = iatiActivity.IsDataSourceAIMS,
                IsCofinancedProject = iatiActivity.IsCofinancedProject,
                IsTrustFundedProject = iatiActivity.IsTrustFundedProject,
                IsCommitmentIncluded = iatiActivity.IsCommitmentIncluded,
                IsDisbursmentIncluded = iatiActivity.IsDisbursmentIncluded,
                IsPlannedDisbursmentIncluded = iatiActivity.IsPlannedDisbursmentIncluded,
                IsInclude = iatiActivity.IsInclude,

                ProjectId = iatiActivity.ProjectId,
                MappedProjectId = iatiActivity.MappedProjectId,
                MappedTrustFundId = iatiActivity.MappedTrustFundId,
                HasChildActivity = iatiActivity.HasChildActivity,
                HasParentActivity = iatiActivity.HasParentActivity,
                MatchedProjects = includeMatched && iatiActivity.MatchedProjects?.Count > 0 ? ToMinifiedIatiActivitiesModel(iatiActivity.MatchedProjects) : null,
                childActivities = includeChilds && iatiActivity.childActivities?.Count > 0 ? ToMinifiedIatiActivitiesModel(iatiActivity.childActivities) : null,
                PercentToBD = iatiActivity.PercentToBD,
                IsRelevant = iatiActivity.IsRelevant,

                Commitments = includeTransactions ? iatiActivity.Commitments : null,
                TotalCommitment = iatiActivity.TotalCommitment,
                CommitmentsThisDPOnly = includeTransactions ? iatiActivity.CommitmentsThisDPOnly : null,
                TotalCommitmentThisDPOnly = iatiActivity.TotalCommitmentThisDPOnly,
                PlannedDisbursments = includeTransactions ? iatiActivity.PlannedDisbursments : null,
                TotalPlannedDisbursment = iatiActivity.TotalPlannedDisbursment,
                Disbursments = includeTransactions ? iatiActivity.Disbursments : null,
                TotalDisbursment = iatiActivity.TotalDisbursment,
                DisbursmentsThisDPOnly = includeTransactions ? iatiActivity.DisbursmentsThisDPOnly : null,
                TotalDisbursmentThisDPOnly = iatiActivity.TotalDisbursmentThisDPOnly,

                AllID = iatiActivity.AllID,

                IatiIdentifier = iatiActivity.IatiIdentifier,
                Title = iatiActivity.Title,
                Description = iatiActivity.Description,
                ReportingOrg = iatiActivity.ReportingOrg,
                ImplementingOrgs = iatiActivity.ImplementingOrgs,
                AidType = iatiActivity.AidType,
                AidTypeCode = iatiActivity.AidTypeCode,
                ActivityStatus = iatiActivity.ActivityStatus,

                PlannedStartDate = iatiActivity.PlannedStartDate,
                ActualStartDate = iatiActivity.ActualStartDate,
                PlannedEndDate = iatiActivity.PlannedEndDate,
                ActualEndDate = iatiActivity.ActualEndDate

            };
        }

        public ProjectMapModelMinified ToMinifiedProjectMapModel(ProjectMapModel projectMapModel)
        {
            return new ProjectMapModelMinified
            {
                selected = projectMapModel.selected,

                MatchedProjects = ToMinifiedProjectFieldMapModel(projectMapModel.MatchedProjects),
                IatiActivitiesNotInAims = ToMinifiedIatiActivitiesModel(projectMapModel.IatiActivitiesNotInAims),
                AimsProjectsNotInIati = ToMinifiedIatiActivitiesModel(projectMapModel.AimsProjectsNotInIati),
                NewProjectsToAddInAims = ToMinifiedIatiActivitiesModel(projectMapModel.NewProjectsToAddInAims),
                ProjectsOwnedByOther = ToMinifiedIatiActivitiesModel(projectMapModel.ProjectsOwnedByOther)

            };
        }

        public List<ProjectFieldMapModelMinified> ToMinifiedProjectFieldMapModel(List<ProjectFieldMapModel> projectFieldMapModels)
        {
            return (from m in projectFieldMapModels
                    select new ProjectFieldMapModelMinified
                    {
                        IsManuallyMapped = m.IsManuallyMapped,
                        IsGrouped = m.IsGrouped,
                        iatiActivity = ToMinifiedIatiActivityModel(m.iatiActivity, true),
                        aimsProject = ToMinifiedIatiActivityModel(m.aimsProject),
                        Fields = m.Fields,
                        TransactionFields = m.TransactionFields,
                        Id = m.Id

                    }).ToList();
        }
    }



}


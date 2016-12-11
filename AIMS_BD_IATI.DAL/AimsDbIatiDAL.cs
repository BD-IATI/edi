using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.DAL;

namespace AIMS_BD_IATI.DAL
{
    public class AimsDbIatiDAL
    {
        #region Declarations
        AIMS_DB_IATIEntities dbContext = new AIMS_DB_IATIEntities();
        List<ExchangeRateModel> ExchangeRates
        {
            get;
            set;
        }
        #endregion Declarations

        #region Constructors
        public AimsDbIatiDAL()
        {
            ExchangeRates = new List<ExchangeRateModel>();
        }
        #endregion Constructors

        #region Save and Update Activites
        public int SaveAtivities(List<Activity> activities, List<iatiactivity> iatiActivities, tblFundSource fundSource)
        {

            foreach (var activity in activities)
            {
                var a = dbContext.Activities.FirstOrDefault(x => x.IatiIdentifier == activity.IatiIdentifier);
                if (a != null)
                {
                    a.OrgId = activity.OrgId;

                    a.IatiActivityPrev = a.IatiActivity;
                    a.IatiActivity = activity.IatiActivity;

                    a.Hierarchy = activity.Hierarchy;
                    a.ParentHierarchy = activity.ParentHierarchy;

                    a.DownloadDatePrev = a.DownloadDate;
                    a.DownloadDate = DateTime.Now;

                    //update aimsdb
                    if (a.ProjectId > 0 || a.MappedProjectId > 0)
                    {
                        var aimsDAL = new AimsDAL();

                        //step 1: project structure
                        var iactivities = new List<iatiactivity>();
                        if (a.Hierarchy == 1)
                            iactivities = ImportLogic.LoadH1ActivitiesWithChild(iatiActivities); // here pass all activities to find out their child activities
                        else
                            iactivities = ImportLogic.LoadH2ActivitiesWithParent(iatiActivities);

                        //step 2: get mapped iatiActivity and aimsProject
                        var iatiActivity = iactivities.Find(f => f.IatiIdentifier == a.IatiIdentifier);
                        // SetExchangedValues
                        SetExchangedValues(iatiActivity);
                        iatiActivity.childActivities.ForEach(ra => SetExchangedValues(ra));

                        var aimsProject = new iatiactivity();

                        if (a.ProjectId > 0)
                        {
                            aimsProject = aimsDAL.GetAIMSProjectInIATIFormat(a.ProjectId);
                            if (aimsProject != null)
                            {
                                //step 3: get general preference
                                var generalPreference = GetFieldMappingPreferenceGeneral(a.OrgId);

                                //step 4: create a ProjectFieldMapModel using iatiActivity, aimsProject and generalPreference
                                var ProjectFieldMapModel = new ProjectFieldMapModel(iatiActivity, aimsProject, generalPreference);

                                //step 5: SetFieldMappingPreferences
                                var ProjectFieldMapModels = new List<ProjectFieldMapModel>(); // here we make a list just to use existing method (e.g existing method require a List parameter)
                                ProjectFieldMapModels.Add(ProjectFieldMapModel);

                                ImportLogic.SetFieldMappingPreferences(ProjectFieldMapModels, ProjectFieldMapModel);

                                //step 6: merge iatiActivity and aimsProject; and get an new merged activity
                                var mergedActivities = ImportLogic.MergeProjects(ProjectFieldMapModels); //now it will allways return a list containing single activity
                                mergedActivities.n(0).AllID = fundSource.Id + "~" + a.OrgId + "~"
                                        + (int)ExecutingAgencyType.DP + "~"
                                        + fundSource.FundSourceCategoryId;
                                //step 7: update aims database with margedActivities
                                aimsDAL.UpdateProjects(mergedActivities, "system");
                            }
                            else
                            {
                                dbContext.Logs.Add(new Log
                                {
                                    IatiIdentifier = activity.IatiIdentifier,
                                    OrgId = activity.OrgId,
                                    ProjectId = a.MappedProjectId,
                                    Message = "The mapped project is not found in AIMS database",
                                    LogType = (int)LogType.AimsProjectNotFound,
                                    DateTime = DateTime.Now
                                });
                            }
                        }
                        else if (a.MappedProjectId > 0) //for co-finance projects
                        {
                            aimsProject = aimsDAL.GetAIMSProjectInIATIFormat(a.MappedProjectId);

                            if (aimsProject != null)
                            {
                                iatiActivity.AllID = fundSource.Id + "~" + a.OrgId + "~"
                                        + (int)ExecutingAgencyType.DP + "~"
                                        + fundSource.FundSourceCategoryId;


                                aimsProject.MatchedProjects.Add(iatiActivity);
                                //step 7: update aims database with margedActivities
                                aimsDAL.UpdateCofinanceProjects(new List<iatiactivity> { aimsProject }, "system");
                            }
                            else
                            {
                                dbContext.Logs.Add(new Log
                                {
                                    IatiIdentifier = activity.IatiIdentifier,
                                    OrgId = activity.OrgId,
                                    ProjectId = a.MappedProjectId,
                                    Message = "The mapped project is not found in AIMS database",
                                    LogType = (int)LogType.AimsProjectNotFound,
                                    DateTime = DateTime.Now
                                });
                            }

                        }

                    }

                }
                else
                {
                    activity.DownloadDate = DateTime.Now;
                    activity.IsInclude = true;
                    dbContext.Activities.Add(activity);

                    dbContext.Logs.Add(new Log
                    {
                        IatiIdentifier = activity.IatiIdentifier,
                        OrgId = activity.OrgId,
                        Message = "Imported new activity",
                        LogType = (int)LogType.AddedNewActivity,
                        DateTime = DateTime.Now
                    });
                }

            }

            return dbContext.SaveChanges();
        }
        public int SaveAtivity(Activity activity, iatiactivity iatiActivity, tblFundSource fundSource)
        {


            return SaveAtivities(new List<Activity> { activity }, new List<iatiactivity> { iatiActivity }, fundSource);
        }

        private List<ProjectFieldMapModel> PrepareMappedActivities(List<iatiactivity> iatiActivities, Activity a, AimsDAL aimsDAL)
        {
            //step 1: project structure
            var iactivities = new List<iatiactivity>();
            if (a.Hierarchy == 1)
                iactivities = ImportLogic.LoadH1ActivitiesWithChild(iatiActivities); // here pass all activities to find out their child activities
            else
                iactivities = ImportLogic.LoadH2ActivitiesWithParent(iatiActivities);

            //step 2: get mapped iatiActivity and aimsProject
            var iatiActivity = iactivities.Find(f => f.IatiIdentifier == a.IatiIdentifier);
            // SetExchangedValues
            SetExchangedValues(iatiActivity);
            iatiActivity.childActivities.ForEach(ra => SetExchangedValues(ra));

            var aimsProject = new iatiactivity();
            if (a.ProjectId > 0)
            {
                aimsProject = aimsDAL.GetAIMSProjectInIATIFormat(a.ProjectId);
            }
            else if (a.MappedProjectId > 0) //for co-finance projects
            {
                aimsProject = aimsDAL.GetAIMSProjectInIATIFormat(a.MappedProjectId);
            }

            //step 3: get general preference
            var generalPreference = GetFieldMappingPreferenceGeneral(a.OrgId);

            //step 4: create a ProjectFieldMapModel using iatiActivity, aimsProject and generalPreference
            var ProjectFieldMapModel = new ProjectFieldMapModel(iatiActivity, aimsProject, generalPreference);

            //step 5: SetFieldMappingPreferences
            var ProjectFieldMapModels = new List<ProjectFieldMapModel>(); // here we make a list just to use existing method (e.g existing method require a List parameter)
            ProjectFieldMapModels.Add(ProjectFieldMapModel);

            ImportLogic.SetFieldMappingPreferences(ProjectFieldMapModels, ProjectFieldMapModel);
            return ProjectFieldMapModels;
        }

        public int AssignActivities(List<iatiactivityModel> activities)
        {
            foreach (var activity in activities)
            {
                var a = dbContext.Activities.FirstOrDefault(x => x.IatiIdentifier == activity.IatiIdentifier);
                if (a != null)
                {
                    a.AssignedOrgId = activity.IATICode;
                    a.AssignedDate = DateTime.Now;
                }
            }

            return dbContext.SaveChanges();
        }

        public int RecallDelegatedActivity(ActivityModel activity)
        {
            var a = dbContext.Activities.FirstOrDefault(x => x.IatiIdentifier == activity.IatiIdentifier);
            if (a != null)
            {
                a.AssignedOrgId = a.OrgId;
                a.AssignedDate = DateTime.Now;
            }

            return dbContext.SaveChanges();
        }

        public int MapActivities(List<iatiactivity> activities)
        {
            foreach (var activity in activities)
            {
                var a = dbContext.Activities.FirstOrDefault(x => x.IatiIdentifier == activity.IatiIdentifier);
                if (a != null)
                {
                    a.ProjectId = activity.ProjectId;
                    a.MappedProjectId = activity.MappedProjectId;
                    a.MappedTrustFundId = activity.MappedTrustFundId;

                    if (activity.childActivities != null)
                        foreach (var ca in activity.childActivities)
                        {
                            var c = dbContext.Activities.FirstOrDefault(f => f.IatiIdentifier == ca.IatiIdentifier);
                            if (c != null)
                                c.IsInclude = ca.IsInclude;

                        }
                }
            }

            return dbContext.SaveChanges();
        }

        public int SetIncludeActivities(List<iatiactivity> iatiActivities)
        {
            foreach (var item in iatiActivities)
            {
                var a = dbContext.Activities.FirstOrDefault(x => x.IatiIdentifier == item.IatiIdentifier);
                if (a != null)
                {
                    a.IsInclude = item.IsInclude;
                }

            }

            return dbContext.SaveChanges();
        }
        //userd for merge conflict activities
        public int SetIgnoreActivity(string iatiIdentifier)
        {
            var a = dbContext.Activities.FirstOrDefault(x => x.IatiIdentifier == iatiIdentifier);
            if (a != null)
            {
                a.IsIgnore = true;
            }

            return dbContext.SaveChanges();
        }
        public int UnMapActivity(string iatiIdentifier)
        {
            var a = dbContext.Activities.FirstOrDefault(x => x.IatiIdentifier == iatiIdentifier);
            if (a != null)
            {
                a.ProjectId = null;
                a.MappedProjectId = null;
                a.MappedTrustFundId = null;
            }

            return dbContext.SaveChanges();
        }
        #endregion Save and Update Activites

        #region Get Activities
        public CFnTFModel GetAssignActivities(string dp, bool mappedOnly = false)
        {
            var q = (from a in dbContext.Activities
                     let isNotMapped = (a.ProjectId ?? 0) == 0 && (a.MappedProjectId ?? 0) == 0 && (a.MappedTrustFundId ?? 0) == 0
                     let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0 || a.MappedTrustFundId > 0
                     where a.OrgId != dp
                         && a.AssignedOrgId == dp
                         && (mappedOnly ? isMapped : isNotMapped)

                     orderby a.IatiIdentifier
                     select new ActivityModel
                     {
                         IatiActivity = a.IatiActivity,
                         IatiIdentifier = a.IatiIdentifier,
                         ProjectId = a.ProjectId,
                         MappedProjectId = a.MappedProjectId,
                         MappedTrustFundId = a.MappedTrustFundId,
                         OrgId = a.OrgId,
                         IsInclude = a.IsInclude
                     }).ToList();

            var iatiActivities = ParseXMLAndResolve(q);

            foreach (var iatiActivity in iatiActivities)
            {
                LoadChildActivities(iatiActivity);
                SetExchangedValues(iatiActivity);

                #region Field Mapping Preference Delegateds
                var FieldMappingPreferenceDelegateds = dbContext.FieldMappingPreferenceDelegateds.Where(w => w.IatiIdentifier == iatiActivity.IatiIdentifier).ToList();
                if (FieldMappingPreferenceDelegateds.Count > 0)
                {
                    var CommitmentIncluded = FieldMappingPreferenceDelegateds.FirstOrDefault(j => j.FieldName == IatiFields.Commitment);
                    iatiActivity.IsCommitmentIncluded = CommitmentIncluded?.IsInclude ?? false;

                    var PlannedDisbursmentIncluded = FieldMappingPreferenceDelegateds.FirstOrDefault(j => j.FieldName == IatiFields.PlannedDisbursment);
                    iatiActivity.IsPlannedDisbursmentIncluded = PlannedDisbursmentIncluded?.IsInclude ?? false;

                    var DisbursmentIncluded = FieldMappingPreferenceDelegateds.FirstOrDefault(j => j.FieldName == IatiFields.Disbursment);
                    iatiActivity.IsDisbursmentIncluded = DisbursmentIncluded?.IsInclude ?? false;

                }
                #endregion


            }


            return new CFnTFModel
            {
                AssignedActivities = iatiActivities,
                AimsProjects = new AimsDAL().GetAIMSProjectsInIATIFormat(dp)
            };
        }

        //public iatiactivityContainer GetAllActivities(string dp)
        //{
        //    var q = (from a in dbContext.Activities
        //             where a.AssignedOrgId == dp
        //             orderby a.IatiIdentifier
        //             select new ActivityModel
        //             {
        //                 IatiActivity = a.IatiActivity,
        //                 OrgId = a.OrgId,
        //                 ProjectId = a.ProjectId,
        //                 MappedProjectId = a.MappedProjectId,
        //                 MappedTrustFundId = a.MappedTrustFundId,
        //                 IsInclude = a.IsInclude
        //             }).ToList();

        //    var iatiActivities = ParseXMLAndResolve(q);



        //    var aimsActivities = new AimsDAL().GetAIMSProjectsInIATIFormat(dp);

        //    return new iatiactivityContainer
        //    {
        //        iatiActivities = iatiActivities,
        //        AimsProjects = aimsActivities
        //    };
        //}
        public ProjectFieldMapModel GetTransactionMismatchedActivity(string iatiIdentifier)
        {
            var q = (from a in dbContext.Activities
                     let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0 || a.MappedTrustFundId > 0
                     where a.IatiIdentifier == iatiIdentifier && isMapped
                     select new ActivityModel
                     {
                         IatiActivity = a.IatiActivity,
                         OrgId = a.OrgId,
                         ProjectId = a.ProjectId,
                         MappedProjectId = a.MappedProjectId,
                         MappedTrustFundId = a.MappedTrustFundId,
                         IsInclude = a.IsInclude
                     }).FirstOrDefault();

            if (q == null) return null;

            var iatiActivity = ParseXMLAndResolve(new List<ActivityModel> { q }).FirstOrDefault();

            LoadChildActivities(iatiActivity);

            var aimsProject = new AimsDAL().GetAIMSProjectInIATIFormat(q.ProjectId > 0 ? q.ProjectId :
                                    q.MappedProjectId > 0 ? q.MappedProjectId :
                                    0);

            foreach (var aimsTransaction in aimsProject.transaction)
            {
                var isFoundInIati = iatiActivity.AllTransactions.Any(a => a.transactiontype?.code == aimsTransaction.transactiontype?.code
                    && a.transactiondate?.isodate == aimsTransaction.transactiondate?.isodate
                    && Math.Floor(a.ValUSD) == Math.Floor(aimsTransaction.ValUSD));

                aimsTransaction.IsConflicted = !isFoundInIati;
            }

            return new ProjectFieldMapModel(iatiActivity, aimsProject);
        }

        public iatiactivityContainer GetNotMappedActivities(string dp)
        {
            var q = (from a in dbContext.Activities
                     let isNotMapped = (a.ProjectId ?? 0) == 0 && (a.MappedProjectId ?? 0) == 0 && (a.MappedTrustFundId ?? 0) == 0
                     where a.OrgId == dp && a.AssignedOrgId == dp && isNotMapped && a.IsIgnore != true
                     orderby a.IatiIdentifier
                     select new ActivityModel
                     {
                         IatiActivity = a.IatiActivity,
                         OrgId = a.OrgId,
                         ProjectId = a.ProjectId,
                         MappedProjectId = a.MappedProjectId,
                         MappedTrustFundId = a.MappedTrustFundId,
                         IsInclude = a.IsInclude
                     }).ToList();

            var iatiActivities = ParseXMLAndResolve(q);
            foreach (var activity in iatiActivities)
            {
                LoadChildActivities(activity);
            }
            var aimsActivities = GetNotMappedAimsProjects(dp);

            //since IATIIdentifier does not fully match between AIMS and IATI
            foreach (var act in aimsActivities)
            {
                var mathcedIatiactivity = iatiActivities.Find(i => i.IatiIdentifier.Replace("-", "").EndsWith(act.IatiIdentifier.Replace("-", "")) ||
                    (i.hierarchy == 2 ? false : i.IatiIdentifier.Contains(act.IatiIdentifier)));
                if (act.IatiIdentifier != mathcedIatiactivity?.IatiIdentifier)
                    act.OriginalIatiIdentifier = mathcedIatiactivity?.IatiIdentifier;
            }

            return new iatiactivityContainer
            {
                iatiActivities = iatiActivities,
                AimsProjects = aimsActivities
            };
        }

        private List<iatiactivity> GetNotMappedAimsProjects(string dp)
        {
            var mappedProjectIds = (from a in dbContext.Activities
                                    let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0 || a.MappedTrustFundId > 0
                                    where a.AssignedOrgId == dp && isMapped && a.IsIgnore != true
                                    select (a.ProjectId > 0 ? a.ProjectId :
                                    a.MappedProjectId > 0 ? a.MappedProjectId :
                                    a.MappedTrustFundId)).ToList();


            var aimsActivities = new AimsDAL().GetNotMappedAIMSProjectsInIATIFormat(dp, mappedProjectIds);
            return aimsActivities;
        }

        public iatiactivityContainer GetMappedActivities(string dp)
        {
            var q = (from a in dbContext.Activities
                     let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0
                     where a.OrgId == dp && a.AssignedOrgId == dp && isMapped && a.IsIgnore != true
                     orderby a.IatiIdentifier
                     select new ActivityModel
                     {
                         IatiActivity = a.IatiActivity,
                         OrgId = a.OrgId,
                         ProjectId = a.ProjectId,
                         MappedProjectId = a.MappedProjectId,
                         MappedTrustFundId = a.MappedTrustFundId,
                         IsInclude = a.IsInclude
                     }).ToList();

            var iatiActivities = ParseXMLAndResolve(q);
            foreach (var activity in iatiActivities)
            {
                LoadChildActivities(activity);
            }

            var aimsActivities = GetMappedAimsProjects(dp);

            return new iatiactivityContainer
            {
                iatiActivities = iatiActivities,
                AimsProjects = aimsActivities
            };
        }

        private List<iatiactivity> GetMappedAimsProjects(string dp)
        {
            var mappedProjectIds = (from a in dbContext.Activities
                                    let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0
                                    where a.AssignedOrgId == dp && isMapped && a.IsIgnore != true
                                    select (a.ProjectId > 0 ? a.ProjectId :
                                    a.MappedProjectId)).ToList();


            var aimsActivities = new AimsDAL().GetMappedAIMSProjectsInIATIFormat(dp, mappedProjectIds);
            return aimsActivities;
        }

        #region Helper Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private List<iatiactivity> ParseXMLAndResolve(List<ActivityModel> q)
        {
            var result = new List<iatiactivity>();
            var serializer = new XmlSerializer(typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivity));

            foreach (var a in q)
            {
                using (TextReader reader = new StringReader(a.IatiActivity))
                {
                    a.iatiActivity = (iatiactivity)serializer.Deserialize(reader);
                }
                a.iatiActivity.MappedProjectId = a.MappedProjectId ?? 0;
                a.iatiActivity.MappedTrustFundId = a.MappedTrustFundId ?? 0;
                a.iatiActivity.IsInclude = a.IsInclude;

                a.iatiActivity.AllID = new AimsDAL().GetFundSourceIDnIATICode(a.OrgId);

                SetExchangedValues(a.iatiActivity);
                result.Add(a.iatiActivity);
            }
            return result;
        }

        public void LoadChildActivities(iatiactivity activity)
        {
            if (activity.HasChildActivity)
            {
                var relatedActivities = new List<iatiactivity>();

                var ras = (from a in dbContext.Activities
                           where a.IatiIdentifier.StartsWith(activity.IatiIdentifier)
                                && a.Hierarchy == 2
                           select new ActivityModel
                           {
                               IatiActivity = a.IatiActivity,
                               OrgId = a.OrgId,
                               ProjectId = a.ProjectId,
                               MappedProjectId = a.MappedProjectId,
                               MappedTrustFundId = a.MappedTrustFundId,
                               IsInclude = a.IsInclude
                           }).ToList();

                relatedActivities = ParseXMLAndResolve(ras);

                activity.childActivities = relatedActivities;
            }
            activity.IsChildActivityLoaded = true;

        }

        public void SetExchangedValues(iatiactivity activity)
        {
            foreach (var tr in activity.AllTransactions)
            {
                SetCurrencyExRateAndVal(tr, activity.defaultcurrency, tr.transactiondate?.isodate ?? default(DateTime));
            }

            if (activity.budget != null)
                foreach (var tr in activity.budget)
                {
                    SetCurrencyExRateAndVal(tr, activity.defaultcurrency);

                }

            if (activity.planneddisbursement != null)
                foreach (var tr in activity.planneddisbursement)
                {
                    SetCurrencyExRateAndVal(tr, activity.defaultcurrency);

                }
        }

        public void SetCurrencyExRateAndVal(ICurrency tr, string defaultcurrency, DateTime trDate = default(DateTime))
        {
            tr.value.currency = tr.value.currency ?? defaultcurrency;

            var cur = tr.value.currency;

            if (ExchangeRates.Exists(e => e.ISO_CURRENCY_CODE == cur) == false)
            {
                ExchangeRates.AddRange(new AimsDAL().GetExchangesRateToUSD(cur));
            }


            var exchangeRates = ExchangeRates.Where(k => k.ISO_CURRENCY_CODE == cur).OrderBy(o => o.DATE);

            var valDate = tr.value.valuedate == default(DateTime) ? trDate : tr.value.valuedate;

            var nearestPast = exchangeRates.Where(k => k.DATE <= valDate).FirstOrDefault();
            var nearestPastDate = nearestPast == null ? default(DateTime) : nearestPast.DATE;
            var nearestFuture = exchangeRates.Where(k => k.DATE >= valDate).FirstOrDefault();
            var nearestFutureDate = nearestFuture == null ? default(DateTime) : nearestFuture.DATE;

            var nearestDate = (nearestFutureDate - valDate).TotalDays <= (valDate - nearestPastDate).TotalDays ? nearestFutureDate : nearestPastDate;


            var curExchangeRate = exchangeRates.Where(k => k.DATE == nearestDate).FirstOrDefault() ?? exchangeRates.FirstOrDefault();

            tr.value.BBexchangeRateDate = curExchangeRate?.DATE ?? default(DateTime).ToSqlDateTime();
            tr.value.BBexchangeRateUSD = curExchangeRate?.DOLLAR_PER_CURRENCY ?? 0;
            tr.value.ValueInUSD = tr.value.Value * tr.value.BBexchangeRateUSD;
            tr.value.BBexchangeRateBDT = curExchangeRate?.TAKA_PER_DOLLAR ?? 0;
            tr.value.ValueInBDT = tr.value.ValueInUSD * tr.value.BBexchangeRateBDT;

        }

        #endregion Helper Methods
        #endregion Get Activities

        #region Field Mapping Preference

        public int SaveFieldMappingPreferenceGeneral(List<FieldMappingPreferenceGeneral> fieldMaps)
        {
            foreach (var fieldMap in fieldMaps)
            {
                var a = dbContext.FieldMappingPreferenceGenerals.FirstOrDefault(x => x.OrgId == fieldMap.OrgId
                                                                                    && x.FundSourceId == fieldMap.FundSourceId
                                                                                    && x.FieldName == fieldMap.FieldName);
                if (a != null)
                {
                    a.OrgId = fieldMap.OrgId;
                    a.FundSourceId = fieldMap.FundSourceId;
                    a.FieldName = fieldMap.FieldName;
                    a.IsSourceIATI = fieldMap.IsSourceIATI;
                }
                else
                {
                    dbContext.FieldMappingPreferenceGenerals.Add(fieldMap);
                }

            }

            return dbContext.SaveChanges();
        }

        public int SaveFieldMappingPreferenceActivity(List<FieldMappingPreferenceActivity> fieldMaps)
        {
            foreach (var fieldMap in fieldMaps)
            {
                var a = dbContext.FieldMappingPreferenceActivities.FirstOrDefault(x => x.IatiIdentifier == fieldMap.IatiIdentifier
                                                                                    //&& x.ProjectId == fieldMap.ProjectId
                                                                                    && x.FieldName == fieldMap.FieldName);
                if (a != null)
                {
                    a.IatiIdentifier = fieldMap.IatiIdentifier;
                    a.ProjectId = fieldMap.ProjectId;
                    a.FieldName = fieldMap.FieldName;
                    a.IsSourceIATI = fieldMap.IsSourceIATI;
                }
                else
                {
                    dbContext.FieldMappingPreferenceActivities.Add(fieldMap);
                }

            }

            return dbContext.SaveChanges();
        }

        public int SaveFieldMappingPreferenceDelegated(List<FieldMappingPreferenceDelegated> fieldMaps)
        {
            foreach (var fieldMap in fieldMaps)
            {
                var a = dbContext.FieldMappingPreferenceDelegateds.FirstOrDefault(x => x.IatiIdentifier == fieldMap.IatiIdentifier
                                                                                    && x.FieldName == fieldMap.FieldName);
                if (a != null)
                {
                    a.IatiIdentifier = fieldMap.IatiIdentifier;
                    a.FieldName = fieldMap.FieldName;
                    a.IsInclude = fieldMap.IsInclude;
                }
                else
                {
                    dbContext.FieldMappingPreferenceDelegateds.Add(fieldMap);
                }

            }

            return dbContext.SaveChanges();
        }

        public List<FieldMappingPreferenceGeneral> GetFieldMappingPreferenceGeneral(string dp)
        {
            var q = (from fieldMap in dbContext.FieldMappingPreferenceGenerals.Where(w => w.OrgId == dp)
                     select fieldMap).ToList();

            return q;
        }

        public List<FieldMappingPreferenceActivity> GetFieldMappingPreferenceActivity(string iatiIdentifier)
        {
            var q = (from fieldMap in dbContext.FieldMappingPreferenceActivities
                     where fieldMap.IatiIdentifier == iatiIdentifier
                     select fieldMap).ToList();

            return q;
        }

        public List<FieldMappingPreferenceDelegated> GetFieldMappingPreferenceDelegated(string iatiIdentifier)
        {
            var q = (from fieldMap in dbContext.FieldMappingPreferenceDelegateds
                     where fieldMap.IatiIdentifier == iatiIdentifier
                     select fieldMap).ToList();

            return q;
        }

        #endregion Field Mapping Preference

        #region Get Dashboard Infos
        public DateTime? GetLastDownloadDate(string dp)
        {
            var q = (from a in dbContext.Activities.Where(a => a.OrgId == dp)
                     orderby a.DownloadDate descending
                     select a.DownloadDate).FirstOrDefault();

            return q == null ? default(DateTime?) : q.Value.ToUniversalTime();

        }
        public List<ActivityModel> GetDelegatedActivities(string dp)
        {
            var q = (from a in dbContext.Activities.Where(a => a.OrgId == dp && a.AssignedOrgId != dp)

                     select new ActivityModel
                     {
                         IatiIdentifier = a.IatiIdentifier,
                         AssignedOrgId = a.AssignedOrgId,
                         AssignedDate = a.AssignedDate,
                         IatiActivity = a.IatiActivity,
                         MappedProjectId = a.MappedProjectId,
                         MappedTrustFundId = a.MappedTrustFundId
                     }).ToList();

            ParseXMLAndResolve(q);

            return q;

        }
        public List<ActivityModel> GetCofinanceProjects(string dp)
        {
            var q = ((from a in dbContext.Activities.Where(a => a.OrgId != dp && a.AssignedOrgId == dp)
                      where a.MappedProjectId > 0
                      select new ActivityModel
                      {
                          IatiIdentifier = a.IatiIdentifier,
                          AssignedOrgId = a.AssignedOrgId,
                          AssignedDate = a.AssignedDate,
                          IatiActivity = a.IatiActivity
                      })).ToList();

            ParseXMLAndResolve(q);

            return q;

        }
        public List<ActivityModel> GetTrustFundProjects(string dp)
        {
            var q = (from a in dbContext.Activities.Where(a => a.OrgId != dp && a.AssignedOrgId == dp)
                     where a.MappedTrustFundId > 0
                     select new ActivityModel
                     {
                         IatiIdentifier = a.IatiIdentifier,
                         AssignedOrgId = a.AssignedOrgId,
                         AssignedDate = a.AssignedDate,
                         IatiActivity = a.IatiActivity
                     }).ToList();

            ParseXMLAndResolve(q);

            return q;

        }

        public int GetTotalActivityCount(string dp)
        {
            var q = dbContext.Activities.Where(w => w.OrgId == dp).Count();
            return q;
        }
        public int GetNewActivityCount(string dp)
        {
            var q = (from a in dbContext.Activities
                     let isNotMapped = (a.ProjectId ?? 0) == 0 && (a.MappedProjectId ?? 0) == 0 && (a.MappedTrustFundId ?? 0) == 0
                     let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0 || a.MappedTrustFundId > 0
                     where a.OrgId == dp && a.AssignedOrgId == dp && isNotMapped
                     select 1).Count();
            return q;
        }
        public int GetMappedActivityCount(string dp)
        {
            var q = (from a in dbContext.Activities
                     let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0 || a.MappedTrustFundId > 0
                     where a.OrgId == dp && a.AssignedOrgId == dp && isMapped
                     select 1).Count();
            return q;
        }
        public int GetAssignedActivityCount(string dp)
        {
            var q = (from a in dbContext.Activities
                     let isNotMapped = (a.ProjectId ?? 0) == 0 && (a.MappedProjectId ?? 0) == 0 && (a.MappedTrustFundId ?? 0) == 0
                     let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0 || a.MappedTrustFundId > 0
                     where a.AssignedOrgId == dp && a.OrgId != dp && isNotMapped
                     select 1).Count();
            return q;
        }
        public List<Log> GetLastDayLogs(string dp)
        {
            Log lastLog = dbContext.Logs.Where(w => w.OrgId == dp).OrderByDescending(o => o.Id).FirstOrDefault();
            DateTime lastDate = lastLog.n().DateTime.n().Value.Date;
            var logs = dbContext.Logs.Where(w => w.OrgId == dp && w.DateTime >= lastDate && w.IsActive != false).ToList();

            return logs;
        }

        #endregion Get Dashboard Infos


        public int SaveExchangeRateFedaral(List<ExchangeRateFederal> list)
        {
            int counter = 0;
            foreach (var obj in list)
            {
                var a = dbContext.ExchangeRateFederals.FirstOrDefault(x => x.Date == obj.Date && x.Currency == obj.Currency);
                if (a == null)
                {
                    a = new ExchangeRateFederal();
                    a.InsertDate = DateTime.Now;

                    dbContext.ExchangeRateFederals.Add(a);
                }
                a.Date = obj.Date;
                a.Rate = obj.Rate;
                a.Currency = obj.Currency;
                a.Frequency = obj.Frequency;

                Console.Write("\r Exchange Rate Counter: {0}   ", counter++);
            }

            return dbContext.SaveChanges();
        }

        public int InsertLog(Log log)
        {
            using (var db = new AIMS_DB_IATIEntities())
            {
                dbContext.Logs.Add(log);
                return db.SaveChanges();
            }
        }
        public int UpdateLog(Log log)
        {
            using (var db = new AIMS_DB_IATIEntities())
            {
                var ll = db.Logs.Where(f => f.LogType == log.LogType && f.IatiIdentifier == log.IatiIdentifier);

                foreach (var item in ll)
                {
                    item.IsActive = log.IsActive;
                }

                return db.SaveChanges();
            }
        }
    }
}

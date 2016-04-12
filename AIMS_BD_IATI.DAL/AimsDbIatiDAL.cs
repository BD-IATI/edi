using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AIMS_BD_IATI.Library;
namespace AIMS_BD_IATI.DAL
{
    public class AimsDbIatiDAL
    {
        AIMS_DB_IATIEntities dbContext = new AIMS_DB_IATIEntities();

        public AimsDbIatiDAL()
        {
            ExchangeRates = new List<ExchangeRateModel>();
        }
        List<ExchangeRateModel> ExchangeRates
        {
            get;
            set;
        }

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

                        //step 1-5
                        var ProjectFieldMapModels = PrepareMappedActivities(iatiActivities, a, aimsDAL);

                        //step 6: merge iatiActivity and aimsProject; and get an new merged activity
                        var mergedActivities = ImportLogic.MergeProjects(ProjectFieldMapModels); //now it will allways return a list containing single activity
                        mergedActivities.n(0).FundSourceIDnIATICode = fundSource.Id + "~" + a.OrgId;
                        //step 7: update aims database with margedActivities
                        aimsDAL.UpdateProjects(mergedActivities, "system");

                    }

                }
                else
                {
                    activity.DownloadDate = DateTime.Now;
                    dbContext.Activities.Add(activity);

                    dbContext.Logs.Add(new Log
                    {
                        IatiIdentifier = activity.IatiIdentifier,
                        OrgId = activity.OrgId,
                        Message = "Imported new activity",
                        LogType = (int)LogType.Info,
                        DateTime = DateTime.Now
                    });
                }

            }

            return dbContext.SaveChanges();
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

        public int AssignActivities(List<iatiactivity> activities)
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
                }
            }

            return dbContext.SaveChanges();
        }
        public CFnTFModel GetAssignActivities(string dp)
        {
            var q = (from a in dbContext.Activities
                     where a.OrgId != dp && a.AssignedOrgId == dp
                     orderby a.IatiIdentifier
                     select new ActivityModel
                     {
                         IatiActivity = a.IatiActivity,
                         IatiIdentifier = a.IatiIdentifier,
                         ProjectId = a.ProjectId,
                         MappedProjectId = a.MappedProjectId,
                         MappedTrustFundId = a.MappedTrustFundId,
                         OrgId = a.OrgId
                     }).ToList();

             var iatiActivities = ParseXMLAndResolve(q);

            foreach (var iatiActivity in iatiActivities)
            {

                #region Field Mapping Preference Delegateds
                var FieldMappingPreferenceDelegateds = dbContext.FieldMappingPreferenceDelegateds.Where(w => w.IatiIdentifier == iatiActivity.IatiIdentifier).ToList();
                if (FieldMappingPreferenceDelegateds.Count > 0)
                {
                    var CommitmentIncluded = FieldMappingPreferenceDelegateds.FirstOrDefault(j => j.FieldName == IatiFields.Commitment);
                    iatiActivity.IsCommitmentIncluded = CommitmentIncluded.n().IsInclude ?? false;

                    var PlannedDisbursmentIncluded = FieldMappingPreferenceDelegateds.FirstOrDefault(j => j.FieldName == IatiFields.PlannedDisbursment);
                    iatiActivity.IsPlannedDisbursmentIncluded = PlannedDisbursmentIncluded.n().IsInclude ?? false;

                    var DisbursmentIncluded = FieldMappingPreferenceDelegateds.FirstOrDefault(j => j.FieldName == IatiFields.Disbursment);
                    iatiActivity.IsDisbursmentIncluded = DisbursmentIncluded.n().IsInclude ?? false;

                }
                #endregion

                LoadChildActivities(iatiActivity);
                SetExchangedValues(iatiActivity);

            }


            return new CFnTFModel
            {
                AssignedActivities = iatiActivities,
                AimsProjects = new AimsDAL().GetAIMSProjectsInIATIFormat(dp)
            };
        }

        private void LoadChildActivities(iatiactivity activity)
        {
            if (activity.HasChildActivity)
            {
                var relatedActivities = new List<iatiactivity>();
                var relatedActivity = new iatiactivity();

                var ras = (from a in dbContext.Activities
                           where a.IatiIdentifier.StartsWith(activity.IatiIdentifier)
                           select new ActivityModel { IatiActivity = a.IatiActivity, OrgId = a.OrgId }).ToList();

                relatedActivities = ParseXMLAndResolve(ras);

                //add all transaction of child activities to parent 
                List<transaction> transactions = new List<transaction>();
                if (activity.transaction != null)
                    transactions = activity.transaction.ToList();

                foreach (var ra in relatedActivities)
                {
                    if (ra.transaction != null)
                        transactions.AddRange(ra.transaction);

                    SetExchangedValues(ra);
                }
                activity.transaction = transactions.ToArray();
            }

        }


        public iatiactivityContainer GetAllActivities(string dp)
        {
            var q = (from a in dbContext.Activities
                     where a.AssignedOrgId == dp
                     orderby a.IatiIdentifier
                     select new ActivityModel { IatiActivity = a.IatiActivity, OrgId = a.OrgId }).ToList();

            var iatiActivities = ParseXMLAndResolve(q);


            var aimsActivities = new AimsDAL().GetAIMSProjectsInIATIFormat(dp);

            return new iatiactivityContainer
            {
                DP = dp,
                iatiActivities = iatiActivities,
                AimsProjects = aimsActivities
            };
        }


        public iatiactivityContainer GetNotMappedActivities(string dp)
        {
            var q = (from a in dbContext.Activities
                     let isNotMapped = (a.ProjectId ?? 0) == 0 && (a.MappedProjectId ?? 0) == 0 && (a.MappedTrustFundId ?? 0) == 0
                     where a.AssignedOrgId == dp && isNotMapped
                     orderby a.IatiIdentifier
                     select new ActivityModel { IatiActivity = a.IatiActivity, OrgId = a.OrgId }).ToList();

            var iatiActivities = ParseXMLAndResolve(q);

            var aimsActivities = GetNotMappedAimsProjects(dp);

            return new iatiactivityContainer
            {
                DP = dp,
                iatiActivities = iatiActivities,
                AimsProjects = aimsActivities
            };
        }

        public ProjectFieldMapModel GetTransactionMismatchedActivity(string iatiIdentifier)
        {
            var q = (from a in dbContext.Activities
                     where a.IatiIdentifier == iatiIdentifier
                     select new ActivityModel { IatiActivity = a.IatiActivity, ProjectId = a.ProjectId, OrgId = a.OrgId }).FirstOrDefault();

            var iatiActivity = ParseXMLAndResolve(new List<ActivityModel> { q }).FirstOrDefault();

            LoadChildActivities(iatiActivity);

            var aimsProject = new AimsDAL().GetAIMSProjectInIATIFormat(q.n().ProjectId);

            foreach (var aimsTransaction in aimsProject.transaction)
            {
                var isFoundInIati = iatiActivity.transaction.Any(a => a.transactiontype.n().code == aimsTransaction.transactiontype.n().code
                    && a.transactiondate.n().isodate == aimsTransaction.transactiondate.n().isodate
                    && Math.Floor(a.ValUSD) == Math.Floor(aimsTransaction.ValUSD));

                aimsTransaction.IsConflicted = !isFoundInIati;
            }

            return new ProjectFieldMapModel
            {
                iatiActivity = iatiActivity,
                aimsProject = aimsProject,
            };
        }

        private List<iatiactivity> GetNotMappedAimsProjects(string dp)
        {
            var mappedProjectIds = (from a in dbContext.Activities
                                    let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0 || a.MappedTrustFundId > 0
                                    where a.AssignedOrgId == dp && isMapped
                                    select (a.ProjectId > 0 ? a.ProjectId :
                                    a.MappedProjectId > 0 ? a.MappedProjectId :
                                    a.MappedTrustFundId)).ToList();


            var aimsActivities = new AimsDAL().GetNotMappedAIMSProjectsInIATIFormat(dp, mappedProjectIds);
            return aimsActivities;
        }

        private List<iatiactivity> ParseXMLAndResolve(List<ActivityModel> q)
        {
            var result = new List<iatiactivity>();
            var serializer = new XmlSerializer(typeof(iatiactivity));

            foreach (var a in q)
            {
                using (TextReader reader = new StringReader(a.IatiActivity))
                {
                    a.iatiActivity = (iatiactivity)serializer.Deserialize(reader);
                }
                a.iatiActivity.MappedProjectId = a.MappedProjectId??0;
                a.iatiActivity.MappedTrustFundId = a.MappedTrustFundId??0;

                a.iatiActivity.FundSourceIDnIATICode = new AimsDAL().GetFundSourceIDnIATICode(a.OrgId);

                SetExchangedValues(a.iatiActivity);
                result.Add(a.iatiActivity);
            }
            return result;
        }

        public void SetExchangedValues(iatiactivity activity)
        {
            if (activity.transaction != null)
                foreach (var tr in activity.transaction)
                {
                    SetCurrencyExRateAndVal(tr, activity.defaultcurrency, tr.transactiondate.n().isodate);
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

            tr.value.BBexchangeRateDate = curExchangeRate.n().DATE.ToSqlDateTime();
            tr.value.BBexchangeRateUSD = curExchangeRate.n().DOLLAR_PER_CURRENCY ?? 0;
            tr.value.ValueInUSD = tr.value.Value * tr.value.BBexchangeRateUSD;
            tr.value.BBexchangeRateBDT = curExchangeRate.n().TAKA_PER_DOLLAR ?? 0;
            tr.value.ValueInBDT = tr.value.ValueInUSD * tr.value.BBexchangeRateBDT;

        }



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
                         IatiActivity = a.IatiActivity
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
                     where a.AssignedOrgId == dp && isNotMapped
                     select 1).Count();
            return q;
        }
        public int GetMappedActivityCount(string dp)
        {
            var q = (from a in dbContext.Activities
                     let isMapped = a.ProjectId > 0 || a.MappedProjectId > 0 || a.MappedTrustFundId > 0
                     where a.AssignedOrgId == dp && isMapped
                     select 1).Count();
            return q;
        }

        public int InsertLog(Log log)
        {
            dbContext.Logs.Add(log);
            return dbContext.SaveChanges();
        }

        public List<Log> GetLastDayLogs(string dp)
        {
            Log lastLog = dbContext.Logs.Where(w => w.OrgId == dp).OrderByDescending(o => o.Id).FirstOrDefault();
            DateTime lastDate = lastLog.n().DateTime.n().Value.Date;
            var logs = dbContext.Logs.Where(w => w.OrgId == dp && w.DateTime >= lastDate).ToList();

            return logs;
        }
        /// <summary>
        /// same as Activity table in AIMS_DB_IATI database
        /// </summary>
        public class ActivityModel
        {
            public int Id { get; set; }
            public string OrgId { get; set; }
            public string IatiIdentifier { get; set; }
            public string IatiActivity { get; set; }
            public Nullable<System.DateTime> DownloadDate { get; set; }
            public string IatiActivityPrev { get; set; }
            public Nullable<System.DateTime> DownloadDatePrev { get; set; }
            public Nullable<int> Hierarchy { get; set; }
            public Nullable<int> ParentHierarchy { get; set; }
            public string AssignedOrgId { get; set; }
            public string AssignedOrgName { get; set; }
            public Nullable<System.DateTime> AssignedDate { get; set; }
            public Nullable<int> ProjectId { get; set; }
            public Nullable<int> MappedProjectId { get; set; }
            public Nullable<int> MappedTrustFundId { get; set; }


            public iatiactivity iatiActivity { get; set; }
        }








    }
}

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

        //static AimsDbIatiDAL()
        //{
        //    ExchangeRates = new List<ExchangeRateModel>();
        //}
        public AimsDbIatiDAL()
        {
            ExchangeRates = new List<ExchangeRateModel>();
        }
        List<ExchangeRateModel> ExchangeRates
        {
            get;
            set;
        }

        public int SaveAtivities(List<Activity> activities)
        {
            foreach (var activity in activities)
            {
                var a = dbContext.Activities.FirstOrDefault(x => x.IatiIdentifier == activity.IatiIdentifier);
                if (a != null)
                {
                    a.OrgId = activity.OrgId;
                    a.IatiIdentifier = activity.IatiIdentifier;

                    a.IatiActivityPrev = a.IatiActivity;
                    a.IatiActivity = activity.IatiActivity;

                    a.Hierarchy = activity.Hierarchy;
                    a.ParentHierarchy = activity.ParentHierarchy;

                    //a.AssignedOrgId = activity.AssignedOrgId;
                    //a.AssignedDate = DateTime.Now;

                    a.DownloadDatePrev = a.DownloadDate;
                    a.DownloadDate = DateTime.Now;
                }
                else
                {
                    activity.DownloadDate = DateTime.Now;
                    dbContext.Activities.Add(activity);
                }

            }

            return dbContext.SaveChanges();
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



        public CFnTFModel GetAssignActivities(string dp)
        {
            var q = from a in dbContext.Activities
                    where a.OrgId != dp && a.AssignedOrgId == dp
                    orderby a.IatiIdentifier
                    select a.IatiActivity;


            var result = new List<iatiactivity>();
            var activity = new iatiactivity();
            var serializer = new XmlSerializer(typeof(iatiactivity));

            foreach (var a in q)
            {
                using (TextReader reader = new StringReader(a))
                {
                    activity = (iatiactivity)serializer.Deserialize(reader);

                }
                result.Add(activity);

                if (activity.HasRelatedActivity)
                {
                    var relatedActivities = new List<iatiactivity>();
                    var relatedActivity = new iatiactivity();

                    var ras = from ac in dbContext.Activities
                              where ac.IatiIdentifier.StartsWith(activity.IatiIdentifier)
                              select ac.IatiActivity;

                    foreach (var ac in ras)
                    {
                        using (TextReader reader = new StringReader(ac))
                        {
                            relatedActivity = (iatiactivity)serializer.Deserialize(reader);
                        }
                        relatedActivities.Add(relatedActivity);
                    }


                    List<transaction> transactions = new List<transaction>();

                    foreach (var ra in relatedActivities)
                    {
                        if (ra.transaction != null)
                            transactions.AddRange(ra.transaction);

                        SetExchangedValues(ra);
                    }
                    activity.transaction = transactions.ToArray();
                }

                SetExchangedValues(activity);

            }


            return new CFnTFModel
            {
                AssignedActivities = result,
                AimsProjects = new AimsDAL().GetAIMSDataInIATIFormat(dp)
            };
        }

        public iatiactivityContainer GetActivities(string dp)
        {
            var q = from a in dbContext.Activities
                    where a.OrgId == dp
                    orderby a.IatiIdentifier
                    select a.IatiActivity;

            var result = new List<iatiactivity>();
            var activity = new iatiactivity();
            var serializer = new XmlSerializer(typeof(iatiactivity));

            foreach (var a in q)
            {
                using (TextReader reader = new StringReader(a))
                {
                    activity = (iatiactivity)serializer.Deserialize(reader);

                }

                SetExchangedValues(activity);

                result.Add(activity);
            }


            return new iatiactivityContainer
            {
                DP = dp,
                iatiActivities = result,
                AimsProjects = new AimsDAL().GetAIMSDataInIATIFormat(dp)
            };
        }

        private void SetExchangedValues(iatiactivity activity)
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

        private void SetCurrencyExRateAndVal(ICurrency tr, string defaultcurrency, DateTime trDate = default(DateTime))
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

            tr.value.BBexchangeRateDate = curExchangeRate.n().DATE;
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
                var a = dbContext.FieldMappingPreferenceActivities.FirstOrDefault(x => x.IATIIdentifier == fieldMap.IATIIdentifier
                    //&& x.ProjectId == fieldMap.ProjectId
                                                                                    && x.FieldName == fieldMap.FieldName);
                if (a != null)
                {
                    a.IATIIdentifier = fieldMap.IATIIdentifier;
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

        public List<FieldMappingPreferenceGeneral> GetFieldMappingPreferenceGeneral(string dp)
        {
            var q = (from fieldMap in dbContext.FieldMappingPreferenceGenerals.Where(w => w.OrgId == dp)
                     select fieldMap).ToList();

            return q;
        }

        public List<FieldMappingPreferenceActivity> GetFieldMappingPreferenceActivity(string iatiIdentifier)
        {
            var q = (from fieldMap in dbContext.FieldMappingPreferenceActivities
                     where fieldMap.IATIIdentifier == iatiIdentifier
                     select fieldMap).ToList();

            return q;
        }

        public DateTime? GetLastDownloadDate(string dp)
        {
            var q = (from a in dbContext.Activities.Where(a => a.OrgId == dp)
                     orderby a.DownloadDate descending
                     select a.DownloadDate).FirstOrDefault();

            return q;

        }
        public List<ActivityModel> GetDelegatedActivities(string dp)
        {
            var q = (from a in dbContext.Activities.Where(a => a.OrgId == dp && a.AssignedOrgId != dp)

                     select new ActivityModel
                     {
                         IatiIdentifier = a.IatiIdentifier,
                         AssignedOrgId = a.AssignedOrgId,
                         AssignedDate = a.AssignedDate
                     }).ToList();


            //var result = ParseActivityXML(q);


            return q;

        }

        public int GetNewActivityCount(string dp)
        {
            var q = dbContext.Activities.Where(w => w.OrgId == dp && w.MappedProjectId == null && w.MappedTrustFundId == null).Count();
            return q;
        }

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
            public Nullable<int> MappedProjectId { get; set; }
            public Nullable<int> MappedTrustFundId { get; set; }
        }



    }
}

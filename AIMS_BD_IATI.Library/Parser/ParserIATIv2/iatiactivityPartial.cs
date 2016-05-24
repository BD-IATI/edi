using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AIMS_BD_IATI.Library.Parser.ParserIATIv2
{
    public class ServerSideAttribute:Attribute
    {

    }
    [Serializable]
    public class iatiactivityContainer
    {
        public iatiactivityContainer()
        {
            iatiActivities = new List<iatiactivity>();
            NewProjects = new List<iatiactivity>();
        }
        ////public string Id { get; set; }
        public List<iatiactivity> iatiActivities { get; set; }
        public List<iatiactivity> RelevantActivities { get { return iatiActivities?.FindAll(f => f.IsRelevant == true); } }
        public List<iatiactivity> NewProjects { get; set; }
        public List<iatiactivity> AimsProjects { get; set; }

        public bool HasChildActivity { get { return iatiActivities.Exists(e => e.relatedactivity?.Count(r => r != null && r.type == "2") > 0); } }


    }
    [Serializable]
    public class TrustFundModel
    {
        //public string Id { get; set; }
        public int TrustFundId { get; set; }
        public string TFIdentifier { get; set; }
        public int FundSourceId { get; set; }
        public List<transaction> transactionsInAims { get; set; }
        public List<iatiactivity> iatiactivities { get; set; }
        public decimal TotalCommitment { get { return transactionsInAims.Count > 0 ? transactionsInAims.Sum(s => s.value.ValueInUSD) : 0; } }
        public TrustFundModel()
        {
            transactionsInAims = new List<transaction>();
            iatiactivities = new List<iatiactivity>();
        }

    }
    [Serializable]
    public class CFnTFModel
    {
        //public string Id { get; set; }
        public List<iatiactivity> AimsProjects { get; set; }
        public List<iatiactivity> AssignedActivities { get; set; }
        public List<LookupItem> TrustFunds { get; set; }
        public List<TrustFundModel> TrustFundDetails { get; set; }

        public CFnTFModel()
        {
            AimsProjects = new List<iatiactivity>();
            AssignedActivities = new List<iatiactivity>();
            TrustFunds = new List<LookupItem>();
            TrustFundDetails = new List<TrustFundModel>();
        }
    }

    public partial class iatiactivity
    {

        public iatiactivity()
        {
            childActivities = new List<iatiactivity>();
            MatchedProjects = new List<iatiactivity>();
        }
        [XmlIgnore]
        public static List<FundSourceLookupItem> FundSources { get; set; }
        [XmlIgnore]
        public bool IsDataSourceAIMS { get; set; }

        #region co-financed and trust fund projects
        [XmlIgnore]
        public bool IsCofinancedProject { get; set; }
        [XmlIgnore]
        public bool IsTrustFundedProject { get; set; }
        [XmlIgnore]
        public bool IsCommitmentIncluded { get; set; }
        [XmlIgnore]
        public bool IsDisbursmentIncluded { get; set; }
        [XmlIgnore]
        public bool IsPlannedDisbursmentIncluded { get; set; }

        #endregion co-financed and trust fund projects

        [XmlIgnore]
        public bool IsInclude { get; set; } //AIMS ProjectId
        [XmlIgnore]
        public int ProjectId { get; set; } //AIMS ProjectId
        [XmlIgnore]
        public int MappedProjectId { get; set; }
        [XmlIgnore]
        public int MappedTrustFundId { get; set; }

        [XmlIgnore]
        public bool HasChildActivity { get { return (relatedactivity?.Count(r => r != null && r.type == "2") > 0); } }
        [XmlIgnore]
        public bool HasParentActivity { get { return (relatedactivity?.Count(r => r != null && r.type == "1") > 0); } }

        [XmlIgnore]
        public List<iatiactivity> childActivities { get; set; }
        [XmlIgnore]
        private List<iatiactivity> includedChildActivities { get { return childActivities.FindAll(f=>f.IsInclude == true); } }

        [XmlIgnore]
        public List<iatiactivity> MatchedProjects { get; set; }

        [XmlIgnore]
        public decimal PercentToBD
        {
            get
            {
                if (recipientcountry != null)
                {
                    var bd = recipientcountry?.FirstOrDefault(f => f?.code == "BD");

                    if (recipientcountry.Count() == 1)
                        return 100;
                    else if (bd != null)
                        return bd.percentage;
                    else
                        return 0;
                }
                else
                    return 0;

            }
        }

        [XmlIgnore]
        public bool? isRelevant;
        [XmlIgnore]
        public bool? IsRelevant
        {
            get
            {
                return isRelevant ?? PercentToBD >= 20 && activitystatus?.code == "2";
            }
            set
            {
                isRelevant = value;
            }
        }

        #region Financial Data

        #region Commitments
        [XmlIgnore]
        public List<transaction> Commitments
        {
            get
            {
                return GetTransactions(ConvertIATIv2.gettransactionCode("C"));
            }
        }
        [XmlIgnore]
        public decimal TotalCommitment
        {
            get
            {
                return Math.Round(Commitments.Sum(s => s.ValUSD), 2);
            }
        }

        [XmlIgnore]
        public List<transaction> CommitmentsThisDPOnly
        {
            get
            {
                return Commitments.FindAll(f => string.IsNullOrWhiteSpace(f.providerorg?.@ref) || IATICode.Contains(f.providerorg?.@ref));
            }
        }
        [XmlIgnore]
        public decimal TotalCommitmentThisDPOnly
        {
            get
            {
                return Math.Round(CommitmentsThisDPOnly.Sum(s => s.ValUSD), 2);
            }
        }

        #endregion Commitments

        #region Planned Disbursements
        [XmlIgnore]
        public List<planneddisbursement> PlannedDisbursments
        {
            get
            {
                var plannedDisbursments = new List<Parser.ParserIATIv2.planneddisbursement>();
                if (this.budget != null || IsDataSourceAIMS)
                {
                    plannedDisbursments.AddRange(GetPlannedDisbursments(this));
                }
                else
                {
                    foreach (var ra in includedChildActivities)
                    {
                        if (ra.budget != null)
                        {
                            plannedDisbursments.AddRange(GetPlannedDisbursments(ra));
                        }
                    }
                }
                return plannedDisbursments;
            }
        }
        [XmlIgnore]
        public decimal TotalPlannedDisbursment
        {
            get
            {
                return Math.Round(PlannedDisbursments.Sum(s => s.ValUSD), 2);
            }
        }
        #endregion Planned Disbursements

        #region Disbursments
        [XmlIgnore]
        public List<transaction> Disbursments
        {
            get
            {
                var disbursments = GetTransactions(ConvertIATIv2.gettransactionCode("D"));
                //expenditures in IATI are also treated as disbursments in AIMS
                disbursments.AddRange(GetTransactions(ConvertIATIv2.gettransactionCode("E")));
                return disbursments;
            }
        }
        [XmlIgnore]
        public decimal TotalDisbursment
        {
            get
            {
                return Math.Round(Disbursments.Sum(s => s.ValUSD), 2);
            }
        }

        [XmlIgnore]
        public List<transaction> DisbursmentsThisDPOnly
        {
            get
            {
                return Disbursments.FindAll(f => string.IsNullOrWhiteSpace(f.providerorg?.@ref) || IATICode.Contains(f.providerorg?.@ref));
            }
        }
        [XmlIgnore]
        public decimal TotalDisbursmentThisDPOnly
        {
            get
            {
                return Math.Round(DisbursmentsThisDPOnly.Sum(s => s.ValUSD), 2);
            }
        }

        #endregion Disbursments

        #region Helper Methods
        private List<planneddisbursement> GetPlannedDisbursments(iatiactivity activity)
        {
            List<planneddisbursement> planneddisbursements = new List<planneddisbursement>();

            if (activity.planneddisbursement != null)
            {
                //get planned disbursements
                planneddisbursements.AddRange(activity.planneddisbursement);
            }

            if (activity.budget != null)
            {
                budget[] budgets = activity.budget;

                //get planned from budget
                var originalBudgets = budgets.Where(w => w.type == "1").ToList();
                var revisedBudgets = budgets.Where(w => w.type == "2").ToList();

                foreach (var revisedBudget in revisedBudgets)
                {
                    originalBudgets.RemoveAll(r =>
                        (
                        r.periodstart?.isodate >= revisedBudget.periodstart?.isodate && r.periodstart?.isodate <= revisedBudget.periodend?.isodate
                        )
                        || (r.periodend?.isodate >= revisedBudget.periodstart?.isodate && r.periodend?.isodate <= revisedBudget.periodend?.isodate
                        )
                        ||
                         (revisedBudget.periodstart?.isodate >= r.periodstart?.isodate && revisedBudget.periodend?.isodate <= r.periodstart?.isodate
                         )
                         || (revisedBudget.periodstart?.isodate >= r.periodend?.isodate && revisedBudget.periodend?.isodate <= r.periodend?.isodate
                        )
                        );
                }
                var margedBudgets = new List<budget>();
                margedBudgets.AddRange(originalBudgets);
                margedBudgets.AddRange(revisedBudgets);

                foreach (var item in margedBudgets)
                {
                    planneddisbursements.Add(new planneddisbursement
                    {
                        periodstart = new planneddisbursementPeriodstart { isodate = item.periodstart?.isodate??default(DateTime) },
                        periodend = new planneddisbursementPeriodend { isodate = item.periodend?.isodate??default(DateTime) },
                        value = item.value
                    });
                }
            }

            return planneddisbursements;
        }

        private List<transaction> GetTransactions(string transactiontypecode)
        {
            var Transactions = new List<transaction>();

            if (transaction != null)
            {
                Transactions.AddRange(transaction.Where(p => p.transactiontype?.code == transactiontypecode));
            }

            foreach (var ra in includedChildActivities)
            {
                if (ra.transaction != null)
                {
                    Transactions.AddRange(ra.transaction.Where(p => p.transactiontype?.code == transactiontypecode));
                }
            }

            foreach (var ra in MatchedProjects)
            {
                if (ra.transaction != null)
                {
                    Transactions.AddRange(ra.transaction.Where(p => p.transactiontype?.code == transactiontypecode));
                }
            }
            return Transactions;
        }

        #endregion Helper Methods

        #endregion Financial Data

        #region for filter other DP's projects
        [XmlIgnore]
        public string ownedBy;
        [XmlIgnore]
        public string FundSourceIDnIATICode
        {
            get
            {
                return string.IsNullOrWhiteSpace(ownedBy) ? ImplementingOrgs.n(0).FundSourceIDnIATICode : ownedBy;
            }
            set
            {
                ownedBy = value;
                //foreach (var mproject in MatchedProjects)
                //{
                //    mproject.FundSourceIDnIATICode = value;
                //}
            }
        }
        [XmlIgnore]

        public int AimsFundSourceId
        {
            get { return string.IsNullOrWhiteSpace(FundSourceIDnIATICode) ? 0 : Convert.ToInt32(FundSourceIDnIATICode.Split('~')[0]); }
        }
        [XmlIgnore]

        public string FundSource
        {
            get
            {
                FundSourceLookupItem r;
                if (FundSources != null)
                    r = FundSources.FirstOrDefault(f => f.ID == AimsFundSourceId);
                else
                    r = new FundSourceLookupItem();

                return r == null ? "" : r.Name;
            }
        }
        [XmlIgnore]

        public string IATICode
        {
            get { return string.IsNullOrWhiteSpace(FundSourceIDnIATICode) ? "" : FundSourceIDnIATICode.Split('~')[1]; }
        }
        #endregion

        #region Wrappers

        [XmlIgnore]
        public string IatiIdentifier
        {
            get
            {
                var identifier = iatiidentifier?.Value;
                return identifier != null && identifier.Length > 50 ? identifier.Substring(0, 40) + "*truncated" : identifier ?? "";
            }

            set
            {
                if (iatiidentifier == null) iatiidentifier = new iatiidentifier();
                iatiidentifier.Value = value;
            }
        }
        [XmlIgnore]
        public string Title
        {
            get
            {
                return title?.narrative.n(0).Value;
            }
            set
            {
                if (title == null) title = new textRequiredType();
                title.narrative = Statix.getNarativeArray(value);
            }
        }
        [XmlIgnore]
        public string Description
        {
            get
            {
                return description.n(0).narrative.n(0).Value;
            }
            set
            {
                description.n(0).narrative = Statix.getNarativeArray(value);
            }
        }
        [XmlIgnore]
        public string ReportingOrg
        {
            get
            {
                return reportingorg?.narrative.n(0).Value;
            }
            //set
            //{
            //    if (reportingorg == null) reportingorg = new reportingorg();
            //    reportingorg.narrative = Statix.getNarativeArray(value);
            //}
        }
        [XmlIgnore]
        public List<participatingorg> ImplementingOrgs
        {
            get
            {
                return participatingorg?.Where(w => w?.role == "4")?.ToList();
            }
        }

        [XmlIgnore]
        public string AidType
        {
            get
            {
                string code;
                if (defaultaidtype == null) // for initializing
                    code = AidTypeCode;

                return defaultaidtype?.name;
                //return "Undefined";
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    defaultaidtype = new defaultaidtype { name = value };
            }
        }
        [XmlIgnore]
        public string AidTypeCode
        {
            get
            {
                if (defaultaidtype != null)
                {
                    return defaultaidtype.code;
                }
                else
                {
                    defaultaidtype = new defaultaidtype();
                    if (transaction != null)
                    {
                        var commitmentTrans = transaction.Where(c => c.transactiontype?.code == "2");

                        var kk = from t in commitmentTrans
                                 group t by new { t.aidtype, t.value } into g
                                 select new
                                 {
                                     g.Key.aidtype,
                                     Sum = g.Sum(s => s.value?.Value)
                                 };

                        var dominatingAidType = kk.OrderByDescending(k => k.Sum).FirstOrDefault();

                        defaultaidtype.code = dominatingAidType == null ? "" : dominatingAidType.aidtype == null ? "" : dominatingAidType.aidtype.code;

                    }
                    else
                    {
                        var allChildAticitiesTrans = new List<transaction>();
                        foreach (var ra in includedChildActivities)
                        {
                            if (ra.transaction != null)
                            {
                                foreach (var item in ra.transaction)
                                {
                                    if (item.aidtype == null)
                                        item.aidtype = new transactionAidtype { code = ra.defaultaidtype?.code };
                                    allChildAticitiesTrans.Add(item);

                                }
                            }

                        }

                        var commitmentTrans = allChildAticitiesTrans.FindAll(c => c.transactiontype?.code == "2");

                        var kk = (from t in commitmentTrans
                                  group t by new { t.aidtype, t.value } into g
                                  select new
                                  {
                                      g.Key.aidtype,
                                      Sum = g.Sum(s => s.value?.Value)
                                  }).ToList();

                        var dominatingAidType = kk.OrderByDescending(k => k.Sum).FirstOrDefault();

                        defaultaidtype.code = dominatingAidType == null ? "" : dominatingAidType.aidtype == null ? "" : dominatingAidType.aidtype.code;


                        //defaultaidtype.code = "A01";

                    }

                    return defaultaidtype.code;
                }
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    defaultaidtype = new Parser.ParserIATIv2.defaultaidtype { code = value };
            }
        }

        [XmlIgnore]
        public string ActivityStatus
        {
            get
            {
                return activitystatus?.name;
            }
            set
            {
                if (activitystatus == null) activitystatus = new activitystatus();
                activitystatus.name = value;
            }

        }

        #region activitydate
        [XmlIgnore]
        public DateTime PlannedStartDate
        {
            get
            {
                var sdate = activitydate?.FirstOrDefault(f => f?.type == "1");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //1
        [XmlIgnore]
        public DateTime ActualStartDate
        {
            get
            {
                var sdate = activitydate?.FirstOrDefault(f => f?.type == "2");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //2
        [XmlIgnore]
        public DateTime PlannedEndDate
        {
            get
            {
                var sdate = activitydate?.FirstOrDefault(f => f?.type == "3");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //3
        [XmlIgnore]
        public DateTime ActualEndDate
        {
            get
            {
                var sdate = activitydate?.FirstOrDefault(f => f?.type == "4");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //4 
        #endregion
        #endregion

    }

    public partial class defaultaidtype
    {

        [XmlIgnore]
        public string name
        {
            get
            {
                if (code == "A01") return "General budget support";
                else if (code == "A02") return "Sector budget support";
                else if (code == "B01") return "Core support to NGOs, other private bodies, PPPs and research institutes";
                else if (code == "B02") return "Core contributions to multilateral institutions";
                else if (code == "B03") return "Contributions to specific-purpose programmes and funds managed by international organisations (multilateral, INGO)";
                else if (code == "B04") return "Basket funds/pooled funding";
                else if (code == "C01") return "Project-type interventions";
                else if (code == "D01") return "Donor country personnel";
                else if (code == "D02") return "Other technical assistance";
                else if (code == "E01") return "Scholarships/training in donor country";
                else if (code == "E02") return "Imputed student costs";
                else if (code == "F01") return "Debt relief";
                else if (code == "G01") return "Administrative costs not included elsewhere";
                else if (code == "H01") return "Development awareness";
                else if (code == "H02") return "Refugees in donor countries";
                else
                    return "";
            }
            set
            {
                if (value == "General budget support") code = "A01";
                else if (value == "Sector budget support") code = "A02";
                else if (value == "Core support to NGOs, other private bodies, PPPs and research institutes") code = "B01";
                else if (value == "Core contributions to multilateral institutions") code = "B02";
                else if (value == "Contributions to specific-purpose programmes and funds managed by international organisations (multilateral, INGO)") code = "B03";
                else if (value == "Basket funds/pooled funding") code = "B04";
                else if (value == "Project-type interventions") code = "C01";
                else if (value == "Donor country personnel") code = "D01";
                else if (value == "Other technical assistance") code = "D02";
                else if (value == "Scholarships/training in donor country") code = "E01";
                else if (value == "Imputed student costs") code = "E02";
                else if (value == "Debt relief") code = "F01";
                else if (value == "Administrative costs not included elsewhere") code = "G01";
                else if (value == "Development awareness") code = "H01";
                else if (value == "Refugees in donor countries") code = "H02";
                else
                    code = "";
            }

        }
    }
    public partial class activitystatus
    {
        [XmlIgnore]
        public string name
        {
            get
            {
                if (code == "1") return "Pipeline/identification";
                else if (code == "2") return "Implementation";
                else if (code == "3") return "Completion";
                else if (code == "4") return "Post-completion";
                else if (code == "5") return "Cancelled";
                else if (code == "6") return "Suspended";
                else
                    return "";
            }
            set
            {
                if (value == "Pipeline/identification") code = "1";
                else if (value == "Implementation") code = "2";
                else if (value == "Completion") code = "3";
                else if (value == "Post-completion") code = "4";
                else if (value == "Cancelled") code = "5";
                else if (value == "Suspended") code = "6";
                else
                    code = "";
            }


        }



    }

    public partial class participatingorg
    {
        [XmlIgnore]
        public string FundSourceIDnIATICode { get; set; }

        public int? _ExecutingAgencyTypeId { get; set; }
        [XmlIgnore]
        public int? ExecutingAgencyTypeId { get { return _ExecutingAgencyTypeId ?? GetAimsExAgencyTypeIdByOrgType(type); } set { _ExecutingAgencyTypeId = value; } }


        [XmlIgnore]
        public int ExecutingAgencyOrganizationId { get { return string.IsNullOrEmpty(AllID) ? default(int) : AllID.Split('~').n(0).ToInt32(); } }

        [XmlIgnore]
        public int ExecutingAgencyOrganizationTypeId { get { return string.IsNullOrEmpty(AllID) ? default(int) : AllID.Split('~').n(3).ToInt32(); } }


        [XmlIgnore]
        public string AllID { get; set; }

        [XmlIgnore]
        public string Name
        {
            get
            {
                return narrative.n(0).Value;
            }
            set
            {
                narrative = Statix.getNarativeArray(value);
            }
        }

        private int? GetAimsExAgencyTypeIdByOrgType(string t)
        {
            int? r = null;

            //10  Government
            //15  Other Public Sector
            //21  International NGO
            //22  National NGO
            //23  Regional NGO
            //30  Public Private Partnership
            //40  Multilateral
            //60  Foundation
            //70  Private Sector
            //80  Academic, Training and Research

            if (t == "10")
                r = (int)ExecutingAgencyType.Government;
            else if (t == "21" || t == "22" || t == "23")
                r = (int)ExecutingAgencyType.NGO;
            else r = (int)ExecutingAgencyType.Others;

            return r;
        }
    }


    public partial class currencyType
    {
        [XmlIgnore]
        public DateTime BBexchangeRateDate { get; set; }

        [XmlIgnore]
        public decimal ValueInUSD { get; set; }

        [XmlIgnore]
        public decimal BBexchangeRateUSD { get; set; }

        [XmlIgnore]
        public decimal ValueInBDT { get; set; }

        [XmlIgnore]
        public decimal BBexchangeRateBDT { get; set; }


    }

    public partial class transaction : ICurrency
    {
        [XmlIgnore]
        public string ProviderOrg
        {
            get
            {
                return providerorg?.narrative.n(0).Value;
            }
            set
            {
                if (providerorg == null) providerorg = new transactionProviderorg();
                providerorg.narrative = Statix.getNarativeArray(value);
            }
        }

        [XmlIgnore]
        public decimal ValUSD
        {
            get
            {
                return value?.ValueInUSD??0;
            }
        }
        [XmlIgnore]
        public bool IsConflicted { get; set; }

    }
    public partial class planneddisbursement : ICurrency
    {
        [XmlIgnore]
        public string ProviderOrg
        {
            get
            {
                return providerorg?.narrative.n(0).Value;
            }
            set
            {
                if (providerorg == null) providerorg = new planneddisbursementProviderorg();
                providerorg.narrative = Statix.getNarativeArray(value);
            }
        }

        [XmlIgnore]
        public decimal ValUSD
        {
            get
            {
                return value?.ValueInUSD??0;
            }
        }
    }
    public partial class budget : ICurrency
    {
    }
    public partial class locationPoint
    {
        public double GetLatitude()
        {
            double lat = 0;
            double.TryParse(pos.Substring(0, pos.IndexOf(' ')), out lat);
            return lat;
        }
        public double GetLongitude()
        {
            double lon = 0;
            double.TryParse(pos.Substring(pos.IndexOf(' ') + 1), out lon);
            return lon;
        }

        public GeoCoordinate GetGeoCoordinate()
        {
            return new GeoCoordinate(GetLatitude(), GetLongitude());
        }

    }
}



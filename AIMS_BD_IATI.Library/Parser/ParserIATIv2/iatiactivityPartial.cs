using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AIMS_BD_IATI.Library.Parser.ParserIATIv2
{

    public class iatiactivityContainer
    {
        public string DP { get; set; }
        public iatiactivityContainer()
        {
            DP = "";
            iatiActivities = new List<iatiactivity>();
            NewProjects = new List<iatiactivity>();
        }
        public List<iatiactivity> iatiActivities { get; set; }
        public List<iatiactivity> RelevantActivities { get { return iatiActivities.n().FindAll(f => f.IsRelevant == true); } }
        public List<iatiactivity> NewProjects { get; set; }
        public List<iatiactivity> AimsProjects { get; set; }

        public bool HasRelatedActivity { get { return iatiActivities.Exists(e => e.relatedactivity.n().Count(r => r != null && r.type == "2") > 0); } }


    }

    public partial class iatiactivity
    {
        public static List<FundSourceLookupItem> FundSources { get; set; }
        public bool IsDataSourceAIMS { get; set; }

        public iatiactivity()
        {
            relatedIatiActivities = new List<iatiactivity>();
            MatchedProjects = new List<iatiactivity>();
        }

        public bool HasRelatedActivity { get { return (relatedactivity.n().Count(r => r != null && r.type == "2") > 0); } }

        [XmlIgnore]
        public int ProjectId { get; set; } //AIMS ProjectId
        [XmlIgnore]
        public int MappedProjectId { get; set; }
        [XmlIgnore]
        public int MappedTrustFundId { get; set; }

        [XmlIgnore]
        public List<iatiactivity> relatedIatiActivities { get; set; }

        [XmlIgnore]
        public List<iatiactivity> MatchedProjects { get; set; }

        [XmlIgnore]
        public decimal PercentToBD
        {
            get
            {
                if (recipientcountry != null)
                {
                    var bd = recipientcountry.n().FirstOrDefault(f => f.n().code == "BD");

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

        #region Financial Data
        [XmlIgnore]
        public decimal TotalCommitment
        {
            get
            {
                return GetTotalTransactionAmt(ConvertIATIv2.gettransactionCode("C"));
            }
        }
        [XmlIgnore]
        public List<transaction> Disbursments
        {
            get
            {
                return GetTransactions(ConvertIATIv2.gettransactionCode("D"));
            }
        }
        [XmlIgnore]
        public List<transaction> Commitments
        {
            get
            {
                return GetTransactions(ConvertIATIv2.gettransactionCode("C"));
            }
        }


        [XmlIgnore]
        public decimal TotalPlannedDisbursment
        {
            get
            {
                return Math.Round(PlannedDisbursments.Sum(s => s.value.n().ValueInUSD), 2);
            }
        }
        [XmlIgnore]
        public List<Parser.ParserIATIv2.planneddisbursement> PlannedDisbursments
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
                    foreach (var ra in relatedIatiActivities)
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

        private List<Parser.ParserIATIv2.planneddisbursement> GetPlannedDisbursments(iatiactivity activity)
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
                        r.periodstart.n().isodate >= revisedBudget.periodstart.n().isodate && r.periodstart.n().isodate <= revisedBudget.periodend.n().isodate
                        )
                        || (r.periodend.n().isodate >= revisedBudget.periodstart.n().isodate && r.periodend.n().isodate <= revisedBudget.periodend.n().isodate
                        )
                        ||
                         (revisedBudget.periodstart.n().isodate >= r.periodstart.n().isodate && revisedBudget.periodend.n().isodate <= r.periodstart.n().isodate
                         )
                         || (revisedBudget.periodstart.n().isodate >= r.periodend.n().isodate && revisedBudget.periodend.n().isodate <= r.periodend.n().isodate
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
                        periodstart = new planneddisbursementPeriodstart { isodate = item.periodstart.n().isodate },
                        periodend = new planneddisbursementPeriodend { isodate = item.periodend.n().isodate },
                        value = item.value
                    });
                }
            }

            return planneddisbursements;
        }


        [XmlIgnore]
        public decimal TotalDisbursment
        {
            get
            {
                return GetTotalTransactionAmt(ConvertIATIv2.gettransactionCode("D"))
                    + (IsDataSourceAIMS == false ? GetTotalTransactionAmt(ConvertIATIv2.gettransactionCode("E")) : 0);
            }
        }

        private decimal GetTotal(transaction[] _transaction, string transactiontypecode)
        {
            var tobj = _transaction.Where(p => p.transactiontype.n().code == transactiontypecode);
            return tobj == null ? 0 : Math.Round(tobj.Sum(s => s.value.n().ValueInUSD), 2); ;
        }

        private decimal GetTotalTransactionAmt(string transactiontypecode)
        {
            decimal total = 0;

            if (transaction != null)
            {
                total = GetTotal(transaction, transactiontypecode);
            }

            foreach (var ra in relatedIatiActivities)
            {
                if (ra.transaction != null)
                {
                    total += GetTotal(ra.transaction, transactiontypecode);
                }
            }

            return total;
        }

        private List<transaction> GetTransactions(string transactiontypecode)
        {
            var Transactions = new List<transaction>();

            if (transaction != null)
            {
                Transactions.AddRange(transaction.Where(p => p.transactiontype.n().code == transactiontypecode));
            }

            foreach (var ra in relatedIatiActivities)
            {
                if (ra.transaction != null)
                {
                    Transactions.AddRange(ra.transaction.Where(p => p.transactiontype.n().code == transactiontypecode));
                }
            }

            return Transactions;
        }
        #endregion

        [XmlIgnore]
        private bool? isRelevant;
        [XmlIgnore]
        public bool? IsRelevant
        {
            //ToDo add AidType criteria
            get
            {
                return isRelevant ?? PercentToBD >= 20 && activitystatus.n().code == "2";
            }
            set
            {
                isRelevant = value;
            }
        }

        #region for filter other DP's projects
        [XmlIgnore]
        private string ownedBy;
        [XmlIgnore]

        public string FundSourceIDnIATICode
        {
            get
            {
                return string.IsNullOrWhiteSpace(ownedBy) ? participatingorg.n().FirstOrDefault(f => f.n().role == "4").n().FundSourceIDnIATICode : ownedBy;
            }
            set
            {
                ownedBy = value;
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
                return iatiidentifier.n().Value;
            }

            set
            {
                iatiidentifier.n().Value = value;
            }
        }
        [XmlIgnore]
        public string Title
        {
            get
            {
                return title.n().narrative.n(0).Value;
            }
            set
            {
                title.n().narrative = Statix.getNarativeArray(value);
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
                return reportingorg.n().narrative.n(0).Value;
            }
            set
            {
                reportingorg.n().narrative = Statix.getNarativeArray(value);
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

                return defaultaidtype.n().name;
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
                        var commitmentTrans = transaction.Where(c => c.transactiontype.n().code == "2");

                        var kk = from t in commitmentTrans
                                 group t by new { t.aidtype, t.value } into g
                                 select new
                                 {
                                     g.Key.aidtype,
                                     Sum = g.Sum(s => s.value.n().Value)
                                 };

                        var dominatingAidType = kk.OrderByDescending(k => k.Sum).FirstOrDefault();

                        defaultaidtype.code = dominatingAidType == null ? "" : dominatingAidType.aidtype == null ? "" : dominatingAidType.aidtype.code;

                    }
                    else
                    {
                        var allChildAticitiesTrans = new List<transaction>();
                        foreach (var ra in relatedIatiActivities)
                        {
                            if (ra.transaction != null)
                            {
                                foreach (var item in ra.transaction)
                                {
                                    if (item.aidtype == null)
                                        item.aidtype = new transactionAidtype { code = ra.defaultaidtype.n().code };
                                    allChildAticitiesTrans.Add(item);

                                }
                            }

                        }

                        var commitmentTrans = allChildAticitiesTrans.FindAll(c => c.transactiontype.n().code == "2");

                        var kk = (from t in commitmentTrans
                                  group t by new { t.aidtype, t.value } into g
                                  select new
                                  {
                                      g.Key.aidtype,
                                      Sum = g.Sum(s => s.value.n().Value)
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
                return activitystatus.n().name;
            }
            set
            {
                activitystatus.n().name = value;
            }

        }

        #region activitydate
        [XmlIgnore]
        public DateTime PlannedStartDate
        {
            get
            {
                var sdate = activitydate.n().FirstOrDefault(f => f.n().type == "1");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //1
        [XmlIgnore]
        public DateTime ActualStartDate
        {
            get
            {
                var sdate = activitydate.n().FirstOrDefault(f => f.n().type == "2");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //2
        [XmlIgnore]
        public DateTime PlannedEndDate
        {
            get
            {
                var sdate = activitydate.n().FirstOrDefault(f => f.n().type == "3");
                return sdate == null ? default(DateTime) : sdate.isodate;
            }
        } //3
        [XmlIgnore]
        public DateTime ActualEndDate
        {
            get
            {
                var sdate = activitydate.n().FirstOrDefault(f => f.n().type == "4");
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




    }


    public partial class currencyType
    {



        [XmlIgnore]
        public decimal ValueInUSD
        {
            get
            ;
            set;
        }

        [XmlIgnore]
        public decimal BBexchangeRateUSD
        {
            get;
            set;
        }
        [XmlIgnore]
        public DateTime BBexchangeRateDate
        {
            get;
            set;
        }

    }

    public partial class transaction : ITransaction
    {
        [XmlIgnore]
        public int AIMSID { get; set; }
    }


}



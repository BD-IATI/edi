using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_BD_IATI.Library;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace AIMS_BD_IATI.DAL
{
    public class AimsDAL
    {
        AIMS_DBEntities dbContext = new AIMS_DBEntities();
        public List<DPLookupItem> GetFundSources(string userId)
        {
            var FundSource = new List<DPLookupItem>();

            var userInfo = dbContext.tblUserRegistrationInfoes.FirstOrDefault(f => f.UserId == userId);

            if (userInfo != null && userInfo.UserId != "guest")
            {
                if (userInfo.ProjectPermissionType == 0) //0 = All
                {
                    FundSource = (from fundSource in dbContext.tblFundSources
                                  where fundSource.IATICode != null && !string.IsNullOrEmpty(fundSource.IATICode)
                                  orderby fundSource.FundSourceName
                                  select new DPLookupItem
                                  {
                                      ID = fundSource.IATICode,
                                      Name = fundSource.FundSourceName + " (" + (fundSource.Acronym ?? "") + ")",
                                      AimsFundSourceId = fundSource.Id
                                  }).ToList();
                }
                else
                {
                    if (userInfo.ProjectPermissionType == 1) //1= Projectwise
                    {
                        FundSource = null;
                    }
                    else if (userInfo.ProjectPermissionType == 2)
                    {
                        FundSource = (from fundSource in dbContext.tblFundSources
                                      join permitted in dbContext.tblUserFundSources.Where(permitted => permitted.UserId == userInfo.Id) on fundSource.Id equals permitted.FundSourceId
                                      where fundSource.IATICode != null && !string.IsNullOrEmpty(fundSource.IATICode)
                                      orderby fundSource.FundSourceName
                                      select new DPLookupItem
                                      {
                                          ID = fundSource.IATICode,
                                          Name = fundSource.FundSourceName + " (" + (fundSource.Acronym ?? "") + ")",
                                          AimsFundSourceId = fundSource.Id
                                      }).ToList();

                    }
                }
            }
            return FundSource;
        }


        private string getIdentifer(tblProjectInfo project)
        {
            return string.IsNullOrWhiteSpace(project.IatiIdentifier) ?
                project.DPProjectNo //project.DPProjectNo.n().StartsWith(project.tblFundSource.n().IATICode) ? project.DPProjectNo : project.tblFundSource.n().IATICode + "-" + project.DPProjectNo
                : project.IatiIdentifier;
        }
        public List<tblFundSource> GetFundSources()
        {
            var fundSources = from fundSource in dbContext.tblFundSources
                              where fundSource.IATICode != null && !string.IsNullOrEmpty(fundSource.IATICode)
                              orderby fundSource.FundSourceName
                              select fundSource;

            return fundSources.ToList();
        }
        public List<DPLookupItem> GetFundSourcesDropdownData()
        {
            var fundSources = from fundSource in dbContext.tblFundSources
                              where fundSource.IATICode != null && !string.IsNullOrEmpty(fundSource.IATICode)
                              orderby fundSource.FundSourceName
                              select new DPLookupItem
                              {
                                  ID = fundSource.IATICode,
                                  Name = fundSource.FundSourceName + " (" + (fundSource.Acronym ?? "") + ")",
                                  AimsFundSourceId = fundSource.Id
                              };

            return fundSources.ToList();
        }
        /// <summary>
        /// Get Managing DPs
        /// </summary>
        /// <returns></returns>
        public List<FundSourceLookupItem> GetAllFundSources()
        {
            var piList = dbContext.tblProjectInfoes.GroupBy(q => q.FundSourceId).Select(x => x.FirstOrDefault().FundSourceId).ToList();

            var fundSources = (from fundSource in dbContext.tblFundSources
                               where piList.Contains(fundSource.Id)
                               orderby fundSource.FundSourceName
                               select new FundSourceLookupItem
                               {
                                   ID = fundSource.Id,
                                   Name = fundSource.FundSourceName + " (" + (fundSource.Acronym ?? "") + ")",
                                   IATICode = fundSource.IATICode
                               }).ToList();

            return fundSources;
        }

        public List<LookupItem> GetProjects(string dp)
        {
            var projects = (from project in dbContext.tblProjectInfoes
                            join fundSource in dbContext.tblFundSources on project.FundSourceId equals fundSource.Id
                            where fundSource.IATICode == dp
                            orderby project.Title
                            select new LookupItem
                            {
                                ID = project.Id,
                                Name = project.Title,
                            }).ToList();

            return projects;
        }
        public List<LookupItem> GetTrustFunds(string dp)
        {
            var projects = (from trustFund in dbContext.tblTrustFunds
                            join fundSource in dbContext.tblFundSources on trustFund.TFFundSourceId equals fundSource.Id
                            where fundSource.IATICode == dp
                            orderby trustFund.TFIdentifier
                            select new LookupItem
                            {
                                ID = trustFund.Id,
                                Name = trustFund.TFIdentifier,
                            }).ToList();

            return projects;
        }

        public TrustFundModel GetTrustFundDetails(int trustFundId)
        {
            TrustFundModel trustFundModel = new TrustFundModel();

            var trustFundDetails = (from trustFund in dbContext.tblTrustFunds
                              join trustFundDetail in dbContext.tblTrustFundDetails on trustFund.Id equals trustFundDetail.TrustFundId
                              join fundSource in dbContext.tblFundSources on trustFundDetail.TFDFundSourceId equals fundSource.Id

                              where trustFund.Id == trustFundId

                              select new
                              {
                                  Id = trustFund.Id,
                                  TFIdentifier = trustFund.TFIdentifier,
                                  FundSourceName = fundSource.FundSourceName,
                                  Amount = trustFundDetail.TFDAmountInUSD,
                              }).ToList();

            trustFundModel.Id = trustFundDetails.n(0).Id;
            trustFundModel.TFIdentifier = trustFundDetails.n(0).TFIdentifier;

            foreach (var trustFundDetail in trustFundDetails)
            {

                trustFundModel.transactionsInAims.Add(new transaction
                {
                    transactiontype = new transactionTransactiontype { code = ConvertIATIv2.gettransactionCode("C") },
                    providerorg = new transactionProviderorg { narrative = Statix.getNarativeArray(trustFundDetail.FundSourceName) },
                    value = new currencyType { currency = Statix.Currency, Value = trustFundDetail.Amount ?? 0 },
                });
            }


            return trustFundModel;
        }

        public int? UpdateProjects(List<iatiactivity> projects, string Iuser)
        {
            var aimsCurrencies = from c in dbContext.tblCurrencies
                                 select new { Id = c.Id, IATICode = c.IATICode };

            var aimsAidCategories = from c in dbContext.tblAidCategories
                                    select new { Id = c.Id, IATICode = c.IATICode };
            try
            {
                foreach (var project in projects)
                {
                    var p = dbContext.tblProjectInfoes.FirstOrDefault(f => f.Id == project.ProjectId);
                    if (p == null)
                    {
                        p = new tblProjectInfo();
                        p.IDate = DateTime.Now;
                        p.IUser = Iuser;
                        dbContext.tblProjectInfoes.Add(p);
                    }


                    p.Title = project.Title;
                    p.Objective = project.Description;

                    var defaultfinancetype = "100";
                    if (project.defaultfinancetype != null && !string.IsNullOrWhiteSpace(project.defaultfinancetype.code))
                        defaultfinancetype = project.defaultfinancetype.code.StartsWith("4") ? "400" : "100"; 

                    #region Commitments
                    var coms = p.tblProjectFundingCommitments.ToList();
                    foreach (var cc in coms)
                    {
                        dbContext.tblProjectFundingCommitments.Remove(cc);
                    }

                    foreach (var trn in project.Commitments)
                    {

                        var aimsCommitment = new tblProjectFundingCommitment();
                        p.tblProjectFundingCommitments.Add(aimsCommitment);
                        aimsCommitment.IDate = DateTime.Now;
                        aimsCommitment.IUser = Iuser;
                        aimsCommitment.IsCommitmentTrustFund = false;

                        //ToDo for co-finance projects it may be different
                        aimsCommitment.FundSourceId = project.AimsFundSourceId;

                        aimsCommitment.CommitmentAgreementSignDate = trn.transactiondate.n().isodate;

                        var aimsCurrency = aimsCurrencies.FirstOrDefault(f => f.IATICode == trn.value.currency);
                        aimsCommitment.CommitmentMaidCurrencyId = aimsCurrency == null ? 1 : aimsCurrency.Id;
                        aimsCommitment.CommittedAmount = trn.value.Value;

                        aimsCommitment.CommitmentEffectiveDate = trn.value.n().BBexchangeRateDate;
                        aimsCommitment.ExchangeRateToUSD = trn.value.n().BBexchangeRateUSD;
                        aimsCommitment.CommittedAmountInUSD = trn.value.n().ValueInUSD;

                        aimsCommitment.ExchangeRateToBDT = trn.value.n().BBexchangeRateBDT;
                        aimsCommitment.CommittedAmountInBDT = trn.value.n().ValueInBDT;
                        
                        aimsCommitment.Remarks = project.IsDataSourceAIMS ? trn.description.n().narrative.n(0).Value : "Importerd From IATI: " + trn.description.n().narrative.n(0).Value;
                        aimsCommitment.VerificationRemarks = "Importerd From IATI: ";

                        //AidCategory
                        if (trn.financetype != null && trn.financetype.code.Length > 1)
                            defaultfinancetype = trn.financetype.code.StartsWith("4") ? "400" : "100";

                        var aimsAidCategory = aimsAidCategories.FirstOrDefault(f => f.IATICode == defaultfinancetype);
                        aimsCommitment.AidCategoryId = aimsAidCategory == null ? 1 : aimsAidCategory.Id;
                    } 
                    #endregion
                    
                    #region PlannedDisbursements
                    var planDisb = p.tblProjectFundingPlannedDisbursements.ToList();
                    foreach (var cc in planDisb)
                    {
                        dbContext.tblProjectFundingPlannedDisbursements.Remove(cc);
                    }

                    foreach (var trn in project.PlannedDisbursments)
                    {
                        var aimsPlanDisbursment = new tblProjectFundingPlannedDisbursement();
                        p.tblProjectFundingPlannedDisbursements.Add(aimsPlanDisbursment);
                        aimsPlanDisbursment.IDate = DateTime.Now;
                        aimsPlanDisbursment.IUser = Iuser;
                        aimsPlanDisbursment.IsPlannedDisbursementTrustFund = false;

                        //ToDo for co-finance projects it may be different
                        aimsPlanDisbursment.FundSourceId = project.AimsFundSourceId;

                        aimsPlanDisbursment.PlannedDisbursementPeriodFromDate = trn.periodstart.n().isodate;
                        aimsPlanDisbursment.PlannedDisbursementPeriodToDate = trn.periodend.n().isodate;
                        
                        var aimsCurrency = aimsCurrencies.FirstOrDefault(f => f.IATICode == trn.value.currency);
                        aimsPlanDisbursment.PlannedDisbursementCurrencyId = aimsCurrency == null ? 0 : aimsCurrency.Id;
                        aimsPlanDisbursment.PlannedDisburseAmount = trn.value.Value;

                        aimsPlanDisbursment.PlannedDisburseExchangeRateToUSD = trn.value.n().BBexchangeRateUSD;
                        aimsPlanDisbursment.PlannedDisburseAmountInUSD = trn.value.n().ValueInUSD;

                        aimsPlanDisbursment.PlannedDisburseExchangeRateToBDT = trn.value.n().BBexchangeRateBDT;
                        aimsPlanDisbursment.PlannedDisburseAmountInBDT = trn.value.n().ValueInBDT;

                        //aimsPlanDisbursment.VerificationRemarks = project.IsDataSourceAIMS ? trn.description.n().narrative.n(0).Value : "Importerd From IATI: " + trn.description.n().narrative.n(0).Value;
                        aimsPlanDisbursment.VerificationRemarks = "Importerd From IATI: ";

                        //AidCategory
                        var aimsAidCategory = aimsAidCategories.FirstOrDefault(f => f.IATICode.StartsWith(defaultfinancetype));
                        aimsPlanDisbursment.AidCategoryId = aimsAidCategory == null ? 1 : aimsAidCategory.Id;
                       
                    }
                    #endregion
                    
                    #region Disbursements
                    var disb = p.tblProjectFundingActualDisbursements.ToList();
                    foreach (var cc in disb)
                    {
                        dbContext.tblProjectFundingActualDisbursements.Remove(cc);
                    }

                    foreach (var trn in project.Disbursments)
                    {
                        var aimsDisbursment = new tblProjectFundingActualDisbursement();
                        p.tblProjectFundingActualDisbursements.Add(aimsDisbursment);
                        aimsDisbursment.IDate = DateTime.Now;
                        aimsDisbursment.IUser = Iuser;
                        aimsDisbursment.IsDisbursedTrustFund = false;

                        //ToDo for co-finance projects it may be different
                        aimsDisbursment.FundSourceId = project.AimsFundSourceId;

                        aimsDisbursment.DisbursementDate = trn.transactiondate.n().isodate;
                        aimsDisbursment.DisbursementToDate = trn.transactiondate.n().isodate;

                        var aimsCurrency = aimsCurrencies.FirstOrDefault(f => f.IATICode == trn.value.currency);
                        aimsDisbursment.DisbursedCurrencyId = aimsCurrency == null ? 0 : aimsCurrency.Id;
                        aimsDisbursment.DisbursedAmount = trn.value.Value;

                        aimsDisbursment.DisbursedExchangeRateToUSD = trn.value.n().BBexchangeRateUSD;
                        aimsDisbursment.DisbursedAmountInUSD = trn.value.n().ValueInUSD;

                        aimsDisbursment.DisbursedExchangeRateToBDT = trn.value.n().BBexchangeRateBDT;
                        aimsDisbursment.DisbursedAmountInBDT = trn.value.n().ValueInBDT;

                        aimsDisbursment.Remarks = project.IsDataSourceAIMS ? trn.description.n().narrative.n(0).Value : "Importerd From IATI: " + trn.description.n().narrative.n(0).Value;
                        aimsDisbursment.VerificationRemarks = "Importerd From IATI: ";

                        //AidCategory
                        if(trn.financetype != null && trn.financetype.code.Length > 1)
                            defaultfinancetype = trn.financetype.code.StartsWith("4") ? "400" : "100";

                        var aimsAidCategory = aimsAidCategories.FirstOrDefault(f => f.IATICode == defaultfinancetype);
                        aimsDisbursment.AidCategoryId = aimsAidCategory == null ? 1 : aimsAidCategory.Id;

                    } 
                    #endregion
                }

                dbContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return 1;
        }

        /// <summary>
        /// Convert AIMS to IATI v2.x
        /// ref:http://iatistandard.org/202/activity-standard/iati-activities/iati-activity
        /// </summary>
        /// <param name="dp">Managing/Lead/Implementing Development Partner's IATI Organization Code</param>
        /// <returns></returns>
        public List<iatiactivity> GetAIMSDataInIATIFormat(string dp)
        {

            var projects = (from project in dbContext.tblProjectInfoes
                            join fundSource in dbContext.tblFundSources on project.FundSourceId equals fundSource.Id
                            where fundSource.IATICode == dp
                            && (project.IatiIdentifier != null || project.IatiIdentifier.Length > 0
                                || project.DPProjectNo != null || project.DPProjectNo.Length > 0)
                            select project);

            List<iatiactivity> iatiactivities = new List<iatiactivity>();

            foreach (var project in projects)
            {
                var iatiActivityObj = new iatiactivity();

                iatiActivityObj.IsDataSourceAIMS = true;

                iatiActivityObj.ProjectId = project.Id;
                //iati-activity
                iatiActivityObj.lastupdateddatetime = DateTime.Now;
                iatiActivityObj.lang = "en";
                iatiActivityObj.defaultcurrency = "USD";
                iatiActivityObj.hierarchy = 1;
                //linked-data-uri

                //iati-identifier
                iatiActivityObj.iatiidentifier = new iatiidentifier { Value = getIdentifer(project) };

                //reporting-org
                iatiActivityObj.reportingorg = new reportingorg
                {
                    @ref = project.tblFundSource.n().IATICode,
                    type = project.tblFundSource.n().tblFundSourceCategory.n().IATICode,
                    //secondary-reporter
                    narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName),
                };

                //title
                iatiActivityObj.title = new textRequiredType { narrative = Statix.getNarativeArray(project.Title) };
                //description
                iatiActivityObj.description = new iatiactivityDescription[1] { new iatiactivityDescription { narrative = Statix.getNarativeArray(project.Objective) } };

                //participating-org
                List<participatingorg> participatingorgList = new List<participatingorg>();
                participatingorgList.Add(new participatingorg
                {
                    narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceGroup),
                    role = "1",
                    @ref = project.tblFundSource.n().IATICode,
                    type = "10"
                });

                participatingorgList.Add(new participatingorg
                {
                    narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName),
                    role = "3",
                    @ref = project.tblFundSource.n().IATICode,
                    type = "10"
                });
                //ToDo
                //iatiActivity.participatingorg[2] = new participatingorg
                //{
                //    narrative = getNarativeArray(project.tblFundSource.FundSourceName),
                //    role = "1",
                //    @ref = project.tblFundSource.IATICode,
                //    type = "10"
                //};
                iatiActivityObj.participatingorg = participatingorgList.ToArray();

                //other-identifier

                //activity-status
                iatiActivityObj.activitystatus = new activitystatus { code = project.tblImplementationStatu.IATICode };

                //activity-date
                List<activitydate> activitydateList = new List<activitydate>();
                activitydateList.Add(new activitydate { type = "1", isodate = project.PlannedProjectStartDate ?? default(DateTime) });
                activitydateList.Add(new activitydate { type = "2", isodate = project.ActualProjectStartDate ?? default(DateTime) });
                activitydateList.Add(new activitydate { type = "3", isodate = project.PlannedProjectCompletionDate ?? default(DateTime) });
                activitydateList.Add(new activitydate { type = "4", isodate = project.RevisedProjectCompletionDate ?? default(DateTime) });
                iatiActivityObj.activitydate = activitydateList.ToArray();

                //contact-info
                List<contactinfo> contactinfoList = new List<contactinfo>();
                contactinfoList.Add(new contactinfo //DP
                {
                    type = "1",
                    organisation = new textRequiredType { narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName) },
                    department = new textRequiredType { narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName) },
                    personname = new textRequiredType { narrative = Statix.getNarativeArray(project.FocalPointDPContactName) },
                    jobtitle = new textRequiredType { narrative = Statix.getNarativeArray(project.FocalPointDPContactDesignation) },
                    telephone = new List<contactinfoTelephone> { new contactinfoTelephone { Value = project.FocalPointDPContactTelephone } }.ToArray(),
                    email = new List<contactinfoEmail> { new contactinfoEmail { Value = project.FocalPointDPContactEmail } }.ToArray(),
                    website = new List<contactinfoWebsite> { new contactinfoWebsite { Value = project.FocalPointDPContactAddress } }.ToArray(),
                    mailingaddress = new List<textRequiredType> { new textRequiredType { narrative = Statix.getNarativeArray(project.FocalPointDPContactAddress) } }.ToArray()
                });
                contactinfoList.Add(new contactinfo //GoB
                {
                    type = "2",
                    organisation = new textRequiredType { narrative = Statix.getNarativeArray(Statix.RecipientCountryName) },
                    department = new textRequiredType { narrative = Statix.getNarativeArray("PD") },
                    personname = new textRequiredType { narrative = Statix.getNarativeArray(project.FocalPointGoBContactName) },
                    jobtitle = new textRequiredType { narrative = Statix.getNarativeArray(project.FocalPointGoBContactDesignation) },
                    telephone = new List<contactinfoTelephone> { new contactinfoTelephone { Value = project.FocalPointGoBContactTelephone } }.ToArray(),
                    email = new List<contactinfoEmail> { new contactinfoEmail { Value = project.FocalPointGoBContactEmail } }.ToArray(),
                    website = new List<contactinfoWebsite> { new contactinfoWebsite { Value = project.FocalPointGoBContactAddress } }.ToArray(),
                    mailingaddress = new List<textRequiredType> { new textRequiredType { narrative = Statix.getNarativeArray(project.FocalPointGoBContactAddress) } }.ToArray()
                });
                iatiActivityObj.contactinfo = contactinfoList.ToArray();

                //activity-scope

                //recipient-country
                List<recipientcountry> recipientcountryList = new List<recipientcountry>();
                recipientcountryList.Add(new recipientcountry { code = Statix.RecipientCountry, narrative = Statix.getNarativeArray(Statix.RecipientCountryName), percentage = 100 });
                iatiActivityObj.recipientcountry = recipientcountryList.ToArray();

                //recipient-region

                //location

                //sector

                //country-budget-items

                //humanitarian-scope

                //policy-marker

                //collaboration-type

                //default-flow-type

                //default-finance-type

                //default-aid-type
                iatiActivityObj.defaultaidtype = new defaultaidtype { code = project.tblAssistanceType.n().IATICode };

                //default-tied-status

                //budget

                //planned-disbursement
                List<planneddisbursement> planneddisbursementList = new List<planneddisbursement>();
                var planneddisbursements = project.tblProjectFundingPlannedDisbursements.ToList();
                foreach (var pd in planneddisbursements)
                {
                    planneddisbursementList.Add(new planneddisbursement
                    {
                        type = "1", //1=Origin, 2=Revised 
                        periodstart = new planneddisbursementPeriodstart { isodate = pd.PlannedDisbursementPeriodFromDate ?? DateTime.MinValue },
                        periodend = new planneddisbursementPeriodend { isodate = pd.PlannedDisbursementPeriodToDate ?? DateTime.MinValue },
                        value = new currencyType { currency = Statix.Currency, Value = pd.PlannedDisburseAmountInUSD ?? 0, valuedate = pd.PlannedDisbursementPeriodFromDate ?? DateTime.MinValue, ValueInUSD = pd.PlannedDisburseAmountInUSD ?? 0 },
                        //provider-org
                        //receiver-org
                    });
                }

                iatiActivityObj.planneddisbursement = planneddisbursementList.ToArray();

                //capital-spend

                #region Transaction
                //Transaction
                List<transaction> transactions = new List<transaction>();

                //Commitment
                var commitments = project.tblProjectFundingCommitments.ToList();
                foreach (var commitment in commitments)
                {
                    transaction tr = new transaction();
                    tr.transactiontype = new transactionTransactiontype { code = ConvertIATIv2.gettransactionCode("C") };
                    var date = commitment.CommitmentAgreementSignDate ?? project.AgreementSignDate;
                    tr.transactiondate = new transactionTransactiondate { isodate = date };
                    tr.value = new currencyType { currency = Statix.Currency, valuedate = date, Value = Convert.ToDecimal(commitment.CommittedAmountInUSD), ValueInUSD = Convert.ToDecimal(commitment.CommittedAmountInUSD) }; //commitment.tblCurrency.IATICode

                    tr.description = new textRequiredType { narrative = Statix.getNarativeArray(commitment.Remarks) };
                    tr.providerorg = new transactionProviderorg { @ref = commitment.tblFundSource.n().IATICode, provideractivityid = project.IatiIdentifier, narrative = Statix.getNarativeArray(commitment.tblFundSource.n().FundSourceName) };
                    tr.receiverorg = new transactionReceiverorg { receiveractivityid = project.IatiIdentifier, @ref = project.tblFundSource.n().IATICode, narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName) }; //type="23"

                    //<disbursement-channel code="1" />
                    tr.disbursementchannel = new transactionDisbursementchannel { code = Statix.DisbursementChannel }; //Money is disbursed directly to the implementing institution and managed through a separate bank account

                    //<sector vocabulary="2" code="111" />

                    //<recipient-country code="AF" />  <!--Note: only a recipient-region OR a recipient-country is expected-->
                    tr.recipientcountry = new transactionRecipientcountry { code = Statix.RecipientCountry };

                    //<recipient-region code="456" vocabulary="1" />

                    //<flow-type code="10" />
                    tr.flowtype = new transactionFlowtype { code = Statix.FlowType };

                    //<finance-type code="110" /> //110= Aid grant excluding debt reorganisation, 410 = Aid loan excluding debt reorganisation
                    tr.financetype = new transactionFinancetype { code = commitment.tblAidCategory.n().IATICode };

                    //<aid-type code="A01" /> 
                    tr.aidtype = new transactionAidtype { code = project.tblAssistanceType.n().IATICode };

                    //<tied-status code="3" />
                    tr.tiedstatus = new transactionTiedstatus { code = project.tblAIDEffectivenessIndicators.Where(q => q.AEISurveyYear == date.Year).ToList().n(0).tblAIDEffectivenessResourceTiedType.n().IATICode };

                    transactions.Add(tr);
                }

                //Actual Disbusement
                var actualDisbursements = project.tblProjectFundingActualDisbursements.ToList();
                foreach (var actualDisbursement in actualDisbursements)
                {
                    transaction tr = new transaction();
                    tr.transactiontype = new transactionTransactiontype { code = ConvertIATIv2.gettransactionCode("D") };
                    var date = actualDisbursement.DisbursementToDate ?? actualDisbursement.DisbursementDate;
                    tr.transactiondate = new transactionTransactiondate { isodate = date };
                    tr.value = new currencyType { currency = Statix.Currency, valuedate = date, Value = Convert.ToDecimal(actualDisbursement.DisbursedAmountInUSD), ValueInUSD = Convert.ToDecimal(actualDisbursement.DisbursedAmountInUSD) }; //actualDisbursement.tblCurrency.IATICode

                    tr.description = new textRequiredType { narrative = Statix.getNarativeArray(actualDisbursement.Remarks) };
                    tr.providerorg = new transactionProviderorg { @ref = actualDisbursement.tblFundSource.n().IATICode, provideractivityid = project.IatiIdentifier, narrative = Statix.getNarativeArray(actualDisbursement.tblFundSource.n().FundSourceName) };
                    tr.receiverorg = new transactionReceiverorg { receiveractivityid = project.IatiIdentifier, @ref = project.tblFundSource.n().IATICode, narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName) }; //type="23"

                    //<disbursement-channel code="1" />
                    tr.disbursementchannel = new transactionDisbursementchannel { code = Statix.DisbursementChannel };

                    //<sector vocabulary="2" code="111" />

                    //<recipient-country code="AF" />  <!--Note: only a recipient-region OR a recipient-country is expected-->
                    tr.recipientcountry = new transactionRecipientcountry { code = Statix.RecipientCountry };

                    //<recipient-region code="456" vocabulary="1" />

                    //<flow-type code="10" />
                    tr.flowtype = new transactionFlowtype { code = Statix.FlowType };

                    //<finance-type code="110" /> //110= Aid grant excluding debt reorganisation, 410 = Aid loan excluding debt reorganisation
                    tr.financetype = new transactionFinancetype { code = actualDisbursement.tblAidCategory.n().IATICode };

                    //<aid-type code="A01" /> 
                    tr.aidtype = new transactionAidtype { code = project.tblAssistanceType.n().IATICode };

                    //<tied-status code="3" />
                    tr.tiedstatus = new transactionTiedstatus { code = project.tblAIDEffectivenessIndicators.Where(q => q.AEISurveyYear == date.Year).ToList().n(0).tblAIDEffectivenessResourceTiedType.n().IATICode };

                    transactions.Add(tr);
                }

                //Expenditure
                var expenditures = project.tblProjectFundingExpenditures.ToList();
                foreach (var expenditure in expenditures)
                {
                    transaction tr = new transaction();
                    tr.transactiontype = new transactionTransactiontype { code = ConvertIATIv2.gettransactionCode("E") };
                    var date = expenditure.ExpenditureReportingPeriodToDate; //?? expenditure.ExpenditureReportingPeriodFromDate;
                    tr.transactiondate = new transactionTransactiondate { isodate = date };
                    tr.value = new currencyType { currency = Statix.Currency, valuedate = date, Value = expenditure.ExpenditureAmountInUSD ?? 0, ValueInUSD = expenditure.ExpenditureAmountInUSD ?? 0 }; //expenditure.tblCurrency.IATICode

                    tr.description = new textRequiredType { narrative = Statix.getNarativeArray(expenditure.Remarks) };
                    tr.providerorg = new transactionProviderorg { @ref = expenditure.tblFundSource.n().IATICode, provideractivityid = project.IatiIdentifier, narrative = Statix.getNarativeArray(expenditure.tblFundSource.n().FundSourceName) };
                    tr.receiverorg = new transactionReceiverorg { receiveractivityid = project.IatiIdentifier, @ref = project.tblFundSource.n().IATICode, narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName) }; //type="23"

                    //<disbursement-channel code="1" />
                    tr.disbursementchannel = new transactionDisbursementchannel { code = Statix.DisbursementChannel };

                    //<sector vocabulary="2" code="111" />

                    //<recipient-country code="AF" />  <!--Note: only a recipient-region OR a recipient-country is expected-->
                    tr.recipientcountry = new transactionRecipientcountry { code = Statix.RecipientCountry };

                    //<recipient-region code="456" vocabulary="1" />

                    //<flow-type code="10" />
                    tr.flowtype = new transactionFlowtype { code = Statix.FlowType };

                    //<finance-type code="110" /> //110= Aid grant excluding debt reorganisation, 410 = Aid loan excluding debt reorganisation
                    tr.financetype = new transactionFinancetype { code = expenditure.tblAidCategory.n().IATICode };

                    //<aid-type code="A01" /> 
                    tr.aidtype = new transactionAidtype { code = project.tblAssistanceType.n().IATICode };

                    //<tied-status code="3" />
                    tr.tiedstatus = new transactionTiedstatus { code = project.tblAIDEffectivenessIndicators.Where(q => q.AEISurveyYear == date.Year).ToList().n(0).tblAIDEffectivenessResourceTiedType.n().IATICode };

                    transactions.Add(tr);
                }

                //Assign all transaction
                iatiActivityObj.transaction = transactions.ToArray();
                #endregion

                //document-link
                List<documentlink> documentlinkList = new List<documentlink>();
                var documents = project.tblProjectAttachments.ToList();
                foreach (var document in documents)
                {
                    List<documentlinkLanguage> documentlinkLanguageList = new List<documentlinkLanguage>();
                    documentlinkLanguageList.Add(new documentlinkLanguage { code = Statix.Language });

                    List<documentlinkCategory> documentlinkCategoryList = new List<documentlinkCategory>();
                    documentlinkCategoryList.Add(new documentlinkCategory { code = document.tblDocumentCategory.n().IATICode });

                    documentlinkList.Add(new documentlink
                    {
                        url = document.AttachmentFileURL ?? Statix.DocumentURL + document.Id,
                        //format = 
                        title = new textRequiredType { narrative = Statix.getNarativeArray(document.AttachmentTitle) },
                        language = documentlinkLanguageList.ToArray(),
                        category = documentlinkCategoryList.ToArray()
                    });
                }
                iatiActivityObj.documentlink = documentlinkList.ToArray();

                //related-activity

                //legacy-data

                //conditions

                //result

                //crs-add

                //fss

                iatiactivities.Add(iatiActivityObj);
            }

            return iatiactivities;
        }

        public List<ExchangeRateModel> GetExchangesRateToUSD(string fromCurrencyISOcode, DateTime? date = null)
        {
            var q = (from e in dbContext.tblExchangeRateBBApis.Where(k => k.ISO_CURRENCY_CODE == fromCurrencyISOcode
                && (date == null ? true : k.DATE == date))
                     select new ExchangeRateModel
                     {
                         DATE = e.DATE,
                         ISO_CURRENCY_CODE = e.ISO_CURRENCY_CODE,

                         DOLLAR_PER_CURRENCY = e.DOLLAR_PER_CURRENCY,
                         CURRENCY_PER_DOLLAR = e.CURRENCY_PER_DOLLAR,

                         TAKA_PER_DOLLAR = e.TAKA_PER_CURRENCY / e.DOLLAR_PER_CURRENCY
                     })
                .ToList();


            return q;
        }
    }


}

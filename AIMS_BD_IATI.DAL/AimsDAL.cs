using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_BD_IATI.Library;

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
        public List<FundSourceLookupItem> GetAllFundSources()
        {
            var fundSources = (from fundSource in dbContext.tblFundSources
                               //where fundSource.IATICode != null && !string.IsNullOrEmpty(fundSource.IATICode)
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

        public List<transaction> GetTrustFundDetails(int trustFundId)
        {
            var trustFunds = (from trustFund in dbContext.tblTrustFunds
                              join trustFundDetail in dbContext.tblTrustFundDetails on trustFund.Id equals trustFundDetail.TrustFundId
                              join fundSource in dbContext.tblFundSources on trustFundDetail.TFDFundSourceId equals fundSource.Id

                              where trustFund.Id == trustFundId

                              orderby trustFund.TFIdentifier
                              select new
                              {
                                  FundSourceName = fundSource.FundSourceName,
                                  Amount = trustFundDetail.TFDAmountInUSD,
                              }).ToList();

            var transactions = new List<transaction>();
            foreach (var trustFund in trustFunds)
            {
                transactions.Add(new transaction
                {
                    transactiontype = new transactionTransactiontype { code = ConvertIATIv2.gettransactionCode("C") },
                    providerorg = new transactionProviderorg { narrative = Statix.getNarativeArray(trustFund.FundSourceName) },
                    value = new currencyType { currency = Statix.Currency, Value = trustFund.Amount ?? 0 },
                });
            }
            return transactions;
        }

        public int? UpdateProjects(List<iatiactivity> projects)
        {
            foreach (var project in projects)
            {
                var p = dbContext.tblProjectInfoes.FirstOrDefault(f => f.Id == project.ProjectId);
                if (p != null)
                {
                    p.Title = project.Title;
                    p.Objective = project.Description;
                }
            }

            return dbContext.SaveChanges();
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
                        value = new currencyType { currency = Statix.Currency, Value = pd.PlannedDisburseAmountInUSD ?? 0, valuedate = pd.PlannedDisbursementPeriodFromDate ?? DateTime.MinValue },
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
                    tr.value = new currencyType { currency = Statix.Currency, valuedate = date, Value = Convert.ToDecimal(commitment.CommittedAmountInUSD) }; //commitment.tblCurrency.IATICode

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
                    tr.value = new currencyType { currency = Statix.Currency, valuedate = date, Value = Convert.ToDecimal(actualDisbursement.DisbursedAmountInUSD) }; //actualDisbursement.tblCurrency.IATICode

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
                    tr.value = new currencyType { currency = Statix.Currency, valuedate = date, Value = Convert.ToDecimal(expenditure.ExpenditureAmountInUSD) }; //expenditure.tblCurrency.IATICode

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

        public object GetExchangeRate(DateTime date, string fromCurrency)
        {
            return null;
        }
    }


}

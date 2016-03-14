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
                              select new DPLookupItem { 
                                ID = fundSource.IATICode,
                                Name = fundSource.FundSourceName + " (" + fundSource.IATICode ?? "" + ")",
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
                                  Name = fundSource.FundSourceName + " (" + fundSource.IATICode ?? "" + ")",
                                  IATICode = fundSource.IATICode
                              }).ToList();

            return fundSources;
        }

        /// <summary>
        /// Convert AIMS to IATI
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
                var iatiActivity = new iatiactivity();

                iatiActivity.iatiidentifier = new iatiidentifier { Value = getIdentifer(project) };

                iatiActivity.title = new textRequiredType { narrative = Statix.getNarativeArray(project.Title) };

                iatiActivity.activitydate = new activitydate[4];
                iatiActivity.activitydate[0] = new activitydate { type = "1", isodate = project.PlannedProjectStartDate ?? default(DateTime) };
                iatiActivity.activitydate[1] = new activitydate { type = "2", isodate = project.ActualProjectStartDate ?? default(DateTime) };
                iatiActivity.activitydate[2] = new activitydate { type = "3", isodate = project.PlannedProjectCompletionDate ?? default(DateTime) };
                iatiActivity.activitydate[3] = new activitydate { type = "4", isodate = project.RevisedProjectCompletionDate ?? default(DateTime) };

                iatiActivity.description = new iatiactivityDescription[1] { new iatiactivityDescription { narrative =Statix.getNarativeArray(project.Objective) } };

                iatiActivity.defaultaidtype = new defaultaidtype { code = project.tblAssistanceType.n().IATICode };

                iatiActivity.reportingorg = new reportingorg
                {
                    narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName),
                    @ref = project.tblFundSource.n().IATICode,
                    type = project.tblFundSource.n().tblFundSourceCategory.n().IATICode
                };


                iatiActivity.participatingorg = new participatingorg[3];

                iatiActivity.participatingorg[0] = new participatingorg
                {
                    narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceGroup),
                    role = "1",
                    @ref = project.tblFundSource.n().IATICode,
                    type = "10"
                };
                iatiActivity.participatingorg[1] = new participatingorg
                {
                    narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName),
                    role = "3",
                    @ref = project.tblFundSource.n().IATICode,
                    type = "10"
                };
                //ToDo
                //iatiActivity.participatingorg[2] = new participatingorg
                //{
                //    narrative = getNarativeArray(project.tblFundSource.FundSourceName),
                //    role = "1",
                //    @ref = project.tblFundSource.IATICode,
                //    type = "10"
                //};

                iatiActivity.recipientcountry = new recipientcountry[1];
                iatiActivity.recipientcountry[0] = new recipientcountry { code = "BD", narrative = Statix.getNarativeArray("Bangladesh") };

                //Transaction
                List<transaction> transactions = new List<transaction>();

                //Commitment
                var commitments = project.tblProjectFundingCommitments.ToList();
                foreach (var commitment in commitments)
                {
                    transaction tr = new transaction();
                    tr.transactiontype = new transactionTransactiontype{ code = ConvertIATIv2.gettransactionCode("C")};
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

                //Assign all transaction
                iatiActivity.transaction = transactions.ToArray();

                iatiactivities.Add(iatiActivity);
            }

            return iatiactivities;
        }

    }


}

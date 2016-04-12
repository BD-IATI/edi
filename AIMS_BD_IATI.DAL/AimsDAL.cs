using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_BD_IATI.Library;
using System.Data.Entity.Validation;
using System.Diagnostics;
using MoreLinq;

namespace AIMS_BD_IATI.DAL
{
    public class AimsDAL
    {
        #region Declarations
        AIMS_DBEntities dbContext = new AIMS_DBEntities();
        AimsDbIatiDAL aimsDBIatiDAL = new AimsDbIatiDAL();

        #endregion

        #region Get Lookup Items
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


        public List<tblFundSource> GetFundSources()
        {
            var fundSources = from fundSource in dbContext.tblFundSources
                              where fundSource.IATICode != null && !string.IsNullOrEmpty(fundSource.IATICode)
                              orderby fundSource.FundSourceName
                              select fundSource;

            return fundSources.ToList();
        }
        public string GetFundSourceIDnIATICode(string IatiCode)
        {
            var fundSourceId = (from fundSource in dbContext.tblFundSources
                                where fundSource.IATICode == IatiCode
                                select fundSource.Id).FirstOrDefault();

            return fundSourceId + "~" + IatiCode;
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

        #endregion Get Fundsources

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
                                        FundSourceId = trustFundDetail.TFDFundSourceId,
                                        Amount = trustFundDetail.TFDAmountInUSD,
                                    }).ToList();

            trustFundModel.Id = trustFundDetails.n(0).Id;
            trustFundModel.TFIdentifier = trustFundDetails.n(0).TFIdentifier;
            foreach (var trustFundDetail in trustFundDetails)
            {
                var tr = new transaction
                {
                    transactiontype = new transactionTransactiontype { code = ConvertIATIv2.gettransactionCode("C") },
                    providerorg = new transactionProviderorg { narrative = Statix.getNarativeArray(trustFundDetail.FundSourceName) },
                    value = new currencyType { currency = Statix.Currency, Value = trustFundDetail.Amount ?? 0 },
                };
                new AimsDbIatiDAL().SetCurrencyExRateAndVal(tr, Statix.Currency);
                trustFundModel.transactionsInAims.Add(tr);
            }


            return trustFundModel;
        }

        public int? UpdateTrustFunds(List<TrustFundModel> TrustFundModels, string Iuser)
        {
            var aimsCurrencies = from c in dbContext.tblCurrencies
                                 select new CurrencyLookupItem { Id = c.Id, IATICode = c.IATICode };

            var aimsAidCategories = from c in dbContext.tblAidCategories
                                    select new AidCategoryLookupItem { Id = c.Id, IATICode = c.IATICode };
            try
            {
                foreach (var TrustFundModel in TrustFundModels)
                {



                    try
                    {

                        var trustFund = dbContext.tblTrustFunds.FirstOrDefault(w => w.Id == TrustFundModel.Id);
                        if (trustFund != null)
                        {
                            foreach (var activity in TrustFundModel.iatiactivities)
                            {
                                trustFund.tblTrustFundDetails.Add(new tblTrustFundDetail
                                      {
                                          TFDAmount = activity.TotalCommitment,
                                          TFDAmountInUSD = activity.TotalCommitment,
                                          TFDExchangeRateToUSD = 1,
                                          TFDFundSourceId = activity.AimsFundSourceId,
                                          TFDCurrencyId = 1,
                                          IUser = Iuser,
                                          IDate = DateTime.Now
                                      });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToDbAndFile(ex, LogType.Error, TrustFundModel.TFIdentifier);

                    }


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

        #region Update Projects
        public int? UpdateProjects(List<iatiactivity> projects, string Iuser)
        {
            var aimsCurrencies = from c in dbContext.tblCurrencies
                                 select new CurrencyLookupItem { Id = c.Id, IATICode = c.IATICode };

            var aimsAidCategories = from c in dbContext.tblAidCategories
                                    select new AidCategoryLookupItem { Id = c.Id, IATICode = c.IATICode };
            try
            {
                foreach (var mergedproject in projects)
                {
                    try
                    {
                        bool isFinancialDataMismathed = false;
                        var defaultfinancetype = "100";
                        if (mergedproject.defaultfinancetype != null && !string.IsNullOrWhiteSpace(mergedproject.defaultfinancetype.code))
                            defaultfinancetype = mergedproject.defaultfinancetype.code.StartsWith("4") ? "400" : "100";

                        var p = dbContext.tblProjectInfoes.FirstOrDefault(f => f.Id == mergedproject.ProjectId);
                        if (p == null)
                        {
                            p = new tblProjectInfo();
                            p.IDate = DateTime.Now;
                            p.IUser = Iuser;
                            dbContext.tblProjectInfoes.Add(p);
                        }
                        else
                        {
                            // first check isFinancialDataMismathed
                            foreach (var MatchedProject in mergedproject.MatchedProjects)
                            {
                                isFinancialDataMismathed = CheckTransactionMismatch(p, MatchedProject);
                                if (isFinancialDataMismathed) break;
                            }
                            //if Financial Data are Mismathed then continue with next project
                            if (isFinancialDataMismathed) continue;

                            //if FinancialData are not Mismathed then delete existing transactions (this DP only)
                            foreach (var MatchedProject in mergedproject.MatchedProjects)
                            {
                                DeleteTransactions(p, MatchedProject);
                            }
                            //here we need another loop to update transactions !!! Do not combine these three identical loops.
                            foreach (var MatchedProject in mergedproject.MatchedProjects)
                            {
                                UpdateTransactions(Iuser, aimsCurrencies, aimsAidCategories, defaultfinancetype, p, MatchedProject);
                            }
                        }

                        isFinancialDataMismathed = CheckTransactionMismatch(p, mergedproject);
                        if (isFinancialDataMismathed) continue;

                        DeleteTransactions(p, mergedproject);

                        UpdateTransactions(Iuser, aimsCurrencies, aimsAidCategories, defaultfinancetype, p, mergedproject);

                        //we need to place this region at bottom due to checking isFinancialDataMismathed
                        #region Other Fields
                        p.Title = mergedproject.Title;
                        p.Objective = mergedproject.Description;

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToDbAndFile(ex, LogType.Error, mergedproject.IATICode, mergedproject.IatiIdentifier);

                    }


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

        private void UpdateTransactions(string Iuser, IQueryable<CurrencyLookupItem> aimsCurrencies, IQueryable<AidCategoryLookupItem> aimsAidCategories, string defaultfinancetype, tblProjectInfo p, iatiactivity MatchedProject)
        {
            #region Commitments
            if (MatchedProject.IsCommitmentIncluded)
            {
                foreach (var trn in MatchedProject.CommitmentsThisDPOnly)
                {

                    var aimsCommitment = new tblProjectFundingCommitment();
                    p.tblProjectFundingCommitments.Add(aimsCommitment);

                    aimsCommitment.IDate = DateTime.Now;
                    aimsCommitment.IUser = Iuser;
                    aimsCommitment.IsCommitmentTrustFund = false;

                    //ToDo for co-finance projects it may be different
                    aimsCommitment.FundSourceId = MatchedProject.AimsFundSourceId;

                    aimsCommitment.CommitmentAgreementSignDate = trn.transactiondate.n().isodate;

                    var aimsCurrency = aimsCurrencies.FirstOrDefault(f => f.IATICode == trn.value.currency);
                    aimsCommitment.CommitmentMaidCurrencyId = aimsCurrency == null ? 1 : aimsCurrency.Id;
                    aimsCommitment.CommittedAmount = trn.value.Value;

                    aimsCommitment.CommitmentEffectiveDate = trn.value.n().BBexchangeRateDate;
                    aimsCommitment.ExchangeRateToUSD = trn.value.n().BBexchangeRateUSD;
                    aimsCommitment.CommittedAmountInUSD = trn.value.n().ValueInUSD;

                    aimsCommitment.ExchangeRateToBDT = trn.value.n().BBexchangeRateBDT;
                    aimsCommitment.CommittedAmountInBDT = trn.value.n().ValueInBDT;

                    aimsCommitment.Remarks = MatchedProject.IsDataSourceAIMS ? trn.description.n().narrative.n(0).Value : "Importerd From IATI: " + trn.description.n().narrative.n(0).Value;
                    aimsCommitment.VerificationRemarks = "Importerd From IATI: ";

                    //AidCategory
                    if (trn.financetype != null && trn.financetype.code.Length > 1)
                        defaultfinancetype = trn.financetype.code.StartsWith("4") ? "400" : "100";

                    var aimsAidCategory = aimsAidCategories.FirstOrDefault(f => f.IATICode == defaultfinancetype);
                    aimsCommitment.AidCategoryId = aimsAidCategory == null ? 1 : aimsAidCategory.Id;
                }
            }
            #endregion

            #region PlannedDisbursements
            if (MatchedProject.IsPlannedDisbursmentIncluded)
            {



                foreach (var trn in MatchedProject.PlannedDisbursments)
                {
                    var aimsPlanDisbursment = new tblProjectFundingPlannedDisbursement();
                    p.tblProjectFundingPlannedDisbursements.Add(aimsPlanDisbursment);

                    aimsPlanDisbursment.IDate = DateTime.Now;
                    aimsPlanDisbursment.IUser = Iuser;
                    aimsPlanDisbursment.IsPlannedDisbursementTrustFund = false;

                    //ToDo for co-finance projects it may be different
                    aimsPlanDisbursment.FundSourceId = MatchedProject.AimsFundSourceId;

                    aimsPlanDisbursment.PlannedDisbursementPeriodFromDate = trn.periodstart.n().isodate;
                    aimsPlanDisbursment.PlannedDisbursementPeriodToDate = trn.periodend.n().isodate;

                    var aimsCurrency = aimsCurrencies.FirstOrDefault(f => f.IATICode == trn.value.currency);
                    aimsPlanDisbursment.PlannedDisbursementCurrencyId = aimsCurrency == null ? 1 : aimsCurrency.Id;
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
            }
            #endregion

            #region Disbursements
            if (MatchedProject.IsDisbursmentIncluded)
            {


                foreach (var trn in MatchedProject.DisbursmentsThisDPOnly)
                {
                    var aimsDisbursment = new tblProjectFundingActualDisbursement();
                    p.tblProjectFundingActualDisbursements.Add(aimsDisbursment);

                    aimsDisbursment.IDate = DateTime.Now;
                    aimsDisbursment.IUser = Iuser;
                    aimsDisbursment.IsDisbursedTrustFund = false;

                    //ToDo for co-finance projects it may be different
                    aimsDisbursment.FundSourceId = MatchedProject.AimsFundSourceId;

                    aimsDisbursment.DisbursementDate = trn.transactiondate.n().isodate;
                    aimsDisbursment.DisbursementToDate = trn.transactiondate.n().isodate;

                    var aimsCurrency = aimsCurrencies.FirstOrDefault(f => f.IATICode == trn.value.currency);
                    aimsDisbursment.DisbursedCurrencyId = aimsCurrency == null ? 1 : aimsCurrency.Id;
                    aimsDisbursment.DisbursedAmount = trn.value.Value;

                    aimsDisbursment.DisbursedExchangeRateToUSD = trn.value.n().BBexchangeRateUSD;
                    aimsDisbursment.DisbursedAmountInUSD = trn.value.n().ValueInUSD;

                    aimsDisbursment.DisbursedExchangeRateToBDT = trn.value.n().BBexchangeRateBDT;
                    aimsDisbursment.DisbursedAmountInBDT = trn.value.n().ValueInBDT;

                    aimsDisbursment.Remarks = MatchedProject.IsDataSourceAIMS ? trn.description.n().narrative.n(0).Value : "Importerd From IATI: " + trn.description.n().narrative.n(0).Value;
                    aimsDisbursment.VerificationRemarks = "Importerd From IATI: ";

                    //AidCategory
                    if (trn.financetype != null && trn.financetype.code.Length > 1)
                        defaultfinancetype = trn.financetype.code.StartsWith("4") ? "400" : "100";

                    var aimsAidCategory = aimsAidCategories.FirstOrDefault(f => f.IATICode == defaultfinancetype);
                    aimsDisbursment.AidCategoryId = aimsAidCategory == null ? 1 : aimsAidCategory.Id;

                }
            }
            #endregion
        }

        private void DeleteTransactions(tblProjectInfo p, iatiactivity MatchedProject)
        {
            //Commitments
            if (MatchedProject.IsPlannedDisbursmentIncluded)
            {
                var aimsCommitments = p.tblProjectFundingCommitments.Where(w => w.FundSourceId == MatchedProject.AimsFundSourceId).ToList();

                foreach (var cc in aimsCommitments)
                {
                    dbContext.tblProjectFundingCommitments.Remove(cc);
                }
            }
            //PlannedDisbursements
            if (MatchedProject.IsPlannedDisbursmentIncluded)
            {
                var planDisb = p.tblProjectFundingPlannedDisbursements.Where(w => w.FundSourceId == MatchedProject.AimsFundSourceId).ToList();
                foreach (var cc in planDisb)
                {
                    dbContext.tblProjectFundingPlannedDisbursements.Remove(cc);
                }
            }
            //Disbursements
            if (MatchedProject.IsDisbursmentIncluded)
            {
                var aimsDisbursements = p.tblProjectFundingActualDisbursements.Where(w => w.FundSourceId == MatchedProject.AimsFundSourceId).ToList();

                foreach (var cc in aimsDisbursements)
                {
                    dbContext.tblProjectFundingActualDisbursements.Remove(cc);
                }
            }
        }

        private bool CheckTransactionMismatch(tblProjectInfo p, iatiactivity MatchedProject)
        {
            bool isFinancialDataMismathed = false;
            #region Commitments
            if (MatchedProject.IsCommitmentIncluded)
            {
                var aimsCommitments = p.tblProjectFundingCommitments.Where(w => w.FundSourceId == MatchedProject.AimsFundSourceId).ToList();

                var iatiCommitments = MatchedProject.CommitmentsThisDPOnly;

                #region Checking transaction mismatch
                if (aimsCommitments.Count > iatiCommitments.Count)
                    foreach (var aimsCommitment in aimsCommitments)
                    {
                        var trandate = aimsCommitment.CommitmentAgreementSignDate ?? p.AgreementSignDate;

                        var notExistInIATI = !iatiCommitments.Exists(e => e.transactiondate.n().isodate.Date == trandate && Math.Floor(e.ValUSD) == Math.Floor(aimsCommitment.CommittedAmountInUSD ?? 0));

                        isFinancialDataMismathed = true;

                        aimsDBIatiDAL.InsertLog(new Log
                        {
                            OrgId = MatchedProject.IATICode,
                            LogType = (int)LogType.FinancialDataMismathed,
                            DateTime = DateTime.Now,
                            IatiIdentifier = MatchedProject.IatiIdentifier,
                            ProjectId = p.Id,
                            Message = "Transactions (C) are mismatched between IATI and AIMS"
                        });

                    }
                #endregion Checking transaction mismatch

                return isFinancialDataMismathed;
            }
            #endregion

            #region Disbursements
            if (MatchedProject.IsDisbursmentIncluded)
            {

                var aimsDisbursements = p.tblProjectFundingActualDisbursements.Where(w => w.FundSourceId == MatchedProject.AimsFundSourceId).ToList();
                var iatiDisbursements = MatchedProject.DisbursmentsThisDPOnly;

                #region  Checking transaction mismatch
                if (aimsDisbursements.Count > iatiDisbursements.Count)
                    foreach (var aimsDisbursement in aimsDisbursements)
                    {
                        var trandate = aimsDisbursement.DisbursementToDate ?? aimsDisbursement.DisbursementDate;

                        var notExistInIATI = !iatiDisbursements.Exists(e => e.transactiondate.n().isodate.Date == trandate && Math.Floor(e.ValUSD) == Math.Floor(aimsDisbursement.DisbursedAmountInUSD ?? 0));

                        isFinancialDataMismathed = true;

                        aimsDBIatiDAL.InsertLog(new Log
                        {
                            DateTime = DateTime.Now,
                            IatiIdentifier = MatchedProject.IatiIdentifier,
                            LogType = (int)LogType.FinancialDataMismathed,
                            ProjectId = p.Id,
                            OrgId = MatchedProject.IATICode,
                            Message = "Transactions (D) are mismatched between IATI and AIMS"
                        });

                    }

                #endregion  Checking transaction mismatch

                return isFinancialDataMismathed;

            }

            return false;
            #endregion
        }

        #endregion Update Projects

        #region Get Aims data in Iati format
        /// <summary>
        /// Convert AIMS to IATI v2.x
        /// ref:http://iatistandard.org/202/activity-standard/iati-activities/iati-activity
        /// </summary>
        /// <param name="dp">Managing/Lead/Implementing Development Partner's IATI Organization Code</param>
        /// <returns></returns>
        public List<iatiactivity> GetAIMSProjectsInIATIFormat(string dp)
        {

            var projects = (from project in dbContext.tblProjectInfoes
                            join fundSource in dbContext.tblFundSources on project.FundSourceId equals fundSource.Id
                            let isIATIactivity = project.IatiIdentifier != null || project.IatiIdentifier.Length > 0 || project.DPProjectNo != null || project.DPProjectNo.Length > 0
                            where fundSource.IATICode == dp
                            && isIATIactivity
                            select project);

            List<iatiactivity> iatiactivities = new List<iatiactivity>();

            foreach (var project in projects)
            {
                var iatiActivityObj = ConvertAimsToIati(project);

                iatiactivities.Add(iatiActivityObj);
            }

            return iatiactivities;
        }

        public List<iatiactivity> GetNotMappedAIMSProjectsInIATIFormat(string dp, List<int?> mappedProjectIds)
        {

            var projects = (from project in dbContext.tblProjectInfoes
                            join fundSource in dbContext.tblFundSources on project.FundSourceId equals fundSource.Id
                            let isIATIactivity = project.IatiIdentifier != null || project.IatiIdentifier.Length > 0 || project.DPProjectNo != null || project.DPProjectNo.Length > 0
                            let isNotMapped = !mappedProjectIds.Contains(project.Id)
                            where fundSource.IATICode == dp
                            && isIATIactivity && isNotMapped
                            select project);

            List<iatiactivity> iatiactivities = new List<iatiactivity>();

            foreach (var project in projects)
            {
                var iatiActivityObj = ConvertAimsToIati(project);

                iatiactivities.Add(iatiActivityObj);
            }

            return iatiactivities;
        }

        public iatiactivity GetAIMSProjectInIATIFormat(int? projectId)
        {
            var prj = dbContext.tblProjectInfoes.FirstOrDefault(f => f.Id == projectId);
            iatiactivity activity = null;
            if (prj != null)
            {
                activity = ConvertAimsToIati(prj);
            }

            return activity;

        }

        private iatiactivity ConvertAimsToIati(tblProjectInfo project)
        {
            var iatiActivityObj = new iatiactivity();

            iatiActivityObj.IsDataSourceAIMS = true;
            iatiActivityObj.IsCofinancedProject = project.IsCofundedProject ?? false;

            iatiActivityObj.FundSourceIDnIATICode = project.FundSourceId + "~" + project.tblFundSource.n().IATICode;

            iatiActivityObj.ProjectId = project.Id;
            //iati-activity
            iatiActivityObj.lastupdateddatetime = DateTime.Now;
            iatiActivityObj.lang = "en";
            iatiActivityObj.defaultcurrency = Statix.Currency;
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
                type = project.tblFundSource.n().tblFundSourceCategory.n().IATICode,
            });

            participatingorgList.Add(new participatingorg
            {
                narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName),
                role = "3",
                @ref = project.tblFundSource.n().IATICode,
                type = project.tblFundSource.n().tblFundSourceCategory.n().IATICode,
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

                    providerorg = new planneddisbursementProviderorg
                    {
                        @ref = pd.tblFundSource.n().IATICode,
                        provideractivityid = project.IatiIdentifier,
                        narrative = Statix.getNarativeArray(pd.tblFundSource.n().FundSourceName),
                        type = pd.tblFundSource.n().tblFundSourceCategory.n().IATICode
                    },

                    receiverorg = new planneddisbursementReceiverorg
                    {
                        receiveractivityid = project.IatiIdentifier,
                        @ref = project.tblFundSource.n().IATICode,
                        narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName),
                        type = project.tblFundSource.n().tblFundSourceCategory.n().IATICode,
                    }

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
                tr.providerorg = new transactionProviderorg
                {
                    @ref = commitment.tblFundSource.n().IATICode,
                    provideractivityid = project.IatiIdentifier,
                    narrative = Statix.getNarativeArray(commitment.tblFundSource.n().FundSourceName),
                    type = commitment.tblFundSource.n().tblFundSourceCategory.n().IATICode
                };
                tr.receiverorg = new transactionReceiverorg
                {
                    @ref = project.tblFundSource.n().IATICode,
                    receiveractivityid = project.IatiIdentifier,
                    narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName),
                    type = project.tblFundSource.n().tblFundSourceCategory.n().IATICode
                };

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


                iatiActivityObj.IsTrustFundedProject = commitment.IsCommitmentTrustFund ?? false;

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
                tr.providerorg = new transactionProviderorg
                {
                    @ref = actualDisbursement.tblFundSource.n().IATICode,
                    provideractivityid = project.IatiIdentifier,
                    narrative = Statix.getNarativeArray(actualDisbursement.tblFundSource.n().FundSourceName),
                    type = actualDisbursement.tblFundSource.n().tblFundSourceCategory.n().IATICode

                };
                tr.receiverorg = new transactionReceiverorg
                {
                    @ref = project.tblFundSource.n().IATICode,
                    receiveractivityid = project.IatiIdentifier,
                    narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName),
                    type = project.tblFundSource.n().tblFundSourceCategory.n().IATICode

                };

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
                tr.providerorg = new transactionProviderorg
                {
                    @ref = expenditure.tblFundSource.n().IATICode,
                    provideractivityid = project.IatiIdentifier,
                    narrative = Statix.getNarativeArray(expenditure.tblFundSource.n().FundSourceName),
                    type = expenditure.tblFundSource.n().tblFundSourceCategory.n().IATICode

                };
                tr.receiverorg = new transactionReceiverorg
                {
                    @ref = project.tblFundSource.n().IATICode,
                    receiveractivityid = project.IatiIdentifier,
                    narrative = Statix.getNarativeArray(project.tblFundSource.n().FundSourceName),
                    type = project.tblFundSource.n().tblFundSourceCategory.n().IATICode
                };

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
            return iatiActivityObj;
        }
        private string getIdentifer(tblProjectInfo project)
        {
            return string.IsNullOrWhiteSpace(project.IatiIdentifier) ?
                project.DPProjectNo //project.DPProjectNo.n().StartsWith(project.tblFundSource.n().IATICode) ? project.DPProjectNo : project.tblFundSource.n().IATICode + "-" + project.DPProjectNo
                : project.IatiIdentifier;
        }

        #endregion Get Aims data in Iati format

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


    public class iatiactivityExtended : iatiactivity
    {

    }

}

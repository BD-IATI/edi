//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectFundingExpenditure
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int FundSourceId { get; set; }
        public int AidCategoryId { get; set; }
        public Nullable<bool> IsExpenditureTrustFund { get; set; }
        public Nullable<int> ExpenditureTrustFundId { get; set; }
        public System.DateTime ExpenditureReportingDate { get; set; }
        public System.DateTime ExpenditureReportingPeriodFromDate { get; set; }
        public System.DateTime ExpenditureReportingPeriodToDate { get; set; }
        public int ExpenditureCurrencyId { get; set; }
        public decimal ExpenditureAmount { get; set; }
        public decimal ExpenditureExchangeRateToUSD { get; set; }
        public Nullable<decimal> ExpenditureAmountInUSD { get; set; }
        public decimal ExpenditureExchangeRateToBDT { get; set; }
        public Nullable<decimal> ExpenditureAmountInBDT { get; set; }
        public Nullable<int> VerificationId { get; set; }
        public string VerifiedBy { get; set; }
        public string VerificationRemarks { get; set; }
        public string Remarks { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string LoanNumber { get; set; }
    
        public virtual tblAidCategory tblAidCategory { get; set; }
        public virtual tblCurrency tblCurrency { get; set; }
        public virtual tblFundSource tblFundSource { get; set; }
        public virtual tblProjectInfo tblProjectInfo { get; set; }
        public virtual tblTrustFund tblTrustFund { get; set; }
        public virtual tblVerificationStatu tblVerificationStatu { get; set; }
    }
}

//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectFundingPlannedDisbursement
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int FundSourceId { get; set; }
        public int AidCategoryId { get; set; }
        public Nullable<bool> IsPlannedDisbursementTrustFund { get; set; }
        public Nullable<int> PlannedDisbursementTrustFundId { get; set; }
        public Nullable<System.DateTime> PlannedDisbursementPeriodFromDate { get; set; }
        public Nullable<System.DateTime> PlannedDisbursementPeriodToDate { get; set; }
        public int PlannedDisbursementCurrencyId { get; set; }
        public decimal PlannedDisburseAmount { get; set; }
        public decimal PlannedDisburseExchangeRateToUSD { get; set; }
        public Nullable<decimal> PlannedDisburseAmountInUSD { get; set; }
        public decimal PlannedDisburseExchangeRateToBDT { get; set; }
        public Nullable<decimal> PlannedDisburseAmountInBDT { get; set; }
        public Nullable<int> VerificationId { get; set; }
        public string VerifiedBy { get; set; }
        public string VerificationRemarks { get; set; }
        public bool IsLocked { get; set; }
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

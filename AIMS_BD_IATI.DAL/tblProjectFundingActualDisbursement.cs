//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectFundingActualDisbursement
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int FundSourceId { get; set; }
        public System.DateTime DisbursementDate { get; set; }
        public Nullable<System.DateTime> DisbursementToDate { get; set; }
        public int AidCategoryId { get; set; }
        public Nullable<bool> IsDisbursedTrustFund { get; set; }
        public Nullable<int> DisbursedTrustFundId { get; set; }
        public int DisbursedCurrencyId { get; set; }
        public decimal DisbursedAmount { get; set; }
        public decimal DisbursedExchangeRateToUSD { get; set; }
        public Nullable<decimal> DisbursedAmountInUSD { get; set; }
        public decimal DisbursedExchangeRateToBDT { get; set; }
        public Nullable<decimal> DisbursedAmountInBDT { get; set; }
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

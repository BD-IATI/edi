//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectFundingIncoming
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int FundSourceId { get; set; }
        public int AidCategoryId { get; set; }
        public int TransactionTypeId { get; set; }
        public Nullable<bool> IsIncomingTrustFund { get; set; }
        public Nullable<int> IncomingTrustFundId { get; set; }
        public System.DateTime IncomingDate { get; set; }
        public Nullable<System.DateTime> IncomingToDate { get; set; }
        public int IncomingCurrencyId { get; set; }
        public decimal IncomingAmount { get; set; }
        public decimal IncomingExchangeRateToUSD { get; set; }
        public Nullable<decimal> IncomingAmountInUSD { get; set; }
        public decimal IncomingExchangeRateToBDT { get; set; }
        public Nullable<decimal> IncomingAmountInBDT { get; set; }
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
        public virtual tblTransactionType tblTransactionType { get; set; }
        public virtual tblTrustFund tblTrustFund { get; set; }
        public virtual tblVerificationStatu tblVerificationStatu { get; set; }
    }
}

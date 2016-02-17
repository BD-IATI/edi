//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectFundingCommitment
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int FundSourceId { get; set; }
        public int AidCategoryId { get; set; }
        public Nullable<bool> IsCommitmentTrustFund { get; set; }
        public Nullable<int> CommitmentTrustFundId { get; set; }
        public string LoanNumber { get; set; }
        public Nullable<System.DateTime> CommitmentAgreementSignDate { get; set; }
        public Nullable<System.DateTime> CommitmentEffectiveDate { get; set; }
        public Nullable<System.DateTime> CommitmentCompletionDate { get; set; }
        public string TypeOfInterest { get; set; }
        public Nullable<decimal> InterestRate { get; set; }
        public string InterestRateComment { get; set; }
        public Nullable<decimal> ManagementFee { get; set; }
        public Nullable<decimal> CommitmentFee { get; set; }
        public Nullable<decimal> GracePeriod { get; set; }
        public Nullable<decimal> MaturityPeriod { get; set; }
        public int CommitmentMaidCurrencyId { get; set; }
        public decimal CommittedAmount { get; set; }
        public decimal ExchangeRateToUSD { get; set; }
        public Nullable<decimal> CommittedAmountInUSD { get; set; }
        public decimal ExchangeRateToBDT { get; set; }
        public Nullable<decimal> CommittedAmountInBDT { get; set; }
        public Nullable<int> NoOfRepaymentPerYear { get; set; }
        public Nullable<int> RepaymentType { get; set; }
        public Nullable<int> VerificationId { get; set; }
        public string VerifiedBy { get; set; }
        public string VerificationRemarks { get; set; }
        public string Remarks { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblAidCategory tblAidCategory { get; set; }
        public virtual tblCurrency tblCurrency { get; set; }
        public virtual tblFundSource tblFundSource { get; set; }
        public virtual tblLoanRepaymentType tblLoanRepaymentType { get; set; }
        public virtual tblProjectInfo tblProjectInfo { get; set; }
        public virtual tblTrustFund tblTrustFund { get; set; }
        public virtual tblVerificationStatu tblVerificationStatu { get; set; }
    }
}

//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblFundSource
    {
        public tblFundSource()
        {
            this.tblAIDPlannings = new HashSet<tblAIDPlanning>();
            this.tblProjectFundingActualDisbursements = new HashSet<tblProjectFundingActualDisbursement>();
            this.tblProjectFundingCommitments = new HashSet<tblProjectFundingCommitment>();
            this.tblProjectFundingExpenditures = new HashSet<tblProjectFundingExpenditure>();
            this.tblProjectFundingPlannedDisbursements = new HashSet<tblProjectFundingPlannedDisbursement>();
            this.tblProjectInfoes = new HashSet<tblProjectInfo>();
            this.tblTrustFunds = new HashSet<tblTrustFund>();
            this.tblTrustFundDetails = new HashSet<tblTrustFundDetail>();
            this.tblUserFundSources = new HashSet<tblUserFundSource>();
        }
    
        public int Id { get; set; }
        public string FundSourceName { get; set; }
        public int FundSourceCategoryId { get; set; }
        public string FundSourceGroup { get; set; }
        public int CurrencyId { get; set; }
        public string Address { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string WebURL { get; set; }
        public string IATICode { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEMail { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string Acronym { get; set; }
    
        public virtual ICollection<tblAIDPlanning> tblAIDPlannings { get; set; }
        public virtual tblCurrency tblCurrency { get; set; }
        public virtual tblFundSourceCategory tblFundSourceCategory { get; set; }
        public virtual ICollection<tblProjectFundingActualDisbursement> tblProjectFundingActualDisbursements { get; set; }
        public virtual ICollection<tblProjectFundingCommitment> tblProjectFundingCommitments { get; set; }
        public virtual ICollection<tblProjectFundingExpenditure> tblProjectFundingExpenditures { get; set; }
        public virtual ICollection<tblProjectFundingPlannedDisbursement> tblProjectFundingPlannedDisbursements { get; set; }
        public virtual ICollection<tblProjectInfo> tblProjectInfoes { get; set; }
        public virtual ICollection<tblTrustFund> tblTrustFunds { get; set; }
        public virtual ICollection<tblTrustFundDetail> tblTrustFundDetails { get; set; }
        public virtual ICollection<tblUserFundSource> tblUserFundSources { get; set; }
    }
}

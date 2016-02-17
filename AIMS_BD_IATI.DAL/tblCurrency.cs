//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblCurrency
    {
        public tblCurrency()
        {
            this.tblCurrencyMappings = new HashSet<tblCurrencyMapping>();
            this.tblExchangeRateDetails = new HashSet<tblExchangeRateDetail>();
            this.tblFundSources = new HashSet<tblFundSource>();
            this.tblProjectFundingActualDisbursements = new HashSet<tblProjectFundingActualDisbursement>();
            this.tblProjectFundingCommitments = new HashSet<tblProjectFundingCommitment>();
            this.tblProjectFundingExpenditures = new HashSet<tblProjectFundingExpenditure>();
            this.tblProjectFundingPlannedDisbursements = new HashSet<tblProjectFundingPlannedDisbursement>();
            this.tblProjectInfoes = new HashSet<tblProjectInfo>();
            this.tblProjectInfoes1 = new HashSet<tblProjectInfo>();
            this.tblTrustFundDetails = new HashSet<tblTrustFundDetail>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public string IATICode { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual ICollection<tblCurrencyMapping> tblCurrencyMappings { get; set; }
        public virtual ICollection<tblExchangeRateDetail> tblExchangeRateDetails { get; set; }
        public virtual ICollection<tblFundSource> tblFundSources { get; set; }
        public virtual ICollection<tblProjectFundingActualDisbursement> tblProjectFundingActualDisbursements { get; set; }
        public virtual ICollection<tblProjectFundingCommitment> tblProjectFundingCommitments { get; set; }
        public virtual ICollection<tblProjectFundingExpenditure> tblProjectFundingExpenditures { get; set; }
        public virtual ICollection<tblProjectFundingPlannedDisbursement> tblProjectFundingPlannedDisbursements { get; set; }
        public virtual ICollection<tblProjectInfo> tblProjectInfoes { get; set; }
        public virtual ICollection<tblProjectInfo> tblProjectInfoes1 { get; set; }
        public virtual ICollection<tblTrustFundDetail> tblTrustFundDetails { get; set; }
    }
}

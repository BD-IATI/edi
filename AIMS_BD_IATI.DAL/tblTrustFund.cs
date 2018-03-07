//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblTrustFund
    {
        public tblTrustFund()
        {
            this.tblProjectFundingActualDisbursements = new HashSet<tblProjectFundingActualDisbursement>();
            this.tblProjectFundingCommitments = new HashSet<tblProjectFundingCommitment>();
            this.tblProjectFundingExpenditures = new HashSet<tblProjectFundingExpenditure>();
            this.tblProjectFundingPlannedDisbursements = new HashSet<tblProjectFundingPlannedDisbursement>();
            this.tblTrustFundDetails = new HashSet<tblTrustFundDetail>();
            this.tblProjectFundingIncomings = new HashSet<tblProjectFundingIncoming>();
        }
    
        public int Id { get; set; }
        public int TFFundSourceId { get; set; }
        public string TFIdentifier { get; set; }
        public Nullable<decimal> TFAmountInUSD { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblFundSource tblFundSource { get; set; }
        public virtual ICollection<tblProjectFundingActualDisbursement> tblProjectFundingActualDisbursements { get; set; }
        public virtual ICollection<tblProjectFundingCommitment> tblProjectFundingCommitments { get; set; }
        public virtual ICollection<tblProjectFundingExpenditure> tblProjectFundingExpenditures { get; set; }
        public virtual ICollection<tblProjectFundingPlannedDisbursement> tblProjectFundingPlannedDisbursements { get; set; }
        public virtual ICollection<tblTrustFundDetail> tblTrustFundDetails { get; set; }
        public virtual ICollection<tblProjectFundingIncoming> tblProjectFundingIncomings { get; set; }
    }
}

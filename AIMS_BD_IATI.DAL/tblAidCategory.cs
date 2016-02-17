//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAidCategory
    {
        public tblAidCategory()
        {
            this.tblProjectFundingActualDisbursements = new HashSet<tblProjectFundingActualDisbursement>();
            this.tblProjectFundingCommitments = new HashSet<tblProjectFundingCommitment>();
            this.tblProjectFundingExpenditures = new HashSet<tblProjectFundingExpenditure>();
            this.tblProjectFundingPlannedDisbursements = new HashSet<tblProjectFundingPlannedDisbursement>();
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
    
        public virtual ICollection<tblProjectFundingActualDisbursement> tblProjectFundingActualDisbursements { get; set; }
        public virtual ICollection<tblProjectFundingCommitment> tblProjectFundingCommitments { get; set; }
        public virtual ICollection<tblProjectFundingExpenditure> tblProjectFundingExpenditures { get; set; }
        public virtual ICollection<tblProjectFundingPlannedDisbursement> tblProjectFundingPlannedDisbursements { get; set; }
    }
}

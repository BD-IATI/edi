//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAIDPlanning
    {
        public tblAIDPlanning()
        {
            this.tblAIDPlanningDetails = new HashSet<tblAIDPlanningDetail>();
        }
    
        public int Id { get; set; }
        public int FundSourceId { get; set; }
        public int Year { get; set; }
        public decimal AIDPlanAmount { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblFundSource tblFundSource { get; set; }
        public virtual ICollection<tblAIDPlanningDetail> tblAIDPlanningDetails { get; set; }
    }
}

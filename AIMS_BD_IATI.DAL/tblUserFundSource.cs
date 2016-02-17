//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblUserFundSource
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FundSourceId { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string Status { get; set; }
    
        public virtual tblFundSource tblFundSource { get; set; }
        public virtual tblUserRegistrationInfo tblUserRegistrationInfo { get; set; }
    }
}

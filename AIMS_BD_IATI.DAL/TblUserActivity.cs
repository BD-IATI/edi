//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblUserActivity
    {
        public long Id { get; set; }
        public int ApplicationId { get; set; }
        public string UserId { get; set; }
        public System.DateTime LoginDate { get; set; }
        public Nullable<System.DateTime> LogoutDate { get; set; }
        public string Workstation { get; set; }
        public string Browser { get; set; }
        public string Remarks { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string SessionID { get; set; }
    }
}

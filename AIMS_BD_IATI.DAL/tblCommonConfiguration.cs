//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblCommonConfiguration
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> SystemLaunchDate { get; set; }
        public Nullable<System.DateTime> DataAvailStartDate { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    }
}

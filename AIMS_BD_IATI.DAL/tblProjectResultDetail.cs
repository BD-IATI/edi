//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectResultDetail
    {
        public int Id { get; set; }
        public int ProjectResultId { get; set; }
        public Nullable<System.DateTime> PeriodStart { get; set; }
        public Nullable<System.DateTime> PeriodEnd { get; set; }
        public string TargetValue { get; set; }
        public string ActualValue { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblProjectResult tblProjectResult { get; set; }
    }
}

//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectGoBExecutingAgency
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Nullable<int> GOBExeAgencyMinistryId { get; set; }
        public Nullable<int> GOBExeAgencyMinistryAgencyId { get; set; }
        public Nullable<bool> IsLeadAgency { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblMinistry tblMinistry { get; set; }
        public virtual tblMinistryAgency tblMinistryAgency { get; set; }
        public virtual tblProjectInfo tblProjectInfo { get; set; }
    }
}

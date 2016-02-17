//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectExecutingAgency
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ExecutingAgencyTypeId { get; set; }
        public Nullable<int> ExecutingAgencyOrganizationTypeId { get; set; }
        public Nullable<int> ExecutingAgencyOrganizationId { get; set; }
        public Nullable<bool> IsLeadAgency { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblExecutingAgencyType tblExecutingAgencyType { get; set; }
        public virtual tblProjectInfo tblProjectInfo { get; set; }
    }
}

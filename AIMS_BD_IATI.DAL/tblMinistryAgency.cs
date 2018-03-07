//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblMinistryAgency
    {
        public tblMinistryAgency()
        {
            this.tblProjectAnalyticalWorksMissionInfoes = new HashSet<tblProjectAnalyticalWorksMissionInfo>();
            this.tblProjectGoBExecutingAgencies = new HashSet<tblProjectGoBExecutingAgency>();
        }
    
        public int Id { get; set; }
        public int MinistryId { get; set; }
        public string AgencyName { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string NameLocal { get; set; }
        public string Remarks { get; set; }
    
        public virtual tblMinistry tblMinistry { get; set; }
        public virtual ICollection<tblProjectAnalyticalWorksMissionInfo> tblProjectAnalyticalWorksMissionInfoes { get; set; }
        public virtual ICollection<tblProjectGoBExecutingAgency> tblProjectGoBExecutingAgencies { get; set; }
    }
}

//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblMinistry
    {
        public tblMinistry()
        {
            this.tblMinistryAgencies = new HashSet<tblMinistryAgency>();
            this.tblProjectGoBExecutingAgencies = new HashSet<tblProjectGoBExecutingAgency>();
        }
    
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public string IATICode { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public bool IsActive { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string MinistryName { get; set; }
        public string StandardCode { get; set; }
        public string IMEDCode { get; set; }
    
        public virtual ICollection<tblMinistryAgency> tblMinistryAgencies { get; set; }
        public virtual ICollection<tblProjectGoBExecutingAgency> tblProjectGoBExecutingAgencies { get; set; }
    }
}

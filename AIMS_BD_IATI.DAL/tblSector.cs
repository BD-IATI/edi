//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblSector
    {
        public tblSector()
        {
            this.tblAIDPlanningDetails = new HashSet<tblAIDPlanningDetail>();
            this.tblProjectSectoralAllocations = new HashSet<tblProjectSectoralAllocation>();
            this.tblSectorSubSectorMaps = new HashSet<tblSectorSubSectorMap>();
            this.tblSubSectors = new HashSet<tblSubSector>();
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
    
        public virtual ICollection<tblAIDPlanningDetail> tblAIDPlanningDetails { get; set; }
        public virtual ICollection<tblProjectSectoralAllocation> tblProjectSectoralAllocations { get; set; }
        public virtual ICollection<tblSectorSubSectorMap> tblSectorSubSectorMaps { get; set; }
        public virtual ICollection<tblSubSector> tblSubSectors { get; set; }
    }
}

//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblSubSector
    {
        public tblSubSector()
        {
            this.tblProjectSectoralAllocations = new HashSet<tblProjectSectoralAllocation>();
            this.tblSectorSubSectorMaps = new HashSet<tblSectorSubSectorMap>();
        }
    
        public int Id { get; set; }
        public int SectorId { get; set; }
        public string SubSectorName { get; set; }
        public string Acronym { get; set; }
        public string IATICode { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual ICollection<tblProjectSectoralAllocation> tblProjectSectoralAllocations { get; set; }
        public virtual tblSector tblSector { get; set; }
        public virtual ICollection<tblSectorSubSectorMap> tblSectorSubSectorMaps { get; set; }
    }
}

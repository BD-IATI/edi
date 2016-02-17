//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectSectoralAllocation
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> SubSectorId { get; set; }
        public Nullable<decimal> TotalCommitmentPercent { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblProjectInfo tblProjectInfo { get; set; }
        public virtual tblSector tblSector { get; set; }
        public virtual tblSubSector tblSubSector { get; set; }
    }
}

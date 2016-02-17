//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAIDPlanningDetail
    {
        public int Id { get; set; }
        public int AIDPlanningId { get; set; }
        public int SectorId { get; set; }
        public decimal AIDPlanAmount { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblAIDPlanning tblAIDPlanning { get; set; }
        public virtual tblSector tblSector { get; set; }
    }
}

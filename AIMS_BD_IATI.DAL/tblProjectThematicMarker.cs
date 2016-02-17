//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectThematicMarker
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int SelectedThematicMarkerId { get; set; }
        public Nullable<decimal> TotalCommitmentPercent { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblProjectInfo tblProjectInfo { get; set; }
        public virtual tblThematicMarker tblThematicMarker { get; set; }
    }
}

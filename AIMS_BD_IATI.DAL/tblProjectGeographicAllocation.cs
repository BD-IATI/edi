//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectGeographicAllocation
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Nullable<int> DivisionId { get; set; }
        public Nullable<int> DistrictId { get; set; }
        public Nullable<int> UpazilaId { get; set; }
        public Nullable<decimal> TotalCommitmentPercentForDistrict { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblDistrict tblDistrict { get; set; }
        public virtual tblDivision tblDivision { get; set; }
        public virtual tblProjectInfo tblProjectInfo { get; set; }
        public virtual tblUpazila tblUpazila { get; set; }
    }
}

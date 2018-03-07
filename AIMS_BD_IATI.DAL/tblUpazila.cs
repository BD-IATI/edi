//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblUpazila
    {
        public tblUpazila()
        {
            this.tblProjectGeographicAllocations = new HashSet<tblProjectGeographicAllocation>();
        }
    
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public string AreaType { get; set; }
        public string UpazilaName { get; set; }
        public string StandardCode { get; set; }
        public Nullable<decimal> GPSLatitude { get; set; }
        public Nullable<decimal> GPSLongitude { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string NameLocal { get; set; }
        public string Remarks { get; set; }
    
        public virtual tblDistrict tblDistrict { get; set; }
        public virtual tblDivision tblDivision { get; set; }
        public virtual ICollection<tblProjectGeographicAllocation> tblProjectGeographicAllocations { get; set; }
    }
}

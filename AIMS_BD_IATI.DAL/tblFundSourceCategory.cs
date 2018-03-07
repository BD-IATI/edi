//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblFundSourceCategory
    {
        public tblFundSourceCategory()
        {
            this.tblFundSources = new HashSet<tblFundSource>();
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
        public string NameLocal { get; set; }
        public string Remarks { get; set; }
    
        public virtual ICollection<tblFundSource> tblFundSources { get; set; }
    }
}

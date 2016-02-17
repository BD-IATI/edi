//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblFinancialYear
    {
        public int Id { get; set; }
        public string FinancialYearName { get; set; }
        public System.DateTime FinancialYearStartDate { get; set; }
        public System.DateTime FinancialYearEndDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    }
}

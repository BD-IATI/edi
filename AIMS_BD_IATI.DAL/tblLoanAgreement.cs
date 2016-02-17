//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblLoanAgreement
    {
        public int Id { get; set; }
        public string LoanAgreementTitle { get; set; }
        public string LoanAgreementCode { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    }
}

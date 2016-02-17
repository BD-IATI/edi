//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblCurrencyMapping
    {
        public int Id { get; set; }
        public int AIMSCurrencyId { get; set; }
        public int BBCurrencyId { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblCurrency tblCurrency { get; set; }
        public virtual tblCurrencyBB tblCurrencyBB { get; set; }
    }
}

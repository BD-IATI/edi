//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblExchangeRateBBApi
    {
        public long Id { get; set; }
        public System.DateTime DATE { get; set; }
        public string ISO_CURRENCY_CODE { get; set; }
        public string CURRENCY_NAME { get; set; }
        public Nullable<decimal> CURRENCY_PER_TAKA { get; set; }
        public Nullable<decimal> TAKA_PER_CURRENCY { get; set; }
        public Nullable<decimal> DOLLAR_PER_CURRENCY { get; set; }
        public Nullable<decimal> CURRENCY_PER_DOLLAR { get; set; }
    }
}

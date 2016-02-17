//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblExchangeRateDetail
    {
        public int Id { get; set; }
        public int ExchangeRateId { get; set; }
        public int CurrencyId { get; set; }
        public Nullable<decimal> ExchangeRateToUSD { get; set; }
        public Nullable<decimal> ExchangeRateToBDT { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblCurrency tblCurrency { get; set; }
        public virtual tblExchangeRate tblExchangeRate { get; set; }
    }
}

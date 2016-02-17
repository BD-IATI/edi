//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblTrustFundDetail
    {
        public int Id { get; set; }
        public int TrustFundId { get; set; }
        public int TFDFundSourceId { get; set; }
        public decimal TFDAmount { get; set; }
        public int TFDCurrencyId { get; set; }
        public Nullable<decimal> TFDExchangeRateToUSD { get; set; }
        public Nullable<decimal> TFDAmountInUSD { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblCurrency tblCurrency { get; set; }
        public virtual tblFundSource tblFundSource { get; set; }
        public virtual tblTrustFund tblTrustFund { get; set; }
    }
}

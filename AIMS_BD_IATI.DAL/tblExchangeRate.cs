//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblExchangeRate
    {
        public tblExchangeRate()
        {
            this.tblExchangeRateDetails = new HashSet<tblExchangeRateDetail>();
        }
    
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal ExchangeRateUSDToBDT { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual ICollection<tblExchangeRateDetail> tblExchangeRateDetails { get; set; }
    }
}

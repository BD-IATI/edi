using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.Library
{
    [Serializable]
    public partial class ExchangeRateModel
    {
        public System.DateTime DATE { get; set; }
        public string ISO_CURRENCY_CODE { get; set; }
        public string CURRENCY_NAME { get; set; }
        public Nullable<decimal> CURRENCY_PER_TAKA { get; set; }
        public Nullable<decimal> TAKA_PER_CURRENCY { get; set; }
        public Nullable<decimal> DOLLAR_PER_CURRENCY { get; set; }
        public Nullable<decimal> CURRENCY_PER_DOLLAR { get; set; }

        public Nullable<decimal> TAKA_PER_DOLLAR { get; set; }
    }

    [Serializable]
    public class ExchangeRateFederal
    {
        public int Id { get; set; }

        /// <summary>
        /// Date formal 1995-01-02
        /// </summary>
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
        /// <summary>
        /// ISO Codes of Currency
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// D = Daily
        /// </summary>
        public string Frequency { get; set; }
    }
}

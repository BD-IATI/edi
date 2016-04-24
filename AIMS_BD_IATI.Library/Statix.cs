using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.Library
{
    public static class Statix
    {
        public static string RecipientCountry { get { return "BD"; } }
        public static string RecipientCountryName { get { return "Bangladesh"; } }
        public static string Currency { get { return "USD"; } }
        public static string Language { get { return "en"; } }
        public static string DocumentURL { get { return "http://aims.erd.gov.bd/AIMS/ProjectInfo/DownloadAttachment?Id="; } }

        /// <summary>
        /// "1", //Money is disbursed through central Ministry of Finance or Treasury
        /// "2", //Money is disbursed directly to the implementing institution and managed through a separate bank account
        /// "3", //Aid in kind: Donors utilise third party agencies, e.g. NGOs or management companies
        /// "4" //Aid in kind: Donors manage funds themselves
        /// </summary>
        public static string DisbursementChannel { get { return "2"; } }

        /// <summary>
        /// ODA = "10", //Official Development Assistance
        /// OOF = "20", //Other Official Flows
        /// Private_grants = "30", //made by NGOs and other civil society organisations (e.e. philantropic foundations) based //in/ the reporting DAC country
        /// Private_Market = "35", //Private market
        /// Non_flow = "40", //e.g. GNI
        /// Other_flows = "50" //e.g. non-ODA component of peacebuilding operations)
        /// </summary>
        public static string FlowType { get { return "10"; } }

        public static narrative[] getNarativeArray(string val)
        {
            return new narrative[1] { new narrative { lang = "en", Value = val } };
        }

        // Clone/Copy an object without changing source
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static DateTime? ToSqlDateTimeNull(this DateTime datetime)
        {
            if (datetime == default(DateTime)) return null;
            else return datetime;
        }
        public static DateTime ToSqlDateTime(this DateTime datetime)
        {
            if (datetime == default(DateTime)) return new DateTime(1700, 01, 01);
            else return datetime;
        }
        /// <summary>
        /// Calculates the percent of a decimal (e.g 10.PercentOf(200) = 20)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="percentOf"></param>
        /// <returns></returns>
        public static decimal PercentOf(this decimal val, decimal percentOf)
        {
            return (val / 100) * percentOf;
        }
        /// <summary>
        /// Calculates the percant from a given value (e.g 10.ToPercent(200) = 5)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal ToPercent(this decimal val, decimal total)
        {
            return total > 0 ? (val / total) * 100 : 0;
        }

        public static T[] Add<T>(this T[] originalArray, T addItem) where T : class
        {
            if (addItem == null)
            {
                throw new ArgumentNullException("addItem");
            }
            var arr = new[] { addItem };
            if (originalArray == null)
            {
                return arr;
            }
            return originalArray.Concat(arr).ToArray(); // although Concat is not recommended for performance reasons, see the accepted answer
        }
    }
    public enum LogType
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Alert = 3,
        FinancialDataMismathed=4,
        AddedNewActivity=5,
        AimsProjectNotFound=6,
        ValidationError=7
    }

}

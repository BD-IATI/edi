using System;
using System.Collections.Generic;
using System.Linq;
using AIMS_BD_IATI.DAL;

namespace AIMS_BD_IATI.Service
{
    public class ExchangeRateParser {
        public List<ExchangeRateFederal> SplitCSV(string url) {
            string fileList = GetCSV(url);

            var list = from line in fileList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Skip(1)
                       let columns = line.Split(',')
                       select new ExchangeRateFederal {
                           Date = Convert.ToDateTime(columns[0]),
                           Rate = Common.ParseDecimal(columns[1]),
                           Currency = (columns[2]),
                           Frequency = (columns[3])
                       };

            return list.ToList();
        }

        public string GetCSV(string url) {
            var req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            var resp = (System.Net.HttpWebResponse)req.GetResponse();

            var sr = new System.IO.StreamReader(resp.GetResponseStream());

            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }
    }
}
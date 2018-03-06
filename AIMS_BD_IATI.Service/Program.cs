using System;

using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.DAL;
using System.Threading;
using AIMS_BD_IATI.Library.Parser;

namespace AIMS_BD_IATI.Service
{
    class Program {
        static void Main(string[] args) {
            try {
                Logger.Write("");
                Logger.Write(" ******************** START IATI PROCESS *********************** ");

                ParseIATI();

                Logger.Write(" ********************** END IATI PROCESS ********************* ");
                Logger.Write("");

                //Logger.Write("");
                //Logger.Write("");
                //Logger.Write(" ******************** START EXCHANGE RATE PROCESS *********************** ");

                //ParseExchangeRate();

                //Logger.Write(" ********************** END EXCHANGE RATE PROCESS ********************* ");
                //Logger.Write("");
            } catch (Exception ex) {
                Logger.WriteToDbAndFile(ex, LogType.Error);
            }
        }

        /// <summary>
        /// Parse IATI XML data from IATI data store and converts from v1 to v2
        /// </summary>
        /// <returns></returns>
        private static void ParseIATI() {

            //Get list of FundSource from AIMS DB
            AimsDAL _AimsDAL = new AimsDAL();
            var fundSources = _AimsDAL.GetFundSources();//.FindAll(q => q.IATICode == "SE-0");

            int i = 1;
            foreach (var fundSource in fundSources) {

                Thread th = new Thread(new ThreadStart(() => {
                    IatiXmlParser p = new IatiXmlParser();
                    //Pilot purpose only
                    //usaid demo data:  
                    if (fundSource.IATICode == "US-USAGOV|US-1|USAIDManualData") {
                        p.Parse(fundSources.Count, i++, fundSource, "http://test.brough.io/usaid/iati/iati-activities.xml");
                    } else if (fundSource.IATICode.Contains("CA-3")) {
                        p.Parse(fundSources.Count, i++, fundSource);
                    } else {
                        p.Parse(fundSources.Count, i++, fundSource);
                    }
                    p = null;
                }));
                th.Start();

                th.Join();
            }

        }
    }
}
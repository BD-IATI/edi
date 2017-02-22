using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser;
using AIMS_BD_IATI.Library.Parser.ParserIATIv1;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_BD_IATI.DAL;
using System.Data.Entity.Validation;
using System.Threading;
using System.Xml.Serialization;

namespace AIMS_BD_IATI.Service {
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
                    Parser p = new Parser();
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
        /// <summary>
        /// Save Converted Data to File
        /// </summary>
        /// <param name="returnResult2"></param>
        private static void SaveToDisk(XmlResultv2 returnResult2) {
            var serializer = ParserIATIv2.serializer;
            TextWriter writer = new StreamWriter("D:\\xxv2.01.xml");
            serializer.Serialize(writer, returnResult2);
            Logger.Write("INFO: " + "Saved Converted Data to File");
        }

        private static void ParseExchangeRate() {
            var obj = new ExchangeRateParser();
            var url = Common.exchangeRate_url;
            var list = obj.SplitCSV(url);
            Logger.Write("INFO: " + list.Count() + " Exchange Rates are dowloaded.");

            var count = new AimsDbIatiDAL().SaveExchangeRateFedaral(list);

            Logger.Write("INFO: " + count + " Exchange Rates are stored in Database");
            Logger.Write("");
        }
    }

    public class Parser {
        static XmlSerializer iatiactivitySerealizer = new XmlSerializer(typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivity), new XmlRootAttribute("iati-activity"));


        public void Parse(int fundSourcesCount, int i, tblFundSource fundSource, string URL = null) {



            IParserIATI parserIATI;
            //IConverterIATI converterIATI;
            string activitiesURL;
            XmlResultv2 returnResult2;
            XmlResultv1 returnResult1;
            try {
                Logger.Write("");
                Logger.Write(i + "/" + fundSourcesCount + " " + fundSource.FundSourceName + " (" + fundSource.IATICode + ")");
                Logger.Write("-------------------->");
                Logger.Write("INFO: Downloading...");

                #region Convert Data from v1.05 to v2.01

                //Parser v2.01
                parserIATI = new ParserIATIv2();
                //activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=CA-3&stream=True" //"http://localhost:1000/UploadedFiles/activity_GB-1_2.xml";
                //single activity : "http://datastore.iatistandard.org/api/1/access/activity.xml?iati-identifier=CA-3-A035529001
                //Params: activity.xml or activity.json, recipient-country=BD, reporting-org=CA-3
                if (URL == null)
                    activitiesURL = Common.iati_url + "recipient-country=" + Common.iati_recipient_country + "&reporting-org=" + fundSource.IATICode + "&stream=" + Common.iati_stream;
                else
                    activitiesURL = URL;
                returnResult2 = (XmlResultv2)parserIATI.ParseIATIXML(activitiesURL);

                Logger.Write("INFO: " + "Parsing...");

                var iatiactivityArray = returnResult2?.iatiactivities?.iatiactivity;
                if (iatiactivityArray != null && iatiactivityArray.n()[0].AnyAttr.n()[0].Value.StartsWith("1.0")) //1.04, 1.05
                {
                    //Parser v1.05
                    parserIATI = new ParserIATIv1();
                    //activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=GB-1&stream=True"; //"http://localhost:1000/UploadedFiles/activity_GB-1_2.xml";
                    //Params: activity.xml or activity.json, recipient-country=BD, reporting-org=GB-1 or XM-DAC-12-1
                    returnResult1 = (XmlResultv1)parserIATI.ParseIATIXML(activitiesURL);

                    Logger.Write("INFO: " + "Parsing completed!");

                    //Conversion
                    ConvertIATIv2 convertIATIv2 = new ConvertIATIv2();
                    returnResult2 = convertIATIv2.ConvertIATI105to201XML(returnResult1, returnResult2);
                    Logger.Write("INFO: " + "Convertion completed!");
                }

                #endregion

                iatiactivityArray = returnResult2?.iatiactivities?.iatiactivity;
                if (iatiactivityArray != null) {
                    SaveToDB(fundSource, iatiactivityArray);
                }
            } catch (DbEntityValidationException ex) {
                string messages = "";
                foreach (var validationErrors in ex.EntityValidationErrors) {
                    foreach (var validationError in validationErrors.ValidationErrors) {
                        messages += string.Format("\nProperty: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                Logger.WriteToDbAndFile(ex, LogType.ValidationError, fundSource.IATICode, null, messages);

            } catch (Exception ex) {
                Logger.WriteToDbAndFile(ex, LogType.Error, fundSource.IATICode);
            }
        }


        /// <summary>
        /// Save Data To DB
        /// </summary>
        /// <param name="returnResult2"></param>
        private void SaveToDB(tblFundSource fundSource, AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivity[] iatiactivityArray) {

            int counter = 1;
            int successfullySavedActivityCounter = 0;
            int totalActivity = iatiactivityArray.Count();

            Logger.Write("INFO: " + "Total Activity found: " + totalActivity);
            Console.WriteLine();
            if (totalActivity > 0) {
                foreach (var iatiactivityItem in iatiactivityArray) {
                    try {
                        var Activity = new Activity();

                        Activity.OrgId = Activity.AssignedOrgId = fundSource.IATICode;// iatiactivityItem.reportingorg?.@ref;
                        Activity.IatiIdentifier = iatiactivityItem.IatiIdentifier;
                        Activity.Hierarchy = iatiactivityItem.hierarchy;

                        using (StringWriter ww = new StringWriter()) {
                            iatiactivitySerealizer.Serialize(ww, iatiactivityItem);
                            Activity.IatiActivity = ww.ToString();
                        }

                        successfullySavedActivityCounter += new AimsDbIatiDAL().SaveAtivity(Activity, iatiactivityItem, fundSource) == 1 ? 1 : 0;

                        Console.Write("\r Activity Counter: {0}   ", counter++);

                    } catch (DbEntityValidationException ex) {
                        string messages = "";
                        foreach (var validationErrors in ex.EntityValidationErrors) {
                            foreach (var validationError in validationErrors.ValidationErrors) {
                                messages += string.Format("\nProperty: {0} Error: {1}",
                                                        validationError.PropertyName,
                                                        validationError.ErrorMessage);
                            }
                        }
                        Logger.WriteToDbAndFile(ex, LogType.ValidationError, fundSource.IATICode, iatiactivityItem.IatiIdentifier, messages);

                    } catch (Exception ex) {
                        Logger.WriteToDbAndFile(ex, LogType.Error, fundSource.IATICode, iatiactivityItem.IatiIdentifier);
                    }

                }

                Logger.Write("INFO: " + successfullySavedActivityCounter + " activities are stored in Database");
            }
        }

    }

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
using System;
using System.IO;
using System.Linq;

using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv1;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_BD_IATI.DAL;
using System.Data.Entity.Validation;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace AIMS_BD_IATI.Library.Parser
{
    public class IatiXmlParser
    {
        static XmlSerializer iatiactivitySerealizer = new XmlSerializer(typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivity), new XmlRootAttribute("iati-activity"));
        public string Message { get; set; } = "";
        /// <summary>
        /// Download the XML from IATI datastore, parse xml then save to DB
        /// </summary>
        /// <param name="fundSourcesCount"></param>
        /// <param name="counter">used for notifying user. i out of total number of projects</param>
        /// <param name="fundSource"></param>
        /// <param name="URL"></param>
        public string Parse(tblFundSource fundSource, int fundSourcesCount = 1, int counter = 1, string URL = null)
        {

            IParserIATI parserIATI;
            string activitiesURL = "";
            XmlResultv2 returnResult2;
            XmlResultv1 returnResult1;
            try
            {
                Logger.Write("");
                Logger.Write(counter + "/" + fundSourcesCount + " " + fundSource.FundSourceName + " (" + fundSource.IATICode + ")");
                Logger.Write("-------------------->");
                Logger.Write("INFO: Downloading...");

                #region Convert Data from v1.05 to v2.01

                //Parser v2.01
                parserIATI = new ParserIATIv2.ParserIATIv2();
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
                    parserIATI = new ParserIATIv1.ParserIATIv1();
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
                if (iatiactivityArray != null)
                {
                    SaveToDB(fundSource, iatiactivityArray);
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Message += string.Format("\nProperty: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                Logger.WriteToDbAndFile(ex, LogType.ValidationError, fundSource.IATICode, null, Message);

            }
            catch (Exception ex)
            {
                Logger.WriteToDbAndFile(ex, LogType.Error, fundSource.IATICode);
                Message = "Download failed. " + activitiesURL;
            }

            return Message == "" ? "IATI data successfully downloaded from " + activitiesURL : Message;
        }


        /// <summary>
        /// Save Data To DB
        /// </summary>
        /// <param name="returnResult2"></param>
        private void SaveToDB(tblFundSource fundSource, List<ParserIATIv2.iatiactivity> iatiactivityArray)
        {

            int counter = 1;
            int successfullySavedActivityCounter = 0;
            int totalActivity = iatiactivityArray.Count();

            Logger.Write("INFO: " + "Total Activity found: " + totalActivity);
            Console.WriteLine();
            if (totalActivity > 0)
            {
                foreach (var iatiactivityItem in iatiactivityArray)
                {
                    try
                    {
                        var Activity = new Activity();

                        Activity.OrgId = Activity.AssignedOrgId = fundSource.IATICode;// iatiactivityItem.reportingorg?.@ref;
                        Activity.IatiIdentifier = iatiactivityItem.IatiIdentifier;
                        Activity.Hierarchy = iatiactivityItem.hierarchy;

                        using (StringWriter ww = new StringWriter())
                        {
                            iatiactivitySerealizer.Serialize(ww, iatiactivityItem);
                            Activity.IatiActivity = ww.ToString();
                        }

                        successfullySavedActivityCounter += new AimsDbIatiDAL().SaveAtivity(Activity, iatiactivityItem, fundSource) == 1 ? 1 : 0;

                        Console.Write("\r Activity Counter: {0}   ", counter++);

                    }
                    catch (DbEntityValidationException ex)
                    {
                        string messages = "";
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                messages += string.Format("\nProperty: {0} Error: {1}",
                                                        validationError.PropertyName,
                                                        validationError.ErrorMessage);
                            }
                        }
                        Logger.WriteToDbAndFile(ex, LogType.ValidationError, fundSource.IATICode, iatiactivityItem.IatiIdentifier, messages);

                    }
                    catch (Exception ex)
                    {
                        Logger.WriteToDbAndFile(ex, LogType.Error, fundSource.IATICode, iatiactivityItem.IatiIdentifier);
                    }

                }

                Logger.Write("INFO: " + successfullySavedActivityCounter + " activities are stored in Database");
            }
        }

    }
}
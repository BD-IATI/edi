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
        public List<Log> Logs { get; set; } = new List<Log>();
        /// <summary>
        /// Download the XML from IATI datastore, parse xml then save to DB
        /// </summary>
        /// <param name="fundSourcesCount"></param>
        /// <param name="counter">used for notifying user. i out of total number of projects</param>
        /// <param name="fundSource"></param>
        /// <param name="URL"></param>
        public List<Log> Parse(tblFundSource fundSource, int fundSourcesCount = 1, int counter = 1, string URL = null)
        {

            IParserIATI parserIATI;
            string activitiesURL = "";
            XmlResultv2 returnResult2 = null;
            XmlResultv1 returnResult1 = null;
            try
            {
                Logger.Write("");
                Logger.Write(counter + "/" + fundSourcesCount + " " + fundSource.FundSourceName + " (" + fundSource.IATICode + ")");
                Logger.Write("-------------------->");

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

                Logs.Add(Logger.Write("Download started from " + activitiesURL));

                try
                {
                    returnResult2 = (XmlResultv2)parserIATI.ParseIATIXML(activitiesURL);
                    Logs.Add(Logger.Write("Download completed.", LoggingType.Success));
                }
                catch (Exception ex)
                {
                    Logs.Add(Logger.Write($@"<h4>Could not access the IATI Datastore</h3>
Please contact IATI Support: support@iatistandard.org <br /><br />
Please include this in your message:<br />
<br />
The Bangladesh AIMS was unable to access the IATI Datastore. Please can you confirm it is working correctly.<br />
<br />
The IATI Datastore URL accessed was: <a href=""{activitiesURL}"" target=""_blank"">{activitiesURL}</a>", LoggingType.Error));

                    return Logs;
                }

                var iatiactivityArray = returnResult2?.iatiactivities?.iatiactivity;
                if (iatiactivityArray != null && iatiactivityArray.Count > 0
                    && iatiactivityArray.n(0).AnyAttr.n(0).Value.StartsWith("1.0")) //1.04, 1.05
                {
                    //Parser v1.05
                    parserIATI = new ParserIATIv1.ParserIATIv1();
                    //activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=GB-1&stream=True"; //"http://localhost:1000/UploadedFiles/activity_GB-1_2.xml";
                    //Params: activity.xml or activity.json, recipient-country=BD, reporting-org=GB-1 or XM-DAC-12-1
                    returnResult1 = (XmlResultv1)parserIATI.ParseIATIXML(activitiesURL);

                    Logs.Add(Logger.Write("Parsing completed!", LoggingType.Success));

                    //Conversion
                    ConvertIATIv2 convertIATIv2 = new ConvertIATIv2();
                    try
                    {
                        returnResult2 = convertIATIv2.ConvertIATI105to201XML(returnResult1, returnResult2, Logs);
                    }
                    catch (Exception ex)
                    {
                        Logs.Add(Logger.Write($@"<h4>Issue with data</h4>
Please contact IATI Support: support@iatistandard.org <br /> <br />
Please include this in your message: <br />
 <br />
The Bangladesh AIMS was unable to process data fetched from the IATI Datastore for {fundSource.FundSourceName}, using Organisation Identifier: {fundSource.IATICode}.  <br />
Please can you confirm that the data is in a good shape; then we can contact the Bangladesh Government (ERD) if there are other issues with the IATI Import Module. <br />
<br />
The IATI Datastore URL accessed was: <a href=""{activitiesURL}"" target=""_blank"">{activitiesURL}</a>", LoggingType.Error));

                        return Logs;
                    }
                    Logs.Add(Logger.Write("Convertion completed!", LoggingType.Success));
                }

                #endregion

                iatiactivityArray = returnResult2?.iatiactivities?.iatiactivity;
                if (iatiactivityArray != null)
                {
                    int totalActivity = iatiactivityArray.Count();

                    if (totalActivity > 0)
                    {
                        Logs.Add(Logger.Write("Total Activity found: " + totalActivity, LoggingType.Success));
                        Console.WriteLine();

                        Logs.AddRange(SaveToDB(fundSource, iatiactivityArray));

                    }
                    else
                    {
                        Logs.Add(Logger.Write($@"<h4>No activities were found </h4>
Please contact IATI Support: support@iatistandard.org <br />
<br />
Please include this in your message: <br />
 <br />
No data was found in the IATI Datastore for {fundSource.FundSourceName}, using Organisation Identifier: {fundSource.IATICode}.  <br />
If the identifier has changed, please let us know, and we will then share this with the Bangladesh Government (ERD). <br />
 <br />
The IATI Datastore URL accessed was: <a href=""{activitiesURL}"" target=""_blank"">{activitiesURL}</a>
", LoggingType.Error));

                    }

                }
            }
            catch (DbEntityValidationException ex)
            {
                string msg = "";
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += string.Format("\nProperty: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                Logs.Add(Logger.WriteToDbAndFile(ex, LoggingType.ValidationError, fundSource.IATICode, null, msg));

            }
            catch (Exception ex)
            {
                Logs.Add(Logger.WriteToDbAndFile(ex, LoggingType.Error, fundSource.IATICode));
            }

            return Logs;
        }


        /// <summary>
        /// Save Data To DB
        /// </summary>
        /// <param name="returnResult2"></param>
        private List<Log> SaveToDB(tblFundSource fundSource, List<ParserIATIv2.iatiactivity> iatiactivityArray)
        {
            List<Log> Logs = new List<Log>();
            int counter = 1;
            int successfullySavedActivityCounter = 0;
            int totalActivity = iatiactivityArray.Count();

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
                        Logs.Add(Logger.WriteToDbAndFile(ex, LoggingType.ValidationError, fundSource.IATICode, iatiactivityItem.IatiIdentifier, messages));

                    }
                    catch (Exception ex)
                    {
                        Logs.Add(Logger.WriteToDbAndFile(ex, LoggingType.Error, fundSource.IATICode, iatiactivityItem.IatiIdentifier));
                    }

                }

                Logs.Add(Logger.Write(successfullySavedActivityCounter + " activities are stored in Database",
                    totalActivity - successfullySavedActivityCounter > 0 ? LoggingType.Alert : LoggingType.Success));
            }

            return Logs;
        }

    }
}
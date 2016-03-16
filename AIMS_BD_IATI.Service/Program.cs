using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser;
using AIMS_BD_IATI.Library.Parser.ParserIATIv1;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System.Xml.Serialization;
using System.Xml;
using AIMS_BD_IATI.DAL;

namespace AIMS_BD_IATI.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ParseIATI().Wait();
            }
            catch (Exception ex)
            {
                Log.Write("ERROR: " + ex.ToString());
            }
        }

        /// <summary>
        /// Parse IATI XML data from IATI data store and converts from v1 to v2
        /// </summary>
        /// <returns></returns>
        private static async Task ParseIATI()
        {
            IParserIATI parserIATI;
            //IConverterIATI converterIATI;
            string activitiesURL;
            XmlResultv2 returnResult2;
            XmlResultv1 returnResult1;

            //Get list of FundSource from AIMS DB
            AimsDAL _AimsDAL = new AimsDAL();
            var fundSources = _AimsDAL.GetFundSources(); //.FindAll(q => q.IATICode == "41114|XM-DAC-41114");

            foreach (var fundSource in fundSources)
            {
                Log.Write("INFO: " + "Parsing Started...for: " + fundSource.IATICode);

                #region Convert Data from v1.05 to v2.01

                //Parser v2.01
                parserIATI = new ParserIATIv2();
                //activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=CA-3&stream=True" //"http://localhost:1000/UploadedFiles/activity_GB-1_2.xml";
                //single activity : "http://datastore.iatistandard.org/api/1/access/activity.xml?iati-identifier=CA-3-A035529001
                //Params: activity.xml or activity.json, recipient-country=BD, reporting-org=CA-3
                activitiesURL = Common.iati_url + "recipient-country=" + Common.iati_recipient_country + "&reporting-org=" + fundSource.IATICode + "&stream=" + Common.iati_stream;
                returnResult2 = (XmlResultv2)parserIATI.ParseIATIXML(activitiesURL);

                var iatiactivityArray = returnResult2.n().iatiactivities.n().iatiactivity;
                if (iatiactivityArray != null && iatiactivityArray.n()[0].AnyAttr.n()[0].Value.StartsWith("1.0")) //1.04, 1.05
                {
                    //Parser v1.05
                    parserIATI = new ParserIATIv1();
                    //activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=GB-1&stream=True"; //"http://localhost:1000/UploadedFiles/activity_GB-1_2.xml";
                    //Params: activity.xml or activity.json, recipient-country=BD, reporting-org=GB-1 or XM-DAC-12-1
                    returnResult1 = (XmlResultv1)parserIATI.ParseIATIXML(activitiesURL);

                    Log.Write("INFO: " + "Parsing completed!");

                    //Conversion
                    ConvertIATIv2 convertIATIv2 = new ConvertIATIv2();
                    returnResult2 = convertIATIv2.ConvertIATI105to201XML(returnResult1, returnResult2);
                    Log.Write("INFO: " + "Convertion completed!");
                }

                #endregion

                iatiactivityArray = returnResult2.n().iatiactivities.n().iatiactivity;
                if (iatiactivityArray != null)
                {
                    SaveToDB(fundSource.IATICode, iatiactivityArray);
                }
            }
        }


        /// <summary>
        /// Save Data To DB
        /// </summary>
        /// <param name="returnResult2"></param>
        private static void SaveToDB(string organization_Id, AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivity[] iatiactivityArray)
        {
            List<Activity> Activities = new List<Activity>();

            int counter = 0;
            int totalActivity = iatiactivityArray.Count();

            Log.Write("INFO: " + "Total Activity found: " + totalActivity);

            foreach (var iatiactivityItem in iatiactivityArray)
            {
                var Activity = new Activity();

                Activity.OrgId = Activity.AssignedOrgId = organization_Id;// iatiactivityItem.reportingorg.n().@ref;
                Activity.IatiIdentifier = iatiactivityItem.iatiidentifier.n().Value;
                Activity.Hierarchy = iatiactivityItem.hierarchy;

                using (StringWriter ww = new StringWriter())
                {
                    var ss = new XmlSerializer(typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivity), new XmlRootAttribute("iati-activity"));
                    ss.Serialize(ww, iatiactivityItem);
                    Activity.IatiActivity = ww.ToString();
                }
                Activities.Add(Activity);
                Console.Write("\r Activity Counter: {0}   ", counter++);
            }

            var c = new AimsDbIatiDAL().SaveAtivities(Activities);
            Log.Write("INFO: " + "All activities are stored in Database");
        }

        /// <summary>
        /// Save Converted Data to File
        /// </summary>
        /// <param name="returnResult2"></param>
        private static void SaveToDisk(XmlResultv2 returnResult2)
        {
            var serializer = new XmlSerializer(typeof(XmlResultv2), new XmlRootAttribute("result"));
            TextWriter writer = new StreamWriter("D:\\xxv2.01.xml");
            serializer.Serialize(writer, returnResult2);
            Log.Write("INFO: " + "Saved Converted Data to File");
        }

        //#region Get Data From AIMS
        //var a = new AimsDAL().getAIMSDataInIATIFormat("CA-3");
        //#endregion
    }


}
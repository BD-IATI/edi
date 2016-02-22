using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIMS_BD_IATI.Library.Parser;
using AIMS_BD_IATI.Library.Parser.ParserIATIv1;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System.Xml.Serialization;
using System.Xml;
using AIMS_BD_IATI.DAL;
//using AIMS_BD_IATI.DAL;

namespace AIMS_BD_IATI.Service
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Parsing Started...");
            #region Convert Data from v1.05 to v2.01
            IParserIATI parserIATI;
            IConverterIATI converterIATI;
            string activitiesURL;

            //Parser v2.01
            parserIATI = new ParserIATIv2();
            //activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=CA-3&stream=True"
            //single activity : "http://datastore.iatistandard.org/api/1/access/activity.xml?iati-identifier=CA-3-A035529001
            //Params: activity.xml or activity.json, recipient-country=BD, reporting-org=CA-3
            //activitiesURL = Common.iati_url + "recipient-country=" + Common.iati_recipient_country + "&reporting-org=" + "CA-3" + "&stream="+Common.iati_stream;
            activitiesURL = "http://localhost:1000/UploadedFiles/activity_GB-1Full.xml";//"http://localhost:1000/UploadedFiles/activity_GB-1_2.xml"; //"http://localhost:1000/UploadedFiles/activity_CA-3_2.xml";
            var returnResult2 = (XmlResultv2)parserIATI.ParseIATIXML(activitiesURL);

            if (returnResult2.iatiactivities.iatiactivity[0].AnyAttr[0].Value == "1.05")
            {
                //Parser v1.05
                parserIATI = new ParserIATIv1();
                //activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=GB-1&stream=True";
                //Params: activity.xml or activity.json, recipient-country=BD, reporting-org=GB-1 or XM-DAC-12-1
                activitiesURL = "http://localhost:1000/UploadedFiles/activity_GB-1_2.xml";
                var returnResult1 = (XmlResultv1)parserIATI.ParseIATIXML(activitiesURL);

                Console.WriteLine("Parsing completed!");

                Console.WriteLine("Convertion completed...");

                //Conversion
                ConvertIATIv2 convertIATIv2 = new ConvertIATIv2();
                returnResult2 = convertIATIv2.ConvertIATI105to201XML(returnResult1, returnResult2);

            }

            #endregion
            Console.WriteLine("Convertion completed!");

            #region Save Converted Data to File

            var serializer = new XmlSerializer(typeof(XmlResultv2), new XmlRootAttribute("result"));
            TextWriter writer = new StreamWriter("D:\\xxv2.01.xml");
            serializer.Serialize(writer, returnResult2);
            Console.WriteLine("Saved Converted Data to File");

            #endregion


            #region Save Data To DB
            List<Activity> Activities = new List<Activity>();

            foreach (var iatiactivity in returnResult2.iatiactivities.iatiactivity)
            {
                var Activity = new Activity();

                Activity.Organization_Id = iatiactivity.reportingorg.@ref;
                Activity.IATI_Identifier = iatiactivity.iatiidentifier.Value;

                //Activity.Last_XML = iatiactivity.ToXmlString();
                using (StringWriter ww = new StringWriter())
                {
                    var ss = new XmlSerializer(typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivity), new XmlRootAttribute("iati-activity"));
                    ss.Serialize(ww, iatiactivity);

                    Activity.Last_XML = ww.ToString();
                }
                Activities.Add(Activity);
            }


            var c = new AimsDbIatiDAL().SaveAtivity(Activities);
            Console.WriteLine("Saved Data To DB");

            #endregion

            #region Get Data From AIMS
            //var a = new AimsDAL().getAIMSDataInIATIFormat("CA-1");

            #endregion


        }
    }

    public static class XmlTools
    {
        public static string ToXmlString<T>(this T input)
        {
            using (var writer = new StringWriter())
            {
                input.ToXml(writer);
                return writer.ToString();
            }
        }
        public static void ToXml<T>(this T objectToSerialize, Stream stream)
        {
            new XmlSerializer(typeof(T)).Serialize(stream, objectToSerialize);
        }

        public static void ToXml<T>(this T objectToSerialize, StringWriter writer)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, objectToSerialize);
        }
    }
}
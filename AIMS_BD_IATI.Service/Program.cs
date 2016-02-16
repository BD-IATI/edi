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

namespace AIMS_BD_IATI.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing Started...");
            IParserIATI parserIATI;
            string activitiesURL;
            
            //Parser v2.01
            parserIATI = new ParserIATIv2();
            //activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=CA-3&stream=True"
            //single activity : "http://datastore.iatistandard.org/api/1/access/activity.xml?iati-identifier=CA-3-A035529001
            //Params: activity.xml or activity.json, recipient-country=BD, reporting-org=CA-3
            //activitiesURL = Common.iati_url + "recipient-country=" + Common.iati_recipient_country + "&reporting-org=" + "CA-3" + "&stream="+Common.iati_stream;
            activitiesURL = "http://localhost:1000/UploadedFiles/activity_GB-1_2.xml"; //"http://localhost:1000/UploadedFiles/activity_CA-3_2.xml";
            var returnResult2 = (XmlResultv2)parserIATI.ParseIATIXML(activitiesURL);
            
            
            //Parser v1.05
            parserIATI = new ParserIATIv1();
            //activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=GB-1&stream=True";
            //Params: activity.xml or activity.json, recipient-country=BD, reporting-org=GB-1 or XM-DAC-12-1
            activitiesURL = "http://localhost:1000/UploadedFiles/activity_GB-1_2.xml";
            var returnResult1 = (XmlResultv1)parserIATI.ParseIATIXML(activitiesURL);

            //ToDo Conversion and AIMS pull
            //XmlResultv2 returnResult2 = (XmlResultv1)returnResult1;
            //returnResult2.AnyAttr
            
            Console.WriteLine("Parsing completed!");
            Console.ReadLine();
        }
    }
}

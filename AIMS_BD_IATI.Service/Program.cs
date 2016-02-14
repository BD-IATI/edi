using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;

namespace AIMS_BD_IATI.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing Started...");
            
            IParserIATI parserIATI = new ParserIATIv2();

            //"http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=BD&reporting-org=CA-3&stream=True"
            //activity.xml or activity.json
            //recipient-country=BD
            //reporting-org=CA-3
            string activitiesURL = "http://localhost:1000/UploadedFiles/activity.xml";

            var returnResult = parserIATI.ParseIATIXML(activitiesURL);

            Console.WriteLine("Parsing completed!");
            Console.ReadLine();
        }
    }
}

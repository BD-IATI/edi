﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AIMS_BD_IATI.Library.Parser;
using AIMS_BD_IATI.Library.Parser.ParserIATIv1;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_BD_IATI.DAL;

namespace AIMS_BD_IATI.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing Started...");
            IParserIATI parserIATI;
            IConverterIATI converterIATI;
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

            Console.WriteLine("Parsing completed!");

            Console.WriteLine("Convertion completed...");

            //Conversion
            ConvertIATIv2 convertIATIv2 = new ConvertIATIv2();
            convertIATIv2.ConvertIATI105to201XML(returnResult1, returnResult2);

            Console.WriteLine("Convertion completed!");
            Console.ReadLine();
            
            var a = AimsDAL.getProjects("GB-1");
            string b = a.FirstOrDefault().tblFundSource.FundSourceName;
        }
    }
}
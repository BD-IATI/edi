using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//added reference Assemblies->Framework->System.Configuration
using System.Configuration;
using System.IO;
using AIMS_BD_IATI.DAL;
using System.Xml.Serialization;
using Newtonsoft.Json;
using AIMS_BD_IATI.Library;

namespace AIMS_BD_IATI.Service
{
    public sealed class Common
    {
        public static string iati_url
        {
            get { return ConfigurationManager.AppSettings["iati_url"]; }
        }
        public static string iati_recipient_country
        {
            get { return ConfigurationManager.AppSettings["iati_recipient-country"]; }
        }
        public static string iati_reporting_org
        {
            get { return ConfigurationManager.AppSettings["iati_reporting-org"]; }
        }
        public static string iati_stream
        {
            get { return ConfigurationManager.AppSettings["iati_stream"]; }
        }
    }

}

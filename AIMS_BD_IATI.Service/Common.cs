using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//added reference Assemblies->Framework->System.Configuration
using System.Configuration;
using System.IO;

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

    public sealed class Log
    {
        public static void Write(string Text)
        {
            string dirPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Log\\";

            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            string filePath = dirPath + DateTime.Now.ToString("yyyy-MM-dd") + "_Log.txt";

            File.AppendAllText(filePath,
                Environment.NewLine +
                DateTime.Now.ToString("d-MMM-yyyy, HH:mm:ss") + "  " + Text);

            System.Console.WriteLine(Text);
        }
    }
}

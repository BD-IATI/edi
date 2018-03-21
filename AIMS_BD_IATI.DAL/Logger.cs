using AIMS_BD_IATI.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.DAL
{
    public sealed class Logger
    {
        public static Log Write(string Text, LoggingType logType = LoggingType.Info)
        {
            string dirPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? "" + "\\Log\\";

            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            string filePath = dirPath + DateTime.Now.ToString("yyyy-MM-dd") + "_Log.txt";

            File.AppendAllText(filePath,
                Environment.NewLine +
                DateTime.Now.ToString("d-MMM-yyyy, HH:mm:ss") + "  " + Text);

            Console.WriteLine(Text);

            return new Log { DateTime = DateTime.Now, LogType = (int)logType, Message = Text };
        }

        public static Log WriteToDbAndFile(dynamic ex, LoggingType type, string orgId = null, string IatiIdentifier = null, string message = null)
        {
            Log log = new Log();
            if (message == null) message = ex.Message;

            log.DateTime = DateTime.Now;
            log.OrgId = orgId;
            log.IatiIdentifier = IatiIdentifier;
            log.Message = message;
            log.LogType = (int?)type;

            try
            {
                using (TextWriter writer = new StringWriter())
                {
                    try
                    {
                        new JsonSerializer().Serialize(new JsonTextWriter(writer), ex);
                        log.ExceptionObj = writer.ToString();
                    }
                    catch { }
                }

                new AimsDbIatiDAL().InsertLog(log);
            }
            catch { }

            try
            {
                Write(type + " " + message);
            }
            catch { }

            return log;
        }

    }
}

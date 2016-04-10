using AIMS_BD_IATI.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.DAL
{
    public sealed class Logger
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

        public static void WriteToDbAndFile(dynamic ex, LogType type, string orgId = null, string IatiIdentifier = null, string message = null)
        {
            try
            {
                if (message == null) message = ex.Message;

                Log log = new Log();
                log.DateTime = DateTime.Now;
                log.OrgId = orgId;
                log.IatiIdentifier = IatiIdentifier;
                log.Message = message;
                log.LogType = type.GetHashCode();

                using (TextWriter writer = new StringWriter())
                {
                    new JsonSerializer().Serialize(new JsonTextWriter(writer), ex);
                    log.ExceptionObj = writer.ToString();
                }

                new AimsDbIatiDAL().InsertLog(log);
            }
            catch { }
            finally
            {
                Write(type + " " + message);
            }
        }

    }
}

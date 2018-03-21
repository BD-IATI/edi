using AIMS_BD_IATI.DAL;
using System.Collections.Generic;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    public class DownloadLogModel
    {
        public List<Log> Logs { get; set; }
    }
}
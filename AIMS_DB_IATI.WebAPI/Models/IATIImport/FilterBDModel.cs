using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    public class FilterBDModel
    {
        public List<iatiactivity> iatiActivities { get; set; }
    }
}
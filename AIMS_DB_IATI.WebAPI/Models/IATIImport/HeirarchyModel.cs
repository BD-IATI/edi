using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    [Serializable]
    public class HeirarchyModel
    {
        public string Id { get; set; } 

        public HeirarchyModel()
        {
            SampleIatiActivity = new iatiactivity();
        }

        public iatiactivity SampleIatiActivity { get; set; }
        public decimal H1Percent { get; set; }
        public decimal H2Percent { get; set; }
        public int SelectedHierarchy { get; set; }
    }
}
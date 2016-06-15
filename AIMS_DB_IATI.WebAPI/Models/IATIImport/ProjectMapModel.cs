using AIMS_BD_IATI.DAL;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIMS_DB_IATI.WebAPI.Models.IATIImport
{
    [Serializable]
    public class ProjectMapModel
    {

        public object selected { get; set; }

        public List<ProjectFieldMapModel> MatchedProjects { get; set; }

        public List<iatiactivity> IatiActivitiesNotInAims { get; set; }
        public List<iatiactivity> AimsProjectsNotInIati { get; set; }
        public List<iatiactivity> NewProjectsToAddInAims { get; set; }
        public List<iatiactivity> ProjectsOwnedByOther { get; set; }

    }
    [Serializable]
    public class ProjectMapModelMinified
    {

        public object selected { get; set; }

        public List<ProjectFieldMapModelMinified> MatchedProjects { get; set; }

        public List<iatiactivityModel> IatiActivitiesNotInAims { get; set; }
        public List<iatiactivityModel> AimsProjectsNotInIati { get; set; }
        public List<iatiactivityModel> NewProjectsToAddInAims { get; set; }
        public List<iatiactivityModel> ProjectsOwnedByOther { get; set; }
        public List<LookupItem> AimsProjectsDrpSrc { get; internal set; }
    }

}


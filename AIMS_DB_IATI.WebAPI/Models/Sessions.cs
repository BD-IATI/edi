using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_DB_IATI.WebAPI.Models.IATIImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIMS_DB_IATI.WebAPI.Models
{
    public static class Sessions
    {
        public static iatiactivityContainer activitiesContainer
        {
            get
            {
                return HttpContext.Current.Session["iatiactivityContainer"] == null ?
                    null
                    : (iatiactivityContainer)HttpContext.Current.Session["iatiactivityContainer"];
            }
            set { HttpContext.Current.Session["iatiactivityContainer"] = value; }
        }
        public static HeirarchyModel heirarchyModel
        {
            get
            {
                return HttpContext.Current.Session["HeirarchyModel"] == null ?
                    null
                    : (HeirarchyModel)HttpContext.Current.Session["HeirarchyModel"];
            }
            set { HttpContext.Current.Session["HeirarchyModel"] = value; }
        }

        public static ProjectFieldMapModel GeneralPreferences
        {
            get
            {
                return HttpContext.Current.Session["GeneralPreferences"] == null ?
                    null
                    : (ProjectFieldMapModel)HttpContext.Current.Session["GeneralPreferences"];
            }
            set { HttpContext.Current.Session["GeneralPreferences"] = value; }
        }

        public static ProjectMapModel ProjectMapModel
        {
            get
            {
                return HttpContext.Current.Session["ProjectMapModel"] == null ?
                    null
                    : (ProjectMapModel)HttpContext.Current.Session["ProjectMapModel"];
            }
            set { HttpContext.Current.Session["ProjectMapModel"] = value; }
        }

        public static List<FundSourceLookupItem> FundSources
        {
            get
            {
                return HttpContext.Current.Session["FundSources"] == null ?
                    new List<FundSourceLookupItem>()
                    : (List<FundSourceLookupItem>)HttpContext.Current.Session["FundSources"];
            }
            set { HttpContext.Current.Session["FundSources"] = value; }
        }

    }
}
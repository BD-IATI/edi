using AIMS_BD_IATI.DAL;
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
                    new iatiactivityContainer()
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
                    new ProjectFieldMapModel()
                    : (ProjectFieldMapModel)HttpContext.Current.Session["GeneralPreferences"];
            }
            set { HttpContext.Current.Session["GeneralPreferences"] = value; }
        }

        public static ProjectMapModel ProjectMapModel
        {
            get
            {
                return HttpContext.Current.Session["ProjectMapModel"] == null ?
                    new ProjectMapModel()
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

        public static string UserId
        {
            get
            {
                return HttpContext.Current.Session["UserId"] == null ?
                    ""
                    : Convert.ToString(HttpContext.Current.Session["UserId"]);
            }
            set { HttpContext.Current.Session["UserId"] = value; }
        }

        public static DPLookupItem DP
        {
            get
            {
                return HttpContext.Current.Session["CurrentDP"] == null ?
                    new DPLookupItem()
                    : (DPLookupItem)HttpContext.Current.Session["CurrentDP"];
            }
            set
            {
                HttpContext.Current.Session["CurrentDP"] = value;
            }
        }

        public static CFnTFModel CFnTFModel
        {
            get
            {
                return HttpContext.Current.Session["CFnTFModel"] == null ?
                    new CFnTFModel()
                    : (CFnTFModel)HttpContext.Current.Session["CFnTFModel"];
            }
            set { HttpContext.Current.Session["CFnTFModel"] = value; }
        }

        public static List<iatiactivity> TrustFunds
        {
            get
            {
                return HttpContext.Current.Session["AssignedActivities"] == null ?
                    new List<iatiactivity>()
                    : (List<iatiactivity>)HttpContext.Current.Session["AssignedActivities"];
            }
            set { HttpContext.Current.Session["AssignedActivities"] = value; }
        }
    }
}
using AIMS_BD_IATI.DAL;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_DB_IATI.WebAPI.Models.IATIImport;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Serialization;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Raven.Client.Document;

namespace AIMS_DB_IATI.WebAPI.Models
{
    public static class Sessions
    {
        static IDocumentStore DocumentStore;
        static Sessions()
        {
            DocumentStore = RavenDbConfig.Initialize();
        }
        private static void SaveSession<T>(T value, string id)
        {
            using (IDocumentSession DocumentSession = DocumentStore.OpenSession())
            {
                //var dj = DocumentSession.Load<dynamic>(id);
                var d = DocumentSession.Load<T>(typeof(T).Name + "/" + id);

                if (d == null)
                    DocumentSession.Store(value, typeof(T).Name + "/" + id);

                    d = value;

                DocumentSession.SaveChanges();
            }
        }
        private static T GetSession<T>(string id)
        {
            T d;
            using (IDocumentSession DocumentSession = DocumentStore.OpenSession())
            {
                //var dj = DocumentSession.Load<dynamic>(id);

                d = DocumentSession.Load<T>(typeof(T).Name + "/" + id);
            }
            return d;
        }
        //private static void SaveSession<T>(T value, string id)
        //{
        //    var d = DocumentSession.Load<SessionHolder>(id);

        //    if (d == null)
        //        DocumentSession.Store(new SessionHolder { Val = value }, id);
        //    else
        //        d.Val = value;

        //    DocumentSession.SaveChanges();
        //}
        //private static T GetSession<T>(string id)
        //{
        //    var d = DocumentSession.Load<SessionHolder>(id);

        //    return (T) d.Val;
        //}

        internal static void Clear()
        {
            //activitiesContainer = null;
            //heirarchyModel = null;
            //GeneralPreferences = null;
            //ProjectMapModel = null;
            //CFnTFModel = null;
            //TrustFunds = null;
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

        public static iatiactivityContainer activitiesContainer
        {
            get
            {
                return GetSession<iatiactivityContainer>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3)) ?? new iatiactivityContainer();
            }
            set
            {
                SaveSession(value, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }
        }

        public static HeirarchyModel heirarchyModel
        {
            get
            {
                return GetSession<HeirarchyModel>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3)) ?? new HeirarchyModel();
            }
            set
            {
                SaveSession(value, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }
        }



        public static ProjectFieldMapModel GeneralPreferences
        {
            get
            {
                return GetSession<ProjectFieldMapModel>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3)) ?? new ProjectFieldMapModel();
            }
            set
            {
                SaveSession(value, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));

            }
        }

        public static ProjectMapModel ProjectMapModel
        {
            get
            {
                return GetSession<ProjectMapModel>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3)) ?? new ProjectMapModel();
            }
            set
            {
                SaveSession(value, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));

            }
        }

        public static List<FundSourceLookupItem> FundSources
        {
            get
            {
                dynamic d = GetSession<dynamic>("List<FundSourceLookupItem>/" + UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
                return d == null ? new List<FundSourceLookupItem>() : d.val;

            }
            set
            {
                using (IDocumentSession DocumentSession = DocumentStore.OpenSession())
                {
                    dynamic d = DocumentSession.Load<dynamic>("List<FundSourceLookupItem>/" + UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
                    if (d == null)
                        DocumentSession.Store(new { val = value ?? new List<FundSourceLookupItem>() }, "List<FundSourceLookupItem>/" + UserId + MethodBase.GetCurrentMethod().Name.Substring(3));

                        d = new { val = value ?? new List<FundSourceLookupItem>() };

                    DocumentSession.SaveChanges();
                }

            }

        }

        public static DPLookupItem DP
        {
            get
            {
                return GetSession<DPLookupItem>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3)) ?? new DPLookupItem();
            }
            set
            {
                SaveSession(value, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }

        }

        public static CFnTFModel CFnTFModel
        {
            get
            {
                return //GetSession<CFnTFModel>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3)) ?? 
                    new CFnTFModel();
            }
            set
            {
                //SaveSession(value, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }
        }

        public static List<iatiactivity> TrustFunds
        {
            get
            {
                dynamic d = GetSession<dynamic>("List<iatiactivity>/" + UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
                return d == null ? new List<iatiactivity>() : d.val;
            }
            set
            {
                using (IDocumentSession DocumentSession = DocumentStore.OpenSession())
                {
                    dynamic d = DocumentSession.Load<dynamic>("List<iatiactivity>/" + UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
                    if (d == null)
                        DocumentSession.Store(new { val = value ?? new List<iatiactivity>() }, "List<iatiactivity>/" + UserId + MethodBase.GetCurrentMethod().Name.Substring(3));

                        d = new { val = value ?? new List<iatiactivity>() };
                    DocumentSession.SaveChanges();
                }

            }
        }

        public static List<LookupItem> ExecutingAgencyTypes
        {
            get
            {
                dynamic d = GetSession<dynamic>("List<LookupItem>/" + UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
                return d == null ? new List<LookupItem>() : d.val;

            }
            set
            {
                using (IDocumentSession DocumentSession = DocumentStore.OpenSession())
                {
                    dynamic d = DocumentSession.Load<dynamic>("List<LookupItem>/" + UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
                    if (d == null)
                        DocumentSession.Store(new { val = value ?? new List<LookupItem>() }, "List<LookupItem>/" + UserId + MethodBase.GetCurrentMethod().Name.Substring(3));

                        d = new { val = value ?? new List<LookupItem>() };

                    DocumentSession.SaveChanges();
                }
            }
        }
    }

    public class SessionHolder
    {
        public object Val { get; set; }
    }

    public class RavenDbConfig
    {
        private static IDocumentStore _store;
        public static IDocumentStore Store
        {
            get
            {
                if (_store == null)
                    Initialize();
                return _store;
            }
        }

        public static IDocumentStore Initialize()
        {
            _store = new EmbeddableDocumentStore
            {
                ConnectionStringName = "SessionDB",
                DefaultDatabase = "IATIDB"
                //UseEmbeddedHttpServer = true
            };

            //_store.Conventions.IdentityPartsSeparator = "-";
            _store.Conventions.JsonContractResolver = new DynamicContractResolver();
            //_store.AggressivelyCache();
            _store.Conventions.MaxNumberOfRequestsPerSession = 4096;
            _store.Initialize();
            //IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), Store);

            return _store;
        }
    }


    public class DynamicContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type,
            MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            properties = properties.Where(p => p.PropertyName != "AnyAttr" && p.PropertyName != "Any").ToList();
            return properties;
        }
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return new List<MemberInfo>();
        }
    }

}
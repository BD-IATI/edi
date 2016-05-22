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
using System.Threading.Tasks;
using System.Threading;
using System.Web.Hosting;

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
            var docId = typeof(T).Name + "/" + id;
            HttpContext.Current.Session[docId] = value;

            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            {
                using (IAsyncDocumentSession DocumentSession = DocumentStore.OpenAsyncSession())
                {
                    //T d;
                    if (value == null)
                    {
                        DocumentSession.Delete(docId);
                    }
                    else
                    {
                        //d = await DocumentSession.LoadAsync<T>(docId);

                        //if (d == null)
                            await DocumentSession.StoreAsync(value, docId);

                        //d = value;
                    }
                    await DocumentSession.SaveChangesAsync();
                }
            });
        }

        private static T GetSession<T>(string id)
        {
            var docId = typeof(T).Name + "/" + id;
            T d = HttpContext.Current.Session[docId] == null ? default(T) : (T)HttpContext.Current.Session[docId];

            if (d == null)
                using (IDocumentSession DocumentSession = DocumentStore.OpenSession())
                {
                    d = DocumentSession.Load<T>(docId);
                }
            return d;
        }

        internal static void Clear()
        {
            CurrentStage = Stage.Begin;
            activitiesContainer = null;
            heirarchyModel = null;
            filterBDModel = null;
            iOrgs = null;
            GeneralPreferences = null;
            ProjectMapModel = null;
            ProjectsToMap = null;
            CFnTFModel = null;
            TrustFunds = null;
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

        public static string CurrentStage
        {
            get
            {
                KeyVal d = GetSession<KeyVal>(UserId + DP.ID + MethodBase.GetCurrentMethod().Name.Substring(3));
                return d == null ? Stage.Begin : d.Val;

            }
            set
            {
                KeyVal d = new KeyVal { Val = value };
                SaveSession(d, UserId + DP.ID + MethodBase.GetCurrentMethod().Name.Substring(3));

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
                return GetSession<HeirarchyModel>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }
            set
            {
                SaveSession(value, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }

            //get
            //{
            //    var id = UserId + MethodBase.GetCurrentMethod().Name.Substring(3);
            //    return (HeirarchyModel)HttpContext.Current.Session[id];
            //}
            //set
            //{
            //    var id = UserId + MethodBase.GetCurrentMethod().Name.Substring(3);
            //    HttpContext.Current.Session[id] = value;
            //}
        }

        public static FilterBDModel filterBDModel
        {
            get
            {
                return GetSession<FilterBDModel>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }
            set
            {
                SaveSession(value, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }
        }

        public static iOrgs iOrgs
        {
            get
            {
                return GetSession<iOrgs>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
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

        public static ProjectMapModel ProjectsToMap
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

        public static CFnTFModel CFnTFModel
        {
            get
            {
                return GetSession<CFnTFModel>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3)) ??
                    new CFnTFModel();
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
                KeyVal d = GetSession<KeyVal>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
                return d == null ? new List<FundSourceLookupItem>() : d.Val;

            }
            set
            {
                KeyVal d = new KeyVal { Val = value ?? new List<FundSourceLookupItem>() };
                SaveSession(d, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));

            }

        }

        public static List<iatiactivity> TrustFunds
        {
            get
            {
                KeyVal d = GetSession<KeyVal>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
                return d == null ? new List<iatiactivity>() : d.Val;
            }
            set
            {
                KeyVal d = new KeyVal { Val = value ?? new List<iatiactivity>() };
                SaveSession(d, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }
        }

        public static List<LookupItem> ExecutingAgencyTypes
        {
            get
            {
                KeyVal d = GetSession<KeyVal>(UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
                return d == null ? new List<LookupItem>() : d.Val;

            }
            set
            {
                KeyVal d = new KeyVal { Val = value ?? new List<LookupItem>() };
                SaveSession(d, UserId + MethodBase.GetCurrentMethod().Name.Substring(3));
            }
        }
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
                ConnectionStringName = "SessionEDB",
                DefaultDatabase = "IATIDB"
                //UseEmbeddedHttpServer = true
            };

            //_store.Conventions.IdentityPartsSeparator = "-";
            _store.Conventions.JsonContractResolver = new DynamicContractResolver();
            _store.Conventions.MaxNumberOfRequestsPerSession = 4096;
            _store.Initialize();
            _store.DisableAggressiveCaching();
            //IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), Store);
            //_store.AggressivelyCache();

            return _store;
        }
    }


    public class DynamicContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type,
            MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, MemberSerialization.OptOut);

            return properties;
        }
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var members = base.GetSerializableMembers(objectType);

            members.RemoveAll(r => r.MemberType != MemberTypes.Property);
            members.RemoveAll(r => ((PropertyInfo)r).CanWrite == false);
            members.RemoveAll(r => r.Name == "AnyAttr" || r.Name == "Any");

            return members;
        }
    }

    public class KeyVal
    {
        public dynamic Val { get; set; }
    }

    public static class Stage
    {
        public const string Begin = "/0Begin";
        public const string Hierarchy = "/1Hierarchy";
        public const string FilterBD = "/2FilterBD";
        public const string FilterDP = "/3FilterDP";
        public const string ShowProjects = "/4Projects";
        public const string MatchProjects = "/5Match";
        public const string GeneralPreferences = "/6GeneralPreferences";
        public const string ReviewAdjustment = "/7ReviewAdjustment";



    }
}
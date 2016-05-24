using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.SessionState;
using System.Xml.Serialization;

namespace AIMS_BD_IATI.WebAPIAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //JSON Media-Type Formatter ref: http://www.asp.net/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DynamicContractResolver(); //DefaultContractResolver

            //XML Media-Type Formatter ref:http://www.asp.net/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.UseXmlSerializer = true;

            /*
            //switched off XML and forced only JSON to be returned.
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            */
        }

        public class DynamicContractResolver : DefaultContractResolver
        {
            //protected override IList<JsonProperty> CreateProperties(Type type,
            //    MemberSerialization memberSerialization)
            //{
            //    IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            //    properties = properties.Where(p => p.PropertyName != "AnyAttr" && p.PropertyName != "Any").ToList();
            //    return properties;
            //}

            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                var members = base.GetSerializableMembers(objectType);

                members.RemoveAll(r => r.MemberType != MemberTypes.Property);
                members.RemoveAll(r => r.GetCustomAttribute(typeof(XmlElementAttribute)) != null);
                members.RemoveAll(r => r.GetCustomAttribute(typeof(XmlAnyElementAttribute)) != null);
                members.RemoveAll(r => r.GetCustomAttribute(typeof(XmlAnyAttributeAttribute)) != null);

                members.RemoveAll(r => r.Name == "AnyAttr" || r.Name == "Any");

                return members;
            }

        }

        public override void Init()
        {
            this.PostAuthenticateRequest += _PostAuthenticateRequest;
            base.Init();
        }

        void _PostAuthenticateRequest(object sender, EventArgs e)
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(
                SessionStateBehavior.Required);
        }
    }
}

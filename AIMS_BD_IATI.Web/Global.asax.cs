using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace AIMS_BD_IATI.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //JSON Media-Type Formatter ref: http://www.asp.net/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver();

            //XML Media-Type Formatter ref:http://www.asp.net/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.UseXmlSerializer = true;

            /*
            //switched off XML and forced only JSON to be returned.
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            */
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

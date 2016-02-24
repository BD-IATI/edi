using AIMS_BD_IATI.DAL;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AIMS_BD_IATI.Web.Controllers
{
    [RoutePrefix("api/ApiHome")]
    public class ApiHomeController : ApiController
    {
        public List<iatiactivity> iatiactivities
        {
            get
            {
                return HttpContext.Current.Session["iatiactivities"] == null ? 
                    null 
                    : (List<iatiactivity>)HttpContext.Current.Session["iatiactivities"];
            }
            set { HttpContext.Current.Session["iatiactivities"] = value; }
        }

        static List<IatiProject> iatiProjects = new List<IatiProject>
        { 
            new IatiProject { title = "Tomato Soup", description = "Groceries"}, 
            new IatiProject { title = "Yo-yo", description = "Toys" }, 
            new IatiProject { title = "Hammer", description = "Dhaka, BD" } 
        };

        static List<AimsProject> aimsProjects = new List<AimsProject>
        { 
            new AimsProject { title = "SoupTomatoooooooooo ", description = "Groceries", matchedProjects = new List<IatiProject>()}, 
            new AimsProject { title = "-yoYo", description = "Toys", matchedProjects = new List<IatiProject>() }, 
            new AimsProject { title = "merHam", description = "Dhaka, BD", matchedProjects = new List<IatiProject>() } 
        };


        RootObject DataModel = new RootObject() { iatiProjects = iatiProjects, aimsProjects = aimsProjects };

        public List<iatiactivity> GetProjectHierarchyData()
        {

            if (iatiactivities == null)
                iatiactivities = new AimsDbIatiDAL().GetActivities("GB-1");

            return iatiactivities;
            //TextWriter tw = new StringWriter();

            //new Newtonsoft.Json.JsonSerializer().Serialize(tw, iatiactivities);

            //return tw.ToString();
        }

        //[HttpGet]
        public RootObject GetData()
        {
            return DataModel;
        }
        public List<DropdownItem> GetFundSources()
        {
            return new AimsDAL().getFundSourcesDropdownData();
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostData(RootObject dataModel)
        {
            return Ok(DataModel);
        }

        //[HttpPut]
        //public async Task<IHttpActionResult> PutEmployee(EmployeeModel employee)
        //{
        //    employee.Id = 22;
        //    emp.Add(employee);
        //    return Ok(employee);
        //}
    }

    public class IatiProject
    {
        public string title { get; set; }
        public string description { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
    }

    public class AimsProject
    {
        public string title { get; set; }
        public string description { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public IList<IatiProject> matchedProjects { get; set; }
    }

    public class RootObject
    {
        public object selected { get; set; }
        public IList<IatiProject> iatiProjects { get; set; }
        public IList<AimsProject> aimsProjects { get; set; }
    }

}


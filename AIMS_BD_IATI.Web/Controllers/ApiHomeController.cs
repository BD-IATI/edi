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
using MoreLinq;

namespace AIMS_BD_IATI.Web.Controllers
{
    [RoutePrefix("api/ApiHome")]
    public class ApiHomeController : ApiController
    {
        public iatiactivityContainer iatiactivities
        {
            get
            {
                return HttpContext.Current.Session["iatiactivityContainer"] == null ?
                    null
                    : (iatiactivityContainer)HttpContext.Current.Session["iatiactivityContainer"];
            }
            set { HttpContext.Current.Session["iatiactivityContainer"] = value; }
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

        public iatiactivityContainer GetProjectHierarchyData(string dp)
        {
            //if (iatiactivities == null)
            iatiactivities = new AimsDbIatiDAL().GetActivities(dp);

            return iatiactivities;
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
        [HttpPost]
        public async Task<IHttpActionResult> SubmitHierarchy(List<iatiactivity> _iatiactivities)
        {
            //var SelectedHierarchy1Activities = _iatiactivities.n().FindAll(f => f.SelectedHierarchy == "Heirarchy1");
            //foreach (var activity in SelectedHierarchy1Activities)
            //{
            //    if  (activity.defaultaidtype == null)
            //    {

            //        //activity.defaultaidtype.code == activity.relatedIatiActivities.Max(m=>m.transaction.Sum(s=>s..value.Value))
            //    }
            //}

            return Ok(_iatiactivities);
        }

        [HttpPost]
        public ProjectMapModel SubmitActivities([FromUri]string orgId,List<iatiactivity> _iatiactivities)
        {
            var relevantActivies = _iatiactivities.n().FindAll(f => f.IsRelevant == true);

            var AimsProjects = new AimsDAL().getAIMSDataInIATIFormat(orgId);

            var MatchedProjects = (from i in relevantActivies
                                   from a in AimsProjects.Where(k=>i.iatiidentifier.Value.EndsWith(k.iatiidentifier.Value)) 
                                   orderby i.iatiidentifier.Value

                                   select i).ToList();

            //for showing mathced projects side by side
            var MatchedProjects2 = (from i in relevantActivies
                                    from a in AimsProjects.Where(k => i.iatiidentifier.Value.EndsWith(k.iatiidentifier.Value))
                                    orderby i.iatiidentifier.Value
                                    select new MatchedProject
                                    {
                                        iatiActivity = i,
                                        aimsProjects = a
                                    }).ToList();

            var IatiActivityNotInAims = relevantActivies.Except(MatchedProjects).ToList();


            var AimsProjectNotInIati = AimsProjects.ExceptBy(MatchedProjects, f => f.iatiidentifier.Value).ToList();


            return new ProjectMapModel
            {
                MatchedProjects = MatchedProjects2,
                IatiActivitiesNotInAims = IatiActivityNotInAims,
                AimsProjectsNotInIati = AimsProjectNotInIati,
                NewProjectsToAddInAims = new List<iatiactivity>()
            };
        }
        //[HttpPut]
        //public async Task<IHttpActionResult> PutEmployee(EmployeeModel employee)
        //{
        //    employee.Id = 22;
        //    emp.Add(employee);
        //    return Ok(employee);
        //}
    }


    public class ProjectMapModel
    {
        public object selected { get; set; }

        public List<MatchedProject> MatchedProjects { get; set; }
        public List<iatiactivity> IatiActivitiesNotInAims { get; set; }
        public List<iatiactivity> AimsProjectsNotInIati { get; set; }
        public List<iatiactivity> NewProjectsToAddInAims { get; set; }


    }

    public class MatchedProject
    {
        public iatiactivity iatiActivity { get; set; }
        public iatiactivity aimsProjects { get; set; }
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


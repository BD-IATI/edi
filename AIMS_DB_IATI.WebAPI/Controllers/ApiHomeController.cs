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

namespace AIMS_BD_IATI.WebAPI.Controllers
{
    [RoutePrefix("api/ApiHome")]
    public class ApiHomeController : ApiController
    {
        public iatiactivityContainer s_activitiesContainer
        {
            get
            {
                return HttpContext.Current.Session["iatiactivityContainer"] == null ?
                    null
                    : (iatiactivityContainer)HttpContext.Current.Session["iatiactivityContainer"];
            }
            set { HttpContext.Current.Session["iatiactivityContainer"] = value; }
        }
        public HeirarchyModel s_heirarchyModel
        {
            get
            {
                return HttpContext.Current.Session["HeirarchyModel"] == null ?
                    null
                    : (HeirarchyModel)HttpContext.Current.Session["HeirarchyModel"];
            }
            set { HttpContext.Current.Session["HeirarchyModel"] = value; }
        }

        public List<FundSourceLookupItem> FundSources
        {
            get
            {
                return HttpContext.Current.Session["FundSources"] == null ?
                    new List<FundSourceLookupItem>()
                    : (List<FundSourceLookupItem>)HttpContext.Current.Session["FundSources"];
            }
            set { HttpContext.Current.Session["FundSources"] = value; }
        }

        public List<DPLookupItem> GetFundSources()
        {
            return new AimsDAL().GetFundSourcesDropdownData();
        }
        public List<FundSourceLookupItem> GetAllFundSources()
        {
            FundSources = new AimsDAL().GetAllFundSources();
            return FundSources;
        }

        [AcceptVerbs("GET", "POST")]
        public HeirarchyModel GetHierarchyData(DPLookupItem dp)
        {
            bool isDPChanged = s_activitiesContainer.n().DP != dp.ID;

            if (isDPChanged)
            {
                s_heirarchyModel = new HeirarchyModel();

                s_activitiesContainer = new AimsDbIatiDAL().GetActivities(dp.ID);

                if (s_activitiesContainer.HasRelatedActivity)
                {
                    var H1Acts = s_activitiesContainer.iatiActivities.FindAll(f => f.hierarchy == 1);
                    var H2Acts = s_activitiesContainer.iatiActivities.FindAll(f => f.hierarchy == 2);

                    var AimsProjects = new AimsDAL().getAIMSDataInIATIFormat(dp.ID);

                    var matchedH1 = (decimal)(from a in AimsProjects
                                              join i in H1Acts on a.IatiIdentifier equals i.IatiIdentifier
                                              select a).Count();

                    var matchedH2 = (decimal)(from a in AimsProjects
                                              join i in H2Acts on a.IatiIdentifier equals i.IatiIdentifier
                                              select a).Count();


                    s_heirarchyModel.H1Percent = H1Acts.Count > 0 ? Math.Round((decimal)(matchedH1 / H1Acts.Count) * 100, 2) : 0;
                    s_heirarchyModel.H2Percent = H2Acts.Count > 0 ? Math.Round((decimal)(matchedH2 / H2Acts.Count) * 100, 2) : 0;



                    #region Populate relatedActivities of the first activity as sample data
                    var parentActivities = s_activitiesContainer.iatiActivities.FindAll(x => x.hierarchy == 1);
                    foreach (var pa in parentActivities)
                    {
                        if (pa.relatedactivity != null)
                        {
                            foreach (var ra in pa.relatedactivity.Where(r => r.type == "2"))
                            {
                                //load related activities
                                var ha = s_activitiesContainer.iatiActivities.Find(f => f.iatiidentifier.Value == ra.@ref);

                                if (ha != null)
                                {
                                    pa.relatedIatiActivities.Add(ha);
                                }
                            }
                            s_heirarchyModel.SampleIatiActivity = pa;
                            break; //we have to show only one hierarchycal project as a sample
                        }
                    }
                    #endregion
                    s_heirarchyModel.SelectedHierarchy = 1;
                }
                else
                {
                    s_heirarchyModel = null;
                }
            }
            return s_heirarchyModel;
        }




        [HttpPost]
        public FilterBDModel SubmitHierarchy(HeirarchyModel heirarchyModel)
        {
            var returnResult = new FilterBDModel();

            if (heirarchyModel == null)
            {
                returnResult.iatiActivities = s_activitiesContainer.iatiActivities;
            }
            else
            {
                returnResult.iatiActivities = s_activitiesContainer.iatiActivities.FindAll(f => f.n().hierarchy == heirarchyModel.n().SelectedHierarchy);

                if (heirarchyModel.SelectedHierarchy == 1)
                {
                    foreach (var pa in returnResult.iatiActivities)
                    {
                        pa.relatedIatiActivities.Clear();
                        if (pa.relatedactivity != null)
                        {
                            foreach (var ra in pa.relatedactivity.Where(r => r.type == "2"))
                            {
                                //load related activities
                                var ha = s_activitiesContainer.iatiActivities.Find(f => f.iatiidentifier.Value == ra.@ref);

                                if (ha != null)
                                {
                                    pa.relatedIatiActivities.Add(ha);
                                }
                            }
                        }
                    }
                }

            }

            return returnResult;
        }

        [HttpPost]
        public object GetAllImplementingOrg(FilterBDModel filterDBModel)
        {
            if (filterDBModel != null)
                s_activitiesContainer.iatiActivities = filterDBModel.iatiActivities;

            var iOrgs = new List<participatingorg>();
            s_activitiesContainer.RelevantActivities.ForEach(e => iOrgs.AddRange(e.participatingorg.n().Where(w => w.role == "4").ToList()));

            var oo = iOrgs.DistinctBy(l => l.@ref);

            return new
            {
                Orgs = oo.OrderBy(o => o.@ref),
                FundSources = GetAllFundSources()
            };
        }

        [HttpPost]
        public List<iatiactivity> FilterDP(List<participatingorg> _iOrgs)
        {

            var iOrgs = new List<participatingorg>();
            s_activitiesContainer.RelevantActivities.ForEach(e => iOrgs.AddRange(e.participatingorg.n().Where(w => w.role == "4").ToList()));

            foreach (var iOrg in _iOrgs)
            {
                iOrgs.FindAll(f => f.@ref == iOrg.@ref).ForEach(e => e.FundSourceIDnIATICode = iOrg.FundSourceIDnIATICode);
            }

            return s_activitiesContainer.RelevantActivities;
        }

        [AcceptVerbs("GET", "POST")]
        public ProjectMapModel SubmitActivities(List<iatiactivity> relevantActivies)
        {
            iatiactivity.FundSources = FundSources;

            var ProjectsOwnedByOther = relevantActivies.FindAll(f => f.IATICode != s_activitiesContainer.DP);

            relevantActivies.RemoveAll(f => f.IATICode != s_activitiesContainer.DP);

            var AimsProjects = new AimsDAL().getAIMSDataInIATIFormat(s_activitiesContainer.n().DP);

            var MatchedProjects = (from i in relevantActivies
                                   from a in AimsProjects.Where(k => i.iatiidentifier.Value.EndsWith(k.iatiidentifier.Value))
                                   orderby i.iatiidentifier.Value

                                   select i).ToList();

            //for showing mathced projects side by side
            var MatchedProjects2 = (from i in relevantActivies
                                    from a in AimsProjects.Where(k => i.iatiidentifier.Value.EndsWith(k.iatiidentifier.Value))
                                    orderby i.iatiidentifier.Value
                                    select new ProjectMapModel.MatchedProject
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
                NewProjectsToAddInAims = new List<iatiactivity>(),
                ProjectsOwnedByOther = ProjectsOwnedByOther
            };
        }

    }

    public class HeirarchyModel
    {
        public HeirarchyModel()
        {
            SampleIatiActivity = new iatiactivity();
        }
        public iatiactivity SampleIatiActivity { get; set; }
        public decimal H1Percent { get; set; }
        public decimal H2Percent { get; set; }
        public int SelectedHierarchy { get; set; }
    }
    public class FilterBDModel
    {
        public List<iatiactivity> iatiActivities { get; set; }
    }
    public class ProjectMapModel
    {
        public object selected { get; set; }

        public List<MatchedProject> MatchedProjects { get; set; }
        public List<iatiactivity> IatiActivitiesNotInAims { get; set; }
        public List<iatiactivity> AimsProjectsNotInIati { get; set; }
        public List<iatiactivity> NewProjectsToAddInAims { get; set; }
        public List<iatiactivity> ProjectsOwnedByOther { get; set; }
        public class MatchedProject
        {
            public iatiactivity iatiActivity { get; set; }
            public iatiactivity aimsProjects { get; set; }
        }
    }

}


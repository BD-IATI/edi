using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_BD_IATI.Library;

namespace AIMS_BD_IATI.DAL
{
    public class AimsDAL
    {
        AIMS_DBEntities dbContext = new AIMS_DBEntities();

        public List<tblFundSource> getFundSources()
        {
            var fundSources = from fundSource in dbContext.tblFundSources
                              where fundSource.IATICode != null && !string.IsNullOrEmpty(fundSource.IATICode)
                              select fundSource;

            return fundSources.ToList();
        }

        public List<DropdownItem> getFundSourcesDropdownData()
        {
            var fundSources = from fundSource in dbContext.tblFundSources
                              where fundSource.IATICode != null && !string.IsNullOrEmpty(fundSource.IATICode)
                              select new DropdownItem { 
                                ID = fundSource.IATICode,
                                Name = fundSource.FundSourceName
                              };

            return fundSources.ToList();
        }
        public List<tblProjectInfo> getProjects(string reportingOrg)
        {

            var projects = from project in dbContext.tblProjectInfoes.Include("tblFundSources")
                           join fundSource in dbContext.tblFundSources on project.FundSourceId equals fundSource.Id
                           //&& fundSource.IATICode equals fun
                           where fundSource.IATICode == reportingOrg

                           select project
                           ;

            return projects.ToList();
        }

        public List<iatiactivity> getAIMSDataInIATIFormat(string reportingOrg)
        {

            var projects = (from project in dbContext.tblProjectInfoes
                            join fundSource in dbContext.tblFundSources on project.FundSourceId equals fundSource.Id
                            where fundSource.IATICode == reportingOrg
                            select project);

            List<iatiactivity> iatiactivities = new List<iatiactivity>();

            foreach (var project in projects)
            {
                var iatiActivity = new iatiactivity();

                iatiActivity.iatiidentifier = new iatiidentifier { Value = getIdentifer(project) };

                iatiActivity.title = new textRequiredType { narrative = getNarativeArray(project.Title) };

                iatiActivity.activitydate = new activitydate[4];
                iatiActivity.activitydate[0] = new activitydate { type = "1", isodate = project.PlannedProjectStartDate ?? default(DateTime) };
                iatiActivity.activitydate[1] = new activitydate { type = "2", isodate = project.ActualProjectStartDate ?? default(DateTime) };
                iatiActivity.activitydate[2] = new activitydate { type = "3", isodate = project.PlannedProjectCompletionDate ?? default(DateTime) };
                iatiActivity.activitydate[3] = new activitydate { type = "4", isodate = project.RevisedProjectCompletionDate ?? default(DateTime) };

                iatiActivity.description = new iatiactivityDescription[1] { new iatiactivityDescription { narrative = getNarativeArray(project.Objective) } };

                iatiActivity.defaultaidtype = new defaultaidtype { code = project.tblAssistanceType.N().IATICode };

                iatiActivity.reportingorg = new reportingorg
                {
                    narrative = getNarativeArray(project.tblFundSource.N().FundSourceName),
                    @ref = project.tblFundSource.N().IATICode,
                    type = project.tblFundSource.N().tblFundSourceCategory.N().IATICode
                };


                iatiActivity.participatingorg = new participatingorg[3];

                iatiActivity.participatingorg[0] = new participatingorg
                {
                    narrative = getNarativeArray(project.tblFundSource.N().FundSourceGroup),
                    role = "1",
                    @ref = project.tblFundSource.N().IATICode,
                    type = "10"
                };
                iatiActivity.participatingorg[1] = new participatingorg
                {
                    narrative = getNarativeArray(project.tblFundSource.N().FundSourceName),
                    role = "3",
                    @ref = project.tblFundSource.N().IATICode,
                    type = "10"
                };
                //ToDo
                //iatiActivity.participatingorg[2] = new participatingorg
                //{
                //    narrative = getNarativeArray(project.tblFundSource.FundSourceName),
                //    role = "1",
                //    @ref = project.tblFundSource.IATICode,
                //    type = "10"
                //};

                iatiActivity.recipientcountry = new recipientcountry[1];
                iatiActivity.recipientcountry[0] = new recipientcountry { code = "BD", narrative = getNarativeArray("Bangladesh") };

                iatiactivities.Add(iatiActivity);
            }

            return iatiactivities;
        }

        private string getIdentifer(tblProjectInfo project)
        {
            return string.IsNullOrWhiteSpace(project.IatiIdentifier) ?
                project.DPProjectNo.StartsWith(project.tblFundSource.N().IATICode) ? project.DPProjectNo : project.tblFundSource.N().IATICode + project.DPProjectNo
                : project.IatiIdentifier;
        }

        public narrative[] getNarativeArray(string val)
        {
            return new narrative[1] { new narrative { lang = "en", Value = val } };
        }

    }


}

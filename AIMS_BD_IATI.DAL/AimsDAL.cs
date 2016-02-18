using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;

namespace AIMS_BD_IATI.DAL
{
    public static class AimsDAL
    {
        static AIMS_DBEntities dbContext = new AIMS_DBEntities();


        public static List<tblProjectInfo> getProjects(string reportingOrg)
        {
            var projects = from project in dbContext.tblProjectInfoes.Include("tblFundSources")
                           join fundSource in dbContext.tblFundSources on project.FundSourceId equals fundSource.Id
                           //&& fundSource.IATICode equals fun
                           where fundSource.IATICode == reportingOrg

                           select project
                           ;

            return projects.ToList();
        }

        public static List<iatiactivity> getAIMSDataInIATIFormat(string organizationId)
        {

            var projects = (from project in dbContext.tblProjectInfoes
                            join fundSource in dbContext.tblFundSources on project.FundSourceId equals fundSource.Id
                            where fundSource.IATICode == organizationId
                            select project);

            List<iatiactivity> iatiactivities = new List<iatiactivity>();

            foreach (var project in projects)
            {
                var iatiActivity = new iatiactivity();

                iatiActivity.iatiidentifier = new iatiidentifier { Value = getIdentifer(project) };

                iatiActivity.title = new textRequiredType { narrative = getNarativeArray(project.Title) };

                iatiActivity.activitydate = new activitydate[4];
                iatiActivity.activitydate[0] = new activitydate { type = "1", isodate = project.PlannedProjectStartDate.Value };
                iatiActivity.activitydate[1] = new activitydate { type = "2", isodate = project.ActualProjectStartDate.Value };
                iatiActivity.activitydate[1] = new activitydate { type = "3", isodate = project.PlannedProjectCompletionDate.Value };
                iatiActivity.activitydate[1] = new activitydate { type = "4", isodate = project.RevisedProjectCompletionDate.Value };

                iatiActivity.description = new iatiactivityDescription[1] { new iatiactivityDescription { narrative = getNarativeArray(project.Objective) } };

                iatiActivity.defaultaidtype = new defaultaidtype { code = project.tblAssistanceType.IATICode };

                iatiActivity.reportingorg = new reportingorg
                {
                    narrative = getNarativeArray(project.tblFundSource.FundSourceName),
                    @ref = project.tblFundSource.IATICode,
                    type = project.tblFundSource.tblFundSourceCategory.IATICode
                };


                iatiActivity.participatingorg = new participatingorg[3];

                iatiActivity.participatingorg[0] = new participatingorg
                {
                    narrative = getNarativeArray(project.tblFundSource.FundSourceGroup),
                    role = "1",
                    @ref = project.tblFundSource.IATICode,
                    type = "10"
                };
                iatiActivity.participatingorg[1] = new participatingorg
                {
                    narrative = getNarativeArray(project.tblFundSource.FundSourceName),
                    role = "3",
                    @ref = project.tblFundSource.IATICode,
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
                iatiActivity.recipientcountry[0] = new recipientcountry { code="BD", narrative = getNarativeArray("Bangladesh") };

                iatiactivities.Add(iatiActivity);
            }

            return iatiactivities;
        }

        private static string getIdentifer(tblProjectInfo project)
        {
            return string.IsNullOrWhiteSpace(project.IatiIdentifier) ?
                project.DPProjectNo.StartsWith(project.tblFundSource.IATICode) ? project.DPProjectNo : project.tblFundSource.IATICode + project.DPProjectNo
                : project.IatiIdentifier;
        }

        public static narrative[] getNarativeArray(string val)
        {
            return new narrative[1] { new narrative { lang = "en", Value = val } };
        }

        

    }

}

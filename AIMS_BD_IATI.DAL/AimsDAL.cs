using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
    }
}

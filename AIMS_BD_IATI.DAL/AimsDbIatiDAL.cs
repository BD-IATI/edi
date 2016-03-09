using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AIMS_BD_IATI.DAL
{
    public class AimsDbIatiDAL
    {
        AIMS_DB_IATIEntities dbContext = new AIMS_DB_IATIEntities();

        public int SaveAtivity(List<Activity> activities)
        {
            foreach (var activity in activities)
            {
                var a = dbContext.Activities.FirstOrDefault(x => x.IatiIdentifier == activity.IatiIdentifier);
                if (a != null)
                {
                    a.OrgId = activity.OrgId;
                    a.IatiIdentifier = activity.IatiIdentifier;

                    a.IatiActivityPrev = a.IatiActivity;
                    a.IatiActivity = activity.IatiActivity;

                    a.Hierarchy = activity.Hierarchy;
                    a.ParentHierarchy = activity.ParentHierarchy;

                    a.ImplementingOrgId = activity.ImplementingOrgId;

                    a.DownloadDatePrev = a.DownloadDate;
                    a.DownloadDate = DateTime.Now;
                }
                else
                {
                    dbContext.Activities.Add(activity);
                }

            }

            return dbContext.SaveChanges();
        }

        public iatiactivityContainer GetActivities(string dp)
        {
            var q = from a in dbContext.Activities
                    where a.OrgId == dp
                    orderby a.IatiIdentifier
                    select a.IatiActivity;

            var result = new List<iatiactivity>();
            var activity = new iatiactivity();
            var serializer = new XmlSerializer(typeof(iatiactivity));

            foreach (var a in q)
            {
                using (TextReader reader = new StringReader(a))
                {
                    activity = (iatiactivity)serializer.Deserialize(reader);

                }
                result.Add(activity);
            }


            return new iatiactivityContainer
            {
                DP = dp,
                iatiActivities = result,
                AimsProjects = new AimsDAL().getAIMSDataInIATIFormat(dp)
            };
        }



    }
}

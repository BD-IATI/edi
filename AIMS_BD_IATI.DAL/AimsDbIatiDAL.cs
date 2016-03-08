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
                var a = dbContext.Activities.FirstOrDefault(x => x.IATI_Identifier == activity.IATI_Identifier);
                if (a != null)
                {
                    a.Organization_Id = activity.Organization_Id;
                    a.IATI_Identifier = activity.IATI_Identifier;
                    a.Last_Downloaded = DateTime.Now;
                    a.Previous_Downloaded = a.Last_Downloaded;
                    a.Last_XML = activity.Last_XML;
                    a.Previous_XML = a.Last_XML;
                    a.Hierarchy = activity.Hierarchy;
                    a.Parent_Hierarchy = activity.Parent_Hierarchy;
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
                    where a.Organization_Id == dp
                    orderby a.IATI_Identifier
                    select a;

            var result = new List<iatiactivity>();
            var activity = new iatiactivity();
            var serializer = new XmlSerializer(typeof(iatiactivity));

            foreach (var a in q)
            {
                using (TextReader reader = new StringReader(a.Last_XML))
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

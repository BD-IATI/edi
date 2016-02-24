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
                    a.strLast_XML = activity.strLast_XML;
                    a.strPrevious_XML = a.strLast_XML;
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
                using (TextReader reader = new StringReader(a.strLast_XML))
                {
                    activity = (iatiactivity)serializer.Deserialize(reader);

                }
                result.Add(activity);
            }

            var parentActivities = result.FindAll(x => x.hierarchy == 1);
            var returnResult = new iatiactivityContainer();
            foreach (var pa in parentActivities)
            {
                if (pa.relatedactivity != null)
                {
                    pa.SelectedHierarchy = "Hierarchy1";
                    foreach (var ra in pa.relatedactivity.Where(r=>r.type == "2"))
                    {
                        //load related activities
                        var ha = result.Find(f => f.iatiidentifier.Value == ra.@ref);

                        if (ha != null)
                        {
                            pa.relatedIatiActivities.Add(ha);
                        }
                    }
                }


            }
            returnResult.iatiActivities = parentActivities;

            return returnResult;
        }


    }

    public class ProjectHierachy
    {
        public string Organization_Id { get; set; }
        public string IATI_Identifier { get; set; }
        public Nullable<System.DateTime> Last_Downloaded { get; set; }
        public Nullable<System.DateTime> Previous_Downloaded { get; set; }

    }
}

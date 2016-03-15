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

        public int SaveAtivities(List<Activity> activities)
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
                AimsProjects = new AimsDAL().GetAIMSDataInIATIFormat(dp)
            };
        }

        public int SaveFieldMappingPreferenceGeneral(List<FieldMappingPreferenceGeneral> fieldMaps)
        {
            foreach (var fieldMap in fieldMaps)
            {
                var a = dbContext.FieldMappingPreferenceGenerals.FirstOrDefault(x => x.OrgId == fieldMap.OrgId 
                                                                                    && x.FundSourceId == fieldMap.FundSourceId 
                                                                                    && x.FieldName == fieldMap.FieldName);
                if (a != null)
                {
                    a.OrgId = fieldMap.OrgId;
                    a.FundSourceId = fieldMap.FundSourceId;
                    a.FieldName = fieldMap.FieldName;
                    a.IsSourceIATI = fieldMap.IsSourceIATI;
                }
                else
                {
                    dbContext.FieldMappingPreferenceGenerals.Add(fieldMap);
                }

            }

            return dbContext.SaveChanges();
        }


        public int SaveFieldMappingPreferenceActivity(List<FieldMappingPreferenceActivity> fieldMaps)
        {
            foreach (var fieldMap in fieldMaps)
            {
                var a = dbContext.FieldMappingPreferenceActivities.FirstOrDefault(x => x.IATIIdentifier == fieldMap.IATIIdentifier
                                                                                    //&& x.ProjectId == fieldMap.ProjectId
                                                                                    && x.FieldName == fieldMap.FieldName);
                if (a != null)
                {
                    a.IATIIdentifier = fieldMap.IATIIdentifier;
                    a.ProjectId = fieldMap.ProjectId;
                    a.FieldName = fieldMap.FieldName;
                    a.IsSourceIATI = fieldMap.IsSourceIATI;
                }
                else
                {
                    dbContext.FieldMappingPreferenceActivities.Add(fieldMap);
                }

            }

            return dbContext.SaveChanges();
        }

        public List<FieldMappingPreferenceGeneral> GetFieldMappingPreferenceGeneral()
        {
            var q = (from fieldMap in dbContext.FieldMappingPreferenceGenerals
                     select fieldMap).ToList();

            return q;
        }

        public List<FieldMappingPreferenceActivity> GetFieldMappingPreferenceActivity(string iatiIdentifier)
        {
            var q = (from fieldMap in dbContext.FieldMappingPreferenceActivities
                     where fieldMap.IATIIdentifier == iatiIdentifier
                     select fieldMap).ToList();

            return q;
        }


    }
}

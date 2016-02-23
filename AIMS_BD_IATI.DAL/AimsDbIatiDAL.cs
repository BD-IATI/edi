﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public object GetProjects(string dp)
        {
            var q = from a in dbContext.Activities
                    where a.Organization_Id == dp
                    select new ProjectHierachy
                    {
                        Organization_Id = a.Organization_Id,
                        IATI_Identifier = a.IATI_Identifier,
                        Last_Downloaded = a.Last_Downloaded,
                        Previous_Downloaded = a.Last_Downloaded,
                        //Hierarchy = a.Hierarchy,
                        //Parent_Hierarchy = a.Parent_Hierarchy
                    };

            return q.ToList();

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

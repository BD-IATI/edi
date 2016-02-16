using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//XML
using System.Xml;
using System.Xml.Serialization;
//Json
//using Newtonsoft.Json;

using AIMS_BD_IATI.Library.Parser.ParserIATIv1;

namespace AIMS_BD_IATI.Library.Parser.ParserIATIv2
{
    public class ConvertIATIv2 : IConverterIATI
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ConvertIATIv2()
        {

        }

        public IXmlResult ConvertIATIXML(IXmlResult obj)
        {
            IXmlResult xmlResult;

            xmlResult = (XmlResultv2)obj;

            return xmlResult;
        }

        /// <summary>
        /// Convert IATI 
        /// </summary>
        /// <returns></returns>
        //public IXmlResult ConvertIATIXML(IXmlResult objSource, IXmlResult objDestinaiton)
        //{
        //    if(objDestinaiton == null)
        //        objDestinaiton = new XmlResultv2();

        //    //parse and assign

        //    return objDestinaiton;
        //}

        public XmlResultv2 ConvertIATI105to201XML(XmlResultv1 objSource, XmlResultv2 objDestinaiton)
        {
            if (objDestinaiton == null)
                objDestinaiton = new XmlResultv2();

            //iatiactivities
            if (objSource != null && objSource.iatiactivities != null && objSource.iatiactivities.Items != null)
            {
                //activity
                foreach (var item in objSource.iatiactivities.Items)
                {
                    if (item.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.iatiactivity))
                    {
                        var activity = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.iatiactivity)item;
                        string srcIatiidentifier = "";

                        if (activity.Items != null)
                        {
                            foreach (var activityItem in activity.Items)
                            {
                                //iati-identifier
                                if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.iatiidentifier))
                                {
                                    var iatiidentifier = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.iatiidentifier)activityItem;
                                    srcIatiidentifier = iatiidentifier.Text.FirstOrDefault();
                                }
                            }

                            var desActivity = objDestinaiton.iatiactivities.iatiactivity.FirstOrDefault(q => q.iatiidentifier.Value == srcIatiidentifier);


                            foreach (var activityItem in activity.Items)
                            {
                                //reporting-org
                                if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.reportingorg))
                                {
                                    var reportingorg = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.reportingorg)activityItem;

                                    narrative[] arrynarrative = getNarrativeArray(reportingorg);

                                    desActivity.reportingorg.narrative = arrynarrative;
                                }
                                //title
                                else if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.textType))
                                {
                                    var title = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.textType)activityItem;

                                    narrative[] arrynarrative = getNarrativeArray(title);

                                    desActivity.title.narrative = arrynarrative;
                                }
                                //description
                                else if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.description))
                                {
                                    var description = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.description)activityItem;

                                    narrative[] arrynarrative = getNarrativeArray(description);

                                    desActivity.description = new iatiactivityDescription[1];
                                    desActivity.description[0].narrative = arrynarrative;
                                }
                                //participating-org
                                else if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.participatingorg))
                                {
                                    var participatingorg = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.participatingorg)activityItem;

                                    narrative[] arrynarrative = getNarrativeArray(participatingorg);

                                    var targetParticipatingOrg = desActivity.participatingorg.FirstOrDefault(x => x.role == participatingorg.role
                                                                                                                && x.@ref == participatingorg.@ref
                                                                                                                && x.type == participatingorg.type);
                                    targetParticipatingOrg.role = getOrgRoleCode(participatingorg.role);
                                    targetParticipatingOrg.narrative = arrynarrative;
                                }
                            }
                        }
                    }
                }
            }


            //parse and assign

            return objDestinaiton;
        }

        private static narrative[] getNarrativeArray(dynamic activityItem)
        {
            narrative narrative = new narrative();
            narrative.lang = "en";
            var narrative_value = activityItem.Any[0];
            narrative.Value = narrative_value != null ? narrative_value.InnerText : "";

            narrative[] narrativeArray = new narrative[1];
            narrativeArray[0] = narrative;

            return narrativeArray;
        }
        private static string getOrgRoleCode(string roleName)
        {
            if (roleName.ToLower() == "funding")
                return "1";
            else if (roleName.ToLower() == "accountable")
                return "2";
            else if (roleName.ToLower() == "extending")
                return "3";
            else if (roleName.ToLower() == "implementing")
                return "4";

            return "";
        }

    }

    public enum OrganizationRole
    {

        Funding = 1,
        Accountable = 2,
        Extending = 3,
        Implementing = 4
    }
}

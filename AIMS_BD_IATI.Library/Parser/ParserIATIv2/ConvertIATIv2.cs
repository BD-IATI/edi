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
                objSource.iatiactivities.version = (decimal)2.01;
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
                            desActivity.AnyAttr[0].Prefix = "";
                            desActivity.AnyAttr[0].Value = "2.01";

                            int otherIdentifierCounter = 0;
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
                                    desActivity.description[0] = new iatiactivityDescription();
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

                                //recipient-country
                                //Same

                                //activity-status
                                //Same

                                //activity-date
                                else if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.activitydate))
                                {
                                    var activitydate = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.activitydate)activityItem;

                                    var targetActivitydate = desActivity.activitydate.FirstOrDefault(x => x.type == activitydate.type);
                                    targetActivitydate.type = getActivityDateCode(activitydate.type);
                                }
                                //contact-info
                                else if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.contactinfo))
                                {
                                    var contactinfo = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.contactinfo)activityItem;
                                    if (desActivity.contactinfo == null) desActivity.contactinfo = new contactinfo[1];
                                    if (desActivity.contactinfo[0] == null) desActivity.contactinfo[0] = new contactinfo();
                                    var desContactInfo = desActivity.contactinfo;

                                    foreach (var it in contactinfo.Items)
                                    {
                                        //organisation
                                        if (it.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.textType)) //[textType has multiple]
                                        {
                                            var org = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.textType)it;

                                            narrative[] arrynarrative2 = getNarrativeArray(org);

                                            desActivity.contactinfo[0].organisation = new textRequiredType();

                                            desActivity.contactinfo[0].organisation.narrative = arrynarrative2;
                                        }
                                        //mailingaddress
                                        if (it.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.contactinfoMailingaddress))
                                        {
                                            var addr = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.contactinfoMailingaddress)it;

                                            narrative[] arrynarrative2 = getNarrativeArray2(addr);

                                            desActivity.contactinfo[0].mailingaddress = new textRequiredType[1];
                                            desActivity.contactinfo[0].mailingaddress[0] = new textRequiredType();

                                            desActivity.contactinfo[0].mailingaddress[0].narrative = arrynarrative2;
                                        }
                                    }

                                }
                                //location
                                //ToDo

                                //sector
                                //same

                                //policy-marker
                                //same

                                //collaboration-type
                                //same

                                //default-finance-type
                                //same

                                //budget
                                else if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.budget))
                                {
                                    var budget = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.budget)activityItem;

                                    foreach (var b in desActivity.budget)
                                    {
                                        b.type = budget.type == "Original" ? "1" : "2";
                                    }

                                }

                                //planned-disbursement
                                //not in 1.05

                                //transaction
                                else if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.transaction))
                                {
                                    var transaction = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.transaction)activityItem;


                                    var targettransaction = desActivity.transaction.FirstOrDefault(x => x.transactiontype.code == transaction.transactiontype.code);
                                    targettransaction.transactiontype.code = gettransactionCode(transaction.transactiontype.code);

                                    //------------------


                                }
                                //document-link
                                else if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.documentlink))
                                {
                                    var documentlink = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.documentlink)activityItem;

                                    var d = documentlink.Items.FirstOrDefault(x => x.GetType() == typeof(textType));

                                    if (d != null)
                                    {
                                        narrative[] arrynarrative = getNarrativeArray((textType)d);

                                        var targetdocumentlink = desActivity.documentlink.FirstOrDefault(x => x.url == documentlink.url);

                                        targetdocumentlink.title = new textRequiredType();
                                        targetdocumentlink.title.narrative = arrynarrative;
                                    }

                                }
                                //conditions 
                                //Not in 1.05

                                //result 
                                //Not in 1.05


                                //other-identifier
                                else if (activityItem.GetType() == typeof(AIMS_BD_IATI.Library.Parser.ParserIATIv1.otheridentifier))
                                {

                                    var otheridentifier = (AIMS_BD_IATI.Library.Parser.ParserIATIv1.otheridentifier)activityItem;

                                    narrative[] arrynarrative = getNarrativeArrayStr(otheridentifier.ownername);

                                    var targetotheridentifier = desActivity.otheridentifier[otherIdentifierCounter];

                                    targetotheridentifier.@ref = otheridentifier.Text.FirstOrDefault();
                                    targetotheridentifier.type = "A1";

                                    targetotheridentifier.ownerorg = new otheridentifierOwnerorg();
                                    targetotheridentifier.ownerorg.@ref = otheridentifier.ownerref;
                                    targetotheridentifier.ownerorg.narrative = arrynarrative;
                                    targetotheridentifier.AnyAttr = null;
                                    otherIdentifierCounter++;
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
        private static narrative[] getNarrativeArray2(dynamic activityItem)
        {
            narrative narrative = new narrative();
            narrative.lang = "en";
            var narrative_value = activityItem.Text[0];
            narrative.Value = narrative_value; //!= null ? narrative_value.InnerText : "";

            narrative[] narrativeArray = new narrative[1];
            narrativeArray[0] = narrative;

            return narrativeArray;
        }
        private static narrative[] getNarrativeArrayStr(string ttext)
        {
            narrative narrative = new narrative();
            narrative.lang = "en";
            narrative.Value = ttext;

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
        private static string getActivityDateCode(string name)
        {
            //http://iatistandard.org/201/codelists/ActivityDateType/
            //1	Planned start
            //2	Actual start
            //3	Planned End	
            //4	Actual end	

            if (name.ToLower() == "start-planned")
                return "1";
            else if (name.ToLower() == "start-actual")
                return "2";
            else if (name.ToLower() == "end-planned")
                return "3";
            else if (name.ToLower() == "end-actual")
                return "4";

            return "";


        }
        private static string gettransactionCode(string name)
        {
            //http://iatistandard.org/205/codelists/TransactionType/
            //2 C	    Commitment	        
            //3 D	    Disbursement	    
            //4 E	    Expenditure	        
            //1 IF	Incoming Funds   
            //5 IR	Interest Repayment	
            //6 LR	Loan Repayment	    
            //7 R	    Reimbursement	    
            //8 QP	Purchase of Equity	
            //9 QS	Sale of Equity	    
            //10 CG	Credit Guarantee	

            if (name == "C")
                return "2";
            else if (name == "D")
                return "3";
            else if (name == "E")
                return "4";
            else if (name == "IF")
                return "1";
            else if (name == "IR")
                return "5";
            else if (name == "LR")
                return "6";
            else if (name == "R")
                return "7";
            else if (name == "QP")
                return "8";
            else if (name == "QS")
                return "9";
            else if (name == "CG")
                return "10";

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

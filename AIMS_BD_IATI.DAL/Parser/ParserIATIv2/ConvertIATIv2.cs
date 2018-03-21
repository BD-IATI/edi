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
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser;
using v1 = AIMS_BD_IATI.Library.Parser.ParserIATIv1;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using v2 = AIMS_BD_IATI.Library.Parser.ParserIATIv2;

namespace AIMS_BD_IATI
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

        public XmlResultv2 ConvertIATI105to201XML(v1.XmlResultv1 objSource, XmlResultv2 objDestinaiton)
        {
            if (objDestinaiton == null)
                objDestinaiton = new XmlResultv2();

            //iatiactivities
            if (objSource != null && objSource.iatiactivities != null && objSource.iatiactivities.Items != null)
            {
                objSource.iatiactivities.version = (decimal)2.02;
                //activity
                foreach (var item in objSource.iatiactivities.Items)
                {

                    if (item.GetType() == typeof(v1.iatiactivity))
                    {
                        var activity = (v1.iatiactivity)item;

                        string srcIatiidentifier = "";

                        if (activity.Items != null)
                        {
                            foreach (var activityItem in activity.Items)
                            {
                                //iati-identifier
                                if (activityItem.GetType() == typeof(v1.iatiidentifier))
                                {
                                    var iatiidentifier = (v1.iatiidentifier)activityItem;
                                    srcIatiidentifier = iatiidentifier.Text.n(0);
                                }
                            }

                            var desActivity = objDestinaiton.iatiactivities.iatiactivity.FirstOrDefault(q => q.IatiIdentifier == srcIatiidentifier);
                            //desActivity.AnyAttr[0].Value = "2.02";
                            desActivity.location = new List<v2.location>();
                            desActivity.result = new List<v2.result>();

                            int otherIdentifierCounter = 0;
                            foreach (var activityItem in activity.Items)
                            {
                                #region reporting-org
                                if (activityItem.GetType() == typeof(v1.reportingorg))
                                {
                                    var reportingorg = (v1.reportingorg)activityItem;

                                    List<narrative> arrynarrative = getNarrativeList(reportingorg);

                                    desActivity.reportingorg.narrative = arrynarrative;
                                }
                                #endregion

                                #region title
                                else if (activityItem.GetType() == typeof(v1.textType))
                                {
                                    var title = (v1.textType)activityItem;

                                    List<narrative> arrynarrative = getNarrativeList(title);

                                    desActivity.title.narrative = arrynarrative;
                                }
                                #endregion

                                #region description
                                else if (activityItem.GetType() == typeof(v1.description))
                                {
                                    var description = (v1.description)activityItem;

                                    List<narrative> arrynarrative = getNarrativeList(description);

                                    desActivity.description = new List<iatiactivityDescription> { new iatiactivityDescription { narrative = arrynarrative } };
                                }
                                #endregion

                                #region participating-org
                                else if (activityItem.GetType() == typeof(v1.participatingorg))
                                {
                                    var participatingorg = (v1.participatingorg)activityItem;

                                    List<narrative> arrynarrative = getNarrativeList(participatingorg);

                                    var targetParticipatingOrg = desActivity.participatingorg.FirstOrDefault(x => x.role == participatingorg.role
                                                                                                                && x.@ref == participatingorg.@ref
                                                                                                                && x.type == participatingorg.type);
                                    targetParticipatingOrg.role = getOrgRoleCode(participatingorg.role);
                                    targetParticipatingOrg.narrative = arrynarrative;
                                }
                                #endregion

                                //recipient-country
                                //Same

                                //activity-status
                                //Same

                                #region activity-date
                                else if (activityItem.GetType() == typeof(v1.activitydate))
                                {
                                    var activitydate = (v1.activitydate)activityItem;

                                    var targetActivitydate = desActivity.activitydate.FirstOrDefault(x => x.type == activitydate.type);
                                    targetActivitydate.type = getActivityDateCode(activitydate.type);
                                }
                                #endregion

                                #region contact-info
                                else if (activityItem.GetType() == typeof(v1.contactinfo))
                                {
                                    var contactinfo = (v1.contactinfo)activityItem;
                                    if (desActivity.contactinfo == null) desActivity.contactinfo = new List<v2.contactinfo>();
                                    if (desActivity.contactinfo[0] == null) desActivity.contactinfo.Add(new contactinfo());
                                    var desContactInfo = desActivity.contactinfo;

                                    foreach (var it in contactinfo.Items)
                                    {
                                        //organisation
                                        if (it.GetType() == typeof(v1.textType)) //[textType has multiple]
                                        {
                                            var org = (v1.textType)it;

                                            List<narrative> arrynarrative2 = getNarrativeList(org);

                                            desActivity.contactinfo[0].organisation = new textRequiredType();

                                            desActivity.contactinfo[0].organisation.narrative = arrynarrative2;
                                        }
                                        //mailingaddress
                                        if (it.GetType() == typeof(v1.contactinfoMailingaddress))
                                        {
                                            var addr = (v1.contactinfoMailingaddress)it;

                                            List<narrative> arrynarrative2 = getNarrativeList2(addr);

                                            desActivity.contactinfo[0].mailingaddress = new List<textRequiredType> { new textRequiredType { narrative = arrynarrative2 } };
                                        }
                                    }

                                }
                                #endregion

                                #region location
                                else if (activityItem.GetType() == typeof(v1.location))
                                {
                                    var location = (v1.location)activityItem;

                                    var locationV2 = new location();

                                    foreach (var it in location.Items)
                                    {
                                        if (it.GetType() == typeof(v1.locationCoordinates))
                                        {
                                            var coordinate = (v1.locationCoordinates)it;
                                            locationV2.point = new locationPoint { pos = coordinate.latitude + " " + coordinate.longitude };
                                        }

                                        else if (it.GetType() == typeof(v1.locationPoint))
                                        {
                                            var point = (v1.locationPoint)it;
                                            locationV2.point = new locationPoint { pos = point.Items.n(0).ToString() };
                                        }

                                        else if (it.GetType() == typeof(v1.locationAdministrative))
                                        {
                                            var adm = (v1.locationAdministrative)it;
                                            var adm2 = new locationAdministrative { vocabulary = adm.vocabulary, level = adm.level, code = adm.code };
                                            if (locationV2.administrative == null) locationV2.administrative = new List<locationAdministrative> { adm2 };
                                            else locationV2.administrative.Add(adm2);
 
                                        }
                                    }

                                    desActivity.location.Add(locationV2);
                                }
                                #endregion

                                //sector
                                //same

                                //policy-marker
                                //same

                                //collaboration-type
                                //same

                                //default-finance-type
                                //same

                                #region budget
                                else if (activityItem.GetType() == typeof(v1.budget))
                                {
                                    var budget = (v1.budget)activityItem;

                                    foreach (var b in desActivity.budget)
                                    {
                                        b.type = budget.type == "Original" ? "1" : "2";
                                    }

                                }
                                #endregion

                                //planned-disbursement
                                //not in 1.05

                                #region transaction

                                else if (activityItem.GetType() == typeof(v1.transaction))
                                {
                                    var transaction = (v1.transaction)activityItem;


                                    var targettransaction = desActivity.transaction.FirstOrDefault(x => x.transactiontype.code == transaction.transactiontype.code);
                                    targettransaction.transactiontype.code = gettransactionCode(transaction.transactiontype.code);

                                    //------------------


                                }
                                #endregion

                                #region document - link
                                else if (activityItem.GetType() == typeof(v1.documentlink))
                                {
                                    var documentlink = (v1.documentlink)activityItem;

                                    var d = documentlink.Items.FirstOrDefault(x => x.GetType() == typeof(v1.textType));

                                    if (d != null)
                                    {
                                        List<narrative> arrynarrative = getNarrativeList((v1.textType)d);

                                        var targetdocumentlink = desActivity.documentlink.FirstOrDefault(x => x.url == documentlink.url);

                                        targetdocumentlink.title = new textRequiredType();
                                        targetdocumentlink.title.narrative = arrynarrative;
                                    }

                                }
                                #endregion
                                //conditions 
                                //Not in 1.05

                                //result 
                                #region result
                                else if (activityItem.GetType() == typeof(v1.result))
                                {
                                    var result = (v1.result)activityItem;

                                    var resultV2 = new result();
                                    resultV2.indicator = new List<resultIndicator>();

                                    foreach (var resultItem in result.Items)
                                    {


                                        if (resultItem.GetType() == typeof(v1.textType))
                                        {
                                            var title = (v1.textType)resultItem;
                                            resultV2.title = new textRequiredType { narrative = getNarrativeList(resultItem) };
                                        }

                                        else if (resultItem.GetType() == typeof(v1.description))
                                        {
                                            var description = (v1.description)resultItem;
                                            resultV2.description = new description { narrative = getNarrativeList(resultItem) };
                                        }

                                        else if (resultItem.GetType() == typeof(v1.resultIndicator))
                                        {
                                            var indicator = (v1.resultIndicator)resultItem;

                                            var indicatorV2 = new resultIndicator();
                                            indicatorV2.period = new List<resultIndicatorPeriod>();

                                            foreach (var indicatorItem in indicator.Items)
                                            {

                                                if (indicatorItem.GetType() == typeof(v1.textType))
                                                {
                                                    var title = (v1.textType)indicatorItem;
                                                    indicatorV2.title = new textRequiredType { narrative = getNarrativeList(indicatorItem) };
                                                }
                                                else if (indicatorItem.GetType() == typeof(v1.description))
                                                {
                                                    var desc = (v1.description)indicatorItem;
                                                    indicatorV2.description = new description { narrative = getNarrativeList(indicatorItem) };
                                                }
                                                else if (indicatorItem.GetType() == typeof(v1.resultIndicatorBaseline))
                                                {
                                                    var baseline = (v1.resultIndicatorBaseline)indicatorItem;
                                                    indicatorV2.baseline = new resultIndicatorBaseline { year = baseline.year, value = baseline.value };
                                                }
                                                else if (indicatorItem.GetType() == typeof(v1.resultIndicatorPeriod))
                                                {
                                                    var period = (v1.resultIndicatorPeriod)indicatorItem;
                                                    var periodV2 = new resultIndicatorPeriod();

                                                    v1.dateType date1 = null;
                                                    v1.dateType date2 = null;
                                                    foreach (var periodItem in period.Items)
                                                    {

                                                        if (periodItem.GetType() == typeof(v1.dateType))
                                                        {
                                                            if (date1 == null)
                                                            {
                                                                date1 = (v1.dateType)periodItem;
                                                            }
                                                            else
                                                            {
                                                                date2 = (v1.dateType)periodItem;
                                                            }

                                                        }
                                                        else if (periodItem.GetType() == typeof(v1.resultIndicatorPeriodActual))
                                                        {
                                                            var periodActual = (v1.resultIndicatorPeriodActual)periodItem;
                                                            periodV2.actual = new resultIndicatorPeriodActual { value = periodActual.value };
                                                        }
                                                        else if (periodItem.GetType() == typeof(v1.resultIndicatorPeriodTarget))
                                                        {
                                                            var periodTarget = (v1.resultIndicatorPeriodTarget)periodItem;
                                                            periodV2.target = new resultIndicatorPeriodTarget { value = periodTarget.value };
                                                        }
                                                    }
                                                    periodV2.periodstart = new resultIndicatorPeriodPeriodstart { isodate = date1.isodate < date2.isodate ? date1.isodate : date2.isodate };
                                                    periodV2.periodend = new resultIndicatorPeriodPeriodend { isodate = date1.isodate > date2.isodate ? date1.isodate : date2.isodate };

                                                    indicatorV2.period.Add(periodV2);
                                                }

                                            }

                                            resultV2.indicator.Add(indicatorV2);

                                        }
                                    }

                                    desActivity.result.Add(resultV2);
                                }
                                #endregion


                                #region other-identifier
                                else if (activityItem.GetType() == typeof(v1.otheridentifier))
                                {

                                    var otheridentifier = (v1.otheridentifier)activityItem;

                                    List<narrative> arrynarrative = Statix.GetNarrativeList(otheridentifier.ownername);

                                    var targetotheridentifier = desActivity.otheridentifier[otherIdentifierCounter];

                                    targetotheridentifier.@ref = otheridentifier.Text.n(0);
                                    targetotheridentifier.type = "A1";

                                    targetotheridentifier.ownerorg = new otheridentifierOwnerorg();
                                    targetotheridentifier.ownerorg.@ref = otheridentifier.ownerref;
                                    targetotheridentifier.ownerorg.narrative = arrynarrative;
                                    targetotheridentifier.AnyAttr = null;
                                    otherIdentifierCounter++;
                                }
                                #endregion


                            }

                            
                        }
                    }
                }
            }


            //parse and assign

            return objDestinaiton;
        }

        public static narrative[] getNarrativeArray(dynamic activityItem)
        {
            if (activityItem == null)
                return null;

            narrative[] narrativeArray = new narrative[1];
            narrative narrative = new narrative();
            narrative.lang = "en";

            if (activityItem.Any != null)
            {
                var narrative_value = activityItem.Any[0];
                narrative.Value = narrative_value != null ? narrative_value.InnerText : "";

                narrativeArray[0] = narrative;
            }

            return narrativeArray;
        }
        public static narrative[] getNarrativeArray2(dynamic activityItem)
        {
            if (activityItem == null)
                return null;

            narrative[] narrativeArray = new narrative[1];
            narrative narrative = new narrative();
            narrative.lang = "en";

            if (activityItem.Text != null)
            {
                var narrative_value = activityItem.Text[0];
                narrative.Value = narrative_value; //!= null ? narrative_value.InnerText : "";


                narrativeArray[0] = narrative;
            }

            return narrativeArray;
        }

        public static List<narrative> getNarrativeList(dynamic activityItem)
        {
            if (activityItem == null)
                return null;

            List<narrative> narrativeList = new List<narrative>();
            narrative narrative = new narrative();
            narrative.lang = "en";

            if (activityItem.Any != null)
            {
                var narrative_value = activityItem.Any[0];
                narrative.Value = narrative_value != null ? narrative_value.InnerText : "";

                narrativeList.Add(narrative);
            }

            return narrativeList;
        }

        public static List<narrative> getNarrativeList2(dynamic activityItem)
        {
            if (activityItem == null)
                return null;

            List<narrative> narrativeList = new List<narrative> { };
            narrative narrative = new narrative();
            narrative.lang = "en";

            if (activityItem.Text != null)
            {
                var narrative_value = activityItem.Text[0];
                narrative.Value = narrative_value; //!= null ? narrative_value.InnerText : "";


                narrativeList.Add(narrative);
            }

            return narrativeList;
        }

        //public static narrative[] getNarrativeArrayStr(string ttext)
        //{
        //    narrative narrative = new narrative();
        //    narrative.lang = "en";
        //    narrative.Value = ttext;

        //    narrative[] narrativeArray = new narrative[1];
        //    narrativeArray[0] = narrative;

        //    return narrativeArray;
        //}
        public static string getOrgRoleCode(string roleName)
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
        public static string getActivityDateCode(string name)
        {
            //http://iatistandard.org/202/codelists/ActivityDateType/
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
        public static string gettransactionCode(string name)
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

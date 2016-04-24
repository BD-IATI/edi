using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using AIMS_BD_IATI.Library.Parser.ParserIATIv1;
using System.Net.Http.Formatting;

namespace AIMS_BD_IATI.WebAPI.Controllers
{
    [RoutePrefix("api/IATIConverter")]
    public class IATIConverterController : ApiController
    {
        /// <summary>
        /// Convert Data from v1.05 to v2.02
        /// </summary>
        /// <param name="org">Organization IATI Identifier, eg: CA-3</param>
        /// <param name="country">Country Prefix, eg: BD</param>
        /// <returns>IATI activity list in XML format</returns>
        [AcceptVerbs("GET", "POST")]
        public XmlResultv2 ConvertIATI(string org, string country)
        {
            string activitiesURL;
            IParserIATI parserIATI;
            XmlResultv2 returnResult2 = null;
            XmlResultv1 returnResult1 = null;

            try
            {
                activitiesURL = "http://datastore.iatistandard.org/api/1/access/activity.xml?recipient-country=" + country + "&reporting-org=" + org + "&stream=True";
                //Parser v2.01
                parserIATI = new ParserIATIv2();

                returnResult2 = (XmlResultv2)parserIATI.ParseIATIXML(activitiesURL);

                var iatiactivityArray = returnResult2.n().iatiactivities.n().iatiactivity;
                if (iatiactivityArray != null && iatiactivityArray.n()[0].AnyAttr.n()[0].Value.StartsWith("1.0"))
                {
                    //Parser v1.05
                    parserIATI = new ParserIATIv1();
                    returnResult1 = (XmlResultv1)parserIATI.ParseIATIXML(activitiesURL);

                    //Conversion
                    ConvertIATIv2 convertIATIv2 = new ConvertIATIv2();
                    returnResult2 = convertIATIv2.ConvertIATI105to201XML(returnResult1, returnResult2);
                }
            }
            catch (Exception ex)
            {
                returnResult2.n().Value = ex.Message;
            }

            return returnResult2;
            //return Newtonsoft.Json.JsonConvert.SerializeObject(returnResult2);
        }

        /// <summary>
        /// Convert AIMS Projects into IATI v2.x format
        /// </summary>
        /// <param name="org">Organization IATI Identifier of Managing DP, eg: CA-3</param>
        /// <returns>IATI activity list in XML format</returns>
        [AcceptVerbs("GET", "POST")]
        public AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivities ConvertAIMStoIATI(string org)
        {
            List<AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivity> iatiactivityList = new List<Library.Parser.ParserIATIv2.iatiactivity>();
            try
            {
                iatiactivityList = new AIMS_BD_IATI.DAL.AimsDAL().GetAIMSProjectsInIATIFormat(org);
            }
            catch (Exception)
            {
                
            }
            return new AIMS_BD_IATI.Library.Parser.ParserIATIv2.iatiactivities { iatiactivity = iatiactivityList.ToArray() };
        }
        
    }

}

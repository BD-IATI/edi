using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//XML
using System.Xml;
using System.Xml.Serialization;
//Json
using Newtonsoft.Json;

namespace AIMS_BD_IATI.Library.Parser.ParserIATIv1
{
    public class ParserIATIv1 : IParserIATI
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ParserIATIv1() 
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Implements ParseXML
        /// </summary>
        /// <returns></returns>
        public IXmlResult ParseIATIXML(string url)
        {
            IXmlResult xmlResult;

            var serializer = new XmlSerializer(typeof(XmlResultv1), new XmlRootAttribute("result"));

            // Create an XmlNamespaceManager to resolve namespaces.
            NameTable nameTable = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);
            nsmgr.AddNamespace("iati-extra", "");

            // Create an XmlParserContext.  The XmlParserContext contains all the information
            // required to parse the XML fragment, including the entity information and the
            // XmlNamespaceManager to use for namespace resolution.
            XmlParserContext xmlParserContext = new XmlParserContext(nameTable, nsmgr, null, XmlSpace.None);

            // Create the reader.
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.NameTable = nameTable;

            WebRequest request = WebRequest.Create(url);
            request.Timeout = 20 * 60 * 1000; //Timeout.Infinite;
            using (WebResponse response = request.GetResponse())
            {
                using (var reader = XmlReader.Create(response.GetResponseStream(), xmlReaderSettings, xmlParserContext))
                {
                    xmlResult = (XmlResultv1)serializer.Deserialize(reader);
                }
            }

            //using (var Reader = XmlReader.Create(url, xmlReaderSettings, xmlParserContext))
            //{
            //    xmlResult = (XmlResultv1)serializer.Deserialize(Reader);
            //}

            return xmlResult;
        }
    }
}

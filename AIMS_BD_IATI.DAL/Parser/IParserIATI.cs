using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.Library.Parser
{
    public interface IParserIATI
    {
        IXmlResult ParseIATIXML(string url);
    }
}

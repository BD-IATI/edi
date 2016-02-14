using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.Library.Parser.ParserIATIv1
{
    /// <summary>
    /// The "XmlResult" is wrapper calss of "iatiactivities" which was generated using following steps:
    /// 1. "iati-activities-schema.xsd" file was downloaded from "http://iatistandard.org/105/schema/downloads/iati-activities-schema.xsd"
    /// 2. "iati-common.xsd" file was downloaded from "http://iatistandard.org/105/schema/downloads/iati-common.xsd"
    /// 3. "iati-activities-schema.cs" was generated using following Visual Studio 2013 commad tool: 
    ///     "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\Shortcuts\Developer Command Prompt for VS2013"
    /// 4. Put the "iati-activities-schema.xsd" and "iati-common.xsd" file in the same directory like D:
    /// 5. Switch to D: directory in the VS2013 command tool=>D:[press enter], then run the following command to create classes with same name with cs extension.
    /// 6. D:\>xsd iati-activities-schema.xsd /classes
    /// 7. Include the "iati-activities-schema.cs" in the project
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class XmlResultv1 : IXmlResult
    {
        //private iatiactivities iatiactivitiesField;

        //private System.Xml.XmlAttribute[] anyAttrField;

        //private string valueField;
        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute("iati-activities")]
        //public iatiactivities iatiactivities
        //{
        //    get
        //    {
        //        return this.iatiactivitiesField;
        //    }
        //    set
        //    {
        //        this.iatiactivitiesField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlAnyAttributeAttribute()]
        //public System.Xml.XmlAttribute[] AnyAttr
        //{
        //    get
        //    {
        //        return this.anyAttrField;
        //    }
        //    set
        //    {
        //        this.anyAttrField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlTextAttribute()]
        //public string Value
        //{
        //    get
        //    {
        //        return this.valueField;
        //    }
        //    set
        //    {
        //        this.valueField = value;
        //    }
        //}
    }
}

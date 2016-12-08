using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.UnitTest
{
    [TestClass]
    public class SerializableTest
    {


    }



    public static class AttributeHelper
    {
        #region Static public methods

        #region GetAttribute

        static public T GetAttribute<T>(object obj)
            where T : Attribute
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            // If the object is a member info then we can use it, otherwise it's an instance of 'something' so get it's type...
            var member = (obj is System.Reflection.MemberInfo) ? (System.Reflection.MemberInfo)obj : obj.GetType();

            return GetAttributeImpl<T>(member);
        }

        #endregion GetAttribute

        #endregion Static public methods

        #region Static methods

        #region GetAttributeImpl

        static T GetAttributeImpl<T>(System.Reflection.MemberInfo member)
            where T : Attribute
        {
            var attribs = member.GetCustomAttributes(typeof(T), false);
            if (attribs == null || attribs.Length == 0)
                return null;

            return attribs[0] as T;
        }

        #endregion GetAttributeImpl

        #endregion Static methods
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
namespace AIMS_BD_IATI.UnitTest
{
    [TestClass]
    public class NullCheckerTest
    {
        [TestMethod]
        public void ReturnTheObjFromSpecifiedPositionOfAnArray()
        {
            string expected = "j";

            string[] array = new string[2] { "j", "k" };

            string actual = array.n(0);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReturnTheObjFromSpecifiedPositionOfAnArray2()
        {
            string expected = string.Empty;

            string[] array = null;

            string actual = array.n(0);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReturnTheObjFromSpecifiedPositionOfAnArray3()
        {

            iatiactivity[] array = null;

            iatiactivity obj = array.n(0);


            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void ReturnTheObjFromSpecifiedPositionOfAnArray4()
        {

            iatiactivity[] array = null;

            iatiactivity obj = array.n(0);


            Assert.IsNotNull(obj);
        }    
    }
}

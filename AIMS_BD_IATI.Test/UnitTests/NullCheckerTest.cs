using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AIMS_BD_IATI.Library;
using AIMS_BD_IATI.Library.Parser.ParserIATIv2;
using System.Collections.Generic;
namespace AIMS_BD_IATI.Test
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
        public void ReturnTheObjFromSpecifiedPositionOfAnArrayfd()
        {
            string expected = string.Empty;

            string[] array = new string[2] { "j", "k" };

            string actual = array.n(3);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void ReturnTheObjFromSpecifiedPositionOfAnArradyfd()
        {
            string expected = string.Empty;

            string[] array = null;

            string actual = array.n(3);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void ToPercentTest()
        {
            decimal val = 20;

            decimal total = 200;

            decimal actual = val.ToPercent(total);

            decimal expected = 10;
            Assert.AreEqual(expected, actual);
        }       
        
        [TestMethod]
        public void PercentOfTest()
        {
            decimal val = 10;

            decimal percentOf = 200;

            decimal actual = val.PercentOf(percentOf);

            decimal expected = 20;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PercentOfTest2()
        {
            decimal val = 200;

            decimal percentOf = 20;

            decimal actual = val.PercentOf(percentOf);

            decimal expected = 40;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void PercentOfTest3()
        {
            decimal val = -100;

            decimal percentOf = 20;

            decimal actual = val.PercentOf(percentOf);

            decimal expected = -20;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PercentOfTest4()
        {
            decimal val = -100;

            decimal percentOf = -20;

            decimal actual = val.PercentOf(percentOf);

            decimal expected = 20;
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

            iatiactivity[] array = new iatiactivity[3];
            array[0] = new iatiactivity { IatiIdentifier = "546" };

            iatiactivity obj = array.n(1);


            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void ReturnTheObjFromSpecifiedPositionOfAnArray5()
        {

            iatiactivity[] array = new iatiactivity[3];
            array[0] = new iatiactivity { IatiIdentifier = "546" };

            iatiactivity obj = array.n(12);


            Assert.IsNotNull(obj);
        }



        [TestMethod]
        public void ReturnTheObjFromSpecifiedPositionOfAnArray6()
        {

            List<iatiactivity> array = new List<iatiactivity> {new iatiactivity { IatiIdentifier = "546" } };

            iatiactivity obj = array.n(0);


            Assert.IsNotNull(obj.IatiIdentifier == "546");
        }

    }


}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeKicker.BBCode.Tests.Unit
{
    [TestClass]
    public class BBTagTests
    {
        [TestMethod]
        [DataRow(null, null, null)]
        [DataRow(null, "", "")]
        [DataRow("", null, "")]
        [DataRow("", "", null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Should_Throw_ArgumentNullException_When_Null_Passed(string name, string openTagTemplate, string closeTagTemplate)
        {
            new BBTag(name, openTagTemplate, closeTagTemplate);
        }



        [TestMethod]
        public void FindAttribute_Method_Should_Return_Attribute_When_Its_Exists()
        {
            var attr = new BBAttribute("id", "size");
            var bbTag = new BBTag("", "", "", attr);

            var actual = bbTag.FindAttribute("size");

            Assert.AreEqual(attr, actual);
        }

        [TestMethod]
        public void FindAttribute_Method_Should_Keep_Return_Attribute_Reference()
        {
            var attr = new BBAttribute("id", "size");
            var bbTag = new BBTag("", "", "", attr);

            var actual = bbTag.FindAttribute("size");

            Assert.IsTrue(ReferenceEquals(attr, actual));
        }

        [TestMethod]
        public void FindAttribute_Method_Should_Return_Null_When_Attribute_With_That_Name_Not_Exists()
        {
            var attr = new BBAttribute("id", "size");
            var bbTag = new BBTag("", "", "", attr);

            var actual = bbTag.FindAttribute("width");

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void FindAttribute_Method_Should_Return_Null_When_Null_Parapeter_Passed()
        {
            var bbTag = new BBTag("", "", "");

            var actual = bbTag.FindAttribute(null);

            Assert.IsNull(actual);
        }
    }
}

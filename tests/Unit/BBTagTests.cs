using NUnit.Framework;
using System;

namespace CodeKicker.BBCode.Tests.Unit
{
    public class BBTagTests
    {
        [TestCase(null, null, null)]
        [TestCase(null, "", "")]
        [TestCase("", null, "")]
        [TestCase("", "", null)]
        public void Constructor_Should_Throw_ArgumentNullException_When_Null_Passed(string name, string openTagTemplate, string closeTagTemplate)
        {
            Assert.Throws<ArgumentNullException>(
                () => new BBTag(name, openTagTemplate, closeTagTemplate));
        }



        [Test]
        public void FindAttribute_Method_Should_Return_Attribute_When_Its_Exists()
        {
            var attr = new BBAttribute("id", "size");
            var bbTag = new BBTag("", "", "", attr);

            var actual = bbTag.FindAttribute("size");

            Assert.AreEqual(attr, actual);
        }

        [Test]
        public void FindAttribute_Method_Should_Keep_Return_Attribute_Reference()
        {
            var attr = new BBAttribute("id", "size");
            var bbTag = new BBTag("", "", "", attr);

            var actual = bbTag.FindAttribute("size");

            Assert.IsTrue(ReferenceEquals(attr, actual));
        }

        [Test]
        public void FindAttribute_Method_Should_Return_Null_When_Attribute_With_That_Name_Not_Exists()
        {
            var attr = new BBAttribute("id", "size");
            var bbTag = new BBTag("", "", "", attr);

            var actual = bbTag.FindAttribute("width");

            Assert.IsNull(actual);
        }

        [Test]
        public void FindAttribute_Method_Should_Return_Null_When_Null_Parapeter_Passed()
        {
            var bbTag = new BBTag("", "", "");

            var actual = bbTag.FindAttribute(null);

            Assert.IsNull(actual);
        }
    }
}

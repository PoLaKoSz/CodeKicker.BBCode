using NUnit.Framework;
using System;

namespace CodeKicker.BBCode.Tests.Unit
{
    public class BBAttributeTests
    {
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void Constructor_Should_Throw_ArgumentNullExcetpion(string id, string name)
        {
            Assert.Throws<ArgumentNullException>(() => new BBAttribute(id, name));
        }
    }
}

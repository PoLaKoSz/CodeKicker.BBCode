using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeKicker.BBCode.Tests.Unit
{
    [TestClass]
    public class BBAttributeTests
    {
        [TestMethod]
        [DataRow(null, null)]
        [DataRow("", null)]
        [DataRow(null, "")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Should_Throw_ArgumentNullExcetpion(string id, string name)
        {
            new BBAttribute(id, name);
        }
    }
}

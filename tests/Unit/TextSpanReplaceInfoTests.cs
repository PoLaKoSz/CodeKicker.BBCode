using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeKicker.BBCode.Tests.Unit
{
    [TestClass]
    public class TextSpanReplaceInfoTests
    {
        [TestMethod]
        [DataRow(-1, -1)]
        [DataRow(-1, 0)]
        [DataRow(0, -1)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Should_Throw_ArgumentOutOfRangeException_When_Less_Than_Zero_Passed(int index, int length)
        {
            new TextSpanReplaceInfo(index, length, null);
        }

        [TestMethod]
        public void Constructor_Replacement_Can_Be_Null()
        {
            new TextSpanReplaceInfo(0, 0, null);
        }
    }
}

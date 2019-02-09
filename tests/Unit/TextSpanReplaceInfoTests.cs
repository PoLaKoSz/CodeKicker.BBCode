using NUnit.Framework;
using System;

namespace CodeKicker.BBCode.Tests.Unit
{
    public class TextSpanReplaceInfoTests
    {
        [TestCase(-1, -1)]
        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        public void Constructor_Should_Throw_ArgumentOutOfRangeException_When_Less_Than_Zero_Passed(int index, int length)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new TextSpanReplaceInfo(index, length, null));
        }

        [Test]
        public void Constructor_Replacement_Can_Be_Null()
        {
            new TextSpanReplaceInfo(0, 0, null);
        }
    }
}

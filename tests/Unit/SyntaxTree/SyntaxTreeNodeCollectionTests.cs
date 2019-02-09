using CodeKicker.BBCode.SyntaxTree;
using NUnit.Framework;
using System;

namespace CodeKicker.BBCode.Tests.Unit.SyntaxTree
{
    public class SyntaxTreeNodeCollectionTests
    {
        [Test]
        public void Contructor_With_Null_Parameter_Should_Return_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SyntaxTreeNodeCollection(null));
        }

        [Test]
        public void Constructor_Without_Parameter_Has_To_Exists()
        {
            // Just for backwards compatibility.

            new SyntaxTreeNodeCollection();
        }
    }
}

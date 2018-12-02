using CodeKicker.BBCode.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CodeKicker.BBCode.Tests.Unit.SyntaxTree
{
    [TestClass]
    public class SyntaxTreeNodeCollectionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Contructor_With_Null_Parameter_Should_Return_ArgumentNullException()
        {
            new SyntaxTreeNodeCollection(null);
        }

        [TestMethod]
        public void Constructor_Without_Parameter_Has_To_Exists()
        {
            // Just for backwards compatibility.

            new SyntaxTreeNodeCollection();

            Assert.IsTrue(true);
        }
    }
}

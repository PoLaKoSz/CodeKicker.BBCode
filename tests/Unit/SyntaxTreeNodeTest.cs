using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeKicker.BBCode.Tests.Unit
{
    [TestClass]
    public class SyntaxTreeNodeTest
    {
        //[TestMethod]
        public void EqualTreesHaveEqualBBCode(out string bbCode1, out string bbCode2)
        {
            var tree1 = BBCodeTestUtil.GetAnyTree();
            var tree2 = BBCodeTestUtil.GetAnyTree();
            bbCode1 = tree1.ToBBCode();
            bbCode2 = tree2.ToBBCode();
            Assert.AreEqual(tree1 == tree2, bbCode1 == bbCode2);
        }

        //[TestMethod]
        public void UnequalTexthasUnequalTrees(out string text1, out string text2)
        {
            var tree1 = BBCodeTestUtil.GetAnyTree();
            var tree2 = BBCodeTestUtil.GetAnyTree();
            text1 = tree1.ToText();
            text2 = tree2.ToText();
            if (text1 != text2) Assert.IsTrue(tree1 != tree2);
        }
    }
}

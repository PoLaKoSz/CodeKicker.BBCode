using CodeKicker.BBCode.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tests.Unit.SyntaxTree
{
    [TestClass]
    public class SequenceNodeTests
    {
        private readonly SyntaxTreeNodeCollection _treeNode;



        public SequenceNodeTests()
        {
            _treeNode = new SyntaxTreeNodeCollection(new List<SyntaxTreeNode>()
            {
                new TagNode(
                    new BBTag("div", "<div>", "</div>"),
                    new List<SyntaxTreeNode>()
                    {
                        new TextNode(" ... "),
                        new TagNode(
                            new BBTag("url", "<a>", "</a>"),
                            new List<SyntaxTreeNode>()
                            {
                                new TextNode("Please click on this link!")
                            }
                        ),
                    }
                )
            });
        }



        [TestMethod]
        public void Constructor_Without_Parameter_Has_To_Exists()
        {
            // Just for backwards compatibility.

            new SequenceNode();

            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_With_Null_Class_Should_Throw_ArgumentNull_Exception()
        {
            SyntaxTreeNodeCollection syntaxTreeNodeCollection = null;
            new SequenceNode(syntaxTreeNodeCollection);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_With_Null_IEnumerable_Should_Throw_ArgumentNull_Exception()
        {
            IEnumerable<SyntaxTreeNode> syntaxTreeNodeCollection = null;
            new SequenceNode(syntaxTreeNodeCollection);
        }



        [TestMethod]
        public void ToHtml_Method_Should_Return_Empty_String_When_Has_No_SubNodes()
        {
            var obj = new SequenceNode();

            Assert.AreEqual("", obj.ToHtml());
        }

        [TestMethod]
        public void ToHTM_Method_With_Deep_Tree()
        {
            var node = new SequenceNode(_treeNode);
            var expected =
                "<div>" +
                    " ... " +
                    "<a>" +
                        "Please click on this link!" +
                    "</a>" +
                "</div>";


            var actual = node.ToHtml();


            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        public void ToBBCode_Method_With_Deep_Tree()
        {
            var node = new SequenceNode(_treeNode);
            var expected =
                 "[div]" +
                     " ... " +
                     "[url]" +
                         "Please click on this link!" +
                     "[/url]" +
                 "[/div]";


            var actual = node.ToBBCode();


            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        public void ToText_Method_With_Deep_Tree()
        {
            var node = new SequenceNode(_treeNode);
            var expected = " ... Please click on this link!";


            var actual = node.ToText();


            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetSubNodes_Should_Return_ArgumenNullException_When_Null_Passed()
        {
            new SequenceNode().SetSubNodes(null);
        }

        [TestMethod]
        public void SetSubNodes_Should_Return_A_SequenceNode()
        {
            var actual = new SequenceNode().SetSubNodes(new List<SequenceNode>());

            Assert.AreEqual(typeof(SequenceNode), actual.GetType());
        }

        [TestMethod]
        public void SetSubNodes_Should_Return_A_SequenceNode_With_The_Parameter_SubNodes()
        {
            var input = new List<SequenceNode>()
            {
                new SequenceNode(),
                new SequenceNode(),
            };

            var actual = new SequenceNode().SetSubNodes(input);

            Assert.AreEqual(2, actual.SubNodes.Count);
        }


        [TestMethod]
        public void Null_Should_Not_Be_Equal_With_SequenceNode()
        {
            var sequenceNode = new SequenceNode(new SyntaxTreeNodeCollection());

            Assert.IsFalse(sequenceNode.Equals(null));
        }

        [TestMethod]
        public void Two_Empty_SequenceNode_Should_Be_Equal()
        {
            var sequenceNode = new SequenceNode(new SyntaxTreeNodeCollection());
            var anotherNode = new SequenceNode(new SyntaxTreeNodeCollection());

            Assert.IsTrue(sequenceNode.Equals(anotherNode));
        }

        [TestMethod]
        public void Two_Different_Reference_SequalNode_With_Same_Amount_And_Type_Children_Should_Be_Equal()
        {
            var sequenceNode = new SequenceNode(new SyntaxTreeNodeCollection(new List<SyntaxTreeNode>() { new TextNode("")}));
            var anotherNode = new SequenceNode(new SyntaxTreeNodeCollection(new List<SyntaxTreeNode>() { new TextNode("") }));

            Assert.IsTrue(sequenceNode.Equals(anotherNode));
        }

        [TestMethod]
        public void Two_Different_Reference_SequalNode_With_Same_Amount_But_Different_Type_Children_Should_Not_Be_Equal()
        {
            var sequenceNode = new SequenceNode(
                new SyntaxTreeNodeCollection(
                    new List<SyntaxTreeNode>()
                    {
                        new TextNode("")
                    }));

            var anotherNode = new SequenceNode(
                new SyntaxTreeNodeCollection(
                    new List<SyntaxTreeNode>()
                    {
                        new TagNode(new BBTag("", "", ""))
                    }));

            Assert.IsFalse(sequenceNode.Equals(anotherNode));
        }
    }
}

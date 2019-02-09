using CodeKicker.BBCode.SyntaxTree;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tests.Unit.SyntaxTree
{
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



        [Test]
        public void Constructor_Without_Parameter_Has_To_Exists()
        {
            // Just for backwards compatibility.

            new SequenceNode();
        }

        [Test]
        public void Constructor_With_Null_Class_Should_Throw_ArgumentNull_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => new SequenceNode(null));
        }



        [Test]
        public void ToHtml_Method_Should_Return_Empty_String_When_Has_No_SubNodes()
        {
            var obj = new SequenceNode();

            Assert.AreEqual("", obj.ToHtml());
        }

        [Test]
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



        [Test]
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



        [Test]
        public void ToText_Method_With_Deep_Tree()
        {
            var node = new SequenceNode(_treeNode);
            var expected = " ... Please click on this link!";


            var actual = node.ToText();


            Assert.AreEqual(expected, actual);
        }



        [Test]
        public void SetSubNodes_Should_Return_ArgumenNullException_When_Null_Passed()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SequenceNode().SetSubNodes(null));
        }

        [Test]
        public void SetSubNodes_Should_Return_A_SequenceNode()
        {
            var actual = new SequenceNode().SetSubNodes(new List<SequenceNode>());

            Assert.AreEqual(typeof(SequenceNode), actual.GetType());
        }

        [Test]
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


        [Test]
        public void Null_Should_Not_Be_Equal_With_SequenceNode()
        {
            var sequenceNode = new SequenceNode(new SyntaxTreeNodeCollection());

            Assert.IsFalse(sequenceNode.Equals(null));
        }

        [Test]
        public void Two_Empty_SequenceNode_Should_Be_Equal()
        {
            var sequenceNode = new SequenceNode(new SyntaxTreeNodeCollection());
            var anotherNode = new SequenceNode(new SyntaxTreeNodeCollection());

            Assert.IsTrue(sequenceNode.Equals(anotherNode));
        }

        [Test]
        public void Two_Different_Reference_SequalNode_With_Same_Amount_And_Type_Children_Should_Be_Equal()
        {
            var sequenceNode = new SequenceNode(new SyntaxTreeNodeCollection(new List<SyntaxTreeNode>() { new TextNode("")}));
            var anotherNode = new SequenceNode(new SyntaxTreeNodeCollection(new List<SyntaxTreeNode>() { new TextNode("") }));

            Assert.IsTrue(sequenceNode.Equals(anotherNode));
        }

        [Test]
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

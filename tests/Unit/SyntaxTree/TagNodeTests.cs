using CodeKicker.BBCode.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tests.Unit.SyntaxTree
{
    [TestClass]
    public class TagNodeTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_With_Null_Tag_Parameter_Should_Throw_ArgumentNullException()
        {
            new TagNode(null);
        }

        [TestMethod]
        public void Constructor_With_Null_SubNodes_Parameter_Should_Not_Throw_Exception()
        {
            new TagNode(new BBTag("", "", ""), subNodes: null);
        }



        [TestMethod]
        public void ToHTM_Method()
        {
            var bbTag = new BBTag("url", "<a href=\"https://codekicker.de\">", "</a>", new BBAttribute("link", ""));
            var subNodes = new List<TagNode>()
            {
                new TagNode(new BBTag("b", "<strong>${boldedContent}", "</strong>", new BBAttribute("boldedContent", ""))),
                new TagNode(new BBTag("quote", "<blockquote>${quote}", "</blockquote>", new BBAttribute("quote", ""))),
            };
            var tagNode = new TagNode(bbTag, subNodes);

            var actual = tagNode.ToHtml();
            var expected =
                "<a href=\"https://codekicker.de\">" +
                    "<strong></strong>" +
                    "<blockquote></blockquote>" +
                "</a>";

            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        public void ToBBCode_Method()
        {
            var bbTag = new BBTag("url", "<a href=\"${link}\">", "</a>", new BBAttribute("link", ""));
            var subNodes = new List<TagNode>()
            {
                new TagNode(new BBTag("b", "<strong>${boldedContent}", "</strong>", new BBAttribute("boldedContent", ""))),
                new TagNode(new BBTag("quote", "<blockquote>${quote}", "</blockquote>", new BBAttribute("quote", ""))),
            };
            var tagNode = new TagNode(bbTag, subNodes);

            var actual = tagNode.ToBBCode();
            var expected =
                "[url]" +
                    "[b][/b]" +
                    "[quote][/quote]" +
                "[/url]";

            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        public void ToText_Method()
        {
            var bbTag = new BBTag("url", "<a href=\"${link}\">", "</a>", new BBAttribute("link", ""));
            var subNodes = new List<TagNode>()
            {
                new TagNode(new BBTag("b", "<strong>${boldedContent}", "</strong>", new BBAttribute("boldedContent", ""))),
                new TagNode(new BBTag("quote", "<blockquote>${quote}", "</blockquote>", new BBAttribute("quote", ""))),
            };
            var tagNode = new TagNode(bbTag, subNodes);

            var actual = tagNode.ToText();

            Assert.AreEqual("", actual);
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetSubNodes_Method_Should_Return_ArgumentNullException_When_Null_Passed()
        {
            new TagNode(new BBTag("", "", "")).SetSubNodes(null);
        }

        [TestMethod]
        public void SetSubNodes_Method_Return_Type_Should_Be_TagNode()
        {
            var tagNode = new TagNode(new BBTag("b", "<b>", "</b>"));

            var actual = tagNode.SetSubNodes(new List<SyntaxTreeNode>());

            Assert.AreEqual(typeof(TagNode), actual.GetType());
        }

        [TestMethod]
        public void SetSubNodes_Method_Should_Keep_Tag_Reference_In_The_Return_TagNode()
        {
            var tag = new BBTag("b", "<b>", "</b>");
            var tagNode = new TagNode(tag);

            var actual = (TagNode)tagNode.SetSubNodes(new List<SyntaxTreeNode>());

            Assert.IsTrue(ReferenceEquals(tag, actual.Tag));
        }

        [TestMethod]
        public void SetSubNodes_Method_Should_Keep_SubModules_Reference_In_The_Return_TagNode()
        {
            var tagNode = new TagNode(new BBTag("b", "<b>", "</b>"));
            var subNodes = new List<SyntaxTreeNode>()
            {
                new TagNode(new BBTag("", "", ""))
            };

            var actual = (TagNode)tagNode.SetSubNodes(subNodes);

            Assert.AreEqual(subNodes.Count, actual.SubNodes.Count, "count");
            Assert.IsTrue(ReferenceEquals(subNodes[0], actual.SubNodes[0]), "IsTrue");
        }

        [TestMethod]
        public void SetSubNodes_Method_Should_Not_Keep_AttributeValues_Reference_In_The_Return_TagNode()
        {
            var tagNode = new TagNode(new BBTag("b", "<b>", "</b>"));

            var actual = (TagNode)tagNode.SetSubNodes(new List<SyntaxTreeNode>());

            Assert.AreEqual(tagNode.AttributeValues.Count, actual.AttributeValues.Count, "count");
            Assert.IsTrue(!ReferenceEquals(tagNode.AttributeValues, actual.AttributeValues), "IsTrue");
        }
    }
}

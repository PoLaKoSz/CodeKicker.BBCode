using CodeKicker.BBCode.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tests.Unit.SyntaxTree
{
    [TestClass]
    public class TextNodeTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_With_Null_Text_Parameter_Should_Throw_ArgumentNullException()
        {
            new TextNode(null);
        }

        [TestMethod]
        public void Constructor_With_Null_HtmlTemplate_Parameter_Should_Not_Throw_Exception()
        {
            new TextNode(text: "", htmlTemplate: null);
        }



        [TestMethod]
        public void ToHTM_Method_With_Null_HtmlTemplate_And_Empty_Text()
        {
            var textNode = new TextNode("", null);

            var actual = textNode.ToHtml();

            Assert.AreEqual("", actual);
        }

        [TestMethod]
        public void ToHTM_Method_Without_HtmlTemplate_Escape_HTML_Characters()
        {
            var textNode = new TextNode("<b>bolded text</b>", null);

            var actual = textNode.ToHtml();

            Assert.AreEqual("&lt;b&gt;bolded text&lt;/b&gt;", actual);
        }

        [TestMethod]
        public void ToHTM_Method_With_Empty_HtmlTemplate_Should_Return_Empty_String()
        {
            var textNode = new TextNode("<b>no matter the content</b>", "");

            var actual = textNode.ToHtml();

            Assert.AreEqual("", actual);
        }

        [TestMethod]
        public void ToHTM_Method_With_HtmlTemplate_And_Empty_Text()
        {
            var textNode = new TextNode("", "html template");

            var actual = textNode.ToHtml();

            Assert.AreEqual("html template", actual);
        }

        [TestMethod]
        public void ToHTM_Method_With_Content_Magic_HtmlTemplate_On_A_Valid_Text()
        {
            var textNode = new TextNode("https://codekicker.de", "<a href=\"${content}\">");

            var actual = textNode.ToHtml();

            Assert.AreEqual("<a href=\"https://codekicker.de\">", actual);
        }

        [TestMethod]
        public void ToHTM_Method_Replace_HtmlTemplate_NewLines()
        {
            var textNode = new TextNode("", "\n\n__\n");

            var actual = textNode.ToHtml();

            Assert.AreEqual("<br /><br />__<br />", actual);
        }

        [TestMethod]
        public void ToHTM_Method_Replace_Text_NewLines()
        {
            var textNode = new TextNode("\nTEXT\n", "My ${content} content\n.");

            var actual = textNode.ToHtml();

            Assert.AreEqual("My <br />TEXT<br /> content<br />.", actual);
        }



        [TestMethod]
        public void ToBBCode_With_Empty_Text_Should_Return_Empty_String()
        {
            var textNode = new TextNode("");

            var actual = textNode.ToBBCode();

            Assert.AreEqual("", actual);
        }

        [TestMethod]
        public void ToBBCode_Should_Escape_Special_Characters()
        {
            var textNode = new TextNode("[b]My \\ text.[/b]");

            var actual = textNode.ToBBCode();

            Assert.AreEqual("\\[b\\]My \\\\ text.\\[/b\\]", actual);
        }



        [TestMethod]
        public void ToText_Method_Should_Return_Text_Property()
        {
            var textNode = new TextNode("[b]My \\ text.[/b]");

            var actual = textNode.ToText();

            Assert.AreEqual("[b]My \\ text.[/b]", actual);
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetSubNodes_Method_Should_Return_ArgumentNullException_When_Null_Passed()
        {
            new TextNode("").SetSubNodes(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetSubNodes_Method_Should_Return_ArgumentException_When_Parameter_Has_Elements()
        {
            var textNode = new TextNode("");
            var subNodes = new List<TextNode>()
            {
                new TextNode("")
            };

            var actual = textNode.SetSubNodes(subNodes);
        }

        [TestMethod]
        public void SetSubNodes_Method_Should_Return_The_Same_Reference()
        {
            var textNode = new TextNode("");

            var actual = textNode.SetSubNodes(new List<TextNode>());

            Assert.IsTrue(ReferenceEquals(textNode, actual));
        }

        [TestMethod]
        public void SetSubNodes_Method_Return_Type_Should_Be_TextNode()
        {
            var tagNode = new TextNode("");

            var actual = tagNode.SetSubNodes(new List<SyntaxTreeNode>());

            Assert.AreEqual(typeof(TextNode), actual.GetType());
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeKicker.BBCode.Tests.Unit
{
    [TestClass]
    public class BBCodeTests
    {
        [TestMethod]
        [DataRow("")]
        public void DefaultParserWellconfigured(string input)
        {
            try
            {
                BBCode.ToHtml(input);
            }
            catch (BBCodeParsingException)
            {
            }
        }

        [TestMethod]
        [DataRow("", "")]
        public void Escape_NoCrash(string text, out string escaped)
        {
            escaped = BBCode.EscapeText(text);
        }

        [TestMethod]
        [DataRow("<a href=\"\"></a>")]
        public void Can_Escape_And_Unescape_HTML(string text)
        {
            var escaped = BBCode.EscapeText(text);
            var unescaped = BBCode.UnescapeText(escaped);

            Assert.AreEqual(text, unescaped);
        }

        [TestMethod]
        [DataRow("[url=https://codekicker.de][/url]")]
        public void Can_Escape_And_Unescape_BBode(string text)
        {
            var escaped = BBCode.EscapeText(text);
            var unescaped = BBCode.UnescapeText(escaped);

            Assert.AreEqual(text, unescaped);
        }

        [TestMethod]
        [DataRow("<div><script>function asd() {}</script></div>")]
        public void EscapedStringIsSafeForParsing(string text)
        {
            var escaped = BBCode.EscapeText(text);

            var ast = GetSimpleParser().ParseSyntaxTree(escaped);

            if (text.Length == 0)
                Assert.AreEqual(0, ast.SubNodes.Count);
            else
                Assert.AreEqual(text, ((TextNode)ast.SubNodes.Single()).Text);
        }

        [TestMethod]
        [DataRow("<b>MyTest<div>in the DIV</div></b>")]
        public void Escape_Parse_ToText_Roundtrip(string text)
        {
            var escaped = BBCode.EscapeText(text);
            var unescaped = GetSimpleParser().ParseSyntaxTree(escaped);
            var text2 = unescaped.ToText();

            Assert.AreEqual(text, text2);
        }

        static BBCodeParser GetSimpleParser()
        {
            return new BBCodeParser(new List<BBTag>());
        }

        [TestMethod]
        public void ReplaceTextSpans_ManualTestCases()
        {
            ReplaceTextSpans_ManualTestCases_TestCase("", "", null, null);
            ReplaceTextSpans_ManualTestCases_TestCase("a", "a", null, null);
            ReplaceTextSpans_ManualTestCases_TestCase("[b]a[/b]", "[b]a[/b]", null, null);
            ReplaceTextSpans_ManualTestCases_TestCase("[b]a[/b]", "[b]a[/b]", txt => new[] { new TextSpanReplaceInfo(0, 0, null), }, null);
            ReplaceTextSpans_ManualTestCases_TestCase("[b]a[/b]", "[b][/b]", txt => new[] { new TextSpanReplaceInfo(0, 1, null), }, null);
            ReplaceTextSpans_ManualTestCases_TestCase("[b]a[/b]", "[b]x[/b]", txt => new[] { new TextSpanReplaceInfo(0, 1, new TextNode("x")), }, null);

            ReplaceTextSpans_ManualTestCases_TestCase("abc[b]def[/b]ghi[i]jkl[/i]", "xyabc[b]z[/b]g2i1[i]jkl[/i]",
                txt =>
                    txt == "abc" ? new[] { new TextSpanReplaceInfo(0, 0, new TextNode("x")), new TextSpanReplaceInfo(0, 0, new TextNode("y")), } :
                    txt == "def" ? new[] { new TextSpanReplaceInfo(0, 3, new TextNode("z")), } :
                    txt == "ghi" ? new[] { new TextSpanReplaceInfo(1, 1, new TextNode("2")), new TextSpanReplaceInfo(3, 0, new TextNode("1")), } :
                    txt == "jkl" ? new[] { new TextSpanReplaceInfo(0, 0, new TextNode("w")), } :
                    null,
                    tagNode => tagNode.Tag.Name != "i");
        }

        static void ReplaceTextSpans_ManualTestCases_TestCase(string bbCode, string expected, Func<string, IList<TextSpanReplaceInfo>> getTextSpansToReplace, Func<TagNode, bool> tagFilter)
        {
            var tree1 = BBCodeTestUtil.GetParserForTest(ErrorMode.Strict, false, BBTagClosingStyle.AutoCloseElement, false).ParseSyntaxTree(bbCode);
            var tree2 = BBCode.ReplaceTextSpans(tree1, getTextSpansToReplace ?? (txt => new TextSpanReplaceInfo[0]), tagFilter);

            Assert.AreEqual(expected, tree2.ToBBCode());
        }

        [TestMethod]
        public void ReplaceTextSpans_WhenNoModifications_TreeIsPreserved()
        {
            var tree1 = BBCodeTestUtil.GetAnyTree();
            var tree2 = BBCode.ReplaceTextSpans(tree1, txt => new TextSpanReplaceInfo[0], null);
            Assert.AreSame(tree1, tree2);
        }

        [TestMethod]
        public void ReplaceTextSpans_WhenEmptyModifications_TreeIsPreserved()
        {
            var tree1 = BBCodeTestUtil.GetAnyTree();
            var tree2 = BBCode.ReplaceTextSpans(tree1, txt => new[] { new TextSpanReplaceInfo(0, 0, null), }, null);
            Assert.AreEqual(tree1.ToBBCode(), tree2.ToBBCode());
        }

        [TestMethod]
        public void ReplaceTextSpans_WhenEverythingIsConvertedToX_OutputContainsOnlyX_CheckedWithContains()
        {
            var tree1 = BBCodeTestUtil.GetAnyTree();
            var tree2 = BBCode.ReplaceTextSpans(tree1, txt => new[] { new TextSpanReplaceInfo(0, txt.Length, new TextNode("x")), }, null);
            Assert.IsTrue(!tree2.ToBBCode().Contains("a"));
        }

        [TestMethod]
        public void ReplaceTextSpans_WhenEverythingIsConvertedToX_OutputContainsOnlyX_CheckedWithTreeWalk()
        {
            var tree1 = BBCodeTestUtil.GetAnyTree();
            var tree2 = BBCode.ReplaceTextSpans(tree1, txt => new[] { new TextSpanReplaceInfo(0, txt.Length, new TextNode("x")), }, null);
            new TextAssertVisitor(str => Assert.IsTrue(str == "x")).Visit(tree2);
        }

        [TestMethod]
        public void ReplaceTextSpans_ArbitraryTextSpans_NoCrash()
        {
            //var tree1 = BBCodeTestUtil.GetAnyTree();
            //var chosenTexts = new List<string>();
            //var tree2 = BBCode.ReplaceTextSpans(tree1, txt =>
            //    {
            //        var count = BBCodeTestUtil.Random.Next(0, 3);
            //        var indexes = PexChoose.Array<int>("indexes", count);
            //        PexAssume.TrueForAll(0, count, i => indexes[i] >= 0 && indexes[i] <= txt.Length && (i == 0 || indexes[i - 1] < indexes[i]));
            //        return
            //            Enumerable.Range(0, count)
            //                .Select(i =>
            //                    {
            //                        var maxIndex = i == count - 1 ? txt.Length : indexes[i + 1];
            //                        var text = PexChoose.ValueNotNull<string>("text");
            //                        chosenTexts.Add(text);
            //                        return new TextSpanReplaceInfo(indexes[i], BBCodeTestUtil.Random.Next(0, indexes[i] - maxIndex + 1), new TextNode(text));
            //                    })
            //                .ToArray();
            //    }, null);
            //var bbCode = tree2.ToBBCode();
            //PexAssert.TrueForAll(chosenTexts, s => bbCode.Contains(s));

            Assert.Fail("This method should be re-implemented!");
        }

        class TextAssertVisitor : SyntaxTreeVisitor
        {
            Action<string> assertFunction;

            public TextAssertVisitor(Action<string> assertFunction)
            {
                this.assertFunction = assertFunction;
            }

            protected internal new SyntaxTreeNode Visit(TextNode node)
            {
                assertFunction(node.Text);
                return node;
            }
        }
    }
}

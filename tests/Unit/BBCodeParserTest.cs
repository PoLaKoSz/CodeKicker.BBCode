using System;
using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode.SyntaxTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeKicker.BBCode.Tests.Unit
{
    [TestClass]
    public class BBCodeParserTest
    {
        [TestMethod]
        public void Test1()
        {
            Assert.AreEqual("", BBEncodeForTest("", BBCodeTestUtil.StrictParsing));
        }

        [TestMethod]
        public void Test2()
        {
            Assert.AreEqual("a", BBEncodeForTest("a", BBCodeTestUtil.StrictParsing));
            Assert.AreEqual(" a b c ", BBEncodeForTest(" a b c ", BBCodeTestUtil.StrictParsing));
        }

        [TestMethod]
        public void Test3()
        {
            Assert.AreEqual("<b></b>", BBEncodeForTest("[b][/b]", BBCodeTestUtil.StrictParsing));
        }

        [TestMethod]
        public void Test4()
        {
            Assert.AreEqual("text<b>text</b>text", BBEncodeForTest("text[b]text[/b]text", BBCodeTestUtil.StrictParsing));
        }

        [TestMethod]
        public void Test5()
        {
            Assert.AreEqual("<a href=\"http://example.org/path?name=value\">text</a>", BBEncodeForTest("[url=http://example.org/path?name=value]text[/url]", BBCodeTestUtil.StrictParsing));
        }

        [TestMethod]
        public void LeafElementWithoutContent()
        {
            Assert.AreEqual("xxxnameyyy", BBEncodeForTest("[placeholder=name]", BBCodeTestUtil.StrictParsing));
            Assert.AreEqual("xxxyyy", BBEncodeForTest("[placeholder=]", BBCodeTestUtil.StrictParsing));
            Assert.AreEqual("xxxyyy", BBEncodeForTest("[placeholder]", BBCodeTestUtil.StrictParsing));
            Assert.AreEqual("axxxyyyb", BBEncodeForTest("a[placeholder]b", BBCodeTestUtil.StrictParsing));
            Assert.AreEqual("<b>a</b>xxxyyy<b>b</b>", BBEncodeForTest("[b]a[/b][placeholder][b]b[/b]", BBCodeTestUtil.StrictParsing));

            try
            {
                BBEncodeForTest("[placeholder][/placeholder]", BBCodeTestUtil.StrictParsing);
                Assert.Fail();
            }
            catch (BBCodeParsingException)
            {
            }

            try
            {
                BBEncodeForTest("[placeholder/]", BBCodeTestUtil.StrictParsing);
                Assert.Fail();
            }
            catch (BBCodeParsingException)
            {
            }
        }

        [TestMethod]
        public void ImgTagHasNoContent()
        {
            Assert.AreEqual("<img src=\"url\" />", BBEncodeForTest("[img]url[/img]", BBCodeTestUtil.StrictParsing));
        }

        [TestMethod]
        public void ListItemIsAutoClosed()
        {
            Assert.AreEqual("<li>item</li>", BBEncodeForTest("[*]item", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, false));
            Assert.AreEqual("<ul><li>item</li></ul>", BBEncodeForTest("[list][*]item[/list]", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, false));
            Assert.AreEqual("<li>item</li>", BBEncodeForTest("[*]item[/*]", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, false));
            Assert.AreEqual("<li><li>item</li></li>", BBEncodeForTest("[*][*]item", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, false));
            Assert.AreEqual("<li>1<li>2</li></li>", BBEncodeForTest("[*]1[*]2", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, false));

            Assert.AreEqual("<li></li>item", BBEncodeForTest("[*]item", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.LeafElementWithoutContent, false));
            Assert.AreEqual("<ul><li></li>item</ul>", BBEncodeForTest("[list][*]item[/list]", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.LeafElementWithoutContent, false));
            Assert.AreEqual("<li></li><li></li>item", BBEncodeForTest("[*][*]item", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.LeafElementWithoutContent, false));
            Assert.AreEqual("<li></li>1<li></li>2", BBEncodeForTest("[*]1[*]2", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.LeafElementWithoutContent, false));

            Assert.AreEqual("<li>item</li>", BBEncodeForTest("[*]item", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<ul><li>item</li></ul>", BBEncodeForTest("[list][*]item[/list]", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li>item</li>", BBEncodeForTest("[*]item[/*]", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li></li><li>item</li>", BBEncodeForTest("[*][*]item", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li>1</li><li>2</li>", BBEncodeForTest("[*]1[*]2", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li>1<b>a</b></li><li>2</li>", BBEncodeForTest("[*]1[b]a[/b][*]2", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li>1<b>a</b></li><li>2</li>", BBEncodeForTest("[*]1[b]a[*]2", BBCodeTestUtil.ErrorFreeParsing, BBTagClosingStyle.AutoCloseElement, true));

            try
            {
                BBEncodeForTest("[*]1[b]a[*]2", BBCodeTestUtil.StrictParsing, BBTagClosingStyle.AutoCloseElement, true);
                Assert.Fail();
            }
            catch (BBCodeParsingException)
            {
            }
        }

        [TestMethod]
        public void TagContentTransformer()
        {
            var parser = new BBCodeParser(new[]
                {
                    new BBTag("b", "<b>", "</b>", true, true, content => content.Trim()),
                });

            Assert.AreEqual("<b>abc</b>", parser.ToHtml("[b] abc [/b]"));
        }

        [TestMethod]
        public void Can_Parse_With_Custom_Attribute_Value_Transformer()
        {
            var parser = new BBCodeParser(BBCodeTestUtil.StrictParsing, null, new[]
            {
                new BBTag("font", "<span style=\"${color}${font}\">", "</span>", true, true,
                    new BBAttribute("color", "color", attributeRenderingContext => string.IsNullOrEmpty(attributeRenderingContext.AttributeValue) ? "" : "color:" + attributeRenderingContext.AttributeValue + ";"),
                    new BBAttribute("font", "font", attributeRenderingContext => string.IsNullOrEmpty(attributeRenderingContext.AttributeValue) ? "" : "font-family:" + attributeRenderingContext.AttributeValue + ";")),
            });

            Assert.AreEqual("<span style=\"color:red;font-family:Arial;\">abc</span>", parser.ToHtml("[font color=red font=Arial]abc[/font]"));
            Assert.AreEqual("<span style=\"color:red;\">abc</span>", parser.ToHtml("[font color=red]abc[/font]"));
        }

        //the parser may never ever throw an exception other that BBCodeParsingException for any non-null input
        //[TestMethod]
        public void NoCrash(IExceptions errorMode, string input, BBTagClosingStyle listItemBbTagClosingStyle, out string output)
        {
            Assert.IsNotNull(errorMode);
            Assert.IsNotNull(listItemBbTagClosingStyle);

            try
            {
                output = BBEncodeForTest(input, errorMode, listItemBbTagClosingStyle, false);
                Assert.IsNotNull(output);
            }
            catch (BBCodeParsingException)
            {
                Assert.AreNotEqual(BBCodeTestUtil.ErrorFreeParsing, errorMode);
                output = null;
            }
        }

        //[TestMethod]
        public void ErrorFreeModeAlwaysSucceeds(string input, out string output)
        {
            output = BBEncodeForTest(input, BBCodeTestUtil.ErrorFreeParsing);
        }

        //no script-tags may be contained in the output under any circumstances
        //[TestMethod]
        public void NoScript_AnyInput(IExceptions errorMode, string input)
        {
            Assert.IsNotNull(errorMode);
            try
            {
                var output = BBEncodeForTest(input, errorMode);
                Assert.IsTrue(!output.Contains("<script"));
            }
            catch (BBCodeParsingException)
            {
                Assert.Fail();
            }
        }

        //no script-tags may be contained in the output under any circumstances
        [TestMethod]
        public void NoScript_AnyInput_Tree()
        {
            var parser = BBCodeTestUtil.GetParserForTest(BBCodeTestUtil.ErrorFreeParsing, true, BBTagClosingStyle.AutoCloseElement, false);
            var tree = BBCodeTestUtil.CreateRootNode(parser.Tags.ToArray());
            var output = tree.ToHtml();
            Assert.IsTrue(!output.Contains("<script"));
        }

        //no html-chars may be contained in the output under any circumstances
        //[TestMethod]
        public void NoHtmlChars_AnyInput(IExceptions errorMode, string input)
        {
            Assert.IsNotNull(errorMode);
            try
            {
                var output = BBCodeTestUtil.SimpleBBEncodeForTest(input, errorMode);
                Assert.IsTrue(output.IndexOf('<') == -1);
                Assert.IsTrue(output.IndexOf('>') == -1);
            }
            catch (BBCodeParsingException)
            {
                Assert.Fail();
            }
        }

        //[TestMethod]
        public void NoScript_FixedInput(IExceptions errorMode)
        {
            Assert.IsNotNull(errorMode);
            Assert.IsFalse(BBEncodeForTest("<script>", errorMode).Contains("<script"));
        }

        //[TestMethod]
        public void NoScriptInAttributeValue(IExceptions errorMode)
        {
            Assert.IsNotNull(errorMode);
            var encoded = BBEncodeForTest("[url=<script>][/url]", errorMode);
            Assert.IsFalse(encoded.Contains("<script"));
        }

        //1. given a syntax tree, encode it in BBCode, parse it back into a second syntax tree and ensure that both are exactly equal
        //2. given any syntax tree, the BBCode it represents must be parsable without error
        //[TestMethod]
        public void Roundtrip(IExceptions errorMode, out string bbcode, out string output)
        {
            Assert.IsNotNull(errorMode);

            var parser = BBCodeTestUtil.GetParserForTest(errorMode, false, BBTagClosingStyle.AutoCloseElement, false);
            var tree = BBCodeTestUtil.CreateRootNode(parser.Tags.ToArray());
            bbcode = tree.ToBBCode();
            var tree2 = parser.ParseSyntaxTree(bbcode);
            output = tree2.ToHtml();
            Assert.IsTrue(tree == tree2);
        }

        //given a BBCode-string, parse it into a syntax tree, encode the tree in BBCode, parse it back into a second sytax tree and ensure that both are exactly equal
        //[TestMethod]
        public void Roundtrip2(IExceptions errorMode, string input, out string bbcode, out string output)
        {
            Assert.IsNotNull(errorMode);

            var parser = BBCodeTestUtil.GetParserForTest(errorMode, false, BBTagClosingStyle.AutoCloseElement, false);
            SequenceNode tree;
            try
            {
                tree = parser.ParseSyntaxTree(input);
            }
#pragma warning disable 168
            catch (BBCodeParsingException e)
#pragma warning restore 168
            {
                Assert.Fail();
                tree = null;
            }

            bbcode = tree.ToBBCode();
            var tree2 = parser.ParseSyntaxTree(bbcode);
            output = tree2.ToHtml();
            Assert.IsTrue(tree == tree2);
        }

        //[TestMethod]
        public void TextNodesCannotBeSplit(IExceptions errorMode, string input)
        {
            Assert.IsNotNull(errorMode);

            var parser = BBCodeTestUtil.GetParserForTest(errorMode, true, BBTagClosingStyle.AutoCloseElement, false);
            SequenceNode tree;
            try
            {
                tree = parser.ParseSyntaxTree(input);
            }
#pragma warning disable 168
            catch (BBCodeParsingException e)
#pragma warning restore 168
            {
                Assert.Fail();
                return;
            }

            AssertTextNodesNotSplit(tree);
        }

        static void AssertTextNodesNotSplit(SyntaxTreeNode node)
        {
            if (node.SubNodes != null)
            {
                SyntaxTreeNode lastNode = null;
                for (int i = 0; i < node.SubNodes.Count; i++)
                {
                    AssertTextNodesNotSplit(node.SubNodes[i]);
                    if (lastNode != null)
                        Assert.IsFalse(lastNode is TextNode && node.SubNodes[i] is TextNode);
                    lastNode = node.SubNodes[i];
                }
            }
        }

        public static string BBEncodeForTest(string bbCode, IExceptions errorMode)
        {
            return BBEncodeForTest(bbCode, errorMode, BBTagClosingStyle.AutoCloseElement, false);
        }

        public static string BBEncodeForTest(string bbCode, IExceptions errorMode, BBTagClosingStyle listItemBbTagClosingStyle, bool enableIterationElementBehavior)
        {
            return BBCodeTestUtil.GetParserForTest(errorMode, true, listItemBbTagClosingStyle, enableIterationElementBehavior).ToHtml(bbCode).Replace("\r", "").Replace("\n", "<br/>");
        }

        //[TestMethod]// Probably was a Pex test suite
        public void ToTextDoesNotCrash(string input, out string text)
        {
            var parser = BBCodeTestUtil.GetParserForTest(BBCodeTestUtil.ErrorFreeParsing, true, BBTagClosingStyle.AutoCloseElement, false);
            text = parser.ParseSyntaxTree(input).ToText();
            Assert.IsTrue(text.Length <= input.Length);
        }

        [TestMethod]
        public void StrictErrorMode()
        {
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"", BBCodeTestUtil.StrictParsing));
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"[b]abc[/b]", BBCodeTestUtil.StrictParsing));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"[b]abc", BBCodeTestUtil.StrictParsing));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"abc[0]def", BBCodeTestUtil.StrictParsing));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"\", BBCodeTestUtil.StrictParsing));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"\x", BBCodeTestUtil.StrictParsing));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"[", BBCodeTestUtil.StrictParsing));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"]", BBCodeTestUtil.StrictParsing));
        }

        [TestMethod]
        public void CorrectingErrorMode()
        {
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"", BBCodeTestUtil.ErrorCorrectorParsing));
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"[b]abc[/b]", BBCodeTestUtil.ErrorCorrectorParsing));
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"[b]abc", BBCodeTestUtil.ErrorCorrectorParsing));

            Assert.AreEqual(@"\", BBEncodeForTest(@"\", BBCodeTestUtil.ErrorCorrectorParsing));
            Assert.AreEqual(@"\x", BBEncodeForTest(@"\x", BBCodeTestUtil.ErrorCorrectorParsing));
            Assert.AreEqual(@"\", BBEncodeForTest(@"\\", BBCodeTestUtil.ErrorCorrectorParsing));
        }

        [TestMethod]
        public void CorrectingErrorMode_EscapeCharsIgnored()
        {
            Assert.AreEqual(@"\\", BBEncodeForTest(@"\\\\", BBCodeTestUtil.ErrorCorrectorParsing));
            Assert.AreEqual(@"\", BBEncodeForTest(@"\", BBCodeTestUtil.ErrorCorrectorParsing));
            Assert.AreEqual(@"\x", BBEncodeForTest(@"\x", BBCodeTestUtil.ErrorCorrectorParsing));
            Assert.AreEqual(@"\", BBEncodeForTest(@"\\", BBCodeTestUtil.ErrorCorrectorParsing));
            Assert.AreEqual(@"[", BBEncodeForTest(@"\[", BBCodeTestUtil.ErrorCorrectorParsing));
            Assert.AreEqual(@"]", BBEncodeForTest(@"\]", BBCodeTestUtil.ErrorCorrectorParsing));
        }

        [TestMethod]
        public void TextNodeHtmlTemplate()
        {
            var parserNull = new BBCodeParser(BBCodeTestUtil.StrictParsing, null, new[]
                {
                    new BBTag("b", "<b>", "</b>"),
                });
            var parserEmpty = new BBCodeParser(BBCodeTestUtil.StrictParsing, "", new[]
                {
                    new BBTag("b", "<b>", "</b>"),
                });
            var parserDiv = new BBCodeParser(BBCodeTestUtil.StrictParsing, "<div>${content}</div>", new[]
                {
                    new BBTag("b", "<b>", "</b>"),
                });

            Assert.AreEqual(@"", parserNull.ToHtml(@""));
            Assert.AreEqual(@"abc", parserNull.ToHtml(@"abc"));
            Assert.AreEqual(@"abc<b>def</b>", parserNull.ToHtml(@"abc[b]def[/b]"));

            Assert.AreEqual(@"", parserEmpty.ToHtml(@""));
            Assert.AreEqual(@"", parserEmpty.ToHtml(@"abc"));
            Assert.AreEqual(@"<b></b>", parserEmpty.ToHtml(@"abc[b]def[/b]"));

            Assert.AreEqual(@"", parserDiv.ToHtml(@""));
            Assert.AreEqual(@"<div>abc</div>", parserDiv.ToHtml(@"abc"));
            Assert.AreEqual(@"<div>abc</div><b><div>def</div></b>", parserDiv.ToHtml(@"abc[b]def[/b]"));
        }

        [TestMethod]
        public void ContentTransformer_EmptyAttribute_CanChooseValueFromAttributeRenderingContext()
        {
            var parser = BBCodeTestUtil.GetParserForTest(BBCodeTestUtil.StrictParsing, true, BBTagClosingStyle.AutoCloseElement, false);

            Assert.AreEqual(@"<a href=""http://codekicker.de"">http://codekicker.de</a>", parser.ToHtml(@"[url2]http://codekicker.de[/url2]"));
            Assert.AreEqual(@"<a href=""http://codekicker.de"">http://codekicker.de</a>", parser.ToHtml(@"[url2=http://codekicker.de]http://codekicker.de[/url2]"));
        }

        [TestMethod]
        public void StopProcessingDirective_Prevent_Parsing_Child_Tags()
        {
            var parser = new BBCodeParser(BBCodeTestUtil.ErrorFreeParsing, null, new[]
            {
                new BBTag("code", "<pre>", "</pre>") { StopProcessing = true },
                new BBTag("b", "<b>", "</b>"),
                new BBTag("i", "<span style=\"font-style:italic;\">", "</span>"),
                new BBTag("u", "<span style=\"text-decoration:underline;\">", "</span>"),
            });

            var input = "Hello! This is my code sample:" +
                "[code]" +
                    "[i]This should [u]be a[/u] text literal[/i]" +
                "[/code]" +
                "[b]Can You guys give me some advice?[/b]";

            var expected = "Hello! This is my code sample:" +
                "<pre>" +
                    "[i]This should [u]be a[/u] text literal[/i]" +
                "</pre>" +
                "<b>Can You guys give me some advice?</b>";

            Assert.AreEqual(expected, parser.ToHtml(input));
        }

        [TestMethod]
        public void GreedyAttributeProcessing_ConsumesAllTokensForAttributeValue()
        {
            var parser = new BBCodeParser(BBCodeTestUtil.ErrorFreeParsing, null, new[]
            {
                new BBTag("quote", "<div><span>Posted by ${name}</span>", "</div>",
                    new BBAttribute("name", "")) { GreedyAttributeProcessing = true }
            });

            var input = "[quote=Test User With Spaces]Here is my comment[/quote]";
            var expected = "<div><span>Posted by Test User With Spaces</span>Here is my comment</div>";

            Assert.AreEqual(expected, parser.ToHtml(input));
        }

        [TestMethod]
        public void NewlineTrailingOpeningTagIsIgnored()
        {
            var parser = new BBCodeParser(BBCodeTestUtil.ErrorFreeParsing, null, new[] { new BBTag("code", "<pre>", "</pre>") });

            var input = "[code]\nHere is some code[/code]";
            var expected = "<pre>Here is some code</pre>"; // No newline after the opening PRE

            Assert.AreEqual(expected, parser.ToHtml(input));
        }

        [TestMethod]
        public void SuppressFirstNewlineAfter_StopsFirstNewlineAfterClosingTag()
        {
            var parser = new BBCodeParser(BBCodeTestUtil.ErrorFreeParsing, null, new[] { new BBTag("code", "<pre>", "</pre>"){ SuppressFirstNewlineAfter = true } });

            var input = "[code]Here is some code[/code]\nMore text!";
            var expected = "<pre>Here is some code</pre>More text!"; // No newline after the closing PRE

            Assert.AreEqual(expected, parser.ToHtml(input));
        }


        [TestMethod]
        public void ToHTML_Method_Should_Return_Only_Tag_Without_Child_Nodes_When_AutoRenderContent_is_False()
        {
            var tags = new List<BBTag>()
            {
                new BBTag("b", "<strong>", "</strong>", autoRenderContent: false, requireClosingTag: true)
            };
            var parser = new BBCodeParser(tags);

            var actual = parser.ToHtml(
                "[b]" +
                    "[s]" +
                        "This is my nested elements." +
                    "[/s]" +
                "[/b]");

            Assert.AreEqual("<strong></strong>", actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToHTML_Method_Should_Throw_ArgumentNullException_When_Null_Passed()
        {
            var parser = new BBCodeParser(new List<BBTag>());

            parser.ToHtml(null);
        }



        [TestMethod]
        public void ToBBCode_Method()
        {
            var bbTags = new List<BBTag>()
            {
                new BBTag("b", "<strong>", "</strong>"),
                new BBTag("i", "<em>", "</em>"),
                new BBTag("u", "<span style=\"text-decoration: line-through\">", "</span>"),

                new BBTag("list", "<ul>", "</ul>") { SuppressFirstNewlineAfter = true },
                new BBTag("li", "<li>", "</li>", true, false),

                new BBTag("url", "<a href=\"${url}\">", "</a>",
                    new BBAttribute("url", "")),

                new BBTag("code", "<pre class=\"prettyprint\">", "</pre>")
                {
                    StopProcessing = true,
                    SuppressFirstNewlineAfter = true
                },
            };
            var parser = new BBCodeParser(bbTags);
            string input =
                "<strong>" +
                    "<em>Hi!</em>" +
                    "Do You ever wished for something and it <strong><span style=\"text-decoration: line-through\">came true</span></strong>?\n" +
                    "<pre class=\"prettyprint\">\n" +
                        "var settings = new JsonSerializerSettings\n" +
                        "{\n" +
                        "    Converters = { new LikeTypeConverter() },\n" +
                        "    ContractResolver = new CamelCasePropertyNamesContractResolver()\n" +
                        "};\n" +
                        "var result = JsonConvert.SerializeObject(myObject, Formatting.Indented, settings);\n" +
                    "</pre>\n" +
                "</strong>";

            string expected =
                "[b]" +
                    "[i]Hi![/i]" +
                    "Do You ever wished for something and it [b][u]came true[u][/b]\n" +
                    "[code]\n" +
                        "var settings = new JsonSerializerSettings\n" +
                        "{\n" +
                        "    Converters = { new LikeTypeConverter() },\n" +
                        "    ContractResolver = new CamelCasePropertyNamesContractResolver()\n" +
                        "};\n" +
                        "var result = JsonConvert.SerializeObject(myObject, Formatting.Indented, settings);\n" +
                    "[/code]\n" +
                "[b]";


            string actual = parser.ToBBCode(input);


            Assert.AreEqual(expected, actual);
        }
    }
}

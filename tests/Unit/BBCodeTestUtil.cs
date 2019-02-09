using System;
using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode.SyntaxTree;
using NUnit.Framework;

namespace CodeKicker.BBCode.Tests.Unit
{
    public static class BBCodeTestUtil
    {
        internal static readonly Random Random;
        internal static readonly ErrorFreeParsing ErrorFreeParsing;
        internal static readonly ErrorCorrectorParsing ErrorCorrectorParsing;
        internal static readonly StrictParsing StrictParsing;



        static BBCodeTestUtil()
        {
            Random = new Random();

            ErrorFreeParsing = new ErrorFreeParsing();
            ErrorCorrectorParsing = new ErrorCorrectorParsing();
            StrictParsing = new StrictParsing();
        }



        public static SequenceNode CreateRootNode(BBTag[] allowedTags)
        {
            var parentNode = new SequenceNode();

            AddSubNodes(allowedTags, parentNode, maxChildNodes: 3);

            return parentNode;
        }

        public static BBCodeParser GetParserForTest(IExceptions errorMode, bool includePlaceholder, BBTagClosingStyle listItemBBTagClosingStyle, bool enableIterationElementBehavior)
        {
            BBTag placeHolderTag = null;

            if (includePlaceholder)
            {
                placeHolderTag = new BBTag("placeholder", "${name}", "", false, BBTagClosingStyle.LeafElementWithoutContent, null,
                                        new BBAttribute("name", "", name => "xxx" + name.AttributeValue + "yyy"));
            }

            return new BBCodeParser(errorMode, null, new[]
            {
                new BBTag("b", "<b>", "</b>"),
                new BBTag("i", "<span style=\"font-style:italic;\">", "</span>"),
                new BBTag("u", "<span style=\"text-decoration:underline;\">", "</span>"),
                new BBTag("code", "<pre class=\"prettyprint\">", "</pre>"),
                new BBTag("img", "<img src=\"${content}\" />", "", false, true),
                new BBTag("quote", "<blockquote>", "</blockquote>"),
                new BBTag("list", "<ul>", "</ul>"),
                new BBTag("*", "<li>", "</li>", true, listItemBBTagClosingStyle, null, enableIterationElementBehavior),
                new BBTag("url", "<a href=\"${href}\">", "</a>", new BBAttribute("href", ""), new BBAttribute("href", "href")),
                new BBTag("url2", "<a href=\"${href}\">", "</a>",
                    new BBAttribute("href", "", GetUrl2Href),
                    new BBAttribute("href", "href", GetUrl2Href)),
                placeHolderTag,
            }.Where(tag => tag != null).ToArray());
        }

        public static BBCodeParser GetSimpleParserForTest(IExceptions errorMode)
        {
            return new BBCodeParser(errorMode, null, new[]
            {
                new BBTag("x", "${content}${x}", "${y}", true, true,
                    new BBAttribute("x", "x"),
                    new BBAttribute("y", "y", x => x.AttributeValue)),
            });
        }

        public static string SimpleBBEncodeForTest(string bbCode, IExceptions errorMode)
        {
            return GetSimpleParserForTest(errorMode).ToHtml(bbCode);
        }

        public static bool IsValid(string bbCode, IExceptions errorMode)
        {
            try
            {
                BBCodeParserTest.BBEncodeForTest(bbCode, errorMode);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static SequenceNode GetAnyTree()
        {
            var parser = GetParserForTest(new StrictParsing(), true, RandomTagClosingStyle(), false);

            return CreateRootNode(parser.Tags.ToArray());
        }


        private static void AddSubNodes(BBTag[] allowedTags, SyntaxTreeNode parentNode, int maxChildNodes)
        {
            int subNodeCount = Random.Next(0, maxChildNodes);

            for (int i = 0; i < subNodeCount; i++)
            {
                var subNode = CreateNode(allowedTags, allowText: true, maxChildNodes: maxChildNodes);

                parentNode.SubNodes.Add(subNode);
            }
        }

        private static SyntaxTreeNode CreateNode(BBTag[] allowedTags, bool allowText, int maxChildNodes)
        {
            int randomBranch = Random.Next(allowText ? 0 : 1, 2);

            if (0 == randomBranch)
            {
                string text = DateTime.Now.ToString();
                Assert.IsFalse(string.IsNullOrEmpty(text));
                return new TextNode(text);
            }
            else if (1 == randomBranch)
            {
                var bbTag = allowedTags[Random.Next(0, allowedTags.Length)];
                var tagNode = new TagNode(bbTag);

                AddSubNodes(allowedTags, tagNode, maxChildNodes: 3);

                if (bbTag.Attributes != null)
                {
                    var selectedIds = new List<string>();
                    foreach (var attr in bbTag.Attributes)
                    {
                        if (!selectedIds.Contains(attr.ID) && (DateTime.Now.Second % 2 == 0))
                        {
                            string val = DateTime.Now.ToString();

                            Assert.IsTrue(val != null);
                            Assert.IsTrue(val.IndexOfAny("[] ".ToCharArray()) == -1);

                            tagNode.AttributeValues[attr] = val;
                            selectedIds.Add(attr.ID);
                        }
                    }
                }
                return tagNode;
            }
            else
            {
                Assert.Fail($"BBCodeTestUtil.CreateNode() was not 0 or 1 but {randomBranch}");
                return null;
            }
        }

        private static string GetUrl2Href(IAttributeRenderingContext attributeRenderingContext)
        {
            if (!string.IsNullOrEmpty(attributeRenderingContext.AttributeValue))
                return attributeRenderingContext.AttributeValue;

            string content = attributeRenderingContext.GetAttributeValueByID(BBTag.ContentPlaceholderName);

            if (!string.IsNullOrEmpty(content) && content.StartsWith("http:"))
                return content;

            return null;
        }

        private static BBTagClosingStyle RandomTagClosingStyle()
        {
            Array values = Enum.GetValues(typeof(BBTagClosingStyle));
            return (BBTagClosingStyle)values.GetValue(Random.Next(values.Length));
        }
    }
}
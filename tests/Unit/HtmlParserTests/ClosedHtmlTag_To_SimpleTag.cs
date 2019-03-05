using CodeKicker.BBCode.Exceptions;
using CodeKicker.BBCode.HtmlComponents;
using CodeKicker.BBCode.Tags.BB;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tests.Unit.HtmlParserTests
{
    class ClosedHtmlTag_To_SimpleTag
    {
        private static readonly ExceptionMessages _exceptionMessage;
        private static readonly object[] _notClosed;



        static ClosedHtmlTag_To_SimpleTag()
        {
            _exceptionMessage = new ExceptionMessages();

            _notClosed = new object[]
            {
                new object[] { "<div>", "div", 1 },
                new object[] { "<div></div> <div>text", "div", 13 },
                new object[] { "<div></div> <div>text <div></div>", "div", 13 },
            };
        }



        [TestCaseSource(nameof(_notClosed))]
        public void HTML_Tag_Not_Closed(string input, string tagName, int index)
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom(tagName)
                    .ParseTo(new SimpleTag("block"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            Assert.That(() => parser.ToBBCode(input),
                Throws.TypeOf<ParserException>()
                .With
                .Message.EqualTo(_exceptionMessage.TagNotClosed(tagName, index)));
        }

        [Test]
        public void HTML_Without_Attribute_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div></div>");


            Assert.AreEqual("[div][/div]", actual);
        }

        [Test]
        public void HTML_With_Text_And_Without_Attribute_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div>text</div>");


            Assert.AreEqual("[div]text[/div]", actual);
        }

        [Test]
        public void HTML_With_One_Attribute_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .WithA(new Attribute("style"))
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div style=\"color:red;\"></div>");


            Assert.AreEqual("[div]color:red;[/div]", actual);
        }

        [Test]
        public void HTML_With_Text_And_With_One_Attribute_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .WithA(new Attribute("style"))
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div style=\"color:red;\">text</div>");


            Assert.AreEqual("[div]textcolor:red;[/div]", actual);
        }

        [Test]
        public void HTML_With_Two_Attributes_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .WithA(new Attribute("class"))
                    .WithA(new Attribute("style"))
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div class=\"bold\" style=\"color:red;\"></div>");


            Assert.AreEqual("[div]boldcolor:red;[/div]", actual);
        }

        [Test]
        public void HTML_With_Two_Attributes_And_With_Text_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .WithA(new Attribute("class"))
                    .WithA(new Attribute("style"))
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div class=\"bold\" style=\"color:red;\">text</div>");


            Assert.AreEqual("[div]textboldcolor:red;[/div]", actual);
        }

        [Test]
        public void Attributes_Definition_Order_Counts()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .WithA(new Attribute("style"))
                    .WithA(new Attribute("class"))
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div class=\"bold\" style=\"color:red;\"></div>");


            Assert.AreEqual("[div]color:red;bold[/div]", actual);
        }

        [Test]
        public void HTML_With_One_Skipped_Attribute_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .SkipAttribute("class")
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div class=\"bold\"></div>");


            Assert.AreEqual("[div][/div]", actual);
        }

        [Test]
        public void HTML_With_One_Skipped_Attribute_And_With_Text_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .SkipAttribute("class")
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div class=\"bold\">text</div>");


            Assert.AreEqual("[div]text[/div]", actual);
        }

        [Test]
        public void HTML_With_One_Attribute_And_One_Skipped_Attribute_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .WithA(new Attribute("class"))
                    .SkipAttribute("style")
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div class=\"bold\" style=\"color:red;\"></div>");


            Assert.AreEqual("[div]bold[/div]", actual);
        }

        [Test]
        public void SkipAttribute_Can_Be_In_Any_Order()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("div")
                    .SkipAttribute("style")
                    .WithA(new Attribute("class"))
                    .SkipAttribute("id")
                    .ParseTo(new SimpleTag("div"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div id=\"container\" class=\"bold\" style=\"color:red;\"></div>");


            Assert.AreEqual("[div]bold[/div]", actual);
        }
    }
}

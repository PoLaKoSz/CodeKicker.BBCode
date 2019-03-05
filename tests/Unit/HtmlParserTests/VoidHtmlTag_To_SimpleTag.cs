using CodeKicker.BBCode.HtmlComponents;
using CodeKicker.BBCode.Tags.BB;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tests.Unit.HtmlParserTests
{
    class VoidHtmlTag_To_SimpleTag
    {
        [Test]
        public void HTML_Without_Attribute_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                VoidHtmlTag.CreateFrom("img")
                    .ParseTo(new SimpleTag("img"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<img>");


            Assert.AreEqual("[img][/img]", actual);
        }

        [Test]
        public void HTML_Without_Attribute_And_Text_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                VoidHtmlTag.CreateFrom("img")
                    .ParseTo(new SimpleTag("img"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("text <img>text");


            Assert.AreEqual("text [img][/img]text", actual);
        }

        [Test]
        public void HTML_With_One_Attribute_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                VoidHtmlTag.CreateFrom("img")
                    .WithA(new Attribute("src"))
                    .ParseTo(new SimpleTag("img"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<img src=\"a.jpg\">");


            Assert.AreEqual("[img]a.jpg[/img]", actual);
        }

        [Test]
        public void HTML_With_Two_Attributes_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                VoidHtmlTag.CreateFrom("img")
                    .WithA(new Attribute("src"))
                    .WithA(new Attribute("style"))
                    .ParseTo(new SimpleTag("img"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<img src=\"a.jpg\" style=\"bold\">");


            Assert.AreEqual("[img]a.jpgbold[/img]", actual);
        }

        [Test]
        public void Attributes_Definition_Order_Counts()
        {
            var tags = new List<HtmlTag>()
            {
                VoidHtmlTag.CreateFrom("img")
                    .WithA(new Attribute("style"))
                    .WithA(new Attribute("src"))
                    .ParseTo(new SimpleTag("img"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<img src=\"a.jpg\" style=\"bold\">");


            Assert.AreEqual("[img]bolda.jpg[/img]", actual);
        }

        [Test]
        public void HTML_With_One_Attribute_And_One_Skipped_Attribute_To_BBCode()
        {
            var tags = new List<HtmlTag>()
            {
                VoidHtmlTag.CreateFrom("img")
                    .WithA(new Attribute("src"))
                    .SkipAttribute("style")
                    .ParseTo(new SimpleTag("img"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<img src=\"a.jpg\" style=\"bold\">");


            Assert.AreEqual("[img]a.jpg[/img]", actual);
        }

        [Test]
        public void SkipAttribute_Can_Be_In_Any_Order()
        {
            var tags = new List<HtmlTag>()
            {
                VoidHtmlTag.CreateFrom("img")
                    .SkipAttribute("style")
                    .WithA(new Attribute("src"))
                    .SkipAttribute("alt")
                    .ParseTo(new SimpleTag("img"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<img alt=\"\" src=\"a.jpg\" style=\"bold\">");


            Assert.AreEqual("[img]a.jpg[/img]", actual);
        }
    }
}

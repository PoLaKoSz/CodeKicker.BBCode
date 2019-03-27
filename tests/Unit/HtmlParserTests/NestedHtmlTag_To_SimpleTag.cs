using CodeKicker.BBCode.HtmlComponents;
using CodeKicker.BBCode.Tags.BB;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tests.Unit.HtmlParserTests
{
    class NestedHtmlTag_To_SimpleTag
    {
        private static readonly ExceptionMessages _exceptionMessage;



        static NestedHtmlTag_To_SimpleTag()
        {
            _exceptionMessage = new ExceptionMessages();
        }



        [Test]
        public void No_Nested_Tag_Found_To_BBCode()
        {
            Assert.Warn("This input should be tested with different parsing mode!");
            Assert.Fail("Refactoring");

            //var tags = NestedHtmlTag.CreateFrom("div")
            //        .WithChild(new VoidHtmlTag("img"))
            //        .ParseTo(new SimpleTag("block"));

            //var parser = new HtmlParser(tags);


            //string actual = parser.ToBBCode("<span></span>");



            //Assert.AreEqual("<span></span>", actual);
        }

        [Test]
        public void Nested_Tag_Child_Colliding_With_An_Another_Rule()
        {
            Assert.Fail("Refactoring");

            //var tags = new List<HtmlTag>()
            //{
            //    ClosedHtmlTag.CreateFrom("strong")
            //        .ParseTo(new SimpleTag("b"))
            //};
            //tags.AddRange(
            //    NestedHtmlTag.CreateFrom("div")
            //        .WithChild(new ClosedHtmlTag("strong"))
            //            .AsA(new Attribute("child"))
            //        .ParseTo(new ValueTag("quote")));

            //var parser = new HtmlParser(tags);


            //string actual = parser.ToBBCode(
            //    "<div>" +
            //        "<strong>strong-child</strong>" +
            //    "</div>" +
            //    "<strong>non div child</strong>");


            //Assert.AreEqual("[quote=strong-child][/quote][b]non div child[/b]", actual);
        }

        [Test]
        public void Nested_Tag_Child_Colliding()
        {
            var tags = new List<HtmlTag>()
            {
                NestedHtmlTag.CreateFrom("div")
                    .WithChild(new ClosedHtmlTag("strong"))
                        .AsA(new Attribute("child"))
                    .ParseTo(new ValueTag("quote"))
            };

            var parser = new HtmlParser(tags);


            string actual = parser.ToBBCode(
                "<div>" +
                    "<strong>strong-child</strong>" +
                "</div>" +
                "<strong>non div child</strong>");


            Assert.AreEqual("[quote=strong-child][/quote]<strong>non div child<strong>", actual);
        }
    }
}

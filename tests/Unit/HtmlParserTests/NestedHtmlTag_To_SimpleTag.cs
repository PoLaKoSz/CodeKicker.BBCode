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
            var tags = NestedHtmlTag.CreateFrom("div")
                    .WithChild(new VoidHtmlTag("img"))
                    .ParseTo(new SimpleTag("block"));

            var parser = new HtmlParser(tags);


            string actual = parser.ToBBCode("<span></span>");


            Assert.AreEqual("<span></span>", actual);
        }
    }
}

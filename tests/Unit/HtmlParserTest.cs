using CodeKicker.BBCode.HtmlComponents;
using CodeKicker.BBCode.Tags.BB;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tests.Unit
{
    public class HtmlParserTest
    {
        [Test]
        public void Constructor_Null_As_Tag_Rules()
        {
            Assert.Throws<ArgumentNullException>(() => new CodeKicker.BBCode.HtmlParser(null));
        }






        [Test]
        public void Simple_Nested_Tags()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("strong")
                    .ParseTo(new SimpleTag("b")),

                ClosedHtmlTag.CreateFrom("em")
                    .ParseTo(new SimpleTag("i")),

                ClosedHtmlTag.CreateFrom("span")
                    .SkipAttribute("style")
                    .ParseTo(new SimpleTag("u")),

                ClosedHtmlTag.CreateFrom("pre")
                    .SkipAttribute("class")
                    .ParseTo(new CodeTag())
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);
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
                    "Do You ever wished for something and it [b][u]came true[/u][/b]?\n" +
                    "[code]\n" +
                        "var settings = new JsonSerializerSettings\n" +
                        "{\n" +
                        "    Converters = { new LikeTypeConverter() },\n" +
                        "    ContractResolver = new CamelCasePropertyNamesContractResolver()\n" +
                        "};\n" +
                        "var result = JsonConvert.SerializeObject(myObject, Formatting.Indented, settings);\n" +
                    "[/code]\n" +
                "[/b]";


            string actual = parser.ToBBCode(input);


            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Bold_Text()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("b")
                    .ParseTo(new SimpleTag("b"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<b>Hi there!</b>");


            Assert.AreEqual("[b]Hi there![/b]", actual);
        }

        [Test]
        public void Italic_Text()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("em")
                    .ParseTo(new SimpleTag("i"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<em>Hi there!</em>");


            Assert.AreEqual("[i]Hi there![/i]", actual);
        }
        
        [Test]
        public void UnderLine_Text()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("span")
                    .SkipAttribute("style")
                    .ParseTo(new SimpleTag("u"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<span style=\"text-decoration:underline\">Hi there!</span>");


            Assert.AreEqual("[u]Hi there![/u]", actual);
        }
        
        [Test]
        public void Font_Size()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("span")
                    .WithA(new Attribute("style", value => value.Substring("font-size:".Length, 4)))
                    .ParseTo(new ValueTag("size"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<span style=\"font-size:14px;\">Hi there!</span>");


            Assert.AreEqual("[size=14px]Hi there![/size]", actual);

        }

        [Test]
        public void Font_Color()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("span")
                    .WithA(new Attribute("style", value => {
                        value = value.Substring("color:".Length);
                        value = value.Substring(0, value.IndexOf(';'));

                        if (value.StartsWith("rgb("))
                        {
                            string[] parts = value.Substring(0, value.Length - 1).Substring(4).Split(',');
                            int r = Convert.ToInt32(parts[0]);
                            int g = Convert.ToInt32(parts[1]);
                            int b = Convert.ToInt32(parts[2]);
                            value = string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
                        }

                        return value;
                    }))
                    .ParseTo(new ValueTag("color"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<span style=\"color:red;\">red</span>, <span style=\"color:green;\">green</span>, <span style=\"color:rgb(255,157,36);\">custom</span>");


            Assert.AreEqual("[color=red]red[/color], [color=green]green[/color], [color=#FF9D24]custom[/color]", actual);
        }

        [Test]
        public void Named_Quote()
        {
            var tags = 
                NestedHtmlTag.CreateFrom("div")
                    .WithChild(new ClosedHtmlTag("strong"))
                        .AsA(new Attribute("author", a => a.Substring(0, a.IndexOf(" wrote:"))))
                    .SkipAttribute("class")
                    .ParseTo(new ValueTag("quote"));

            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div class=\"quote\"><strong>JohnF.Kennedy wrote:</strong>Those who dare to fail miserably can achieve greatly.</div>");


            Assert.AreEqual("[quote=JohnF.Kennedy]Those who dare to fail miserably can achieve greatly.[/quote]", actual);
        }

        [Test]
        public void Named_Quote_With_Space()
        {
            var tags = NestedHtmlTag.CreateFrom("div")
                    .WithChild(new ClosedHtmlTag("strong"))
                        .AsA(new Attribute("author", a => a.Substring(0, a.IndexOf(" wrote:"))))
                    .SkipAttribute("class")
                    .ParseTo(new ValueTag("quote"));

            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div class=\"quote\"><strong>Stephen King wrote:</strong>Get busy living or get busy dying.</div>");


            Assert.AreEqual("[quote=Stephen King]Get busy living or get busy dying.[/quote]", actual);
        }

        [Test]
        public void Named_Quote_With_Other_Attributes()
        {
            var tags =
                NestedHtmlTag.CreateFrom("div")
                    .WithA(new Attribute("class"))
                    .WithChild(new ClosedHtmlTag("strong"))
                        .AsA(new Attribute("author", a => a.Substring(0, a.IndexOf(" wrote:"))))
                    .WithChild(new ClosedHtmlTag("span"))
                        .AsA(new Attribute("date"))
                    .SkipAttribute("class")
                    .ParseTo(new ParameterizedTag("quote",
                                new AttributeGenerator()
                                    .KeyValue("author")
                                    .KeyValue("date")));

            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<div class=\"quote\"><strong>JohnF.Kennedy wrote:</strong><span>2019-02-28</span>Those who dare to fail miserably can achieve greatly.</div>");


            Assert.AreEqual("[quote author=\"JohnF.Kennedy\" date=\"2019-02-28\"]Those who dare to fail miserably can achieve greatly.[/quote]", actual);
        }

        [Test]
        public void Link()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("a")
                    .WithA(new Attribute("href"))
                    .SkipTextNode("Click me!")
                    .ParseTo(new SimpleTag("url"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<a href=\"https://polakosz.hu/\">Click me!</a>");


            Assert.AreEqual("[url]https://polakosz.hu/[/url]", actual);

        }
        
        [Test]
        public void Link_With_Name()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("a")
                    .WithA(new Attribute("href"))
                    .ParseTo(new ValueTag("url"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<a href=\"https://polakosz.hu/\">Click me!</a>");


            Assert.AreEqual("[url=https://polakosz.hu/]Click me![/url]", actual);

        }

        [Test]
        public void Image()
        {
            var tags = new List<HtmlTag>()
            {
                new VoidHtmlTag("img", new SimpleTag("img"), new Attribute[]
                {
                    new Attribute("src"),
                })
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<img src=\"https://www.bbcode.org/images/lubeck_small.jpg\" alt=\"\">");


            Assert.AreEqual("[img]https://www.bbcode.org/images/lubeck_small.jpg[/img]", actual);
        }

        [Test]
        public void Image_With_Modified_Size()
        {
            var tags = new List<HtmlTag>()
            {
                VoidHtmlTag.CreateFrom("img")
                    .WithA(new Attribute("src"))
                    .WithA(new Attribute("width"))
                    .WithA(new Attribute("height"))
                    .SkipAttribute("alt")
                    .ParseTo(ValueTag.New("img", attrValueTemplate: "${width}x${height}")
                                .WithTextNodeFrom("src"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<img src=\"https://www.bbcode.org/images/lubeck_small.jpg\" alt=\"\" width=\"100\" height=\"50\">");


            Assert.AreEqual("[img=100x50]https://www.bbcode.org/images/lubeck_small.jpg[/img]", actual);
        }

        [Test]
        public void Image_With_Modified_Size_And_With_Meta_Data()
        {
            var tags = new List<HtmlTag>()
            {
                VoidHtmlTag.CreateFrom("img")
                    .WithA(new Attribute("src"))
                    .WithA(new Attribute("width"))
                    .WithA(new Attribute("alt"))
                    .WithA(new Attribute("title"))
                    .WithA(new Attribute("height"))
                    .ParseTo(new ParameterizedTag("img",
                        new AttributeGenerator()
                            .KeyValue("width")
                            .KeyValue("height")
                            .KeyValue("alt")
                            .KeyValue("title")))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<img alt=\"Lubeck city gate\" title=\"This is one of the medieval city gates of Lubeck\" src=\"https://www.bbcode.org/images/lubeck_small.jpg\" width=\"100\" height=\"50\">");


            Assert.AreEqual("[img width=\"100\" height=\"50\" alt=\"Lubeck city gate\" title=\"This is one of the medieval city gates of Lubeck\"]https://www.bbcode.org/images/lubeck_small.jpg[/img]", actual);
        }

        [Test]
        public void List_Ordered()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("ol")
                    .ParseTo(new SimpleTag("ol")),
                ClosedHtmlTag.CreateFrom("li")
                    .ParseTo(new SimpleTag("li"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode(
                "<ol>" +
                    "<li>Item one</li>" +
                    "<li>Item two</li>" +
                "</ol>");


            Assert.AreEqual(
                "[ol]" +
                    "[li]Item one[/li]" +
                    "[li]Item two[/li]" +
                "[/ol]", actual);
        }
        
        [Test]
        public void List_UnOrdered_With_UL_Tag()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("ul")
                    .ParseTo(new SimpleTag("ul")),
                ClosedHtmlTag.CreateFrom("li")
                    .ParseTo(new SimpleTag("li"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode(
                "<ul>" +
                    "<li>Item one</li>" +
                    "<li>Item two</li>" +
                "</ul>");


            Assert.AreEqual(
                "[ul]" +
                    "[li]Item one[/li]" +
                    "[li]Item two[/li]" +
                "[/ul]", actual);
        }

        [Test]
        public void List_UnOrdered_With_List_BB_Tag()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("ul")
                    .ParseTo(new SimpleTag("list")),
                ClosedHtmlTag.CreateFrom("li")
                    .ParseTo(new SimpleTag("li"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode(
                "<ul>" +
                    "<li>Item one</li>" +
                    "<li>Item two</li>" +
                "</ul>");


            Assert.AreEqual(
                "[list]" +
                    "[li]Item one[/li]" +
                    "[li]Item two[/li]" +
                "[/list]", actual);
        }

        [Test]
        public void Code()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("code")
                    .ParseTo(new SimpleTag("code"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode(
                "<code>" +
                    "echo \"hello world\";" +
                "</code>");


            Assert.AreEqual(
                "[code]echo \"hello world\";[/code]", actual);
        }

        [Test]
        public void Code_Keep_WhiteSpaces()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("code")
                    .ParseTo(new CodeTag())
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode(
                "<code>" +
                    " ; ;" +
                "</code>");


            Assert.AreEqual(
                "[code] ; ;[/code]", actual);
        }

        [Test]
        public void Table_With_Content()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("table")
                    .ParseTo(new SimpleTag("table")),
                ClosedHtmlTag.CreateFrom("tr")
                    .ParseTo(new SimpleTag("tr")),
                ClosedHtmlTag.CreateFrom("th")
                    .ParseTo(new SimpleTag("th")),
                ClosedHtmlTag.CreateFrom("td")
                    .ParseTo(new SimpleTag("td"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode(
                "<table>" +
                    "<tr>" +
                        "<th>Name</th>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>John Doe</td>" +
                    "</tr>" +
                "</table>");


            Assert.AreEqual(
                "[table]" +
                    "[tr]" +
                        "[th]Name[/th]" +
                    "[/tr]" +
                    "[tr]" +
                        "[td]John Doe[/td]" +
                    "[/tr]" +
                "[/table]", actual);
        }

        [Test]
        public void YouTube()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("iframe")
                    .WithA(new Attribute("src", v => v.Substring("https://www.youtube.com/embed/".Length, "6jP0z4Z3M98".Length)))
                    .SkipAttribute("width")
                    .SkipAttribute("height")
                    .SkipAttribute("frameborder")
                    .SkipAttribute("allow")
                    .SkipAttribute("allowfullscreen")
                    .ParseTo(new SimpleTag("youtube"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/6jP0z4Z3M98?controls=0\" frameborder=\"0\" allow=\"accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>");


            Assert.AreEqual("[youtube]6jP0z4Z3M98[/youtube]", actual);
        }

        [Test]
        public void Same_HtmlTag_Rule_With_Different_Attribute_Value()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("span")
                    .WithA(new ExactAttribute("style", "text-decoration: underline;"))
                    .ParseTo(new SimpleTag("u")),

                ClosedHtmlTag.CreateFrom("span")
                    .WithA(new ExactAttribute("style", "font-style: italic;"))
                    .ParseTo(new SimpleTag("i"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode(
                "<span style=\"text-decoration: underline;\">Underlined</span>" +
                "<span style=\"font-style: italic;\">Italic</span>");


            Assert.AreEqual(
                "[u]Underlined[/u]" +
                "[i]Italic[/i]",
                actual);
        }

        [Test]
        public void Nested_Tag_Child_Colliding_With_An_Another_Rule()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("strong")
                    .ParseTo(new SimpleTag("b"))
            };
            tags.AddRange(
                NestedHtmlTag.CreateFrom("div")
                    .WithChild(new ClosedHtmlTag("strong"))
                        .AsA(new Attribute("strong"))
                    .ParseTo(new ValueTag("quote")));

            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode(
                "<div>" +
                    "<strong>div-child</strong>" +
                "</div>" +
                "<strong>non div child</strong>");


            Assert.AreEqual("[quote=div-child][/quote][b]non div child[/b]", actual);
        }

        [Test]
        public void Tag_With_Attr_Not_Exists_In_Input()
        {
            var tags = new List<HtmlTag>()
            {
                ClosedHtmlTag.CreateFrom("strong")
                    .WithA(new Attribute("style"))
                    .ParseTo(new SimpleTag("b"))
            };
            var parser = new CodeKicker.BBCode.HtmlParser(tags);


            string actual = parser.ToBBCode("<strong>(...)</strong>");


            Assert.AreEqual("<strong>(...)</strong>", actual);
        }

        // TODO : NestedHtmlTag
            // First and Last children will be attrs and the attrs order are the same in the BB Code tag
            // First and Last children will be attrs and the attrs order are the opposite in the BB Code tag
    }
}

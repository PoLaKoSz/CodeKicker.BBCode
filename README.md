# CodeKicker.BBCode

Codekicker.BBCode is a stable and fast BBCode-Parser for .NET. It can transform any BBCode into HTML or into an in-memory
syntax tree that can be analyzed or translated. All tags are fully customizable, nothing is hardcoded.

### The design goals of Codekicker.BBCode were

- Stable - it never ever crashes
- Safe - it does not let through unsafe HTML like `<script>` under any circumstances
- Performance - it is meant to be used in production
- Easy to use - Just call `BBCode.ToHtml("[url=http://example.com/]example.com[url]")`
- Customizable - Customizing the translation of every tag is easy and flexible
- Error messages - the messages are available in English and German. Language contributions are welcome.

# Getting started

There is no built-in parser rule but You can make your own in 10 sec, so let's start:
To make a parsing rule You will need to initialize a `new BBTag` class.
For instance if You have simple tag like [b]

````
[b]My bold text.[/b]
````

and You want to parse it to this

```` html
<strong>My bold text.</strong>
````

You only need to write

```` c#
new BBTag("b", "<strong>", "</strong>")
````

**But what if You need to extract data to a specific attribute into the HTML tag?**

## Attributes

Given this BBCode input

````
[url=https://bbcode.codeplex.com class=jsCanModifyThis]Official CodeKicker parser link[/url]
````

which should be parsed to this HTML code

```` html
<a href="https://bbcode.codeplex.com" class="jsCanModifyThis">Official CodeKicker parser link</a>
````

You need to pass `BBAttribute` objects into the `BBTag` constructor like this

```` c#
var attrs = new BBAttribute[]
{
    new BBAttribute("url", ""),
    new BBAttribute("className", "class")
};
new BBTag("url", "<a href=\"${url}\" class=\"${className}\">", "</a>", attrs),
````

to extract the link's URL and the class name from it.

## Parsing

Now that You can build custom parsing rules You only need to know how to perform an actual BBCode to HTML parsing.
First make a parser rule collection

```` c#
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
````

Initialize the parser

```` c#
var parser = new BBCodeParser(bbTags);
````

And execute the parser on a BBCode

```` c#
string output = parser.ToHtml(
    "[i]Not to be confused with [url=https://en.wikipedia.org/wiki/Text_formatting]Text formatting[/url]." +
    "\"Rich text\" redirects here. For text/richtext, see [url=https://en.wikipedia.org/wiki/Enriched_text]Enriched text.[/url][/i]" +

    "[list]" +
        "[li]Capitalization: I am NOT making this up.[/li]" +
        "[li]Surrounding with underscores: I am _not_ making this up.[/li]" +
        "[li]Surrounding with asterisks: I am *not* making this up.[/li]" +
        "[li]Spacing: I am n o t making this up.[/li]" +
    "[/list]");
````

And the output should be this (in one line)

```` html
<em>Not to be confused with <a href=\"https://en.wikipedia.org/wiki/Text_formatting\">Text formatting</a>.
"&quot;Rich text&quot; redirects here. For text/richtext, see <a href=\"https://en.wikipedia.org/wiki/Enriched_text\">Enriched text.</a></em>

<ul>
    <li>Capitalization: I am NOT making this up.</li>
    <li>Surrounding with underscores: I am _not_ making this up.</li>
    <li>Surrounding with asterisks: I am *not* making this up.</li>
    <li>Spacing: I am n o t making this up.</li>
</ul>
````

For more info check out the [Documentation](docs/index.md).

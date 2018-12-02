[Documentation](./index.md) / Components / BBCode to HTML

# BBCode to HTML

After You [initialized the parser](bbcodeparser.md) correctly the only thing You need to do is to call the
`ToHTL()` method on it:

```` c#
using CodeKicker.BBCode;
using System.Collections.Generic;


var parser = new BBCodeParser(new List<BBTag>()
{
    new BBTag("b", "<b>", "</b>")
});
string output = parser.ToHtml("[b]My bold text.[/b]");

Console.WriteLine(output);
// <b>My bold text.</b>
````

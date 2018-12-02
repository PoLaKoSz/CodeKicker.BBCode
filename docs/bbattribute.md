[Documentation](./index.md) / Components / BBAttribute

# BBAttribute

This class will define how a HTML tag's attributes can parsed to a BBCode tag's attribute and vice versa.

## Constructor parameters
| Type                                     | Name                                                                            | Null enabled |
|------------------------------------------|---------------------------------------------------------------------------------|:------------:|
| int                                      | [id](#int-id)                                                                   |              |
| string                                   | [name](#string-name)                                                            |              |
| Func<IAttributeRenderingContext, string> | [contentTransformer](#funciattributerenderingcontext-string-contenttransformer) |      Yes     |
| HtmlEncodingMode                         | [htmlEncodingMode](#htmlencodingmode-htmlencodingmode)                          |              |

## Example

``` c#
var bbTags = new List<BBTag>()
{
   new BBTag("font", "<div style=\"font-size: ${fontSize}\">", "</div>", new BBAttribute("fontSize", "size"))
};

var parser = new BBCodeParser(bbTags);

string output = parser.ToHtml("[font size=12px]Lorem ipsum[/font]");

// Output: <div style="font-size: 12px">Lorem ipsum</div>
```


#### int `id`
- The name of the attribute declared in the BBTag and surrounded with the `${}` symbol.

- In the example the `fontSize` is the ID of the attribute.

#### string `name`
- The name of the BB attribute.

- In the parsed example BBCode there is a `[font]` BB tag which has an attribute called `size`.

#### Func<IAttributeRenderingContext, string> `contentTransformer`
- Template how the attribute should be rendered in the parsed output.

#### HtmlEncodingMode `htmlEncodingMode`
- Sets how should be encoded in the parsing process.

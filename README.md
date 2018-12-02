# CodeKicker.BBCode

Codekicker.BBCode is a stable and fast BBCode-Parser for .NET. It can transform any BBCode into HTML or into an in-memory syntax tree that can be analyzed or translated. All tags are fully customizable, nothing is hardcoded.

### The design goals of Codekicker.BBCode were

- Stable - it never ever crashes
- Safe - it does not let through unsafe HTML like `<script>` under any circumstances
- Performance - it is meant to be used in production
- Easy to use - Just call `BBCode.ToHtml("[url=http://example.com/]example.com[url]")`
- Customizable - Customizing the translation of every tag is easy and flexible
- Error messages - the messages are available in English and German. Language contributions are welcome.

### Examples

| `[b]Hi![/b]`                                   | -> | `<strong>Hi!</strong>`                                      |
|------------------------------------------------|----|-------------------------------------------------------------|
| `[font size=12px]Lorem ipsum (...).[/font]`    | -> | `<font style=\"font-size: 12px\">Lorem ipsum (...).</font>` |
| `[url=http://codekicker.de/]Click here![/url]` | -> | `<a href=\"http://codekicker.de/\">Click here!</a>`         |

but You can create custom ones ... after You take a look at the [Documentation/BBCode -> HTML](docs/bbcode2html.md) section.

### Documentation

- [History of this project](docs/history.md)
- Components
  - [BBCodeParser](docs/bbcodeparser.md)
  - [BBTag](docs/bbtag.md)
  - [BBAttribute](docs/bbattribute.md)
  - [BBCode -> HTML](docs/bbcode2html.md)
  - [Exceptions](docs/exceptions.md)
- [Licence](docs/licence.md)

[Documentation](./index.md) / Components / BBCodeParser

# BBCodeParser

There are multiple ways to initialize the `BBCodeParser`:
- define only the [Tag](bbtag.md)s that You want to parse and use the default [Exception](exceptions.md) handling mechanism:

```` c#
public BBCodeParser(IList<BBTag> tags)
````

- set how the parser handle [Exceptions](exceptions.md) and the [Tag](bbtag.md)s that You want to parse:
```` c#
public BBCodeParser(ErrorMode errorMode, IList<BBTag> tags)
````

- define a custom template for TextNodes in the parsable string with the [Tag](bbtag.md)s that You want to parse and with the default [Exception](exceptions.md) handling mechanism:
```` c#
public BBCodeParser(string textNodeHtmlTemplate, IList<BBTag> tags)
````

- and the most advanced:
```` c#
public BBCodeParser(ErrorMode errorMode, string textNodeHtmlTemplate, IList<BBTag> tags)
````

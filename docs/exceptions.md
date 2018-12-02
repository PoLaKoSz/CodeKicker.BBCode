[Documentation](./index.md) / Components / Exceptions

# Exceptions

You can customize the parser how to handle Exceptions with the `CodeKicker.BBCode.ErrorMode` enum: 

| ErrorMode          | Description                                                                                         | Default |
|--------------------|-----------------------------------------------------------------------------------------------------|:-------:|
| ErrorFree          | The parser will never throw an exception. Invalid tags like "array[0]" will be interpreted as text. |    X    |
| TryErrorCorrection | Syntax errors with obvious meaning will be corrected automatically.                                 |         |
| Strict             | Every syntax error throws a BBCodeParsingException.                                                 |         |

## Examples

Use the default:
```` c#
using CodeKicker.BBCode;
using System.Collections.Generic;

var parser = new BBCodeParser(new List<BBTag>());
````

or set yourself one:
```` c#
using CodeKicker.BBCode;
using System.Collections.Generic;

var parser = new BBCodeParser(ErrorMode.Strict, new List<BBTag>());
````

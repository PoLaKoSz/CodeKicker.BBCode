[Documentation](./index.md) / Components / BBTag

# BBTag

This class will define how a HTML tag can parsed to a BBCode tag and vice versa.

## Constructor parameters
| Type                 | Name                                                                   | Null enabled |
|----------------------|------------------------------------------------------------------------|:------------:|
| string               | [name](#string-name)                                                   |              |
| string               | [openTagTemplate](#string-openTagTemplate)                             |              |
| string               | [closeTagTemplate](#string-closeTagTemplate)                           |              |
| bool                 | [autoRenderContent](#bool-autorendercontent)                           |              |
| bool                 | [requireClosingTag](#bool-requireclosingtag)                           |              |
| BBTagClosingStyle    | [tagClosingClosingStyle](#bbtagclosingstyle-tagclosingclosingstyle)    |      Yes     |
| Func<string, string> | [contentTransformer](#funcstring-string-contenttransformer)            |      Yes     |
| bool                 | [enableIterationElementBehavior](#bool-enableiterationelementbehavior) |              |
| params BBAttribute[] | [attributes](#params-bbattribute-attributes)                           |      Yes     |


#### string `name`
- The name of the BB tag.

- For example: *b* which will be parsed to *[b]*.

#### string `openTagTemplate`
- Template how the start tag implemented in HTML.

- For example:  *&lt;span&gt;*, *&lt;div&gt;*, etc.

#### string `closeTagTemplate`
- Template how the end tag implemented in HTML.

- For example: *<\/span>*, *<\/div>*, etc.

#### bool `autoRenderContent`
- 

#### bool `requireClosingTag`
- Indicates that this BBTag requires a closing tag or not.

- For example: &lt;img&gt; not.

#### BBTagClosingStyle `tagClosingClosingStyle`
- This property tells to the parser how to treat the tag's closing tag.

#### Func<string, string> `contentTransformer`
- Allows for custom modification of the tag content before rendering takes place.

#### bool `enableIterationElementBehavior`
- 

#### params BBAttribute[] `attributes`
- Attributes that should be extracted from the source.

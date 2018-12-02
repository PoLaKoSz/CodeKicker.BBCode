using System;

namespace CodeKicker.BBCode
{
    public class BBTag
    {
        /// <summary>
        /// The name of the BB tag. For ex.: b witch will be parsed to [b].
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Template how the start tag implemented in HTML.
        /// For ex.: &lt;span&gt;, &lt;div&gt;, etc.
        /// </summary>
        public string OpenTagTemplate { get; }

        /// <summary>
        /// Template how the end tag implemented in HTML.
        /// For ex.: &lt;/span&gt;, &lt;/div&gt;, etc.
        /// </summary>
        public string CloseTagTemplate { get; }

        /// <summary>
        /// When it's set to <c>TRUE</c> the child nodes should be parsed too.
        /// </summary>
        public bool AutoRenderContent { get; }

        /// <summary>
        /// Set to <c>TRUE</c> when You don't want to
        /// parse the child tags (for ex.: between
        /// &lt;pre&gt;bb code goes here&lt;/pre&gt;.
        /// </summary>
        public bool StopProcessing { get; set; }

        /// <summary>
        /// When it's set to <c>TRUE</c>, this will allow to use
        /// <see cref="BBAttribute"/> with spaces.
        /// </summary>
        public bool GreedyAttributeProcessing { get; set; }

        /// <summary>
        /// When it's set to <c>TRUE</c>, and this <see cref="BBTag"/>
        /// object followed by a \n it will be removed from the response.
        /// </summary>
        public bool SuppressFirstNewlineAfter { get; set; }

        public bool EnableIterationElementBehavior { get; set; }

        /// <summary>
        /// Indicates that this <see cref="BBTag"/> requires a closing tag or not.
        /// For ex.: &lt;img&gt; not.
        /// </summary>
        public bool RequiresClosingTag
        {
            get { return TagClosingStyle == BBTagClosingStyle.RequiresClosingTag; }
        }

        /// <summary>
        /// This property tells to the parser how to treat
        /// the tag's closing tag.
        /// </summary>
        public BBTagClosingStyle TagClosingStyle { get; }

        /// <summary>
        /// Allows for custom modification of the tag content before rendering takes place.
        /// </summary>
        public Func<string, string> ContentTransformer { get; }

        /// <summary>
        /// The <see cref="BBTag"/> important attributes.
        /// </summary>
        public BBAttribute[] Attributes { get; }

        /// <summary>
        /// When no custom format has provided this string will
        /// be placed to complete the parsing operation.
        /// </summary>
        public const string ContentPlaceholderName = "content";



        /// <summary>
        /// Initialize an instance which will autorender the content and
        /// require the closing tag.
        /// </summary>
        /// <param name="name">The name of the BB tag. For ex.: b witch will be parsed to [b]. Can not be null!</param>
        /// <param name="openTagTemplate">Template how the start tag implemented in HTML. Can not be null!
        /// For ex.: &lt;span&gt;, &lt;div&gt;, etc.</param>
        /// <param name="closeTagTemplate">Template how the end tag implemented in HTML. Can not be null!
        /// For ex.: &lt;/span&gt;, &lt;/div&gt;, etc.</param>
        /// <param name="attributes">Attributes that should be extracted from the source.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public BBTag(string name, string openTagTemplate, string closeTagTemplate, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, true, true, attributes) { }

        /// <summary>
        /// Initialize an instance without a content transformer.
        /// </summary>
        /// <param name="name">The name of the BB tag. For ex.: b witch will be parsed to [b]. Can not be null!</param>
        /// <param name="openTagTemplate">Template how the start tag implemented in HTML. Can not be null!
        /// For ex.: &lt;span&gt;, &lt;div&gt;, etc.</param>
        /// <param name="closeTagTemplate">Template how the end tag implemented in HTML. Can not be null!
        /// For ex.: &lt;/span&gt;, &lt;/div&gt;, etc.</param>
        /// <param name="autoRenderContent">Set to <c>TRUE</c> if this tag has child elements and
        /// You want to keep them in the parsed result.</param>
        /// <param name="requireClosingTag">Indicates that this <see cref="BBTag"/> requires a closing tag or not.
        /// For ex.: &lt;img&gt; not.</param>
        /// <param name="attributes">Attributes that should be extracted from the source.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, bool requireClosingTag, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, autoRenderContent, requireClosingTag, null, attributes) { }

        /// <summary>
        /// Initialize an instance.
        /// </summary>
        /// <param name="name">The name of the BB tag. For ex.: b witch will be parsed to [b]. Can not be null!</param>
        /// <param name="openTagTemplate">Template how the start tag implemented in HTML. Can not be null!
        /// For ex.: &lt;span&gt;, &lt;div&gt;, etc.</param>
        /// <param name="closeTagTemplate">Template how the end tag implemented in HTML. Can not be null!
        /// For ex.: &lt;/span&gt;, &lt;/div&gt;, etc.</param>
        /// <param name="autoRenderContent">Set to <c>TRUE</c> if this tag has child elements and
        /// You want to keep them in the parsed result.</param>
        /// <param name="requireClosingTag">Indicates that this <see cref="BBTag"/> requires a closing tag or not.
        /// For ex.: &lt;img&gt; not.
        /// <para>If set to <c>FALSE</c> the parser will autoclose the element.</para></param>
        /// <param name="contentTransformer">Allows for custom modification of the tag content before rendering takes place.</param>
        /// <param name="attributes">Attributes that should be extracted from the source.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, bool requireClosingTag, Func<string, string> contentTransformer, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, autoRenderContent, requireClosingTag ? BBTagClosingStyle.RequiresClosingTag : BBTagClosingStyle.AutoCloseElement, contentTransformer, attributes) { }

        /// <summary>
        /// Initialize an instance with EnableIterationElementBehavior property set to <c>FALSE</c>.
        /// </summary>
        /// <param name="name">The name of the BB tag. For ex.: b witch will be parsed to [b]. Can not be null!</param>
        /// <param name="openTagTemplate">Template how the start tag implemented in HTML. Can not be null!
        /// For ex.: &lt;span&gt;, &lt;div&gt;, etc.</param>
        /// <param name="closeTagTemplate">Template how the end tag implemented in HTML. Can not be null!
        /// For ex.: &lt;/span&gt;, &lt;/div&gt;, etc.</param>
        /// <param name="autoRenderContent">Set to <c>TRUE</c> if this tag has child elements and
        /// You want to keep them in the parsed result.</param>
        /// <param name="tagClosingClosingStyle">This property tells to the parser how to treat
        /// the tag's closing tag.</param>
        /// <param name="contentTransformer">Allows for custom modification of the tag content before rendering takes place.</param>
        /// <param name="attributes">Attributes that should be extracted from the source.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, BBTagClosingStyle tagClosingClosingStyle, Func<string, string> contentTransformer, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, autoRenderContent, tagClosingClosingStyle, contentTransformer, false, attributes) { }

        /// <summary>
        /// Initialize an instance.
        /// </summary>
        /// <param name="name">The name of the BB tag. For ex.: b witch will be parsed to [b]. Can not be null!</param>
        /// <param name="openTagTemplate">Template how the start tag implemented in HTML. Can not be null!
        /// For ex.: &lt;span&gt;, &lt;div&gt;, etc.</param>
        /// <param name="closeTagTemplate">Template how the end tag implemented in HTML. Can not be null!
        /// For ex.: &lt;/span&gt;, &lt;/div&gt;, etc.</param>
        /// <param name="autoRenderContent">Set to <c>TRUE</c> if this tag has child elements and
        /// You want to keep them in the parsed result.</param>
        /// <param name="tagClosingClosingStyle">This property tells to the parser how to treat
        /// the tag's closing tag.</param>
        /// <param name="contentTransformer">Allows for custom modification of the tag content before rendering takes place.</param>
        /// <param name="enableIterationElementBehavior"></param>
        /// <param name="attributes">Attributes that should be extracted from the source.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, BBTagClosingStyle tagClosingClosingStyle, Func<string, string> contentTransformer, bool enableIterationElementBehavior, params BBAttribute[] attributes)
        {
            if (!Enum.IsDefined(typeof(BBTagClosingStyle), tagClosingClosingStyle))
                throw new ArgumentException(nameof(tagClosingClosingStyle));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            OpenTagTemplate = openTagTemplate ?? throw new ArgumentNullException(nameof(openTagTemplate));
            CloseTagTemplate = closeTagTemplate ?? throw new ArgumentNullException(nameof(closeTagTemplate));
            AutoRenderContent = autoRenderContent;
            TagClosingStyle = tagClosingClosingStyle;
            ContentTransformer = contentTransformer;
            EnableIterationElementBehavior = enableIterationElementBehavior;
            Attributes = attributes ?? new BBAttribute[0];
        }



        /// <summary>
        /// Get the <see cref="BBAttribute"/> by it's name.
        /// </summary>
        /// <param name="name">Name property value of the <see cref="BBAttribute"/> class.</param>
        /// <returns>Reference class from the Attribute property.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public BBAttribute FindAttribute(string name)
        {
            return Array.Find(Attributes, a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
using System;

namespace CodeKicker.BBCode
{
    public class BBAttribute
    {
        /// <summary>
        /// Used to reference the attribute value in
        /// the parsing process.
        /// <para>Example:</para>
        /// &lt;img src="https://codekicker.de/${relativePath}.jpg" /&gt;
        /// <para>'relativePath' is the ID of the attribute.</para>
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Name is used during parsing.
        /// <para>Example:</para>
        /// [font size=12px] ... [/font]
        /// <para>'size' is the Name of the attribute.</para>
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Allows for custom modification of the attribute value before rendering takes place.
        /// </summary>
        public Func<IAttributeRenderingContext, string> ContentTransformer { get; private set; }

        /// <summary>
        /// Specifies how this attribute should be Encoded.
        /// </summary>
        public HtmlEncodingMode HtmlEncodingMode { get; set; }



        /// <summary>
        /// Initialize an instance with a HtmlAttributeEncode <see cref="HtmlEncodingMode"/>.
        /// </summary>
        /// <param name="id"><para>Example:</para>
        /// &lt;img src="https://codekicker.de/${relativePath}.jpg" /&gt;
        /// <para>'relativePath' is the ID of the attribute.</para></param>
        /// <param name="name"><para>Example:</para>
        /// [font size=12px] ... [/font]
        /// <para>'size' is the Name of the attribute.</para></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public BBAttribute(string id, string name)
            : this(id, name, null, HtmlEncodingMode.HtmlAttributeEncode) { }

        /// <summary>
        /// Initialize an instance with a HtmlAttributeEncode <see cref="HtmlEncodingMode"/>.
        /// </summary>
        /// <param name="id"><para>Example:</para>
        /// &lt;img src="https://codekicker.de/${relativePath}.jpg" /&gt;
        /// <para>'relativePath' is the ID of the attribute.</para></param>
        /// <param name="name"><para>Example:</para>
        /// [font size=12px] ... [/font]
        /// <para>'size' is the Name of the attribute.</para></param>
        /// <param name="contentTransformer">Function how the ID parameter should be formatted.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public BBAttribute(string id, string name, Func<IAttributeRenderingContext, string> contentTransformer)
            : this(id, name, contentTransformer, HtmlEncodingMode.HtmlAttributeEncode) { }

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="id"><para>Example:</para>
        /// &lt;img src="https://codekicker.de/${relativePath}.jpg" /&gt;
        /// <para>'relativePath' is the ID of the attribute.</para></param>
        /// <param name="name"><para>Example:</para>
        /// [font size=12px] ... [/font]
        /// <para>'size' is the Name of the attribute.</para></param>
        /// <param name="contentTransformer">Function how the ID parameter should be formatted.</param>
        /// <param name="htmlEncodingMode">Sets how this <see cref="BBAttribute"/> should be encoded in the parsing process.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public BBAttribute(string id, string name, Func<IAttributeRenderingContext, string> contentTransformer, HtmlEncodingMode htmlEncodingMode)
        {
            if (!Enum.IsDefined(typeof(HtmlEncodingMode), htmlEncodingMode))
                throw new ArgumentException(nameof(htmlEncodingMode));

            ID = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContentTransformer = contentTransformer;
            HtmlEncodingMode = htmlEncodingMode;
        }



        public static Func<IAttributeRenderingContext, string> AdaptLegacyContentTransformer(Func<string, string> contentTransformer)
        {
            return contentTransformer == null ? (Func<IAttributeRenderingContext, string>) null : ctx => contentTransformer(ctx.AttributeValue);
        }
    }
}

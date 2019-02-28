using System.Collections.Generic;

namespace CodeKicker.BBCode.Tags.BB
{
    /// <summary>
    /// BB element without any attributes.
    /// </summary>
    public class SimpleTag : IBBTag
    {
        /// <summary>
        /// Start tag of the element. For example: b which will become [b].
        /// </summary>
        public string OpeningTag { get; }

        /// <summary>
        /// End tag of the element. For example: b which will become [/b].
        /// </summary>
        public string ClosingTag { get; }

        internal static char OpenSymbol;
        internal static char CloseSymbol;



        static SimpleTag()
        {
            OpenSymbol  = '[';
            CloseSymbol = ']';
        }

        /// <summary>
        /// Initializes a new element with the same opening and closing tag.
        /// </summary>
        /// <param name="openingTag">Non null name of the start tag.
        /// For example: b, u, code, url, etc.</param>
        public SimpleTag(string openingTag)
            : this(openingTag, openingTag) { }

        public SimpleTag(string openingTag, string closingTag)
        {
            OpeningTag = openingTag;
            ClosingTag = openingTag;
        }



        public virtual string ToBBCode(List<Attribute> attributes, string content)
        {
            if (0 < attributes.Count)
            {
                foreach (var attr in attributes)
                {
                    content += attr.GetValue();
                }
            }

            return $"{OpenSymbol}{OpeningTag}{CloseSymbol}" +
                        $"{content}" +
                    $"{OpenSymbol}/{ClosingTag}{CloseSymbol}";
        }

        public virtual void AfterOpenTagClosed(string input, ref int currentIndex)
        {
            HtmlParser.ParseWhitespace(input, ref currentIndex);
        }
    }
}

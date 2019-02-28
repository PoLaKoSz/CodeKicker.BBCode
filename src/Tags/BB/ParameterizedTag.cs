using System;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tags.BB
{
    /// <summary>
    /// BB element with multiple attributes.
    /// </summary>
    public class ParameterizedTag : SimpleTag
    {
        private string _attrTemplate { get; }



        /// <summary>
        /// Initializes a new element with the same opening and closing tag.
        /// </summary>
        /// <param name="openingTag">Non null name of the start tag.
        /// For example: b, u, code, url, etc.</param>
        public ParameterizedTag(string openingTag, AttributeGenerator attrTemplate)
            : this(openingTag, attrTemplate.ToString()) { }

        /// <summary>
        /// Initializes a new element with the same opening and closing tag.
        /// </summary>
        /// <param name="openingTag">Non null name of the start tag.
        /// For example: b, u, code, url, etc.</param>
        public ParameterizedTag(string openingTag, string attrTemplate)
            : base(openingTag)
        {
            _attrTemplate = attrTemplate;
        }



        public override string ToBBCode(List<Attribute> attributes, string content)
        {
            if (attributes.Count < 1)
                throw new ArgumentException("ParameterizedTag can not be generated without a HTML attribute!");

            if (content.Length == 0)
                content = attributes[0].Value;

            return $"{OpenSymbol}{OpeningTag}{CustomAttributeManager.Replace(_attrTemplate, attributes)}{CloseSymbol}" +
                        $"{content}" +
                    $"{OpenSymbol}/{ClosingTag}{CloseSymbol}";
        }
    }
}

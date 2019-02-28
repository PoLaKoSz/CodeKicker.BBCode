using System;
using System.Collections.Generic;

namespace CodeKicker.BBCode.Tags.BB
{
    /// <summary>
    /// BB element with only 1 attribute.
    /// </summary>
    public class ValueTag : SimpleTag
    {
        private string _attrTemplate { get; }
        private string _contentAttrTemplate { get; set; }



        /// <summary>
        /// Initializes a new element with the same opening and closing tag.
        /// </summary>
        /// <param name="openingTag">Non null name of the start tag.
        /// For example: b, u, code, url, etc.</param>
        public ValueTag(string openingTag)
            : this(openingTag, CustomAttributeManager.Default) { }

        /// <summary>
        /// Initializes a new element with the same opening and closing tag.
        /// </summary>
        /// <param name="openingTag">Non null name of the start tag.
        /// For example: b, u, code, url, etc.</param>
        public ValueTag(string openingTag, AttributeGenerator attrTemplate)
            : this(openingTag, attrTemplate.ToString()) { }

        /// <summary>
        /// Initializes a new element with the same opening and closing tag.
        /// </summary>
        /// <param name="openingTag">Non null name of the start tag.
        /// For example: b, u, code, url, etc.</param>
        public ValueTag(string openingTag, string attrValueTemplate)
            : base(openingTag)
        {
            _attrTemplate = attrValueTemplate;
            _contentAttrTemplate = "";
        }


        public static ValueTag New(string openingTag, string attrValueTemplate)
        {
            return new ValueTag(openingTag, attrValueTemplate);
        }

        public ValueTag WithTextNodeFrom(string attrName)
        {
            _contentAttrTemplate = new AttributeGenerator().JustValue(attrName).ToString();

            return this;
        }

        public override string ToBBCode(List<Attribute> attributes, string content)
        {
            if (attributes.Count < 1)
                throw new ArgumentException("ValueTag needs at least one Attribute!");

            content += CustomAttributeManager.Replace(_contentAttrTemplate, attributes);

            return $"{OpenSymbol}{OpeningTag}={CustomAttributeManager.Replace(_attrTemplate, attributes)}{CloseSymbol}" +
                        $"{content}" +
                    $"{OpenSymbol}/{ClosingTag}{CloseSymbol}";
        }
    }
}

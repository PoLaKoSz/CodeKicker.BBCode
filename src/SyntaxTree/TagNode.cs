using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeKicker.BBCode.SyntaxTree
{
    public sealed class TagNode : SyntaxTreeNode
    {
        public BBTag Tag { get; private set; }

        public IDictionary<BBAttribute, string> AttributeValues { get; private set; }



        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="tag">Can not be null!</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TagNode(BBTag tag)
            : this(tag, null) { }

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="subNodes">Can be null.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TagNode(BBTag tag, IEnumerable<SyntaxTreeNode> subNodes)
            : base(subNodes)
        {
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            AttributeValues = new Dictionary<BBAttribute, string>();
        }



        /// <summary>
        /// Get the node content as a formatted HTML string.
        /// </summary>
        /// <returns>\n replaced with <br />s</returns>
        public override string ToHtml()
        {
            string content = GetContent();

            return ReplaceAttributeValues(Tag.OpenTagTemplate, content) + (Tag.AutoRenderContent ? content : null) + ReplaceAttributeValues(Tag.CloseTagTemplate, content);
        }

        /// <summary>
        /// Get the node content as a formatted BBCode string.
        /// </summary>
        public override string ToBBCode()
        {
            string content = string.Concat(SubNodes.Select(s => s.ToBBCode()).ToArray());

            string attrs = "";
            BBAttribute defAttr = Tag.FindAttribute("");

            if (defAttr != null)
            {
                if (AttributeValues.ContainsKey(defAttr))
                    attrs += "=" + AttributeValues[defAttr];
            }

            foreach (var attrKvp in AttributeValues)
            {
                if (attrKvp.Key.Name == "")
                    continue;

                attrs += " " + attrKvp.Key.Name + "=" + attrKvp.Value;
            }

            return "[" + Tag.Name + attrs + "]" + content + "[/" + Tag.Name + "]";
        }

        /// <summary>
        /// Return the object's and it's SubNodes summarized
        /// value as a text.
        /// </summary>
        public override string ToText()
        {
            return string.Concat(SubNodes.Select(s => s.ToText()).ToArray());
        }

        /// <summary>
        /// Create a new <see cref="TagNode"/> and expand it with
        /// the parameter <see cref="SyntaxTreeNode"/> collection.
        /// </summary>
        /// <param name="subNodes">Can not be null!</param>
        /// <exception cref="ArgumentNullException"></exception>
        public override SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes)
        {
            if (subNodes == null)
                throw new ArgumentNullException(nameof(subNodes));

            return new TagNode(Tag, subNodes)
            {
                AttributeValues = new Dictionary<BBAttribute, string>(AttributeValues),
            };
        }


        internal override SyntaxTreeNode AcceptVisitor(SyntaxTreeVisitor visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException(nameof(visitor));

            return visitor.Visit(this);
        }


        /// <summary>
        /// Custom Equal comparer for the base class Equal function.
        /// </summary>
        /// <returns><c>TRUE</c>, if the object's Tag and Attributes are equal,
        /// <c>FALSE</c> otherwise.</returns>
        protected override bool EqualsCore(SyntaxTreeNode b)
        {
            var casted = (TagNode)b;
            return
                Tag == casted.Tag &&
                AttributeValues.All(attr => casted.AttributeValues[attr.Key] == attr.Value) &&
                casted.AttributeValues.All(attr => AttributeValues[attr.Key] == attr.Value);
        }


        private string GetContent()
        {
            var content = string.Concat(SubNodes.Select(s => s.ToHtml()).ToArray());

            return Tag.ContentTransformer == null ? content : Tag.ContentTransformer(content);
        }

        private string ReplaceAttributeValues(string template, string content)
        {
            var attrList = (from attr in Tag.Attributes
                            group attr by attr.ID into gAttrByID
                            let val = (from attr in gAttrByID
                                       let val = TryGetValue(attr)
                                       where val != null
                                       select new { attr, val }).FirstOrDefault()
                            select new { attrID = gAttrByID.Key, attrAndVal = val }).ToList();

            var attrDictionary = attrList
                .Where(x => x.attrAndVal != null)
                .ToDictionary(x => x.attrID, x => x.attrAndVal.val);

            if (!attrDictionary.ContainsKey(BBTag.ContentPlaceholderName))
                attrDictionary.Add(BBTag.ContentPlaceholderName, content);

            var output = template;
            foreach (var x in attrList)
            {
                var placeholderStr = "${" + x.attrID + "}";

                if (x.attrAndVal != null)
                {
                    //replace attributes with values
                    var rawValue = x.attrAndVal.val;
                    var attribute = x.attrAndVal.attr;
                    output = ReplaceAttribute(output, attribute, rawValue, placeholderStr, attrDictionary);
                }
            }

            //replace empty attributes
            var attributeIDsWithValues = new HashSet<string>(attrList.Where(x => x.attrAndVal != null).Select(x => x.attrID));
            var emptyAttributes = Tag.Attributes.Where(attr => !attributeIDsWithValues.Contains(attr.ID)).ToList();

            foreach (var attr in emptyAttributes)
            {
                var placeholderStr = "${" + attr.ID + "}";
                output = ReplaceAttribute(output, attr, null, placeholderStr, attrDictionary);
            }

            output = output.Replace("${" + BBTag.ContentPlaceholderName + "}", content);
            return output;
        }

        private static string ReplaceAttribute(string output, BBAttribute attribute, string rawValue, string placeholderStr, Dictionary<string, string> attrValuesByID)
        {
            string effectiveValue;
            if (attribute.ContentTransformer == null)
            {
                effectiveValue = rawValue;
            }
            else
            {
                var ctx = new AttributeRenderingContextImpl(attribute, rawValue, attrValuesByID);
                effectiveValue = attribute.ContentTransformer(ctx);
            }

            if (effectiveValue == null)
                effectiveValue = "";

            var encodedValue =
                attribute.HtmlEncodingMode == HtmlEncodingMode.HtmlAttributeEncode ? HttpUtility.HtmlAttributeEncode(effectiveValue)
                    : attribute.HtmlEncodingMode == HtmlEncodingMode.HtmlEncode ? HttpUtility.HtmlEncode(effectiveValue)
                          : effectiveValue;

            output = output.Replace(placeholderStr, encodedValue);

            return output;
        }

        private string TryGetValue(BBAttribute attr)
        {
            string val;
            AttributeValues.TryGetValue(attr, out val);
            return val;
        }

        class AttributeRenderingContextImpl : IAttributeRenderingContext
        {
            public BBAttribute Attribute { get; private set; }

            public string AttributeValue { get; private set; }

            public IDictionary<string, string> GetAttributeValueByIDData { get; private set; }



            public AttributeRenderingContextImpl(BBAttribute attribute, string attributeValue, IDictionary<string, string> getAttributeValueByIdData)
            {
                Attribute = attribute;
                AttributeValue = attributeValue;
                GetAttributeValueByIDData = getAttributeValueByIdData;
            }



            public string GetAttributeValueByID(string id)
            {
                string value;

                if (!GetAttributeValueByIDData.TryGetValue(id, out value))
                    return null;

                return value;
            }
        }
    }
}

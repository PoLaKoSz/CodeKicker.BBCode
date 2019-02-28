using CodeKicker.BBCode.SyntaxTree;
using CodeKicker.BBCode.Tags.BB;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode.HtmlComponents
{
    /// <summary>
    /// HTML element cannot have any child nodes only has attributes.
    /// For example: &lt;img&gt;.
    /// </summary>
    public class VoidHtmlTag : HtmlTag
    {
        /// <summary>
        /// Attributes of this HTML element.
        /// </summary>
        internal List<Attribute> Attributes { get; set; }
        internal List<string> SkippableAttributes { get; private set; }



        /// <summary>
        /// Initialize an instance.
        /// </summary>
        /// <param name="openTag">Non null name of the HTML tag. For ex.: &lt;b&gt;&lt;/b&gt;.</param>
        public VoidHtmlTag(string openTag)
            : this(openTag, null, new Attribute[0]) { }

        /// <summary>
        /// Initialize an instance.
        /// </summary>
        /// <param name="openTag">Non null name of the HTML tag. For ex.: &lt;b&gt;&lt;/b&gt;.</param>
        /// <param name="resultTag">Non null BB Code representation of this element.
        /// For example: [img]{url}[/img].</param>
        /// <param name="attributes">Attributes that should be extracted from the source.</param>
        public VoidHtmlTag(string openTag, IBBTag resultTag, params Attribute[] attributes)
            : base(openTag)
        {
            SkippableAttributes = new List<string>();
            ResultTag = resultTag;
            Attributes = attributes.ToList();
        }

        public VoidHtmlTag(VoidHtmlTag tag)
            : this(tag.OpenTag, tag.ResultTag, tag.Attributes.ToArray())
        {
            Attributes = new List<Attribute>(tag.Attributes);

            for (int i = 0; i < tag.Attributes.Count; i++)
                Attributes[i] = new Attribute(tag.Attributes[i]);

            SkippableAttributes = new List<string>(tag.SkippableAttributes);
        }



        public static VoidHtmlTag CreateFrom(string openTag)
        {
            return new VoidHtmlTag(openTag);
        }

        public VoidHtmlTag WithA(Attribute attribute)
        {
            Attributes.Add(attribute);

            return this;
        }

        public VoidHtmlTag ParseTo(IBBTag output)
        {
            ResultTag = output;

            return this;
        }

        public VoidHtmlTag SkipAttribute(string name)
        {
            SkippableAttributes.Add(name);

            return this;
        }


        internal override string ToBBCode(string content)
        {
            return ResultTag.ToBBCode(Attributes, content);
        }

        internal override Node IsThisTag(string tagName, Dictionary<string, string> attrs, Stack<SyntaxTreeNode> stack, IExceptions exceptionMode)
        {
            if (!OpenTag.Equals(tagName))
                return null;

            if (!EveryAttrMatch(attrs))
                return null;

            return new Node(new VoidHtmlTag(this));
        }

        protected bool EveryAttrMatch(Dictionary<string, string> foundAttrs)
        {
            for (int i = Attributes.Count - 1; 0 <= i; i--)
            {
                var attr = Attributes[i];

                bool isKeyExists = foundAttrs.TryGetValue(attr.Name, out string foundAttrValue);

                if (SkippableAttributes.Exists(a => a.Equals(attr.Name)))
                {
                    Attributes.RemoveAt(i);
                    continue;
                }

                if (!isKeyExists)
                    return false;

                attr.Value = foundAttrValue;
            }

            return true;
        }

        internal override void ShouldBeOnStack(Stack<SyntaxTreeNode> stack, Node node) { }

        internal override string ParseTagEnd(string input, ref int pos, IExceptions exception) => "";

        internal override bool CanHaveTextNode(string text)
        {
            return false;
        }
    }
}

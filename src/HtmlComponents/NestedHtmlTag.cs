using CodeKicker.BBCode.SyntaxTree;
using CodeKicker.BBCode.Tags.BB;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode.HtmlComponents
{
    public partial class NestedHtmlTag : ClosedHtmlTag
    {
        private readonly List<HtmlTag> _childTags;
        private readonly List<Attribute> _attributes;



        public NestedHtmlTag(string openTag)
            : base(openTag)
        {
            _childTags = new List<HtmlTag>();
            _attributes = new List< Attribute>();
        }

        public NestedHtmlTag(NestedHtmlTag oldTag)
            : base(oldTag)
        {
            _childTags = new List<HtmlTag>(oldTag._childTags);
            _attributes = new List<Attribute>(oldTag._attributes);
        }



        public new static NestedHtmlTag CreateFrom(string openTag)
        {
            return new NestedHtmlTag(openTag);
        }

        public new NestedHtmlTag WithA(Attribute attribute)
        {
            base.WithA(attribute);

            return this;
        }

        public NestedHtmlTag WithChild(VoidHtmlTag tag)
        {
            _childTags.Add(tag);

            return this;
        }

        public NestedHtmlTag AsA(Attribute attribute)
        {
            _childTags.Last().ResultTag = new AttributeTag(attribute);
            _attributes.Add(attribute);

            return this;
        }

        public new NestedHtmlTag SkipAttribute(string name)
        {
            SkippableAttributes.Add(name);

            return this;
        }

        public new List<HtmlTag> ParseTo(IBBTag output)
        {
            ResultTag = output;

            _childTags.Add(this);

            return _childTags;
        }

        internal override Node IsThisTag(string tagName, Dictionary<string, string> attrs, Stack<SyntaxTreeNode> stack, IExceptions exceptionMode)
        {
            if (!OpenTag.Equals(tagName))
                return null;

            if (!base.EveryAttrMatch(attrs))
                return null;

            return new Node(new NestedHtmlTag(this));
        }

        internal override string ToBBCode(string content)
        {
            if (_childTags.Count < _attributes.Count)
                throw new BBCodeParsingException($"The tag with name {OpenTag} has more alias for children than child alone!");

            content = NestedAttributeManager.Replace(content, _attributes);

            Attributes.AddRange(_attributes);

            return base.ToBBCode(content);
        }
    }
}

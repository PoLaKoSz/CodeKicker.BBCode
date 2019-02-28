using System.Collections.Generic;
using CodeKicker.BBCode.SyntaxTree;

namespace CodeKicker.BBCode.HtmlComponents
{
    /// <summary>
    /// HTML element with child nodes and attributes.
    /// For example: &lt;div&gt;&lt;/div&gt;, etc.
    /// </summary>
    public class ClosedHtmlTag : VoidHtmlTag
    {
        private List<string> _skippableTextNodes;
        private readonly string _closeTag;



        public ClosedHtmlTag(string openTag)
            : base(openTag)
        {
            _closeTag = "/" + openTag;
            _skippableTextNodes = new List<string>();
        }

        public ClosedHtmlTag(ClosedHtmlTag tag)
            : base(tag)
        {
            _closeTag = tag._closeTag;
            _skippableTextNodes = tag._skippableTextNodes;
        }



        public new static ClosedHtmlTag CreateFrom(string openTag)
        {
            return new ClosedHtmlTag(openTag);
        }

        public new ClosedHtmlTag WithA(Attribute attribute)
        {
            base.WithA(attribute);

            return this;
        }

        public ClosedHtmlTag SkipTextNode(string value)
        {
            _skippableTextNodes.Add(value);

            return this;
        }


        internal override string ToBBCode(string content)
        {
            return base.ToBBCode(content);
        }

        internal override Node IsThisTag(string tagName, Dictionary<string, string> attrs, Stack<SyntaxTreeNode> stack, IExceptions exceptionMode)
        {
            if (!OpenTag.Equals(tagName))
                return null;

            if (!base.EveryAttrMatch(attrs))
                return null;

            return new Node(new ClosedHtmlTag(this));
        }

        internal override void ShouldBeOnStack(Stack<SyntaxTreeNode> stack, Node node)
        {
            stack.Push(node);
        }

        internal override string ParseTagEnd(string input, ref int pos, IExceptions exception)
        {
            int currentIndex = pos;

            if (!HtmlParser.ParseChar(input, ref currentIndex, '<'))
                return null;

            if (!HtmlParser.ParseChar(input, ref currentIndex, '/'))
                return null;

            string tagName = HtmlParser.ParseName(input, ref currentIndex);
            if (tagName == null)
                return null;

            HtmlParser.ParseWhitespace(input, ref currentIndex);

            if (!HtmlParser.ParseChar(input, ref currentIndex, '>'))
            {
                if (exception.TagNotClosed(OpenTag))
                    return null;
            }

            pos = currentIndex;

            return tagName;
        }

        internal override bool CanHaveTextNode(string text)
        {
            foreach (var textNodeNoNeed in _skippableTextNodes)
            {
                if (textNodeNoNeed.Equals(text))
                    return false;
            }

            return true;
        }
    }
}

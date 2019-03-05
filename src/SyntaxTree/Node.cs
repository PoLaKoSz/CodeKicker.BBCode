using CodeKicker.BBCode.HtmlComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode.SyntaxTree
{
    public class Node : SyntaxTreeNode
    {
        public int Index { get; internal set; }
        internal HtmlTag Tag { get; }



        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="tag">Can not be null!</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Node(HtmlTag tag)
            : this(tag, null) { }

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="subNodes">Can be null.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Node(HtmlTag tag, IEnumerable<SyntaxTreeNode> subNodes)
            : base(subNodes)
        {
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
        }



        /// <summary>
        /// Get the node content as a formatted HTML string.
        /// </summary>
        /// <returns>\n replaced with <br />s</returns>
        public override string ToHtml()
        {
            return "";
        }

        /// <summary>
        /// Get the node content as a formatted BBCode string.
        /// </summary>
        public override string ToBBCode()
        {
            string content = string.Concat(SubNodes.Select(s => s.ToBBCode()).ToArray());

            return Tag.ToBBCode(content);
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
        /// Create a new <see cref="Node"/> and expand it with
        /// the parameter <see cref="SyntaxTreeNode"/> collection.
        /// </summary>
        /// <param name="subNodes">Can not be null!</param>
        /// <exception cref="ArgumentNullException"></exception>
        public override SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes)
        {
            if (subNodes == null)
                throw new ArgumentNullException(nameof(subNodes));

            return new Node(Tag, subNodes);
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
            var casted = (Node)b;

            return Tag == casted.Tag;
        }

        public override string ToString()
        {
            return Tag.ToString();
        }
    }
}

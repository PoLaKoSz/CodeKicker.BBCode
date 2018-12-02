using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode.SyntaxTree
{
    public sealed class SequenceNode : SyntaxTreeNode
    {
        public SequenceNode() { }

        /// <summary>
        /// Initialize an instance with the given <see cref="SyntaxTreeNodeCollection"/>.
        /// </summary>
        /// <param name="subNodes">Can not be null!</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SequenceNode(SyntaxTreeNodeCollection subNodes)
            : base(subNodes)
        {
            if (subNodes == null)
                throw new ArgumentNullException(nameof(subNodes));
        }

        /// <summary>
        /// Initialize an instance with the given list of <see cref="SyntaxTreeNode"/>.
        /// </summary>
        /// <param name="subNodes">Can not be null!</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SequenceNode(IEnumerable<SyntaxTreeNode> subNodes)
            : base(subNodes)
        {
            if (subNodes == null)
                throw new ArgumentNullException(nameof(subNodes));
        }



        /// <summary>
        /// Get the node content as a formatted HTML string.
        /// </summary>
        /// <returns>\n replaced with <br />s</returns>
        public override string ToHtml()
        {
            return string.Concat(SubNodes.Select(s => s.ToHtml()).ToArray());
        }

        /// <summary>
        /// Get the node content as a formatted BBCode string.
        /// </summary>
        public override string ToBBCode()
        {
            return string.Concat(SubNodes.Select(s => s.ToBBCode()).ToArray());
        }

        /// <summary>
        /// Get the node content with it's children content
        /// as a text.
        /// </summary>
        public override string ToText()
        {
            return string.Concat(SubNodes.Select(s => s.ToText()).ToArray());
        }

        /// <summary>
        /// Create a new <see cref="SequenceNode"/> with the given
        /// collection of <see cref="SyntaxTreeNode"/>.
        /// </summary>
        /// <param name="subNodes">Can not be null! These will be the child
        /// elements of the new object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public override SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes)
        {
            if (subNodes == null)
                throw new ArgumentNullException(nameof(subNodes));

            return new SequenceNode(subNodes);
        }


        internal override SyntaxTreeNode AcceptVisitor(SyntaxTreeVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.Visit(this);
        }


        /// <summary>
        /// Custom Equal comparer for the base class Equal function.
        /// </summary>
        /// <returns><c>TRUE</c> allways.</returns>
        protected override bool EqualsCore(SyntaxTreeNode b)
        {
            return true;
        }
    }
}

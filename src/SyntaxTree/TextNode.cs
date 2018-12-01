using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeKicker.BBCode.SyntaxTree
{
    public sealed class TextNode : SyntaxTreeNode
    {
        /// <summary>
        /// The node content.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Template how the node should be rendered.
        /// It's null, if no template provided.
        /// </summary>
        public string HtmlTemplate { get; private set; }



        /// <summary>
        /// Initialize a node without a HTML template.
        /// </summary>
        /// <param name="text">Can not be null!</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TextNode(string text)
            : this(text, null) { }

        /// <summary>
        /// Initialize a node with a HTML template.
        /// </summary>
        /// <param name="text">Can not be null!</param>
        /// <param name="htmlTemplate">Can be null.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TextNode(string text, string htmlTemplate)
            : base(null)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            HtmlTemplate = htmlTemplate;
        }



        /// <summary>
        /// Get the node content as a formatted HTML string.
        /// </summary>
        /// <returns>\n replaced with <br />s</returns>
        public override string ToHtml()
        {
            // Right-oh, future Paul - the below doesn't do anything sensible with newlines but the obvious 'replace newlines with BRs' tactic
            // doesn't work because you don't want to do it in all circumstances. Need to add a 'suppress BR after' property or similar to
            // BBTag? This'd control if we trim the first newline after a tag of that type closes. We could *then* replace all \n with <br /> and
            // be on our merry way
            if (HtmlTemplate == null)
            {
                return HttpUtility.HtmlEncode(Text);
            }
            else
            {
                return HtmlTemplate
                    .Replace("${content}", HttpUtility.HtmlEncode(Text))
                    .Replace("\n", "<br />");
            }
        }

        /// <summary>
        /// Get the node content as a formatted BBCode string.
        /// </summary>
        public override string ToBBCode()
        {
            return Text.Replace("\\", "\\\\").Replace("[", "\\[").Replace("]", "\\]");
        }

        /// <summary>
        /// Return the object's Text property.
        /// </summary>
        public override string ToText()
        {
            return Text;
        }

        /// <summary>
        /// Return this object without any modification.
        /// </summary>
        /// <param name="subNodes">Can not be null. Can not have Submodules.</param>
        /// <returns>This object.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public override SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes)
        {
            if (subNodes == null)
                throw new ArgumentNullException(nameof(subNodes));

            if (subNodes.Any())
                throw new ArgumentException($"{nameof(subNodes)} cannot contain any nodes for a TextNode.");

            return this;
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
        /// <returns><c>TRUE</c>, if the object's Text and HtmlTemplate are equal,
        /// <c>FALSE</c> otherwise.</returns>
        protected override bool EqualsCore(SyntaxTreeNode b)
        {
            var casted = (TextNode)b;
            return Text == casted.Text && HtmlTemplate == casted.HtmlTemplate;
        }
    }
}
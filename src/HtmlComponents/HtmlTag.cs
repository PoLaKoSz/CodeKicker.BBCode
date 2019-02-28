using CodeKicker.BBCode.SyntaxTree;
using CodeKicker.BBCode.Tags.BB;
using System.Collections.Generic;

namespace CodeKicker.BBCode.HtmlComponents
{
    public abstract class HtmlTag
    {
        /// <summary>
        /// Name of the element. For example: &lt;img&gt;, &lt;br&gt;, etc.
        /// </summary>
        internal string OpenTag { get; }

        /// <summary>
        /// BB Code representation of this element. For example: [img]{url}[/img].
        /// </summary>
        internal IBBTag ResultTag { get; set; }



        public HtmlTag(string openTag)
        {
            OpenTag = openTag;
        }


        
        internal abstract string ToBBCode(string content);

        internal abstract Node IsThisTag(string tagName, Dictionary<string, string> attrs, Stack<SyntaxTreeNode> stack, IExceptions exceptionMode);

        internal abstract void ShouldBeOnStack(Stack<SyntaxTreeNode> stack, Node node);

        internal abstract string ParseTagEnd(string input, ref int pos, IExceptions exception);

        internal abstract bool CanHaveTextNode(string text);
    }
}

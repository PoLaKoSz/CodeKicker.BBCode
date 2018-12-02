using System;

namespace CodeKicker.BBCode.SyntaxTree
{
    public class SyntaxTreeVisitor
    {
        /// <param name="node">Can be null.</param>
        /// <returns><c>Null</c>, if the parameter is also null or a
        /// <c><see cref="SyntaxTreeNode"/></c>.</returns>
        public SyntaxTreeNode Visit(SyntaxTreeNode node)
        {
            if (node == null)
                return null;

            return node.AcceptVisitor(this);
        }


        /// <param name="node">Can be null.</param>
        /// <returns><c>Null</c>, if the parameter is also null or a
        /// <c><see cref="SyntaxTreeNode"/></c>.</returns>
        protected internal virtual SyntaxTreeNode Visit(SequenceNode node)
        {
            if (node == null)
                return null;

            var modifiedSubNodes = GetModifiedSubNodes(node);

            if (modifiedSubNodes == null)
                return node; //unmodified
            else
                return node.SetSubNodes(modifiedSubNodes); //subnodes were modified
        }

        /// <param name="node">Can be null.</param>
        /// <returns><c>Null</c>, if the parameter is also null or a
        /// <c><see cref="SyntaxTreeNode"/></c>.</returns>
        protected internal virtual SyntaxTreeNode Visit(TagNode node)
        {
            if (node == null)
                return null;

            var modifiedSubNodes = GetModifiedSubNodes(node);

            if (modifiedSubNodes == null)
                return node; //unmodified
            else
                return node.SetSubNodes(modifiedSubNodes); //subnodes were modified
        }

        /// <summary>
        /// Return the parameter <see cref="TextNode"/>.
        /// </summary>
        protected internal virtual SyntaxTreeNode Visit(TextNode node)
        {
            return node;
        }


        private SyntaxTreeNodeCollection GetModifiedSubNodes(SyntaxTreeNode node)
        {
            SyntaxTreeNodeCollection modifiedSubNodes = null; //lazy

            for (int i = 0; i < node.SubNodes.Count; i++)
            {
                var subNode = node.SubNodes[i];

                var replacement = Visit(subNode);
                if (replacement != subNode)
                {
                    if (modifiedSubNodes == null) //lazy init
                    {
                        modifiedSubNodes = new SyntaxTreeNodeCollection();
                        for (int j = 0; j < i; j++) //copy unmodified nodes
                            modifiedSubNodes.Add(node.SubNodes[j]);
                    }

                    if (replacement != null) //insert replacement
                        modifiedSubNodes.Add(replacement);
                }
                else
                {
                    if (modifiedSubNodes != null) //only insert unmodified subnode if the lazy collection has been initialized
                        modifiedSubNodes.Add(subNode);
                }
            }
            return modifiedSubNodes;
        }
    }
}

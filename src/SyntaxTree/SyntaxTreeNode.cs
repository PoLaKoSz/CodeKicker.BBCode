using System;
using System.Collections.Generic;

namespace CodeKicker.BBCode.SyntaxTree
{
    public abstract class SyntaxTreeNode : IEquatable<SyntaxTreeNode>
    {
        /// <summary>
        /// Never null.
        /// </summary>
        public ISyntaxTreeNodeCollection SubNodes { get; private set; }



        protected SyntaxTreeNode()
            : this(new SyntaxTreeNodeCollection()) { }

        /// <summary>
        /// Initialize the class with a predefinied <see cref="ISyntaxTreeNodeCollection"/>.
        /// </summary>
        /// <param name="subNodes">Can be null.</param>
        protected SyntaxTreeNode(ISyntaxTreeNodeCollection subNodes)
        {
            SubNodes = subNodes ?? new SyntaxTreeNodeCollection();
        }

        /// <summary>
        /// Initialize the class with a collection of <see cref="SyntaxTreeNode"/> which
        /// will be encapsulated in a <see cref="SyntaxTreeNodeCollection"/>.
        /// </summary>
        /// <param name="subNodes">Can be null.</param>
        protected SyntaxTreeNode(IEnumerable<SyntaxTreeNode> subNodes)
        {
            SubNodes = subNodes == null ? new SyntaxTreeNodeCollection() : new SyntaxTreeNodeCollection(subNodes);
        }



        /// <summary>
        /// Custom implementation for derived classes how
        /// to display it's content as HTML.
        /// </summary>
        public abstract string ToHtml();

        /// <summary>
        /// Custom implementation for derived classes how
        /// to display it's content as a BBCode.
        /// </summary>
        public abstract string ToBBCode();

        /// <summary>
        /// Custom implementation for derived classes how
        /// to display it's content as a text.
        /// </summary>
        public abstract string ToText();

        public abstract SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes);


        internal abstract SyntaxTreeNode AcceptVisitor(SyntaxTreeVisitor visitor);


        /// <summary>
        /// Custom Equal comparer for the derived classes.
        /// </summary>
        protected abstract bool EqualsCore(SyntaxTreeNode b);


        /// <summary>
        /// Check for reference equality.
        /// </summary>
        /// <returns><c>TRUE</c> if the two parameter has the same reference
        /// and their SubModules too, <c>FALSE</c> otherwise.</returns>
        public bool Equals(SyntaxTreeNode other)
        {
            return this == other;
        }

        /// <summary>
        /// Check for reference equality.
        /// </summary>
        /// <returns><c>TRUE</c> if the two parameter has the same reference
        /// and their SubModules too, <c>FALSE</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SyntaxTreeNode);
        }

        /// <summary>
        /// Return the object only with BBCodes.
        /// </summary>
        public override string ToString()
        {
            return ToBBCode();
        }

        public override int GetHashCode()
        {
            return 1008241338 + EqualityComparer<ISyntaxTreeNodeCollection>.Default.GetHashCode(SubNodes);
        }

        /// <summary>
        /// Check for reference equality.
        /// </summary>
        /// <returns><c>TRUE</c> if the two parameter has the same reference
        /// and their SubModules too, <c>FALSE</c> otherwise.</returns>
        public static bool operator ==(SyntaxTreeNode a, SyntaxTreeNode b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
            if (a.GetType() != b.GetType()) return false;

            if (a.SubNodes.Count != b.SubNodes.Count) return false;
            if (!ReferenceEquals(a.SubNodes, b.SubNodes))
            {
                for (int i = 0; i < a.SubNodes.Count; i++)
                    if (a.SubNodes[i] != b.SubNodes[i]) return false;
            }

            return a.EqualsCore(b);
        }

        /// <summary>
        /// Check for reference inequality.
        /// </summary>
        /// <returns><c>FALSE</c> if the two parameter has the same reference
        /// and their SubModules too, <c>TRUE</c> otherwise.</returns>
        public static bool operator !=(SyntaxTreeNode a, SyntaxTreeNode b)
        {
            return !(a == b);
        }
    }
}
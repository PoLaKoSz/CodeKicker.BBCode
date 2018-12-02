using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CodeKicker.BBCode.SyntaxTree
{
    public class SyntaxTreeNodeCollection : Collection<SyntaxTreeNode>, ISyntaxTreeNodeCollection
    {
        public SyntaxTreeNodeCollection()
            : base() { }

        /// <summary>
        /// Initialize a new object.
        /// </summary>
        /// <param name="list">Can not be null!</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SyntaxTreeNodeCollection(IEnumerable<SyntaxTreeNode> list)
            : base(list.ToArray())
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
        }



        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.
        /// Can not be null.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected override void SetItem(int index, SyntaxTreeNode item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            base.SetItem(index, item);
        }

        /// <summary>
        /// Inserts an element into the Collection
        /// at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. Can not be null.</param>
        protected override void InsertItem(int index, SyntaxTreeNode item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            base.InsertItem(index, item);
        }
    }
}

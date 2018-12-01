using System;
using CodeKicker.BBCode.SyntaxTree;

namespace CodeKicker.BBCode
{
    public class TextSpanReplaceInfo
    {
        /// <summary>
        /// The index where it's starts in
        /// the given document.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The Replacement's content length.
        /// </summary>
        public int Length { get; private set; }

        public SyntaxTreeNode Replacement { get; private set; }



        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="index">The index where it's starts in
        /// the given document.</param>
        /// <param name="length">The Replacement's content length.</param>
        /// <param name="replacement">Can be null.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TextSpanReplaceInfo(int index, int length, SyntaxTreeNode replacement)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

            Index = index;
            Length = length;
            Replacement = replacement;
        }
    }
}

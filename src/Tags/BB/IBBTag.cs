using System.Collections.Generic;

namespace CodeKicker.BBCode.Tags.BB
{
    public interface IBBTag
    {
        /// <summary>
        /// Start tag of the element. For example: b which will become [b].
        /// </summary>
        string OpeningTag { get; }

        /// <summary>
        /// End tag of the element. For example: b which will become [/b].
        /// </summary>
        string ClosingTag { get; }



        string ToBBCode(List<Attribute> attributes, string content);

        void AfterOpenTagClosed(string input, ref int currentIndex);
    }
}

using System;

namespace CodeKicker.BBCode
{
    public enum BBTagClosingStyle
    {
        RequiresClosingTag = 0,

        AutoCloseElement = 1,

        /// <summary>
        /// They are closed emediately.
        /// </summary>
        LeafElementWithoutContent = 2,
    }
}
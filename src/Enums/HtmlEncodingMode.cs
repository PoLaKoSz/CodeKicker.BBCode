using System;

namespace CodeKicker.BBCode
{
    public enum HtmlEncodingMode
    {
        /// <summary>
        /// Converts a string to an HTML-encoded string.
        /// </summary>
        HtmlEncode = 0,

        /// <summary>
        /// Minimally converts a string to an HTML-encoded string.
        /// </summary>
        HtmlAttributeEncode = 1,

        UnsafeDontEncode = 2,
    }
}
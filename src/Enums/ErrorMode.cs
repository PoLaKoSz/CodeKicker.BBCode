using System;

namespace CodeKicker.BBCode
{
    public enum ErrorMode
    {
        /// <summary>
        /// The parser will never throw an exception. Invalid tags like "array[0]" will be interpreted as text.
        /// </summary>
        ErrorFree,

        /// <summary>
        /// Syntax errors with obvious meaning will be corrected automatically.
        /// </summary>
        TryErrorCorrection,

        /// <summary>
        /// Every syntax error throws a BBCodeParsingException.
        /// </summary>
        Strict,
    }
}

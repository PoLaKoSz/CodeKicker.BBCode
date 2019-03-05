using CodeKicker.BBCode.SyntaxTree;

namespace CodeKicker.BBCode
{
    public class ErrorFreeParsing : MessagesHelper, IExceptions
    {
        private static readonly bool _result;



        static ErrorFreeParsing()
        {
            _result = true;
        }



        bool IExceptions.DuplicateAttribute(string tagName, string attributeName) => _result;

        bool IExceptions.EscapeChar() => _result;

        bool IExceptions.MissingDefaultAttribute(string tagName) => _result;

        bool IExceptions.NoAttributesAllowed(string tagName) => _result;

        bool IExceptions.NonEscapedChar() => _result;

        bool IExceptions.TagNotClosed(string tagName) => _result;

        void IExceptions.TagNotClosed(Node node) { }
        
        bool IExceptions.TagNotMatching(string startTagName, string endTagName) => _result;

        bool IExceptions.TagNotOpened(string tagName) => _result;

        bool IExceptions.UnknownAttribute(string tagName, string attributeName) => _result;

        bool IExceptions.UnknownTag(string tagName) => _result;

        /// <summary>
        /// The specified tag does not exist in the user defined rules.
        /// </summary>
        /// <param name="tagName">Non null name of the unknown tag.</param>
        /// <param name="index">The position where the tag begins.</param>
        /// <returns>Non null formatted string.</returns>
        bool IExceptions.UnknownTag(string tagName, int index) => _result;
    }
}

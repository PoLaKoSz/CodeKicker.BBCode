using System;

namespace CodeKicker.BBCode
{
    public class ErrorFreeParsing : MessagesHelper, IExceptions
    {
        bool IExceptions.DuplicateAttribute(string tagName, string attributeName)
        {
            return true;
        }

        bool IExceptions.EscapeChar()
        {
            return true;
        }

        bool IExceptions.MissingDefaultAttribute(string tagName)
        {
            return true;
        }

        bool IExceptions.NoAttributesAllowed(string tagName)
        {
            return true;
        }

        bool IExceptions.NonEscapedChar()
        {
            return true;
        }

        bool IExceptions.TagNotClosed(string tagName)
        {
            return true;
        }

        bool IExceptions.TagNotMatching(string startTagName, string endTagName)
        {
            return true;
        }

        bool IExceptions.TagNotOpened(string tagName)
        {
            return true;
        }

        bool IExceptions.UnknownAttribute(string tagName, string attributeName)
        {
            return true;
        }

        bool IExceptions.UnknownTag(string tagName)
        {
            return true;
        }
    }
}

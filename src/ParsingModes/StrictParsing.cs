﻿using CodeKicker.BBCode.Exceptions;
using CodeKicker.BBCode.SyntaxTree;

namespace CodeKicker.BBCode
{
    public class StrictParsing : MessagesHelper, IExceptions
    {
        bool IExceptions.DuplicateAttribute(string tagName, string attributeName)
        {
            throw new BBCodeParsingException(base.DuplicateAttribute(tagName, attributeName));
        }

        bool IExceptions.EscapeChar()
        {
            throw new BBCodeParsingException(base.EscapeChar());
        }

        bool IExceptions.MissingDefaultAttribute(string tagName)
        {
            throw new BBCodeParsingException(base.MissingDefaultAttribute(tagName));
        }

        bool IExceptions.NoAttributesAllowed(string tagName)
        {
            throw new BBCodeParsingException(base.NoAttributesAllowed(tagName));
        }

        bool IExceptions.NonEscapedChar()
        {
            throw new BBCodeParsingException(base.NonEscapedChar());
        }

        bool IExceptions.TagNotClosed(string tagName)
        {
            throw new BBCodeParsingException(base.TagNotClosed(tagName));
        }

        void IExceptions.TagNotClosed(Node node)
        {
            throw new ParserException(base.TagNotClosed(node));
        }

        bool IExceptions.TagNotMatching(string startTagName, string endTagName)
        {
            throw new BBCodeParsingException(base.TagNotMatching(startTagName, endTagName));
        }

        bool IExceptions.TagNotOpened(string tagName)
        {
            throw new BBCodeParsingException(base.TagNotOpened(tagName));
        }

        bool IExceptions.UnknownAttribute(string tagName, string attributeName)
        {
            throw new BBCodeParsingException(base.UnknownAttribute(tagName, attributeName));
        }

        bool IExceptions.UnknownTag(string tagName)
        {
            throw new BBCodeParsingException(base.UnknownTag(tagName));
        }

        /// <summary>
        /// The specified tag does not exist in the user defined rules.
        /// </summary>
        /// <param name="tagName">Non null name of the unknown tag.</param>
        /// <param name="index">The position where the tag begins.</param>
        /// <returns>Non null formatted string.</returns>
        bool IExceptions.UnknownTag(string tagName, int index)
        {
            throw new ParserException(base.TagNotClosed(tagName, index));
        }
    }
}
﻿using System;

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
    }
}

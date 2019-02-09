using System;

namespace CodeKicker.BBCode
{
    public interface IExceptions
    {
        /// <summary>
        /// The tag {tagName} has a duplicate attribute {attributeName}.
        /// </summary>
        /// <param name="tagName">Non null name of the attribute owner.</param>
        /// <param name="attributeName">Non null name of the attribute.</param>
        /// <returns>Non null formatted string.</returns>
        bool DuplicateAttribute(string tagName, string attributeName);

        /// <summary>
        /// The character "\" is used to escape "]" and "[". Please escape it like this "\\".
        /// </summary>
        /// <returns>Non null formatted string.</returns>
        bool EscapeChar();

        /// <summary>
        /// The tag {tagName} cannot have a value.
        /// </summary>
        /// <param name="tagName">Non null name of the tag which not
        /// contains any default attribute.</param>
        /// <returns>Non null formatted string.</returns>
        bool MissingDefaultAttribute(string tagName);

        /// <summary>
        /// In the tag {tagName} no attributes are allowed.
        /// </summary>
        /// <param name="tagName">Non null name of the tag that
        /// mustn't have any attribute.</param>
        /// <returns>Non null formatted string.</returns>
        bool NoAttributesAllowed(string tagName);

        /// <summary>
        /// The character "]" cannot be used in text or code without
        /// beeing escaped. Please write "\]" instead.
        /// </summary>
        /// <returns>Non null formatted string.</returns>
        bool NonEscapedChar();

        /// <summary>
        /// The tag {tagName} was not closed correctly.
        /// </summary>
        /// <param name="tagName">Non null name of the not closed tag.</param>
        /// <returns>Non null formatted string.</returns>
        bool TagNotClosed(string tagName);

        /// <summary>
        /// The end-tag {startTagName} does not match the preceding start-tag {endTagName}.
        /// </summary>
        /// <param name="startTagName">Non null name of the attribute owner.</param>
        /// <param name="endTagName">Non null name of the attribute.</param>
        /// <returns>Non null formatted string.</returns>
        bool TagNotMatching(string startTagName, string endTagName);

        /// <summary>
        /// The tag {tagName} has not been closed.
        /// </summary>
        /// <param name="tagName">Non null name of the not opened tag.</param>
        /// <returns>Non null formatted string.</returns>
        bool TagNotOpened(string tagName);

        /// <summary>
        /// The tag {tagName} does not have an attribute {attributeName}.
        /// </summary>
        /// <param name="tagName">Non null name of the attribute owner.</param>
        /// <param name="attributeName">Non null name of the attribute.</param>
        /// <returns>Non null formatted string.</returns>
        bool UnknownAttribute(string tagName, string attributeName);

        /// <summary>
        /// The tag {tagName} does not exists.
        /// </summary>
        /// <param name="tagName">Non null name of the unknown tag.</param>
        /// <returns>Non null formatted string.</returns>
        bool UnknownTag(string tagName);
    }
}

using CodeKicker.BBCode.SyntaxTree;
using System.Resources;

namespace CodeKicker.BBCode
{
    public abstract class MessagesHelper
    {
        private static readonly ResourceManager _resMgr;



        static MessagesHelper()
        {
            _resMgr = new ResourceManager(typeof(Messages));
        }



        /// <summary>
        /// The tag {tagName} has a duplicate attribute {attributeName}.
        /// </summary>
        /// <param name="tagName">Non null name of the attribute owner.</param>
        /// <param name="attributeName">Non null name of the attribute.</param>
        /// <returns>Non null formatted string.</returns>
        protected string DuplicateAttribute(string tagName, string attributeName)
        {
            return string.Format(GetString("DuplicateAttribute"), tagName, attributeName);
        }

        /// <summary>
        /// The character "\" is used to escape "]" and "[". Please escape it like this "\\".
        /// </summary>
        /// <returns>Non null formatted string.</returns>
        protected string EscapeChar()
        {
            return GetString("EscapeChar");
        }

        /// <summary>
        /// The tag {tagName} cannot have a value.
        /// </summary>
        /// <param name="tagName">Non null name of the tag which not
        /// contains any default attribute.</param>
        /// <returns>Non null formatted string.</returns>
        protected string MissingDefaultAttribute(string tagName)
        {
            return GetString("MissingDefaultAttribute", tagName);
        }

        /// <summary>
        /// In the tag {tagName} no attributes are allowed.
        /// </summary>
        /// <param name="tagName">Non null name of the tag that
        /// mustn't have any attribute.</param>
        /// <returns>Non null formatted string.</returns>
        protected string NoAttributesAllowed(string tagName)
        {
            return GetString("NoAttributesAllowed", tagName);
        }

        /// <summary>
        /// The character "]" cannot be used in text or code without
        /// beeing escaped. Please write "\]" instead.
        /// </summary>
        /// <returns>Non null formatted string.</returns>
        protected string NonEscapedChar()
        {
            return GetString("NonescapedChar");
        }

        /// <summary>
        /// The tag {tagName} was not closed correctly.
        /// </summary>
        /// <param name="tagName">Non null name of the not closed tag.</param>
        /// <returns>Non null formatted string.</returns>
        protected string TagNotClosed(string tagName)
        {
            return GetString("TagNotClosed", tagName);
        }

        /// <summary>
        /// The tag {tagName} was not closed correctly.
        /// </summary>
        /// <param name="tagName">Non null name of the not closed tag.</param>
        /// <returns>Non null formatted string.</returns>
        protected string TagNotClosed(Node node)
        {
            return TagNotClosed(node.Tag.OpenTag, node.Index);
        }

        protected string TagNotClosed(string tagName, int index)
        {
            return GetString("TagNodeNotClosed", tagName, index);
        }

        /// <summary>
        /// The end-tag {startTagName} does not match the preceding start-tag {endTagName}.
        /// </summary>
        /// <param name="startTagName">Non null name of the attribute owner.</param>
        /// <param name="endTagName">Non null name of the attribute.</param>
        /// <returns>Non null formatted string.</returns>
        protected string TagNotMatching(string startTagName, string endTagName)
        {
            return GetString("TagNotMatching", startTagName, endTagName);
        }

        /// <summary>
        /// The tag {tagName} has not been closed.
        /// </summary>
        /// <param name="tagName">Non null name of the not opened tag.</param>
        /// <returns>Non null formatted string.</returns>
        protected string TagNotOpened(string tagName)
        {
            return GetString("TagNotOpened", tagName);
        }

        /// <summary>
        /// The tag {tagName} does not have an attribute {attributeName}.
        /// </summary>
        /// <param name="tagName">Non null name of the attribute owner.</param>
        /// <param name="attributeName">Non null name of the attribute.</param>
        /// <returns>Non null formatted string.</returns>
        protected string UnknownAttribute(string tagName, string attributeName)
        {
            return GetString("UnknownAttribute", tagName, attributeName);
        }

        /// <summary>
        /// The tag {tagName} does not exists.
        /// </summary>
        /// <param name="tagName">Non null name of the unknown tag.</param>
        /// <returns>Non null formatted string.</returns>
        protected string UnknownTag(string tagName)
        {
            return GetString("UnknownTag", tagName);
        }
        
        protected string GetString(string key)
        {
            return _resMgr.GetString(key);
        }

        protected string GetString(string key, params object[] parameters)
        {
            return string.Format(_resMgr.GetString(key), parameters);
        }
    }
}

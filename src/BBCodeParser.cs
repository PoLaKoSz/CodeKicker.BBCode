using System;
using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode.SyntaxTree;

namespace CodeKicker.BBCode
{
    /// <summary>
    /// This class is useful for creating a custom parser. You can customize which tags are available
    /// and how they are translated to HTML.
    /// In order to use this library, we require a link to http://codekicker.de/ from you. Licensed unter
    /// the Creative Commons Attribution 3.0 Licence: http://creativecommons.org/licenses/by/3.0/.
    /// </summary>
    public class BBCodeParser
    {
        /// <summary>
        /// Ruleset how to parse the input BBCode will
        /// be passed in the ToHTML(string bbCode) function.
        /// </summary>
        public IList<BBTag> Tags { get; private set; }

        public string TextNodeHtmlTemplate { get; private set; }

        /// <summary>
        /// Exception throwing rule for the parser.
        /// </summary>
        public ErrorMode ErrorMode { get; private set; }



        /// <summary>
        /// Initalize a new instance with DISABLED Exception throwing.
        /// </summary>
        /// <param name="tags">Ruleset how to parse the input BBCode will
        /// be passed in the ToHTML(string bbCode) function.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BBCodeParser(IList<BBTag> tags)
            : this(default(ErrorMode), null, tags) { }

        /// <summary>
        /// Initalize a new instance without a custom <see cref="TextNode"/> HTML templace.
        /// </summary>
        /// <param name="errorMode">Exception throwing rule for the parser.</param>
        /// <param name="tags">Ruleset how to parse the input BBCode will
        /// be passed in the ToHTML(string bbCode) function.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BBCodeParser(ErrorMode errorMode, IList<BBTag> tags)
            : this(errorMode, null, tags) { }

        /// <summary>
        /// Initialize a new parser instance with a custom <see cref="TextNode"/> HTML templace.
        /// </summary>
        /// <param name="textNodeHtmlTemplate">Template how to parse the <see cref="TextNode"/>s.</param>
        /// <param name="tags">Ruleset how to parse the input BBCode will
        /// be passed in the ToHTML(string bbCode) function.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BBCodeParser(string textNodeHtmlTemplate, IList<BBTag> tags)
            : this(default(ErrorMode), textNodeHtmlTemplate, tags) { }

        /// <summary>
        /// Initialize a new parser instance.
        /// </summary>
        /// <param name="errorMode">Exception throwing rule for the parser.</param>
        /// <param name="textNodeHtmlTemplate"></param>
        /// <param name="tags">Ruleset how to parse the input BBCode will
        /// be passed in the ToHTML(string bbCode) function.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BBCodeParser(ErrorMode errorMode, string textNodeHtmlTemplate, IList<BBTag> tags)
        {
            if (!Enum.IsDefined(typeof(ErrorMode), errorMode))
                throw new ArgumentOutOfRangeException(nameof(errorMode));

            ErrorMode = errorMode;
            TextNodeHtmlTemplate = textNodeHtmlTemplate;
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
        }



        /// <summary>
        /// Parse the input BBCode to HTML.
        /// </summary>
        /// <param name="bbCode">Raw BBCode.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual string ToHtml(string bbCode)
        {
            if (bbCode == null)
                throw new ArgumentNullException(nameof(bbCode));

            return ParseSyntaxTree(bbCode).ToHtml();
        }

        public virtual SequenceNode ParseSyntaxTree(string bbCode)
        {
            if (bbCode == null)
                throw new ArgumentNullException(nameof(bbCode));

            Stack<SyntaxTreeNode> stack = new Stack<SyntaxTreeNode>();
            SequenceNode rootNode = new SequenceNode();
            stack.Push(rootNode);

            int currentIndex = 0;

            while (currentIndex < bbCode.Length)
            {
                if (MatchStartTag(bbCode, ref currentIndex, stack))
                    continue;

                if (MatchTextNode(bbCode, ref currentIndex, stack))
                    continue;

                if (MatchTagEnd(bbCode, ref currentIndex, stack))
                    continue;

                if (ErrorMode != ErrorMode.ErrorFree)
                    throw new BBCodeParsingException(""); //there is no possible match at the current position

                AppendText(bbCode[currentIndex].ToString(), stack); //if the error free mode is enabled force interpretation as text if no other match could be made
                currentIndex++;
            }

            while (stack.Count > 1) //close all tags that are still open and can be closed implicitly
            {
                var node = (TagNode)stack.Pop();

                if (node.Tag.RequiresClosingTag && ErrorMode == ErrorMode.Strict)
                    throw new BBCodeParsingException(MessagesHelper.GetString("TagNotClosed", node.Tag.Name));
            }

            if (stack.Count != 1)
            {
                throw new BBCodeParsingException(""); //only the root node may be left
            }

            return rootNode;
        }

        /// <summary>
        /// Get the BBCode representation of the given HTML.
        /// </summary>
        /// <param name="htmlCode">Non null HTML string.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public string ToBBCode(string htmlCode)
        {
            if (htmlCode == null)
                throw new ArgumentNullException(nameof(htmlCode));

            return "";
        }


        /// <summary>
        /// Match tag start.
        /// </summary>
        /// <param name="bbCode">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <param name="stack">Non null collection.</param>
        /// <returns>FALSE if the ParseTagStart returned a null <see cref="TagNode"/>
        /// or the topmost <paramref name="stack"/> item is a <see cref="TagNode"/>,
        /// TRUE otherwise.</returns>
        /// <exception cref="BBCodeParsingException"></exception>
        private bool MatchStartTag(string bbCode, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            // Before we do *anything* - if the topmost node on the stack is marked as StopProcessing then
            // don't match anything
            TagNode topmost = stack.Peek() as TagNode;
            if (topmost != null && topmost.Tag.StopProcessing)
            {
                return false;
            }

            int currentIndex = pos;
            TagNode tagNode = ParseTagStart(bbCode, ref currentIndex);

            if (tagNode != null)
            {
                if (tagNode.Tag.EnableIterationElementBehavior)
                {
                    //this element behaves like a list item: it allows tags as content, it auto-closes and it does not nest.
                    //the last property is ensured by closing all currently open tags up to the opening list element

                    bool isThisTagAlreadyOnStack = stack.OfType<TagNode>().Any(n => n.Tag == tagNode.Tag);
                    //if this condition is false, no nesting would occur anyway

                    if (isThisTagAlreadyOnStack)
                    {
                        TagNode openingNode = stack.Peek() as TagNode; //could also be a SequenceNode

                        if (openingNode.Tag != tagNode.Tag
                            && ErrorMode == ErrorMode.Strict
                            && ErrorOrReturn("TagNotMatching", tagNode.Tag.Name, openingNode.Tag.Name))
                            return false;

                        while (true)
                        {
                            TagNode poppedOpeningNode = (TagNode)stack.Pop();

                            if (poppedOpeningNode.Tag != tagNode.Tag)
                            {
                                //a nesting imbalance was detected

                                if (openingNode.Tag.RequiresClosingTag
                                    && ErrorMode == ErrorMode.Strict
                                    && ErrorOrReturn("TagNotMatching", tagNode.Tag.Name, openingNode.Tag.Name))
                                    return false;
                                //close the (wrongly) open tag. we have already popped so do nothing.
                            }
                            else
                            {
                                //the opening node matches the closing node
                                //close the already open li-item. we have already popped. we have already popped so do nothing.
                                break;
                            }
                        }
                    }
                }

                stack.Peek().SubNodes.Add(tagNode);

                if (tagNode.Tag.TagClosingStyle != BBTagClosingStyle.LeafElementWithoutContent)
                    stack.Push(tagNode); //leaf elements have no content - they are closed immediately

                pos = currentIndex;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Match <see cref="TextNode"/>.
        /// </summary>
        /// <param name="bbCode">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <param name="stack">Non null collection.</param>
        /// <returns>TRUE if text found, FALSE otherwise.</returns>
        private bool MatchTextNode(string bbCode, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int currentIndex = pos;

            string text = ParseText(bbCode, ref currentIndex);
            if (text != null)
            {
                AppendText(text, stack);

                pos = currentIndex;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Match tag end.
        /// </summary>
        /// <param name="bbCode">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <param name="stack">Non null collection.</param>
        /// <returns>FALSE if the ParseTagEnd returned a null string,
        /// TRUE otherwise.</returns>
        /// <exception cref="BBCodeParsingException"></exception>
        private bool MatchTagEnd(string bbCode, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int currentIndex = pos;

            string tagEnd = ParseTagEnd(bbCode, ref currentIndex);
            if (tagEnd != null)
            {
                while (true)
                {
                    TagNode openingNode = stack.Peek() as TagNode; //could also be a SequenceNode

                    if (openingNode == null && ErrorOrReturn("TagNotOpened", tagEnd))
                        return false;

                    if (!openingNode.Tag.Name.Equals(tagEnd, StringComparison.OrdinalIgnoreCase))
                    {
                        //a nesting imbalance was detected

                        if (openingNode.Tag.RequiresClosingTag && ErrorOrReturn("TagNotMatching", tagEnd, openingNode.Tag.Name))
                            return false;
                        else
                            stack.Pop();
                    }
                    else
                    {
                        //the opening node properly matches the closing node
                        stack.Pop();
                        break;
                    }
                }
                pos = currentIndex;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Append parameter string as a new <see cref="TextNode"/>
        /// or merge to the last <see cref="TextNode.Text"/> property.
        /// </summary>
        /// <param name="textToAppend">Non null string.</param>
        /// <param name="stack">Non null collection.</param>
        private void AppendText(string textToAppend, Stack<SyntaxTreeNode> stack)
        {
            SyntaxTreeNode currentNode = stack.Peek();

            TextNode lastChild = currentNode.SubNodes.Count == 0 ? null : currentNode.SubNodes[currentNode.SubNodes.Count - 1] as TextNode;
            TextNode newChild;

            if (lastChild == null)
            {
                newChild = new TextNode(textToAppend, TextNodeHtmlTemplate);
                currentNode.SubNodes.Add(newChild);
            }
            else
            {
                newChild = new TextNode(lastChild.Text + textToAppend, TextNodeHtmlTemplate);
                currentNode.SubNodes[currentNode.SubNodes.Count - 1] = newChild;
            }
        }

        /// <summary>
        /// Parse Tag if any near by.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <returns>NULL if not a tag start, or returns the parsed <see cref="TagNode"/>.</returns>
        /// <exception cref="BBCodeParsingException"></exception>
        private TagNode ParseTagStart(string input, ref int pos)
        {
            int currentIndex = pos;

            if (!ParseChar(input, ref currentIndex, '['))
                return null;

            string tagName = ParseName(input, ref currentIndex);
            if (tagName == null)
                return null;

            BBTag tag = Tags.SingleOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
            if (tag == null && ErrorOrReturn("UnknownTag", tagName))
                return null;

            TagNode resultTagNode = new TagNode(tag);

            string defaultAttrValue = ParseAttributeValue(input, ref currentIndex, tag.GreedyAttributeProcessing);
            if (defaultAttrValue != null)
            {
                BBAttribute attr = tag.FindAttribute("");

                if (attr == null && ErrorOrReturn("UnknownAttribute", tag.Name, "\"Default Attribute\""))
                    return null;

                resultTagNode.AttributeValues.Add(attr, defaultAttrValue);
            }

            while (true)
            {
                ParseWhitespace(input, ref currentIndex);

                string attrName = ParseName(input, ref currentIndex);

                if (attrName == null)
                    break;

                string attrVal = ParseAttributeValue(input, ref currentIndex);

                if (attrVal == null && ErrorOrReturn(""))
                    return null;

                if (tag.Attributes == null && ErrorOrReturn("UnknownTag", tag.Name))
                    return null;

                BBAttribute attr = tag.FindAttribute(attrName);

                if (attr == null && ErrorOrReturn("UnknownTag", tag.Name, attrName))
                    return null;

                if (resultTagNode.AttributeValues.ContainsKey(attr) && ErrorOrReturn("DuplicateAttribute", tagName, attrName))
                    return null;

                resultTagNode.AttributeValues.Add(attr, attrVal);
            }

            if (!ParseChar(input, ref currentIndex, ']') && ErrorOrReturn("TagNotClosed", tagName))
                return null;

            ParseWhitespace(input, ref currentIndex);

            pos = currentIndex;

            return resultTagNode;
        }

        /// <summary>
        /// Parse Tag end if any near by.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <returns>NULL if not a tag end, or returns the tag name.</returns>
        /// <exception cref="BBCodeParsingException"></exception>
        private string ParseTagEnd(string input, ref int pos)
        {
            int currentIndex = pos;

            if (!ParseChar(input, ref currentIndex, '['))
                return null;

            if (!ParseChar(input, ref currentIndex, '/'))
                return null;

            string tagName = ParseName(input, ref currentIndex);
            if (tagName == null)
                return null;

            ParseWhitespace(input, ref currentIndex);

            if (!ParseChar(input, ref currentIndex, ']'))
            {
                if (ErrorMode == ErrorMode.ErrorFree)
                    return null;
                else
                    throw new BBCodeParsingException("");
            }

            BBTag tag = Tags.SingleOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            if (tag != null && tag.SuppressFirstNewlineAfter)
            {
                ParseLimitedWhitespace(input, ref currentIndex, 1);
            }

            pos = currentIndex;

            return tagName;
        }

        /// <summary>
        /// Get the text after the given position.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <returns>NULL or a text value.</returns>
        private string ParseText(string input, ref int pos)
        {
            int currentIndex = pos;
            bool escapeFound = false;
            bool anyEscapeFound = false;

            while (currentIndex < input.Length)
            {
                if (input[currentIndex] == '[' && !escapeFound)
                    break;

                if (input[currentIndex] == ']' && !escapeFound)
                {
                    if (ErrorMode == ErrorMode.Strict)
                        throw new BBCodeParsingException(MessagesHelper.GetString("NonescapedChar"));
                }

                if (input[currentIndex] == '\\' && !escapeFound)
                {
                    escapeFound = true;
                    anyEscapeFound = true;
                }
                else if (escapeFound)
                {
                    if (!(input[currentIndex] == '[' || input[currentIndex] == ']' || input[currentIndex] == '\\'))
                    {
                        if (ErrorMode == ErrorMode.Strict)
                            throw new BBCodeParsingException(MessagesHelper.GetString("EscapeChar"));
                    }
                    escapeFound = false;
                }

                currentIndex++;
            }

            if (escapeFound)
            {
                if (ErrorMode == ErrorMode.Strict)
                    throw new BBCodeParsingException("");
            }

            var result = input.Substring(pos, currentIndex - pos);

            if (anyEscapeFound)
            {
                var result2 = new char[result.Length];
                int writePos = 0;
                bool lastWasEscapeChar = false;
                for (int i = 0; i < result.Length; i++)
                {
                    if (!lastWasEscapeChar && result[i] == '\\')
                    {
                        if (i < result.Length - 1)
                        {
                            if (!(result[i + 1] == '[' || result[i + 1] == ']' || result[i + 1] == '\\'))
                                result2[writePos++] = result[i]; //the next char was not escapable. write the slash into the output array
                            else
                                lastWasEscapeChar = true; //the next char is meant to be escaped so the backslash is skipped
                        }
                        else
                        {
                            result2[writePos++] = '\\'; //the backslash was the last char in the string. just write it into the output array
                        }
                    }
                    else
                    {
                        result2[writePos++] = result[i];
                        lastWasEscapeChar = false;
                    }
                }
                result = new string(result2, 0, writePos);
            }

            pos = currentIndex;
            return result == "" ? null : result;
        }

        /// <summary>
        /// Get the attribute/tag name starting from the given position.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <returns>NULL, or the attribute name.</returns>
        private string ParseName(string input, ref int pos)
        {
            int currentIndex = pos;

            for (; currentIndex < input.Length && (char.ToLower(input[currentIndex]) >= 'a' && char.ToLower(input[currentIndex]) <= 'z' || (input[currentIndex]) >= '0' && (input[currentIndex]) <= '9' || input[currentIndex] == '*'); currentIndex++)
            { }

            if (currentIndex - pos == 0)
                return null;

            string name = input.Substring(pos, currentIndex - pos);

            pos = currentIndex;

            return name;
        }

        /// <summary>
        /// Get the attribute value if the next character is a = symbol.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <param name="greedyProcessing"></param>
        /// <returns>NULL if the next character is not = or
        /// the attribute name after the = symbol.</returns>
        private string ParseAttributeValue(string input, ref int pos, bool greedyProcessing = false)
        {
            var currentIndex = pos;

            if (currentIndex >= input.Length || input[currentIndex] != '=')
                return null;

            currentIndex++;

            var endIndex = -1;

            if (!greedyProcessing)
            {
                endIndex = input.IndexOfAny(" []".ToCharArray(), currentIndex);
            }
            else
            {
                endIndex = input.IndexOfAny("[]".ToCharArray(), currentIndex);
            }

            if (endIndex == -1)
            {
                endIndex = input.Length;
            }

            int valStart = pos + 1;

            string result = input.Substring(valStart, endIndex - valStart);

            pos = endIndex;

            return result;
        }

        /// <summary>
        /// Parse whitespaces in the given string.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <returns>TRUE if space found, FALSE otherwise.</returns>
        private bool ParseWhitespace(string input, ref int pos)
        {
            int currentIndex = pos;

            while (currentIndex < input.Length && char.IsWhiteSpace(input[currentIndex]))
            {
                currentIndex++;
            }

            bool found = pos != currentIndex;

            pos = currentIndex;

            return found;
        }

        /// <summary>
        /// Parse whitespaces in the given string.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <param name="maxNewlinesToConsume">Number how many whitespaces should be consumed.</param>
        /// <returns>TRUE if whitespace found, FALSE otherwise.</returns>
        private bool ParseLimitedWhitespace(string input, ref int pos, int maxNewlinesToConsume)
        {
            int currentIndex = pos;
            int consumedNewlines = 0;

            while (currentIndex < input.Length && consumedNewlines < maxNewlinesToConsume)
            {
                char thisChar = input[currentIndex];
                if (thisChar == '\r')
                {
                    currentIndex++;
                    consumedNewlines++;

                    if (currentIndex < input.Length && input[currentIndex] == '\n')
                    {
                        // Windows newline - just consume it
                        currentIndex++;
                    }
                }
                else if (thisChar == '\n')
                {
                    // Unix newline
                    currentIndex++;
                    consumedNewlines++;
                }
                else if (char.IsWhiteSpace(thisChar))
                {
                    // Consume the whitespace
                    currentIndex++;
                }
                else
                {
                    break;
                }
            }

            var found = pos != currentIndex;
            pos = currentIndex;
            return found;
        }

        /// <summary>
        /// Check if the char is at the given position and increase the position
        /// if yes.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">This is where the charater will be checked.</param>
        /// <param name="c">This character what we looking for.</param>
        /// <returns>False if the position greater or equal than the <paramref name="input"/>.Length
        /// or the character is not int the specified position. False otherwise.</returns>
        private bool ParseChar(string input, ref int pos, char c)
        {
            if (input.Length <= pos || input[pos] != c)
                return false;

            pos++;

            return true;
        }

        /// <summary>
        /// Return TRUE or a <see cref="BBCodeParsingException"/> depending on the settings.
        /// </summary>
        /// <param name="msgKey">Non null string which should exists in the Resources.</param>
        /// <param name="parameters">Custom parameters.</param>
        /// <returns>TRUE if error throwing disabled, <see cref="BBCodeParsingException" /> otherwise.</returns>
        /// <exception cref="BBCodeParsingException"></exception>
        private bool ErrorOrReturn(string msgKey, params string[] parameters)
        {
            if (ErrorMode == ErrorMode.ErrorFree)
                return true;
            else
            {
                string message = "";

                if (!string.IsNullOrEmpty(msgKey))
                    message = MessagesHelper.GetString(msgKey, parameters);

                throw new BBCodeParsingException(message);
            }
        }
    }
}

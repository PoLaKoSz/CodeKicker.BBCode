using System;
using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode.SyntaxTree;
using System.Diagnostics;

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
            var rootNode = new SequenceNode();
            stack.Push(rootNode);

            int end = 0;
            while (end < bbCode.Length)
            {
                if (MatchTagEnd(bbCode, ref end, stack))
                    continue;

                if (MatchStartTag(bbCode, ref end, stack))
                    continue;

                if (MatchTextNode(bbCode, ref end, stack))
                    continue;

                if (ErrorMode != ErrorMode.ErrorFree)
                    throw new BBCodeParsingException(""); //there is no possible match at the current position

                AppendText(bbCode[end].ToString(), stack); //if the error free mode is enabled force interpretation as text if no other match could be made
                end++;
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


        private bool MatchTagEnd(string bbCode, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int end = pos;

            var tagEnd = ParseTagEnd(bbCode, ref end);
            if (tagEnd != null)
            {
                while (true)
                {
                    var openingNode = stack.Peek() as TagNode; //could also be a SequenceNode

                    if (openingNode == null && ErrorOrReturn("TagNotOpened", tagEnd))
                        return false;

                    Debug.Assert(openingNode != null); //ErrorOrReturn will either or throw make this stack frame exit

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
                pos = end;
                return true;
            }

            return false;
        }

        private bool MatchStartTag(string bbCode, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            // Before we do *anything* - if the topmost node on the stack is marked as StopProcessing then
            // don't match anything
            var topmost = stack.Peek() as TagNode;
            if (topmost != null && topmost.Tag.StopProcessing)
            {
                return false;
            }

            int end = pos;
            TagNode tagNode = ParseTagStart(bbCode, ref end);

            if (tagNode != null)
            {
                if (tagNode.Tag.EnableIterationElementBehavior)
                {
                    //this element behaves like a list item: it allows tags as content, it auto-closes and it does not nest.
                    //the last property is ensured by closing all currently open tags up to the opening list element

                    var isThisTagAlreadyOnStack = stack.OfType<TagNode>().Any(n => n.Tag == tagNode.Tag);
                    //if this condition is false, no nesting would occur anyway

                    if (isThisTagAlreadyOnStack)
                    {
                        var openingNode = stack.Peek() as TagNode; //could also be a SequenceNode

                        if (openingNode.Tag != tagNode.Tag && ErrorMode == ErrorMode.Strict && ErrorOrReturn("TagNotMatching", tagNode.Tag.Name, openingNode.Tag.Name)) return false;

                        while (true)
                        {
                            var poppedOpeningNode = (TagNode)stack.Pop();

                            if (poppedOpeningNode.Tag != tagNode.Tag)
                            {
                                //a nesting imbalance was detected

                                if (openingNode.Tag.RequiresClosingTag && ErrorMode == ErrorMode.Strict && ErrorOrReturn("TagNotMatching", tagNode.Tag.Name, openingNode.Tag.Name))
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

                pos = end;
                
                return true;
            }

            return false;
        }

        private bool MatchTextNode(string bbCode, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int end = pos;

            var textNode = ParseText(bbCode, ref end);
            if (textNode != null)
            {
                AppendText(textNode, stack);
                pos = end;
                return true;
            }

            return false;
        }

        private void AppendText(string textToAppend, Stack<SyntaxTreeNode> stack)
        {
            var currentNode = stack.Peek();
            var lastChild = currentNode.SubNodes.Count == 0 ? null : currentNode.SubNodes[currentNode.SubNodes.Count - 1] as TextNode;

            TextNode newChild;
            if (lastChild == null)
                newChild = new TextNode(textToAppend, TextNodeHtmlTemplate);
            else
                newChild = new TextNode(lastChild.Text + textToAppend, TextNodeHtmlTemplate);

            if (currentNode.SubNodes.Count != 0 && currentNode.SubNodes[currentNode.SubNodes.Count - 1] is TextNode)
                currentNode.SubNodes[currentNode.SubNodes.Count - 1] = newChild;
            else
                currentNode.SubNodes.Add(newChild);
        }

        private TagNode ParseTagStart(string input, ref int pos)
        {
            var end = pos;

            if (!ParseChar(input, ref end, '['))
                return null;

            var tagName = ParseName(input, ref end);

            if (tagName == null)
                return null;

            var tag = Tags.SingleOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            if (tag == null && ErrorOrReturn("UnknownTag", tagName))
                return null;

            var result = new TagNode(tag);

            var defaultAttrValue = ParseAttributeValue(input, ref end, tag.GreedyAttributeProcessing);
            if (defaultAttrValue != null)
            {
                var attr = tag.FindAttribute("");

                if (attr == null && ErrorOrReturn("UnknownAttribute", tag.Name, "\"Default Attribute\""))
                    return null;

                result.AttributeValues.Add(attr, defaultAttrValue);
            }

            while (true)
            {
                ParseWhitespace(input, ref end);

                var attrName = ParseName(input, ref end);

                if (attrName == null)
                    break;

                var attrVal = ParseAttributeValue(input, ref end);

                if (attrVal == null && ErrorOrReturn(""))
                    return null;

                if (tag.Attributes == null && ErrorOrReturn("UnknownTag", tag.Name))
                    return null;

                var attr = tag.FindAttribute(attrName);

                if (attr == null && ErrorOrReturn("UnknownTag", tag.Name, attrName))
                    return null;

                if (result.AttributeValues.ContainsKey(attr) && ErrorOrReturn("DuplicateAttribute", tagName, attrName))
                    return null;

                result.AttributeValues.Add(attr, attrVal);
            }

            if (!ParseChar(input, ref end, ']') && ErrorOrReturn("TagNotClosed", tagName))
                return null;

            ParseWhitespace(input, ref end);

            pos = end;
            return result;
        }

        private string ParseTagEnd(string input, ref int pos)
        {
            var end = pos;

            if (!ParseChar(input, ref end, '[')) return null;
            if (!ParseChar(input, ref end, '/')) return null;

            var tagName = ParseName(input, ref end);
            if (tagName == null) return null;

            ParseWhitespace(input, ref end);

            if (!ParseChar(input, ref end, ']'))
            {
                if (ErrorMode == ErrorMode.ErrorFree) return null;
                else throw new BBCodeParsingException("");
            }

            var tag = Tags.SingleOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            if (tag != null && tag.SuppressFirstNewlineAfter)
            {
                ParseLimitedWhitespace(input, ref end, 1);
            }

            pos = end;
            return tagName;
        }

        private string ParseText(string input, ref int pos)
        {
            int end = pos;
            bool escapeFound = false;
            bool anyEscapeFound = false;
            while (end < input.Length)
            {
                if (input[end] == '[' && !escapeFound) break;
                if (input[end] == ']' && !escapeFound)
                {
                    if (ErrorMode == ErrorMode.Strict)
                        throw new BBCodeParsingException(MessagesHelper.GetString("NonescapedChar"));
                }

                if (input[end] == '\\' && !escapeFound)
                {
                    escapeFound = true;
                    anyEscapeFound = true;
                }
                else if (escapeFound)
                {
                    if (!(input[end] == '[' || input[end] == ']' || input[end] == '\\'))
                    {
                        if (ErrorMode == ErrorMode.Strict)
                            throw new BBCodeParsingException(MessagesHelper.GetString("EscapeChar"));
                    }
                    escapeFound = false;
                }

                end++;
            }

            if (escapeFound)
            {
                if (ErrorMode == ErrorMode.Strict)
                    throw new BBCodeParsingException("");
            }

            var result = input.Substring(pos, end - pos);

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

            pos = end;
            return result == "" ? null : result;
        }

        private static string ParseName(string input, ref int pos)
        {
            int end = pos;
            for (; end < input.Length && (char.ToLower(input[end]) >= 'a' && char.ToLower(input[end]) <= 'z' || (input[end]) >= '0' && (input[end]) <= '9' || input[end] == '*'); end++) ;
            if (end - pos == 0) return null;

            var result = input.Substring(pos, end - pos);
            pos = end;
            return result;
        }

        private static string ParseAttributeValue(string input, ref int pos, bool greedyProcessing = false)
        {
            var end = pos;

            if (end >= input.Length || input[end] != '=') return null;
            end++;

            var endIndex = -1;

            if (!greedyProcessing)
            {
                endIndex = input.IndexOfAny(" []".ToCharArray(), end);
            }
            else
            {
                endIndex = input.IndexOfAny("[]".ToCharArray(), end);
            }

            if (endIndex == -1) endIndex = input.Length;

            var valStart = pos + 1;
            var result = input.Substring(valStart, endIndex - valStart);
            pos = endIndex;
            return result;
        }

        private static bool ParseWhitespace(string input, ref int pos)
        {
            int end = pos;
            while (end < input.Length && char.IsWhiteSpace(input[end]))
                end++;

            var found = pos != end;
            pos = end;
            return found;
        }

        private static bool ParseLimitedWhitespace(string input, ref int pos, int maxNewlinesToConsume)
        {
            int end = pos;
            int consumedNewlines = 0;

            while (end < input.Length && consumedNewlines < maxNewlinesToConsume)
            {
                char thisChar = input[end];
                if (thisChar == '\r')
                {
                    end++;
                    consumedNewlines++;

                    if (end < input.Length && input[end] == '\n')
                    {
                        // Windows newline - just consume it
                        end++;
                    }
                }
                else if (thisChar == '\n')
                {
                    // Unix newline
                    end++;
                    consumedNewlines++;
                }
                else if (char.IsWhiteSpace(thisChar))
                {
                    // Consume the whitespace
                    end++;
                }
                else
                {
                    break;
                }
            }

            var found = pos != end;
            pos = end;
            return found;
        }

        private static bool ParseChar(string input, ref int pos, char c)
        {
            if (pos >= input.Length || input[pos] != c) return false;
            pos++;
            return true;
        }

        private bool ErrorOrReturn(string msgKey, params string[] parameters)
        {
            if (ErrorMode == ErrorMode.ErrorFree) return true;
            else throw new BBCodeParsingException(string.IsNullOrEmpty(msgKey) ? "" : MessagesHelper.GetString(msgKey, parameters));
        }
    }
}

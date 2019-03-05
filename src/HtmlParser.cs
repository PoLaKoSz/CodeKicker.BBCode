using CodeKicker.BBCode.HtmlComponents;
using CodeKicker.BBCode.SyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode
{
    public class HtmlParser
    {
        private readonly char _startTag;
        private readonly char _endTag;
        private readonly IExceptions _exception;

        /// <summary>
        /// Ruleset how to parse the input BBCode will
        /// be passed in the ToHTML(string bbCode) function.
        /// </summary>
        private readonly IReadOnlyCollection<HtmlTag> _tagRules;



        /// <summary>
        /// Initalize a new instance.
        /// </summary>
        /// <param name="tags">Ruleset how to parse the input HTML will
        /// be passed in the ToHTML(string bbCode) function.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HtmlParser(IReadOnlyCollection<HtmlTag> tags)
        {
            _tagRules = tags ??
                throw new ArgumentNullException(nameof(tags), "Parser initialized with null!");

            _exception = new StrictParsing();

            _startTag = '<';
            _endTag = '>';
        }



        /// <summary>
        /// Get the BBCode representation of the given HTML.
        /// </summary>
        /// <param name="htmlCode">Non null HTML string.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public string ToBBCode(string htmlCode)
        {
            if (htmlCode == null)
                throw new ArgumentNullException(nameof(htmlCode), "Parser can't work with null!");

            return ToTree(htmlCode).ToBBCode();
        }


        private SequenceNode ToTree(string htmlCode)
        {
            if (htmlCode == null)
                throw new ArgumentNullException(nameof(htmlCode), "Parser can't work with null!");

            return ParseSyntaxTree(htmlCode);
        }

        private SequenceNode ParseSyntaxTree(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            foreach (var tag in _tagRules)
            {
                tag.CheckUserParameters();
            }

            Stack<SyntaxTreeNode> stack = new Stack<SyntaxTreeNode>();
            SequenceNode rootNode = new SequenceNode();
            stack.Push(rootNode);

            int currentIndex = 0;

            while (currentIndex < input.Length)
            {
                if (MatchStartTag(input, ref currentIndex, stack))
                    continue;

                if (MatchTextNode(input, ref currentIndex, stack))
                    continue;

                if (MatchTagEnd(input, ref currentIndex, stack))
                    continue;

                AppendText(input[currentIndex].ToString(), stack); //if the error free mode is enabled force interpretation as text if no other match could be made
                currentIndex++;
            }

            while (stack.Count > 1) //close all tags that are still open and can be closed implicitly
            {
                var node = (Node)stack.Pop();

                _exception.TagNotClosed(node);
            }

            if (stack.Count != 1)
            {
                throw new BBCodeParsingException("There should be only one root node!"); //only the root node may be left
            }

            return rootNode;
        }

        /// <summary>
        /// Tries to find a tag in the given position.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <param name="stack">Non null collection.</param>
        /// <returns>FALSE if the ParseTagStart returned a null <see cref="Node"/>
        /// or the topmost <paramref name="stack"/> item is a <see cref="Node"/>,
        /// TRUE otherwise.</returns>
        /// <exception cref="BBCodeParsingException"></exception>
        private bool MatchStartTag(string input, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int currentIndex = pos;
            Node tagNode = ParseTagStart(input, ref currentIndex, stack);

            if (tagNode != null)
            {
                stack.Peek().SubNodes.Add(tagNode);

                tagNode.Tag.ShouldBeOnStack(stack, tagNode);

                pos = currentIndex;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Match <see cref="TextNode"/>.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <param name="stack">Non null collection.</param>
        /// <returns>TRUE if text found, FALSE otherwise.</returns>
        private bool MatchTextNode(string input, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int currentIndex = pos;

            string text = ParseText(input, ref currentIndex);
            if (text != null)
            {
                var parentNode = stack.Peek() as Node;

                if (parentNode == null || parentNode.Tag.CanHaveTextNode(text))
                    AppendText(text, stack);

                pos = currentIndex;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Match tag end.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <param name="stack">Non null collection.</param>
        /// <returns>FALSE if the ParseTagEnd returned a null string,
        /// TRUE otherwise.</returns>
        /// <exception cref="BBCodeParsingException"></exception>
        private bool MatchTagEnd(string input, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int currentIndex = pos;

            var currentNode = stack.Peek() as Node;

            string tagEnd = currentNode.Tag.ParseTagEnd(input, ref currentIndex, _exception);
            if (tagEnd != null)
            {
                while (true)
                {
                    SyntaxTreeNode openingNode = stack.Peek() as SyntaxTreeNode;

                    if (openingNode == null && _exception.TagNotOpened(tagEnd))
                        return false;

                    // The opening node properly matches the closing node
                    stack.Pop();

                    break;
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

            var childNodes = currentNode.SubNodes;

            if (childNodes.Count == 0 || !(childNodes.Last() is TextNode))
            {
                childNodes.Add(new TextNode(textToAppend));
            }
            else
            {
                TextNode newChild = new TextNode((childNodes.Last() as TextNode).Text + textToAppend);
                childNodes[childNodes.Count - 1] = newChild;
            }
        }

        /// <summary>
        /// Parse Tag if any near by.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <returns>NULL if not a tag start, or returns the parsed <see cref="Node"/>.</returns>
        /// <exception cref="BBCodeParsingException"></exception>
        private Node ParseTagStart(string input, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int currentIndex = pos;

            if (!ParseChar(input, ref currentIndex, _startTag))
                return null;

            string tagName = ParseName(input, ref currentIndex);
            if (tagName == null)
                return null;

            Node resultNode = null;

            foreach (var rule in _tagRules)
            {
                var attrs = ParseAttributes(input, ref currentIndex);
                resultNode = rule.IsThisTag(tagName, attrs, stack, _exception);

                if (resultNode != null)
                    break;
            }

            if (resultNode == null && _exception.UnknownTag(tagName, currentIndex - tagName.Length))
                return null;

            resultNode.Index = currentIndex - tagName.Length;

            if (!ParseChar(input, ref currentIndex, _endTag) && _exception.TagNotClosed(tagName))
                return null;

            resultNode.Tag.ResultTag.AfterOpenTagClosed(input, ref currentIndex);

            pos = currentIndex;

            return resultNode;
        }

        private Dictionary<string, string> ParseAttributes(string input, ref int currentIndex)
        {
            var attrs = new Dictionary<string, string>();

            while (true)
            {
                ParseWhitespace(input, ref currentIndex);

                string attrName = ParseName(input, ref currentIndex);

                if (attrName == null)
                    break;

                string attrVal = ParseAttributeValue(input, ref currentIndex);

                if (attrVal == null)
                    break;

                try
                {
                    attrs.Add(attrName, attrVal);
                }
                catch (ArgumentException)
                {
                    //_exception.DuplicateAttribute(tagName, attrName);
                }
            }

            return attrs;
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
            int escapeIndex = -1;
            bool escapeFound = false;
            bool anyEscapeFound = false;

            while (currentIndex < input.Length)
            {
                if (input[currentIndex] == _startTag && !escapeFound)
                    break;

                if (input[currentIndex] == _endTag && !escapeFound)
                {
                    _exception.NonEscapedChar();
                }

                if (input[currentIndex] == '\\' && !escapeFound)
                {
                    escapeFound = true;
                    escapeIndex = currentIndex;
                    anyEscapeFound = true;
                }
                else if (escapeFound)
                {
                    if (!(input[currentIndex] == _startTag || input[currentIndex] == _endTag || input[currentIndex] == '\\'))
                    {
                        _exception.EscapeChar();
                    }
                    escapeFound = false;
                }

                currentIndex++;
            }

            if (escapeFound)
            {
                // TODO : _exception.EscapeException("Escape character found at position {0}", escapeIndex);
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
                            if (!(result[i + 1] == _startTag || result[i + 1] == _endTag || result[i + 1] == '\\'))
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
        internal static string ParseName(string input, ref int pos)
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
        /// <returns>NULL if the next character is not = or
        /// the attribute name after the = symbol.</returns>
        private string ParseAttributeValue(string input, ref int pos)
        {
            var currentIndex = pos;

            if (currentIndex >= input.Length || input[currentIndex] != '=')
                return null;

            currentIndex += 2;

            while (!ThisChar(input, currentIndex, '"'))
            {
                currentIndex++;
            }

            var endIndex = currentIndex;

            if (endIndex == -1)
            {
                endIndex = input.Length;
            }

            int valStart = pos + 2;

            string result = input.Substring(valStart, endIndex - valStart);

            pos = endIndex + 1;

            return result;
        }

        /// <summary>
        /// Parse whitespaces in the given string.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">Starting position where the operation begins.</param>
        /// <returns>TRUE if space found, FALSE otherwise.</returns>
        internal static bool ParseWhitespace(string input, ref int pos)
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
        /// Check if the char is at the given position and increase the position
        /// if yes.
        /// </summary>
        /// <param name="input">Non null string.</param>
        /// <param name="pos">This is where the charater will be checked.</param>
        /// <param name="c">This character what we looking for.</param>
        /// <returns>False if the position greater or equal than the <paramref name="input"/>.Length
        /// or the character is not int the specified position. False otherwise.</returns>
        internal static bool ParseChar(string input, ref int pos, char c)
        {
            if (input.Length <= pos || input[pos] != c)
                return false;

            pos++;

            return true;
        }

        private bool ThisChar(string input, int position, char c)
        {
            return input[position] == c;
        }
    }
}

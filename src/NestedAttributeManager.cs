using CodeKicker.BBCode.Tags.BB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeKicker.BBCode
{
    internal static class NestedAttributeManager
    {
        private static readonly string _prefix;



        static NestedAttributeManager()
        {
            _prefix = "attr_";
        }



        public static string Generate(Attribute attribute)
        {
            return $"{_prefix}{attribute.Name}";
        }

        public static string Replace(string content, List<Attribute> attrs)
        {
            string pattern = "";

            foreach (var attr in attrs)
            {
                string attrName = attr.Name;

                pattern += 
                    $"{GetRegexOpenTag(attrName)}" +
                        $"(?<{attrName}>.+)?" +
                    $"{GetRegexCloseTag(attrName)}";
            }

            Regex rgx = new Regex(pattern);

            var match = rgx.Match(content);

            foreach (var attr in attrs)
            {
                var match2 = match.Groups[attr.Name];

                string attrVal = match2.Value;

                attr.Value = attrVal;

                string attrName = attr.Name;
            }


            var stringBuilder = new StringBuilder(content);

            for (int index = match.Groups.Count - 1; 0 < index; index--)
            {
                var currentMatch = match.Groups[index];

                int startIndex = Find('[', stringBuilder.ToString(), currentMatch.Index, i => i-=1);
                int length     = Find(']', stringBuilder.ToString(), currentMatch.Index, i => i+=1) + 1 - startIndex;

                stringBuilder.Remove(startIndex, length);
            }

            return stringBuilder.ToString();
        }

        private static string GetRegexOpenTag(string tagName)
        {
            return $"\\{SimpleTag.OpenSymbol}{_prefix}{tagName}\\{SimpleTag.CloseSymbol}";
        }

        private static string GetRegexCloseTag(string tagName)
        {
            return $"\\{SimpleTag.OpenSymbol}/{_prefix}{tagName}\\{SimpleTag.CloseSymbol}";
        }

        /// <summary>
        /// Finds the first occurance of the given charachter.
        /// </summary>
        /// <param name="c">The character that we are looking for.</param>
        /// <param name="input">The non null string where we looking for.</param>
        /// <param name="index">Starting index from the search begins.</param>
        /// <param name="direction">Can be incremental (left to right) or
        /// decremental (right to left).</param>
        /// <returns>-1 if not found, or the c index in the given string.</returns>
        private static int Find(char c, string input, int index, Func<int, int> direction)
        {
            for (int i = index; i < input.Length; i = direction(i))
            {
                if (input[i].Equals(c))
                    return i;
            }

            return -1;
        }
    }
}

using System;

namespace CodeKicker.BBCode
{
    public class BBTag
    {
        public string Name { get; private set; }

        public string OpenTagTemplate { get; private set; }

        public string CloseTagTemplate { get; private set; }

        public bool AutoRenderContent { get; private set; }

        public bool StopProcessing { get; set; }

        public bool GreedyAttributeProcessing { get; set; }

        public bool SuppressFirstNewlineAfter { get; set; }

        public bool EnableIterationElementBehavior { get; set; }

        public bool RequiresClosingTag
        {
            get { return TagClosingStyle == BBTagClosingStyle.RequiresClosingTag; }
        }

        public BBTagClosingStyle TagClosingStyle { get; private set; }

        public Func<string, string> ContentTransformer { get; private set; } //allows for custom modification of the tag content before rendering takes place

        public BBAttribute[] Attributes { get; private set; }

        public const string ContentPlaceholderName = "content";



        public BBTag(string name, string openTagTemplate, string closeTagTemplate, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, true, true, attributes) { }

        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, bool requireClosingTag, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, autoRenderContent, requireClosingTag, null, attributes) { }

        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, bool requireClosingTag, Func<string, string> contentTransformer, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, autoRenderContent, requireClosingTag ? BBTagClosingStyle.RequiresClosingTag : BBTagClosingStyle.AutoCloseElement, contentTransformer, attributes) { }

        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, BBTagClosingStyle tagClosingClosingStyle, Func<string, string> contentTransformer, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, autoRenderContent, tagClosingClosingStyle, contentTransformer, false, attributes) { }

        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, BBTagClosingStyle tagClosingClosingStyle, Func<string, string> contentTransformer, bool enableIterationElementBehavior, params BBAttribute[] attributes)
        {
            if (!Enum.IsDefined(typeof(BBTagClosingStyle), tagClosingClosingStyle))
                throw new ArgumentException(nameof(tagClosingClosingStyle));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            OpenTagTemplate = openTagTemplate ?? throw new ArgumentNullException(nameof(openTagTemplate));
            CloseTagTemplate = closeTagTemplate ?? throw new ArgumentNullException(nameof(closeTagTemplate));
            AutoRenderContent = autoRenderContent;
            TagClosingStyle = tagClosingClosingStyle;
            ContentTransformer = contentTransformer;
            EnableIterationElementBehavior = enableIterationElementBehavior;
            Attributes = attributes ?? new BBAttribute[0];
        }



        public BBAttribute FindAttribute(string name)
        {
            return Array.Find(Attributes, a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
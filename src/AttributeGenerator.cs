using System.Text;

namespace CodeKicker.BBCode
{
    public class AttributeGenerator
    {
        private readonly StringBuilder _pattern;



        public AttributeGenerator()
        {
            _pattern = new StringBuilder();
        }



        public AttributeGenerator JustValue(string attrName)
        {
            _pattern
                .Append(CustomAttributeManager.StartSign)
                .Append(attrName)
                .Append(CustomAttributeManager.EndSign);

            return this;
        }

        public AttributeGenerator KeyValue(string attrName)
        {
            _pattern
                .Append(" ")
                .Append(attrName)
                .Append("=")
                .Append("\"");

            JustValue(attrName);

            _pattern.Append("\"");

            return this;
        }

        public AttributeGenerator CustomValue(string value)
        {
            _pattern.Append(value);

            return this;
        }

        public override string ToString()
        {
            return _pattern.ToString();
        }
    }
}

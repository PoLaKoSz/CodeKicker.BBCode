using System.Collections.Generic;

namespace CodeKicker.BBCode
{
    class CustomAttributeManager
    {
        internal static readonly string StartSign;
        internal static readonly string EndSign;
        internal static readonly string Default;



        static CustomAttributeManager()
        {
            StartSign = "${";
            EndSign   = "}";
            Default   = $"{StartSign}default{EndSign}";
        }


        
        public static string Replace(string template, List<Attribute> attributes)
        {
            string result = template;

            for (int i = 0; i < attributes.Count; i++)
            {
                Attribute attr = attributes[i];

                string oldValue = StartSign + attr.Name + EndSign;
                string newValue = attr.GetValue();

                result = result.Replace(oldValue, newValue);
            }

            if (attributes.Count == 1)
                result = result.Replace(Default, attributes[0].GetValue());

            return result;
        }
    }
}

using System;

namespace CodeKicker.BBCode
{
    public class ExactAttribute : Attribute
    {
        /// <summary>
        /// Non null string.
        /// </summary>
        internal new string Value { get => base.Value; set { UpdateValue(value); } }



        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <param name="name">Non null name of the attribute.</param>
        /// <param name="value">Non null value of the attribute.</param>
        public ExactAttribute(string name, string value)
            : base(name, value, t => t) { }

        /// <summary>
        /// Initializes a new attribute with it's name.
        /// </summary>
        /// <param name="name">Non null name of this attribute.
        /// For example in HTML: class, id, etc.</param>
        public ExactAttribute(string name, string value, Func<string, string> valueFormatter)
            : base(name, value, valueFormatter){ }



        private void UpdateValue(string value)
        {
            if (HasValue)
                return;

            base.Value = value;
        }
    }
}

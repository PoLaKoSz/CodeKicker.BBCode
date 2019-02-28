using System;

namespace CodeKicker.BBCode
{
    public class Attribute
    {
        /// <summary>
        /// Non null name of this attribute. For example in HTML: class, id, etc.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Non null clean value without the custom format.
        /// </summary>
        internal string Value { get; set; }

        internal bool HasValue => !Value.Equals("");

        private readonly Func<string, string> _valueFormatter;



        /// <summary>
        /// Initializes a new attribute with it's name.
        /// </summary>
        /// <param name="name">Non null name of this attribute.
        /// For example in HTML: class, id, etc.</param>
        public Attribute(string name)
            : this(name, t => t) { }

        /// <summary>
        /// Initializes a new attribute with it's name.
        /// </summary>
        /// <param name="name">Non null name of this attribute.
        /// For example in HTML: class, id, etc.</param>
        public Attribute(string name, Func<string, string> valueFormatter)
            : this(name, "", valueFormatter){ }

        /// <summary>
        /// Initializes a new attribute with it's name.
        /// </summary>
        /// <param name="name">Non null name of this attribute.
        /// For example in HTML: class, id, etc.</param>
        public Attribute(string name, string value, Func<string, string> valueFormatter)
        {
            Name = name;
            Value = value;
            _valueFormatter = valueFormatter;
        }

        public Attribute(Attribute old)
            : this(old.Name, old._valueFormatter)
        {
            Value = old.Value;
        }



        /// <summary>
        /// Gets the formatted value.
        /// </summary>
        /// <returns>Non null string.</returns>
        public string GetValue()
        {
            return _valueFormatter(Value);
        }

        public override string ToString()
        {
            return Name + "=" + Value;
        }
    }
}

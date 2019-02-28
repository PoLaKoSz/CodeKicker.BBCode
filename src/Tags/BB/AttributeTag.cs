using System;

namespace CodeKicker.BBCode.Tags.BB
{
    class AttributeTag : SimpleTag
    {
        public AttributeTag(Attribute attribute)
            : base(NestedAttributeManager.Generate(attribute)) { }



        public override void AfterOpenTagClosed(string input, ref int currentIndex) { }
    }
}

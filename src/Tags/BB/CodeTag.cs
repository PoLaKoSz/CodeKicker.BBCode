using System;

namespace CodeKicker.BBCode.Tags.BB
{
    public class CodeTag : SimpleTag
    {
        public CodeTag()
            : base("code") { }



        public override void AfterOpenTagClosed(string input, ref int currentIndex) {}
    }
}

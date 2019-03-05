using System;

namespace CodeKicker.BBCode.Tests.Unit
{
    class ExceptionMessages : MessagesHelper
    {
        public new string TagNotClosed(string tagName, int index)
        {
            return base.TagNotClosed(tagName, index);
        }
    }
}

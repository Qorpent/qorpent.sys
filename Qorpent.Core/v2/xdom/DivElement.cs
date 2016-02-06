using System;

namespace qorpent.v2.xdom {
    public class DivElement : ContentElement {
        public DivElement(params object[] content) : base(content) { }
    }

    public abstract class H_Element : ContentElement {
        protected H_Element(params object[] content) : base(content) { }
    }
    public class H1Element : H_Element
    {
        public H1Element(params object[] content) : base(content) { }
    }
    public class H2Element : H_Element
    {
        public H2Element(params object[] content) : base(content) { }
    }
    public class H3Element : H_Element
    {
        public H3Element(params object[] content) : base(content) { }
    }
    public class H4Element : H_Element
    {
        public H4Element(params object[] content) : base(content) { }
    }
}
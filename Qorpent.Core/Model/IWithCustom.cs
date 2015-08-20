using System.Collections.Generic;

namespace Qorpent.Model {
    public interface IWithCustom {
        IDictionary<string,object> Custom { get; set; }
    }

    public interface IWithDefinition {
        object Definition { get; set; }
    }
}
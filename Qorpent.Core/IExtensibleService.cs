using System.Collections.Generic;

namespace Qorpent {
    public interface IExtensibleService<TExtensionType> {
        IEnumerable<TExtensionType> GetExtensions();
        void RegisterExtension(TExtensionType extension);
        void RemoveExtension(TExtensionType extension);
        void ClearExtensions();
    }
}
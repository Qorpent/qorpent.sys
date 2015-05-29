using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Qorpent.IoC;
using Qorpent.Model;

namespace Qorpent {
    public abstract class ExtensibleServiceBase<TExtensionType> :ServiceBase, IExtensibleService<TExtensionType> {
        private IList<TExtensionType> _extensions;

        [Inject]
        protected IList<TExtensionType> Extensions {
            get { return _extensions ?? (_extensions = new List<TExtensionType>()); }
            set { _extensions = value; }
        }

        public override void OnContainerCreateInstanceFinished() {
            OnChangeExtensions();
        }

        protected virtual void OnChangeExtensions() {
            lock (this) {
                if (typeof (IWithIndex).IsAssignableFrom(typeof (TExtensionType))) {
                    Extensions = Extensions.OrderBy(_ => {
                        var index = _ as IWithIndex;
                        return index != null ? index.Idx : 0;
                    }).ToList();
                }
            }
        }

        public IEnumerable<TExtensionType> GetExtensions() {
            return Extensions.ToArray();
        }

        public void RegisterExtension(TExtensionType extension) {
            lock (this) {
                if (!Extensions.Contains(extension)) {
                    Extensions.Add(extension);
                    OnChangeExtensions();
                }
            }
        }

        public void RemoveExtension(TExtensionType extension) {
            lock (this) {
                if (Extensions.Contains(extension)) {
                    Extensions.Remove(extension);
                    OnChangeExtensions();
                }
            }
        }

        public void ClearExtensions() {
            lock (this) {
                Extensions.Clear();
            }
        }
    }
}
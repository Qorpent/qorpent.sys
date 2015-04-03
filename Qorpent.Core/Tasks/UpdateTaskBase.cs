using System.Linq;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.IO;

namespace Qorpent.Tasks {
    public abstract class UpdateTaskBase : TaskBase {
        /// <summary>
        ///     Исходный дескриптор
        /// </summary>
        public IVersionedDescriptor Source { get; set; }

        /// <summary>
        ///     Целевой дескриптор
        /// </summary>
        public IVersionedDescriptor Target { get; set; }

        public override void Refresh() {
            base.Refresh();
            if (null != Source) {
                Source.Refresh();
            }
            if (null != Target) {
                Target.Refresh();
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override bool RequireExecution() {
            if (null != Target && null != Source) {
                if (Target.Hash == "INIT") {
                    return true;
                }
                if (Target.Hash == Source.Hash) {
                    return false;
                }
                return Source.Version > Target.Version;
            }
            return base.RequireExecution();
        }

        protected override bool HasUpdatedOnce() {
            if (null != Target) {
                return Target.Hash != "INIT";
            }
            return base.HasUpdatedOnce();
        }

        protected override void CheckoutParameters() {
            if (null != Source) {
                if (null == Requirements || 0 == Requirements.Length) {
                    var requirements = Source.Header.Elements("require").ToArray();
                    Requirements = requirements.Select(_ => _.Attr("code")).ToArray();
                }
                RunOnce = RunOnce || Source.Header.Attr("updateonce").ToBool();
                IgnoreErrors = IgnoreErrors || Source.Header.Attr("ignoreerrors").ToBool();
                var idx = Source.Header.Attr("idx").ToInt();
                if (0 != idx) {
                    Idx = idx;
                }
            }
        }
    }
}
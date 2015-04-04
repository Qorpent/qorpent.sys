using System.IO;
using Qorpent.Tasks;
using Qorpent.Utils.IO;

namespace Qorpent.Core.Tests.Tasks {
    /// <summary>
    /// </summary>
    public class TestUpdater : UpdateTaskBase {
        private readonly TextWriter o;

        public TestUpdater(TextWriter sw, string fileFrom, string fileTo) {
            o = sw ?? new StringWriter();
            Source = new FileDescriptorEx {FullName = fileFrom, AllowNotExisted = true};
            Target = new FileDescriptorEx {FullName = fileTo, AllowNotExisted = true};
        }

        protected override void InternalWork() {
            WriteName();
            File.Copy(Source.FullName, Target.FullName, true);
            o.WriteLine("copy " + Path.GetFileName(Source.FullName) + " to " + Path.GetFileName(Target.FullName));
        }

        protected override void OnStateChange() {
            WriteName();
            o.WriteLine("S:" + State);
        }

        private void WriteName() {
            if (!string.IsNullOrWhiteSpace(Name)) {
                o.Write(Name + " - ");
            }
        }
    }
}
using System;
using System.IO;
using Qorpent.Tasks;

namespace Qorpent.Core.Tests.Tasks {
    /// <summary>
    /// </summary>
    public class TestTask : TaskBase {
        private readonly TextWriter o;

        public TestTask(TextWriter output = null) {
            o = output ?? new StringWriter();
        }

        public bool DoError { get; set; }
        public bool WasRunOnce { get; set; }
        public bool NoRun { get; set; }

        protected override bool HasUpdatedOnce() {
            return WasRunOnce;
        }

        protected override bool RequireExecution() {
            return !NoRun;
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

        protected override void OnError() {
            WriteName();
            o.WriteLine("OnError");
        }

        protected override void InternalWork() {
            if (DoError) {
                throw new Exception("Error");
            }
            WriteName();
            o.WriteLine("InternalWork");
        }
    }
}
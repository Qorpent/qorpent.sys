using System.IO;

namespace Qorpent.Log.NewLog {
    public class TextWriterAppender : AppenderBase {
        private TextWriter _output;

        public TextWriterAppender() {
            
        }

        public TextWriterAppender(TextWriter output) {
            _output = output;
        }

        public void SetOutput(TextWriter output) {
            Flush();
            _output = output;
        }

        protected override void InternalWrite(LoggyMessage message) {
            _output?.WriteLine(GetText(message));
        }

        public override void Flush() {
            base.Flush();
            _output?.Flush();
        }
    }
}
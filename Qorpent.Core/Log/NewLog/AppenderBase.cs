using Qorpent.Utils.Extensions;

namespace Qorpent.Log.NewLog {
    public abstract class AppenderBase : ILogAppender {
        public string Format { get; set; }

        public virtual void Dispose() {
            Flush();
        }

        protected string GetText(LoggyMessage message) {
            var result = message.Message;
            if (!string.IsNullOrWhiteSpace(Format)) {
                result = Format.Interpolate(message);
            }
            return result;
        }

        public abstract void Write(LoggyMessage message);

        public virtual void Flush() {
            
        }

        public LogLevel Level { get; set; }
    }
}
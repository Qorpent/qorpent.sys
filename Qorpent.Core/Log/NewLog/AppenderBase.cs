using System;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace Qorpent.Log.NewLog {
    public abstract class AppenderBase : ILogAppender {
        public string Format { get; set; }

        public string Id = System.Guid.NewGuid().ToString();

        public ILoggyManager Manager { get; set; }

        public virtual void Dispose() {
            Flush();
        }

        protected string GetText(LoggyMessage message) {
            if (string.IsNullOrWhiteSpace(message.Message) && null != message.Exception) {
                message.Message =
                    message.Exception.GetType().Name + ": " +
                    message.Exception.Message + Environment.NewLine;
                if (null != message.Exception.StackTrace) {
                    message.Message +=
                   message.Exception.StackTrace.Substring(0, Math.Min(message.Exception.StackTrace.Length, 200)) + "...";
                }
                
            }
            var result = message.Message;
            
            if (!string.IsNullOrWhiteSpace(Format)) {
                result = Format.Interpolate(message);
            }
            return result;
        }

        public  void Write(LoggyMessage message) {
            try {
                if (message.Visited.Contains(this.Id)) {
                    return;
                }
                InternalWrite(message);
                message.Visited.Add(this.Id);
            }
            catch (Exception ex) {
                (Manager ?? Loggy.Manager).Get("_failsafe").Error(new{appendertype=GetType().Name,error=ex.ToString()}.stringify());
            }
        }

        protected abstract void InternalWrite(LoggyMessage message);

        public virtual void Flush() {
            
        }

        public LogLevel Level { get; set; }
    }
}
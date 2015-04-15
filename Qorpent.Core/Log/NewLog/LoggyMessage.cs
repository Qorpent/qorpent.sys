using System;
using System.Linq;

namespace Qorpent.Log.NewLog {
    public class LoggyMessage {
        private LogLevel _level;
        private object[] _args;
        private string _message;
        private Exception _exception;
        public string LoggerName { get; set; }

        public LoggyMessage(LogLevel level, params object[] args) {
            _level = level;
            _args = args;
        }

        public LogLevel Level {
            get {
                return _level;
                
            }
            set { _level = value; }
        }

        public string Message {
            get {
                if (_message == null) {
                    _message = _args.OfType<string>().FirstOrDefault() ?? "";
                }
                return _message;
            }
            set { _message = value; }
        }
        static Exception Stub  = new Exception();
        public Exception Exception {
            get {
                if (null == _exception) {
                    _exception = _args.OfType<Exception>().FirstOrDefault() ?? Stub;
                }
                return _exception==Stub ? null : _exception;
            }
            set { _exception = value; }
        }
    }
}
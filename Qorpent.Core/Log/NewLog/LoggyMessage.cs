using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Log.NewLog {
    public class LoggyMessage {
        private object[] _args;
        private string _message;
        private Exception _exception;
        private DateTime _timestamp;
        private string _userName;
        public string LoggerName { get; set; }

        public IList<string> Visited = new List<string>();  

        public LoggyMessage(LogLevel level, params object[] args) {
            Level = level;
            _args = args;
        }

        public LogLevel Level { get; set; }

        public string UserName {
            get {
                if (null == _userName) {
                    _userName = "unknown";
                    if (Applications.Application.HasCurrent) {
                        _userName = Applications.Application.Current.Principal.CurrentUser.Identity.Name;
                    }
                }
                return _userName;
            }
            set { _userName = value; }
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
        static readonly Exception Stub  = new Exception();
        public Exception Exception {
            get {
                if (null == _exception) {
                    _exception = _args.OfType<Exception>().FirstOrDefault() ?? Stub;
                }
                return _exception==Stub ? null : _exception;
            }
            set { _exception = value; }
        }

        public string ErrorMessage {
            get { return null == Exception ? "" : Exception.Message; }
        }

        public DateTime Timestamp {
            get {
                if (_timestamp.Year <= 1900) {
                    _timestamp = _args.OfType<DateTime>().FirstOrDefault();
                    if (_timestamp.Year <= 1900) {
                        _timestamp = DateTime.Now;
                    }
                }
                return _timestamp;
            }
            set { _timestamp = value; }
        }

        public int Year {
            get { return Timestamp.Year; }
        }
        public int Month
        {
            get { return Timestamp.Month; }
        }
        public int Day
        {
            get { return Timestamp.Day; }
        }
        public int Hour
        {
            get { return Timestamp.Hour; }
        }
        public int Minute
        {
            get { return Timestamp.Minute; }
        }
        public int Second
        {
            get { return Timestamp.Second; }
        }
        public int MSecond
        {
            get { return Timestamp.Millisecond; }
        }

    }
}
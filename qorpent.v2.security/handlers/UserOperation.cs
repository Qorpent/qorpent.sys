using System;
using System.Linq;
using System.Threading;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log;
using Qorpent.Log.NewLog;

namespace qorpent.v2.security.handlers {
    public class UserOperation : HandlerBase, IRequestHandler {
        private ILoggyManager _loggyManager;
        private ILoggy _userOpLog;
        public LogLevel ErrorLevel = LogLevel.Error;
        public LogLevel ExceptionLevel = LogLevel.None;
        public bool IsUserOperation = true;
        public LogLevel SuccessLevel = LogLevel.Trace;
        public bool TreatFalseAsError = false;

        protected virtual bool IsError(HandlerResult result) {
            var iserror = result.State != 200 || (false.Equals(result.Result) && TreatFalseAsError);
            return iserror;
        }

        public UserOperation() : base() {
            var opattr =
                this.GetType()
                    .GetCustomAttributes(typeof (UserOpAttribute), true)
                    .OfType<UserOpAttribute>()
                    .FirstOrDefault();
            if (null != opattr) {
                this.ErrorLevel = opattr.ErrorLevel;
                this.UserOpName = opattr.GetName();
                this.SuccessLevel = opattr.SuccessLevel;
                this.ExceptionLevel = opattr.ExceptionLevel;
                this.TreatFalseAsError = opattr.TreatFalseAsError;
                this.IsUserOperation = opattr.IsUserOperation;
            }
        }
        /// <summary>
        /// </summary>
        [Inject]
        public ILoggyManager LoggyManager {
            get { return _loggyManager ?? (_loggyManager = Loggy.Manager); }
            set { _loggyManager = value; }
        }

        protected ILoggy UserOpLog {
            get { return _userOpLog ?? (_userOpLog = (LoggyManager ?? Loggy.Manager).Get(UserOpName)); }
            set { _userOpLog = value; }
        }

        public string UserOpName { get; set; }

        public virtual void Run(IHostServer server, WebContext context, string callbackEndPoint,
            CancellationToken cancel) {
            try {
                var result = DefaultProcess(server, context, callbackEndPoint, cancel);
                
                if (IsUserOperation) {
                    
                    if (IsError(result)) {
                        if (ErrorLevel != LogLevel.None) {
                            if (UserOpLog.IsFor(ErrorLevel)) {
                                var message = GetUserOperationLog(true, ErrorLevel, result,context);
                                UserOpLog.Write(ErrorLevel, message);
                            }
                        }
                    }
                    else {
                        if (SuccessLevel != LogLevel.None) {
                            if (UserOpLog.IsFor(SuccessLevel)) {
                                var message = GetUserOperationLog(false, SuccessLevel, result,context);
                                UserOpLog.Write(SuccessLevel, message);
                            }
                        }
                    }
                }
            }
            catch (Exception e) {
                if (ExceptionLevel != LogLevel.None) {
                    if (UserOpLog.IsFor(ExceptionLevel)) {
                        var message =
                            new {error = true, type = e.GetType().Name, message = e.Message, stack = e.StackTrace}
                                .stringify();
                        UserOpLog.Write(ExceptionLevel, message);
                    }
                }
                throw;
            }
        }

        public virtual string GetUserOperationLog(bool iserror, LogLevel level, HandlerResult result,WebContext context) {
            return (result.Data??result.Result).stringify();
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using qorpent.v2.security.authorization;
using qorpent.v2.security.messaging;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log.NewLog;

namespace qorpent.v2.security.handlers.management {
    [ContainerComponent(Lifestyle.Singleton, "sendmail.handler", ServiceType = typeof (ISendMailHandler))]
    public class SendMailHandler : ISendMailHandler {
        private ILoggy loggy;

        [Inject]
        public IMessageQueue Queue { get; set; }

        [Inject]
        public IMessageSender Sender { get; set; }

        [Inject]
        public IRoleResolverService Roles { get; set; }

        [Inject]
        public ILoggyManager LoggyManager { get; set; }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            this.loggy = this.loggy ?? LoggyManager.Get("handler.sendmail");
            if (!Roles.IsInRole(context.User, "ADMIN")) {
                context.Finish(new {error = "notauth"}.stringify(), status: 500);
                return;
            }
            var p = RequestParameters.Create(context);
            var count = p.num("count");
            var messages = Queue.GetRequireSendMessages(count).ToArray();
            var sent = 0;
            IList<object> errors = new List<object>();
            foreach (var message in messages) {
                try {
                    Sender.Send(message);
                    Queue.MarkSent(message.Id);
                    sent++;
                }
                catch (Exception e) {
                    var inner = null == e.InnerException ? "" : e.InnerException.ToString();
                    var erinfo = new {message, error = e.ToString(), inner};
                    errors.Add(erinfo);
                    if (loggy.IsForError()) {
                        loggy.Error(erinfo.stringify());
                    }
                }
            }
            if (errors.Count == 0) {
                if (loggy.IsForTrace()) {
                    loggy.Trace(new{sent=messages.Length, ids=string.Join(", ",messages.Select(_=>_.Id))});
                }
               
            }
            context.Finish(new {
                src = messages.Length,
                sent,
                errors
            }.stringify(), status: errors.Count==0?200:500);
        }
    }
}
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

namespace qorpent.v2.security.handlers.management {
    [ContainerComponent(Lifestyle.Singleton, "sendmail.handler", ServiceType = typeof (ISendMailHandler))]
    public class SendMailHandler : ISendMailHandler {
        [Inject]
        public IMessageQueue Queue { get; set; }

        [Inject]
        public IMessageSender Sender { get; set; }

        [Inject]
        public IRoleResolverService Roles { get; set; }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
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
                    errors.Add(new {message, error = e.Message});
                }
            }
            context.Finish(new {
                src = messages.Length,
                sent,
                errors
            }.stringify(), status: 500);
        }
    }
}
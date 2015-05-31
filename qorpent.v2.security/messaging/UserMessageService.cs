using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using qorpent.v2.security.user;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.messaging {
    [ContainerComponent(Lifestyle.Singleton, "usermessage.service", ServiceType = typeof (IUserMessagingService))]
    public class UserMessageService : InitializeAbleService, IUserMessagingService {
        private IDictionary<string, string> _templateCache = new Dictionary<string, string>();

        public UserMessageService() {
            AppInfo = new Dictionary<string, object>();
        }

        [Inject]
        public IMessageQueue MessageQueue { get; set; }

        public IDictionary<string, object> AppInfo { get; set; }

        public IDictionary<string, string> TemplateCache {
            get { return _templateCache; }
            set { _templateCache = value; }
        }

        public PostMessage SendWelcome(IUser target) {
            return SendTemplated(target, "ref:wellcome", "support", "Регистрация на ресурсе \"${resource.name}\"", null);
        }

        public PostMessage SendPasswordReset(IUser target) {
            return SendTemplated(target, "ref:pwdreset", "support", "Сброс пароля на ресурсе \"${resource.name}\"", null);
        }

        public PostMessage SendTemplated(IUser target, string template, string from, string subject, IScope scope) {
            if (string.IsNullOrWhiteSpace(template)) {
                throw new Exception("invalid template");
            }
            if (string.IsNullOrWhiteSpace(target.Email)) {
                throw new Exception("not email defined");
            }
            var pm = CompoundMessage(target, template, from, subject, scope);
            return MessageQueue.PushMessage(pm);
        }

        public override void InitializeFromXml(XElement e) {
            e = e.Element("appinfo");
            if (null != e) {
                var dict = e.ToDict();
                if (null == AppInfo) {
                    AppInfo = dict;
                }
                else {
                    foreach (var o in dict) {
                        if (!AppInfo.ContainsKey(o.Key)) {
                            AppInfo[o.Key] = o.Value;
                        }
                    }
                }
            }
        }

        public PostMessage CompoundMessage(IUser target, string template, string @from, string subject, IScope scope) {
            var workingtemplate = ResolveTemplate(template);
            scope = scope ?? new Scope();
            scope.Set("user", target);
            scope.Set("resource", AppInfo);
            var resultmessage = workingtemplate.Interpolate(scope);
            subject = (subject ?? "").Interpolate(scope);
            var email = target.Email;
            var pm = new PostMessage {
                Addresses = new[] {email},
                From = @from,
                CanUseDefault = true,
                Subject = subject,
                Message = resultmessage.ToString(),
                Tags = new Dictionary<string, object>()
            };
            foreach (var pair in scope) {
                if (pair.Key == "user") {
                    pm.Tags["user"] = target.Login;
                }
                else {
                    pm.Tags[pair.Key] = pair.Value;
                }
            }
            return pm;
        }

        public XElement ResolveTemplate(string template) {
            var templatetext = template;
            if (template.StartsWith("ref:")) {
                templatetext = LoadTemplate(template.Substring(4));
            }
            if (!templatetext.StartsWith("<")) {
                templatetext = "<div>" + templatetext + "</div>";
            }
            return XElement.Parse(templatetext);
            ;
        }

        private string LoadTemplate(string name) {
            if (TemplateCache.ContainsKey(name)) {
                return TemplateCache[name];
            }
            var templatePath = EnvironmentInfo.ResolvePath("@repos@/.www/messagetemplates/" + name + ".xml");
            if (File.Exists(templatePath)) {
                TemplateCache[name] = File.ReadAllText(templatePath);
                return TemplateCache[name];
            }
            var resourcename = "message_template_" + name + ".xml";
            if (typeof (UserMessageService).Assembly.ExistsManifestResource(resourcename)) {
                TemplateCache[name] = typeof (UserMessageService).Assembly.ReadManifestResource(resourcename);
                return TemplateCache[name];
            }
            throw new Exception("cannot find template");
        }
    }
}
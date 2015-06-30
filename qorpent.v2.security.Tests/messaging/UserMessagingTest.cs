using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.messaging;
using qorpent.v2.security.user;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.Tests.messaging
{
    [TestFixture]
    public class UserMessagingTest :BaseFixture
    {
        [Test]
        public void CanCompoundWellCome() {
            var um = (UserMessageService)_container.Get<IUserMessagingService>();
            var u = new User {Login = "test", Name = "Иванов", Email = "ivanov@gmail.com"};
            new PasswordManager().MakeRequest(u,24*60);
            var message = um.CompoundMessage(u, "ref:wellcome", "support","test",null);
            Console.WriteLine(message.Message.Simplify(SimplifyOptions.SingleQuotes));
            Assert.AreEqual(@"<div>
  <h1>Уважаемый(ая) Иванов!</h1>
  <p>
    Данным письмом мы уведомляем Вас о том, что вы зарегистрированы на
    ресурсе '<a href='https://super.puper.com/login.html?referer=/home.html'>Супер-Пупер</a>'
    под учетной записью <strong>test</strong></p>
  <p>
    Для активации вашей учетной записи и регистрации Вашего пароля просим
    вас перейти по ссылке
    <a href='https://super.puper.com/resetpwd.html?login=test&amp;key=[KEY]'>Активация пользователя</a></p>
  <p>
    В случае, если Вы не уверены, что данная регистрация предназначается Вам,
    вы можете уточнить этот вопрос, послав соответствующий запрос
    ответным письмом
  </p>
  <p>
    C Уважением, адмнистрация ресурса 'Супер-Пупер'
  </p>
</div>".Replace("[KEY]", u.ResetKey), message.Message.Simplify(SimplifyOptions.SingleQuotes));
        }

        [Test]
        [Explicit]
        public void CanSendWellcome() {
            var um = (UserMessageService)_container.Get<IUserMessagingService>();
            var u = new User { Login = "test", Name = "Иванов", Email = "fagim.sadykov@gmail.com" };
            new PasswordManager().MakeRequest(u, 24 * 60);
            um.SendWelcome(u);
            var q = _container.Get<IMessageQueue>();
            var sender = _container.Get<IMessageSender>();
            var messages = q.GetRequireSendMessages().ToArray();

            foreach (var postMessage in messages) {
                if (postMessage.Addresses[0] == u.Email) {
                    sender.Send(postMessage);
                    q.MarkSent(postMessage.Id);
                }    
            }
        }
    }
}

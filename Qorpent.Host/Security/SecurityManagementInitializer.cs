using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Security
{
    //[ContainerComponent(ServiceType = typeof(IHostServerInitializer),Name = "security.manager.initializer")]
    public class SecurityManagementInitializer:IHostServerInitializer
    {
        /// <summary>
        /// Применяет специальные хэндлеры для настройки безопасности
        /// </summary>
        /// <param name="server"></param>
        public  void Initialize(IHostServer server) {
            var supportMembership = server.Config.Definition.Attr("securityadmin").ToBool();
            if(!supportMembership)return;
            var updatelogin = new UpdateLoginInfoHandler(server);
            server.OnContext("/sa/update", _ => updatelogin.Run(server, _, null, CancellationToken.None));
            server.Config.Log.Info("SecurityManagementInitializer applyed");
        }

        
    }

    
}

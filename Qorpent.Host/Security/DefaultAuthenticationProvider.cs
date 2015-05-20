using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Security;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Security{

    

	/// <summary>
	/// </summary>
	public class DefaultAuthenticationProvider : ServiceBase, IAuthenticationProvider{

		private readonly IDictionary<string, UserInfo> _ticketCache = new Dictionary<string, UserInfo>();

		private readonly UserInfo error = new UserInfo{
			Ok = false,
			Type = TokenType.Error,
			Principal = new GenericPrincipal(new GenericIdentity("local\\error"), null)
		};

		private readonly UserInfo guest = new UserInfo{
			Ok = false,
			Type = TokenType.Guest,
			Principal = new GenericPrincipal(new GenericIdentity("local\\guest"), null)
		};

        private readonly UserInfo admin = new UserInfo
        {
            Ok = true,
            Type = TokenType.Admin,
            Login = "local\\admin",
            Principal = new GenericPrincipal(new GenericIdentity("local\\admin"), null)
        };


        [Inject]
        public IRoleResolver RoleResolver { get; set; }

	    [Inject]
	    public ILogonProvider LogonProvider { get; set; }

        [Inject]
        public ILoginSourceProvider LoginSourceProvider { get; set; }

       


		private IHostServer _server;

		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		public void Initialize(IHostServer server){
			_server = server;
		    ExclusiveAuth = _server.Config.Definition.ResolveValue("exclusiveauth").ToBool();
		    IgnoreAuth = _server.Config.Definition.ResolveValue("ignoreauth").ToBool();
            server.OnContext("/logon", Logon);
            server.OnContext("/logout", Logout);
            server.OnContext("/isauth", IsAuth);
            server.OnContext("/isrole", IsRole);
		    server.OnContext("/mylogin", MyLogin);
		    server.OnContext("/myinfo", MyInfo);
            server.OnContext("/authreset", AuthReset);
		}

	    private void MyInfo(WebContext obj) {
            if (!GetIsAuth(obj))
            {
                throw new Exception("need auth");
            }
            var data = RequestParameters.Create(obj);
            var qhp = obj.User as QorpentHostPrincipal;

	        var login = data.Get("login");
	        if (string.IsNullOrWhiteSpace(login) || login == qhp.Identity.Name) {

	            if (null != qhp) {
	                var info = LoginSourceProvider.Get(qhp.Identity.Name);
	                if (null != info) {
	                    obj.Finish(JsonSerializer.Stringify(info));
	                }
	                else {
	                    obj.Finish("null");
	                }
	            }
	            else {
	                throw new Exception("auth required");
	            }
	        }
	        else {
                var isadmin = RoleResolver.IsInRole(obj.User.Identity.Name, "ADMIN");
                if (!isadmin)
                {
                    throw new Exception("not admin");
                }
	            var info = LoginSourceProvider.Get(login);
                if (null != info)
                {
                    obj.Finish(JsonSerializer.Stringify(info));
                }
                else
                {
                    obj.Finish("null");
                }
	        }
	    }

	    private void MyLogin(WebContext obj) {
            var qhp = obj.User as QorpentHostPrincipal;
	        if (null != qhp) {
	            obj.Finish(JsonSerializer.Stringify(qhp.Info));
	        }
	        else {
	            throw new Exception("auth required");
	        }
	    }

	    private void IsRole(WebContext context) {
            var result = GetIsRole(context);
            context.Finish(result.ToString().ToLowerInvariant());
	    }


	    private bool GetIsRole(WebContext context) {
	        if (!GetIsAuth(context)) {
	            throw new Exception("need auth");
	        }
	        var data = RequestParameters.Create(context);
	        var username = data.Get("login");
	        var role = data.Get("role");
	        var exact = data.Get("exact").ToBool();
	        if (string.IsNullOrWhiteSpace(role)) return false;
	        if (string.IsNullOrWhiteSpace(username) || (username.ToLowerInvariant() == context.User.Identity.Name))
	        {
	            return RoleResolver.IsInRole(context.User.Identity.Name, role,exact);
	        }
	        var isadmin = RoleResolver.IsInRole(context.User.Identity.Name, "ADMIN");
	        if (!isadmin) {
	            throw new Exception("not admin");
	        }
	        return RoleResolver.IsInRole(username, role, exact);
	    }

	    private void AuthReset(WebContext ctx) {
	        Reset(new ResetEventData(true));
            ctx.Finish("true");
	    }

	    public override object Reset(ResetEventData data) {
            LoginSourceProvider.Reset(data);
            _ticketCache.Clear();
            UserTicketMap.Clear();
            ((IResetable)RoleResolver).Reset(data);
	        return true;
	    }

	    public bool IgnoreAuth { get; set; }

	    public bool ExclusiveAuth { get; set; }

	    /// <summary>
		/// </summary>
		/// <param name="context"></param>
        public void Authenticate(WebContext context) {
            UserInfo result = guest;
	        if (CheckFullTrustOrigin(context)) return;
	      
	            string ticket = GetTicket(context);

	            bool auth = false;
	            if (!string.IsNullOrWhiteSpace(ticket)) {
	                if (IsIgnoreAuthentication(context)) {
	                    auth = true;
                        SetTicketCookie(context, ticket);
	                }
	                else {
	                    result = CheckTicket(ticket, context);
	                    if (null != result) {
	                        auth = true;
	                        SetTicketCookie(context, ticket);
	                    }
	                    else {
	                        result = guest;
	                        _ticketCache.Remove(ticket);

	                    }
	                }
	            }
	            if (!auth) {
	                SetTicketCookie(context, null);
	            }
	        	        var principal = new QorpentHostPrincipal(result);
		    context.User = principal;

		}

	    private bool CheckFullTrustOrigin(WebContext context) {
	        if (context.Request.Headers.ContainsKey("Origin") &&
	            _server.Config.Definition.Attr("grant-admin-origin") == context.Request.Headers["Origin"]) {
	            if (context.Request.RemoteEndPoint.Address.ToString() == "127.0.0.1") {
	                context.User = new QorpentHostPrincipal(admin);
	                return true;
	            }
	        }
	        return false;
	    }

	    private bool IsIgnoreAuthentication(WebContext context) {
	        if (IgnoreAuth) {
	            return true;
	        }
	        var path = context.Uri.AbsolutePath;
	        if (path.EndsWith(".js")) {
	            return true;
	        }
	        if (path.EndsWith(".css")) {
	            return true;
	        }
	        if (path.EndsWith(".html") && !_server.Config.RequireLogin) {
	            return true;
	        }
	        if (path.EndsWith(".css.map")) {
	            return true;
	        }
	        if (path.EndsWith(".js.map")) {
	            return true;
	        }
	        if (context.Uri.AbsolutePath == "/logout") {
	            return true;
	        }
	        return false;
	    }


	    public void Authenticate(WebContext context, string username, string password)
        {
            bool isauth = LogonProvider.IsAuth(username, password);
            if (isauth)
            {
                string ticket = RegisterTicket(username,context);
                SetTicketCookie(context, ticket);
                UserInfo result = _ticketCache[ticket];
                var principal = new QorpentHostPrincipal(result);
                context.User = principal;
            }
            else
            {
                SetTicketCookie(context, null);
                context.User = new QorpentHostPrincipal(error);
            }
        }

        


	    public void Logon(WebContext context) {
            RequestParameters data = RequestParameters.Create(context);
            string login = data.Get("login");
            string elogin = data.Get("elogin");
            string pass = data.Get("pass");
            string epass = data.Get("epass");
            if (!string.IsNullOrWhiteSpace(elogin))
            {
                login = _server.Encryptor.Decrypt(elogin);
            }
            if (!string.IsNullOrWhiteSpace(epass))
            {
                pass = _server.Encryptor.Decrypt(epass);
            }
            if (string.IsNullOrWhiteSpace(login))
            {
                context.Finish("'no login'");
                return;
            }
            if (string.IsNullOrWhiteSpace(pass))
            {
                context.Finish("'no pass'");
                return;
            }
            Authenticate(context, login, pass);
            
            UserInfo result = (context.User as QorpentHostPrincipal).Info;
            if (result.Ok && result != guest)
            {
                context.Finish("true");
            }
            else
            {
                if (result == guest)
                {
                    context.Finish("false");
                }
                else
                {
                    context.Finish("\"invalid user name  or password\"");
                }
            }
	    }

	    /// <summary>
		///     Выполняет выход из контекста
		/// </summary>
		/// <param name="context"></param>
        public void Logout(WebContext context)
        {
			string currentTicket = GetTicket(context);
			if (!string.IsNullOrWhiteSpace(currentTicket) && _ticketCache.ContainsKey(currentTicket)){
				_ticketCache.Remove(currentTicket);
			}
			SetTicketCookie(context, null);
			context.Finish("true");
		}

		private bool IsValid(UserInfo user, string ticket,WebContext context){
			if (user == guest) return true;
			if (user == error) return false;
			if (DateTime.Now > user.Expire) return false;
		    if (ExclusiveAuth && UserTicketMap[user.Login] != ticket) {
                _server.Config.Log.Warn("Exclusive login required");
		        return false;
		    }
		    var ua = context.Request.UserAgent.GetMd5(5);
		    if (user.UserAgent != ua) {
		        _server.Config.Log.Error("Invalid User Agent on Ticket");
		        return false;
		    }
		    if (context.Request.LocalEndPoint.Address.ToString() != user.LocalAddress) {
                _server.Config.Log.Error("Invalid Local endpoint on Ticket");
                return false;
		    }
            if (context.Request.RemoteEndPoint.Address.ToString() != user.RemoteAddress)
            {
                _server.Config.Log.Error("Invalid Remote endpoint on Ticket");
                return false;
            }
			return user.Ok;
		}

		private string RegisterTicket(string username, WebContext context){
			var result = new UserInfo{
				Expire = DateTime.Today.AddDays(1),
				Login = username.ToLowerInvariant(),
				Ok = true,
				Principal = new GenericPrincipal(new GenericIdentity(username.ToLowerInvariant()), null),
				Token = "",
				Type = TokenType.Remote,
                LoginTime = DateTime.Now,
                UserAgent= context.Request.UserAgent.GetMd5(5),
                LocalAddress = context.Request.LocalEndPoint.Address.ToString(),
                RemoteAddress = context.Request.RemoteEndPoint.Address.ToString()
			};
			var str = new MemoryStream();
			var wr = new BinaryWriter(str);
			result.Write(wr);
			wr.Flush();
			string ticket = _server.Encryptor.Encrypt(str.ToArray());
		    result.Token = ticket;
			_ticketCache[ticket] = result;
		    UserTicketMap[result.Login] = ticket;
			return ticket;
		}

		private void SetTicketCookie(WebContext context, string ticket){
			ticket = ticket ?? "";
			var cookie = new Cookie(_server.Config.AuthCookieName, ticket);
		    cookie.Path = "/";
			if (string.IsNullOrWhiteSpace(ticket)){
				cookie.Expires = DateTime.Now.AddYears(-1);
			}
			else{
				cookie.Expires = DateTime.Today.AddDays(1);
			}
		    cookie.HttpOnly = true;
		    cookie.Secure = true;
			context.Response.Cookies.Add(cookie);
		}

        IDictionary<string, string> UserTicketMap = new Dictionary<string, string>();

		private UserInfo CheckTicket(string ticket,WebContext context) {
		    var token = GetToken(ticket);
		    if (null != token) {
		        if (!IsValid(token,ticket,context)) return null;
		    }
		    return token;
		}

	    private UserInfo GetToken(string ticket) {
	        if (_ticketCache.ContainsKey(ticket)) {
	            return _ticketCache[ticket];
	        }
	        try {
	            var data = new MemoryStream(_server.Encryptor.DecryptData(ticket));
	            data.Position = 0;
	            var reader = new BinaryReader(data);
	            var token = new UserInfo();
	            token.Read(reader);
	            _ticketCache[ticket] = token;
	            if (!UserTicketMap.ContainsKey(token.Login)) {
	                UserTicketMap[token.Login] = ticket;
	            }
	            return token;
	        }
	        catch {
	            return null;
	        }
	    }

	    /// <summary>
		/// Проверка аутентифицированного контекста
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
        public void IsAuth(WebContext context) {
	        var result = GetIsAuth(context);
	        context.Finish(result.ToString().ToLowerInvariant());
	    }

	    private static bool GetIsAuth(WebContext context) {
	        var principal = context.User;
	        var result = false;
	        if (null != principal) {
	            result = principal.Identity.IsAuthenticated;
	        }
	        return result;
	    }

	    private string GetTicket(WebContext context)
        {
			Cookie cookie = context.Cookies[_server.Config.AuthCookieName];
			if (null == cookie) return null;
			return cookie.Value;
		}
	}
}
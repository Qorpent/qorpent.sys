using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using Qorpent.Host.Utils;

namespace Qorpent.Host.Security{
	/// <summary>
	/// </summary>
	public class DefaultAuthenticationProvider : IAuthenticationProvider{
		private readonly WinLogon _logon = new WinLogon();

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

		private readonly FieldInfo m_User = typeof (HttpListenerContext).GetField("m_User",
		                                                                          BindingFlags.SetField |
		                                                                          BindingFlags.NonPublic |
		                                                                          BindingFlags.Instance) ?? 
		                                                                          
		                                   typeof (HttpListenerContext).GetField("user",   // IN MONO
		                                                                          BindingFlags.SetField |
		                                                                          BindingFlags.NonPublic |
		                                                                          BindingFlags.Instance) ;

		private IHostServer _server;

		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		public void Initialize(IHostServer server){
			_server = server;
		}

		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		public void Authenticate(HttpListenerContext context){
			string ticket = GetTicket(context);
			UserInfo result = guest;
			bool auth = false;
			if (!string.IsNullOrWhiteSpace(ticket)){
				result = CheckTicket(ticket);
				if (null != result && IsValid(result)){
					auth = true;
					SetTicketCookie(context, ticket);
				}
				else{
					if (null == result){
						result = guest;
						_ticketCache.Remove(ticket);
					}
				}
			}
			if (!auth){
				SetTicketCookie(context, null);
			}
			var principal = new QorpentHostPrincipal(result);
			m_User.SetValue(context, principal);
			
		}

		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public void Authenticate(HttpListenerContext context, string username, string password){
			bool isauth = _logon.Logon(username, password);
			if (isauth){
				string ticket = RegisterTicket(username);
				SetTicketCookie(context, ticket);
				UserInfo result = _ticketCache[ticket];
				var principal = new QorpentHostPrincipal(result);
				m_User.SetValue(context, principal);
			}
			else{
				SetTicketCookie(context, null);
				m_User.SetValue(context, new QorpentHostPrincipal(error));
			}
		}

		/// <summary>
		///     Выполняет разбор параметров и осуществляет вход
		/// </summary>
		/// <param name="context"></param>
		public void Logon(HttpListenerContext context){
			RequestParameterSet data = new RequestDataRetriever(context.Request).GetRequestData();
			string login = data.Get("login");
			string elogin = data.Get("elogin");
			string pass = data.Get("pass");
			string epass = data.Get("epass");
			if (!string.IsNullOrWhiteSpace(elogin)){
				login = _server.Encryptor.Decrypt(elogin);
			}
			if (!string.IsNullOrWhiteSpace(epass)){
				pass = _server.Encryptor.Decrypt(epass);
			}
			if (string.IsNullOrWhiteSpace(login)){
				context.Finish("no login", status: 500);
				return;
			}
			if (string.IsNullOrWhiteSpace(pass)){
				context.Finish("no pass", status: 500);
				return;
			}
			Authenticate(context, login, pass);
			UserInfo result = (context.User as QorpentHostPrincipal).Info;
			if (result.Ok && result != guest){
				context.Finish("true", "application/json");
			}
			else{
				if (result == guest){
					context.Finish("false", "application/json");
				}
				else{
					context.Finish("'error'", "application/json", 500);
				}
			}
		}

		/// <summary>
		///     Выполняет выход из контекста
		/// </summary>
		/// <param name="context"></param>
		public void Logout(HttpListenerContext context){
			string currentTicket = GetTicket(context);
			if (!string.IsNullOrWhiteSpace(currentTicket) && _ticketCache.ContainsKey(currentTicket)){
				_ticketCache.Remove(currentTicket);
			}
			SetTicketCookie(context, null);
			context.Response.Finish("true", "application/json");
		}

		private bool IsValid(UserInfo user){
			if (user == guest) return true;
			if (user == error) return false;
			if (DateTime.Now > user.Expire) return false;
			return user.Ok;
		}

		private string RegisterTicket(string username){
			var result = new UserInfo{
				Expire = DateTime.Today.AddDays(1),
				Login = username.ToLowerInvariant(),
				Ok = true,
				Principal = new GenericPrincipal(new GenericIdentity(username.ToLowerInvariant()), null),
				Token = "",
				Type = TokenType.Remote
			};
			var str = new MemoryStream();
			var wr = new BinaryWriter(str);
			result.Write(wr);
			wr.Flush();
			string ticket = _server.Encryptor.Encrypt(str.ToArray());
			_ticketCache[ticket] = result;
			return ticket;
		}

		private void SetTicketCookie(HttpListenerContext context, string ticket){
			ticket = ticket ?? "";
			var cookie = new Cookie(_server.Config.AuthCookieName, ticket);
			if (string.IsNullOrWhiteSpace(ticket)){
				cookie.Expires = DateTime.Now.AddYears(-1);
			}
			else{
				cookie.Expires = DateTime.Today.AddDays(1);
			}
			context.Response.Cookies.Add(cookie);
		}

		

		private UserInfo CheckTicket(string ticket){
			if (_ticketCache.ContainsKey(ticket)) return _ticketCache[ticket];
			try{
				var data = new MemoryStream(_server.Encryptor.DecryptData(ticket));
				data.Position = 0;
				var reader = new BinaryReader(data);
				var token = new UserInfo();
				token.Read(reader);
				_ticketCache[ticket] = token;
				return token;
			}
			catch{
				return null;
			}
		}

		/// <summary>
		/// Проверка аутентифицированного контекста
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public bool IsAuth(HttpListenerContext context){
			var result = !string.IsNullOrWhiteSpace(GetTicket(context));
			context.Finish(result.ToString().ToLowerInvariant(),mimeType:"application/json");
			return result;
		}
		private string GetTicket(HttpListenerContext context){
			Cookie cookie = context.Request.Cookies[_server.Config.AuthCookieName];
			if (null == cookie) return null;
			return cookie.Value;
		}
	}
}
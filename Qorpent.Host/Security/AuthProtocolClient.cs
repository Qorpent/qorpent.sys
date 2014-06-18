using System;
using System.Threading.Tasks;
using Qorpent.Host.SimpleSockets;

namespace Qorpent.Host.Security{
	/// <summary>
	/// </summary>
	public class AuthProtocolClient{
		private readonly SimpleSocketClient<AuthProtocol, AuthProtocol> _client;

		/// <summary>
		/// </summary>
		public AuthProtocolClient(SimpleSocketConfig config){
			_client = new SimpleSocketClient<AuthProtocol, AuthProtocol>(config);
		}

		/// <summary>
		/// </summary>
		/// <param name="client"></param>
		public AuthProtocolClient(SimpleSocketClient<AuthProtocol, AuthProtocol> client = null){
			_client = client ??
			          new SimpleSocketClient<AuthProtocol, AuthProtocol>(new SimpleSocketConfig{Port = AuthProtocol.DefaultPort});
		}

		/// <summary>
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public async Task<AuthProtocol> ExecuteAsync(AuthProtocol request){
			return await _client.CallAsync(request);
		}

		/// <summary>
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public AuthProtocol Execute(AuthProtocol request){
			return _client.Call(request);
		}

		/// <summary>
		/// </summary>
		/// <param name="login"></param>
		/// <param name="pass"></param>
		/// <returns></returns>
		public UserInfo Auth(string login, string pass){
			AuthProtocol req = GetAuthReq(login, pass);
			return GetTokenInfo(Execute(req));
		}

		/// <summary>
		/// </summary>
		/// <param name="login"></param>
		/// <param name="pass"></param>
		/// <returns></returns>
		public async Task<UserInfo> AuthAsync(string login, string pass){
			AuthProtocol req = GetAuthReq(login, pass);
			return GetTokenInfo(await ExecuteAsync(req));
		}

		private UserInfo GetTokenInfo(AuthProtocol result){
			if (result.Response.HasFlag(AuthProtocolResponseType.Error)){
				throw new Exception("Error in auth: " + result.ErrorCode + " ( " + result.ErrorStatus + " )");
			}
			if (result.Response.HasFlag(AuthProtocolResponseType.True)){
				return new UserInfo{Ok = true, Expire = result.Expire, Login = result.Login, Token = result.Token};
			}
			return new UserInfo();
		}

		private AuthProtocol GetAuthReq(string login, string pass){
			return new AuthProtocol{Request = AuthProtocolRequestType.AuthBasic, Login = login, PassOrDigest = pass};
		}
	}
}
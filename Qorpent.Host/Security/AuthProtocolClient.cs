using System;
using System.Threading.Tasks;
using Qorpent.Host.Exe.SimpleSockets;

namespace Qorpent.Host.Exe.Security{
	/// <summary>
	/// 
	/// </summary>
	public class AuthProtocolClient{
		private SimpleSocketClient<AuthProtocol, AuthProtocol> _client;
		/// <summary>
		/// 
		/// </summary>
		public AuthProtocolClient(SimpleSocketConfig config)
		{
			_client =new SimpleSocketClient<AuthProtocol, AuthProtocol>(config);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="client"></param>
		public AuthProtocolClient(SimpleSocketClient<AuthProtocol, AuthProtocol> client = null){
			_client = client ?? new SimpleSocketClient<AuthProtocol, AuthProtocol>(new SimpleSocketConfig{Port = AuthProtocol.DefaultPort});
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public async Task<AuthProtocol> ExecuteAsync(AuthProtocol request){
			return await _client.CallAsync(request);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public AuthProtocol Execute(AuthProtocol request){
			return _client.Call(request);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="pass"></param>
		/// <returns></returns>
		public TokenInfo Auth(string login, string pass){
			var req = GetAuthReq(login, pass);
			return GetTokenInfo(Execute(req));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="pass"></param>
		/// <returns></returns>
		public async Task<TokenInfo> AuthAsync(string login, string pass)
		{
			var req = GetAuthReq(login, pass);
			return GetTokenInfo(await ExecuteAsync(req));
		}

		private TokenInfo GetTokenInfo(AuthProtocol result){
			if (result.Response.HasFlag(AuthProtocolResponseType.Error)){
				throw new Exception("Error in auth: "+result.ErrorCode+" ( "+result.ErrorStatus+" )");
			}
			if (result.Response.HasFlag(AuthProtocolResponseType.True)){
				return new TokenInfo{Ok = true, Expire = result.Expire, Login = result.Login, Token = result.Token};
			}
			return new TokenInfo();
		}

		private AuthProtocol GetAuthReq(string login, string pass){
			return new AuthProtocol{Request = AuthProtocolRequestType.AuthBasic, Login = login, PassOrDigest = pass};
		}
	}
}
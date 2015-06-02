using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Host.SimpleSockets;
using Qorpent.Security;

namespace Qorpent.Host.Security{
	/// <summary>
	/// </summary>
	public class DefaultAuthHandler : ISimpleSocketHandler<AuthProtocol, AuthProtocol>{
		private readonly ILogonProvider _logon;

		/// <summary>
		/// </summary>
		public DefaultAuthHandler() {
		    _logon = new DefaultLogonProvider {Logons = new[] {new SysLogon()}};
		}

		/// <summary>
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public async Task Execute(ISimpleSocketRequest<AuthProtocol, AuthProtocol> request){
			AuthProtocol req = await request.GetQuery();
			var result = new AuthProtocol();
			if (req.Request == AuthProtocolRequestType.State){
				State(result);
			}
			else if (req.Request == AuthProtocolRequestType.AuthBasic){
				Auth(req, result);
			}
			else{
				Error(result, 100, "Not supported request");
			}
			await request.Send(result, true);
		}

		private void Auth(AuthProtocol req, AuthProtocol result){
			if (_logon.IsAuth(req.Login, req.PassOrDigest)){
				result.Response = AuthProtocolResponseType.True | AuthProtocolResponseType.Token;
				result.Login = req.Login.ToLowerInvariant();
				result.Expire = DateTime.Today.AddDays(1);
				result.Token =
					Convert.ToBase64String(
						MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(result.Login + result.Expire.ToString("yyyyMMddhhmm"))));
			}
			else{
				result.Response = AuthProtocolResponseType.False;
			}
		}

		private void Error(AuthProtocol result, int code, string status){
			result.Response = AuthProtocolResponseType.Error;
			result.ErrorCode = code;
			result.ErrorStatus = status;
		}


		private void State(AuthProtocol result){
			result.Response = AuthProtocolResponseType.True | AuthProtocolResponseType.State;
			result.State = AuthProtocolStatus.None;
		}
	}
}
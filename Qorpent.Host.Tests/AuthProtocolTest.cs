using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Host.Exe.Security;
using Qorpent.Host.Exe.SimpleSockets;

namespace Qorpent.Host.Tests{
	[TestFixture]
	public class AuthProtocolTest{
		
		[TestCase("valid","invalid",false,"")]
		[TestCase("invalid","invalid",false,"")]
		[TestCase("lValid","valid",true,"local\\valid")]
		[TestCase("lvalid","valid",true,"local\\valid")]
		[TestCase("Valid","valid",true,"domain\\valid")]
		[TestCase("valid","valid",true,"domain\\valid")]
		public void BasicAuthEmulation(string login,string pass, bool result, string resultLogin){
			using (var srv = SimpleSocket.CreateServer(new FakeAuthHandler())){
				srv.Start();
				var cli = new AuthProtocolClient();
				var token = cli.Auth(login, pass);
				Assert.AreEqual(token.Ok,result);
				if (token.Ok){
					Assert.AreEqual(resultLogin,token.Login);
					Console.WriteLine(token.Token);
					Assert.False(string.IsNullOrWhiteSpace(token.Token));
				}
			}
		}

		public class FakeAuthHandler:ISimpleSocketHandler<AuthProtocol,AuthProtocol>{
			public async Task Execute(SimpleSocketRequest<AuthProtocol, AuthProtocol> request){
				var req = await request.GetQuery();
				var result = new AuthProtocol();
				if (req.Request == AuthProtocolRequestType.State){
					State(result);
				}else if (req.Request == AuthProtocolRequestType.AuthBasic){
					Auth(req,result);
				}
				else{
					Error(result, 100, "Not supported request");
				}
				await request.Send(result,true);
			}

			private void Auth(AuthProtocol req, AuthProtocol result){
				if (req.Login.Contains("invalid") || req.PassOrDigest.Contains("invalid")){
					result.Response = AuthProtocolResponseType.False;
				}
				else{
					result.Response = AuthProtocolResponseType.True | AuthProtocolResponseType.Token;
					result.Login =
						(req.Login.StartsWith("l") ? ("local\\" + req.Login.Substring(1)) : ("domain\\" + req.Login)).ToLowerInvariant();
					result.Expire = DateTime.Today.AddDays(1);
					result.Token =
						Convert.ToBase64String(
							MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(result.Login + result.Expire.ToString("yyyyMMddhhmm"))));
				}
			}

			private void Error(AuthProtocol result, int code, string status){
				result.Response = AuthProtocolResponseType.Error;
				result.ErrorCode = code;
				result.ErrorStatus = status;
			}

			

			private void State(AuthProtocol result){
				result.Response = AuthProtocolResponseType.True|AuthProtocolResponseType.State;
				result.State = AuthProtocolStatus.None;
			}
		}
	}
}
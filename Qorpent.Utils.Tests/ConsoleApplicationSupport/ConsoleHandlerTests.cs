using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Qorpent.Utils.Tests.ConsoleApplicationSupport
{
	[TestFixture]
	public class ConsoleHandlerTests
	{
		private ConsoleApplicationHandler h;

		[SetUp]
		public void Setup(){
			h = new ConsoleApplicationHandler{ExePath = "qs.tc"};
		}
		[Test]
		public void CanSimpleRunAndReadOutput(){
			var r = h.Run();
			Assert.AreEqual(0,r.State);
			Assert.AreEqual("hello\r\n\r\n", r.Output);
		}

		[Test]
		public void CanWorkWithBothOutputAndErrorStreamAndExitCode(){
			h.StandardArguments["error"] = "error";
			h.StandardArguments["state"] = "-1";
			var r = h.Run();
			Assert.AreEqual(-1, r.State);
			Assert.AreEqual("error\r\n\r\n", r.Error);
			Assert.AreEqual("hello\r\n\r\n", r.Output);
		}

		[Test]
		public void CanCatchTimeout()
		{
			h.StandardArguments["timeout"] = "5";
			h.Timeout = 1000;
			var r = h.Run();
			Assert.True(r.Timeouted);
			Assert.AreEqual("hello\r\n\r\n", r.Output);
		}

		[Test]
		public void CanSendReadLine()
		{
			h.Listener = new ConsoleApplicationListener();
			h.StandardArguments["doreadln"] = "true";
			h.Listener.Send("bro");
			var r = h.Run();
			Assert.AreEqual("hello\r\nbro\r\n\r\n", r.Output);
		}

		[Test]
		public void CanWorkInteractive(){
			h.Listener = new ConsoleApplicationListener{
				OnOutput = _ =>{
					if (_ == "a"){
						h.Listener.Send("12");
					}
					else if (_ == "b"){
						h.Listener.Send("14");
					}
				}
			};
			h.StandardArguments["interactive"] = "true";
			var r = h.Run();
			Assert.AreEqual("hello\r\na\r\nb\r\n26\r\n\r\n", r.Output);
		}
		[Test]
		[Ignore("ReadKey не поддерживает при перенаправленной консоли")]
		public void CanLoginWithGitLikeConsole(){
			h.Listener = new ConsoleApplicationListener();
			h.Listener.Send("myname");
			h.Listener.Send("mypass");
			h.StandardArguments["passread"] = "true";
			h.StandardArguments["debug"] = "true";
			var r = h.Run();
			Assert.AreEqual("hello\r\na\r\nb\r\n26\r\n\r\n", r.Output);
		}
	}
}

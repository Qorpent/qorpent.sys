using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.IO.Protocols;
using Qorpent.Log;

namespace Qorpent.IO.Tests.Protocol
{
	/// <summary>
	/// Проверка общей работоспособности протокола
	/// </summary>
	[TestFixture]
	public class BasicProtocolTest
	{
		private ProtocolBufferOptions bufoptions;
		private byte[] buffer;
		private MemoryStream stream;
		private ProtocolBuffer protocolBuffer;
		private SimpleProtocol protocol;
		private ProtocolBufferResult result;

		[Test]
		public void CanReadCunkedStream(){
			SimleSetup();
			Execute();
			CheckOk();
			Assert.AreEqual("hello world",protocol.Result);
			Assert.True(protocolBuffer.Pages.Where(_=>null!=_).All(_=>_.State==ProtocolBufferPage.Free));
		}

		private void Execute(){
			result = protocolBuffer.Read(stream, protocol);
		}

		private void CheckOk(){
			if (null != result.Error){
				Console.WriteLine(result.Error);
			}
			Assert.True(result.Ok);
			Assert.True(result.Success);
			Assert.Null(result.Error);
		}

		private readonly IUserLog log = ConsoleLogWriter.CreateLog(level: LogLevel.All,customFormat:"${Message}");
		private void SimleSetup(){
			bufoptions = new ProtocolBufferOptions(pageSize: 2);
			this.buffer = Encoding.ASCII.GetBytes("hello world");
			this.stream = new MemoryStream(buffer);
			this.protocolBuffer = new ProtocolBuffer(bufoptions);
			protocolBuffer.Log = log;
			this.protocol = new SimpleProtocol();
			protocol.Log = log;
		}

		[Test]
		public void CanStopIfProtocolStop(){
			SimleSetup();
			protocol.MaxCount = 3;
			Execute();	
			CheckOk();
			Assert.AreEqual("hello ",protocol.Result);
		}

		[Test]
		public void CanGetErrors()
		{
			SimleSetup();
			protocol.MaxCount = 3;
			protocol.ThrowOnOverMax = true;
			Execute();
			Assert.Null(protocol.Result);
			Assert.NotNull(result.Error);
		}
	}
}

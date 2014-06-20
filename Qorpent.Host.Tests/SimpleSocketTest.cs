using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Host.SimpleSockets;
using Qorpent.IO;

namespace Qorpent.Host.Tests
{
	[TestFixture]
	public class SimpleSocketTest
	{
		/// <summary>
		/// 
		/// </summary>
		class HelloWorldHandler :ISimpleSocketHandler<int,string>{
			public async Task Execute(ISimpleSocketRequest<int, string> request){
				var q =await request.GetQuery();
				if (q == 1){
					await request.Send("hello",true);
				}else if (q == 2){
					await request.Send("привет",true);
				}
			}
		}
		/// <summary>
		/// Пример обменного протокола
		/// </summary>
		class SimpleData:IBinarySerializable{
			public bool IsResult;
			public byte Command;
			public int Arg;
			public int Arg2;
			public int Result;

			public void Read(BinaryReader reader){
				Command = reader.ReadByte();
				if (0 == Command){
					IsResult = true;
					Result = reader.ReadInt32();
				}
				else{
					Arg = reader.ReadInt32();
					Arg2 = reader.ReadInt32();
				}

			}

			public void Write(BinaryWriter writer){
				if (IsResult){
					writer.Write((byte) 0);
					writer.Write(Result);
				}
				else{
					writer.Write(Command);
					writer.Write(Arg);
					writer.Write(Arg2);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		class Calculator : ISimpleSocketHandler<SimpleData, SimpleData>
		{
			public async Task Execute(ISimpleSocketRequest<SimpleData, SimpleData> request)
			{
				var q = await request.GetQuery();
				q.IsResult = true;
				if (q.Command == 1){
					q.Result = q.Arg + q.Arg2;
				}else if (q.Command == 2){
					q.Result = q.Arg - q.Arg2;
				}
				await request.Send(q,true);
			}
		}

		[Test]
		public void BasicFunctionality(){
			var server = SimpleSocket.CreateServer(new HelloWorldHandler());
			var client = new SimpleSocketClient<int, string>();
			try{
				server.Start();
				Assert.AreEqual("hello",client.Call(1));
				Assert.AreEqual("привет", client.Call(2));
				
			}
			finally{
				server.Stop();
			}
		}

		[Test]
		public void CanStopAndStartListener()
		{
			var server = SimpleSocket.CreateServer(new HelloWorldHandler());
			var client = new SimpleSocketClient<int, string>();
			try
			{
				server.Start();
				client.Call(1);
				//Assert.AreEqual("hello", client.Call(1));
				server.Stop();
				Assert.Throws<AggregateException>(()=>client.Call(1));
				server.Start();
				client.Call(1);
				//Assert.AreEqual("hello", client.Call(1));
			}
			finally
			{
				server.Stop();
			}
		}

		[Test]
		public void BinaryFormatterFunctionality(){
			var server = SimpleSocket.CreateServer(new Calculator());
			var client = new SimpleSocketClient<SimpleData, SimpleData>();
			try{
				server.Start();
				Assert.AreEqual(10,client.Call(new SimpleData{Command = 1,Arg = 6,Arg2 = 4}).Result);
				Assert.AreEqual(2, client.Call(new SimpleData { Command = 2, Arg = 6, Arg2 = 4 }).Result);
				
			}
			finally{
				server.Stop();
			}
		}

		[TestCase(100)]
		[TestCase(500)]
		[TestCase(1000)]
		[TestCase(2000)]
		[TestCase(4000)]
		public void SimplePerformanceTest(int cnt)
		{
			var server = SimpleSocket.CreateServer(new Calculator());
			var client = new SimpleSocketClient<SimpleData, SimpleData>();
			try{
				server.Start();
				var sw = Stopwatch.StartNew();
				IList<Task> agenda = new List<Task>();
				for (var i = 0; i <= cnt; i++){
					agenda.Add(client.CallAsync(new SimpleData{Command = 1, Arg = i, Arg2 = i}));
				}
				Task.WaitAll(agenda.ToArray());
				sw.Stop();
				Console.WriteLine(sw.Elapsed);	
				Console.WriteLine(((decimal)cnt)/((decimal)sw.Elapsed.TotalMilliseconds)*1000m);
				
			}
			finally
			{
				server.Stop();
			}
		}
		[TestCase(8000)]
		[TestCase(16000)]
		[TestCase(32000)]
		[Explicit]
		public void StressPerformanceTest(int cnt)
		{
			var server = SimpleSocket.CreateServer(new Calculator());
			var client = new SimpleSocketClient<SimpleData, SimpleData>();
			try
			{
				server.Start();
				var sw = Stopwatch.StartNew();
				IList<Task> agenda = new List<Task>();
				for (var i = 0; i <= cnt; i++)
				{
					agenda.Add(client.CallAsync(new SimpleData { Command = 1, Arg = i, Arg2 = i }));
				}
				Task.WaitAll(agenda.ToArray());
				sw.Stop();
				Console.WriteLine(sw.Elapsed);
				Console.WriteLine(((decimal)cnt) / ((decimal)sw.Elapsed.TotalMilliseconds) * 1000m);

			}
			finally
			{
				server.Stop();
			}
		}
	}
}

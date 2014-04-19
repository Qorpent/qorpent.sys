using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Uson;

namespace Qorpent.Core.Tests.Log{
	[TestFixture]
	public class SessionLogTest{
		/// <summary>
		/// Тест на возможность регистрации и поиска по фильтрам
		/// </summary>
		[Test]
		public void CanRegisterAndFind(){
			var sl = new SessionLog();
			var time = DateTime.Now;
			var m1 = sl.Register(new LogMessage{Code = "a", Level = LogLevel.Error,Time = time.AddSeconds(10),RequireAccept = true});
			var m2 = sl.Register(new LogMessage { Code = "a", Level = LogLevel.Warning, Time = time.AddSeconds(20) });
			var m3 = sl.Register(new LogMessage { Code = "a", Level = LogLevel.Error, Time = time.AddSeconds(30) });
			var m4 = sl.Register(new LogMessage { Code = "a", Level = LogLevel.Warning, Time = time.AddSeconds(40) ,RequireAccept = true,Accepted = true});
			var m5 = sl.Register(new LogMessage { Code = "a", Level = LogLevel.Warning, Time = time.AddSeconds(50) ,RequireAccept = true, Active = false});
			Assert.AreEqual(4,sl.Get().Count());
			Assert.AreEqual(4,sl.Get(new SessionLogQuery{StartLevel = LogLevel.Warning}).Count());
			Assert.AreEqual(2,sl.Get(new SessionLogQuery{StartLevel = LogLevel.Error}).Count());
			Assert.AreEqual(0,sl.Get(new SessionLogQuery{StartLevel = LogLevel.Fatal}).Count());
			Assert.AreEqual(3,sl.Get(new SessionLogQuery{StartTimestamp = m1.Timestamp}).Count());
			Assert.AreEqual(2,sl.Get(new SessionLogQuery{StartTimestamp = m2.Timestamp}).Count());
			Assert.AreEqual(1,sl.Get(new SessionLogQuery{StartTimestamp = m3.Timestamp}).Count());
			Assert.AreEqual(0,sl.Get(new SessionLogQuery{StartTimestamp = m4.Timestamp}).Count());
			Assert.AreEqual(2,sl.Get(new SessionLogQuery{OnlyRequests = true}).Count());
			Assert.AreEqual(1,sl.Get(new SessionLogQuery{OnlyAccepted = true}).Count());
			Assert.AreEqual(1,sl.Get(new SessionLogQuery{OnlyNotAccepted = true}).Count());
		}

		[Test]
		public void CanTrackConfigured(){
			var sl = new SessionLog();
			sl.Register(SessionLogTypeConfiguration.ApplicationAlert("x"));
			var time = DateTime.Now;
			var m = sl.Register(new LogMessage{Code = "x",Time=time, Level = LogLevel.Error, Message = "xxx", Data = "yyy"});
			Assert.True(m.RequireAccept);
			Assert.False(m.Accepted);
			Assert.True(m.Active);

			//data and time grade
			var m2 =
				sl.Register(new LogMessage{
					Code = "x",
					Time = time.AddSeconds(10),
					Level = LogLevel.Error,
					Message = "xxx",
					Data = "yyy2"
				});
			Assert.AreSame(m2,m);
			Assert.AreEqual("yyy2",m.Data);
			Assert.AreEqual(time.AddSeconds(10), m.Time);

			//auto accept
			m2 =
				sl.Register(new LogMessage
				{
					Code = "x",
					Time = time.AddSeconds(10),
					Level = LogLevel.Trace,
					Message = "xxx",
					Data = "yyy2"
				});
			Assert.AreSame(m2, m);
			Assert.True(m.Accepted);
			Assert.True(m.Active);


			//auto remove
			m2 =
				sl.Register(new LogMessage
				{
					Code = "x",
					Time = time.AddSeconds(10),
					Level = LogLevel.Debug,
					Message = "xxx",
					Data = "yyy2"
				});
			Assert.AreSame(m2, m);
			Assert.True(m.Accepted);
			Assert.False(m.Active);
		}

		class CustomComparer:IEqualityComparer{
			public bool Equals(object x, object y){
				var _x = x as string;
				var _y = y as string;
				return _x[0] == _y[0];
			}

			public int GetHashCode(object obj){
				return obj.GetHashCode();
			}
		}

		[Test]
		public void CanTrackWithCustomComparer()
		{
			var sl = new SessionLog();
			sl.Register(SessionLogTypeConfiguration.ApplicationAlert("x",new CustomComparer()));
			var m = sl.Register(new LogMessage { Code = "x",Level = LogLevel.Error, Message = "xxx", Data = "yyy" });
			var m2 = sl.Register(new LogMessage{Code = "x", Level = LogLevel.Error, Message = "xxx", Data = "yxx"});//really not changed emulation
			Assert.AreSame(m2,m);
			Assert.AreEqual("yyy",m.Data);
			m2 = sl.Register(new LogMessage { Code = "x", Level = LogLevel.Error, Message = "xxx", Data = "xyy" });//really not changed emulation
			Assert.AreSame(m2, m);
			Assert.AreEqual("xyy", m.Data);
			
		}


		[Test]
		public void CanTrackWithUObjData()
		{
			var sl = new SessionLog();
			sl.Register(SessionLogTypeConfiguration.ApplicationAlert("x"));
			var m = sl.Register(new LogMessage { Code = "x", Level = LogLevel.Error, Message = "xxx", Data =new{x=1,y=2}.ToUson()});
			var u = m.Data;
			sl.Register(new LogMessage { Code = "x", Level = LogLevel.Error, Message = "xxx", Data = new { y = 2, x = 1 }.ToUson() });
			Assert.AreEqual(u,m.Data);
			sl.Register(new LogMessage { Code = "x", Level = LogLevel.Error, Message = "xxx", Data = new { y = 1, x = 2 }.ToUson() });
			Assert.AreNotEqual(u, m.Data);
		}
	}
}
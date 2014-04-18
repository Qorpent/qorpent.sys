using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Qorpent.Core.Tests.Tasking
{
	[TestFixture]
	public class ScheduleTaskTest
	{
		[Test]
		public void CanRunRegular(){
			var times = 0;
			ScheduledTask s = null;
			s = new ScheduledTask(() =>{
				times++;
				if (times == 3){
					s.Stop();
				}
			}){StartInterval = 10, AfterSuccessInterval = 10};
			s.Start();
			Thread.Sleep(100);
			Assert.AreEqual(3,times);
		}

		[Test]
		public void CanRunAfterError()
		{
			var times = 0;
			var errors = 0;
			ScheduledTask s = null;
			s = new ScheduledTask(() =>
			{
				times++;
				if (times == 2){
					throw new Exception("error");
				}
				if (times == 4)
				{
					s.Stop();
				}
			}) { StartInterval = 10, AfterSuccessInterval = 10, AfterErrorInterval = 10,ProceedOnError = true};
			s.OnError += _ => errors++;
			s.Start();
			Thread.Sleep(100);
			Assert.AreEqual(4, times);
			Assert.AreEqual(1, errors);
		}
	}
}

#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : StaticInstanceTest.cs
// Project: Qorpent.Utils.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace Qorpent.Utils.Tests {
	/// <summary>
	/// 	Goal of this test is to determine if non-static design is preferable (thread safe and quick)
	/// 	current result - they are significally equal
	/// 
	/// 	solution for now - instance, static and static/singleinst design are very close by performance in
	/// 	real-world range of thread count
	/// 
	/// 	while statics are little quicker, we choose static/singleinst+inst design because of more flexivity -
	/// 	we can add some cache, special logic to instance , still using common functionality through static
	/// 	without code duplacate
	/// </summary>
	[TestFixture]
	public class StaticInstanceTest {
		public class instance {
			public string exec(int command) {
				var result = "";
				for (var i = 0; i < 2000 - command; i++) {
					result += command + "_";
				}
				return result;
			}
		}

		public static class statics {
			private static readonly instance _e;

			static statics() {
				_e = new instance();
			}

			public static string exec(int command) {
				var result = "";

				for (var i = 0; i < 2000 - command; i++) {
					result += command + "_";
				}

				return result;
			}

			public static string exec2(int command) {
				return _e.exec(command);
			}
		}

		public class tester {
			public tester(int command, IDictionary<int, string> result) {
				_inst = new instance();
				_command = command;

				_result = result;
			}

			public string byinst() {
				Thread.Sleep(5);
				var r = _inst.exec(_command);
				Thread.Sleep(5);
				lock (_result) {
					_result[_command] = r;
				}
				return r;
			}

			public string bystat() {
				Thread.Sleep(5);
				var r = statics.exec(_command);
				Thread.Sleep(5);
				lock (_result) {
					_result[_command] = r;
				}
				return r;
			}

			public string bystatinst() {
				Thread.Sleep(5);
				var r = statics.exec2(_command);
				Thread.Sleep(5);
				lock (_result) {
					_result[_command] = r;
				}
				return r;
			}

			private readonly int _command;
			private readonly instance _inst;
			private readonly IDictionary<int, string> _result;
		}

		private TimeSpan instResult;
		private TimeSpan statResult;
		private TimeSpan statinstResult;

		[Test]
		[Ignore(
			"it's .net itself test to ensure performance and design issues on class architecture, not tend for everyday testing")
		]
		public void instTest() {
			IList<Thread> threads = new List<Thread>();
			IDictionary<int, string> result = new Dictionary<int, string>();
			for (var i = 0; i < 500; i++) {
				var i1 = i;
				var t = new tester(i1, result);
				var thread = new Thread(() => t.byinst());
				threads.Add(thread);
			}
			var sw = Stopwatch.StartNew();
			foreach (var thread in threads) {
				thread.Start();
			}
			foreach (var thread in threads) {
				thread.Join();
			}
			sw.Stop();
			instResult = sw.Elapsed;
			Console.WriteLine("inst: " + instResult);
			foreach (var test in result) {
				Assert.AreEqual(statics.exec(test.Key), test.Value);
			}
		}

		[Test]
		[Ignore(
			"it's .net itself test to ensure performance and design issues on class architecture, not tend for everyday testing")
		]
		public void performanceEqualityTest() {
			staticsTest();
			instTest();
			staticsInstTest();

			Assert.True(Math.Abs((instResult - statResult).TotalMilliseconds) <= 200);
		}

		[Test]
		[Ignore(
			"it's .net itself test to ensure performance and design issues on class architecture, not tend for everyday testing")
		]
		public void staticsInstTest() {
			IList<Thread> threads = new List<Thread>();
			IDictionary<int, string> result = new Dictionary<int, string>();
			for (var i = 0; i < 500; i++) {
				var i1 = i;
				var t = new tester(i1, result);
				var thread = new Thread(() => t.bystatinst());
				threads.Add(thread);
			}
			var sw = Stopwatch.StartNew();
			foreach (var thread in threads) {
				thread.Start();
			}
			foreach (var thread in threads) {
				thread.Join();
			}
			sw.Stop();
			statinstResult = sw.Elapsed;
			Console.WriteLine("statinst: " + statResult);
			foreach (var test in result) {
				Assert.AreEqual(statics.exec(test.Key), test.Value);
			}
		}

		[Test]
		[Ignore(
			"it's .net itself test to ensure performance and design issues on class architecture, not tend for everyday testing")
		]
		public void staticsTest() {
			IList<Thread> threads = new List<Thread>();
			IDictionary<int, string> result = new Dictionary<int, string>();
			for (var i = 0; i < 500; i++) {
				var i1 = i;
				var t = new tester(i1, result);
				var thread = new Thread(() => t.bystat());
				threads.Add(thread);
			}
			var sw = Stopwatch.StartNew();
			foreach (var thread in threads) {
				thread.Start();
			}
			foreach (var thread in threads) {
				thread.Join();
			}
			sw.Stop();
			statResult = sw.Elapsed;
			Console.WriteLine("stat: " + statResult);
			foreach (var test in result) {
				Assert.AreEqual(statics.exec(test.Key), test.Value);
			}
		}
	}
}
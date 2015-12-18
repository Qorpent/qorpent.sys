using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Data.elastic;
using Qorpent.IO.Net;

namespace Qorpent.Data.Tests.Elastic
{
    [TestFixture]
    public class SequenceTest {
        private const string TEST_INDEX = "test_seq";
        private const string TEST_URL = "http://127.0.0.1:9200";
        private readonly HttpClient http = new HttpClient {BaseUri = new Uri(TEST_URL)};
        [SetUp]
        public void Setup() {
            http.Call(HttpRequest.Delete($"{TEST_URL}/{TEST_INDEX}"));
            Assert.AreEqual(404,http.Call(HttpRequest.Head($"{TEST_URL}/{TEST_INDEX}")).State);
        }
        [Test]
        public void BasicFunctionality() {
            var seq = new EsSequence {Index = TEST_INDEX, Url = TEST_URL};
            Assert.AreEqual(10,seq.Next());
            Assert.AreEqual(20,seq.Next());
        }

        [Test]
        public void DifferentKeys()
        {
            var seq = new EsSequence { Index = TEST_INDEX, Url = TEST_URL };
            var seq2 = new EsSequence { Index = TEST_INDEX, Url = TEST_URL, Key = "2"};
            Assert.AreEqual(10, seq.Next());
            Assert.AreEqual(20, seq.Next());
            Assert.AreEqual(10, seq2.Next());
            Assert.AreEqual(20, seq2.Next());
        }

        [Test]
        public void CanBeSet()
        {
            var seq = new EsSequence { Index = TEST_INDEX, Url = TEST_URL };
            Assert.AreEqual(10, seq.Next());
            Assert.AreEqual(20, seq.Next());
            seq.Set(10);
            Assert.AreEqual(20, seq.Next());
        }

        [Test]
        public void CanGetCurrent() {
            var seq = new EsSequence { Index = TEST_INDEX, Url = TEST_URL };
            Assert.AreEqual(int.MinValue, seq.GetCurrent());
            Assert.AreEqual(10, seq.Next());
            Assert.AreEqual(20, seq.Next());
            Assert.AreEqual(20,seq.GetCurrent());
            Assert.AreEqual(30, seq.Next());
        }


        [Test]
        public void DifferentSteps()
        {
            var seq = new EsSequence { Index = TEST_INDEX, Url = TEST_URL };
            var seq2 = new EsSequence { Index = TEST_INDEX, Url = TEST_URL, Step = 1 };
            Assert.AreEqual(10, seq.Next());
            Assert.AreEqual(20, seq.Next());
            Assert.AreEqual(21, seq2.Next());
            Assert.AreEqual(31, seq.Next());
        }

        [Test]
        public void DifferentInit() {
            var seq = new EsSequence { Index = TEST_INDEX, Url = TEST_URL, Initial = -2, Step = -3};
            Assert.AreEqual(-2, seq.Next());
            Assert.AreEqual(-5, seq.Next());
        }

        [Test]
        public void OneThreadPerformance1000() {
            var sw = Stopwatch.StartNew();
            var seq = new EsSequence { Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0};
            for (var i = 0; i < 1000; i++) {
                var s = seq.Next();
                Assert.AreEqual(s,i);
            }
            sw.Stop();
            var ms = sw.ElapsedMilliseconds;
            Assert.LessOrEqual(ms,2000);
        }

        [Test]
        public void Concurent() {
            ConcurrentDictionary<int,int> _result = new ConcurrentDictionary<int,int>();
            var tasks = new List<Task>();
            var seqs = new[] {
                new EsSequence {Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0},
                new EsSequence {Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0},
                new EsSequence {Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0},
                new EsSequence {Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0}
            };
            for (var i = 0; i < 4*250; i++) {
                var s = seqs[i % 4];
                tasks.Add(Task.Run(() => {
                    var next = s.Next();
                    _result.TryAdd(next, next);
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Assert.AreEqual(1000,_result.Count);
        }

        [Test]
        public void ConcurentThreadSafe()
        {
            ConcurrentDictionary<int, int> _result = new ConcurrentDictionary<int, int>();
            var tasks = new List<Task>();
            var seq = new EsSequence {Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0};
            
            for (var i = 0; i < 4 * 250; i++)
            {
               tasks.Add(Task.Run(() => {
                    var next = seq.Next();
                    _result.TryAdd(next, next);
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Assert.AreEqual(1000, _result.Count);
        }

        [Test]
        public void ConcurentPerformance10000() {
            var sw = Stopwatch.StartNew();
            var tasks = new List<Task>();
            var seqs = new[] {
                new EsSequence {Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0},
                new EsSequence {Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0},
                new EsSequence {Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0},
                new EsSequence {Index = TEST_INDEX, Url = TEST_URL, Step = 1, Initial = 0}
            };
            for (var i = 0; i < 4 * 2500; i++) //10000
            {
                var s = seqs[i % 4];
                tasks.Add(Task.Run(() => {
                    var next = s.Next();
                }));
            }
            Task.WaitAll(tasks.ToArray());
            sw.Stop();
            var ms = sw.ElapsedMilliseconds;
            Assert.LessOrEqual(ms,3000);
        }
    }
}

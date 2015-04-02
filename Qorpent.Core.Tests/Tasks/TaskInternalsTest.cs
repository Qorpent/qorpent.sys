using System;
using System.IO;
using NUnit.Framework;

namespace Qorpent.Core.Tests.Tasks {
    [TestFixture]
    public class TaskInternalsTest {
        [Test]
        public void CanSimplyRun() {
            var sw = new StringWriter();
            new TestTask(sw).Execute();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"
S:Pending
S:Executing
InternalWork
S:Success
".Trim(), sw.ToString().Trim());
        }

        [Test]
        public void CanErrorRun() {
            var sw = new StringWriter();
            new TestTask(sw) {DoError = true}.Execute();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"
S:Pending
S:Executing
S:Error
OnError
".Trim(), sw.ToString().Trim());
        }

        [Test]
        public void CanIgnoreErrorRun() {
            var sw = new StringWriter();
            new TestTask(sw) {DoError = true, IgnoreErrors = true}.Execute();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"
S:Pending
S:Executing
S:SuccessIgnoreErrors
".Trim(), sw.ToString().Trim());
        }

        [Test]
        public void NoRunOnceMode() {
            var sw = new StringWriter();
            new TestTask(sw) {WasRunOnce = true}.Execute();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"
S:Pending
S:Executing
InternalWork
S:Success
".Trim(), sw.ToString().Trim());
        }

        [Test]
        public void RunOnceMode() {
            var sw = new StringWriter();
            new TestTask(sw) {WasRunOnce = true, RunOnce = true}.Execute();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"
S:Pending
S:SuccessOnce
".Trim(), sw.ToString().Trim());
        }

        [Test]
        public void CascadeError() {
            var sw = new StringWriter();
            var t1 = new TestTask {DoError = true};
            var t2 = new TestTask(sw);
            t2.RequiredModules.Add(t1);
            t1.Execute();
            t2.Execute();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"
S:Pending
S:CascadeError
".Trim(), sw.ToString().Trim());
        }

        [Test]
        public void SuccessNotRun() {
            var sw = new StringWriter();
            new TestTask(sw) {NoRun = true}.Execute();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"
S:Pending
S:SuccessNotRun
".Trim(), sw.ToString().Trim());
        }

        [Test]
        public void SuccessNotRun_Over_CascadeError() {
            var sw = new StringWriter();
            var t1 = new TestTask {DoError = true};
            var t2 = new TestTask(sw) {NoRun = true};
            t2.RequiredModules.Add(t1);
            t1.Execute();
            t2.Execute();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"
S:Pending
S:SuccessNotRun

".Trim(), sw.ToString().Trim());
        }

        [Test]
        public void SuccessOnce_Over_CascadeError() {
            var sw = new StringWriter();
            var t1 = new TestTask {DoError = true};
            var t2 = new TestTask(sw) {WasRunOnce = true, RunOnce = true};
            t2.RequiredModules.Add(t1);
            t1.Execute();
            t2.Execute();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"
S:Pending
S:SuccessOnce

".Trim(), sw.ToString().Trim());
        }
    }
}
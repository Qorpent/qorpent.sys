using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Qorpent.Tasks;
using Qorpent.Utils;

namespace Qorpent.Core.Tests.Tasks {
    [TestFixture]
    public class SimpleUpdaterTest {
        private string dir;
        private string dira;
        private string dirb;
        private StringWriter sw;
        private string filea;
        private string fileb;
        private TestUpdater uab;
        private TestUpdater uba;

        [SetUp]
        public void Setup() {
            sw = new StringWriter();
            dir = FileSystemHelper.ResetTemporaryDirectory();
            dira = Path.Combine(dir, "a");
            dirb = Path.Combine(dir, "b");
            filea = Path.Combine(dira, "a.txt");
            fileb = Path.Combine(dirb, "b.txt");
            Directory.CreateDirectory(dira);
            Directory.CreateDirectory(dirb);
            uab = new TestUpdater(sw, filea, fileb);
            uba = new TestUpdater(sw, fileb, filea);

        }

        private void writeA(string content) {
            Thread.Sleep(100);
            File.WriteAllText(filea, content);
        }

        private void writeB(string content) {
            Thread.Sleep(100);
            File.WriteAllText(fileb, content);
        }

        private void check(string result) {
            var actual = sw.ToString().Trim();
            var test = result.Trim();
            Console.WriteLine(actual);
            Assert.AreEqual(test, actual);
        }

        [Test]
        public void CanCopyNewer() {
            writeB("x");
            writeA("y");
            uab.Execute();
            check(@"
S:Pending
S:Executing
copy a.txt to b.txt
S:Success
");

        }

        [Test]
        public void WillNotCopyTwiceAfterRefresh()
        {
            writeB("x");
            writeA("y");
            uab.Execute();
            uab.Refresh();
            uab.Execute();
            check(@"
S:Pending
S:Executing
copy a.txt to b.txt
S:Success
S:Pending
S:SuccessNotRun
");

        }

        [Test]
        public void WillNotMirrorTwiceSame()
        {
            writeB("x");
            writeA("y");
            uab.Execute();
            uba.Execute();
            check(@"
S:Pending
S:Executing
copy a.txt to b.txt
S:Success
S:Pending
S:SuccessNotRun
");

        }

        [Test]
        public void PreventCopyOlder() {
            writeA("y");
            writeB("x");

            uab.Execute();
            check(@"
S:Pending
S:SuccessNotRun
");
        }

        [Test]
        public void WillCopyNotExisted()
        {
            writeA("y");

            uab.Execute();
            check(@"
S:Pending
S:Executing
copy a.txt to b.txt
S:Success
");
        }


        [Test]
        public void OnceCopyTest()
        {
            var syncer = new Job();
            uab.RunOnce = true;
            uba.RunOnce = true;
            syncer.Tasks["ab"] = uab;
            syncer.Tasks["ba"] = uba;
            writeA("y");
            syncer.Execute();
            writeA("y2");
            syncer.Execute();
            writeB("y3");
            syncer.Execute();
            check(@"
ab - S:Pending
ba - S:Pending
ab - S:Executing
ab - copy a.txt to b.txt
ab - S:Success
ba - S:SuccessOnce
ab - S:Pending
ba - S:Pending
ab - S:SuccessOnce
ba - S:SuccessOnce
ab - S:Pending
ba - S:Pending
ab - S:SuccessOnce
ba - S:SuccessOnce
");
        }

        [Test]
        public void SimpleSyncerJob() {
            var syncer = new Job();
            syncer.Tasks["ab"] = uab;
            syncer.Tasks["ba"] = uba;
            writeA("y");
            syncer.Execute();
            syncer.Execute();
            writeA("y2");
            syncer.Execute();
            writeB("y3");
            syncer.Execute();
            check(@"
ab - S:Pending
ba - S:Pending
ab - S:Executing
ab - copy a.txt to b.txt
ab - S:Success
ba - S:SuccessNotRun
ab - S:Pending
ba - S:Pending
ab - S:SuccessNotRun
ba - S:SuccessNotRun
ab - S:Pending
ba - S:Pending
ab - S:Executing
ab - copy a.txt to b.txt
ab - S:Success
ba - S:SuccessNotRun
ab - S:Pending
ba - S:Pending
ab - S:SuccessNotRun
ba - S:Executing
ba - copy b.txt to a.txt
ba - S:Success
");
        }
    }
}
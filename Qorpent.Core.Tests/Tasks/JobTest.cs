using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Tasks;

namespace Qorpent.Core.Tests.Tasks
{
    [TestFixture]
    public class JobTest
    {
        [Test]
        public void InitializesTasks() {
            var sw = new StringWriter();
            var job = new Job();
            var x = job.Tasks["x"] = new TestTask(sw){Idx=10,Requirements = new []{"y","z"}};
            var y = job.Tasks["y"] = new TestTask(sw);
            var z = job.Tasks["z"] = new TestTask(sw){Requirements = new []{"y"}};
            job.Initialize();
            Assert.AreEqual("x",x.Name);
            Assert.AreEqual("y",y.Name);
            Assert.AreEqual("z",z.Name);
            Assert.AreEqual(2,x.RequiredModules.Count);
            Assert.AreEqual(1,z.RequiredModules.Count);
            Assert.AreEqual(0,y.RequiredModules.Count);
            Assert.AreEqual(10,x.Idx);
            Assert.AreEqual(1000000,y.Idx);
            Assert.AreEqual(1000000,z.Idx);
        }

        [Test]
        public void SimpleExecution() {
            var sw = new StringWriter();
            var job = new Job();
            job.Tasks["x"] = new TestTask(sw) { Idx = 10, Requirements = new[] { "y", "z" } };
            job.Tasks["y"] = new TestTask(sw);
            job.Tasks["z"] = new TestTask(sw) { Requirements = new[] { "y" } };
            Assert.False(job.Success);
            job.Execute();
            Assert.True(job.Success);
            Console.WriteLine(sw.ToString());
            var result = sw.ToString().Trim();
            Assert.AreEqual(@"
x - S:Pending
y - S:Pending
z - S:Pending
y - S:Executing
y - InternalWork
y - S:Success
z - S:Executing
z - InternalWork
z - S:Success
x - S:Executing
x - InternalWork
x - S:Success
".Trim(),result);
        }

        [Test]
        public void SimpleOrder()
        {
            var sw = new StringWriter();
            var job = new Job();
            job.Tasks["x"] = new TestTask(sw) { Idx = 10};
            job.Tasks["y"] = new TestTask(sw) {Idx = 30};
            job.Tasks["z"] = new TestTask(sw) { Idx=20 };
             job.Execute();
            Console.WriteLine(sw.ToString());
            var result = sw.ToString().Trim();
            Assert.AreEqual(@"
x - S:Pending
y - S:Pending
z - S:Pending
x - S:Executing
x - InternalWork
x - S:Success
z - S:Executing
z - InternalWork
z - S:Success
y - S:Executing
y - InternalWork
y - S:Success
".Trim(), result);
        }

        [Test]
        public void GroupDependencyOrder()
        {
            var sw = new StringWriter();
            var job = new Job();
            job.Tasks["x"] = new TestTask(sw) { Idx = 10, Requirements = new []{"@X"}};
            job.Tasks["y"] = new TestTask(sw) { Idx = 30 ,Group = "X"};
            job.Tasks["z"] = new TestTask(sw) { Idx = 20,Group  = "X"};
            job.Execute();
            Console.WriteLine(sw.ToString());
            var result = sw.ToString().Trim();
            Assert.AreEqual(@"
x - S:Pending
y - S:Pending
z - S:Pending
z - S:Executing
z - InternalWork
z - S:Success
y - S:Executing
y - InternalWork
y - S:Success
x - S:Executing
x - InternalWork
x - S:Success
".Trim(), result);
        }

        [Test]
        public void ErrorExecution() {
            var sw = new StringWriter();
            var job = new Job();
            job.Tasks["x"] = new TestTask(sw) { Idx = 10, Requirements = new[] { "y", "z" } };
            job.Tasks["y"] = new TestTask(sw);
            job.Tasks["z"] = new TestTask(sw) { Requirements = new[] { "y" } ,DoError = true};
            Assert.False(job.Success);
            Assert.False(job.HasError);
            job.Execute();
            Assert.False(job.Success);
            Assert.True(job.HasError);
            Console.WriteLine(sw.ToString());
            var result = sw.ToString().Trim();
            Assert.AreEqual(@"
x - S:Pending
y - S:Pending
z - S:Pending
y - S:Executing
y - InternalWork
y - S:Success
z - S:Executing
z - S:Error
z - OnError
x - S:CascadeError
".Trim(), result);
        }

        [Test]
        public void CycledEscapeExecution()
        {
            var sw = new StringWriter();
            var job = new Job();
            job.Tasks["x"] = new TestTask(sw) { Idx = 10, Requirements = new[] { "y", "z" } };
            job.Tasks["y"] = new TestTask(sw);
            job.Tasks["z"] = new TestTask(sw) { Requirements = new[] { "y","x" } };
            Assert.Throws<Exception>(()=>job.Execute());
            
        }

        [Test]
        public void DetectsDependencyLack()
        {
            var sw = new StringWriter();
            var job = new Job();
            var x = job.Tasks["x"] = new TestTask(sw) { Idx = 10, Requirements = new[] { "AA", "z" } };
            var y = job.Tasks["y"] = new TestTask(sw);
            var z = job.Tasks["z"] = new TestTask(sw) { Requirements = new[] { "y" } };
            Assert.Throws<Exception>(()=>job.Initialize());
            
        }
    }
}

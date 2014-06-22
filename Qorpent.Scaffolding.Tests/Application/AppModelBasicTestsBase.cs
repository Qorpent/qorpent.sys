using System;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Application;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Tests.Application {
    public abstract class AppModelBasicTestsBase {
        public  ApplicationModel TestModel(string code, string testresult = null) {
            var ctx = BSharpCompiler.Compile(code);
            var model = new ApplicationModel().Setup(ctx);
            if (!model.IsValid) {
                foreach (var error in model.Errors) {
                    Console.WriteLine(error.ToLogString());
                }
            }
            Assert.True(model.IsValid);
            var strmodel = model.ToString();
            Console.WriteLine(strmodel);
            if (!string.IsNullOrWhiteSpace(testresult)) {
                Assert.AreEqual(testresult.Trim().LfOnly(), strmodel.Trim().LfOnly());
            }
            return model;
        }
    }
}
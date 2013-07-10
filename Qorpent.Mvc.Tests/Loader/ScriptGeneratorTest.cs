using System;
using NUnit.Framework;
using Qorpent.Mvc.Loader;

namespace Qorpent.Mvc.Tests.Loader {
    [TestFixture]
    public class ScriptGeneratorTest {
        [Test]
        public void GenerateFullAdminScript() {
            var raw = PackageGenerator.Get(3);
            var set = new LoadPackageSet(raw);
            var gen = new LoadScriptGenerator();
            var result = gen.Generate(set[LoadLevel.Admin]);
            Console.WriteLine(result);
            Assert.AreEqual(@"/* auto generated load set started */ 
window.templates = window.templates || {};
/* auto generated pkg ag () started */ 
document.write(""<script src='scripts/agg.js' type='text/javascript' ></script>"");
document.write(""<link rel='stylesheet' href='styles/agu.css' type='text/css' />"");
$.ajax({ url: 'tpl/aga.html', async: false }).success(function(data){templates['aga'] = data;});
/* auto generated pkg ag finished */ 
/* auto generated pkg au () started */ 
document.write(""<link rel='stylesheet' href='styles/aug.css' type='text/css' />"");
$.ajax({ url: 'tpl/auu.html', async: false }).success(function(data){templates['auu'] = data;});
document.write(""<script src='scripts/aua.js' type='text/javascript' ></script>"");
/* auto generated pkg au finished */ 
/* auto generated pkg aa () started */ 
$.ajax({ url: 'tpl/aag.html', async: false }).success(function(data){templates['aag'] = data;});
document.write(""<script src='scripts/aau.js' type='text/javascript' ></script>"");
document.write(""<link rel='stylesheet' href='styles/aaa.css' type='text/css' />"");
/* auto generated pkg aa finished */ 
/* auto generated load set finished */", result.Trim());
        }


        [Test]
        public void GenerateFullAuthScript()
        {
            var raw = PackageGenerator.Get(3);
            var set = new LoadPackageSet(raw);
            var gen = new LoadScriptGenerator();
            var result = gen.Generate(set[LoadLevel.Auth]);
            Console.WriteLine(result);
            Assert.AreEqual(@"/* auto generated load set started */ 
window.templates = window.templates || {};
/* auto generated pkg ag () started */ 
document.write(""<script src='scripts/agg.js' type='text/javascript' ></script>"");
document.write(""<link rel='stylesheet' href='styles/agu.css' type='text/css' />"");
/* auto generated pkg ag finished */ 
/* auto generated pkg au () started */ 
document.write(""<link rel='stylesheet' href='styles/aug.css' type='text/css' />"");
$.ajax({ url: 'tpl/auu.html', async: false }).success(function(data){templates['auu'] = data;});
/* auto generated pkg au finished */ 
/* auto generated load set finished */", result.Trim());
        }

        [Test]
        public void GenerateFullGuestScript()
        {
            var raw = PackageGenerator.Get(3);
            var set = new LoadPackageSet(raw);
            var gen = new LoadScriptGenerator();
            var result = gen.Generate(set[LoadLevel.Guest]);
            Console.WriteLine(result);
            Assert.AreEqual(@"/* auto generated load set started */ 
window.templates = window.templates || {};
/* auto generated pkg ag () started */ 
document.write(""<script src='scripts/agg.js' type='text/javascript' ></script>"");
/* auto generated pkg ag finished */ 
/* auto generated load set finished */", result.Trim());
        }
    }
}
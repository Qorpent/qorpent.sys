using System;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Mvc.Loader;

namespace Qorpent.Mvc.Tests.Loader {
    [TestFixture]
    public class BxlLoadingTest {
        private LoadPackageSet packages;
        private LoadScriptGenerator gen;
        private const string LivingScript = @"
pkg qorpent.ui level=auth
    uses qorpent.core
    load qorpent.main.js
    load qorpent.main.css
    load qorpent.admin.tpl.html level=admin
    load qorpent.admin.js level=admin
pkg jquery
    load jquery.min.js
pkg qorpent.core
    uses jquery
    load ""rel='favicon' href='./favicon.png'""
    load ""generator='qorpent.ui'""
    load qorpent.qweb.js
    load qorpent.layout.js
    load qorpent.widget.js
    load qorpent.auth.js
    load qorpent.debug.js level=auth
    load qorpent.core.css
";
        [SetUp]
        public void Setup() {
            var bxl = new BxlParser();
            var reader = new LoadPackageReader();
            var xml = bxl.Parse(LivingScript);
            var pkgs = reader.Read(xml);
            packages = new LoadPackageSet(pkgs);
            gen = new LoadScriptGenerator();

        }
            
        [Test]
        public void ValidConfigGeneration_Admin() {
            var result = gen.Generate(packages[LoadLevel.Admin]);
            Console.WriteLine(result);
            Assert.AreEqual(@"/* auto generated load set started */ 
window.templates = window.templates || {};
/* auto generated pkg jquery () started */ 
document.write(""<script src='scripts/jquery.min.js' type='text/javascript' ></script>"");
/* auto generated pkg jquery finished */ 
/* auto generated pkg qorpent.core (jquery) started */ 
document.write(""<link rel='favicon' href='./favicon.png' />"");
document.write(""<meta generator='qorpent.ui' />"");
document.write(""<script src='scripts/qorpent.qweb.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.layout.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.widget.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.auth.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.debug.js' type='text/javascript' ></script>"");
document.write(""<link rel='stylesheet' href='styles/qorpent.core.css' type='text/css' />"");
/* auto generated pkg qorpent.core finished */ 
/* auto generated pkg qorpent.ui (qorpent.core) started */ 
document.write(""<script src='scripts/qorpent.main.js' type='text/javascript' ></script>"");
document.write(""<link rel='stylesheet' href='styles/qorpent.main.css' type='text/css' />"");
$.ajax({ url: 'tpl/qorpent.admin.tpl.html', async: false }).success(function(data){templates['qorpent.admin'] = data;});
document.write(""<script src='scripts/qorpent.admin.js' type='text/javascript' ></script>"");
/* auto generated pkg qorpent.ui finished */ 
/* auto generated load set finished */", result.Trim());
        }

        [Test]
        public void ValidConfigGeneration_Auth()
        {
            var result = gen.Generate(packages[LoadLevel.Auth]);
            Console.WriteLine(result);
            Assert.AreEqual(@"/* auto generated load set started */ 
window.templates = window.templates || {};
/* auto generated pkg jquery () started */ 
document.write(""<script src='scripts/jquery.min.js' type='text/javascript' ></script>"");
/* auto generated pkg jquery finished */ 
/* auto generated pkg qorpent.core (jquery) started */ 
document.write(""<link rel='favicon' href='./favicon.png' />"");
document.write(""<meta generator='qorpent.ui' />"");
document.write(""<script src='scripts/qorpent.qweb.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.layout.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.widget.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.auth.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.debug.js' type='text/javascript' ></script>"");
document.write(""<link rel='stylesheet' href='styles/qorpent.core.css' type='text/css' />"");
/* auto generated pkg qorpent.core finished */ 
/* auto generated pkg qorpent.ui (qorpent.core) started */ 
document.write(""<script src='scripts/qorpent.main.js' type='text/javascript' ></script>"");
document.write(""<link rel='stylesheet' href='styles/qorpent.main.css' type='text/css' />"");
/* auto generated pkg qorpent.ui finished */ 
/* auto generated load set finished */", result.Trim());
        }

        [Test]
        public void ValidConfigGeneration_Guest()
        {
            var result = gen.Generate(packages[LoadLevel.Guest]);
            Console.WriteLine(result);
            Assert.AreEqual(@"/* auto generated load set started */ 
window.templates = window.templates || {};
/* auto generated pkg jquery () started */ 
document.write(""<script src='scripts/jquery.min.js' type='text/javascript' ></script>"");
/* auto generated pkg jquery finished */ 
/* auto generated pkg qorpent.core (jquery) started */ 
document.write(""<link rel='favicon' href='./favicon.png' />"");
document.write(""<meta generator='qorpent.ui' />"");
document.write(""<script src='scripts/qorpent.qweb.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.layout.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.widget.js' type='text/javascript' ></script>"");
document.write(""<script src='scripts/qorpent.auth.js' type='text/javascript' ></script>"");
document.write(""<link rel='stylesheet' href='styles/qorpent.core.css' type='text/css' />"");
/* auto generated pkg qorpent.core finished */ 
/* auto generated load set finished */", result.Trim());
        }
    }
}
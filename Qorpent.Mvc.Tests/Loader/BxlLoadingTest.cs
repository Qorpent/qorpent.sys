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
(function(root,actions){
root.templates = root.templates || {};
root.arms = ((document.head.getElementsByClassName('qorpent-loader')[0].getAttribute('arm'))||'').split(',');
function allowed(arm,command){
    if(!(arm||command))return true; //empty condition
    if(arm=='default'&&!command)return true; // default arm always exists
    if(arm!='default'&& $.inArray(arm, root.arms)==-1)return false; //arm not match
    if(!command) return true; //arm match, command empty
    var cmd = command.split(',');
    if(!!actions[cmd[0]]){
        if(!!actions[cmd[0]][cmd[1]]){
            return true; //command match
        }
    }
    return false; //arm or command not match
}

/* auto generated pkg jquery ():default: started */ 
if(allowed('default','')){
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/jquery.min.js';document.head.appendChild(e);
}
}
/* auto generated pkg jquery finished */ 
/* auto generated pkg qorpent.core (jquery):default: started */ 
if(allowed('default','')){
if(allowed('','')){document.head.appendChild($('<link rel='favicon' href='./favicon.png' />')[0]);
}
if(allowed('','')){document.head.appendChild($('<meta generator='qorpent.ui' />')[0]);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.qweb.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.layout.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.widget.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.auth.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.debug.js';document.head.appendChild(e);
}
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/qorpent.core.css'})[0]);
}
}
/* auto generated pkg qorpent.core finished */ 
/* auto generated pkg qorpent.ui (qorpent.core):default: started */ 
if(allowed('default','')){
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.main.js';document.head.appendChild(e);
}
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/qorpent.main.css'})[0]);
}
if(allowed('','')){$.ajax({ url: 'tpl/qorpent.admin.tpl.html', async: false }).success(function(data){templates['qorpent.admin'] = data;});
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.admin.js';document.head.appendChild(e);
}
}
/* auto generated pkg qorpent.ui finished */ 
/* auto generated load set finished */ 
})(window, window.qweb.embedStorage._sys__myactions)", result.Trim());
        }

        [Test]
        public void ValidConfigGeneration_Auth()
        {
            var result = gen.Generate(packages[LoadLevel.Auth]);
            Console.WriteLine(result);
			Assert.AreEqual(@"/* auto generated load set started */ 
(function(root,actions){
root.templates = root.templates || {};
root.arms = ((document.head.getElementsByClassName('qorpent-loader')[0].getAttribute('arm'))||'').split(',');
function allowed(arm,command){
    if(!(arm||command))return true; //empty condition
    if(arm=='default'&&!command)return true; // default arm always exists
    if(arm!='default'&& $.inArray(arm, root.arms)==-1)return false; //arm not match
    if(!command) return true; //arm match, command empty
    var cmd = command.split(',');
    if(!!actions[cmd[0]]){
        if(!!actions[cmd[0]][cmd[1]]){
            return true; //command match
        }
    }
    return false; //arm or command not match
}

/* auto generated pkg jquery ():default: started */ 
if(allowed('default','')){
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/jquery.min.js';document.head.appendChild(e);
}
}
/* auto generated pkg jquery finished */ 
/* auto generated pkg qorpent.core (jquery):default: started */ 
if(allowed('default','')){
if(allowed('','')){document.head.appendChild($('<link rel='favicon' href='./favicon.png' />')[0]);
}
if(allowed('','')){document.head.appendChild($('<meta generator='qorpent.ui' />')[0]);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.qweb.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.layout.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.widget.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.auth.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.debug.js';document.head.appendChild(e);
}
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/qorpent.core.css'})[0]);
}
}
/* auto generated pkg qorpent.core finished */ 
/* auto generated pkg qorpent.ui (qorpent.core):default: started */ 
if(allowed('default','')){
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.main.js';document.head.appendChild(e);
}
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/qorpent.main.css'})[0]);
}
}
/* auto generated pkg qorpent.ui finished */ 
/* auto generated load set finished */ 
})(window, window.qweb.embedStorage._sys__myactions)", result.Trim());
        }

        [Test]
        public void ValidConfigGeneration_Guest()
        {
            var result = gen.Generate(packages[LoadLevel.Guest]);
            Console.WriteLine(result);
			Assert.AreEqual(@"/* auto generated load set started */ 
(function(root,actions){
root.templates = root.templates || {};
root.arms = ((document.head.getElementsByClassName('qorpent-loader')[0].getAttribute('arm'))||'').split(',');
function allowed(arm,command){
    if(!(arm||command))return true; //empty condition
    if(arm=='default'&&!command)return true; // default arm always exists
    if(arm!='default'&& $.inArray(arm, root.arms)==-1)return false; //arm not match
    if(!command) return true; //arm match, command empty
    var cmd = command.split(',');
    if(!!actions[cmd[0]]){
        if(!!actions[cmd[0]][cmd[1]]){
            return true; //command match
        }
    }
    return false; //arm or command not match
}

/* auto generated pkg jquery ():default: started */ 
if(allowed('default','')){
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/jquery.min.js';document.head.appendChild(e);
}
}
/* auto generated pkg jquery finished */ 
/* auto generated pkg qorpent.core (jquery):default: started */ 
if(allowed('default','')){
if(allowed('','')){document.head.appendChild($('<link rel='favicon' href='./favicon.png' />')[0]);
}
if(allowed('','')){document.head.appendChild($('<meta generator='qorpent.ui' />')[0]);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.qweb.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.layout.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.widget.js';document.head.appendChild(e);
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/qorpent.auth.js';document.head.appendChild(e);
}
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/qorpent.core.css'})[0]);
}
}
/* auto generated pkg qorpent.core finished */ 
/* auto generated load set finished */ 
})(window, window.qweb.embedStorage._sys__myactions)", result.Trim());
        }
    }
}
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

/* auto generated pkg ag ():: started */ 
if(allowed('','')){
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/agg.js';document.head.appendChild(e);
}
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/agu.css'})[0]);
}
if(allowed('','')){$.ajax({ url: 'tpl/aga.html', async: false }).success(function(data){templates['aga'] = data;});
}
}
/* auto generated pkg ag finished */ 
/* auto generated pkg au ():: started */ 
if(allowed('','')){
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/aug.css'})[0]);
}
if(allowed('','')){$.ajax({ url: 'tpl/auu.html', async: false }).success(function(data){templates['auu'] = data;});
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/aua.js';document.head.appendChild(e);
}
}
/* auto generated pkg au finished */ 
/* auto generated pkg aa ():: started */ 
if(allowed('','')){
if(allowed('','')){$.ajax({ url: 'tpl/aag.html', async: false }).success(function(data){templates['aag'] = data;});
}
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/aau.js';document.head.appendChild(e);
}
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/aaa.css'})[0]);
}
}
/* auto generated pkg aa finished */ 
/* auto generated load set finished */ 
})(window, window.qweb.embedStorage._sys__myactions)", result.Trim());
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

/* auto generated pkg ag ():: started */ 
if(allowed('','')){
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/agg.js';document.head.appendChild(e);
}
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/agu.css'})[0]);
}
}
/* auto generated pkg ag finished */ 
/* auto generated pkg au ():: started */ 
if(allowed('','')){
if(allowed('','')){document.head.appendChild($('<link/>').attr({rel:'stylesheet', href:'styles/aug.css'})[0]);
}
if(allowed('','')){$.ajax({ url: 'tpl/auu.html', async: false }).success(function(data){templates['auu'] = data;});
}
}
/* auto generated pkg au finished */ 
/* auto generated load set finished */ 
})(window, window.qweb.embedStorage._sys__myactions)", result.Trim());
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

/* auto generated pkg ag ():: started */ 
if(allowed('','')){
if(allowed('','')){var e = document.createElement('script');e.async=false;e.src='scripts/agg.js';document.head.appendChild(e);
}
}
/* auto generated pkg ag finished */ 
/* auto generated load set finished */ 
})(window, window.qweb.embedStorage._sys__myactions)", result.Trim());
        }
    }
}
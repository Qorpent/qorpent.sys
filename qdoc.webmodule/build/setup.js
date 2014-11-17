
var name = process.argv[2];
var namespace = name;
if(process.argv.length > 3){
    namespace = process.argv[3];
}
var fs = require("fs");
var manifest = require("../package.json");
manifest.moduleName = name;
manifest.name = name;
manifest.defaultNamespace =namespace;


if(process.argv.length>4){
    var type=process.argv[4];
    manifest.compile.type=type;
    manifest.compile.root=name;
    if(!fs.existsSync("src/js/"+name+".ts")) {
        fs.writeFileSync("src/js/" + name + ".ts", "export module "+name+" {\r\n\texport var __moduleName='"+name+"'}")
    }
    if(!fs.existsSync("tests/"+name+"-general.js")) {
        fs.writeFileSync("tests/" + name + "-general.js", "define(['" + name + "'],function($root){describe('" + name + "',function(){it('loaded',function(){$root['"+name+"'].__moduleName.should.equals('"+name+"')})})});");
        manifest.tests.push({name:name+"-general",deps:[name]});
    }
}
fs.writeFileSync("package.json",JSON.stringify(manifest,null,"\t"));

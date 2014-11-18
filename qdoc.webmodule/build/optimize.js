var manifest = require('./../package');
var requirejs = require('requirejs');
var reposRoot = "../../../../"; //path to match repository root
var fs = require("fs");
var config = {
    baseUrl: './src/js',
    paths: {},
    name: manifest.moduleName,
    out: './dist/js/' + manifest.moduleName + '.js',
    wrap: {
        end: "define(['" + manifest.moduleName + "'],function(_){return _;});"
    },
    include : [],
    generateSourceMaps: true,
    preserveLicenseComments: false,
    optimize: "uglify2",
    uglify2 : {
        compress : {
            global_defs : {
                "DEBUG" : true,
                "PROFILE":true
            }
        }
    }
};

if(!!manifest.defines){
    for(var d in manifest.defines){
        config.uglify2.compress.global_defs[d] = manifest.defines[d];
    }
}




config.paths = config.paths || {};
config.paths[manifest.moduleName] = "../../build/main";

if (!!manifest.compile){
    var type = manifest.compile.type || "lib";
    if(type=="api"){
        if(!!manifest.compile.root){
            config.paths[manifest.moduleName] = manifest.compile.root;
        }
    }else if(type=="lib"){
        var end = "define([";
        if(!!manifest.compile.root){
            end+="'"+manifest.compile.root+"'";
        }
        if(!!manifest.compile.include){
            manifest.compile.include.forEach(function(_,i){
                if(i!=0 || !!manifest.compile.root)end+=", ";
                end+="'"+_+"'"
            });
        }
        end+="], function(";
        if(!!manifest.compile.root){
            end+="$root";
        }
        if(!!manifest.compile.include){
            manifest.compile.include.forEach(function(_,i){
                if(i!=0 || !!manifest.compile.root)end+=", ";
                end+="dep"+i;
            });
        }
        if(!!manifest.compile.root){
            end+="){var lib=$root;";
        }else {
            end += "){var lib={};";
        }
        if(!!manifest.compile.include){
            manifest.compile.include.forEach(function(_,i){
                end+="lib['"+_+"']=dep"+i+";";
            });
        }
        end+="return lib;});"
        config.wrap.end = end;
    }
    if(!!manifest.compile.include){
        manifest.compile.include.forEach(function(_){
            config.include.push(_);
        });
    }
}

// setup fake module dependency
if (manifest.webModuleDependency) {
    for (var i in manifest.webModuleDependency) {
        config.paths[i] = "empty:";
    }
}

var libs = fs.readdirSync("lib");
libs.forEach(function(_){
    if(_.match(/\.js$/)){
        var name = _.replace(/\.js$/,'');
        if (!(name in config.paths)){
            config.paths[name] = "empty:";
        }
    }
});

// generates module without external dependencies (due to stubModules)
requirejs.optimize(config, function () {

    //NEXT GENERATION
    //generate full version with all modules bundled - usefull for standalone usage and browser testing
    config.out = config.out.replace(/\.js$/, '-full.js');
    // setup module dependency
    if (manifest.webModuleDependency) {
        for (var i in manifest.webModuleDependency) {
            var path = manifest.webModuleDependency[i];
            // если путь не является обычным путем к файлу (полным или относительным)
            if(!path.match(/^((\/)|(\w\:\/)|(\.))/)){
                //если путь включает в себя ссылку на файл
                if(path.match(/[^!?](\!)|(\?)[\w\-\d]+$/)){
                    path = path.replace(/\!/,'.webmodule/dist/js/');
                    path = path.replace(/\?/,'.webmodule/src/js/');
                }else if (path.match(/^\!/)) { //нестандартный путь от репозитория, без веб-модулей
                    path = path.substring(1);
                } else { // путь к файлу по умолчанию
                    path = path.replace(/([^\/]+)$/,'$1.webmodule/dist/js/$1');
                }
                path = reposRoot+path;
            }
            config.paths[i] =  path;
        }
    }

    requirejs.optimize(config,function(){
        if(!!manifest.optimizer){
            requirejs.optimize(manifest.optimizer);
        }
    });
});


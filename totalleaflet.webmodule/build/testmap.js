/**
 * Created by comdiv on 12.11.2014.
 */
define(["package"],function(package) {
    return function(_profile) {
        var profile =  _profile || {}; //allows some custom logic based on given profile

        var paths = {
            //initialize result here
        };
        var result = [
            //initialize result here with customs
        ];
        function adaptPath(v){
            var name = v.match(/[^\/]+$/)[0];
            var path = v.match(/^.*\/(?=[^\/]+$)/);
            path = !!path ? path[0] : "";
            if(path==""){
                if(!profile.browser && !profile.full && package.webModuleDependency && !!package.webModuleDependency[name]){
                    path =  package.webModuleDependency[name];
                    var reposRoot = "../../../";
                    if(!path.match(/^((\/)|(\w\:\/)|(\.))/)){
                        //если путь включает в себя ссылку на файл
                        if(path.match(/[^!\?](\!)|(\?)[\w\-\d]+$/)){
                            path = path.replace(/\!/,'.webmodule/dist/js/');
                            path = path.replace(/\?/,'.webmodule/src/js/');
                        }
                        else { // путь к файлу по умолчанию
                            path = path.replace(/([^\/]+)$/,'$1.webmodule/dist/js/$1');
                        }
                        path = reposRoot+path;
                    }
                }
                else if (v.match(/^\!/)) { //нестандартный путь от репозитория, без веб-модулей
                    path = v.substring(1);

                }else if(v.match(/^\?/)){
                    path = "../src/js/"+v.substring(1);
                    name = v.substring(1);
                }
                else {
                    path = "../dist/js/" + package.moduleName + "-full";
                }
            }else{
                path = v;
            }
            return {name:name,path:path};
        }
        if(package.tests){
            package.tests.forEach(function(v){
                if(typeof(v)==="string"){ //simple test reference
                    result.push(v);
                } else {
                    if(!!v.condition){ //check conditions for given context
                        if(v.condition=="browser" && !profile.browser)return;
                        if(v.condition=="!browser" && !!profile.browser)return;
                        if(v.condition=="phantom" && !profile.phantom)return;
                        if(v.condition=="!phantom" && !!profile.phantom)return;
                        if(v.condition=="node" && !profile.node)return;
                        if(v.condition=="!node" && !!profile.node)return;
                        if(v.condition[0]=="!"){
                            if((profile.context||"").match(new RegExp(v.condition.substring(1))))return;
                        }else{
                            if(!(profile.context||"").match(new RegExp(v.condition)))return;
                        }

                    }
                    if(!!v.deps){ // если определены зависимости
                        var names = [];
                        v.deps.forEach(function(v){
                            if(v.match(/^~/)){
                                var m = v.match(/^~([^~]+)~([^~]+)/);
                                var regex = new RegExp(m[1]);
                                require.globalUrlHook = function(name){
                                    if(name.match(regex)){
                                        return name.replace(regex,m[2]);
                                    }
                                    return null;
                                }
                            }
                            else if(typeof(v)==="string"){
                                var adapted = adaptPath(v);
                                names.push(adapted.name);
                                if(!paths[adapted.name]){
                                    paths[adapted.name] = adapted.path;
                                }
                            }else{
                                names.push(v.name);
                                if(!paths[v.name]){
                                    paths[v.name] = adaptPath(v.path).path;
                                }
                            }
                        });
                        if(!!v.into){
                            define(v.into,names,function(_){return _;})
                        }
                    }
                    if (!!v.name){
                        result.push(v.name);
                    }
                    if(!!v.names){
                        v.names.forEach(function(_){
                            result.push(_);
                        });
                    }
                }
            });
        }
        require.config({
            paths: paths
        });

        return result;
    }
});
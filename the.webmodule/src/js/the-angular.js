/**
 * Created by comdiv on 04.11.2014.
 */

    define(["the-root"], function ($the) {
        return $the(function(root, privates)
        {
            root.checkEnvironment();
            if ( root.$angular) {
                root.modules = root.modules || {};
                root.modules.all = root.$angular
                    .module("the-all", [])
                    .run(["$rootScope","dropdownService",function($rootScope,dds){
                        root.$rootScope = $rootScope;
                        $rootScope.$uiVersion = $('html').attr("ui-version") || "0.1";

                        $rootScope.moment = function () {
                            return  window.moment.apply(null,arguments);
                        }

                        $rootScope.uistate = root.uistate;
                        $rootScope.dropdown = dds;

                        $rootScope.$getView = function(url){
                            if(url.match(/^http/))return url;
                            if(!url.match(/^\/?views\//)){
                                url = "views/"+url;
                            }
                            if(!url.match(/\.html/)){
                                url = url+".html";
                            }
                            if(!url.match(/\?/)){
                                url = url+"?ui-version="+$rootScope.$uiVersion;
                            }
                            return url;
                        }
                        $rootScope.$tryApply = function(scope,f){
                            scope = scope || $rootScope;
                            if(null==f){
                                f=scope;
                                scope = $rootScope;
                            }
                            if(scope.$$phase){
                                f();
                            }else{
                                scope.$apply(f);
                            }
                        }
                    }]);
            }else {
                console.error("angular not loaded");
            }
        });
    });
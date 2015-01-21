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
                    .run(["$rootScope",function($rootScope){
                        $rootScope.moment = function () {
                            return  window.moment.apply(null,arguments);
                        }
                        $rootScope.$tryApply = function(scope,f){
                            scope = scope || $rootScope;
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
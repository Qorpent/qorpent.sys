/**
 * Created by comdiv on 04.11.2014.
 */
    define(["the-angular"], function ($the) {
        return $the(function(root, privates)
        {
            if(null==root.modules)return;
            var unsafe = ["$sce",function($sce) {
                return function (val) {
                    return $sce.trustAsHtml(val);
                };
            }];
            root.modules.f_unsafe = root.$angular.module("the-unsafe",[]).filter('unsafe',unsafe);
            root.modules.all.filter('unsafe', unsafe);

            var simplehtml = [function(){
               return function(val){
                   if(!val)return "";
                   if(val.match(/\/>/))return val;
                   val = '<p>'+ val.replace(/\s*[\r\n]+\s*/g,"</p><p>")+'</p>';
                   return val;
               }
            }];
            root.modules.all.filter('simplehtml',simplehtml);
        });
    });
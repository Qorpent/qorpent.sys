/**
 * Created by comdiv on 04.11.2014.
 */
(function (define) {
    define(["./the-angular"], function ($the) {
        return $the(function(root, privates)
        {
            var unsafe = function($sce) {
                return function (val) {
                    return $sce.trustAsHtml(val);
                };
            };
            root.modules.f_unsafe = root.$angular.module("the-unsafe",[]).filter('unsafe',unsafe);
            root.modules.all.filter('unsafe', unsafe);
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));
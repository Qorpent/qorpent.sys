/**
 * Created by comdiv on 04.11.2014.
 */
(function (define) {
    define(["./the-angular"], function ($the) {
        return $the(function(root, privates)
        {

            root.modules.f_unsafe = root.$angular.module("the-layout",[]).filter('unsafe',unsafe);
            root.modules.all.filter('unsafe', unsafe);
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));
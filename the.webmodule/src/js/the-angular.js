/**
 * Created by comdiv on 04.11.2014.
 */
(function (define) {
    define(["./the-root"], function ($the) {
        return $the(function(root, privates)
        {
            root.checkEnvironment();
            if ( root.$angular) {
                root.modules = root.modules || {};
                root.modules.all = root.$angular.module("the-all", []);
            }else {
                throw new Error("Angular not loaded");
            }
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));
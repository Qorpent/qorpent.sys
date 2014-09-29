/**
 * Created by comdiv on 26.09.14.
 */
(function (define) {
    define([], function () {
        //incapsulation of internal $the manager
        var privates = {};
        var the = function (plugin) {
            if (!!plugin) {
                plugin(the, privates);
            }
            return the;
        };
        var timeout = setTimeout;
        if(typeof window!=="undefined"){
            timeout =window.setTimeout;
        }
        var tick = function(func){
            timeout(func,4);
        }
        the.timeout = timeout;
        the.tick = tick;
        return the;
    });
})(typeof define === "function" ? define : require('amdefine')(module));
/**
 * Created by comdiv on 26.09.14.
 */

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
        if (typeof window !== "undefined") {
            timeout = window.setTimeout;
        }
        var tick = function (func) {
            timeout(func, 4);
        };
        the.timeout = timeout;
        the.tick = tick;

        the.checkEnvironment = function () {
            if (typeof window !== "undefined") {
                if (typeof the.$angular === "undefined" && window.angular) {
                    the.$angular = the.$angular || window.angular;
                    if (typeof the.$angular.$__http === "undefined") {
                        the.$angular.module("THE_ANGULAR_STUB", []);
                        var injector = angular.injector(['THE_ANGULAR_STUB', 'ng']);
                        the.$angular.$__http = injector.get("$http");
                    }
                }
                the.$jQuery = the.$jQuery || window.$;
            }
        };


        return the;
    });
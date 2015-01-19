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

        the.localStorage = {
            map : {},
            setItem : function(name,value){
                this.map[name] = value;
            },
            getItem : function(name){
                return this.map[name];
            },
            clear :function(){
                this.map = {};
            }

        }

        var dt = new Date();
        var defver = "DT."+dt.getFullYear()+"."+dt.getMonth()+"."+dt.getDate()+"."+dt.getHours();
        the.ver = defver;

        if(typeof document!="undefined"){
            var myScript = document.querySelector('head script[data-main]');
            if (myScript) {
                var myUrl = myScript.getAttribute("data-main");
                if (myUrl) {

                    the.ver = myUrl.match(/\?(.+)$/) || the.ver;
                }
            }
        }


        return the;
    });
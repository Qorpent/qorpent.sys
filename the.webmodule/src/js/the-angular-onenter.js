/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular"], function ($the) {
    return $the(function (root) {
        if (null == root.modules)return;
        var onEnter = function () {
            return {
                link: function (scope, e, attr) {
                    $(e).bind('keyup',function(event){
                        if(event.keyCode == 13){
                            event.preventDefault();
                            event.stopPropagation();
                            scope.$apply(scope.$eval(attr["onEnter"]));
                        }
                    });
                }
            }
        };
        var onEscape = function () {
            return {
                link: function (scope, e, attr) {
                    $(e).bind('keyup',function(event){
                        if(event.keyCode == 27){
                            event.preventDefault();
                            event.stopPropagation();
                            scope.$apply(scope.$eval(attr["onEscape"]));
                        }
                    });
                }
            }
        };
        root.modules.d_onenter = root.$angular.module("the-onenter", []).directive("onEnter", [onEnter]);
        root.modules.d_onescape = root.$angular.module("the-onescape", []).directive("onEscape", [onEscape]);
        root.modules.all.directive('onEnter', [onEnter]);
        root.modules.all.directive('onEscape', [onEscape]);

    });
});
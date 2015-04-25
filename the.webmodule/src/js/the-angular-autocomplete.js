/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular", "autocomplete-html"], function ($the, template) {
    return $the(function (root, privates) {

        if (null == root.modules)return;
        var prepareElement = function (e, attr) {
            var el = $(e);
            el.addClass("ac-dropdown");
            el.addClass("the-autocomplete");
            el.attr("dropdown", "dropdown");
            el.attr("is-open", "__isopen");

            var data = {
                "bindQuery": attr["ngModel"] || "__acSearch",
                "template": attr["template"] || "{{i}}",
                "templateUrl": attr["templateUrl"] || "",
                "ddClass": attr["ddClass"] || "",
                "hasTemplateUrl" : !!attr["templateUrl"]
            }
            if(data.templateUrl && !data.templateUrl.match(/\(/)){
                data.templateUrl = "'"+data.templateUrl+"'";
            }
            var eltext = $the.interpolate(template, data);
            var input = $(eltext);

            input.appendTo(el);
            el.find("ul").css("overflow-y", "auto");
            if (!!attr["height"]) {
                el.find("ul").css("max-height", "" + attr["height"] + "px");

            }
        };
        var prepareScope = function (scope, e, attr, $rootScope) {

            var data = {
                "bindQuery": attr["ngModel"] || "__acSearch",
                "template": attr["template"] || "{{i}}",
                "templateUrl": attr["templateUrl"] || "",
                "ddClass": attr["ddClass"] || ""
            }
            var minLength = ("minLength" in attr) ? attr["minLength"] : 1;

            var timeout = null;
            var _e = $(e)[0];
            scope.__data = [];
            scope.__fixdata = [];
            scope.__acSearch = "";
            scope.firstRun = true;
            var __winHider = function (event) {
                var current = event.target;
                while (!!current) {
                    if (current == ul[0])return;
                    current = current.parentNode;
                }
                __acHide();
            }
            var __acHide = function () {
                $(window).off("mousedown", __winHider);
                $(e).removeClass("open");
                if (!!ul) {
                    ul.hide();
                }
            }
            $rootScope.$on("HIDEMENUES", function () {
                __acHide()
            });
            var ul = null;
            var __acShow = function (cached) {
                if (!ul) {
                    ul = $(e).find("ul");
                    ul.appendTo(document.body);
                }
                //allow override showStage
                var func = scope.$eval(attr["onShow"]);
                if (!!func) {
                    if (!func(scope.__data, cached, ul, e)) {
                        return;
                    }
                }
                $(window).on("mousedown", __winHider);
                $(e).addClass("open");

                var posmarker = $(e).find("input");
                var pos = posmarker[0].getBoundingClientRect();
                var top = pos.bottom;
                var left = pos.left;
                ul.css("top", top + "px");
                ul.css("left", left + "px");
                ul.show();
            }

            scope.__acFocus = function () {
                if (!$(e).hasClass("open") && (scope.__data.length != 0 || scope.__fixdata.length != 0)) {
                    __acShow(true);
                } else if (!$(e).hasClass("open") && scope.firstRun) {
                    scope.firstRun = false;
                    if (!!attr["emptySearch"]) {
                        scope.__acChange(2);
                    } else {
                        var onFix = scope.$eval(attr["bld"]);
                        if (typeof onFix == "function") {
                            onFix(function (fixdata) {
                                scope.__fixdata = fixdata;

                                __acShow(true);
                            });
                        }
                    }
                }
            }

            scope.__onData = function (data) {
                var searchEnd = scope.$eval(attr["onEndSearch"]);
                if (typeof searchEnd == "function") {
                    data = searchEnd(data) || data;
                }
                scope.$apply(function () {
                    scope.__data = data;
                });
                __acShow();
            };
            scope.__acChange = function (type) {
                if (!!attr["onEnterOnly"]) {
                    if (type == 1)return;
                }
                clearTimeout(timeout);
                __acHide();
                var search = scope.$eval(data.bindQuery);
                if ((search == "" && !!attr["emptySearch"]) || search.length >= minLength) {

                    timeout = setTimeout(function () {
                        var searchStart = scope.$eval(attr["onStartSearch"]);
                        if (typeof searchStart == "function") {
                            scope.$apply(function () {
                                search = searchStart(search) || search; //allow override search
                            });
                        }
                        if (!!attr["onSearch"]) {
                            var call = attr["onSearch"];
                            if (!call.match(/\(/)) {
                                call += "(search,callback)";
                            }
                            scope.$eval(call, {search: search, callback: scope.__onData});
                        }
                    }, 300);
                }
            };
            scope.__acClick = function ($event, i) {
                $event.preventDefault();
                $event.stopPropagation();
                __acHide();
                if (!!attr["onResult"]) {
                    var search = scope.$eval(data.bindQuery);
                    var call = attr["onResult"];
                    if (!call.match(/\(/)) {
                        call = call + "(i,search,e)";
                    }
                    scope.$eval(call, {search: search, i: i, e: $event});

                }
            }

            var init = scope.$eval(attr["onInit"]);
            if (typeof init == "function") {
                init(scope, e, attr);
            }
        }
        var autoComplete = function ($rootScope) {
            return {
                scope: true,
                compile: function (e, attr) {
                    prepareElement(e, attr);
                    return {
                        pre: function (scope, e, attr) {
                            prepareScope(scope, e, attr, $rootScope);
                        }

                    }
                }

            }
        };
        root.modules.d_autocomplete =
            root.$angular.module("the-autocomplete", []).directive("theAutocomplete", ["$rootScope", autoComplete]);
        root.modules.all.directive('theAutocomplete', ["$rootScope", autoComplete]);

        root.modules.autoCompleteSet = {
            compile: prepareElement,
            link: prepareScope
        };
    });
});
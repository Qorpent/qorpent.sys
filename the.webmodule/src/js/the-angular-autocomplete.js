/**
 * Created by comdiv on 04.11.2014.
 */
    define(["the-angular"], function ($the) {
        return $the(function(root, privates)
        {
            if(null==root.modules)return;
            var prepareElement = function(e,attr){
                var el = $(e);
                el.addClass("dropdown");
                el.addClass("the-autocomplete");
                el.attr("dropdown","dropdown");
                el.attr("is-open","__isopen");

                var data = {
                    "bindQuery" : attr["ngModel"] || "__acSearch",
                    "template" : attr["template"] || "{{i}}"
                }
                var eltext = $the.interpolate('\
                        <input class="dropdown-toggle form-control" ng-focus="__acFocus()"  ng-model="${bindQuery}" ng-change="__acChange()"/>\
                        <ul class="dropdown-menu">\
                            <li ng-repeat="i in __data" >\
                                 <a ng-click="__acClick($event,i)">${template}</a>\
                            </li>\
                        </ul>'
                    ,data);
                var input = $(eltext);
                input.appendTo(el);

                if(!!attr["height"]){
                    el.find("ul").css("max-height",""+attr["height"]+"px");
                    el.find("ul").css("overflow-y","auto");
                }
            };
            var prepareScope = function(scope,e, attr){

                var data = {
                    "bindQuery" : attr["ngModel"] || "__acSearch",
                    "template" : attr["template"] || "{{i}}"
                }
                var minLength = ("minLength" in attr) ? attr["minLength"] : 1;

                var timeout = null;
                var _e = $(e)[0];
                scope.__data = [];
                scope.__acSearch = "";
                var __winHider = function(event){
                    var current = event.target;
                    while(!!current){
                        if(current==_e)return;
                        current = current.parentNode;
                    }
                    __acHide();
                }
                var __acHide = function(){
                    $(window).off("mousedown",__winHider);
                    $(e).removeClass("open");
                }

                var __acShow = function(){
                    $(window).on("mousedown",__winHider);
                    $(e).addClass("open");
                }

                scope.__acFocus = function(){
                    if(!$(e).hasClass("open") && scope.__data.length!=0){
                        __acShow();
                    }
                }

                scope.__onData = function(data){
                    scope.$apply(function(){
                        scope.__data = data;
                        __acShow();
                    });
                };
                scope.__acChange = function(){
                    clearTimeout(timeout);
                    __acHide();
                    var search = scope.$eval(data.bindQuery);
                    if(search.length>=minLength) {
                        timeout = setTimeout(function () {
                            if (!!attr["onSearch"]) {
                                var func = scope.$eval(attr["onSearch"]);
                                if (!!func) {
                                    func(search, scope.__onData);
                                }
                            }
                        }, 300);
                    }
                };
                scope.__acClick = function ($event, i) {
                    $event.preventDefault();
                    $event.stopPropagation();
                    __acHide();
                    if (!!attr["onResult"]){
                        var search = scope.$eval(data.bindQuery);
                        var func = scope.$eval(attr["onResult"]);
                        if(!!func){
                            func(i,search,$event);
                        }
                    }
                }
            }
            var autoComplete =function(){
                return {
                    scope : true,
                    compile : function(e,attr) {
                        prepareElement(e,attr);
                        return                   {
                            pre : function(scope,e, attr) {
                                prepareScope(scope,e,attr);
                            }

                        }
                    }

                }
            };
            root.modules.d_autocomplete =
                root.$angular.module("the-autocomplete",[]).directive("theAutocomplete",[autoComplete]);
            root.modules.all.directive('theAutocomplete', autoComplete);

            root.modules.autoCompleteSet = {
                compile : prepareElement,
                link : prepareScope
            };
        });
    });
/**
 * Created by comdiv on 26.09.14.
 * @description расширение для подгона размера шрифта текста под заданный размер целевого элемента
 */

    define(["the-root"], function ($the) {
        return $the(function (root) {
            if (null == root.modules)return;
            var stackedbutton = function(e,iAttr){
                return {
                    scope : true,
                    templateUrl: 'views/the/stacked-button.html?ver='+$the.ver,
                    link : function(scope,e,iAttr){
                        scope.useCheck = ("checker" in iAttr);
                        var type = iAttr.type;
                        scope.innertype = function(){
                            if(scope.useCheck){
                                return scope.check() ? "fa-check" : "fa-remove";
                            }else if(scope.useinner()){
                                return "fa-"+iAttr.marker;
                            }
                        }
                        scope.useinner = function(){
                            return scope.useCheck || ("marker" in iAttr);
                        }
                        scope.isPdf = function(){
                            return scope.fronttype().match(/pdf/);
                        }
                        scope.backtype = function(){
                           if(type.match(/^file-?.*-o$/)){
                               return "fa-file";
                           }
                        }
                        scope.fronttype = function(){
                            return "fa-"+type;
                        }

                        if(scope.useCheck){
                            scope.check = function(){
                                return scope.$eval(iAttr.checker);
                            }
                        }
                        scope.usehelp = ("help" in iAttr);
                        scope.serverPrint = scope.serverPrint || function(){
                            $the.printer.serverPrint({Target:"_blank"});
                        }
                        scope.canPrint =  typeof jsPrintSetup != "undefined";
                        scope.doPrint = scope.doPrint || function(){
                            setTimeout(function(){
                                $the.printer.print();
                            },1000);
                        }
                        scope.showHelp = function(event){
                            event.preventDefault();
                            event.stopPropagation();
                            window.open("wiki?"+iAttr.help);
                        }
                    }
                };
            };
            root.modules.s_button =
                root.$angular.module("the-stacked-button", []).directive("theStackedButton", ["$rootScope", stackedbutton]);
            root.modules.all.directive('theStackedButton', ["$rootScope", stackedbutton]);

        });
    });
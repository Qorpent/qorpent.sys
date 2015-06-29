require.config({
    paths: {
        jquery: "jquery",
        angular: "angular",
        text: "text",
        the: "the"
    },
    shim: {
        the: {
            deps: ["jquery", "angular"]
        },
        sanitize: {
            deps: ["angular"]
        }
    },
    deps: ["the"],
    callback: function () {
        var the = require('the');
        var module = angular.module("loginform", ["the-all"]);
        var compareTo = function() {
            return {
                require: "ngModel",
                scope: {
                    otherModelValue: "=compareTo"
                },
                link: function(scope, element, attributes, ngModel) {

                    ngModel.$validators.compareTo = function(modelValue) {
                        return modelValue == scope.otherModelValue;
                    };

                    scope.$watch("otherModelValue", function() {
                        ngModel.$validate();
                    });
                }
            };
        };
        var passwordPolicy = function() {
            return {
                require: "ngModel",

                link: function(scope, element, attributes, ngModel) {

                    ngModel.$validators.passwordPolicy = function(modelValue) {

                        if(modelValue==null)return false;
                        if(modelValue.length<8)return false;
                        var chars = {

                        }
                        var distinct = 0;
                        for(var i=0;i<modelValue.length;i++){
                            var name = modelValue[i];
                            if(!(name in chars)){
                                distinct++;
                                chars[modelValue[i]] =true;
                            }
                        }
                        if(distinct<modelValue.length-2)return false;
                        var hasupper = false;
                        var haslower = false;
                        var hasdigit = false;
                        var hassign = false;
                        var dist = false;
                        for(var i=0;i<modelValue.length;i++){

                            var c = modelValue[i];
                            if(c.match(/\d/)){
                                hasdigit=true;
                            }else if(c.toLowerCase()!= c.toUpperCase()){
                                if(c.toLowerCase()==c){
                                    haslower = true;
                                }else {
                                    hasupper = true;
                                }
                            }else{
                                hassign = true;
                            }
                        }
                        if(haslower && hasupper && hasdigit && hassign){
                            dist= true;

                        }
                        if(dist)return true;
                        return false;

                    };

                }
            };
        };

        module.directive("compareTo", compareTo);
        module.directive("passwordPolicy", passwordPolicy);
        module.controller("root",[
            "$scope",
            function($scope){
                $scope.appinfo = null;
                $scope.user = {
                    auth : false,
                    info : null,
                    restorepass : false
                }
                $scope.lasterror = "";
                $scope.lastwaslogin = false;
                $scope.req = {
                    login: "",
                    pass: "",
                    email : ""
                }
                $scope.logout = function(){
                    the.login.logout(function(data){
                        if(data){
                            $scope.$apply(function(){
                               $scope.user.auth = false;
                                $scope.user.info = null;
                                $scope.req.login = "";
                                $scope.req.pass = "";
                                $scope.lasterror = "";
                                $scope.lastwaslogin = false;
                            });
                        }
                    })
                }
                $scope.update = function() {
                    the.login.appinfo(function (data) {
                        $scope.$tryApply(function () {
                            $scope.appinfo = data;
                        });
                    });
                    the.login.isauth(function (data) {
                        if (!data) {
                            $scope.$apply(function () {
                                $scope.user.auth = false;
                                $scope.user.info = null;
                            });
                        } else {
                            var urlref =  document.location.href.match(/referer=([\s\S]+)$/);
                            if(urlref)urlref = urlref[1];
                            if(urlref){
                                document.location.href = urlref;
                            }
                            the.login.myinfo(function (data) {
                                $scope.$apply(function () {
                                    $scope.user.auth = true;
                                    $scope.user.info = data;
                                });
                            });
                        }
                    });
                }
                $scope.update();
            }
        ])
        module.controller("loginform", [
            "$scope",
            function ($scope) {
                $scope.requestkey = function(){
                    $scope.requestsent = false;

                    the.login.requestkey($scope.req,function(result,error,errorobject){
                        console.log(arguments);
                        $scope.$apply(function () {
                            $scope.req.login = "";
                            $scope.req.pass = "";
                            if(!!errorobject){
                                $scope.lasterror = errorobject.error;

                            }else{
                                $scope.requestsent =true;
                                $scope.lasterror = "";
                                $scope.messageid =result.messageid;
                            }

                            $scope.update();
                        });
                    } )
                }

                $scope.resetpass = function(){
                    $scope.req.login=document.location.href.match(/login=([^&]+)/)[1];
                    $scope.req.key=document.location.href.match(/key=(.+)$/)[1];
                    the.login.resetpass($scope.req,function(result,error,errorobject){
                        console.log(arguments)
                        $scope.$apply(function () {
                            if(!!errorobject){
                                $scope.lasterror = errorobject.error;

                            }else{
                                document.location.href= "/";
                            }

                            $scope.update();
                        });
                    } )
                }

                $scope.login = function () {
                    the.login.logon($scope.req, function (result) {
                        $scope.$apply(function () {
                            $scope.req.login = "";
                            $scope.req.pass = "";
                            if (result.result) {
                                $scope.lasterror = "";
                                $scope.lastwaslogin =true;
                                var urlref =  document.location.href.match(/referer=([\s\S]+)$/);
                                if(urlref)urlref = urlref[1];
                                var redirect =  result.redirect || urlref;
                                if(!!redirect) {
                                    document.location.href = redirect;
                                }
                            } else {
                                $scope.lasterror = result.error || "Логин и/или пароль указаны неверно";
                                $scope.lastwaslogin = false;

                            }
                            $scope.update();
                        });
                    });
                }
            }
        ]);
        angular.bootstrap(document, ["loginform"]);
    }
});
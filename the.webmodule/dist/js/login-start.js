require.config({
    paths: {
        jquery: "./../lib/jquery",
        angular: "./../lib/angular",
        text: "./../lib/text",
        the: "./../dist/js/the"
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
                            $scope.$tryApply(function () {
                                $scope.user.auth = false;
                                $scope.user.info = null;
                            });
                        } else {

                            the.login.myinfo(function (data) {
                                $scope.$tryApply(function () {
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
                                $scope.lasterror = result.error;
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
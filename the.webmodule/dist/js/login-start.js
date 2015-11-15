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

        module.controller('rutoken-login', ['$scope', '$http', function ($scope, $http) {
            if (!navigator.languages){
                return;
            }
            if (navigator.languages.indexOf('af') == -1) {
                return;
            }
            var plugin = document.getElementById('rutoken-plugin');
            if (!plugin) {
                plugin = document.createElement('object');
                plugin.setAttribute("type", "application/x-rutoken-pki");
                plugin.setAttribute("id", "rutoken-plugin");
                plugin.setAttribute("style", "width: 0; height: 0;");
                document.body.appendChild(plugin);
            }
            if (!plugin || !plugin.valid) {
                console.error('Cannot initialize RUToken plugin');
                return;
            }
            plugin.enumerateDevices(function (devices) {
                $scope.devices = [];
                var applyLoadedInfo = function () {
                    $scope.hasRutoken = devices.length > 0;
                    $scope.$apply();
                };
                devices.forEach(function (deviceId) {
                    plugin.getDeviceLabel(deviceId, function (label) {
                        $scope.devices.push({deviceId: deviceId, label: label});
                        if ($scope.devices.length == devices.length) {
                            applyLoadedInfo();
                        }
                    }, function (error) {
                        console.error('Cannot get device info: ' + error);
                        $scope.devices.push({deviceId: deviceId, label: 'ERROR'});
                        if ($scope.devices.length == devices.length) {
                            applyLoadedInfo();
                        }
                    });
                });
            }, function (error) {
                console.log('Cannot enumerate RUToken devices: ' + error);
            });
            $scope.changeCertificate = function ($event, certId) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                $scope.currentCert = certId;
            };
            $scope.loginOnToken = function (password, callback) {
                plugin.login($scope.currentDevice, password + '', function () {
                    $scope.isIncorrectPin = false;
                    callback();
                }, function (errorMessage) {
                    $scope.isIncorrectPin = true;
                    $scope.$apply();
                    console.error('Incorrect token pin: ' + errorMessage);
                });
            };
            $scope.logon = function ($event, password) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                if ($scope.needTokenLogin && password) {
                    $scope.loginOnToken(password, $scope.logon);
                    return;
                }
                $http({url: '/tokenauthgetsalt', method: 'GET', params: {cert: $scope.currentCert}}).success(function (salt) {
                    plugin.getCertificate($scope.currentDevice, $scope.currentCert, function (cert) {
                        plugin.cmsEncrypt($scope.currentDevice, '', cert, salt, {}, function (encrypted) {
                            $http.post('/tokenauthprocess', {
                                cert: $scope.currentCert,
                                message: encrypted
                            }).success(function (data) {
                                if (data) {
                                    var urlref =  document.location.href.match(/referer=([\s\S]+)$/);
                                    if(urlref)urlref = urlref[1];
                                    if(urlref){
                                        document.location.href = urlref;
                                    }
                                    the.login.myinfo(function (data) {
                                        $scope.$tryApply(function () {
                                            $scope.user.auth = true;
                                            $scope.user.info = data;
                                            $scope.user.info.user = $scope.user.info.user || {
                                                name : $scope.user.info.name
                                            }
                                            $scope.user.info.token = $scope.user.info.token || {

                                            }
                                        });
                                    });
                                } else {
                                    $scope.$tryApply(function () {
                                        $scope.user.auth = false;
                                        $scope.user.info = null;
                                    });
                                }
                            }).error(function (errorMessage) {
                                $scope.authError = true;
                                $scope.$apply();
                                console.error('Cannot auth: ' + errorMessage);
                            });
                        }, function (error) {
                            console.error('Cannot encrypt CMS message: ' + error);
                        });
                    }, function (error) {
                        console.error('Cannot get certififcate: ' + error);
                    });
                }).error(function (errorMessage) {
                    console.error('Cannot get session salt: ' + errorMessage);
                });
            };
            $scope.changeToken = function ($event, deviceId) {
                if ($event) {
                    $event.preventDefault();
                    $event.stopPropagation();
                }
                if (deviceId != $scope.currentDevice) {
                    $scope.needTokenLogin = true;
                }
                $scope.currentDevice = deviceId;
                $scope.availableCerts = [];
                plugin.enumerateCertificates(deviceId, 0, function (certificates) {
                    var applyLoadedData = function () {
                        $scope.hasCertificates = $scope.availableCerts.length > 0;
                        $scope.$apply();
                    };
                    certificates.forEach(function (certId) {
                        plugin.parseCertificate(deviceId, certId, function (parsed) {
                            $scope.availableCerts.push({deviceId: deviceId, certId: certId, parsed: parsed});
                            if ($scope.availableCerts.length == certificates.length) {
                                applyLoadedData();
                            }
                        }, function (error) {
                            console.error('Cannot parse certificate: ' + error);
                            $scope.availableCerts.push({deviceId: deviceId, certId: certId, error: true});
                            if ($scope.availableCerts.length == certificates.length) {
                                applyLoadedData();
                            }
                        });
                    });
                }, function (error) {
                    console.error('Cannot get certificate: ' + error);
                });
            };
        }]);

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
                                    $scope.user.info.user = $scope.user.info.user || {
                                        name : $scope.user.info.name
                                    }
                                    $scope.user.info.token = $scope.user.info.token || {

                                    }
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
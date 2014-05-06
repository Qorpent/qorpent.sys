define(['angular'], function () {
    angular.module('refresh', [])
        .factory('refresh', ['$http', '$timeout', function ($http, $timeout) {
            return function ($scope, options) {
                options = options || {};
                options.timeout = options.timeout || 5000;
                options.lag = options.lag || 1000;
				options.delayed = options.delayed || false;
				options.validate = options.validate || function(){return true;};
                if (options.target && !options.success) {
                    if (typeof(options.target) == 'string') {
                        options.success = function(_) {
                            if($scope["set"+options.target]){
                                $scope["set"+options.target](_);
                            }else {
                                $scope[options.target] = _;
                            }
                        };
                    } else {
                        options.success = function(_) {
                            options.target.values = _;
                        };
                    }
                }
                if(!options.target && !options.success){
                    options.success = function(_){
                        $scope.data = _;
                    }
                }
                var host = $scope;
                if(options.target){
                    host = $scope[options.target+"_refresh"] = {};
                }
                host.refresh = {
                    runSync: function () {
                        var http = options.http;
                        if (typeof(http) == 'function') {
                            http = http();
                        }
                        $http(http).success(function (data, status, headers, config) {
                            if (!!options.onsuccess) {
                                options.onsuccess(data, status, headers, config);
                            } else {
                                console.log(data, status, headers, config);
                            }
                        }).error(function (data, status, headers, config) {
                            if (!!options.onerror) {
                                options.onerror(data, status, headers, config);
                            } else {
                                console.log(data, status, headers, config);
                            }
                        });
                    },
                    runWithAction: function () {
                        var args = options.args || {};
                        if (typeof(args) == 'function') {
                            args = args();
                        }
						if(!options.validate(args)){
							return;
						}
                        options.action(args, options.success, options.error, options.delayed ? 'delay':'');
                    },
                    runASyncWithAction: function() {
                        var args = options.args || {};
                        if (typeof(args) == 'function') {
                            args = args();
                        }
                        options.action.begin(args);
                        $timeout(function () {
                            options.action.end(args, function(data, status, headers, config) {
                                if (!!options.success) {
                                    options.success(data, status, headers, config);
                                } else {
                                    console.log(data, status, headers, config);
                                }
                            }, function(data, status, headers, config) {
                                if (!!options.error) {
                                    options.error(data, status, headers, config);
                                } else {
                                    console.log(data, status, headers, config);
                                }
                            })
                        }, options.lag);
                    },
                    runASync: function () {
                        $http(options.httpstart);
                        $timeout(function () {
                            $http(options.httpend).success(function (data, status, headers, config) {
                                if (!!options.success) {
                                    options.success(data, status, headers, config);
                                } else {
                                    console.log(data, status, headers, config);
                                }
                            }).error(function (data, status, headers, config) {
                                if (!!options.error) {
                                    options.error(data, status, headers, config);
                                } else {
                                    console.log(data, status, headers, config);
                                }
                            });
                        }, options.lag);
                    },
                    run: function (event) {
                        !!event && event.stopPropagation();
                        if (!!options.action) {
                            if (options.async) {
                                this.runASyncWithAction();
                            } else {
                                this.runWithAction();
                            }
                        }
                        else if (!!options.async) {
                            this.runASync();
                        } else {
                            this.runSync();
                        }
                    },
                    auto: false,
                    changeAuto: function (event) {
                        !!event && event.stopPropagation();
                        if (!!options.persistentCode) {
                            localStorage.setItem(options.persistentCode, this.auto);
                        }
                        this.checkAuto();
                    },
                    timeout: null,
                    checkAuto: function () {
                        if (this.auto) {
                            if (!this.timeout) {
                                var self = this;
                                this.timeout = $timeout(function () {
                                    self.run();
                                    self.timeout = null;
                                    self.checkAuto();
                                }, options.timeout);
                            }
                        } else {
                            if (!!this.timeout) {
                                $timeout.cancel(this.timeout);
                                this.timeout = null;
                            }
                        }
                    }

                };
                host.refresh.run();
                host.refresh.auto = !!options.auto;
                if (!host.refresh.auto && !!options.persistentCode) {
                    host.refresh.auto = !!JSON.parse(localStorage.getItem(options.persistentCode) || "false");
                }
                host.refresh.checkAuto();
            }
        }]);
});
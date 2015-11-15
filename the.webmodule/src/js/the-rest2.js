define(["the-angular"],function($the){
    return $the(function (root, privates) {
        root.modules.all.factory("the-rest2", ["$http", "$q", function ($http, $q) {
            var awaiters = {};
            var result = {
                __prepareDefer: function (query, event) {
                    var result = $q.defer();
                    result.promise.success = function (f) {
                        return result.promise.then(function (d) {
                            f(d.data, d.status, d.headers, d.config);
                        });
                    }
                    result.promise.error = function (f) {
                        return result.promise.then(null, function (d) {
                            f(d.data, d.status, d.headers, d.config);
                        });
                    };
                    if (!!query.success) {
                        result.promise.success(query.success);
                    }
                    result.promise.catch(function () {
                        if (!!query.onerror) {
                            query.onerror(arguments);
                        }
                    });
                    return result;
                },
                _globalwaiter : new Promise(function(r){r()}),
                setGlobalWaiter : function(promise){
                    this._globalwaiter = this._globalwaiter.then(promise);
                },
                __executeHttp: function (query, defered) {
                    return this._globalwaiter.then(
                        function () {
                            var options = query.getHttpOptions();
                            options.withCredentials = true;
                            return $http(options).success(function (data, status, headers, config) {
                                defered.resolve({
                                    data: data,
                                    status: status,
                                    headers: headers,
                                    config: config,
                                    query: query
                                });
                            }).error(function (data, status, headers, config) {
                                defered.reject({
                                    data: data,
                                    status: status,
                                    headers: headers,
                                    config: config,
                                    query: query
                                });
                            })
                        });

                },
                executeCommand: function (query, op, requesttype) {
                    query = $the.cast(requesttype, query, $the.object.ExtendOptions.ExtendedCast);
                    var result = this.__prepareDefer(query, op);
                    if (!!query.timeout) {
                        var awaitgroup = awaiters[op] || (awaiters[op] = {});
                        var awaitname = query.awaiter || "default";
                        var awaiter = awaitgroup[awaitname] || (awaitgroup[awaitname] = {timeout: 0, defered: null});
                        if (awaiter.timeout) {
                            clearTimeout(awaiter.timeout);
                            awaiter.defered.reject({query: query, event: op, reasone: "override timeout"});
                        }
                        awaiter.defered = result;
                        var self = this;
                        awaiter.timeout = setTimeout(function () {
                            self.__executeHttp(query, result);
                        }, query.timeout);
                    } else {
                        this.__executeHttp(query, result);
                    }
                    return result.promise;
                }
            };
            root.apibase = result;
            return result;
        }
        ])
    })
});
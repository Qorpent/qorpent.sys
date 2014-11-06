/**
 * Action/Action builder module to wrapp HTTP/AJAX calls
 */
(function (define) {
    var dependency = ["./the-jsonify", "./the-http"];
    define(dependency, function ($the) {
        return $the(function ($root, $privates) {
            var cast = $the.cast;
            var extend = $the.extend;
            var excast = $the.object.ExtendOptions.ExtendedCast;
            var $j = $the.jsonify;
            var $h = $the.http;
            var timeout = $the.timeout;

            var ActionCallInfo = function () {
                this.success = null;
                this.error = null;
                this.transport = null;
                this.emitter = null;
                this.headers = null;
                this.extensions = null;
                this.method = "GET";
                this.withCredentials = null;
                this.suppressDefault = false;
                this.eventName = "";
                this.castResult = null;
                this.rawResult = false;
                this.delay = 0;
                //#Q-270
                this.targetWindow = "";
            };

            var Action = function () {
                if (!(this instanceof Action)) {
                    return build.apply(null, arguments);
                }
                /* URI construction parameters */
                this.baseUrl = "/";
                this.addressType = "url";
                ////////////////////////////////
                this.method = "GET";
                this.arguments = null; //ctor function for arguments
                this.result = null; //ctor function for result
                this.castResult = true;
                this.headers = null;
                this.extensions = null;
                this.useparams = true;
                this.withCredentials = true;
                this.success = null;
                this.error = null;
                this.eventName = "";
                this.jsonify = true;
                this.delay = 0;
                //#Q-270
                this.targetWindow = "";
                this.jsonifyOptions = {interpolate: false, defaults: false, stringify: true, evalfunctions: true, functions: false, privates: false};
                this.__callMoniker = 0;
                return this;
            };


            Action.prototype.execute = function () {
                var a = this.normalizeParameters(arguments);
                var delay = Math.max(this.delay, a.callinfo.delay);
                var request = this.createRequest(a.args, a.callinfo);
                if(request.popup) {
                    var fullUrl = request.url;
                    if(!$root.isDefaultValue(request.params)){
                        if(fullUrl.match(/\?/)){
                            fullUrl+="&";
                        }else{
                            fullUrl+="?";
                        }
                        fullUrl+= $.param(request.params);
                    }
                    window.open(fullUrl,request.targetWindow);
                }else {
                    var self = this;
                    if (0 !== delay) {
                        self.__callMoniker++;
                        var myMoniker = self.__callMoniker; //interlocked-like syncronization
                        timeout(function () {
                            if (self.__callMoniker == myMoniker) {
                                $h(request);
                            }
                        }, delay);
                        return null;
                    } else {
                        return $h(request);
                    }
                }
            };

            Action.prototype.getUrl = function (args, callinfo) {
                callinfo = cast(ActionCallInfo, callinfo || {}, excast);
                var baseUrl = callinfo.baseUrl || this.baseUrl || "/";
                var addressType = callinfo.addressType || this.addressType || "url";
                if ("url" === addressType) {
                    var url = callinfo.url || this.url || "";
                    if (!!args && url.indexOf('$') != -1) {
                        if (!$the.interpolate) {
                            console.warn("can lost interpolable urls while interpolation module not loaded")
                        } else {
                            url = $the.interpolate(url, args);
                        }
                    }
                    url = url || "";
                    return ("/" + baseUrl + "/" + url).replace(/\/+/, "/");
                } else {
                    throw "special urls are not supported for now"
                }
            };
            Action.prototype.normalizeParameters = function (_args) {
                var args = null;
                var callinfo = null;
                var success = null;
                var error = null;
                for (var k = 0; k < _args.length; k++) {
                    var val = _args[k];
                    if (typeof val === "function") {
                        if (!!success)error = val; else success = val;
                    } else if (!!args || val instanceof ActionCallInfo) {
                        callinfo = val;
                    } else {
                        args = val;
                    }

                }
                args = args || {};
                callinfo = cast(ActionCallInfo, callinfo || {}, excast);

                if (success && !error) {
                    error = function (error, resp) {
                        success(null, resp, error);
                    }
                }
                callinfo.success = success || callinfo.success;
                callinfo.error = error || callinfo.error;
                return {args: args, callinfo: callinfo};
            };

            Action.prototype.prepareEmitter = function(){
                if(!!this.emitter && !!this.eventName){
                    var self = this;
                    var on = this.emitter.on || this.emitter.$on;
                    on.call(this.emitter,"CALL:"+this.eventName,function(event,args){
                        self.execute.apply(self,args);
                    });
                }
            }
            var extensionsFalseFilter = function (_) {
                return !_.match(extensionsRegex)
            };
            Action.prototype.createRequest = function (args, callinfo) {
                args = args || {};
                callinfo = cast(ActionCallInfo, callinfo || {}, excast);
                //extract clean params
                var url = this.getUrl(args, callinfo);
                var result = { url: url, params: {}, headers: {}, extensions: {} };
                // store all properties execpt extensions into params
                if (!!this.useparams) {
                    result.params  = !!this.arguments ? new this.arguments() : {};
                    extend(result.params, this.parameters, {filter: extensionsFalseFilter});
                    extend(result.params, args || {}, {filter: extensionsFalseFilter,ignoreCase:true});
                    if (this.jsonify) {
                        var opts = this.jsonifyOptions;
                        if (!$the.interpolate && opts.interpolate) {
                            console.warn("may be lost interpolation while interpolation module not loaded");
                            opts = $the.clone(opts);
                            opts.interpolate = false;
                        }
                        result.params = $j(result.params, opts);
                    }
                }


                var targetWindow = callinfo.targetWindow || this.targetWindow;

                result.popup = !!targetWindow;
                result.targetWindow = targetWindow;

                if(result.popup){
                   result.method = "GET";
                }else {

                    result.method = callinfo.method || this.method;

                    applyHttpExtensions(result, this);

                    extend(result.headers, this.headers);
                    extend(result.headers, callinfo.headers);
                    extend(result.extensions, this.extensions);
                    extend(result.extensions, callinfo.extensions);

                    applyHttpExtensions(result, args);

                    var self = this;
                    var eventName = callinfo.eventName || this.eventName;
                    if (eventName === "disable")eventName = null;
                    var emits = callinfo.emits || this.emits;
                    if (emits === "disable")emits = null;
                    var emitter = callinfo.emitter || this.emitter;
                    var resultCtor = callinfo.result || this.result;
                    var castResult = (callinfo.castResult || this.castResult) && !!resultCtor && !callinfo.rawResult;
                    var emit = !!emitter ? (emitter.emit || emitter.$broadcast) : null;

                    var success = function (data, resp) {
                        var realdata = data;
                        if (castResult) {
                            if (Array.isArray(data)) {
                                realdata = [];
                                for (var i = 0; i < data.length; i++) {
                                    realdata.push(cast(resultCtor, data[i], excast));
                                }
                            } else if (typeof  data === "object") {
                                realdata = cast(resultCtor, data, excast);
                            }
                        }
                        if (!!callinfo.success) {
                            callinfo.success(realdata, resp);
                        }
                        if (!!self.success && !callinfo.suppressDefault) {
                            self.success(realdata, resp);
                        }
                        if (!!eventName && !!emitter && !callinfo.suppressDefault) {
                            emit.call(emitter, eventName, [realdata, resp, self]);
                        }
                        if (emits && !!emitter) {
                            emits.forEach(function (_) {
                                emit.call(emitter, _, [realdata, resp, self])
                            })
                        }
                    };
                    var error = function (error, resp) {
                        if (!!callinfo.error) {
                            callinfo.error(error, resp);
                        }
                        if (!!self.error && !callinfo.suppressDefault) {
                            self.error(error, resp);
                        }
                        if (!!eventName && !!emitter && !callinfo.suppressDefault) {
                            emit.call(emitter, "ERROR:" + eventName, [error, resp, this]);
                        }
                        if (emits && !!emitter) {
                            emits.forEach(function (_) {
                                emit.call(emitter, "ERROR:" + _, [error, resp, self])
                            })
                        }
                    };
                    result.success = success;
                    result.error = error;

                    var withCredentials = this.withCredentials;
                    if (null != callinfo.withCredentials) {
                        withCredentials = callinfo.withCredentials;
                    }
                    if (withCredentials) {
                        result.withCredentials = withCredentials;
                    }

                    if ($the.isDefaultValue(result.headers)) {
                        delete result.headers;
                    }
                    if ($the.isDefaultValue(result.extensions)) {
                        delete result.extensions;
                    }

                }

                return result;
            };


            var build = $root.action = Action.build = function (action, $transport, $emitter, $baseUrl) {
                action = action||{};
                var t = action.transport || null;
                var e = action.emitter || null;
                delete action.transport;
                delete action.emitter;
                action = cast(Action, action || {}, excast);
                action.transport = $transport || t;
                action.emitter = $emitter || e;

                action.baseUrl = $baseUrl || action.baseUrl;

                var result = function () {
                    return action.execute.apply(action, arguments);
                };
                result.action = action;
                result.createRequest = action.createRequest.bind(action);
                result.getUrl = action.getUrl.bind(action);
                result.__callMoniker = 0;
                action.prepareEmitter();
                return result;
            };

            var extensionsRegex = /h[qd]_/;

            var stubtransport = $privates.stubtransport = {

            };

            var stubemitter = $privates.stubemitter = {

            };

            Action.prototype.transport = stubtransport;
            Action.prototype.emitter = stubemitter;




            var applyHttpExtensions = $privates.applyHttpExtensions = function (target, source) {
                for (var s in source) {
                    if (!source.hasOwnProperty(s))continue;
                    if (!!s.match(extensionsRegex)) {
                        var val = source[s];
                        var type = s.substring(1, 2);
                        var name = s.substring(3);
                        if (type == 'q') { //request extension
                            if (typeof (val) == 'object') {
                                target.extensions[name] = target.extensions[name] || {};
                                extend(target.extensions[name], val);
                            } else {
                                target.extensions[name] = val;
                            }
                        } else if (type == 'd') { //named header
                            target.headers = target.headers || {};
                            target.headers[name] = val;
                        }
                    }
                }
                return target;
            };

            $root.action.Action = Action;

        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));
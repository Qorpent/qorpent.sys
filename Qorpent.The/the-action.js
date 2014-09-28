/**
 * Action/Action builder module to wrapp HTTP/AJAX calls
 */
(function (define) {
    define(["./the-jsonify", "./the-interpolation"], function ($the) {
        return $the(function ($root,$privates) {
            var cast = $the.cast;
            var extend = $the.extend;
            var interpolate =$the.interpolate;
            var excast= $the.object.ExtendOptions.ExtendedCast;

            var ActionCallInfo = function () {
                this.onSuccess = null;
                this.onError = null;
                this.transport = null;
                this.emitter = null;
                this.headers = null;
                this.extensions = null;
                this.method = "GET";
                this.args = null;
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
                this.headers = null;
                this.extensions = null;
                this.useparams = true;
                return this;
            }


            Action.prototype.execute = function (args, callinfo) {
                args = args || {};
                callinfo = cast(ActionCallInfo, callinfo || {});
            };

            Action.prototype.getUrl = function (args, callinfo) {
                callinfo = cast(ActionCallInfo,callinfo||{},excast );
                var baseUrl = callinfo.baseUrl || this.baseUrl || "/";
                var addressType = callinfo.addressType || this.addressType || "url";
                if("url"===addressType){
                    var url = callinfo.url || this.url;
                    if(!!args && url.indexOf('$')){
                        url = interpolate(url, args);
                    }
                    url = url || "";
                    return ("/"+baseUrl+"/"+url).replace(/\/+/,"/");
                }else{
                    throw "special urls are not supported for now"
                }
            }

            var extensionsFalseFilter = function (_) {
                return !_.match(extensionsRegex)
            };
            var extensionsTrueFilter = function (_) {
                return _.match(extensionsRegex)
            };

            Action.prototype.getQuery = function (args, callinfo) {
                callinfo = cast(ActionCallInfo,callinfo||{},excast);

                //extract clean params
                var params = {};
                var result = { params: params, extensions: {} };
                // store all properties execpt extensions into params
                if(!!this.useparams){
                    extend(params, (!!this.arguments ? cast(this.arguments, args) : args), {filter: extensionsFalseFilter});  //arguments is special word

                    if (!!this.parameters) {
                        extend(result.params, this.parameters, {filter: extensionsFalseFilter});
                    }
                    extend(result.params, args || {}, {filter: extensionsFalseFilter});
                }



                applyHttpExtensions(result.extensions, this);

                if (!!this.headers) {
                    result.extensions.headers = result.extensions.headers || {};
                    for (var i in this.headers) {
                        if (this.headers.hasOwnProperty(i)) {
                            result.extensions.headers[i] = this.headers[i];
                        }
                    }
                }
                if (!!callinfo.headers) {
                    result.extensions.headers = result.extensions.headers || {};
                    for (var i in callinfo.headers) {
                        if (callinfo.headers.hasOwnProperty(i)) {
                            result.extensions.headers[i] = callinfo.headers[i];
                        }
                    }
                } if (!!this.extensions) {
                    for (var i in this.extensions) {
                        if (this.extensions.hasOwnProperty(i)) {
                            result.extensions[i] = this.extensions[i];
                        }
                    }
                }
                if (!!callinfo.extensions) {
                    for (var i in callinfo.extensions) {
                        if (callinfo.extensions.hasOwnProperty(i)) {
                            result.extensions[i] = callinfo.extensions[i];
                        }
                    }
                }
                applyHttpExtensions(result.extensions, args);

                return result;
            }


            var build = $root.action = Action.build = function (action, $transport, $emitter, $baseUrl) {
                action = cast(Action, action||{}, excast);
                action.transport = $transport || action.transport;
                action.emitter = $emitter || action.emitter;
                action.baseUrl = $baseUrl || action.baseUrl;
                var result = function () {
                    return action.execute.apply(action, arguments);
                }
                result.action = action;
                result.getQuery = action.getQuery.bind(action);
                result.getUrl = action.getUrl.bind(action);
                return result;
            }

            Action.prototype.transport = stubtransport;
            Action.prototype.emitter = stubemitter;


            var extensionsRegex = /h[qd]_/;

            var stubtransport = $privates.stubtransport = {

            };

            var stubemitter = $privates.stubemitter = {

            }

            var applyHttpExtensions = $privates.applyHttpExtensions = function (target, source) {
                for (var s in source) {
                    if (!source.hasOwnProperty(s))continue;
                    if (!!s.match(extensionsRegex)) {
                        var val = source[s];
                        var type = s.substring(1, 2);
                        var name = s.substring(3);
                        if (type == 'q') { //request extension
                            if (typeof (val) == 'object') {
                                target[name] = target[name] || {};
                                extend(target[name], val);
                            } else {
                                target[name] = val;
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
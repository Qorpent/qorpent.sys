/**
 * Action/Action builder module to wrapp HTTP/AJAX calls
 */
(function (define) {
    define(["./the-jsonify", "./the-interpolation","./the-http"], function ($the) {
        return $the(function ($root,$privates) {
            var cast = $the.cast;
            var extend = $the.extend;
            var interpolate =$the.interpolate;
            var excast= $the.object.ExtendOptions.ExtendedCast;
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
                this.castResult =null;
                this.rawResult = false;
                this.delay = 0;
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
                this.castResult =true;
                this.headers = null;
                this.extensions = null;
                this.useparams = true;
                this.withCredentials = true;
                this.success = null;
                this.error = null;
                this.eventName = "";
                this.jsonify = true;
                this.delay = 0;
                this.jsonifyOptions = {interpolate:true,defaults:false,stringify:true,evalfunctions:true,functions:false};
                this.__callMoniker  = 0;
                return this;
            }


            Action.prototype.execute = function (args, callinfo) {
                args = args || {};
                callinfo = cast(ActionCallInfo, callinfo || {},excast);
                var delay = Math.max(this.delay,callinfo.delay);
                var request  = this.createRequest(args,callinfo);
                var self = this;
                if(0!==delay){
                    self.__callMoniker++;
                    var myMoniker = self.__callMoniker; //interlocked-like syncronization
                    timeout(
                        function(){
                            if(self.__callMoniker == myMoniker){
                                $h(request);
                            }
                        }, delay
                    );
                    return null;
                }else{
                    return $h(request);
                }
            };

            Action.prototype.getUrl = function (args, callinfo) {
                callinfo = cast(ActionCallInfo,callinfo||{},excast );
                var baseUrl = callinfo.baseUrl || this.baseUrl || "/";
                var addressType = callinfo.addressType || this.addressType || "url";
                if("url"===addressType){
                    var url = callinfo.url || this.url || "";
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

            Action.prototype.createRequest = function (args, callinfo) {
                args = args || {};
                callinfo = cast(ActionCallInfo,callinfo||{},excast);

                //extract clean params
                var url = this.getUrl(args,callinfo);
                var result = { url: url, params: {}, headers:{}, extensions: {} };
                // store all properties execpt extensions into params
                if(!!this.useparams){
                    extend(result.params, (!!this.arguments ? cast(this.arguments, args) : args), {filter: extensionsFalseFilter});  //arguments is special word
                    extend(result.params, this.parameters, {filter: extensionsFalseFilter});
                    extend(result.params, args || {}, {filter: extensionsFalseFilter});
                    if(this.jsonify){
                        result.params = $j(result.params,this.jsonifyOptions);
                    }
                }

                result.method = callinfo.method || this.method;

                applyHttpExtensions(result, this);

                extend(result.headers,this.headers);
                extend(result.headers,callinfo.headers);
                extend(result.extensions,this.extensions);
                extend(result.extensions,callinfo.extensions);

                applyHttpExtensions(result, args);

                var self =this;
                var eventName = callinfo.eventName || this.eventName;
                var emitter = callinfo.emitter || this.emitter;
                var resultCtor = callinfo.result || this.result;
                var castResult = (callinfo.castResult || this.castResult)&&!!resultCtor && !callinfo.rawResult;
                var success = function(data,resp){
                    var realdata = data;
                    if(castResult){
                        if(Array.isArray(data)){
                            realdata = [];
                            for(var i = 0;i<data.length;i++){
                                realdata.push(cast(resultCtor,data[i],excast));
                            }
                        }else if(typeof  data === "object"){
                            realdata = cast(resultCtor,data,excast);
                        }
                    }
                    if(!!callinfo.success){
                        callinfo.success(realdata,resp);
                    }
                    if(!!self.success && !callinfo.suppressDefault){
                        self.success(realdata,resp);
                    }
                    if(!!eventName && !!emitter && !callinfo.suppressDefault){
                        emitter.emit(eventName,[realdata,resp,self]);
                    }
                }
                var error = function(error,resp){
                    if(!!callinfo.error){
                        callinfo.error(data,resp);
                    }
                    if(!!self.error && !callinfo.suppressDefault){
                        self.error(data,resp);
                    }
                    if(!!eventName && !!emitter && !callinfo.suppressDefault){
                        emitter.emit("ERROR:"+eventName,[error,resp,this]);
                    }
                }
                result.success =success;
                result.error =error;

                var withCredentials = this.withCredentials;
                if(null!=callinfo.withCredentials){
                    withCredentials = callinfo.withCredentials;
                }
                if(withCredentials){
                    result.withCredentials = withCredentials;
                }

                if($the.isDefaultValue(result.headers)){
                    delete result.headers;
                }
                if($the.isDefaultValue(result.extensions)){
                    delete result.extensions;
                }

                return result;
            }


            var build = $root.action = Action.build = function (action, $transport, $emitter, $baseUrl) {
                action = cast(Action, action||{}, excast);
                action.transport = $transport || action.transport;
                action.baseUrl = $baseUrl || action.baseUrl;
                action.emitter= $emitter || action.emitter;
                var result = function () {
                    return action.execute.apply(action, arguments);
                }
                result.action = action;
                result.createRequest = action.createRequest.bind(action);
                result.getUrl = action.getUrl.bind(action);
                result.__callMoniker = 0;
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
/**
 * Action/Action builder module to wrapp HTTP/AJAX calls
 */
(function (define) {
    define(["./the-object"], function ($the) {
        return $the(function ($root,$privates) {
            var timeout = setTimeout;
            if(typeof window!=="undefined"){
                timeout =window.setTimeout;
            }
            var h = $root.http = function(call){
                var defaultTransport = h.DetectDefaultTransport()
                var request = $the.cast(r,call,$the.object.ExtendOptions.ExtendedCast);
                request.transport = call.transport;
                var transport = request.transport || defaultTransport;
                return transport.execute(request);
            };

            var listeners = $privates._httpSubscribtions = [];
            h.addListener = function(func){
                listeners.push(func);
            }


            var r = h.Request = function(){};

            var t = h.Transport = function(){};

            var resp = h.Response = function(request,transport){
                this.request= request;
                this.transport = transport;
                this.timeouted = false;
                this.complete = false;
                this.success =false;
                this.isError = false;
                this.error = null;
                this.nativeResult = null;
                this.result = null;
            };
            resp.prototype.notify = function(eventName){
                for(var i=0;i<listeners.length;i++){
                    listeners[i](eventName,this);
                }
            }
            resp.prototype.execute = function(){
                var self = this;
                if(!!this.request.timeout){
                    timeout(function(){
                        if(self.isError)return;
                        self.timeouted = true;
                        self.isError = true;
                        self.error = "timeout";
                        if(!!self.request.onError){
                            self.request.onError("timeout",self);
                        }
                        self.notify("timeout");
                    },this.request.timeout)
                }
                var result;
                try{
                    this.transport.callData(this.request,
                        function(data,nativeResponse){
                            if(self.timeouted)return;
                            self.complete = true;
                            self.success = true;
                            self.nativeResult = nativeResponse;
                            self.result = data;
                            if(!!self.request.onSuccess){
                                self.request.onSuccess(self.result,self);
                            }
                            self.notify("success");
                        },
                        function(data,nativeResponse){
                            if(self.timeouted)return;
                            self.complete = true;
                            self.nativeResult = nativeResponse;
                            self.isError =true;
                            self.error = data;
                            if(!!self.request.onError){
                                self.request.onError(self.error,self);
                            }
                            self.notify("error");
                        }
                    );
                }catch(e){
                    self.isError = true;
                    self.error = e;
                    self.complete = true;
                    if(!!self.request.onError){
                        self.request.onError(self.error,self);
                    }
                    self.notify("error");
                }
                return this;
            }

            t.prototype.execute = function(request){
                var result = new resp(request,this);
                return result.execute();
            }

            t.prototype.callData  = function(request,success,error){
                throw "abstract, not implemented"
            }

            var JQueryTransport = h.JQueryTransport = function(){
                t.call(this);
            };
            JQueryTransport.prototype = Object.create(t.prototype);
            JQueryTransport.prototype.callData = function(request,success,error){
                h.CheckEnvironment();
                var jr = {};
                jr.url = request.url;
                jr.contentType  = request.contentType || 'application/x-www-form-urlencoded; charset=UTF-8';
                jr.data = request.parameters || {};
                jr.headers= request.headers || {};
                jr.ifModified = true;
                jr.password = request.password || "";
                jr.timeout = request.timeout || 0;
                jr.type = request.method || "GET";
                jr.username = request.username || "";
                jr.xhrFields = request.extensions || {};
                jr.xhrFields.withCredentials = !!request.withCredentials || false;
                jr.error = function(xhr,s,e){
                    error(e,xhr);
                }
                jr.success = function(d,s,xhr){
                    success(d,xhr);
                }
                return $the.$jQuery.ajax(jr);
            };




            var AngularTransport = h.AngularTransport = function(){
                t.call(this);
            };
            AngularTransport.prototype = Object.create(t.prototype);
            AngularTransport.prototype.callData = function(request,success,error){
                h.CheckEnvironment();
                var ar = {};
                ar.url = request.url;
                ar.method = request.method || "GET";
                if(request.method == "POST"){
                    ar.data = request.parameters || {};
                }else{
                    ar.params = request.parameters || {};
                }
                ar.headers= request.headers || {};
                ar.withCredentials = !!request.withCredentials || false;
                return $the.$angular.$__http(ar)
                    .success(function(data, status, headers, config) {
                        success(data,{status:status,headers:headers,config:config});
                    })
                    .error(function(data, status, headers, config) {
                        error(data,{status:status,headers:headers,config:config});
                    })
            }

            var XHRTransport = h.XHRTransport = function(){
                t.call(this);
            };
            XHRTransport.prototype = Object.create(t.prototype);
            XHRTransport.prototype.callData = function(request,success,error){
                error({erorr:"not implemented"});
            }

            var TestTransport = h.TestTransport = function(){
                t.call(this);
            };
            TestTransport.prototype = Object.create(t.prototype);
            TestTransport.prototype.callData = function(request,success,error){

                var resp = null;
                if(typeof request.response === "undefined"){
                    resp = TestTransport.responseFactory(request);
                }

                var t = resp.timeout || 20;
                timeout(function(){
                if(!!resp.error){
                    error (resp.error,resp);
                }else{
                    success(resp.data,resp);
                }},t);
            }
            TestTransport.responseFactory = function(req){return {}};


            h.DefaultTransport  = null;
            h.CheckEnvironment = function(){
                if(typeof window !== "undefined" ){
                    if(typeof $the.$angular==="undefined" ){
                        $the.$angular = $the.$angular || window.angular;
                        if(typeof $the.$angular.$__http === "undefined"){
                            var m = $the.$angular.module("THE_ANGULAR_STUB",[]);
                            var injector = angular.injector(['THE_ANGULAR_STUB', 'ng']);
                            var http = injector.get("$http");
                            $the.$angular.$__http = http;
                        }
                    }
                    $the.$jQuery = $the.$jQuery || window.$;
                }
            }
            h.DetectDefaultTransport = function(){
                h.CheckEnvironment();
                if(null!= h.DefaultTransport)return h.DefaultTransport;

                if(!!$the.$jQuery){
                    return h.DefaultTransport = new JQueryTransport();
                }
                if(!!$the.$angular){
                    return h.DefaultTransport = new AngularTransport();
                }
                return h.DefaultTransport = new XHRTransport();
            };


        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));
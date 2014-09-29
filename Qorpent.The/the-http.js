/**
 * Action/Action builder module to wrapp HTTP/AJAX calls
 */
(function (define) {
    define(["./the-object"], function ($the) {
        return $the(function ($root,$privates) {

            var excast= $the.object.ExtendOptions.ExtendedCast;
            var timeout = $the.timeout;
            var tick = $the.tick;
            var h = $root.http = function(call){
                try{
                var defaultTransport = h.DetectDefaultTransport()
                var request = $the.cast(Request,call,$the.object.ExtendOptions.ExtendedCast);

                request.transport = call.transport;
                var transport = request.transport || defaultTransport;

                return transport.execute(request);

                }catch(e){
                    if(call.error){
                        call.error(e);  
                    }
                }
            };

            var listeners = $privates._httpSubscribtions = [];
            h.addListener = function(func){
                listeners.push(func);
                return h;
            }
            h.removeListener = function(func){
                var idx = listeners.indexOf(func);
                if(-1!=idx){
                    listeners.splice(idx,1);
                }
                return h;
            }

            h.cleanListeners = function(func){
                listeners = $privates._httpSubscribtions = [];
                return this;
            }



            var t = h.Transport = function(){};

            var Request = h.Request = function(){
                this.url = "/";
                this.params= {};
                this.headers= {};
                this.withCredentials = true;
                this.timeout = 0;
                this.username = "";
                this.password = "";
                this.method = "GET";
                this.contentType =  'application/x-www-form-urlencoded; charset=UTF-8';
            }

            var resp = h.Response = function(request,transport){
                request = $the.cast(Request,request,excast);
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
                var name = eventName;
                var self = this;
                for(var i=0;i<listeners.length;i++){
                    tick(function(){
                        listeners[i](name,self);
                    });
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
                        if(!!self.request.error){
                            tick(function(){
                                self.request.error("timeout",self);
                            });
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
                            if(!!self.request.success){
                                tick(function(){
                                    self.request.success(self.result,self);
                                });
                            }
                            self.notify("success");
                        },
                        function(data,nativeResponse){
                            if(self.timeouted)return;
                            self.complete = true;
                            self.nativeResult = nativeResponse;
                            self.isError =true;
                            self.error = data;
                            if(!!self.request.error){
                                tick(function(){
                                    self.request.error(self.error,self);
                                });
                            }
                            self.notify("error");
                        }
                    );
                }catch(e){
                    self.isError = true;
                    self.error = e;
                    self.complete = true;
                    if(!!self.request.onError){
                        tick(function(){
                        self.request.onError(self.error,self);

                        });
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
                jr.data = request.params || {};
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
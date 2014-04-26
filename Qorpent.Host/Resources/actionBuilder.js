/*
	Action Builder Module, angular friendly
	
	ASSUME that $http is angular $http instance, that can be
	got in service/controller constructor
	
	Example 1: very simple action
		var ping = createAction ( $http, { url:'/ping' } ); //while siteroot is second argument it can be skipped
		ping().success(function(result){console.log(result);});
	
	Example 2: qweb action with "strongly-typed" parameters and result
		function sqrquery (){
			this.x = 1;
			this.y = 1;
		}
		function sqrresult (){
			this.sqr = 0;
			this.perimiter = 0;
		}		
		var calcsquare = createAction ( $http, { controller : 'sample', type:'json', name : 'calcsquare', arguments : myquery, result : sqrresult });
		calcsquare ( { X:23 } ); // will send /sample/name.json.qweb?x=23&y=1 with case and all parameters corrected
		
	Example 3 : calling with $http extensions ( withCredentials options and MYHEADER will be set )
		var ping = createAction ( $http, { url:'/ping' } );
		ping ( { hq_withCredentials : true, hd_MYHEADER : 'mydata' } ); // will send /ping with withCredentials $http option an MYHEADER=mydata header
		
	Example 4 : provide header with query constructor 
		function searchQuery (args) {
			this.pattern = "*";
			this.type = "all";
			if(args && typeof(args)=="string"){
				this.pattern = args;
			}
			this.hd_CLIENT_TIMESTAMP = new Date();
		}
		var search = createAction ( $http, { url : '/search' , arguments : searchQuery }  );
		search ( 'abc' ); will send /search?pattern=abc&type=all and  CLIENT_TIMESTAMP with current client date will be added
		
	Example 4 : config can provide extensions too
		var search = createAction ( $http, { url : '/search' , arguments : searchQuery, hq_withCredentials : true }  );
		//now all calls of search() will be with withCredentials
		
	Example 5: any option in config can be function(args) from args, for ex: url
		var ping = createAction ( $http, { url : function(args){ return '/ping/'+JSON.stringify(args)+'/';}});
		ping({a:1}) will be call to /ping/{a:1}/?a=1
		
	See all supported request parameters for angular at http://docs.angularjs.org/api/ng/service/$http
	
	Attention - if u want to use wrapped data in result alwais use this mode of call:
		action( {...}, function(...){}),
		not action( {...} ).success(function(...){}), - because here it will be native $http handler, not managed
		
*/
define(['qObject'], function (qObject) {
	
	var planalize = qObject.planalize;
	var planalizeOptions = qObject.planalizeOptions;
	var extend = qObject.extend;
	var extensionsRegex = qObject.extensionsRegex;
	var applyHttpExtensions = qObject.applyHttpExtensions;
	
	return  function createAction($http,_emitter, _siteroot,_config){
		var emitter = _emitter;
        var siteroot = _siteroot;
		var config = _config;
		// allows to skip siteroot in call
		if(typeof(siteroot) == 'object'){
			config = siteroot;
			siteroot = '/';
		}
		siteroot = siteroot||"/";
		var self = null;
		var result = self = function( args , success, error ,hint){
			return self.execute (args,success,error,hint);
		};
	    extend(result,config);
		result.__delayTimeout = result.__delayTimeout || 500;
		result.getQuery = function( args ){
			//extract clean params
			var params = {};
			extend ( params, (config.arguments ? new config.arguments(args) : {}) , '',  extensionsRegex);  //arguments is special word
			var result = { params : params, extensions : {} };
		    // store all properties execpt extensions into params
		    if (config.parameters) {
		        extend(result.params, config.parameters, this.extendprefix, extensionsRegex);
		    }
			extend(result.params, args||{}, this.extendprefix, extensionsRegex); 
			applyHttpExtensions (result.extensions, this);
			applyHttpExtensions (result.extensions, args );
			return result;
		};
		result.getUrl = function (args, hint) {
			siteroot = siteroot || '/';
			if(!!this.url){
			    var url = planalize(this.url, args, true, true, true);
			    if ( !!hint) {
			        url = "/"+hint + url.substring(1);
			    }
				return (siteroot + url).replace(/\/\//,'/');
			}
			var controller = planalize(this.controller,args);
			var name = planalize(this.name,args);
			var type = planalize(this.type,args);
			return siteroot + controller + '/' + name + '.' + type + '.qweb';
		};
		result.getResult = function ( data, args ) {
			if(!this.result){
				return data;
			}
			if ( this.isarray ) {
				var arrayresult  = [];
				for (var i=0; i<data.length; i++ ){
					var item = this.result ? new this.result(args) : {};
					extend(item,data[i] || {});
					arrayresult.push(item);
				}
				return arrayresult;
			}else{
				var singleresult =  this.result ? new this.result(args) :{};
				extend(singleresult, data || {});
				return singleresult;
			}
		};
		result.begin = function (_args, _success, _error) {
		    var hint = "";
		    if (!!this.supportasync) {
		        hint = "~";
		    }
		    var self = this;
		    var new_success = function(_, a, b, c) {
		        self.lastCall = _;
		        if (!!_success) {
		            _success(_, a, b, c);
		        }
		    };
		    return result.execute(_args, new_success, _error,hint);
		};
		result.end = function (_args, _success, _error) {
		    var hint = "";
		    if (!!this.supportasync) {
		        hint = "!";

		    } else {
		        hint = "cached";
		    }
		    return result.execute(_args, _success, _error, hint);
		};
		
		result.__callMoniker = 0; // syncronization for delayed call
		result.execute = function( _args , _success, _error, _hint ){
			if(typeof(_success)=='string'){
				_hint = _success;
				_success = null;
			}
			if(typeof(_error)=='string'){
				_hint = _error;
				_error = null;
			}
			var self = this;
			// delayed launch support for scenario with search strings and so on, when we must delay
			// response and dismiss all except last call in sequence
			if('delay'==_hint){
				self.__callMoniker++;
				var myMoniker = self.__callMoniker; //interlocked-like syncronization
				var cachedArguments = [_args,_success,_error];
				window.setTimeout(
					function(){
						if(self.__callMoniker == myMoniker){
							self.execute.apply(self,cachedArguments);
						}
					}, self.__delayTimeout
				);
				return;
			}
			
			
			if (!!config.confirm) {
                if (!confirm(config.confirm)) return;
            }
			var args = _args;
			var success = _success;
			var error = _error;
			//guess that it's single success call
			if ((!success && !!args && typeof(args)=="function")||args===console){
				success = args;
				args = {};
			}
		    
            //special case - load into var
			if (typeof (args) == "string" && args.match(/^->\w[\w\d_\.]*$/) && !success) {
			    success = args;
                args = {};
			}
		    
			if (typeof (success) == "string") {
			    var eval_ = success;
                if (success.match(/^->\w[\w\d_\.]*$/)) {
                    eval_ =  success.substring(2)+"=_";
                }
			    success = function(_) {
			        eval(eval_);
			    };
			}

			//use args as callback to action if function given as args
			if(typeof(args)=="function"){
				args = args(this);
			}
		    
            if (success === console) {
                success = function(_) {
                    console.log(_);
                };
            }
		    
            if (_hint == "cached") {
                if (success) {
                    success(this.lastCall);
                    return;
                }
            }
			
			var query = this.getQuery ( args );
			var method = this.method || 'GET';

			var planoptions = new planalizeOptions();
			if (method == "POST" && this.jsonify) {
			    planoptions.stringifyobjects = false;
			}
		    if (this.keepdefaults) {
		        planoptions.skipempty = false;
		        planoptions.skipzeroes = false;
		        planoptions.skipfalses = false;
		    }
		    var data = planalize(query.params, args, planoptions);
			
			var callinfo = { 
				method : method, 
				url :this.getUrl(args,_hint), 
				params : method=='POST' ?  null : data ,
				data : method=='POST' ? data : null
			};
		    // remove difference between $.ajax and angular $http
            if (!!$ && callinfo.params) {
                if ($http == $.ajax) {
                    callinfo.data = callinfo.params;
                    delete callinfo.params;
                }
            }
			
			extend ( callinfo , query.extensions ) ;
		
		    
			var result = $http(callinfo)				
			.success( function( data, status, headers, config ) {
				var wrappedData = data;
				if(!this.skipwrap){
					wrappedData = self.getResult (data) ;
				}
				if (!!success) {
					if(success===console.log||success===console){
						console.log(wrappedData);
					}else{
						success(wrappedData, status, headers, config, data);
					}
				}
                    result.emit(wrappedData, '', callinfo);
			})
			.error( function( data, status, headers, config ) {
				if (!!error) {
				    error(data, status, headers, config);
				}
                    result.emit(data, '_ERROR', callinfo);
			});
			result.emit = function(data, eventSuffix, callinfo) {
                if (!emitter || !config.emits || (config.emits.length == 0)) return;
                config.emits.forEach(function(e, i) {
                    var eventName = e + eventSuffix;

                    emitter.$emit(eventName, [data, callinfo]);
                });
            };
			return result;
		};	
		return result;
	};
});
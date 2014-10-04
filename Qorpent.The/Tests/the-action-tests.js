/**
 * Created by comdiv on 24.09.14.
 */
(function (describe, it, before) {
    describe("the.action", function () {
        this.timeout(5000);

        describe("#execute",function(){
            this.timeout(2000);
            it("simplest test",function(done){
               $({url:"good"})({},{
                   success: function(data){
                       data.should.eql({good:true});
                       done();
                   }
               })
            });
            it("simplest short notation",function(done){
                $({url:"good"})(function(data){
                    data.should.eql({good:true});
                    done();
                });
            });
            it("simplest short notation (with args)",function(done){
                $({url:"echo"})({a:1},function(data){
                    data.should.eql({a:1});
                    done();
                });
            });
            it("simplest short notation (error)",function(done){
                $({url:"bad"})(function(data,resp,error){
                    should.not.exist(data);
                    error.should.equal("some fail");
                    done();
                });
            });
            it("simplest default success",function(done){
                $({url:"good",success: function(data){
                    data.should.eql({good:true});
                    done();
                }})();
            });
            it("simplest emmiter",function(done){
                var e = {
                    emit: function(name,data){
                        name.should.equal("TEST");
                        data[0].should.eql({good:true});
                        done();
                    },
                    on : function(){}
                }
                $({url:"good",emitter: e,eventName:"TEST"})();
            });
            it("call from emmiter",function(done){
                var e = {
                    emit: function(name,data){
                        name.should.equal("TEST");
                        data[0].should.eql({good:true});
                        done();
                    },
                    on : function(name,func){
                        this.callback = func
                    },
                    fire : function(){
                        this.callback({},[{url:"good",emitter: e,eventName:"TEST"}])
                    }
                }
                $({url:"good",emitter: e,eventName:"TEST"});
                e.fire();
            });
            it("strong typed result support",function(done){
                var r = function(){this.GOOD=false};
                $({url:"good",result:r, success: function(data){
                    data.should.eql({GOOD:true});
                    done();
                }})();
            });
            it("strong typed  override result support",function(done){
                var r = function(){this.GOOD=false};
                var r2 = function(){this.Good=false};
                $({url:"good",result:r, success: function(data){
                    data.should.eql({Good:true});
                    done();
                }})({},{result:r2});
            });
            it("strong typed raw result override",function(done){
                var r = function(){this.GOOD=false};
                $({url:"good",result:r, success: function(data){
                    data.should.eql({good:true});
                    done();
                }})({},{rawResult:true});
            });
            it("can call echo (test propose)",function(done){
                $({url:"echo",success: function(data){
                    data.should.eql({x:1});
                    done();
                }})({x:1});
            });
            it("can call delayed",function(done){
                var res = [];
                var a = $({url:"echo",delay:15,success:function(_){res.push(_)}});
                setTimeout(function(){a({x:1})},10);
                setTimeout(function(){a({x:2})},40);
                setTimeout(function(){a({x:3})},50);
                setTimeout(function(){a({x:4})},60);
                setTimeout(function(){
                    res.should.eql([{x:1},{x:4}]);
                    done();
                },150)
            });
        });

        describe("#getUrl",function(){
            it("simplest",function(){
                $().getUrl().should.equal("/");
            });
            it("fixed url",function(){
                $({url:"/test"}).getUrl().should.equal("/test");
            });
            it("base url and url path fix",function(){
                $({baseUrl:"app", url:"test"}).getUrl().should.equal("/app/test");
            });
            it("interpolated urls",function(){
                $({ url:"/${year}/${month}"}).getUrl({year:2013,month:10}).should.equal("/2013/10");
            });
            it("baseUrl override from call info",function(){
                $({baseUrl:"/",url:"test"}).getUrl({},{baseUrl:"/other"}).should.equal("/other/test");
            });
            it("url override from call info",function(){
                $({baseUrl:"/",url:"test"}).getUrl({},{url:"/other"}).should.equal("/other");
            });
        })

        describe("#createRequest",function(){
            var delfun = function(req){
                //removes trivials
                delete req.success;
                delete req.error;
                delete req.method;
                return req;
            }
            it("simplest",function(){
                delfun($().createRequest({x:1})).should.eql({ url: '/', params: { x: 1 }, withCredentials:true });
            });

            it("headers in callinfo",function(){
                delfun($().createRequest({x:1},{headers:{a:1}})).should.eql({ url: '/',params:{x:1},headers:{a:1}, withCredentials:true});
            });
            it("can disable parameters",function(){
                var a= $({useparams:false}).action;
                a.useparams.should.equal(false);
                delfun(a.createRequest({x:1},{headers:{a:1}})).should.eql({ url: '/',params:{},headers:{a:1}, withCredentials:true});

            });
            it("can use strongly typed parameters",function(){
               var f = function(){this.a =1,this.b=2,this.d=4;};
                delfun($({arguments:f}).createRequest({A:3,B:2,C:3})).should.eql({ url: '/',params:{a:3,b:2,d:4,C:3}, withCredentials:true});
            });
            it("can use strongly typed parameters - real sample",function(){
                var f = function(){this.Pattern="a";};
                delfun($({arguments:f}).createRequest({pattern:'b'})).should.eql({ url: '/',params:{Pattern:'b'}, withCredentials:true});
            });
            it("headers in action",function(){
                var a = new $.Action();
                a.headers = {a:1};
                delfun(a.createRequest({x:1})).should.eql({ url: '/',params:{x:1},headers:{a:1}, withCredentials:true});
            });
            it("headers in args hd_ style",function(){
                var a = new $.Action();
                delfun(a.createRequest({x:1,hd_a:1})).should.eql({ url: '/',params:{x:1},headers:{a:1}, withCredentials:true});
            });
            it("headers in actions hd_ style",function(){
                var a = new $.Action();
                a.hd_a = 1;
                delfun(a.createRequest({x:1})).should.eql({ url: '/',params:{x:1},headers:{a:1}, withCredentials:true});
            });

            it("extensions in callinfo",function(){
                var a = new $.Action();
                delfun(a.createRequest({x:1},{extensions:{a:1}})).should.eql({ url: '/',params:{x:1},extensions:{a:1}, withCredentials:true});
            });
            it("extensions in action",function(){
                var a = new $.Action();
                a.extensions = {a:1};
                delfun(a.createRequest({x:1})).should.eql({ url: '/',params:{x:1},extensions:{a:1}, withCredentials:true});
            });
            it("extensions in args hq_ style",function(){
                var a = new $.Action();
                delfun(a.createRequest({x:1,hq_a:1})).should.eql({ url: '/',params:{x:1},extensions:{a:1}, withCredentials:true});
            });
            it("can disable credentials with action",function(){
                var a = $({withCredentials:false});
                delfun(a.createRequest({x:1})).should.eql({ url: '/',params:{x:1}});
            });
            it("can disable credentials with callinfo",function(){
                var a = new $.Action();
                delfun(a.createRequest({x:1},{withCredentials:false})).should.eql({ url: '/',params:{x:1}});
            });
            it("jsonify max by default",function(){
                var a = new $.Action();
                delfun(a.createRequest({x:1,y:null,z:0,a:false,b:{a:"${x}${x}"},c:function(){return this.x+23;}},{withCredentials:false})).should.eql({ url: '/',params:{x:1,b:"{\"a\":\"11\"}",c:24}});
            });
            it("jsonify can be disabled",function(){
                var a = $({jsonify:false}).action;
                delfun(a.createRequest({x:1,y:"${x}"},{withCredentials:false})).should.eql({ url: '/',params:{x:1,y:"${x}"}});
            });
            it("jsonify can be modified",function(){
                var a = $({jsonifyOptions:{defaults:true}}).action;
                delfun(a.createRequest({x:1,y:"${x}${x}",z:null},{withCredentials:false})).should.eql({ url: '/',params:{x:1,y:"11",z:null}});
            });
            it("extensions in actions hq_ style",function(){
                var a = new $.Action();
                a.hq_a = 1;
                delfun(a.createRequest({x:1})).should.eql({ url: '/',params:{x:1},extensions:{a:1}, withCredentials:true});
            });

            it("args/callinfo/action override priority for headers",function(){
                var a = new $.Action();
                a.headers= {a:1,b:2,c:3};
                var ci = {
                    headers : {a:2,b:3}
                }
                var args = {
                    x:1,
                    hd_a : 4
                }
                delfun( a.createRequest(args,ci)).should.eql({url:"/", params:{x:1},headers:{a:4,b:3,c:3}, withCredentials:true});
            });
            it("args/callinfo/action override priority for extensions",function(){
                var a = new $.Action();
                a.extensions= {a:1,b:2,c:3};
                var ci = {
                    extensions : {a:2,b:3}
                }
                var args = {
                    x:1,
                    hq_a : 4
                }
                delfun(a.createRequest(args,ci)).should.eql({url:"/", params:{x:1},extensions:{a:4,b:3,c:3}, withCredentials:true});
            });
        });




        var $ = null;
        var $root = null;
        var should = null;
        var $h = null;

        before(function (done) {
            var requirejs = null;
            try {
                if (!!define) { //cause exception
                    requirejs = require;
                }
            } catch (e) {
                requirejs = require("requirejs");
                requirejs.config({
                    baseurbaseUrl: '.',
                    nodeRequire: require});
            }
            try {
                requirejs(["./lib/chai", "../the-action","../the-http-test","../the-interpolation"], function ($should, $the) {
                    should = $should.Should();
                    $ = $the.action;
                    $root = $the;
                    $h = $the.http;
                    $h.DefaultTransport = new $h.TestTransport();
                    done();
                });
            } catch (e) {
                console.log(e);
                done();
            }
        });


    });
})(describe, it, before);
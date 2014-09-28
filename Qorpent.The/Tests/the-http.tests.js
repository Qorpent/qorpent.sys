/**
 * Created by comdiv on 24.09.14.
 */
(function (describe, it, before) {

    describe("the.http", function () {
        this.timeout(1000);
        var $ = null;
        var $root = null;
        var should = null;

        if(typeof window !== "undefined") {
            it("can use $jq transport",function(done){
                $({
                    url:"sample.json",
                    transport: new $.JQueryTransport(),
                    onSuccess : function(data,resp){
                        data.should.eql({x:1});
                        done();
                    },
                    onError : function(resp){
                        resp.error.should.equal("");
                        done();
                    }
                });
            });
            it("can use $angular transport",function(done){
                $({
                    url:"sample.json",
                    transport: new $.AngularTransport(),
                    onSuccess : function(data,resp){
                        data.should.eql({x:1});
                        done();
                    },
                    onError : function(resp){
                        resp.error.should.equal("");
                        done();
                    }
                });
            });
        }

        it("can use TestTransport (success)",function(done){
            $({
                url:"/good",
                onSuccess : function(data){
                    data.should.eql({good:true});
                    done();
                },
                onError : function(error){
                    error.should.equal("");
                    done();
                }
            });
        });

        it("can use TestTransport (error)",function(done){
            $({
                url:"/bad",
                onSuccess : function(data,resp){
                    data.should.equal("");
                    done();
                },
                onError : function(error,resp){
                    error.should.equal("some fail")
                    done();
                }
            });
        });

        it("can use TestTransport (timeout)",function(done){
            $({
                url:"/good",
                timeout:10,
                onSuccess : function(data,resp){
                    data.should.equal("");
                    done();
                },
                onError : function(error,resp){
                    error.should.equal("timeout")
                    done();
                }
            });
        });




        before(function (done) {
            var requirejs = null;
            try {
                if (!!define) { //cause exception
                    requirejs = require;
                }
            } catch (e) {
                requirejs = require("requirejs");
                requirejs.config({baseurbaseUrl: '.', nodeRequire: require});
            }
            try {
                requirejs(["./lib/chai", "../the-http"], function ($should, $the,$j) {
                    should = $should.Should();
                    $ = $the.http;
                    $root = $the;
                    $.TestTransport.responseFactory = function(req){
                        if(req.url==="/good"){
                            var res = {data:{good:true}};
                            if(!!req.timeout){
                                res.timeout = req.timeout+10;
                            }
                            return res;
                        }  else{
                            return {error:"some fail"};
                        }
                    };
                    $.DefaultTransport = new $.TestTransport();
                    done();
                });
            } catch (e) {
                console.log(e);
                done();
            }
        });


    });
})(describe, it, before);
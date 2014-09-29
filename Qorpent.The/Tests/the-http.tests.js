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
                    success : function(data,resp){
                        data.should.eql({x:1});
                        done();
                    },
                    error : function(resp){
                        resp.error.should.equal("");
                        done();
                    }
                });
            });
            it("can use $angular transport",function(done){
                $({
                    url:"sample.json",
                    transport: new $.AngularTransport(),
                    success : function(data,resp){
                        data.should.eql({x:1});
                        done();
                    },
                    error : function(resp){
                        resp.error.should.equal("");
                        done();
                    }
                });
            });
        }

        it("can use TestTransport (success)",function(done){
            $({
                url:"/good",
                success : function(data){
                    data.should.eql({good:true});
                    done();
                },
                error : function(error){
                    error.should.equal("");
                    done();
                }
            });
        });

        it("can use TestTransport (error)",function(done){
            $({
                url:"/bad",
                success : function(data,resp){
                    data.should.equal("");
                    done();
                },
                error : function(error,resp){
                    error.should.equal("some fail")
                    done();
                }
            });
        });

        it("can use TestTransport (timeout)",function(done){
            $({
                url:"/good",
                timeout:10,
                success : function(data,resp){
                    data.should.equal("");
                    done();
                },
                error : function(error,resp){
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
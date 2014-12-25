/**
 * Created by comdiv on 24.09.14.
 */
define(["the","chai","the-http-test"],function($the,chai){
    var should = chai.Should();
    var $ = $the.http;
    var $root = $the;
    $.DefaultTransport = new $.TestTransport();

    describe("the.http", function () {
        this.timeout(1000);

        if(typeof window !== "undefined") {
            it("can use $jq transport",function(done){
                $({
                    url:"../tests/sample.json",
                    transport: new $.JQueryTransport(),
                    success : function(data,resp){
                        if(typeof data==="string")data =JSON.parse(data);
                        data.should.eql({x:1});
                        done();
                    },
                    error : function(resp){
                        console.error(resp);
                        resp.error.should.equal("");
                        done();
                    }
                });
            });
            it("can use $angular transport",function(done){
                $({
                    url:"../tests/sample.json",
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
    });
});
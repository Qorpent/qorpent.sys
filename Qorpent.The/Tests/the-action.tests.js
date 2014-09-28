/**
 * Created by comdiv on 24.09.14.
 */
(function (describe, it, before) {
    describe("the.action", function () {
        this.timeout(5000);

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

        describe("#getQuery",function(){
            it("simplest",function(){
                $().getQuery({x:1}).should.eql({params:{x:1},extensions:{}});
            });

            it("headers in callinfo",function(){
                $().getQuery({x:1},{headers:{a:1}}).should.eql({params:{x:1},extensions:{headers:{a:1}}});
            });
            it("can disable parameters",function(){
                var a= $({useparams:false}).action;
                a.useparams.should.equal(false);
                a.getQuery({x:1},{headers:{a:1}}).should.eql({params:{},extensions:{headers:{a:1}}});

            });
            it("can use strongly typed parameters",function(){
               var f = function(){this.a =1,this.b=2,this.d=4;};
                $({arguments:f}).getQuery({A:3,B:2,C:3}).should.eql({params:{a:3,b:2,d:4,C:3},extensions:{}});
            });
            it("headers in action",function(){
                var a = new $.Action();
                a.headers = {a:1};
                a.getQuery({x:1}).should.eql({params:{x:1},extensions:{headers:{a:1}}});
            });
            it("headers in args hd_ style",function(){
                var a = new $.Action();
                a.getQuery({x:1,hd_a:1}).should.eql({params:{x:1},extensions:{headers:{a:1}}});
            });
            it("headers in actions hd_ style",function(){
                var a = new $.Action();
                a.hd_a = 1;
                a.getQuery({x:1}).should.eql({params:{x:1},extensions:{headers:{a:1}}});
            });

            it("extensions in callinfo",function(){
                var a = new $.Action();
                a.getQuery({x:1},{extensions:{a:1}}).should.eql({params:{x:1},extensions:{a:1}});
            });
            it("extensions in action",function(){
                var a = new $.Action();
                a.extensions = {a:1};
                a.getQuery({x:1}).should.eql({params:{x:1},extensions:{a:1}});
            });
            it("extensions in args hq_ style",function(){
                var a = new $.Action();
                a.getQuery({x:1,hq_a:1}).should.eql({params:{x:1},extensions:{a:1}});
            });
            it("extensions in actions hq_ style",function(){
                var a = new $.Action();
                a.hq_a = 1;
                a.getQuery({x:1}).should.eql({params:{x:1},extensions:{a:1}});
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
                a.getQuery(args,ci).should.eql({params:{x:1},extensions:{headers:{a:4,b:3,c:3}}});
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
                a.getQuery(args,ci).should.eql({params:{x:1},extensions:{a:4,b:3,c:3}});
            });
        });


        var $ = null;
        var $root = null;
        var should = null;

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
                requirejs(["./lib/chai", "../the-action"], function ($should, $the) {
                    should = $should.Should();
                    $ = $the.action;
                    $root = $the;
                    done();
                });
            } catch (e) {
                console.log(e);
                done();
            }
        });


    });
})(describe, it, before);
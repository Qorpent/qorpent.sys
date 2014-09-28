/**
 * Created by comdiv on 24.09.14.
 */
(function (describe, it, before) {
    describe("the.interpolate", function () {
        this.timeout(5000);

        it("string(obj)",function(){
            $("${x}",{x:1}).should.equal("1");
            $("${x}-${y}",{x:1,y:2}).should.equal("1-2");
        });
        it("string(obj)#JSON",function(){
            $("${x}-${y}-${obj}",{x:1,y:2,obj:{a:1}}).should.equal("1-2-{\"a\":1}");
        });
        it("string(obj)#function",function(){
            $("${x}-${y}-${func}",{x:1,y:2,func:function(){
                return this.x+this.y;
            }}).should.equal("1-2-3");
        });
        it("string(obj)#null",function(){
            $("${x}-${y}",{x:1,y:null}).should.equal("1-");
        });
        it("string(obj)#undefined",function(){
            $("${x}-${y}",{x:1}).should.equal("1-${y}");
        });

        it("string(layered)",function(){
            $("${x}",$l({x:1})).should.equal("1");
            $("${x}-${y}",$l({x:1,y:2})).should.equal("1-2");
        });
        it("string(layered)#JSON",function(){
            $("${x}-${y}-${obj}",$l({x:1,y:2,obj:{a:1}})).should.equal("1-2-{\"a\":1}");
        });
        it("string(layered)#function",function(){
            $("${x}-${y}-${func}",$l({x:1,y:2,func:function(){
                return this.get('x')+this.get('y');
            }})).should.equal("1-2-3");
        });
        it("string(layered)#null",function(){
            $("${x}-${y}",$l({x:1,y:null})).should.equal("1-");
        });
        it("string(layered)#undefined",function(){
            $("${x}-${y}",$l({x:1})).should.equal("1-${y}");
        });

        it("object(obj)",function(){
           $({a:"${x}"},{x:1}).should.eql({a:"1"});
        });
        it("object(obj)#self",function(){
            $({a:"${x}-${b}", b:2},{x:1}).should.eql({a:"1-2",b:2});
        });
        it("object(obj)#recursive",function(){
            $({a:"${b}-${c}-${d}", b:2, c:"${b}2", d:"${c}3"},{x:1}).should.eql({
                "a": "2-22-223",
                "b": 2,
                "c": "22",
                "d": "223"
            });
        });
        it("object(obj)#internals",function(){
            $({a:2,b:"${a}3",c:{
                a:3,
                d:"${a}${b}",
                e:"${d}5"
            }},{}).should.eql({
                    "a": 2,
                    "b": "23",
                    "c": {
                        "a": 3,
                        "d": "323",
                        "e": "3235"
                    }
                });
        });


        it("object(layered)",function(){
            $({a:"${x}"},$l({x:1})).should.eql({a:"1"});
        });
        it("object(layered)#self",function(){
            $({a:"${x}-${b}", b:2},$l({x:1})).should.eql({a:"1-2",b:2});
        });
        it("object(layered)#recursive",function(){
            $({a:"${b}-${c}-${d}", b:2, c:"${b}2", d:"${c}3"},$l({x:1})).should.eql({
                "a": "2-22-223",
                "b": 2,
                "c": "22",
                "d": "223"
            });
        });
        it("object(layered)#internals",function(){
            $({a:2,b:"${a}3",c:{
                a:3,
                d:"${a}${b}",
                e:"${d}5"
            }},$l({})).should.eql({
                    "a": 2,
                    "b": "23",
                    "c": {
                        "a": 3,
                        "d": "323",
                        "e": "3235"
                    }
                });
        });



        var $ = null;
        var $l = null;
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
                requirejs(["chai", "../the-interpolation","../the-collections-layered"], function ($should, $the) {
                    should = $should.Should();
                    $ = $the.interpolate;
                    $l = $the.collections.LayeredDictionary;
                    done();
                });
            } catch (e) {
                console.log(e);
                done();
            }
        });


    });
})(describe, it, before);
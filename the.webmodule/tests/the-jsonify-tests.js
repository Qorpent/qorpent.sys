/**
 * Created by comdiv on 24.09.14.
 */
(function (describe, it, before) {
    describe("the.jsonify", function () {
        this.timeout(5000);

        it("always make fresh clone",function(){
            var f = function(){this.a=1,this.c={x:1},this.f=function(){}};
            f.prototype.b=22;
            var x= new f();
            var _x = $(x);
            _x.f.should.a("function");
            delete _x.f;
            _x.should.eql({a:1,c:{x:1}});
            _x.should.not.equal(x);
            _x.should.not.instanceOf(f);
            _x.__proto__.should.equal(Object.prototype);
        });
        it("can remove functions",function(){
            $({a:1,b:function(){}},{functions:false}).should.eql({a:1});
        });

        var del = function(obj,fields){
            var result = $root.clone(obj);
            for(var i =0;i<fields.length;i++){
                delete result[fields[i]];
            }
            return result;
        }
        var base = {a:1,a2:{x:1},a3:"x",a4:true, a5:[1], b1:null,b2:0,b3:"",b4:false,b5:[],b6:{}};
        it("can remove nulls and defaults",function(){

            $(base).should.eql(base);
            $(base,{nulls:false}).should.eql(del(base,["b1"]));
            $(base,{defaults:false}).should.eql(del(base,["b1","b2","b3","b4","b5","b6"]));
        });
        it("does not remove 0,false,'' if only null option",function(){

            $({a:""}).should.eql({a:""});
            $({a:""},{nulls:false}).should.eql({a:""});

        });
        it("can eval functions",function(){
            $({a:1,b:function(){return this.a*20}},{evalfunctions:true}).should.eql({a:1,b:20});
        });
        it("can remove privates",function(){
            $({a:1,__a:1},{privates:false}).should.eql({a:1});
        });
        it("can stringify internals",function(){
            $({a:1,b:{x:1}}).should.eql({a:1,b:{x:1}});
            $({a:1,b:{x:1}},{stringify:true}).should.eql({a:1,b:"{\"x\":1}"});
        });
        it("can stringify functions",function(){
            var s =  $({a:1,b:function(){}},{stringify:true}); //fix for IE toString
            s.a.should.equal(1);
            s.b.replace(/\s+/g,"").should.equal("function(){}");
        });
        it("can call interpolations if the-interpolation loaded",function(){
            $({a:1,b:"${a}${a}"},{interpolate:true}).should.eql({a:1,b:"11"});
        });
        it("can both stringify and interpolate if the-interpolation loaded",function(){
            $({a:1,b:{x:"${a}${a}"}},{interpolate:true,stringify:true}).should.eql({a:1,b:"{\"x\":\"11\"}"});
        });
        it("can filter options",function(){
            $({a:1,b:2,c:3},{filter:function(_,val){return _!="b" && val!=3}}).should.eql({a:1});
        });
        it("can filter options (with expression module)",function(){
            $({a:1,b:2,c:3},{filter:"_!='b' && arguments[1]!=3"}).should.eql({a:1});
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
                requirejs(["./lib/chai", "../the-jsonify","../the-interpolation","../the-expression"], function ($should, $the) {
                    should = $should.Should();
                    $ = $the.jsonify;
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
/**
 * Created by comdiv on 24.09.14.
 */
(function(describe,it,before){
describe("the.object", function(){
    this.timeout(5000);
    var should =null;
    var $ = null;


    describe("#extend",function(){
         it("'usual' mode",function(){
             $.extend({x:1,y:2,z:{a:1, y:{u:3}}},{x:3,z:{b:2, y:{w:4}}}).should.eql({ x: 3, y: 2, z: { b: 2, y:{w:4}} });
         });
        it("'deep' mode",function(){
            $.extend({x:1,y:2,z:{a:1, y:{u:3}}},{x:3,z:{b:2, y:{w:4}}},{deep:true,extensions:true}).should.eql({ x: 3, y: 2, z: { a:1, b: 2, y:{u:3,w:4} } });
        });

        it("'ignoreCase' mode",function(){
            $.extend({x:1,y:2,z:{a:1, y:{u:3}}},{X:3,Z:{B:2, Y:{W:4}}},{ignoreCase:true,deep:true,extensions:true}).should.eql({ x: 3, y: 2, z: { a:1, B: 2, y:{u:3,W:4} } });
        });
        it("'clone' mode",function(){
            var x = {x:1};
            var x_ = $.extend(x,{y:2});
            x_.should.eql({x:1,y:2});
            x.should.eql({x:1,y:2});
            x_.should.equal(x);

            x = {x:1};
            var x_ = $.extend(x,{y:2},{clone:true,extensions:true});
            x_.should.eql({x:1,y:2});
            x.should.not.eql({x:1,y:2});
            x_.should.not.equal(x);
        });
        it("'deep / cloneInternals ' mode",function(){
            var x = {x:{a:1}};
            var oldx = x.x;
            $.extend(x,{x:{b:2}},{deep:true});
            x.should.eql({x:{a:1,b:2}});
            x.x.should.equal(oldx);

            x = {x:{a:1}};
            oldx = x.x;
            $.extend(x,{x:{b:2}},{deep:true,cloneInternals:true});
            x.should.eql({x:{a:1,b:2}});
            x.x.should.eql({a:1,b:2});
            oldx.should.eql({a:1});
            x.x.should.not.equal(oldx);
        });
        it("filter mode",function(){
           var x= {a:1,b:2};
            $.extend(x,{c:3,d:4}).should.eql({a:1,b:2,c:3,d:4});
            x = {a:1,b:2};
            $.extend(x,{c:3,d:4},{filter:function(_){return _!="c";}}).should.eql({a:1,b:2,d:4});

        });
        it("no funciton mode",function(){
            var x= {a:1};
            should.exist($.extend(x,{b:function(){}})["b"]);
            x = {a:1};
            should.not.exist($.extend(x,{b:function(){}},{functions:false})["b"]);
        });
    });

    describe("#cast",function(){
        var t = function(a,b){this.a=a, this.b = b};
       it("can cast from array",function(){
           var x = $.cast(t,[1,2]);
           $.cast(t,[1,2]).should.instanceOf(t);
           $.cast(t,[1,2]).should.eql({a:1,b:2});
       }) ;
        it("can cast from object strong type (default)",function(){
            $.cast(t,{A:1,B:2,C:3}).should.instanceOf(t);
            $.cast(t,{A:1,B:2,C:3}).should.eql({a:1,b:2});
        }) ;

        it("can cast from object with extensions",function(){
            $.cast(t,{A:1,B:2,C:3}, $.ExtendOptions.ExtendedCast).should.instanceOf(t);
            $.cast(t,{A:1,B:2,C:3}, $.ExtendOptions.ExtendedCast).should.eql({a:1,b:2,C:3});
        }) ;
        it("knows parse instance method",function(){
            var t = function(a,b){this.a=a;this.b=b;}
            t.prototype.__parse__=function(s){this.a= Number(s.match(/\d/g)[0]),this.b= Number(s.match(/\d/g)[1]);}
            $.cast(t,"12").should.instanceOf(t);
            $.cast(t,"12").a.should.equal(1);
            $.cast(t,"12").b.should.equal(2);
        }) ;
        it("knows parse static method",function(){
            var t = function(a,b){this.a=a;this.b=b;}
            t.__parse__ = function(s){this.a= Number(s.match(/\d/g)[0]),this.b= Number(s.match(/\d/g)[1]);}
            $.cast(t,"12").should.instanceOf(t);
            $.cast(t,"12").should.eql({a:1,b:2});
        }) ;
    });
    describe("#clone",function(){
        it("clones objects",function(){
            $.clone({a:1,c:{d:1}}).should.eql({a:1,c:{d:1}});
        });
        it("clones arrays",function(){
            $.clone([1,2]).should.eql([1,2]);
            var x = [1,2];
            var y = $.clone(x);
            x.should.not.equal(y);
        });
        it("clones arrays in objects",function(){
            $.clone({x:[1,2]}).should.eql({x:[1,2]});
            var x = {x:[1,2]};
            var y = $.clone(x);
            x.x.should.not.equal(y.x);
        });
    });
    it("#isUserObject",function(){
            var f = function(){};
            $.isUserObject({}).should.equal(true);
            $.isUserObject({x:1}).should.equal(true);
            $.isUserObject(new f()).should.equal(true);
            $.isUserObject(/x/).should.equal(false);
            $.isUserObject(new Date()).should.equal(false);
            $.isUserObject("2").should.equal(false);
            $.isUserObject(2).should.equal(false);
            $.isUserObject(null).should.equal(false);

    });
    before (function(done){
        var requirejs= null;
        try{
            if(!!define){ //cause exception
                requirejs = require;
            }
        }catch(e){
            requirejs = require("requirejs");
            requirejs.config({baseurbaseUrl: '.', nodeRequire: require});
        }
        try{
            requirejs(["chai","../the-object"], function($should,$the){
                should = $should.Should();
                $ = $the.object;
                done();
              });
        }catch(e){
            console.log(e);
            done();
        }
    });





});
})(describe,it,before);
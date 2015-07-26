/**
 * Created by comdiv on 24.09.14.
 */
define(["the","chai"],function($the,chai){
    var $ = $the.object;
    var should= chai.Should();
    describe("the.object", function () {
        this.timeout(5000);

        describe("#extend", function () {
            it("'usual' mode", function () {
                $.extend({x: 1, y: 2, z: {a: 1, y: {u: 3}}}, {x: 3, z: {b: 2, y: {w: 4}}}).should.eql({ x: 3, y: 2, z: { b: 2, y: {w: 4}} });
            });

            it("'preservefalse' mode",function(){
                $.extend({x:false,y:true},{x:true,y:false},{preservefalse:true}).should.eql({x:false,y:false});
                $.extend({x:false,y:true},{x:true,y:false}).should.eql({x:true,y:false});
            });



            it("'deep' mode", function () {
                $.extend({x: 1, y: 2, z: {a: 1, y: {u: 3}}}, {x: 3, z: {b: 2, y: {w: 4}}}, {deep: true, extensions: true}).should.eql({ x: 3, y: 2, z: { a: 1, b: 2, y: {u: 3, w: 4} } });
            });

            it("'ignoreCase' mode", function () {
                $.extend({x: 1, y: 2, z: {a: 1, y: {u: 3}}}, {X: 3, Z: {B: 2, Y: {W: 4}}}, {ignoreCase: true, deep: true, extensions: true}).should.eql({ x: 3, y: 2, z: { a: 1, B: 2, y: {u: 3, W: 4} } });
            });
            it("'clone' mode", function () {
                var x = {x: 1};
                var x_ = $.extend(x, {y: 2});
                x_.should.eql({x: 1, y: 2});
                x.should.eql({x: 1, y: 2});
                x_.should.equal(x);

                x = {x: 1};
                var x_ = $.extend(x, {y: 2}, {clone: true, extensions: true});
                x_.should.eql({x: 1, y: 2});
                x.should.not.eql({x: 1, y: 2});
                x_.should.not.equal(x);
            });
            it("'deep / cloneInternals ' mode", function () {
                var x = {x: {a: 1}};
                var oldx = x.x;
                $.extend(x, {x: {b: 2}}, {deep: true});
                x.should.eql({x: {a: 1, b: 2}});
                x.x.should.equal(oldx);

                x = {x: {a: 1}};
                oldx = x.x;
                $.extend(x, {x: {b: 2}}, {deep: true, cloneInternals: true});
                x.should.eql({x: {a: 1, b: 2}});
                x.x.should.eql({a: 1, b: 2});
                oldx.should.eql({a: 1});
                x.x.should.not.equal(oldx);
            });
            it("filter mode", function () {
                var x = {a: 1, b: 2};
                $.extend(x, {c: 3, d: 4}).should.eql({a: 1, b: 2, c: 3, d: 4});
                x = {a: 1, b: 2};
                $.extend(x, {c: 3, d: 4}, {filter: function (_) {
                    return _ != "c";
                }}).should.eql({a: 1, b: 2, d: 4});

            });
            it("no funciton mode", function () {
                var x = {a: 1};
                should.exist($.extend(x, {b: function () {
                }})["b"]);
                x = {a: 1};
                should.not.exist($.extend(x, {b: function () {
                }}, {functions: false})["b"]);
            });

        });

        describe("#hahing",function(){
           it("#toHex",function(){
               $.toHex(0).should.equal("00000000");
               $.toHex(1).should.equal("00000001");
               $.toHex(2).should.equal("00000002");
               $.toHex(3).should.equal("00000003");
               $.toHex(4).should.equal("00000004");
               $.toHex(5).should.equal("00000005");
               $.toHex(6).should.equal("00000006");
               $.toHex(8).should.equal("00000008");
               $.toHex(9).should.equal("00000009");
               $.toHex(10).should.equal("0000000a");
               $.toHex(11).should.equal("0000000b");
               $.toHex(12).should.equal("0000000c");
               $.toHex(13).should.equal("0000000d");
               $.toHex(14).should.equal("0000000e");
               $.toHex(15).should.equal("0000000f");
               $.toHex(16).should.equal("00000010");
               $.toHex(17).should.equal("00000011");
               $.toHex(18).should.equal("00000012");
               $.toHex(19).should.equal("00000013");
               $.toHex(256).should.equal("00000100")
               $.toHex(257).should.equal("00000101")
           }) ;
            it("#fixedOrder",function(){
                    var x1 = {b:2,c:3,a:1};
                    var x2 = {c:3,a:1,b:2};
                    var fx1 = $.fixedOrder(x1);
                    var fx2 = $.fixedOrder(x2);
                    var js1 = JSON.stringify(fx1);
                    var js2 = JSON.stringify(fx2);
                    js1.should.equal(js2);
                    js1.should.equal(JSON.stringify({a:1,b:2,c:3}));
                });
            it("#equal",function(){
                $.equal({a:1,b:2,c:function(){return 1;}},{b:2,a:1,c:function(){return 2;}}).should.equal(true);
                $.equal({a:1,b:2,c:function(){return 1;}},{b:2,a:2,c:function(){return 2;}}).should.equal(false);
            })
            it("#digest",function(){
                $.digest(null).should.equal("000be10033f0acaa");
                $.digest({x:1}).should.equal("f71e5fb5ed016ee9");
                $.digest({x:1,y:2}).should.equal("0fdf544a5a8163f7");
                $.digest({y:2,x:1}).should.equal("0fdf544a5a8163f7");
                $.digest({x:2}).should.equal("0eaebfb5ec846ee8");
                $.digest({x:2,y:function(){}}).should.equal("0eaebfb5ec846ee8");
            });
        });

        describe("#propertise",function(){
           it("can bind properties",function(){
               var obj = {
                   _a  : 2,
                   _b  : 3,
                   a : {get:function(){return this._a*2},set:function(_){this._a = _;}},
                   b : {get:function(){return this._b*3},set:function(_){this._b = _;}}
               }
               $.propertise(obj);
               obj.a.should.equal(4);
               obj.b.should.equal(9);
               obj.a = 1;
               obj.a.should.equal(2);
           }) ;
            it("can bind properties (prototype)",function(){
                var cls = function(){
                    this._a =2;
                    this._b =3;
                }
                cls.prototype.a ={get:function(){return this._a*2},set:function(_){this._a = _;}};
                cls.prototype.b ={get:function(){return this._b*3},set:function(_){this._b = _;}};
                $.propertise(cls.prototype);
                var obj = new cls();
                obj.a.should.equal(4);
                obj.b.should.equal(9);
                obj.a = 1;
                obj.a.should.equal(2);
            }) ;
        });

        describe("#cast", function () {
            var t = function (a, b) {
                this.a = a, this.b = b
            };
            it("can cast from array", function () {
                var x = $.cast(t, [1, 2]);
                $.cast(t, [1, 2]).should.instanceOf(t);
                $.cast(t, [1, 2]).should.eql({a: 1, b: 2});
            });
            it("can cast with functions",function(){
               var f = function(){
                   this.a = null;
                   this.onKey = null;
               };
               var r = $.cast(f,{a:{c:1},onKey:function(){return this.a.c}});
                r.onKey().should.equal(1);
            });
            it("can use extend in .ctor",function(){
                var f = function(opts){
                    this.a = null;
                    this.onKey = null;
                    $.extend(this,opts);
                };
                var r = new f({a:{c:1},onKey:function(){return this.a.c}});
                r.onKey().should.equal(1);
            });
            it("can cast from object strong type (default)", function () {
                $.cast(t, {A: 1, B: 2, C: 3}).should.instanceOf(t);
                $.cast(t, {A: 1, B: 2, C: 3}).should.eql({a: 1, b: 2});
            });

            it("can cast from object with extensions", function () {
                $.cast(t, {A: 1, B: 2, C: 3}, $.ExtendOptions.ExtendedCast).should.instanceOf(t);
                $.cast(t, {A: 1, B: 2, C: 3}, $.ExtendOptions.ExtendedCast).should.eql({a: 1, b: 2, C: 3});
            });
            it("knows parse instance method", function () {
                var t = function (a, b) {
                    this.a = a;
                    this.b = b;
                }
                t.prototype.__parse__ = function (s) {
                    this.a = Number(s.match(/\d/g)[0]), this.b = Number(s.match(/\d/g)[1]);
                }
                $.cast(t, "12").should.instanceOf(t);
                $.cast(t, "12").a.should.equal(1);
                $.cast(t, "12").b.should.equal(2);
            });
            it("knows parse static method", function () {
                var t = function (a, b) {
                    this.a = a;
                    this.b = b;
                }
                t.__parse__ = function (s) {
                    this.a = Number(s.match(/\d/g)[0]), this.b = Number(s.match(/\d/g)[1]);
                }
                $.cast(t, "12").should.instanceOf(t);
                $.cast(t, "12").should.eql({a: 1, b: 2});
            });
        });
        describe("#clone", function () {
            it("clones objects", function () {
                $.clone({a: 1, c: {d: 1}}).should.eql({a: 1, c: {d: 1}});
            });
            it("clones arrays", function () {
                $.clone([1, 2]).should.eql([1, 2]);
                var x = [1, 2];
                var y = $.clone(x);
                x.should.not.equal(y);
            });
            it("clones arrays in objects", function () {
                $.clone({x: [1, 2]}).should.eql({x: [1, 2]});
                var x = {x: [1, 2]};
                var y = $.clone(x);
                x.x.should.not.equal(y.x);
            });
        });
        it("#isUserObject", function () {
            var f = function () {
            };
            $.isUserObject({}).should.equal(true);
            $.isUserObject({x: 1}).should.equal(true);
            $.isUserObject(new f()).should.equal(true);
            $.isUserObject(/x/).should.equal(false);
            $.isUserObject(new Date()).should.equal(false);
            $.isUserObject("2").should.equal(false);
            $.isUserObject(2).should.equal(false);
            $.isUserObject(null).should.equal(false);

        });
    });
});
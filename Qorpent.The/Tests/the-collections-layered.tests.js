/**
 * Created by comdiv on 24.09.14.
 */
(function (describe, it, before) {
    describe("the.collections.LayeredDictionary", function () {
        this.timeout(5000);
        it("will be treated as enumeration",function(){
            new $().should.instanceOf($e);
        });
        it("can be used as Enumeration",function(){
            new $({x:1,y:2},{y:3,z:4}).toArray().should.eql([
                {
                    "key": "x",
                    "value": 1
                },
                {
                    "key": "y",
                    "value": 3
                },
                {
                    "key": "z",
                    "value": 4
                }
            ]);

        });
        it("can set values", function(){
            var c = new $();
            should.not.exist(c.index["x"]);
            c.set('x',1);
            c.internalIndex["x"].should.equal(1);
        });
        it("can get values", function(){
            var c = new $();
            should.not.exist(c.get("x"));
            c.set('x',1);
            c.get('x').should.equal(1);
        });
        it("can remove values", function(){
            var c = new $();
            should.not.exist(c.get("x"));
            c.set('x',1);
            c.get('x').should.equal(1);
            c.set('x',undefined);
            c.exists('x').should.equal(false);
        });
        it("remove accepts only one level", function(){
            var c = new $();
            should.not.exist(c.get("x"));
            c.set('x',1);
            c.get('x').should.equal(1);
            c.parent = new $({x:2});
            c.get('x').should.equal(1);
            c.set('x',undefined);
            c.exists('x').should.equal(true);
            c.get('x').should.equal(2);
        });
        it("can check existence", function(){
            var c = new $();
            c.exists("x").should.equal(false);
            c.set('x',1);
            c.exists("x").should.equal(true);
        });
        it("can get values hierarchically", function(){
            var c = new $();
            c.parent = new $();
            c.parent.parent = new $();
            should.not.exist(c.get("x"));
            c.parent.parent.set('x',1);
            c.get('x').should.equal(1);
        });
        it("can override values hierarchically", function(){
            var c = new $();
            c.parent = new $();
            c.parent.parent = new $();
            should.not.exist(c.get("x"));
            c.parent.parent.set('x',1);
            c.get('x').should.equal(1);
            c.parent.set('x',2);
            c.get('x').should.equal(2);
        });
        it("can check existence hierarchically", function(){
            var c = new $();
            c.parent = new $();
            c.parent.parent = new $();
            c.exists("x").should.equal(false);
            c.parent.parent.set('x',1);
            c.exists("x").should.equal(true);
        });
        it("can get key set",function(){
            var c = new $();
            c.parent = new $();
            c.parent.parent = new $();
            c.parent.parent.set("x",1);
            c.parent.parent.set("y",1);
            c.parent.parent.set("z",1);
            c.parent.set("x",2);
            c.parent.set("y",2);
            c.parent.set("a",2);
            c.set("x",3);
            c.set("a",3);
            c.set("b",3);
            c.getKeys().should.eql(["x","a","b","y","z"]);
        });
        it("can set from objects", function(){
           var c = new $({x:1,y:1,z:1},{x:2,a:2},{y:3,b:3});
            c.get('x').should.equal(2);
            c.get('y').should.equal(3);
            c.get('z').should.equal(1);
            c.get('a').should.equal(2);
            c.get('b').should.equal(3);
        });
        it("can set from other configs", function(){
            var cbase = new $({x:1,y:1,z:1});
            var c = new $(cbase,{x:2,a:2},{y:3,b:3});
            c.get('x').should.equal(2);
            c.get('y').should.equal(3);
            c.get('z').should.equal(1);
            c.get('a').should.equal(2);
            c.get('b').should.equal(3);
        });

        it("can can get from certain depth", function(){
            var cb1 = new $({x:1,a:1});
            var cb2 = new $({x:2,b:2});
            var cb3 = new $({x:3,c:3});
            var c = new $(cb1,cb2,cb3,{x:4,d:4});

            c.get('x').should.equal(4);
            c.get('x',{mindepth:1}).should.equal(3);
            c.get('x',{mindepth:2}).should.equal(2);
            c.get('x',{mindepth:3}).should.equal(1);

            c.get('d').should.equal(4);
            c.get('d',{maxdepth:5}).should.equal(4);
            should.not.exist(c.get('d',[1,1]));

            c.get("a").should.equal(1);
            c.get("a",{mindepth:1}).should.equal(1);
            c.get("a",{mindepth:1,maxdepth:5}).should.equal(1);
            should.not.exist(c.get("a",{mindepth:5}));
            should.not.exist(c.get("a",{mindepth:4,maxdepth:5}));
        });

        it("can can be converted to plain object", function(){
            var cb1 = new $({x:1,a:1});
            var cb2 = new $({x:2,b:2});
            var cb3 = new $({x:3,c:3});
            var c = new $(cb1,cb2,cb3,{x:4,d:4});
            c.toObject().should.eql({x:4,d:4,c:3,b:2,a:1});
        });

        it("can be built from chain", function () {
            var c = $.build({x: 1, y: 1, z: 1}, {x: 2, a: 2}, {y: 3, b: 3});
            c.get('x').should.equal(2);
            c.get('y').should.equal(3);
            c.get('z').should.equal(1);
            c.get('a').should.equal(2);
            c.get('b').should.equal(3);
            c.get('x', 1).should.equal(2);
            c.get('x', 2).should.equal(1);
            var c = $({x: 1, y: 1, z: 1}, {x: 2, a: 2}, {y: 3, b: 3}); //not "new" style
            c.get('x').should.equal(2);
            c.get('y').should.equal(3);
            c.get('z').should.equal(1);
            c.get('a').should.equal(2);
            c.get('b').should.equal(3);
            c.get('x', 1).should.equal(2);
            c.get('x', 2).should.equal(1);
        });


        var $ = null;
        var $e = null;
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
                requirejs(["chai", "../the-collections-layered"], function ($should, $the) {
                    should = $should.Should();
                    $ = $the.collections.LayeredDictionary;
                    $e = $the.collections.Enumeration;
                    done();
                });
            } catch (e) {
                console.log(e);
                done();
            }
        });


    });
})(describe, it, before);
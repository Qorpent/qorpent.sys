/**
 * Created by comdiv on 24.09.14.
 */
(function(describe,it,before){
describe("the.collections.core", function(){
    this.timeout(5000);
    var should =null;
    var $ = null;
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
            requirejs(["chai","../the-collections-core"], function($should,$thenum){
                should = $should.Should();
                $ = $thenum.collections;
                done();
              });
        }catch(e){
            console.log(e);
            done();
        }
    });


     describe ("can creates enumerations",function(){
       it("from arrays",function(){
           $([]).should.instanceOf($.Enumeration);
       });
        it("from objects",function(){
            $({}).should.instanceOf($.Enumeration);
        });
        it("from strings",function(){
            $("").should.instanceOf($.Enumeration);
        });
        it("from numbers",function(){
            $(10).should.instanceOf($.Enumeration);
        });
        it("from nulls",function(){
            $(null).should.instanceOf($.Enumeration);
        });
        it("always new...",function(){
           var a  = [];
            var e = $(a);
            var e2 = $(a);
            e2.should.not.equal(e);
        });
        it("... except proxize enumerations",function(){
            var e = $([]);
            var e2 = $(e);
            e2.should.equal(e);
        });

    });

    describe("has appropriate api",function(){
        it("has next",function(){
            var e = $([]);
            e.next.should.a("function");
        })
    });

    it("track state of next() and current value",function(){
        var i = $([1,3]);
        i.current.should.equal($.StartOfEnumeration);
        i.next().should.equal(true);
        i.current.should.equal(1);
        i.next().should.equal(true);
        i.current.should.equal(3);

        i.next().should.equal(false);
        i.current.should.equal($.EndOfEnumeration);

        i.next().should.equal(false);
        i.current.should.equal($.EndOfEnumeration);

    });

    try{
        if(typeof Iterator!=="undefined"){
            it("can be used with native iterator (only under Javascript 1.8)",function(){

                var i = $({x:1,y:2});
                var res = [];
                for(var item in i){
                    if(i.hasOwnProperty(item)){
                        res.push(item);
                    }
                }
                res.should.eql([ { key: 'x', value: 1 }, { key: 'y', value: 2 } ]);
            });
        }

    }catch(e){
        it("can be used with native iterator (only under Javascript 1.8)");
    }



    describe("can iterates over",function(){

        it("arrays",function(){
            var i = $([1,2]);
            var res = [];
            while(i.next()){
                res.push(i.current);
            }
            res.should.eql([1,2]);
        });
        it("strings",function(){
            var i = $("abc");
            var res = [];
            while(i.next()){
                res.push(i.current);
            }
            res.should.eql(["a","b","c"]);
        });
        it("objects",function(){
            var i = $({a:1,b:2});
            var res = [];
            while(i.next()){
                res.push(i.current);
            }
            res.should.eql([{key:"a",value:1},{key:"b",value:2}]);
            res[0].should.instanceOf($.KeyValuePair);
        });
        it("numbers",function(){
            var i = $(3);
            var res = [];
            while(i.next()){
                res.push(i.current);
            }
            res.should.eql([0,1,2]);
        });
        it ("can be chained",function(){
            var ibase = $([1,2]);
            var i = new $.Enumeration(ibase);
            var r = [];
            while(i.next())r.push(i.current);
            r.should.eql([1,2]);
        } );
    });
    describe("reset support",function(){
        it("can be reseted itself",function(){
            var i = $([1,2]);
            var r = [];
            while(i.next())r.push(i.current);
            i.reset();
            while(i.next())r.push(i.current);
            r.should.eql([1,2,1,2]);
        });
        it("can be reseted chained",function(){
            var ibase = $([1,2]);
            var i = new $.Enumeration(ibase);
            var r = [];
            while(i.next())r.push(i.current);
            i.reset();
            while(i.next())r.push(i.current);
            r.should.eql([1,2,1,2]);
        });
    });

});
})(describe,it,before);
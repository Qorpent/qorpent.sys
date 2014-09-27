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
            requirejs(["chai","../thenum"], function($should,$thenum){
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

    describe("is LINQ like",function(){
        it("supports any([condition])",function(){
            var i = $([1,2,3]);
            i.any().should.equal(true);
            i.any(">2").should.equal(true);
            i.any(">3").should.equal(false);
            i.any(function(_){return _==1}).should.equal(true);
        });
        it("supports toArray([condition])",function(){
            var i = $(4);
            i.toArray().should.eql([0,1,2,3]);
            i.toArray(">1").should.eql([2,3]);
        });
        it("supports count([condition])",function(){
           var i =  $(4);
            i.count().should.equal(4);
            i.count(">1").should.equal(2);
        });
        it("supports first([condition])",function(){
            var i =  $(4);
            i.first().should.equal(0);
            i.first(">1").should.equal(2);
            (function(){
                i.first(">5");
            }).should.throw();
        });
        it("supports firstOrDefault([condition],[defaultValue])",function(){
            var i =  $(4);
            i.firstOrDefault().should.equal(0);
            i.firstOrDefault(">1").should.equal(2);
            should.not.exist(i.firstOrDefault(">4"));
            i.firstOrDefault(">4",10).should.equal(10);
        });
        it("supports where(condition)",function(){
            var i =  $(4).where(">1");
            i.any().should.equal(true);
            i.count().should.equal(2);
            i.toArray().should.eql([2,3]);
        });
        it("supports union(enum)",function(){
            var i =  $(4).union(3);
            i.toArray().should.eql([0,1,2,3,0,1,2]);
            i.any().should.equal(true);
            i.count().should.equal(7);
        });
        it("supports select(projection)",function(){
            var i = $(4).select("*2");
            i.toArray().should.eql([0,2,4,6]);
        });
        it("supports distinct([keyfunc])",function(){
            var i =  $([1,2,3,1,2,5]).distinct();
            i.toArray().should.eql([1,2,3,5]);
            i =  $([1,2,3,1,2,5]).distinct("_==2||_==5?1:_");
            i.toArray().should.eql([1,3]);
        });
        it("supports except(enum, [keyfunc])",function(){
            var i =  $([1,2,3,1,2,5]).except([1,5]);
            i.toArray().should.eql([2,3,2]);
            i =  $([1,2,3,1,2,5]).except([1,5],"_==2?5:_");
            i.toArray().should.eql([3]);
        });
        it("supports reverse()",function(){
            var i =  $([1,2,3]).reverse();
            i.toArray().should.eql([3,2,1]);
        });
        it("supports skip(numOrCondition)",function(){
            var i =  $([0,1,2,3,4]).skip(2);
            i.toArray().should.eql([2,3,4]);
            i =  $([0,1,2,3,4]).skip("!=2");
            i.toArray().should.eql([2,3,4]);
        });
        it("supports take(numOrCondition)",function(){
            var i =  $([0,1,2,3,4]).take(2);
            i.toArray().should.eql([0,1]);
            i =  $([0,1,2,3,4]).take("!=2");
            i.toArray().should.eql([0,1]);
        });

        it("supports each(func)",function(){
            var i =  $([0,1,2,3,4]);
            var res = [];
            i.each(function(_){
                if(_==3)return false;
                if(0==(_%2))res.push(_);
                return true;
            });
            res.should.eql([0,2]);
        });
        it("supports order([keyOrCompare])",function(){
            var i =  $([2,0,1,4,3]).order();
            i.toArray().should.eql([0,1,2,3,4]);
            i =  $([2,0,1,4,3]).order("-_");
            i.toArray().should.eql([4,3,2,1,0]);
            i =  $([2,0,1,4,3]).order(function(_){
                if(0==(_%2))return (_+1)*10;
                return _;
            });
            i.toArray().should.eql([1,3,0,2,4]);
            i =  $([2,0,1,4,3]).order(function(a,b){
                if(a==3 && b==4)return 1;
                if(a==4 && b==3)return -1;
                return a-b;
            });
            i.toArray().should.eql([0,1,2,4,3]);
        });


    });


});
})(describe,it,before);
/**
 * Created by comdiv on 24.09.14.
 */
(function(describe,it,before){
describe("the.collections.linq", function(){
    this.timeout(5000);




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
        if(typeof  window !=="undefined" && !!window.phantomCallback){
            it("supports order([keyOrCompare]) - SOME PHANTOM ISSUES")
        }else{
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
        }

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
                requirejs(["./lib/chai","../the-collections-linq","../the-expression"], function($should,$thenum){
                    should = $should.Should();
                    $ = $thenum.collections;
                    done();
                });
            }catch(e){
                console.log(e);
                done();
            }
        });

    });


});
})(describe,it,before);
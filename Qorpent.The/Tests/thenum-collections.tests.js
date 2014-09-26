/**
 * Created by comdiv on 24.09.14.
 */
describe("Thenum.Collections", function(){
    this.timeout(5000);
    describe("Extensions",function(){
       [
           ["toList", "LinkedList"],
           ["toStack", "Stack"],
           ["toQueue", "Queue"],
           ["toCycle", "CycleList"]]
           .forEach(function(pair){
           it("Enumeration#"+pair[0],function(){
               should.exist($.Enumeration.prototype[pair[0]]);
               $()[pair[0]]().should.be.instanceOf($k[pair[1]]);
           });
       }) ;
    });
    describe ("Linkedlist",function(){
        it("can be instantiated",function(){
            var ll = new $k.LinkedList();
        });
        it("can is an enumeration",function(){
            var ll = new $k.LinkedList();
            ll.should.be.instanceOf($.Enumeration);
            ll.any().should.be.false;
        });
        it("keeps real linked list",function(){
            var ll = new $k.LinkedList();
            ll.append(1);
            ll.append(2);
            ll.any().should.be.true;
            ll.count().should.be.equal(2);
            ll.toArray().should.be.eql([1,2]);
            ll.first.should.be.instanceOf($k.LinkedItem);
            ll.first.value.should.be.equal(1);
            ll.first.next.value.should.be.equal(2);
            ll.last.should.be.instanceOf($k.LinkedItem);
            ll.last.value.should.be.equal(2);
        });
        it("#append(any)",function(){
           var ll = new $k.LinkedList();
            ll.append(1);
            ll.first.value.should.be.equal(1);
            ll.last.value.should.be.equal(1);
            ll.append(2);
            ll.first.next.value.should.be.equal(2);
            ll.last.value.should.be.equal(2);
            should.not.exist(ll.first.previous);
            should.not.exist(ll.last.next);
            ll.last.previous.should.be.equal(ll.first);
        });
        it("#prepend(any)",function(){
            var ll = new $k.LinkedList();
            ll.prepend(1);
            ll.first.value.should.be.equal(1);
            ll.last.value.should.be.equal(1);
            ll.prepend(2);
            ll.first.next.value.should.be.equal(1);
            ll.last.value.should.be.equal(1);
            ll.first.value.should.be.equal(2);
            should.not.exist(ll.first.previous);
            should.not.exist(ll.last.next);
            ll.last.previous.should.be.equal(ll.first);
        });
        it("#find (condition)",function(){
            var ll = new $k.LinkedList([1,2,3]);
            should.exist(ll.find(1));
            should.exist(ll.find(">2"));
            should.exist(ll.find(2));
            should.exist(ll.find(3));
            should.exist(ll.find("_.next.value==3"));
            should.not.exist(ll.find(4));
            should.not.exist(ll.find(">3"));
            should.not.exist(ll.find("_.next.value==4"));
        });
        it("#insertBefore(any,[key])",function(){
            var ll = new $k.LinkedList([1,2,3]);
            ll.insertBefore(4,2);
            ll.insertBefore(5,3);
            ll.count().should.be.equal(5);
            ll.toArray().should.be.eql([1,4,2,5,3]);
            ll.insertBefore(6,1);
            ll.count().should.be.equal(6);
            ll.toArray().should.be.eql([6,1,4,2,5,3]);
        });
        it("#insertAfter(any,[key])",function(){
            var ll = new $k.LinkedList([1,2,3]);
            ll.insertAfter(4,2);
            ll.insertAfter(5,3);
            ll.count().should.be.equal(5);
            ll.toArray().should.be.eql([1,2,4,3,5]);
            ll.insertAfter(6,1);
            ll.count().should.be.equal(6);
            ll.toArray().should.be.eql([1,6,2,4,3,5]);
            ll.insertAfter(7,">2");
            ll.toArray().should.be.eql([1,6,7,2,4,3,5]);
        });
        it("#replace(any,conditionOrItem))" , function(){
            var ll = new $k.LinkedList([1,3,2,3]);
            ll.replace(4,"_.next.value==3");
            ll.replace(5,"_.next.value==3 && _.previous.value==3");
            ll.toArray().should.be.eql([4,3,5,3]);
        });
        it("#remove(conditionOrItem))" , function(){
            var ll = new $k.LinkedList([1,3,2,3]);
            ll.remove("_.next.value==3");
            ll.remove("_.next.value==3 && _.previous.value==3");
            ll.toArray().should.be.eql([3,3]);
        });

        it("can be instantiated over other enumeration",function(){
           var ll = new $k.LinkedList([1,2,3]);
            ll.any().should.be.true;
            ll.count().should.be.equal(3);
            ll.toArray().should.be.eql([1,2,3]);

        });
        it("can be keyed",function(){
            var ll = new $k.LinkedList([1,2,3],{onKey:"*2"});
            ll.find(2).value.should.be.equal(1);
            ll.find(4).value.should.be.equal(2);
            ll.find(6).value.should.be.equal(3);
        });
        it("can merge",function(){
            var ll = new $k.LinkedList([{k:"c1",x:2},{k:"c2", x:3}],{onKey:".k"});
            ll.merge([{k:"c1",x:2},{k:"c3",x:4}]).should.be.eql({ removed: [], created: [ { k: 'c3', x: 4 } ], updated: [] });
            ll.toArray().should.be.eql([ { k: 'c1', x: 2 }, { k: 'c2', x: 3 }, { k: 'c3', x: 4 } ]);
            ll.merge([ { k: 'c1', x: 3 },  { k: 'c3', x: 5 } ],{replace:true}).should.be.eql({ removed: [],
                created: [],
                updated: [ { k: 'c1', x: 3 }, { k: 'c3', x: 5 } ] });
            ll.toArray().should.be.eql([ { k: 'c1', x: 3 }, { k: 'c2', x: 3 }, { k: 'c3', x: 5 } ]);
            ll.merge([ { k: 'c1', x: 4 },  { k: 'c3', x: 6 } ],{replace:false}).should.be.eql( { removed: [], created: [], updated: [] } );
            ll.toArray().should.be.eql([ { k: 'c1', x: 3 }, { k: 'c2', x: 3 }, { k: 'c3', x: 5 } ]);
            ll.merge([ { k: 'c1', x: 4 },  { k: 'c3', x: 6 } ],{replace:true,remove:true}).should.be.eql({ removed: [ { k: 'c2', x: 3 } ],
                created: [],
                updated: [ { k: 'c1', x: 4 }, { k: 'c3', x: 6 } ] } );
            ll.toArray().should.be.eql( [ { k: 'c1', x: 4 }, { k: 'c3', x: 6 } ] );
        });
    });

    describe("Stack",function(){
       it("creates collection in back order",function(){
           var stack = new $k.Stack([1,2,3]);
           stack.toArray().should.be.eql([3,2,1]);
       }) ;
        it("#pop",function(){
            var stack = new $k.Stack([1,2,3]);
            stack.pop().should.be.equal(3);
            stack.toArray().should.be.eql([2,1]);
            stack.pop().should.be.equal(2);
            stack.toArray().should.be.eql([1]);
            stack.pop().should.be.equal(1);
            stack.toArray().should.be.eql([]);
            should.not.exist(stack.pop());
        });
        it("#push",function(){
            var stack = new $k.Stack([1,2,3]);
            stack.pop().should.be.equal(3);
            stack.toArray().should.be.eql([2,1]);
            stack.push(4);
            stack.toArray().should.be.eql([4,2,1]);
            stack.pop().should.be.equal(4);
        });
    });

    describe("Queue",function(){
        it("#pop",function(){
            var queue = new $k.Queue([1,2,3]);
            queue.pop().should.be.equal(1);
            queue.toArray().should.be.eql([2,3]);
            queue.pop().should.be.equal(2);
            queue.toArray().should.be.eql([3]);
            queue.pop().should.be.equal(3);
            queue.toArray().should.be.eql([]);
            should.not.exist(queue.pop());
        });
        it("#push",function(){
            var queue = new $k.Queue([1,2,3]);
            queue.pop().should.be.equal(1);
            queue.toArray().should.be.eql([2,3]);
            queue.push(4);
            queue.toArray().should.be.eql([2,3,4]);
            queue.pop().should.be.equal(2);
            queue.pop().should.be.equal(3);
            queue.pop().should.be.equal(4);
        });
    });

    describe("CycleList",function(){
       it("repeat it's elements cycle by cycle",function(){
           var cl = new $k.CycleList([1,2,3]);
           cl.pop().should.be.equal(1);
           cl.pop().should.be.equal(2);
           cl.pop().should.be.equal(3);
           cl.pop().should.be.equal(1);
           cl.pop().should.be.equal(2);
           cl.pop().should.be.equal(3);
       });
        it("we can append elements to queue",function(){
            var cl = new $k.CycleList([1,2,3]);
            cl.pop().should.be.equal(1);
            cl.pop().should.be.equal(2);
            cl.insertAfter(4);
            cl.pop().should.be.equal(4);
            cl.pop().should.be.equal(3);
            cl.pop().should.be.equal(1);
            cl.pop().should.be.equal(2);
            cl.pop().should.be.equal(4);
            cl.pop().should.be.equal(3);
        });
        it("#goto(key,offset)",function(){
            var cl = new $k.CycleList([1,2,3,4,5,6]);
            cl.goto(4,-2);
            cl.pop().should.be.equal(3);
            cl.pop().should.be.equal(4);
            cl.pop().should.be.equal(5);
            cl.pop().should.be.equal(6);
            cl.pop().should.be.equal(1);
            cl.goto(5,3);
            cl.pop().should.be.equal(3);
            cl.pop().should.be.equal(4);
            cl.pop().should.be.equal(5);
            cl.pop().should.be.equal(6);
            cl.pop().should.be.equal(1);
        })
    });


    var $ = null;
    var $k = null;
    var should = null;
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
            requirejs(["../Dependency/chai","../thenum-collections","../thenum"], function($should,$kollection,$thenum){
                should = $should.Should();
                $k = $kollection;
                $ = $thenum;
                done();
            });
        }catch(e){
            console.log(e);
            done();
        }
    });







});
/**
 * Created by comdiv on 24.09.14.
 */
(function(describe,it,before){
describe("the.collections.Linked", function(){
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
               $()[pair[0]]().should.instanceOf($[pair[1]]);
           });
       }) ;
    });
    describe("Linkedlist",function(){
        it("can be instantiated",function(){
            new $.LinkedList();
        });
        it("can is an enumeration",function(){
            var ll = new $.LinkedList();
            ll.should.instanceOf($.Enumeration);
            ll.any().should.equal(false);
        });
        it("keeps real linked list",function(){
            var ll = new $.LinkedList();
            ll.append(1);
            ll.append(2);
            ll.any().should.equal(true);
            ll.count().should.equal(2);
            ll.toArray().should.eql([1,2]);
            ll.first.should.instanceOf($.LinkedItem);
            ll.first.value.should.equal(1);
            ll.first.next.value.should.equal(2);
            ll.last.should.instanceOf($.LinkedItem);
            ll.last.value.should.equal(2);
        });
        it("#append(any)",function(){
           var ll = new $.LinkedList();
            ll.append(1);
            ll.first.value.should.equal(1);
            ll.last.value.should.equal(1);
            ll.append(2);
            ll.first.next.value.should.equal(2);
            ll.last.value.should.equal(2);
            should.not.exist(ll.first.previous);
            should.not.exist(ll.last.next);
            ll.last.previous.should.equal(ll.first);
        });

         it("#prepend(any)",function(){
            var ll = new $.LinkedList();
            ll.prepend(1);
            ll.first.value.should.equal(1);
            ll.last.value.should.equal(1);
            ll.prepend(2);
            ll.first.next.value.should.equal(1);
            ll.last.value.should.equal(1);
            ll.first.value.should.equal(2);
            should.not.exist(ll.first.previous);
            should.not.exist(ll.last.next);
            ll.last.previous.should.equal(ll.first);
        });
        it("#find (condition)",function(){
            var ll = new $.LinkedList([1,2,3]);
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
            var ll = new $.LinkedList([1,2,3]);
            ll.insertBefore(4,2);
            ll.insertBefore(5,3);
            ll.count().should.equal(5);
            ll.toArray().should.eql([1,4,2,5,3]);
            ll.insertBefore(6,1);
            ll.count().should.equal(6);
            ll.toArray().should.eql([6,1,4,2,5,3]);
        });
        it("#insertAfter(any,[key])",function(){
            var ll = new $.LinkedList([1,2,3]);
            ll.insertAfter(4,2);
            ll.insertAfter(5,3);
            ll.count().should.equal(5);
            ll.toArray().should.eql([1,2,4,3,5]);
            ll.insertAfter(6,1);
            ll.count().should.equal(6);
            ll.toArray().should.eql([1,6,2,4,3,5]);
            ll.insertAfter(7,">2");
            ll.toArray().should.eql([1,6,7,2,4,3,5]);
        });
        it("#replace(any,conditionOrItem))" , function(){
            var ll = new $.LinkedList([1,3,2,3]);
            ll.replace(4,"_.next.value==3");
            ll.replace(5,"_.next.value==3 && _.previous.value==3");
            ll.toArray().should.eql([4,3,5,3]);
        });
         it("#remove(conditionOrItem))" , function(){
            var ll = new $.LinkedList([1,3,2,3]);
            ll.remove("_.next.value==3");
            ll.remove("_.next.value==3 && _.previous.value==3");
            ll.toArray().should.eql([3,3]);
        });

        it("item#index",function(){
            var ll = new $.LinkedList([1,2,3]);
            ll.first.index.should.equal(0);
            ll.first.next.index.should.equal(1);
            ll.first.next.next.index.should.equal(2);

        });
         it("#indexOf",function(){
            var ll = new $.LinkedList([3,2,1]);
            ll.indexOf(3).should.equal(0);
            ll.indexOf(2).should.equal(1);
            ll.indexOf(1).should.equal(2);
        });
        it("#itemByIndex",function(){
            var ll = new $.LinkedList([3,2,1]);
            ll.itemByIndex(0).value.should.equal(3);
            ll.itemByIndex(1).value.should.equal(2);
            ll.itemByIndex(2).value.should.equal(1);

        });
        it("#byIndex",function(){
            var ll = new $.LinkedList([3,2,1]);
            ll.byIndex(0).should.equal(3);
            ll.byIndex(1).should.equal(2);
            ll.byIndex(2).should.equal(1);

        });
         it("#currentIndex",function(){
            var ll = new $.LinkedList([3,2,1]);
            ll.byIndex(0).should.equal(3);
            ll.currentIndex.should.equal(-1);
            ll.next();
            ll.currentIndex.should.equal(0);
            ll.currentIndex = 1;
            ll.current.should.equal(2);
        });

        it("#goto",function(){
            var ll = new $.LinkedList([-1,0,1,2,3,4,5,6,7]);
            ll.goto(2);
            ll.current.should.equal(2);
            ll.currentIndex.should.equal(3);
            ll.next();
            ll.current.should.equal(3);
            ll.goto(2,-1);
            ll.current.should.equal(1);
            ll.goto(2,2);
            ll.current.should.equal(4);
            ll.goto(2,-4);
            should.not.exist(ll.currentItem);
            ll.goto(2,10);
            should.not.exist(ll.currentItem);
        });

         it("#gotoIndex",function(){
            var ll = new $.LinkedList([7,6,5,4,3,2,1]);
            ll.gotoIndex(2);
            ll.current.should.equal(5);
            ll.next();
            ll.current.should.equal(4);
            ll.gotoIndex(2,-1);
            ll.current.should.equal(6);
            ll.gotoIndex(2,2);
            ll.current.should.equal(3);
            ll.gotoIndex(2,-3);
            should.not.exist(ll.currentItem);
            ll.gotoIndex(2,10);
            should.not.exist(ll.currentItem);
        });

         it("can be instantiated over other enumeration",function(){
           var ll = new $.LinkedList([1,2,3]);
            ll.any().should.equal(true);
            ll.count().should.equal(3);
            ll.toArray().should.eql([1,2,3]);

        });
        it("can be keyed",function(){

            var ll = new $.LinkedList([1,2,3],{onKey:"*2"});
            ll.onKey(2).should.equal(4);
            ll.find(2).value.should.equal(1);
            ll.find(4).value.should.equal(2);
            ll.find(6).value.should.equal(3);

        });
        it("can merge",function(){
            var ll = new $.LinkedList([{k:"c1",x:2},{k:"c2", x:3}],{onKey:".k"});
            ll.merge([{k:"c1",x:2},{k:"c3",x:4}]).should.eql({ removed: [], created: [ { k: 'c3', x: 4 } ], updated: [] });
            ll.toArray().should.eql([ { k: 'c1', x: 2 }, { k: 'c2', x: 3 }, { k: 'c3', x: 4 } ]);
            ll.merge([ { k: 'c1', x: 3 },  { k: 'c3', x: 5 } ],{replace:true}).should.eql({ removed: [],
                created: [],
                updated: [ { k: 'c1', x: 3 }, { k: 'c3', x: 5 } ] });
            ll.toArray().should.eql([ { k: 'c1', x: 3 }, { k: 'c2', x: 3 }, { k: 'c3', x: 5 } ]);
            ll.merge([ { k: 'c1', x: 4 },  { k: 'c3', x: 6 } ],{replace:false}).should.eql( { removed: [], created: [], updated: [] } );
            ll.toArray().should.eql([ { k: 'c1', x: 3 }, { k: 'c2', x: 3 }, { k: 'c3', x: 5 } ]);
            ll.merge([ { k: 'c1', x: 4 },  { k: 'c3', x: 6 } ],{replace:true,remove:true}).should.eql({ removed: [ { k: 'c2', x: 3 } ],
                created: [],
                updated: [ { k: 'c1', x: 4 }, { k: 'c3', x: 6 } ] } );
            ll.toArray().should.eql( [ { k: 'c1', x: 4 }, { k: 'c3', x: 6 } ] );
        });
        it("can merge default settings",function(){
            var ll = new $.LinkedList([{k:"c1",x:2},{k:"c2", x:3}],{onKey:".k",replaceOnMerge:true,removeOnMerge:true});

            ll.merge([{k:"c1",x:2},{k:"c3",x:4}],{replace:false,remove:false}).should.eql({ removed: [], created: [ { k: 'c3', x: 4 } ], updated: [] });
            ll.toArray().should.eql([ { k: 'c1', x: 2 }, { k: 'c2', x: 3 }, { k: 'c3', x: 4 } ]);
            ll.merge([ { k: 'c1', x: 3 },  { k: 'c3', x: 5 } ],{replace:true,remove:false}).should.eql({ removed: [],
                created: [],
                updated: [ { k: 'c1', x: 3 }, { k: 'c3', x: 5 } ] });
            ll.toArray().should.eql([ { k: 'c1', x: 3 }, { k: 'c2', x: 3 }, { k: 'c3', x: 5 } ]);
            ll.merge([ { k: 'c1', x: 4 },  { k: 'c3', x: 6 } ],{replace:false,remove:false}).should.eql( { removed: [], created: [], updated: [] } );
            ll.toArray().should.eql([ { k: 'c1', x: 3 }, { k: 'c2', x: 3 }, { k: 'c3', x: 5 } ]);
            ll.merge([ { k: 'c1', x: 4 },  { k: 'c3', x: 6 } ]).should.eql({ removed: [ { k: 'c2', x: 3 } ],
                created: [],
                updated: [ { k: 'c1', x: 4 }, { k: 'c3', x: 6 } ] } );
            ll.toArray().should.eql( [ { k: 'c1', x: 4 }, { k: 'c3', x: 6 } ] );
        });
        it("#onSetup",function(){
            var x = 0;
            var ll = new $.LinkedList([1,2],{onSetup:function(){x=x+1}});
            ll.next();
            ll.next();
            x.should.equal(1);
        });
        it("#onBeforeNext",function(){
            var x = 0;
            var ll = new $.LinkedList([1,2],{onBeforeNext:function(){x=x+1}});
            ll.next();
            ll.next();
            x.should.equal(2);
        });
        it("#onFetch",function(){
            var res = [];
            var ll = new $.LinkedList([1,2,3,4],{onFetch:function(_){res.push(_)}});
            ll.next(">1");
            ll.next(">3");
            res.should.eql([2,4]);
        });
        it("#onItemTrigger",function(){
            var res = [];
            var val = function(_){
                if(null==_)return "";
                return _.value;
            }
            var ll = new $.LinkedList(["aa","bb"],{
                replaceOnMerge :true,
                removeOnMerge:true,
                onItemTrigger:function(op,n,o){
                    res.push(op+":"+ val(n) +":"+ val(o));
                }
            });
            ll.merge(["aa","cc"]);
            res.should.eql(["insert:aa:","insert:bb:","delete::bb","update:aa:aa","insert:cc:"]);
        });
    });

    describe("Stack",function(){
       it("creates collection in back order",function(){
           var stack = new $.Stack([1,2,3]);
           stack.toArray().should.eql([3,2,1]);
       }) ;
        it("#pop",function(){
            var stack = new $.Stack([1,2,3]);
            stack.pop().should.equal(3);
            stack.toArray().should.eql([2,1]);
            stack.pop().should.equal(2);
            stack.toArray().should.eql([1]);
            stack.pop().should.equal(1);
            stack.toArray().should.eql([]);
            should.not.exist(stack.pop());
        });
        it("#push",function(){
            var stack = new $.Stack([1,2,3]);
            stack.pop().should.equal(3);
            stack.toArray().should.eql([2,1]);
            stack.push(4);
            stack.toArray().should.eql([4,2,1]);
            stack.pop().should.equal(4);
        });
    });

    describe("Queue",function(){
        it("#pop",function(){
            var queue = new $.Queue([1,2,3]);
            queue.pop().should.equal(1);
            queue.toArray().should.eql([2,3]);
            queue.pop().should.equal(2);
            queue.toArray().should.eql([3]);
            queue.pop().should.equal(3);
            queue.toArray().should.eql([]);
            should.not.exist(queue.pop());
        });
        it("#push",function(){
            var queue = new $.Queue([1,2,3]);
            queue.pop().should.equal(1);
            queue.toArray().should.eql([2,3]);
            queue.push(4);
            queue.toArray().should.eql([2,3,4]);
            queue.pop().should.equal(2);
            queue.pop().should.equal(3);
            queue.pop().should.equal(4);
        });
    });

    describe("CycleList",function(){
       it("repeat it's elements cycle by cycle",function(){
           var cl = new $.CycleList([1,2,3]);
           cl.pop().should.equal(1);
           cl.pop().should.equal(2);
           cl.pop().should.equal(3);
           cl.pop().should.equal(1);
           cl.pop().should.equal(2);
           cl.pop().should.equal(3);
       });
        it("we can append elements to queue",function(){
            var cl = new $.CycleList([1,2,3]);
            cl.pop().should.equal(1);
            cl.pop().should.equal(2);
            cl.insertAfter(4);
            cl.pop().should.equal(4);
            cl.pop().should.equal(3);
            cl.pop().should.equal(1);
            cl.pop().should.equal(2);
            cl.pop().should.equal(4);
            cl.pop().should.equal(3);
        });
        it("#goto(key,offset)",function(){
            var cl = new $.CycleList([1,2,3,4,5,6]);
            cl.goto(4,-2);
            cl.pop().should.equal(3);
            cl.pop().should.equal(4);
            cl.pop().should.equal(5);
            cl.pop().should.equal(6);
            cl.pop().should.equal(1);
            cl.goto(5,3);
            cl.pop().should.equal(3);
            cl.pop().should.equal(4);
            cl.pop().should.equal(5);
            cl.pop().should.equal(6);
            cl.pop().should.equal(1);
        })
    });

    var $ = null;
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
            requirejs(["./lib/chai","../the-collections-linked","../the-expression","../the-collections-linq"], function($should,$the){
                should = $should.Should();
                $ = $the.collections;
                done();
            });
        }catch(e){
            console.log(e);
            done();
        }
    });







});
})(describe,it,before);